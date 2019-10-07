using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DNA
{

    //Genes
    #region
    public int Velocidade; //1-100
    public int Visao; //1-100
    public int VontadeDeAcasalamento; //2-100
    public int Inteligencia; //1-100

    //esses aqui não tem valor funcional, só ilustrativo
    public int Red; //0-255
    public int Green; //0-255
    public int Blue; //0-255

    #endregion

    //configurações
    #region
        //mutações
    private float MutationChance = 0.2f;
    private float MutationRate = 20f;
    //outros
    #endregion

    //Constructors
    #region
    //construtor da geração espontanea, exige chamar o SetRandomGenes()
    public DNA()
    {
    }

    //construtor dos descendentes
    public DNA(int[] father, int[] mother)
    {
        Velocidade = ChooseGene(father[0], mother[0]);
        Visao = ChooseGene(father[1], mother[1]);
        VontadeDeAcasalamento = ChooseGene(father[2], mother[2]);
        Inteligencia = ChooseGene(father[3], father[3]);

        Vector3Int corDoFilho = ChooseColorGene(new Vector3Int(father[4], father[5], father[6]), new Vector3Int(mother[4], mother[5], mother[6]));
        Red = corDoFilho.x;
        Green = corDoFilho.y;
        Blue = corDoFilho.z;

    }
    #endregion

    public void SetRandomGenes()
    {
        Velocidade = Random.Range(5, 95);
        Visao = Random.Range(5, 95);
        VontadeDeAcasalamento = Random.Range(2, 95);
        Inteligencia = Random.Range(5, 95);

        Red = Random.Range(0, 255);
        Green = Random.Range(0, 255);
        Blue = Random.Range(0, 255);
    }

    public void SetDefaultGenes()
    {
        Velocidade = 30;
        Visao = 30;
        VontadeDeAcasalamento = 30;
        Inteligencia = 30;

        Red = 127;
        Green = 127;
        Blue = 127;
    }

    //O custo energetico do dna
    public int GetCost()
    {
        return (Velocidade + Visao)/2;
    }

    public int[] ToArray()
    {
        int[] output = new int[7];
        output[0] = Velocidade;
        output[1] = Visao;
        output[2] = VontadeDeAcasalamento;
        output[3] = Inteligencia;
        output[4] = Red;
        output[5] = Green;
        output[6] = Blue;
        return output;
    }

    //Para criar os genes e a cor do filho, resp.
    private int ChooseGene(int a, int b)
    {
        //escolhe um dos pais para fornecer o gene
        //int output = Random.Range(0f, 1f) < 0.5f ? a : b;
        int output = a;
        if (Random.Range(0f , 1f) < MutationChance)
        {
            output += (int)Random.Range(-MutationRate, MutationRate);
        }
        
        return AvoidOverflow(output, 100);
    }
    private Vector3Int ChooseColorGene(Vector3Int a, Vector3Int b)
    {
        /* média perfeita
        float mediaR = Mathf.Sqrt((Mathf.Pow(a.x, 2) + Mathf.Pow(b.x, 2))/2);
        float mediaG = Mathf.Sqrt((Mathf.Pow(a.y, 2) + Mathf.Pow(b.y, 2))/2);
        float mediaB = Mathf.Sqrt((Mathf.Pow(a.z, 2) + Mathf.Pow(b.z, 2))/2);
        */

        //média simples
        float mediaR = (a.x + b.x) / 2;
        float mediaG = (a.y + b.y) / 2;
        float mediaB = (a.z + b.z) / 2;


        //escolhe um dos pais para fornecer o gene
        //Vector3Int output = Random.Range(0, 1) < 0.5f ? a : b;

        Vector3Int output = new Vector3Int((int)mediaR, (int)mediaG, (int)mediaB);

        //Cria (talvez) mutações
        if (Random.Range(0.0f, 1.0f) < MutationChance)
        {
            output.x += (int)Random.Range(-MutationRate*2, MutationRate * 2);
        }
        if (Random.Range(0.0f, 1.0f) < MutationChance)
        {
            output.y += (int)Random.Range(-MutationRate * 2, MutationRate * 2);
        }
        if (Random.Range(0.0f, 1.0f) < MutationChance)
        {
            output.z += (int)Random.Range(-MutationRate * 2, MutationRate * 2);
        }
        //evita overflows
        output.x = AvoidOverflow(output.x, 255);
        output.y = AvoidOverflow(output.y, 255);
        output.z = AvoidOverflow(output.z, 255);
        return output;
    }

    private int AvoidOverflow(int num, int max)
    {
        if (num < 1)
            return 1;
        else if (num > max)
            return max;
        return num;
    }




}
