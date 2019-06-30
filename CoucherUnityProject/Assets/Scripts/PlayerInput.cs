using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInput : MonoBehaviour, IListener
{

    // [Header("HP")]
    private int maxHP = 100;

    public static int hp;

    private Slider hpSlider;

    [SerializeField]
    int _playerNumber;
    [SerializeField]
    float dashLength;
    float _verticalAxes;
    float _horizontalAxes;
    float _turnV;
    float _turnH;
    Quaternion _aimRotation;
    float dashTimer;
    Vector3 oldPosition;
    Vector3 newPosition;



    Vector2 _aimDirection;
    float _aimAngle;
    public float movementSpeed;
    public float rotationSpeed;
    public float dashDistance;
    public float dashDistanceDefault = 5;
    public float dashCooldown;
    public Transform indicator;
    public LineRenderer dashlineP1;
    public EdgeCollider2D dashlineP1Collider;
    public EdgeCollider2D dashlineP2Collider;
    public bool _playerCanMove;


    public LineRenderer dashlineP2;

    public LayerMask wallLayer;

    private Vector3? dashRayHitpoint;

    private Rigidbody2D body2d;
    private SpriteRenderer spriteRenderer;

    [Header("DEBUG")]
    public bool keyboardMovement = false;

    private Coroutine dashCoroutine;
    public bool InDash => dashCoroutine != null;

    private bool dashing;
    public bool Dashing
    {
        get { return dashing; }
        set
        {
            if (dashing != value)
            {
                dashing = value;
                if (dashing == false)
                {
                    newPosition = transform.position;

                    switch (getPlayerNumber())
                    {
                        case 1:
                            SoundManager.instance.PlayAudioOnSource(SoundManager.instance.player1Dashing, SoundManager.instance.audioSourceSFX, 0, 0);
                            dashlineP1Collider.enabled = true;
                            dashlineP1.SetPosition(0, oldPosition);
                            dashlineP1.SetPosition(1, newPosition);

                            Vector2[] colliderPointsP1;

                            colliderPointsP1 = dashlineP1Collider.GetComponent<EdgeCollider2D>().points;
                            colliderPointsP1[0] = oldPosition;
                            colliderPointsP1[1] = newPosition;
                            dashlineP1Collider.GetComponent<EdgeCollider2D>().points = colliderPointsP1;
                            break;
                        case 2:
                            SoundManager.instance.PlayAudioOnSource(SoundManager.instance.player2Dashing, SoundManager.instance.audioSourceSFX, 0, 0);
                            dashlineP2Collider.enabled = true;
                            dashlineP2.SetPosition(0, oldPosition);
                            dashlineP2.SetPosition(1, newPosition);

                            Vector2[] colliderPointsP2;

                            colliderPointsP2 = dashlineP2Collider.GetComponent<EdgeCollider2D>().points;
                            colliderPointsP2[0] = oldPosition;
                            colliderPointsP2[1] = newPosition;
                            dashlineP2Collider.GetComponent<EdgeCollider2D>().points = colliderPointsP2;
                            break;
                        default:
                            break;
                    }
                    Debug.LogWarning("DASH ENDED");

                    Invoke("DisableLineCollider", .1f);
                }
            }
        }
    }

    private Coroutine takeDamageCoroutine;
    public bool IFramesActive => takeDamageCoroutine != null;

    private void OnEnable()
    {
        EventManager.GameOver.AddListener(this, OnGameOver);
    }

    private void OnDisable()
    {
        EventManager.GameOver.RemoveListener(this);
    }

    private void Start()
    {
        body2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        hpSlider = GameObject.Find("Healthbar").GetComponent<Slider>();
        _playerCanMove = true;
        hp = maxHP;
        //DEBUG
        /*
        string[] controller = Input.GetJoystickNames();
        for(int i = 0; i < controller.Length; i++)
        {
            print(i + " " + controller[i]);
        }*/
        if (_playerNumber == 0)
            _playerNumber++;
    }
    private void FixedUpdate()
    {
        Dashing = body2d.velocity.magnitude > 4f;
        if (Dashing == true ||_playerCanMove == false) return;

        if (keyboardMovement)
        {
            _verticalAxes = Input.GetAxis("VerticalP" + _playerNumber + "K") * movementSpeed * Time.deltaTime;
            _horizontalAxes = Input.GetAxis("HorizontalP" + _playerNumber + "K") * movementSpeed * Time.deltaTime;

        }
        else if (_playerCanMove)
        {

            _verticalAxes = Input.GetAxis("VerticalP" + _playerNumber) * movementSpeed * Time.deltaTime;
            _horizontalAxes = Input.GetAxis("HorizontalP" + _playerNumber) * movementSpeed * Time.deltaTime;

            _turnV = Input.GetAxis("VerticalRightStickP" + _playerNumber);
            _turnH = Input.GetAxis("HorizontalRightStickP" + _playerNumber);

            _aimDirection = new Vector2(_turnH, _turnV);
            _aimAngle = Mathf.Atan2(_turnV, _turnH) * Mathf.Rad2Deg;

            if ((Mathf.Abs(_turnH) + Mathf.Abs(_turnV)) > 1)
            {
                indicator.transform.localPosition = new Vector3(_turnH * rotationSpeed, -1 * -_turnV * rotationSpeed, 0);
            }



        }
        HandleDash();
        transform.Translate(_horizontalAxes, _verticalAxes, 0, Space.World);
        /* if (_turnV != 0 && _turnH != 0)

         {
             _aimRotation = Quaternion.AngleAxis(_aimAngle, Vector3.forward);
             transform.rotation = Quaternion.Slerp(transform.rotation, _aimRotation, rotationSpeed * Time.deltaTime);
         }
         //Debug.Log(Input.GetAxis("VerticalRightStickP" + _playerNumber) + "|"+Input.GetAxis("HorizontalRightStickP" + _playerNumber);
         */

        //if (_turnV != null && _turnH != null)
        //{
        //    _turnV = Input.GetAxis("VerticalRightStickP" + _playerNumber);
        //    _turnH = Input.GetAxis("HorizontalRightStickP" + _playerNumber);


        //    if ((Mathf.Abs(_turnH) + Mathf.Abs(_turnV)) > 1)
        //    {
        //        indicator.transform.localPosition = new Vector3(_turnH * rotationSpeed, -1 * -_turnV * rotationSpeed, 0);
        //    }

        //}


    }

    private void OnTriggerEnter2D(Collider2D c)
    {
        Enemy enemy = c.GetComponent<Enemy>();
        // Hit by enemy
        if (enemy != null && IFramesActive == false && Dashing == false)
        {
            takeDamageCoroutine = StartCoroutine(TakeDamageRoutine());
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (dashRayHitpoint != null)
        {
            Gizmos.DrawWireSphere(dashRayHitpoint.Value, .25f);
        }
    }

    private void OnGameOver()
    {
        _playerCanMove = false;
        body2d.velocity = Vector2.zero;
        body2d.isKinematic = true;
    }

    public int getPlayerNumber()
    {
        return _playerNumber;
    }

    public void setPlayerNumber(int playerNumber)
    {
        _playerNumber = playerNumber;
    }

    private void HandleDash()
    {

        dashDistance = dashDistanceDefault;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, (indicator.position - transform.position).normalized, dashDistance, wallLayer);
        Debug.DrawRay(transform.position, (indicator.position - transform.position).normalized * dashDistance);
        //Debug.DrawLine(transform.position, (indicator.position - transform.position) * dashDistance, Color.white);
        if (hit.collider != null)
        {
            dashRayHitpoint = hit.point;
            var distToHitPoint = Vector2.Distance(transform.position, hit.point);
            dashDistance = distToHitPoint;
        }
        else dashRayHitpoint = null;

        switch (_playerNumber)
        {

            case 1:
                if (Input.GetAxis("RTriggerP" + _playerNumber) > 0 && dashTimer <= 0)
                {
                    if (Dashing == false) Dashing = true;
                    if (!dashlineP1.gameObject.activeSelf)
                        dashlineP1.gameObject.SetActive(true);

                    if (!dashlineP1Collider.gameObject.activeSelf)
                        dashlineP1Collider.gameObject.SetActive(true);

                 

                    dashTimer = dashCooldown;
                    oldPosition = transform.position;
                    body2d.AddForce(_aimDirection * 60f, ForceMode2D.Impulse);

                }
                else
                {

                    dashTimer -= Time.deltaTime;
                }
                break;
            ////Player 2 Movement
            case 2:
                if (Input.GetAxis("RTriggerP" + _playerNumber) > 0 && dashTimer <= 0)
                {
                    if (Dashing == false) Dashing = true;
                    if (!dashlineP2.gameObject.activeSelf)
                        dashlineP2.gameObject.SetActive(true);

                    if (!dashlineP2Collider.gameObject.activeSelf)
                        dashlineP2Collider.gameObject.SetActive(true);

                   


                    dashTimer = dashCooldown;
                    oldPosition = transform.position;

                    body2d.AddForce(_aimDirection * 60f, ForceMode2D.Impulse);

                }
                else
                {
                    dashTimer -= Time.deltaTime;
                }
                break;
        }
    }

    private IEnumerator DashRoutine(Vector2 trgPos)
    {
        Debug.LogWarning("DashRoutine");
        //yield return StartCoroutine(Utility.MoveGameObjectRoutine(transform, trgPos, 1f));
        while ((Vector2)transform.position != trgPos)
        {
            transform.position = Vector2.MoveTowards(transform.position, trgPos, .1f);
            yield return null;
        }
        dashCoroutine = null;
    }

    private IEnumerator TakeDamageRoutine()
    {
        hp -= 25;
        spriteRenderer.color = Color.red;
        hpSlider.value = hp;
        if (hp <= 0)
        {
            EventManager.GameOver.RaiseEvent();
            yield break;
        }
        for (int i = 0; i < 9; i++)
        {
            spriteRenderer.color = i % 2 == 0 ? Color.white : Color.red;
            yield return new WaitForSeconds(.05f);
        }
        takeDamageCoroutine = null;
    }

    private void DisableLineCollider()
    {
        switch (getPlayerNumber())
        {
            case 1:
                dashlineP1Collider.enabled = false;
                break;
            case 2:
                dashlineP2Collider.enabled = false;
                break;
            default:
                break;
        }
    }

}
