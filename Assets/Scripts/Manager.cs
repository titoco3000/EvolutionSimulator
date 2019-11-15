using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Manager : MonoBehaviour
{
    public float Radius = 40f;
    public int enemyDelay = 300;
    private float MeasureDelay = .1f;

    public GameObject criatura;
    public GameObject predador;
    public Transform Parent_criatura;
    public Transform Parent_predador;
    public SaveHandler data;
    


    public int initialAmount_criatura = 10;
    public int initialAmount_predador = 8;


    public bool ReiniciarNaMorteDePredadores;
    public bool PredadorConciente;

    public int NumeroDeCriaturas;
    public int NumeroDePredadores;

    public float MutationChance;
    public float MutationRate;

    public int valorNutricionalComida;
    public int valorNutricionalCriatura;

    void Start()
    {
        criatura.GetComponent<Comida>().valorNutricional = valorNutricionalCriatura;
            FindObjectOfType<FoodSpawnwer>().originalFood.GetComponent<ComidaMorta>().valorNutricional = valorNutricionalComida;


        PlaceCriatura( initialAmount_criatura );
        Invoke("SpawnEnemies", enemyDelay);

    }
    private void Update()
    {
        CountCreatures();

    }

    void SpawnEnemies()
    {
        PlacePredador( initialAmount_predador);

    }

    public void PlacePredador(int n = 1)
    {
        for (int i = 0; i < n; i++)
        { 
            Vector3 spawnPosition = Random.insideUnitCircle.normalized * Random.Range(-Radius, Radius);
            spawnPosition.z = spawnPosition.y;
            spawnPosition.y = 0;
            Instantiate(predador, spawnPosition, transform.rotation, Parent_predador);
        }
    }
    public void PlaceCriatura(int n = 1)
    {
        for (int i = 0; i < n; i++)
        {
            Vector3 spawnPosition = Random.insideUnitCircle.normalized * Random.Range(-Radius, Radius);
            spawnPosition.z = spawnPosition.y;
            spawnPosition.y = 0;
            Instantiate(criatura, spawnPosition, transform.rotation, Parent_criatura);
        }
    }


    public bool DeveReiniciar(int c, int p)
    {
        NumeroDeCriaturas = c;
        NumeroDePredadores = p;
        if (NumeroDeCriaturas == 0 || (ReiniciarNaMorteDePredadores && NumeroDePredadores == 0) )
        {
            Invoke("Restart", 60);
            return true;
        }
        return false;

    }

    void Restart()
    {
        Debug.Log("Reiniciando...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void UpdateData()
    {
        Debug.Log("Updating data");
        
        //data.UpdateHistorico(currentGenCriaturas, currentGenPredadores);
        //Invoke("UpdateData", MeasureDelay);

    }

    void CountCreatures()
    {
        NumeroDeCriaturas = Parent_criatura.childCount;
        NumeroDePredadores = Parent_predador.childCount;
    }

}
