﻿/*
Reage aos genes e ao momento do dia para guiar as criaturas até seus destinos e 
fazer as ações:
-comer
-reproduzir
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Criatura : MonoBehaviour
{
    public DNA Genes;

    //para a locomoção
    private NavMeshAgent agent;

    public int FoodAmount = 0;
    public int Generation = 1;

    public bool IndisponivelParaNamoro = true;
    public bool Apaixonado = false;
    public bool Passivo = false;

    private int VagarDistancia = 10;
    private float distanciaMinimaParaInteragir = 1.2f;

    private string TargetType;
    Transform TargetTransform;

    //parâmetros para a visão
    public LayerMask criaturaLayer;
    public LayerMask comidaLayer;

    //Configurações
    public float descansoPosBebe = 10f;
    public float tempoDeInfancia = 12f;
    public float QualidadeDaVisao = 0.2f;
    private int CustoDeAcasalamento = 10;
    public float maxTempoDeNamoro = 10f;
    //prefabs
    private GameObject prefabDoFilho;


    private void Start()
    {
        //Ativa quando há geração espontânea
        if(Genes.Velocidade == 0)
        {
            Genes = new DNA();
            Genes.SetRandomGenes();
            //Genes.SetDefaultGenes();
            FoodAmount = 2000; //valor inicial padrão
            //Invoke("VoltarAoRomance", 3f);
            TornarseAdulto();
        }
        //ativa qnd NÃO é a primeira geração
        else
        {
            //Tornar-se adulto
            Invoke("TornarseAdulto", tempoDeInfancia);
        }
        
        //referencia a scripts
        agent = GetComponent<NavMeshAgent>();

        //referencia a resources
        prefabDoFilho = Resources.Load("Criatura Filhote") as GameObject;

        //configurações iniciais
        SetColor(Genes.Red, Genes.Green, Genes.Blue);
        agent.speed = agent.speed * (float)Genes.Velocidade / 100f;

        //primeiro pensamento
        Invoke("Pensar", 2f);
    }

    private void Update()
    {
        if (TargetTransform != null)
        {

            if (Vector3.Distance(transform.position, TargetTransform.position) <= distanciaMinimaParaInteragir)
            {
                //para em frente ao target
                agent.SetDestination(transform.position);
                //encara ele
                if (TargetType == "Comida")
                {
                    TargetTransform.GetComponent<Comida>().Comer(this);
                }
                else if (TargetType == "Parceiro" && !Passivo && !IndisponivelParaNamoro)
                {
                    TerFilho(TargetTransform.GetComponent<Criatura>());
                }
            }
            else if(TargetType == "Parceiro")
            {
                //nesse caso é feito no update pq o alvo se mexe
                agent.SetDestination(TargetTransform.position);
            }
        }
        else {
            if (Apaixonado)
            {
                Apaixonado = false;
                Passivo = false;
                Debug.Log("parando de vagabundar");
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
            GastarComida(Genes.GetCost());
            FoodAmount -= Genes.GetCost();

            TargetTransform = null;
            //se tiver comida o suficiente, procurar parceiro
            if(PodeReproduzir())
            {
                Transform parceiroT = SearchFor("Parceiro", Genes.Visao);
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
                    }
                }
            }
            //se não tiver comida suficiente ou não achar parceiro, procurar comida
            if (!Apaixonado)
            {
                Transform comidaT = SearchFor("Comida", Genes.Visao);
                if(comidaT != null)
                {
                    //achou comida!
                    Comida comida = comidaT.GetComponent<Comida>();
                    if(comida.QuemEstaComendo == null)
                    {
                        comida.QuemEstaComendo = transform;
                        TargetTransform = comidaT;
                        TargetType = "Comida";

                    }
                }
            }

            //se não conseguir comida nem namoro, vagar
            if(TargetTransform == null && agent.isOnNavMesh)
            {
                TargetType = "";
                Vector3 targetPosition = new Vector3(
                    transform.position.x + Random.Range(-VagarDistancia, VagarDistancia),
                    transform.position.y,
                    transform.position.z + Random.Range(-VagarDistancia, VagarDistancia));
                agent.SetDestination(targetPosition);
            }
            else if(agent.isOnNavMesh)
                //se consegui, andar até ele
                agent.SetDestination(TargetTransform.position);


            

            //pensar novamente depois
            //"2" é o tempo mínimo de descanso
            Invoke("Pensar", 2 + ((100 - Genes.Inteligencia) / 10));
        }


    }

    //Determina a cor de acordo com os genes
    private void SetColor(int r, int g, int b)
    {
        Color color = GetComponent<Renderer>().material.color;
        float divider = 255;
        color.r = r / divider;
        color.g = g / divider;
        color.b = b / divider;
        GetComponent<Renderer>().material.color = color;
    }

    public void GastarComida(int amount)
    {
        FoodAmount -= amount;
        if(FoodAmount <= 0)
        {
            //Debug.Log(transform.name + " morreu de fome");
            Destroy(gameObject);
            this.enabled = false;
        }
    }

    //retorna parceiro ou comida válidos
    private Transform SearchFor(string procura, float radius = 10f)
    {
        LayerMask procuraLayer = procura == "Comida" ? comidaLayer : criaturaLayer;
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius * QualidadeDaVisao, procuraLayer);


        //filtra os disponíveis
        List<Transform> itemsQuePodem = new List<Transform>(); ;
        foreach (var item in colliders)
        {
            if(procura == "Comida")
            {
                if(item.GetComponent<Comida>().QuemEstaComendo != transform)
                {
                    itemsQuePodem.Add(item.transform);
                }
            }
            else
            {
                if(item.GetComponent<Criatura>().PodeReproduzir() && item.transform != transform)
                {
                    itemsQuePodem.Add(item.transform);
                }
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
            return true;
        }
        return false;
    }

    public bool PodeReproduzir()
    {
        bool retorno = !Apaixonado && !IndisponivelParaNamoro && Genes.VontadeDeAcasalamento * CustoDeAcasalamento < FoodAmount;
        return retorno;
    }

    //Para gerar o filho
    private void TerFilho(Criatura outro)
    {
        //deduzir comida 
        GastarComida(Genes.VontadeDeAcasalamento * CustoDeAcasalamento / 2);
        //FoodAmount -= Genes.VontadeDeAcasalamento / 2;
        outro.GastarComida(outro.Genes.VontadeDeAcasalamento * CustoDeAcasalamento / 2);
        //outro.FoodAmount -= outro.Genes.VontadeDeAcasalamento / 2;

        
        DNA dnaDoFilho = new DNA(outro.Genes.ToArray(), Genes.ToArray());
        Criatura filho = Instantiate(prefabDoFilho, transform.position, transform.rotation, transform.parent).GetComponent<Criatura>();
        filho.Genes = dnaDoFilho;
        filho.Generation += Generation > outro.Generation ? Generation : outro.Generation;
        filho.FoodAmount = Genes.VontadeDeAcasalamento * CustoDeAcasalamento / 2 + outro.Genes.VontadeDeAcasalamento * CustoDeAcasalamento / 2;

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
        transform.name = "Criatura";
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
        if(transform.localScale.x < novoTamanho)
        {
            transform.localScale = Vector3.one * novoTamanho;
        }
    }


    //Debug
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, Genes.Visao * QualidadeDaVisao);
    }

}
