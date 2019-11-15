using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Controller : MonoBehaviour
{
    public Grafico Graph;


    public Image graphButtomImage;
    public Sprite ShownGraphSprite;
    public Sprite UnshownGraphSprite;
    public SaveHandler saveHandler;

    public string[] Tipos;
    public string[] Nomes;


    private Population Populacao;
    private int currentTipo = 0;
    private int currentFonte = 0;
    private bool GraphShown = false;

    private CameraMovement movement;


    //Variaveis do infografico
    public GameObject Infografico;
    public Text GenerationText;
    public Text GenesText;
    public Text ComidaText;
    public Text IdadeText;

    void Start()
    {
        movement = Camera.main.gameObject.GetComponent<CameraMovement>();
    }

    private void Update()
    {
        if (!GraphShown)
        {
            //se houver clique
            if (Input.GetButtonDown("Fire1"))
            {
                //enviar ray da posição do mouse
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit)){
                    Criatura criatura = hit.transform.GetComponent<Criatura>();
                    if (criatura)
                    {
                        //focar na criatura
                        movement.DefinirTarget(criatura.transform);

                        //colocar os valores do gene na UI
                        ColocarValoresNoInfografico(criatura);

                        //exibi-la
                        Infografico.SetActive(true);
                    }
                }


            }
            else if (Input.GetButtonUp("Fire1"))
            {
                movement.ResetTarget();
                Infografico.SetActive(false);
            }

        }
        else
            Infografico.SetActive(false);
    }
    
    

    public void OnGraphButtomPress()
    {
        //esconder
        if (GraphShown)
        {
            GraphShown = false;
            //muda o sprite do botao
            graphButtomImage.sprite = UnshownGraphSprite;
            //esconde o grafico
            Graph.gameObject.SetActive(false);
        }
        //revelar
        else
        {
            GraphShown = true;
            //muda o sprite do botao
            graphButtomImage.sprite = ShownGraphSprite;
            //Revela o gráfico
            Graph.gameObject.SetActive(true);
            //Carrega a informação
            Populacao = saveHandler.LoadPopulation(currentFonte);
            //Coloca alguma informação no gráfico
            ExibirGraficoDeTipo(Nomes[currentTipo]);
        }
    }


    void ColocarValoresNoGrafico(string title, List<float> valoresCriatura, List<float> valoresPredador, float escala)
    {
        if(title != "Quantidade")
        {
            Graph.SetGraph(title, valoresCriatura, valoresPredador, escala, 100);
        }
        else
            Graph.SetGraph(title, valoresCriatura, valoresPredador, escala);
    }

    void ColocarValoresNoInfografico(Criatura criatura)
    {
        GenerationText.text = criatura.Generation.ToString();
        ComidaText.text = criatura.FoodAmount.ToString();
        GenesText.text = criatura.Genes.Velocidade + "\n" + criatura.Genes.Visao + "\n" + criatura.Genes.VontadeDeAcasalamento + "\n" + criatura.Genes.Inteligencia + "\n" + criatura.Genes.Saude;
        StartCoroutine(KeepInfogrValuesUpdated(criatura));
    }

    IEnumerator KeepInfogrValuesUpdated(Criatura criatura)
    {
        while (Input.GetButton("Fire1"))
        {
            if(criatura == null)
            {
                Infografico.SetActive(false);
            }
            IdadeText.text = (int)(Time.time - criatura.BirthTime) + "/" + criatura.Genes.Saude * criatura.TempoDeVida ;
            ComidaText.text = criatura.FoodAmount.ToString();
            yield return new WaitForEndOfFrame();
        }
    }

    void ExibirGraficoDeTipo(string titulo)
    {
        if(titulo == "Cor")
        {
            List<float> RedCriatura = Populacao.GetStat(Populacao.listaCriaturas, "R");
            List<float> GreenCriatura = Populacao.GetStat(Populacao.listaCriaturas, "G");
            List<float> BlueCriatura = Populacao.GetStat(Populacao.listaCriaturas, "B");
          
            List<float> RedPredador = Populacao.GetStat(Populacao.listaPredadores, "R");
            List<float> GreenPredador = Populacao.GetStat(Populacao.listaPredadores, "G");
            List<float> BluePredador = Populacao.GetStat(Populacao.listaPredadores, "B");

            List<Color> coresCriatura = new List<Color>();
            List<Color> coresPredador = new List<Color>();

            for (int i = 0; i < RedCriatura.Count; i++)
            {
                //divide por 255 para fica na escala 0-1
                coresCriatura.Add(new Color(RedCriatura[i]/255f, GreenCriatura[i]/255f, BlueCriatura[i] / 255f));
                coresPredador.Add(new Color(RedPredador[i] / 255f, GreenPredador[i] / 255f, BluePredador[i] / 255f));
            }
            Graph.SetGraph(titulo, coresCriatura, coresPredador, Populacao.EscalaDeTempo);
            Graph.DisplayGraph("Colorido");
            
        }
        else
        {
            //exibir gráfico numérico
            Graph.DisplayGraph("Numerico");
            //Colocar valores nele
            ColocarValoresNoGrafico(titulo, Populacao.GetStat(Populacao.listaCriaturas, Tipos[currentTipo]), Populacao.GetStat(Populacao.listaPredadores, Tipos[currentTipo]), Populacao.EscalaDeTempo);
        }
    }

    #region Botoes
    public void OnRightButtomPress()
    {
        currentTipo++;
        if (currentTipo > Tipos.Length - 1)
            currentTipo = 0;
        ExibirGraficoDeTipo(Nomes[currentTipo]);

    }
    public void OnLeftButtomPress()
    {
        currentTipo--;
        if (currentTipo < 0)
            currentTipo = Tipos.Length - 1;
        ExibirGraficoDeTipo(Nomes[currentTipo]);

    }
    public void TrocarFonteDeValores()
    {
        //inverte a fonte
        if (currentFonte == 0)
            currentFonte = 1;
        else
            currentFonte = 0;

        Populacao = saveHandler.LoadPopulation(currentFonte);
        ExibirGraficoDeTipo(Nomes[currentTipo]);
    }
    #endregion
}
