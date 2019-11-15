using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawnwer : MonoBehaviour
{
    public float Radius = 40f;
    public int maxFoodPerBatch = 15;
    public float foodChance = 0.75f;
    public float foodProtectionRadius = 3f;

    public int repeatRate = 6;

    public Transform originalFood;

    public LayerMask foodLayer;

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
            Vector3 SpawnPosition= Random.insideUnitCircle.normalized * Random.Range(-Radius, Radius); 


            SpawnPosition.z = SpawnPosition.y;
            //SpawnPosition.y = clone.position.y;
            SpawnPosition.y = transform.position.y;

            if (Physics.OverlapSphere(SpawnPosition, foodProtectionRadius, foodLayer ).Length == 0)
            {
                Transform clone = Instantiate(originalFood, this.transform);
                clone.position = SpawnPosition;
                spawnedInThisBatch++;
            }

        }
    }

  
}
