using System.Collections;
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

    [Range(1, 10)] [SerializeField] protected int startingHealth;
    public int StartingHealth
    {
        get => startingHealth;
        protected set => startingHealth = Mathf.Clamp(value, 0, int.MaxValue);
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

    protected bool stunned;
    public bool Stunned
    {
        get => stunned;
        protected set => stunned = value;
    }

    protected bool alive;
    public bool Alive
    {
        get => alive;
        protected set => alive = value;
    }

    protected SpriteRenderer spriteRenderer;
    public SpriteRenderer SpriteRenderer
    {
        get => spriteRenderer;
        protected set => spriteRenderer = value;
    }

    protected GameObject stunnedGameobject;
    public GameObject StunnedGameobject
    {
        get => stunnedGameobject;
        protected set => stunnedGameobject = value;
    }

    protected GameObject explosionGameobject;
    public GameObject ExplosionGameobject
    {
        get => explosionGameobject;
        protected set => explosionGameobject = value;
    }

    protected Animator explosionanimator;
    public Animator Explosionanimator
    {
        get => explosionanimator;
        protected set => explosionanimator = value;
    }

    protected Animator animator;
    public Animator Animator
    {
        get => animator;
        protected set => animator = value;
    }

    protected ParticleSystem deathPS;
    public ParticleSystem DeathPS
    {
        get => deathPS;
        protected set => deathPS = value;
    }

    public float maxLifeTimeDeathParticles => DeathPS.main.startLifetime.constantMax;

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

    protected DashlineBehaviour collidingPlayer;
    public DashlineBehaviour CollidingPlayer
    {
        get => collidingPlayer;
        protected set => collidingPlayer = value;
    }

    protected DashlineBehaviour playerHitByMostRecently;
    public DashlineBehaviour PlayerHitByMostRecently
    {
        get => playerHitByMostRecently;
        protected set => playerHitByMostRecently = value;
    }


    protected virtual void Awake()
    {
        Alive = true;
        Animator = GetComponent<Animator>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        DeathPS = transform.Find("DeathEffects").GetComponentInChildren<ParticleSystem>();
        GetPlayerReferences();
        Health = StartingHealth;
        StunnedGameobject = transform.Find("StunnedEnemy").gameObject;
        StunnedGameobject.SetActive(false);
        ExplosionGameobject = transform.Find("DeathEffects").gameObject;
        Explosionanimator = ExplosionGameobject.GetComponentInChildren<Animator>();
        ExplosionGameobject.SetActive(false);
    }

    protected virtual void Start()
    {
        Target = FindTarget();
    }

    protected virtual void FixedUpdate()
    {
        PursueTarget();
    }

    private void OnTriggerEnter2D(Collider2D c)
    {
        CollidingPlayer = c.GetComponent<DashlineBehaviour>();
        if (CollidingPlayer != null && CollidingPlayer != PlayerHitByMostRecently)
        {
            PlayerHitByMostRecently = CollidingPlayer;
            OnHitByPlayerDash(1);
        }
        //else if (CollidingPlayer != null && CollidingPlayer == PlayerHitByMostRecently)
        //{
        //    print("Hit by the same player again!");
        //}
        //else if (CollidingPlayer == null)
        //{
        //    print("CollidingPlayer == null");
        //}
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
        if (Target == null || Alive == false) return;
        transform.position = Vector3.MoveTowards(transform.position, Target.position, moveSpeed * Time.fixedDeltaTime);
    }

    public virtual void TakeDamage(int amount)
    {
        Health -= Mathf.Abs(amount);
    }

    public virtual void Stun()
    {
        if (Stunned == true) return;
        Stunned = true;
        StunnedGameobject.SetActive(Stunned);
        Target = null;
        Animator.SetFloat("speedMod", 0f);
    }

    public virtual void OnHitByPlayerDash(int damageAmount)
    {
        if (Stunned == false)
            Stun();
        else
            TakeDamage(damageAmount);
    }

    public virtual void Recover()
    {
        if (Stunned == false) return;
        Stunned = false;
        StunnedGameobject.SetActive(Stunned);
        Target = FindTarget();
        Animator.SetFloat("speedMod", 1f);
    }

    protected virtual void Die()
    {
        Alive = false;
        SpriteRenderer.enabled = false;
        //DeathPS.Play();
        ExplosionGameobject.SetActive(true);
        Explosionanimator.SetTrigger("Die");
        EventManager.EnemyDied.RaiseEvent(this);
        Destroy(gameObject, maxLifeTimeDeathParticles);
    }
}
