using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour, IListener
{
    private Camera cam;

    private int currentLevelIndex = 0;

    private int playerReachedExitCounter = 0;

    [SerializeField] private Tilemap[] levelTilemaps;
    [SerializeField] private LevelExit[] levelExits;
    [Header("Camera movement")]
    [SerializeField] private float camGotoLevelAnimDuration;
    [SerializeField] private AnimationCurve camGotoLevelAnimCurve;

    [Header("Tiles")]
    [SerializeField] private Tile doorOpenTileTopWall;
    [SerializeField] private Tile doorClosedTileTopWall;


    private PlayerInput[] players;

    private void Awake()
    {
        cam = Camera.main;
        players = FindObjectsOfType<PlayerInput>();
    }

    private void OnEnable()
    {
        EventManager.AllWavesDone.AddListener(this, OnAllWavesDone);
        EventManager.PlayerEnteredExit.AddListener(this, OnPlayerEnteredExit);
    }

    private void OnDisable()
    {
        EventManager.AllWavesDone.RemoveListener(this);
        EventManager.PlayerEnteredExit.RemoveListener(this);
    }

    void Start()
    {
        StartCoroutine(InitRoutine());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            StartNextLevel();
        }
    }

    private void OnAllWavesDone()
    {
        StartCoroutine(EndofLevelRoutine());
    }

    private void OnPlayerEnteredExit()
    {
        playerReachedExitCounter++;
    }

    private IEnumerator EndofLevelRoutine()
    {
        // Open door
        levelExits[currentLevelIndex - 1].OpenExit();
        do
        {
            yield return null;
        } while (playerReachedExitCounter < 2);

        StartNextLevel();
        playerReachedExitCounter = 0;
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
        // Move Players
        players[0].transform.position = trgPosPlayers + (Vector3.right * 2f);
        players[1].transform.position = trgPosPlayers + (Vector3.left * 2f);
        players[0]._playerCanMove = true;
        players[1]._playerCanMove = true;
        players[0].GetComponent<SpriteRenderer>().enabled = true;
        players[1].GetComponent<SpriteRenderer>().enabled = true;
        EventManager.NewLevelStarted.RaiseEvent((currentLevelIndex, levelTilemaps[currentLevelIndex].transform.position));
        currentLevelIndex++;
    }

}
