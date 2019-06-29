using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelExit : MonoBehaviour
{
    private HashSet<PlayerInput> players;

    private BoxCollider2D blockerCollider;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        players = new HashSet<PlayerInput>();
        blockerCollider = transform.Find("BlockerCollider").GetComponent<BoxCollider2D>();
        spriteRenderer = transform.Find("LevelExitSprite").GetComponent<SpriteRenderer>();
    }

    public void OpenExit()
    {
        blockerCollider.enabled = false;
        spriteRenderer.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D c)
    {
        PlayerInput player = c.GetComponent<PlayerInput>();
        if (player != null && players.Contains(player) == false)
        {
            player._playerCanMove = false;
            players.Add(player);
            EventManager.PlayerEnteredExit.RaiseEvent();
            print("PlayerEnteredExit");
        }
    }
}
