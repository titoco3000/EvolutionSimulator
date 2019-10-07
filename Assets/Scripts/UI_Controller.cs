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

    void Start()
    {
        
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
            ColocarValoresNoGrafico(Nomes[currentTipo], Populacao.GetStat(Tipos[currentTipo]), Populacao.EscalaDeTempo);

        }
    }


    void ColocarValoresNoGrafico(string title, List<float> valores, int escala)
    {
        if(title != "Quantidade")
        {
            Graph.SetGraph(title, valores, escala, 100);
        }
        else
            Graph.SetGraph(title, valores, escala);
    }
    public void OnRightButtomPress()
    {
        currentTipo++;
        if (currentTipo > Tipos.Length - 1)
            currentTipo = 0;
        ColocarValoresNoGrafico(Nomes[currentTipo], Populacao.GetStat(Tipos[currentTipo]), Populacao.EscalaDeTempo);

    }

    public void OnLeftButtomPress()
    {
        currentTipo--;
        if (currentTipo < 0)
            currentTipo = Tipos.Length - 1;
        ColocarValoresNoGrafico(Nomes[currentTipo], Populacao.GetStat(Tipos[currentTipo]), Populacao.EscalaDeTempo);

    }
    public void TrocarFonteDeValores()
    {
        //inverte a fonte
        if (currentFonte == 0)
            currentFonte = 1;
        else
            currentFonte = 0;

        Populacao = saveHandler.LoadPopulation(currentFonte);
        ColocarValoresNoGrafico(Nomes[currentTipo], Populacao.GetStat(Tipos[currentTipo]), Populacao.EscalaDeTempo);
    }

}
