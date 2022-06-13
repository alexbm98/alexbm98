using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class DroneController : MonoBehaviour
{
    public static DroneController Instance { get; private set; }

    public Transform drone;
    private Transform player;
    public GameObject healthPotionPrefab;
    public GameObject staminaPotionPrefab;
    public string pocionASoltar;
    private float spawnTime;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LoadParameters();
        StartSpawn();
    }

    public void StartSpawn()
    {
        InvokeRepeating("SpawnPotion", 5f, spawnTime);
    }

    void SpawnPotion()
    {
        Vector3 pos = new Vector3(drone.position.x, drone.position.y - 1f, drone.position.z);

        if (pocionASoltar == "health")
        {
            Instantiate(healthPotionPrefab, pos, Quaternion.identity);
        }

        if (pocionASoltar == "stamina")
        {
            Instantiate(staminaPotionPrefab, pos, Quaternion.identity);
        }
    }

    private void LoadParameters()
    {
        if (PlayerPrefs.GetString("Difficulty") == "Facil")
        {
            spawnTime = 10f;
        }

        if (PlayerPrefs.GetString("Difficulty") == "Normal")
        {
            spawnTime = 20f;
        }

        if (PlayerPrefs.GetString("Difficulty") == "Dificil")
        {
            spawnTime = 30f;
        }
    }
}
