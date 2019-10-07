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
    }

    public List<Generation> lista;
    public int EscalaDeTempo;


    public List<float> GetStat(string stat)
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
            else
                Debug.LogError("Valor imcompatível (" + stat + ")");

        }
        return retorno;
    }

    

}
