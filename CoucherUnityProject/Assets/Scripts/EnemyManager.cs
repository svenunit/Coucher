using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private Vector2 gridOrigin;
    [SerializeField] private Vector2 gridSize;
    [SerializeField] private float gridSpacing = 1f;

    public Vector2[][] SpawnGrid { get; private set; }

    private void Awake()
    {
        InitGrid();
    }

    private void Start()
    {

    }


    private void Update()
    {

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
                    Gizmos.DrawWireCube(pos, (Vector2.one * gridSpacing) - new Vector2(0.01f, 0.01f));
                }
            }
        }
    }

    private void InitGrid()
    {
        int sizeX = (int)(gridSize.x * (1f / gridSpacing));
        int sizeY = (int)(gridSize.y * (1f / gridSpacing));
        SpawnGrid = new Vector2[sizeX][];
        for (int i = 0; i < SpawnGrid.Length; i++) SpawnGrid[i] = new Vector2[sizeY];
        Vector2 lowerLeft = gridOrigin - (gridSize * .5f) + new Vector2(.5f * gridSpacing, .5f * gridSpacing);
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                SpawnGrid[x][y] = lowerLeft + new Vector2(x * gridSpacing, y * gridSpacing);
            }
        }
    }
}
