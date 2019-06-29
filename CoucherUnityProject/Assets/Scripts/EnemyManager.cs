using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour, IListener
{
    [Header("Spawn Grid")]
    [SerializeField] private Vector2 gridOrigin;
    [SerializeField] private Vector2 gridSize;
    [SerializeField] private float gridSpacing = 1f;
    private Vector2 gridLowerLeft;
    private Vector2 gridLowerRight;
    private Vector2 gridTopLeft;
    private Vector2 gridTopRight;

    public Vector2[][] SpawnGrid { get; private set; }

    [Header("Enemy Prefabs")]
    [SerializeField] private Enemy BasicEnemyPrefab;

    [Header("Enemy Level Data")]
    [SerializeField] private EnemyLevelData[] enemyLevelData;
    private int currentLevelIndex = 0;
    private EnemyLevelData currentEnemyLevelData;
    private Wave currentWave;
    private int waveIndex = 0;

    public List<Enemy> Enemies { get; private set; }

    private Collider2D[] positionCheckColliders = new Collider2D[1];

    private Coroutine spawnWaveCoroutine;

    private void Awake()
    {
        Enemies = new List<Enemy>(FindObjectsOfType<Enemy>());
        InitGrid();
    }

    private void OnEnable()
    {
        EventManager.EnemyDied.AddListener(this, OnEnemyDied);
    }

    private void OnDisable()
    {
        EventManager.EnemyDied.RemoveListener(this);
    }

    private void Start()
    {
        currentEnemyLevelData = enemyLevelData[0];
        currentWave = currentEnemyLevelData.Waves[waveIndex];
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0f;
            pos = GetClosestGridPos(pos);
            if ((PositionInsideGrid(pos) == true) && PositionIsOccupied(pos) == false)
            {
                SpawnEnemy(BasicEnemyPrefab, pos);
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //foreach (var enemy in Enemies)
            //    enemy.Stun();
            spawnWaveCoroutine = StartCoroutine(SpawnWave(currentWave));
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            //foreach (var enemy in Enemies)
            //    enemy.Recover();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(gridOrigin, gridSize);
        if (SpawnGrid != null)
        {
            foreach (var posArray in SpawnGrid)
            {
                foreach (var pos in posArray)
                {
                    Gizmos.DrawWireCube(pos + new Vector2(.5f * gridSpacing, .5f * gridSpacing), (Vector2.one * gridSpacing) - new Vector2(0.01f, 0.01f));
                }
            }
        }
    }

    private void OnEnemyDied(Enemy enemy)
    {
        Enemies.Remove(enemy);
        Destroy(enemy.gameObject);
        if (Enemies.Count == 0)
        {
            if (WavesLeft(currentEnemyLevelData) == true)
            {
                currentWave = currentEnemyLevelData.Waves[waveIndex];
                waveIndex++;
                spawnWaveCoroutine = StartCoroutine(SpawnWave(currentWave));
            }
            else
                EventManager.AllEnemiesDead.RaiseEvent();
        }
    }

    private bool WavesLeft(EnemyLevelData currentEnemyLevelData)
    {
        if (waveIndex < currentEnemyLevelData.Waves.Length) return true;
        else return false;
    }

    private void InitGrid()
    {
        int sizeX = (int)(gridSize.x * (1f / gridSpacing));
        int sizeY = (int)(gridSize.y * (1f / gridSpacing));
        SpawnGrid = new Vector2[sizeX][];
        for (int i = 0; i < SpawnGrid.Length; i++) SpawnGrid[i] = new Vector2[sizeY];
        gridLowerLeft = gridOrigin - (gridSize * .5f);
        gridLowerRight = gridOrigin + new Vector2(gridSize.x * .5f, -gridSize.y * .5f);
        gridTopLeft = gridOrigin + new Vector2(-gridSize.x * .5f, gridSize.y * .5f);
        gridTopRight = gridOrigin + (gridSize * .5f);
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                SpawnGrid[x][y] = gridLowerLeft + new Vector2(x * gridSpacing, y * gridSpacing);
            }
        }
    }

    public Vector2 GetClosestGridPos(Vector2 inputPosition)
    {
        return new Vector2(Mathf.Round(inputPosition.x), Mathf.Round(inputPosition.y));
    }

    public bool PositionInsideGrid(Vector2 inputPosition)
    {
        if (inputPosition.x >= gridLowerLeft.x && inputPosition.x <= gridLowerRight.x &&
            inputPosition.y >= gridLowerLeft.y && inputPosition.y <= gridTopRight.y)
            return true;
        else return false;
    }


    public IEnumerator SpawnWave(Wave wave)
    {
        float delayAfterEachSpawn = wave.EnemySpawnDuration / wave.EnemiesToSpawn.Length;
        foreach (var enemy in wave.EnemiesToSpawn)
        {
            Vector2 spawnPos = FindEnemySpawnPosition();
            SpawnEnemy(enemy, spawnPos);
            if (wave.SpawnEnemiesOverTime == true)
                yield return new WaitForSeconds(delayAfterEachSpawn);
        }
        yield return null;
        spawnWaveCoroutine = null;
    }

    public void SpawnEnemy(Enemy enemyToSpawn, Vector2 pos)
    {
        Enemies.Add(Instantiate(enemyToSpawn, pos, Quaternion.identity));
    }

    private Vector2 FindEnemySpawnPosition()
    {
        int x = UnityEngine.Random.Range(0, SpawnGrid.Length - 1);
        int y = UnityEngine.Random.Range(0, SpawnGrid[0].Length - 1);
        Vector2 spawnPos = SpawnGrid[x][y];
        while (PositionIsOccupied(spawnPos) == true)
        {
            x = UnityEngine.Random.Range(0, SpawnGrid.Length - 1);
            y = UnityEngine.Random.Range(0, SpawnGrid[0].Length - 1);
            spawnPos = SpawnGrid[x][y];
        }
        return spawnPos;
    }

    private bool PositionIsOccupied(Vector2 position)
    {
        if (Physics2D.OverlapBoxNonAlloc(position, Vector2.one * gridSpacing, 0f, positionCheckColliders) > 0)
            return true;
        else return false;
    }
}
