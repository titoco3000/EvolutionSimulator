/*
Reage aos genes para guiar as criaturas até seus destinos e 
fazer as ações:
-comer
-reproduzir
-fugir
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Criatura : MonoBehaviour
{
    public DNA Genes;


    public int FoodAmount = 0;
    public int Generation = 1;


    private string TargetType;
    private Transform TargetTransform;
    private Vector3 TargetPos;

    //Referências
    public LayerMask criaturaLayer;
    public LayerMask comidaLayer;
    public LayerMask inimigoLayer;

    //Configurações
    public int InitialFood = 500;
    public int MaxFoodAmount = 800;
    public float descansoPosBebe = 10f;
    public float tempoDeInfancia = 12f;
    public float QualidadeDaVisao = 1f;
    public int CustoDeAcasalamento = 2;
    private int MinFoodParaAcasalar = 60;
    public float maxTempoDeNamoro = 10f;
    private int VagarDistancia = 10;
    private float distanciaMinimaParaInteragir = 1.2f;
    public float TempoDeVida = 40f;
    public int custoEnergeticoMultiplyer = 1;
    private float AttentionRadius = 10f;

    public string nomeDoPrefab = "Criatura Filhote";
    public string nomeDaCriatura = "Criatura";

    //prefabs
    private GameObject prefabDoFilho;

    //Referencia a componentes
    private NavMeshAgent agent;
    private ParticleSystem LoveParticles;
    [HideInInspector] public Manager manager;


    //para a reprodução
    [HideInInspector]public bool IndisponivelParaNamoro = true;
    [HideInInspector] public bool Apaixonado = false;
    [HideInInspector] public bool Passivo = false;

    [HideInInspector] public float BirthTime;

    public virtual void Start()
    {
        manager = FindObjectOfType<Manager>();

        //Ativa quando há geração espontânea
        if (Genes.Velocidade == 0)
        {
            
                Genes = (manager == null)? new DNA(0,0) : new DNA(manager.MutationChance, manager.MutationRate);
            

            //duas opções de inicialização dos genes:
            //Genes.SetRandomGenes();
            Genes.SetDefaultGenes();
            
            FoodAmount = InitialFood; //valor inicial 
            TornarseAdulto();
        }
        //ativa qnd NÃO é a primeira geração
        else
        {
            //Tornar-se adulto
            Invoke("TornarseAdulto", tempoDeInfancia);
        }
        

        //referencias
        prefabDoFilho = Resources.Load(nomeDoPrefab) as GameObject;
        agent = GetComponent<NavMeshAgent>();
        LoveParticles = GetComponentInChildren<ParticleSystem>();

        //configurações iniciais
        SetColor(Genes.Red, Genes.Green, Genes.Blue);
        agent.speed = agent.speed * (float)Genes.Velocidade / 100f;
        BirthTime = Time.time;

        //primeiro pensamento
        Invoke("Pensar", 1 + ((100 - Genes.Inteligencia) / 10));

        //morte por velhice
        //int saudeMultiplier = Genes.Saude / 10 >= 1 ? Genes.Saude / 10 : 1;
        Invoke("MorrerDeVelhice", TempoDeVida * Genes.Saude );
    }

    private void Update()
    {

        //Procura inimigo no seu entorno
        Transform inimigo = null; //SearchFor("Inimigo", AttentionRadius);
        if(inimigo != null)
        {
            Debug.Log("Achei perigo");
            //fugir
            DeterminarDestino((transform.position - inimigo.position).normalized * AttentionRadius);
        }
        else
        {
            if (TargetTransform != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, TargetTransform.position);
            if ( distanceToTarget <= distanciaMinimaParaInteragir)
            {
                //para em frente ao target
                DeterminarDestino(transform.position);
                //interage com ele
                if (TargetType == "Comida")
                {
                    TargetTransform.GetComponent<Comida>().Comer(this);
                }
                else if (TargetType == "Parceiro" && !Passivo && !IndisponivelParaNamoro)
                {
                    TerFilho(TargetTransform.GetComponent<Criatura>());
                }
            }
            else
            {
                if(TargetType == "Parceiro"&&distanceToTarget <= distanciaMinimaParaInteragir * 2 && Passivo)
                {
                    //se é o passivo, parar a uma distancia maior
                     DeterminarDestino(transform.position);
                }
                else
                {
                    DeterminarDestino(TargetTransform.position);
                }
            }
        }
            else {
            //garante que não fique apaixonado depois do fim do relacionamento
            if (Apaixonado)
            {
                Apaixonado = false;
                Passivo = false;
            }
        }

        }

    }

    //Chamado em intervalos que dependem da sua inteligencia
    private void Pensar()
    {

        //o personagem está "ocupado", adiar
        if (Apaixonado)
        {
               Invoke("Pensar", 3f);
        }

        else
        {
            //cobrar o preço energético
            GastarComida(Genes.GetCost() * custoEnergeticoMultiplyer);
            //FoodAmount -= Genes.GetCost();

            TargetTransform = null;
            TargetType = "";


            //se não estiver DESESPERADO por comida, procura predador
            if (FoodAmount > Genes.GetCost() * custoEnergeticoMultiplyer)
            {
                Transform inimigo = SearchFor("Inimigo", Genes.Visao * QualidadeDaVisao);
                if (inimigo != null)
                {
                    //fugir
                    TargetPos = (transform.position - inimigo.position).normalized * Genes.Visao * QualidadeDaVisao / 2;
                }
            }

            //se tiver comida o suficiente, procurar parceiro
            if (PodeReproduzir() && TargetType == "")
            {
                Transform parceiroT = SearchFor("Parceiro", Genes.Visao * QualidadeDaVisao *2);
                if(parceiroT!= null)
                {
                    Criatura parceiro = parceiroT.GetComponent<Criatura>();
                    //enviar msg para o parceiro
                    if (parceiro.QuerCasarComigo(transform))
                    {
                        //o parceiro aceitou!
                        Apaixonado = true;
                        //andar até ele
                        TargetTransform = parceiroT;
                        TargetType = "Parceiro";
                        //emitir corações
                        LoveParticles.Play();
                    }
                }
            }
            
            //se não tiver comida suficiente ou não achar parceiro, procurar comida
            if ( FoodAmount < MaxFoodAmount && !Apaixonado && TargetType == "")
            {
                Transform comidaT = SearchFor("Comida", Genes.Visao * QualidadeDaVisao);
                if(comidaT != null)
                {
                    //achou comida!
                    Comida comida = comidaT.GetComponent<Comida>();
                    if(comida.Comido == false)
                    {
                        TargetTransform = comidaT;
                        TargetType = "Comida";

                    }
                }
            }

            //se não conseguir comida nem namoro, vagar
            if(TargetTransform == null)
            {
                if(TargetType == "Inimigo")
                {
                    DeterminarDestino(TargetPos);
                }
                else
                {
                    TargetType = "";
                    TargetPos = new Vector3(
                        transform.position.x + Random.Range(-VagarDistancia, VagarDistancia),
                        transform.position.y,
                        transform.position.z + Random.Range(-VagarDistancia, VagarDistancia));
                    DeterminarDestino(TargetPos);
                }
            }
            else 
                DeterminarDestino(TargetTransform.position);


            

            //pensar novamente depois
            //"2" é o tempo mínimo de descanso
            Invoke("Pensar", 2 + ((100 - Genes.Inteligencia) / 10));
        }


    }

    //Determina a cor de acordo com os genes
    public void SetColor(int r, int g, int b)
    {
        Color color = GetComponent<Renderer>().material.color;
        float divider = 255;
        color.r = r / divider;
        color.g = g / divider;
        color.b = b / divider;
        GetComponent<Renderer>().material.color = color;
    }

    public virtual void GastarComida(int amount)
    {
        FoodAmount -= amount;
        if(FoodAmount <= 0)
        {
            //Debug.Log(transform.name + " morreu de fome");
            Morrer();
        }
    }


    public virtual void MorrerDeVelhice()
    {
        //Debug.Log("morri de velhice");
        Morrer();
    }
    void Morrer()
    {
        Destroy(gameObject);
        this.enabled = false;
    }

    //retorna parceiro ou comida válidos
    private Transform SearchFor(string procura, float radius = 10f)
    {
        LayerMask procuraLayer = procura == "Comida" ? comidaLayer : criaturaLayer;
        if (procura == "Inimigo")
            procuraLayer = inimigoLayer;

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius * QualidadeDaVisao, procuraLayer);

        if(procura == "Comida" && !PossoCaçar())
        {
            return null;
        }

        //filtra os disponíveis
        List<Transform> itemsQuePodem = new List<Transform>();
        foreach (var item in colliders)
        {
            if(procura == "Comida")
            {
                if(item.GetComponent<Comida>().Comido == false)
                {
                    itemsQuePodem.Add(item.transform);
                }
            }
            else if(procura == "Parceiro")
            {
                if(item.GetComponent<Criatura>().PodeReproduzir() && item.GetComponent<Criatura>().enabled && item.transform != transform)
                {
                    itemsQuePodem.Add(item.transform);
                }
            }
            else if(procura == "Inimigo")
            {
                itemsQuePodem.Add(item.transform);
            }
        }
        if (itemsQuePodem.Count == 0)
            return null;
        Transform closestTransform = itemsQuePodem[0].transform;
        float closestDistance = Vector3.Distance(itemsQuePodem[0].position, transform.position);
        for (int i = 1; i < itemsQuePodem.Count; i++)
        {

            float Distance = Vector3.Distance(itemsQuePodem[i].position, transform.position);
            if (Distance < closestDistance)
            {
                closestTransform = itemsQuePodem[i];
                closestDistance = Distance;
            }
        }
        return closestTransform;

    }

    public bool QuerCasarComigo(Transform noivo)
    {
        if (PodeReproduzir())
        {
            TargetType = "Parceiro";
            TargetTransform = noivo;
            Apaixonado = true;
            Passivo = true;
            //emitir corações
            LoveParticles.Play();
            return true;
        }
        return false;
    }

    public virtual bool PodeReproduzir()
    {
        bool retorno = !Apaixonado && !IndisponivelParaNamoro && (100 - Genes.VontadeDeAcasalamento) * CustoDeAcasalamento < FoodAmount + Genes.GetCost()* custoEnergeticoMultiplyer;
        return retorno;
    }

    public virtual bool PossoCaçar()
    {
        return true;
    }
    //Para gerar o filho
    private void TerFilho(Criatura outro)
    {
        //deduzir comida 
        GastarComida( MinFoodParaAcasalar+ (100 - Genes.VontadeDeAcasalamento) * CustoDeAcasalamento);
        outro.GastarComida(MinFoodParaAcasalar +(100 - outro.Genes.VontadeDeAcasalamento) * CustoDeAcasalamento);

        
        DNA dnaDoFilho = new DNA(outro.Genes.ToArray(), Genes.ToArray(), manager.MutationChance, manager.MutationRate);
        Criatura filho = Instantiate(prefabDoFilho, transform.position, transform.rotation, transform.parent).GetComponent<Criatura>();
        filho.transform.localScale = Vector3.one * 0.5f;
        filho.Genes = dnaDoFilho;
        filho.Generation += Generation > outro.Generation ? Generation : outro.Generation;
        filho.FoodAmount = 2*MinFoodParaAcasalar +(100 - Genes.VontadeDeAcasalamento) * CustoDeAcasalamento + (100 - outro.Genes.VontadeDeAcasalamento) * CustoDeAcasalamento;

        //tira os pais do modo apaixonado
        Apaixonado = false;
        Passivo = false;

        outro.Apaixonado = false;
        outro.Passivo = false;

        //coloca o casal pra descansar
        IndisponivelParaNamoro = true;
        Invoke("VoltarAoRomance", descansoPosBebe);

        outro.IndisponivelParaNamoro = true;
        outro.Invoke("VoltarAoRomance", descansoPosBebe);

    }

    private void VoltarAoRomance()
    {
        IndisponivelParaNamoro = false;
    }

    private void TornarseAdulto()
    {
        //aumentar de tamanho
        StartCoroutine(Crescer(1));
        //disponibilizar para namoro
        Invoke("VoltarAoRomance", 6f);
    }
    IEnumerator Crescer(float novoTamanho)
    {
        transform.name = nomeDaCriatura;
        float growRate = 0.5f;
        while(transform.localScale.x < novoTamanho)
        {
             Vector3 newScale = new Vector3(
                transform.localScale.x + growRate * Time.deltaTime,
                transform.localScale.y + growRate * Time.deltaTime,
                transform.localScale.z + growRate * Time.deltaTime
                );
            transform.localScale = newScale;
            yield return new WaitForEndOfFrame();
        }
        
         transform.localScale = Vector3.one * novoTamanho;
        
    }

    void DeterminarDestino(Vector3 destination)
    {
        if(agent.isOnNavMesh)
            agent.SetDestination(destination);
    }



    //Debug
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, Genes.Visao * QualidadeDaVisao);
        
    }



}
