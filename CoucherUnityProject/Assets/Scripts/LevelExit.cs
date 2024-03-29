﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelExit : MonoBehaviour
{
    private HashSet<PlayerInput> players;

    private BoxCollider2D blockerCollider;
    private SpriteRenderer spriteRenderer;

    private GameObject lightAndParticlesGameObject;

    private void Awake()
    {
        players = new HashSet<PlayerInput>();
        blockerCollider = transform.Find("BlockerCollider").GetComponent<BoxCollider2D>();
        spriteRenderer = transform.Find("LevelExitSprite").GetComponent<SpriteRenderer>();
        lightAndParticlesGameObject = transform.Find("LevelExitSprite").Find("Light_Door").gameObject;
    }

    public void OpenExit()
    {
        blockerCollider.enabled = false;
        spriteRenderer.enabled = true;
        lightAndParticlesGameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D c)
    {
        PlayerInput player = c.GetComponent<PlayerInput>();
        if (player != null && players.Contains(player) == false)
        {
            player._playerCanMove = false;
            players.Add(player);
            EventManager.PlayerEnteredExit.RaiseEvent();
            player.GetComponent<SpriteRenderer>().enabled = false;
            print("PlayerEnteredExit");
        }
    }
}
