using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Population
{
    [System.Serializable]
    public class Generation
    {
        public int Velocidade;
        public int Visao;
        public int VontadeDeAcasalamento;
        public int Inteligencia;
        public int Quantidade;
        public int Saude;
        public int R, G, B;
    }


    public List<Generation> listaCriaturas;
    public List<Generation> listaPredadores;

    public int EscalaDeTempo;


    public List<float> GetStat(List<Generation> lista, string stat)
    {
        List<float> retorno = new List<float>();

        foreach (Generation generation in lista)
        {
            if (stat == "Velocidade")
                retorno.Add(generation.Velocidade);
            else if (stat == "Visao")
                retorno.Add(generation.Visao);
            else if (stat == "VontadeDeAcasalamento")
                retorno.Add(generation.VontadeDeAcasalamento);
            else if (stat == "Inteligencia")
                retorno.Add(generation.Inteligencia);
            else if (stat == "Quantidade")
                retorno.Add(generation.Quantidade);
            else if (stat == "Saude")
                retorno.Add(generation.Saude);
            else if (stat == "R")
                retorno.Add(generation.R);
            else if (stat == "G")
                retorno.Add(generation.G);
            else if (stat == "B")
                retorno.Add(generation.B);
            else
                Debug.LogError("Valor imcompatível (" + stat + ")");

        }
        return retorno;
    }

    

}
