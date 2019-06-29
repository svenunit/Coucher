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

    private void Awake()
    {
        cam = Camera.main;
        print(levelTilemaps[0].size);
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
        // Move cam to door

        // Open door

        // More things?

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

        yield return null;
        EventManager.NewLevelStarted.RaiseEvent(currentLevelIndex);
        currentLevelIndex++;
    }

}
