using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Predador : Criatura
{
    
    public override bool PossoCaçar()
    {
        if (manager.PredadorConciente)
        {
            return manager.NumeroDeCriaturas/2 > manager.NumeroDePredadores;
        }
        return true;
    }

    public override bool PodeReproduzir()
    {
        if(manager.NumeroDeCriaturas/2 < manager.NumeroDePredadores && manager.PredadorConciente )
        {
            return false;
        }
        return base.PodeReproduzir();
    }
}
