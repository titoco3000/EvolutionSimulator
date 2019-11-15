using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComidaMorta : Comida
{
    public override void Comer(Criatura cliente)
    {
        if (!Comido)
        {
            cliente.FoodAmount += valorNutricional;
            base.Comer(cliente);
        }
    }
}
