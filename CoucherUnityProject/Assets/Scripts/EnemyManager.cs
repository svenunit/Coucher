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
    [SerializeField] private Enemy basicEnemyPrefab;
    [SerializeField] private GameObject enemySpawnIndicator;
    [SerializeField] private GameObject basicEnemyDeathPSPrefab;

    private ParticleSystem basicEnemyDeathPS;

    [Header("Enemy Level Data")]
    [SerializeField] private EnemyLevelData[] enemyLevelData;
    private int currentLevelIndex = 0;
    private EnemyLevelData currentEnemyLevelData;
    private Wave currentWave;
    public Wave nextWave => waveIndex <= currentEnemyLevelData?.Waves?.Length - 1 ? currentEnemyLevelData.Waves[waveIndex] : null;
    private int waveIndex = 0;

    [Header("Enemy Spawn")]
    [Range(.1f, 2f)] [SerializeField] private float enemySpawnDelay = 1f;

    public bool SpawnProcessOngoing => spawnWaveCoroutine != null;
    private bool inbetweenLevels = false;
    private float nextWaveTimer = 0f;

    public List<Enemy> Enemies { get; private set; }

    private Collider2D[] positionCheckColliders = new Collider2D[1];

    private Coroutine spawnWaveCoroutine;

    private void Awake()
    {
        Enemies = new List<Enemy>(FindObjectsOfType<Enemy>());
        basicEnemyDeathPS = Instantiate(basicEnemyDeathPSPrefab, Vector3.zero, Quaternion.identity).GetComponent<ParticleSystem>();
        InitSpawnGrid();
    }

    private void OnEnable()
    {
        EventManager.EnemyDied.AddListener(this, OnEnemyDied);
        EventManager.NewLevelStarted.AddListener(this, OnNewLevelStarted);
    }

    private void OnDisable()
    {
        EventManager.EnemyDied.RemoveListener(this);
        EventManager.NewLevelStarted.RemoveListener(this);
    }

    private void Start()
    {

    }

    private void Update()
    {
        HandleCurrentWaveTimer();
        if (Input.GetMouseButtonDown(0))
        {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0f;
            pos = GetClosestGridPos(pos);
            if ((PositionInsideGrid(pos) == true) && PositionIsOccupied(pos) == false)
            {
                SpawnEnemy(basicEnemyPrefab, pos);
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TrySpawnNextWave();
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            foreach (var enemy in Enemies.ToArray())
                enemy.OnHitByPlayerDash(1);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(gridOrigin, gridSize);
        //if (SpawnGrid != null)
        //{
        //    foreach (var posArray in SpawnGrid)
        //    {
        //        foreach (var pos in posArray)
        //        {
        //            Gizmos.DrawWireCube(pos + new Vector2(.5f * gridSpacing, .5f * gridSpacing), (Vector2.one * gridSpacing) - new Vector2(0.01f, 0.01f));
        //        }
        //    }
        //}
    }

    private void OnEnemyDied(Enemy enemy)
    {
        Enemies.Remove(enemy);
        if (Enemies.Count == 0)
            TrySpawnNextWave();
    }

    private void OnNewLevelStarted((int levelIndex, Vector2 levelCenter) levelInfo)
    {
        // Adjust position of enemy spawn grid and pathfinding grid before init of level.
        gridOrigin = levelInfo.levelCenter;
        for (int x = 0; x < SpawnGrid.Length; x++)
        {
            for (int y = 0; y < SpawnGrid[x].Length; y++)
            {
                SpawnGrid[x][y] += levelInfo.levelCenter;
            }
        }
        InitLevel(levelInfo.levelIndex);
    }

    private bool WavesLeft(EnemyLevelData currentEnemyLevelData)
    {
        if (waveIndex < currentEnemyLevelData.Waves.Length) return true;
        else return false;
    }

    private void InitSpawnGrid()
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

    private void InitLevel(int index)
    {
        if (index < 0 || index > enemyLevelData.Length - 1)
        {
            throw new Exception("Cannot get level with index: " + index);
        }
        waveIndex = 0;
        currentLevelIndex = index;
        currentEnemyLevelData = enemyLevelData[currentLevelIndex];
        currentWave = currentEnemyLevelData.Waves[waveIndex];
        TrySpawnNextWave();
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

    private void TrySpawnNextWave()
    {
        if (WavesLeft(currentEnemyLevelData) == false)
        {
            EventManager.AllWavesDone.RaiseEvent();
            print("AllWavesDone");
            return;
        }
        currentWave = currentEnemyLevelData.Waves[waveIndex];
        waveIndex++;
        spawnWaveCoroutine = StartCoroutine(SpawnWave(currentWave));
        print("Spawning next wave!");
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
        nextWaveTimer = 0f;
    }

    public void SpawnEnemy(Enemy enemyToSpawn, Vector2 pos)
    {
        StartCoroutine(EnemySpawnRoutine(enemyToSpawn, pos));
    }

    private IEnumerator EnemySpawnRoutine(Enemy enemyToSpawn, Vector2 pos)
    {
        // Spawn enemy spawn indicator.
        var indicator = Instantiate(enemySpawnIndicator, pos, Quaternion.identity);
        Animator animator = indicator.GetComponent<Animator>();
        do
        {
            yield return null;
        } while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f);
        // Spawn enemy and remove indicator.
        indicator.SetActive(false);
        Enemies.Add(Instantiate(enemyToSpawn, pos, Quaternion.identity));
        Destroy(indicator);
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

    private void HandleCurrentWaveTimer()
    {
        if (SpawnProcessOngoing == true || (nextWave == null || nextWave.TriggeredByTime == false) || inbetweenLevels == true) return;
        nextWaveTimer += Time.deltaTime;
        if (nextWaveTimer > nextWave.TimeTriggerMin)
        {
            TrySpawnNextWave();
        }
    }
}
