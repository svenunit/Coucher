using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public abstract class Enemy : MonoBehaviour, IListener
{

    [Range(0.1f, 5f)] [SerializeField] protected float baseMoveSpeed = 1f;
    public float BaseMoveSpeed
    {
        get => baseMoveSpeed;
        protected set => Mathf.Clamp(value, 0, int.MaxValue);
    }
    [SerializeField] protected float moveSpeed;
    public float MoveSpeed
    {
        get => moveSpeed;
        protected set => Mathf.Clamp(value, 0, maxMoveSpeed);
    }

    public float moveSpeedIncreaseOnRecovery => BaseMoveSpeed * .5f;
    public float maxMoveSpeed => BaseMoveSpeed * 10f;
    public float recoveryDecreaseOnRecovery => RecoveryTime * .2f;
    public float minRecovery => .5f;

    [Range(1, 10)] [SerializeField] protected int startingHealth;
    public int StartingHealth
    {
        get => startingHealth;
        protected set => startingHealth = Mathf.Clamp(value, 0, int.MaxValue);
    }

    [Range(0.25f, 5f)] [SerializeField] protected float recoveryTime = 1f;
    public float RecoveryTime
    {
        get => recoveryTime;
        protected set => recoveryTime = Mathf.Clamp(value, minRecovery, int.MaxValue);
    }
    private float recoveryTimer = 0f;

    [SerializeField] protected Sprite hitByPlayer1Sprite;
    public Sprite HitByPlayer1Sprite
    {
        get => hitByPlayer1Sprite;
        protected set => hitByPlayer1Sprite = value;
    }

    [SerializeField] protected Sprite hitByPlayer2Sprite;
    public Sprite HitByPlayer2Sprite
    {
        get => hitByPlayer2Sprite;
        protected set => hitByPlayer2Sprite = value;
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

    protected SpriteRenderer stunnedEnemySpriteRenderer;
    public SpriteRenderer StunnedEnemySpriteRenderer
    {
        get => stunnedEnemySpriteRenderer;
        protected set => stunnedEnemySpriteRenderer = value;
    }

    private Sprite defaultSprite;

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

    protected Pathfinding pathfinding;
    public Pathfinding Pathfinding
    {
        get => pathfinding;
        protected set => pathfinding = value;
    }

    protected PathfindingGrid pathfindingGrid;
    public PathfindingGrid PathfindingGrid
    {
        get => pathfindingGrid;
        protected set => pathfindingGrid = value;
    }

    protected List<Vector2> pathToTarget;
    public List<Vector2> PathToTarget
    {
        get => pathToTarget;
        protected set => pathToTarget = value;
    }
    private int pathIndex = 0;

    protected virtual void Awake()
    {
        Alive = true;
        Animator = GetComponent<Animator>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        StunnedEnemySpriteRenderer = transform.Find("StunnedEnemySprite").GetComponent<SpriteRenderer>();
        defaultSprite = SpriteRenderer.sprite;
        DeathPS = transform.Find("DeathEffects").GetComponentInChildren<ParticleSystem>();
        GetPlayerReferences();
        Health = StartingHealth;
        StunnedGameobject = transform.Find("StunnedEnemyStars").gameObject;
        StunnedGameobject.SetActive(false);
        ExplosionGameobject = transform.Find("DeathEffects").gameObject;
        Explosionanimator = ExplosionGameobject.GetComponentInChildren<Animator>();
        ExplosionGameobject.SetActive(false);
        Pathfinding = FindObjectOfType<Pathfinding>();
        PathfindingGrid = FindObjectOfType<PathfindingGrid>();
        PathToTarget = new List<Vector2>();
        MoveSpeed = BaseMoveSpeed;
    }

    protected void OnEnable()
    {
        EventManager.PlayerWasHit.AddListener(this, OnPlayerWasHit);

    }

    protected void OnDisable()
    {
        EventManager.PlayerWasHit.RemoveListener(this);
    }

    protected virtual void Start()
    {
        Target = FindTarget();
    }

    protected virtual void FixedUpdate()
    {
        PursueTarget();
        HandleTimedRecovery();
    }

    private void OnTriggerEnter2D(Collider2D c)
    {
        if (Alive == false) return;
        CollidingPlayer = c.GetComponent<DashlineBehaviour>();
        if (CollidingPlayer != null && CollidingPlayer != PlayerHitByMostRecently)
        {
            PlayerHitByMostRecently = CollidingPlayer;
            OnHitByPlayerDash(1);
            // assign new sprite based on hit by which player.
            if (Alive == false) return;

            switch (CollidingPlayer.gameObject.tag)
            {
                case "DashCollider1":
                    StunnedEnemySpriteRenderer.sprite = HitByPlayer1Sprite;
                    break;
                case "DashCollider2":
                    StunnedEnemySpriteRenderer.sprite = HitByPlayer2Sprite;
                    break;
                default:
                    break;
            }
            StunnedEnemySpriteRenderer.enabled = true;
        }
    }

    private void OnDrawGizmos()
    {
        if (Target == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, Target.position);
        if (PathToTarget != null)
        {
            foreach (var pos in PathToTarget)
            {
                Gizmos.DrawWireCube(pos, Vector2.one * .25f);
            }
        }
    }

    private void OnPlayerWasHit(PlayerInput playerInput)
    {
        Target = FindTarget(randomTarget: true);
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

    protected virtual Transform FindTarget(bool randomTarget = false)
    {
        if (Player1 == null || Player2 == null) return null;
        if (randomTarget == true) return GetRandomTarget();
        else
        {
            // default: get closest player
            float dist1 = Vector2.Distance(transform.position, player1.position);
            if (dist1 < Vector2.Distance(transform.position, player2.position))
                return Player1;
            else return Player2;
        }
    }

    protected Transform GetRandomTarget()
    {
        bool targetPlayer1 = UnityEngine.Random.value > .5 ? true : false;
        return targetPlayer1 ? Player1 : Player2;
    }

    protected virtual void PursueTarget()
    {
        if (Target == null || Alive == false) return;
        transform.position = Vector3.MoveTowards(transform.position, Target.position, moveSpeed * Time.fixedDeltaTime);
        //if ((Vector2)transform.position == PathToTarget[pathIndex])
        //{
        //    pathIndex++;
        //}
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
        {
            SoundManager.instance.PlayAudioOnSource(SoundManager.instance.enemyHit, SoundManager.instance.audioSourceSFXEnemy, 0, 0);
            Stun();
        }
        else
        {
            TakeDamage(damageAmount);
        }
    }

    public virtual void Recover()
    {
        if (Stunned == false) return;
        StunnedEnemySpriteRenderer.sprite = null;
        StunnedEnemySpriteRenderer.enabled = false;
        Stunned = false;
        StunnedGameobject.SetActive(Stunned);
        Target = FindTarget();
        Animator.SetFloat("speedMod", 1f);
        recoveryTimer = 0f;
        MoveSpeed += moveSpeedIncreaseOnRecovery;
        RecoveryTime -= recoveryDecreaseOnRecovery;
        PlayerHitByMostRecently = null;
    }

    private void HandleTimedRecovery()
    {
        if (Stunned == true)
        {
            recoveryTimer += Time.fixedDeltaTime;
            if (recoveryTimer >= RecoveryTime)
            {
                Recover();
            }
        }
    }

    protected virtual void Die()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        SoundManager.instance.PlayAudioOnSource(SoundManager.instance.enemyDeath, SoundManager.instance.audioSourceSFXEnemy, 0, 0);
        Alive = false;
        StunnedEnemySpriteRenderer.sprite = null;
        StunnedEnemySpriteRenderer.enabled = false;
        SpriteRenderer.enabled = false;

        StunnedGameobject.SetActive(false);
        //DeathPS.Play();
        ExplosionGameobject.SetActive(true);
        Explosionanimator.SetTrigger("Die");
        EventManager.EnemyDied.RaiseEvent(this);
        Destroy(gameObject, maxLifeTimeDeathParticles);
    }
}
