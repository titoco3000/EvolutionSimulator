using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class SaveHandler : MonoBehaviour
{
    public Transform ParentCriaturas;
    public Transform ParentPredadores;

    private float MeasureDelay = 7.5f;
    public int MaxGraphDots = 41;

    private string Path_historico;
    private string Path_historicoBest;

    private Manager manager;
    private float lastUpdatedTime = 0;


    void Start()
    {
        Path_historico = Application.dataPath + "/historico.json";
        Path_historicoBest = Application.dataPath + "/historicoBest.json";

        manager = FindObjectOfType<Manager>();

        Invoke("IniciarJson", .1f);
        Invoke("UpdateHistorico", MeasureDelay);
    }

    void OnApplicationQuit()
    {
        SetBestHistorico();
    }

    public void SetBestHistorico()
    {
        Population bestHistorico = JsonUtility.FromJson<Population>(File.ReadAllText(Path_historicoBest));
        Population currentHistorico = JsonUtility.FromJson<Population>(File.ReadAllText(Path_historico));
        if (TamanhoDoHistorico(bestHistorico) * bestHistorico.EscalaDeTempo < TamanhoDoHistorico(currentHistorico) * currentHistorico.EscalaDeTempo)
        {
            SetJson(currentHistorico, Path_historicoBest);
        }
    }

    void IniciarJson()
    {
        Population populacao = new Population();
        List<Population.Generation> Geracoes = new List<Population.Generation>();

        Geracoes.Add(GetCurrentGenerationMedians(ParentCriaturas));
        populacao.listaCriaturas = Geracoes;

        Geracoes = new List<Population.Generation>();
        Geracoes.Add(GetCurrentGenerationMedians(ParentPredadores));
        populacao.listaPredadores = Geracoes;

        populacao.EscalaDeTempo = MeasureDelay;
        SetJson(populacao, Path_historico);
        //garante q os arquivo Best existe
        if (!File.Exists(Path_historicoBest))
        {
            SetJson(populacao, Path_historicoBest);
        }
    }


    void SetJson(Population populacao, string path)
    {
        string json = JsonUtility.ToJson(populacao);
        File.WriteAllText(path, json);

    }
    void AddToJson(Population.Generation GeracaoCriaturas, Population.Generation GeracaoPredadores )
    {
        Population populacao = JsonUtility.FromJson<Population>(File.ReadAllText(Path_historico));
        populacao.listaCriaturas.Add(GeracaoCriaturas);
        populacao.listaPredadores.Add(GeracaoPredadores);

        if(TamanhoDoHistorico(populacao) >= MaxGraphDots)
        {
            //Simplificar o histórico
            MeasureDelay *= 2;
            populacao = SimplifyInHalf(populacao);
            populacao.EscalaDeTempo = MeasureDelay;
        }

        File.WriteAllText(Path_historico, JsonUtility.ToJson(populacao));
        
    }

    public Population LoadPopulation(int id = 0)
    {
        if (id == 0)
        {
            return JsonUtility.FromJson<Population>(File.ReadAllText(Path_historico));
        }
        return JsonUtility.FromJson<Population>(File.ReadAllText(Path_historicoBest));
    }

    public Population SimplifyInHalf(Population original)
    {
        if (original.listaCriaturas.Count % 2 == 0)
        {
            Debug.LogError("even number");
            return original;
        }
        else
        {
            Population retorno = new Population();

            retorno.listaCriaturas = new List<Population.Generation>();
            retorno.listaCriaturas.Add(original.listaCriaturas[0]);
            for (int i = 1; i + 1 < original.listaCriaturas.Count; i += 2)
            {
                retorno.listaCriaturas.Add(GenerationMedium(original.listaCriaturas[i], original.listaCriaturas[i + 1]));
            }

            retorno.listaPredadores = new List<Population.Generation>();
            retorno.listaPredadores.Add(original.listaPredadores[0]);
            for (int i = 1; i + 1 < original.listaPredadores.Count; i += 2)
            {
                retorno.listaPredadores.Add(GenerationMedium(original.listaPredadores[i], original.listaPredadores[i + 1]));
            }
            Debug.Log("nova lista é " + ((float)retorno.listaCriaturas.Count-1) / ((float)original.listaCriaturas.Count-1) + " da antiga");
            
            return retorno;
        }

    }

    public void UpdateHistorico()
    {
        
            Population.Generation currentGenCriaturas = GetCurrentGenerationMedians(ParentCriaturas);
            Population.Generation currentGenPredadores = GetCurrentGenerationMedians(ParentPredadores);
            lastUpdatedTime = lastUpdatedTime + MeasureDelay;
            AddToJson(currentGenCriaturas, currentGenPredadores);

            if( manager.DeveReiniciar(currentGenCriaturas.Quantidade, currentGenPredadores.Quantidade))
            {
                SetBestHistorico();
            }
            else{
                Invoke("UpdateHistorico", MeasureDelay);
            }

    }
    
    public Population.Generation GetCurrentGenerationMedians(Transform Parent)
    {
        Criatura[] criaturas = Parent.GetComponentsInChildren<Criatura>();
        int velocidade = 0, visao = 0, vontadeDeAcasalamento = 0, inteligencia = 0, saude = 0, R = 0, G = 0,B=0 ;
        int quantidade = criaturas.Length;
        for (int i = 0; i < quantidade; i++)
        {
            velocidade += criaturas[i].Genes.Velocidade;
            visao += criaturas[i].Genes.Visao;
            vontadeDeAcasalamento += criaturas[i].Genes.VontadeDeAcasalamento;
            inteligencia += criaturas[i].Genes.Inteligencia;
            saude += criaturas[i].Genes.Saude;
            R += criaturas[i].Genes.Red;
            G += criaturas[i].Genes.Green;
            B += criaturas[i].Genes.Blue;
        }

        Population.Generation geracao = new Population.Generation();
        if(quantidade != 0)
        {
            geracao.Velocidade = velocidade / quantidade;
            geracao.Visao = visao / quantidade;
            geracao.VontadeDeAcasalamento = vontadeDeAcasalamento / quantidade;
            geracao.Inteligencia = inteligencia / quantidade;
            geracao.Saude = saude / quantidade;
            geracao.Quantidade = quantidade;
            geracao.R = R / quantidade;
            geracao.G = G / quantidade;
            geracao.B = B / quantidade;

        }
        else
        {
            geracao.Velocidade = 0;
            geracao.Visao = 0;
            geracao.VontadeDeAcasalamento = 0;
            geracao.Inteligencia = 0;
            geracao.Saude =0;
            geracao.Quantidade = 0;
            geracao.R =  0;
            geracao.G = 0;
            geracao.B =  0;
        }
        return geracao;
    }

    public Population.Generation GenerationMedium(Population.Generation a, Population.Generation b)
    {
        Population.Generation retorno = new Population.Generation();
        retorno.Quantidade = (a.Quantidade + b.Quantidade) / 2;
        retorno.Velocidade = (a.Velocidade + b.Velocidade) / 2;
        retorno.VontadeDeAcasalamento = (a.VontadeDeAcasalamento + b.VontadeDeAcasalamento) / 2;
        retorno.Inteligencia = (a.Inteligencia + b.Inteligencia) / 2;
        retorno.Visao = (a.Visao + b.Visao) / 2;
        retorno.Saude = (a.Saude + b.Saude) / 2;
        retorno.R = (a.R + b.R) / 2;
        retorno.G = (a.G + b.G) / 2;
        retorno.B = (a.B + b.B) / 2;

        return retorno;
    }

    int TamanhoDoHistorico(Population pop)
    {
        return pop.listaCriaturas.Count;
    }

   
}
