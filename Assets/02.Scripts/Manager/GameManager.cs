using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Player player;
    public MapGenerator mapGenerator;
    public EnemySpawner enemySpawner;

    public int currentFloor = 1; //기본 층 수 B1


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 중복 방지
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬 전환 시 유지
    }

    private void Start()
    {
        CreateNewFloor();
    }

    public void CreateNewFloor()
    {
        mapGenerator.GenerateNewFloor();
        mapGenerator.RenderAll();
        var baker = FindObjectOfType<MapNavBaker>();
        if (baker != null) baker.Build();

        enemySpawner.SpawnAllEnemies();
    }
}
