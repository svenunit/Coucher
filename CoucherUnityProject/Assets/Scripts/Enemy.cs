using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Range(0.1f, 5f)] [SerializeField] protected float moveSpeed = 1f;
    public float MoveSpeed
    {
        get => moveSpeed;
        protected set => moveSpeed = value;
    }

    protected Transform target;
    public Transform Target
    {
        get => target;
        protected set => target = value;
    }

    [SerializeField] protected Transform player1;
    public Transform Player1
    {
        get => player1;
        set => player1 = value;
    }

    [SerializeField] protected Transform player2;
    public Transform Player2
    {
        get => player2;
        set => player2 = value;
    }

    protected virtual void Awake()
    {
        GetPlayerReferences();
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
        // Find player here
    }

    protected virtual Transform FindTarget()
    {
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
}
