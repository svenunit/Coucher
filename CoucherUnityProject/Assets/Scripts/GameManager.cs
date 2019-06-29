using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour, IListener
{
    private Camera cam;

    private int currentLevelIndex = 0;

    [SerializeField] private Tilemap[] levelTilemaps;
    [SerializeField] private Transform[] levelExits;
    [Header("Camera movement")]
    [SerializeField] private float camGotoLevelAnimDuration;
    [SerializeField] private AnimationCurve camGotoLevelAnimCurve;

    [Header("Tiles")]
    [SerializeField] private Tile doorOpenTileTopWall;
    [SerializeField] private Tile doorClosedTileTopWall;

    private Tile doorOpenTileRightWall;
    private Tile doorClosedTileRightWall;

    private PlayerInput[] players;

    private void Awake()
    {
        cam = Camera.main;
        print(levelTilemaps[0].size);
        players = FindObjectsOfType<PlayerInput>();
        BoundsInt bounds = levelTilemaps[0].cellBounds;
        TileBase[] allTiles = levelTilemaps[0].GetTilesBlock(bounds);
        foreach (var tile in allTiles)
        {
            print(tile);
        }
    }

    private void OnEnable()
    {
        EventManager.AllWavesDone.AddListener(this, OnAllWavesDone);
    }

    private void OnDisable()
    {
        EventManager.AllWavesDone.RemoveListener(this);
    }

    void Start()
    {
        StartCoroutine(InitRoutine());
    }

    void Update()
    {

    }

    private void OnAllWavesDone()
    {
        StartCoroutine(EndofLevelRoutine());
    }

    private IEnumerator EndofLevelRoutine()
    {
        // Open door
        levelExits[currentLevelIndex - 1].GetComponent<SpriteRenderer>().enabled = true;
        StartNextLevel();
        // More things?
        yield return null;
    }

    private IEnumerator InitRoutine()
    {
        // Fade into the first room

        yield return null;
        StartNextLevel();
    }

    private void StartNextLevel()
    {
        StartCoroutine(StartNextLevelRoutine());
    }

    private IEnumerator StartNextLevelRoutine()
    {
        // Move camera to room
        Vector3 trgPosCam = levelTilemaps[currentLevelIndex].gameObject.transform.position;
        Vector3 trgPosPlayers = levelTilemaps[currentLevelIndex].gameObject.transform.position;
        trgPosCam.z = -10f;
        yield return StartCoroutine(Utility.MoveGameObjectRoutine(cam.transform, trgPosCam, camGotoLevelAnimDuration, camGotoLevelAnimCurve));
        players[0].transform.position = trgPosPlayers + (Vector3.right * 2f);
        players[1].transform.position = trgPosPlayers + (Vector3.left * 2f);
        EventManager.NewLevelStarted.RaiseEvent((currentLevelIndex, levelTilemaps[currentLevelIndex].transform.position));
        currentLevelIndex++;
    }

}
