using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHolder : MonoBehaviour
{
    public int numeroDeCriaturas;
    public int numeroDePredadores;

    public int valorNutricionalCriaturas,
        valorNutricionalComida;

    public bool ReiniciarNaMorteDePredadores;
    public bool ConcienciaAmbiental;

    public float mutationRate, 
        mutationChance;



    // Start is called before the first frame update
    void Start()
    {
        if (FindObjectsOfType<DataHolder>().Length > 1 ) {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this);

    }

    private void OnLevelWasLoaded(int level)
    {
        Manager manager = FindObjectOfType<Manager>();
        if(manager != null)
        {
            manager.initialAmount_criatura = numeroDeCriaturas;
            manager.initialAmount_predador = numeroDePredadores;
            
            manager.MutationRate = mutationRate;
            manager.MutationChance = mutationChance;

            manager.valorNutricionalComida = valorNutricionalComida;
            manager.valorNutricionalCriatura = valorNutricionalCriaturas;

            manager.ReiniciarNaMorteDePredadores = ReiniciarNaMorteDePredadores;
            manager.PredadorConciente = ConcienciaAmbiental;

        }
    }

}
