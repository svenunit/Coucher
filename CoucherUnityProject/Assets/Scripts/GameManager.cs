using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour, IListener
{

    private Camera cam;

    private int currentLevelIndex = 0;

    private void Awake()
    {
        cam = Camera.main;
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

    }

    private IEnumerator InitRoutine()
    {
        // Fade into the first room

        yield return null;
        StartNextLevel();
    }

    private IEnumerator StartNextLevelRoutine()
    {
        // Move camera to next room

        yield return null;
        StartNextLevel();
    }
    private void StartNextLevel()
    {
        EventManager.NewLevelStarted.RaiseEvent(currentLevelIndex);
        currentLevelIndex++;
    }
}
