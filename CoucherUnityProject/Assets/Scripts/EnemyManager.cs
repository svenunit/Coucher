﻿using System;
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

    public List<Enemy> Enemies { get; private set; }

    private Collider2D[] positionCheckColliders = new Collider2D[1];

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
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireCube(gridOrigin, gridSize);
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
        Destroy(enemy.gameObject);
        if (Enemies.Count == 0)
        {
            EventManager.AllEnemiesDead.RaiseEvent();
        }
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

    public void SpawnEnemy(Enemy enemyToSpawn, Vector2 pos)
    {
        Enemies.Add(Instantiate(enemyToSpawn, pos, Quaternion.identity));
    }

    private bool PositionIsOccupied(Vector2 position)
    {
        if (Physics2D.OverlapBoxNonAlloc(position, Vector2.one * gridSpacing, 0f, positionCheckColliders) > 0)
            return true;
        else return false;
    }
}