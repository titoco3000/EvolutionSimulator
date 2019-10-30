using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ComidaViva : Comida
{
    public override void Comer(Criatura cliente)
    {
        if (!Comido)
        {
            Criatura minhaCriatura = GetComponent<Criatura>();
            GetComponent<NavMeshAgent>().enabled = false;
            cliente.FoodAmount += minhaCriatura.FoodAmount*2;
            minhaCriatura.enabled = false;

            base.Comer(cliente);
        }
    }
}
