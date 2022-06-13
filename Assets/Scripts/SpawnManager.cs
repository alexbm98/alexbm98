using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }
    public GameObject enemyPrefabBoxidemon;
    public GameObject enemyPrefabTurtleShell;
    public float prob = 0.5f;
    private int nSpawns = 4;
    private float respawnTime;
    private Vector3[] coordSpawns;
    public List<DemonController> enemies;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        coordSpawns = new Vector3[nSpawns];

        coordSpawns[0] = new Vector3(55.37f, 2.67f, 41.82f);
        coordSpawns[1] = new Vector3(20.30f, 1.49f, 50.57f);
        coordSpawns[2] = new Vector3(14.49f, 1.56f, 29.74f);
        coordSpawns[3] = new Vector3(43.03f, 2.81f, 23.38f);

        if (PlayerPrefs.GetString("Difficulty") == "Facil")
        {
            prob = 0.25f;
            respawnTime = 15f;
        }

        if (PlayerPrefs.GetString("Difficulty") == "Normal")
        {
            prob = 0.5f;
            respawnTime = 25f;
        }

        if (PlayerPrefs.GetString("Difficulty") == "Dificil")
        {
            prob = 0.75f;
            respawnTime = 35f;
        }
    }

    // Update is called once per frame
    public void StartSpawn()
    {
        InvokeRepeating("SpawnEnemy", 3f, respawnTime);
    }

    void SpawnEnemy()
    {
        GameObject enemyPrefab;

        for (int i = 0; i < nSpawns; i++)
        {
            float eleccion = Random.value;

            if (eleccion < prob)
            {
                enemyPrefab = enemyPrefabBoxidemon;
            }
            else
            {
                enemyPrefab = enemyPrefabTurtleShell;
            }

            Instantiate(enemyPrefab, coordSpawns[i], Quaternion.identity);
        }
    }
}
