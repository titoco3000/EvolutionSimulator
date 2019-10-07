using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    public GameObject adultPrefab;
    public float Radius = 40f;
    public int initialAmount = 10;
    public Transform Parent;

    void Start()
    {
        SpawnInitialBatch(initialAmount);
        InvokeRepeating("CheckNumberOfCreatures", 10f, 10f);
    }


    private void SpawnInitialBatch(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector3 spawnPosition = Random.insideUnitCircle.normalized* Random.Range(-Radius, Radius);
            spawnPosition.z = spawnPosition.y;
            spawnPosition.y = 0;
            Instantiate(adultPrefab, spawnPosition, transform.rotation, Parent);
        }
    }

    private void CheckNumberOfCreatures()
    {
        GameObject[] criaturas = GameObject.FindGameObjectsWithTag("Player");
        if(criaturas.Length <= 1)
        {
            Restart();
            GetComponent<SaveHandler>().SetBestHistorico();
        }
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
