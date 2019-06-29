﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Enemy : MonoBehaviour
{
    [Range(0.1f, 5f)] [SerializeField] protected float moveSpeed = 1f;
    public float MoveSpeed
    {
        get => moveSpeed;
        protected set => moveSpeed = value;
    }

    [Range(1, 10)] protected int health;
    public int Health
    {
        get => health;
        protected set
        {
            health = Mathf.Clamp(value, 0, int.MaxValue);
            if (health == 0) Die();
        }
    }

    [Range(1, 10)] [SerializeField] protected int startingHealth;
    public int StartingHealth
    {
        get => startingHealth;
        protected set => startingHealth = Mathf.Clamp(value, 0, int.MaxValue);
    }

    protected bool stunned;
    public bool Stunned
    {
        get => stunned;
        protected set => stunned = value;
    }

    protected Transform target;
    public Transform Target
    {
        get => target;
        protected set => target = value;
    }

    protected Transform player1;
    public Transform Player1
    {
        get => player1;
        set => player1 = value;
    }

    protected Transform player2;
    public Transform Player2
    {
        get => player2;
        set => player2 = value;
    }

    protected virtual void Awake()
    {
        GetPlayerReferences();
        Health = StartingHealth;
    }

    protected virtual void Start()
    {
        Target = FindTarget();
    }

    protected virtual void FixedUpdate()
    {
        PursueTarget();
    }

    private void OnDrawGizmos()
    {
        if (Target == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, Target.position);
    }

    private void GetPlayerReferences()
    {
        // Find players here, Player1 is the one with the lower PlayerNumber.
        var players = FindObjectsOfType<PlayerInput>();
        if (players == null || players.Length < 2)
        {
            throw new System.Exception("Need 2 players in the scene for enemies to work properly!");
        }
        if (players[0].getPlayerNumber() < players[1].getPlayerNumber())
        {
            Player1 = players[0].transform;
            Player2 = players[1].transform;
        }
        else
        {
            Player1 = players[1].transform;
            Player2 = players[0].transform;
        }
    }

    protected virtual Transform FindTarget()
    {
        if (Player1 == null || Player2 == null) return null;
        // default: get closest player
        float dist1 = Vector2.Distance(transform.position, player1.position);
        if (dist1 < Vector2.Distance(transform.position, player2.position))
            return Player1;
        else return Player2;
    }

    protected virtual void PursueTarget()
    {
        if (Target == null) return;
        transform.position = Vector3.MoveTowards(transform.position, Target.position, moveSpeed * Time.fixedDeltaTime);
    }

    public virtual void TakeDamage(int amount)
    {
        Health -= Mathf.Abs(amount);
    }

    public virtual void Stun()
    {
        Stunned = true;
    }

    public virtual void Recover()
    {
        Stunned = false;
    }

    protected virtual void Die()
    {
        EventManager.EnemyDied.RaiseEvent(this);
    }
}