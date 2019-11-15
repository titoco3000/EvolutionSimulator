using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CriaturaDoMenu : Criatura
{

    public override void Start()
    {
        base.Start();
        Genes.Velocidade = 30;
        Genes.Inteligencia = 90;
        SetColor(Random.Range(0, 255), Random.Range(0, 255), Random.Range(0, 255));
    }
    public override void GastarComida(int amount)
    {
         FoodAmount = MaxFoodAmount-1;
    }
    public override void MorrerDeVelhice()
    {
        //base.MorrerDeVelhice();
    }
}
