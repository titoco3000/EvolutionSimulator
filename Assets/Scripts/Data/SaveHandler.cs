using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveHandler : MonoBehaviour
{
    public Transform Parent;
    public int MeasureDelay = 10;
    public int MaxGraphDots = 41;

    private string Path;
    private string PathOfBest;


    void Start()
    {
        Path = Application.dataPath + "/CurrentHistorico.json";
        PathOfBest = Application.dataPath + "/BestHistorico.json";
        Invoke("IniciarJson", .1f);
        Invoke("UpdateHistorico",MeasureDelay);
    }

    void OnApplicationQuit()
    {
        SetBestHistorico();
    }

    public void SetBestHistorico()
    {
        Population bestHistorico = JsonUtility.FromJson<Population>(File.ReadAllText(PathOfBest));
        Population currentHistorico = JsonUtility.FromJson<Population>(File.ReadAllText(Path));
        if (TamanhoDoHistorico(bestHistorico) * bestHistorico.EscalaDeTempo < TamanhoDoHistorico(currentHistorico) * currentHistorico.EscalaDeTempo)
        {
            SetJson(currentHistorico, PathOfBest);
        }
    }

    void IniciarJson()
    {
        Population populacao = new Population();
        List<Population.Generation> Geracoes = new List<Population.Generation>();

        Geracoes.Add(GetCurrentGenerationMedians());
        populacao.lista = Geracoes;
        populacao.EscalaDeTempo = MeasureDelay;
        SetJson(populacao, Path);
        //garante q os arquivo Best existe
        if (!File.Exists(PathOfBest))
        {
            SetJson(populacao, PathOfBest);
        }
    }


    void SetJson(Population populacao, string path)
    {
        string json = JsonUtility.ToJson(populacao);
        File.WriteAllText(path, json);

    }
    void AddToJson(Population.Generation Geracao)
    {
        Population populacao = JsonUtility.FromJson<Population>(File.ReadAllText(Path));
        populacao.lista.Add(Geracao);

        if(TamanhoDoHistorico(populacao) >= MaxGraphDots)
        {
            //Simplificar o histórico
            MeasureDelay *= 2;
            populacao = SimplifyInHalf(populacao);
            populacao.EscalaDeTempo = MeasureDelay;
        }

        File.WriteAllText(Path, JsonUtility.ToJson(populacao));
        
    }

    public Population LoadPopulation(int id = 0)
    {
        if (id == 0)
        {
            return JsonUtility.FromJson<Population>(File.ReadAllText(Path));
        }
        return JsonUtility.FromJson<Population>(File.ReadAllText(PathOfBest));
    }

    public Population SimplifyInHalf(Population original)
    {
        if (original.lista.Count % 2 == 0)
        {
            Debug.LogError("even number");
            return original;
        }
        else
        {
            Population retorno = new Population();
            retorno.lista = new List<Population.Generation>();
            retorno.lista.Add(original.lista[0]);
            for (int i = 1; i+1 < original.lista.Count; i += 2)
            {
                retorno.lista.Add(GenerationMedium(original.lista[i], original.lista[i + 1]));
            }
            Debug.Log("nova lista é " + ((float)retorno.lista.Count-1) / ((float)original.lista.Count-1) + " da antiga");
            
            return retorno;
        }

    }

    void UpdateHistorico()
    {
        AddToJson(GetCurrentGenerationMedians());
        Invoke("UpdateHistorico", MeasureDelay);
    }
    
    Population.Generation GetCurrentGenerationMedians()
    {
        Criatura[] criaturas = Parent.GetComponentsInChildren<Criatura>();
        int velocidade = 0, visao=0, vontadeDeAcasalamento=0, inteligencia=0;
        int quantidade = criaturas.Length;
        for (int i = 0; i < quantidade; i++)
        {
            velocidade += criaturas[i].Genes.Velocidade;
            visao += criaturas[i].Genes.Visao;
            vontadeDeAcasalamento += criaturas[i].Genes.VontadeDeAcasalamento;
            inteligencia += criaturas[i].Genes.Inteligencia;
        }

        Population.Generation geracao = new Population.Generation();
        geracao.Velocidade = velocidade / quantidade;
        geracao.Visao = visao / quantidade;
        geracao.VontadeDeAcasalamento = vontadeDeAcasalamento / quantidade;
        geracao.Inteligencia = inteligencia / quantidade;
        geracao.Quantidade = quantidade;

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

        return retorno;
    }

    int TamanhoDoHistorico(Population pop)
    {
        return pop.lista.Count;
    }
}
