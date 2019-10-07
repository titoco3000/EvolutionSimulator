using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawnwer : MonoBehaviour
{
    bool CircularSpawn = true;

    public float Radius = 40f;
    public int maxFoodPerBatch = 15;
    public float foodChance = 0.75f;

    public int repeatRate = 6;

    public Transform originalFood;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnFood", 0, repeatRate);
    }

    void SpawnFood()
    {
        int spawnedInThisBatch = 0;
        while ( spawnedInThisBatch < maxFoodPerBatch && Random.Range(0.0f,1.0f) < foodChance)
        {
            Vector3 SpawnPosition;
            if (CircularSpawn)
            {
                SpawnPosition = Random.insideUnitCircle.normalized * Random.Range(-Radius, Radius);
            }
            else
            {
                SpawnPosition = Vector3.zero;
            }
            Transform clone = Instantiate(originalFood, this.transform);
            SpawnPosition.z = SpawnPosition.y;
            SpawnPosition.y = clone.position.y;
            clone.position = SpawnPosition;
            spawnedInThisBatch++;
        }
    }
}
