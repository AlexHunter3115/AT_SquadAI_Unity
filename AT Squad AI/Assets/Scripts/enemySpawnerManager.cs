using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemySpawnerManager : MonoBehaviour
{
    public static enemySpawnerManager instance;

    public List<Transform> spawnPlaces;

    public GameObject enemyPrefab;

    public float lastSpawn;
    public float spawnRate = 20;

    public Transform point;

    public bool allowed = false;

    private void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (allowed) 
        {
            if (Time.time > lastSpawn + spawnRate)
            {
                lastSpawn = Time.time;

                int ranNum = Random.Range(0, spawnPlaces.Count);

                GameObject newRef = Instantiate(enemyPrefab, spawnPlaces[ranNum].transform.position, enemyPrefab.transform.rotation, this.transform);
                newRef.GetComponent<EnemyScript>().CallMove();
            }
        }
    }
}
