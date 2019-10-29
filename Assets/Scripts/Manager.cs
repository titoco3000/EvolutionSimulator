using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public float Radius = 40f;
    public int enemyDelay = 300;

    public GameObject criatura;
    public GameObject predador;
    public int initialAmount_criatura = 10;
    public int initialAmount_predador = 8;
    public Transform Parent_criatura;
    public Transform Parent_predador;

    void Start()
    {
        SpawnInitialBatch(criatura, initialAmount_criatura, Parent_criatura);
        Invoke("SpawnEnemies", enemyDelay);
    }

    void SpawnEnemies()
    {
        SpawnInitialBatch(predador, initialAmount_predador, Parent_predador);

    }

    private void SpawnInitialBatch( GameObject prefab, int amount, Transform Parent)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector3 spawnPosition = Random.insideUnitCircle.normalized* Random.Range(-Radius, Radius);
            spawnPosition.z = spawnPosition.y;
            spawnPosition.y = 0;
            Instantiate(prefab, spawnPosition, transform.rotation, Parent);
        }
    }


    
}
