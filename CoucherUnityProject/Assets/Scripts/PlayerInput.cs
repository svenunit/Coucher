using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
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
    bool _playerCanMove { get; set; }
   


    Vector2 _aimDirection;
    float _aimAngle;
    public float movementSpeed;
    public float rotationSpeed;
    public float dashDistance;
    public float dashCooldown;
    public Transform indicator;
    public LineRenderer dashlineP1;
    public EdgeCollider2D dashlineP1Collider;
    public EdgeCollider2D dashlineP2Collider;

    public LineRenderer dashlineP2;



    [Header("DEBUG")]
    public bool keyboardMovement = false;
   



    private void Start()
    {
        _playerCanMove = true;
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
        if(GetComponent<Rigidbody2D>().velocity == Vector2.zero)
        {
            _playerCanMove=true;
        }
        else
        {
            _playerCanMove = false;
        }
            

        if (keyboardMovement)
        {
            _verticalAxes = Input.GetAxis("VerticalP" + _playerNumber + "K") * movementSpeed * Time.deltaTime;
            _horizontalAxes = Input.GetAxis("HorizontalP" + _playerNumber + "K") * movementSpeed * Time.deltaTime;

        }
        else if(_playerCanMove)
        {

            _verticalAxes = Input.GetAxis("VerticalP" + _playerNumber) * movementSpeed * Time.deltaTime;
            _horizontalAxes = Input.GetAxis("HorizontalP" + _playerNumber) * movementSpeed * Time.deltaTime;



            _aimDirection = new Vector2(_turnH, _turnV);
            _aimAngle = Mathf.Atan2(_turnH, _turnV) * Mathf.Rad2Deg;





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

        if (_turnV != null && _turnH != null)
        {
            _turnV = Input.GetAxis("VerticalRightStickP" + _playerNumber);
            _turnH = Input.GetAxis("HorizontalRightStickP" + _playerNumber);

          
            if ((Mathf.Abs(_turnH) + Mathf.Abs(_turnV)) > 1)
            {
                indicator.transform.localPosition = new Vector3(_turnH * rotationSpeed, -1 * -_turnV * rotationSpeed, 0);
            }

        }


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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, (Vector3)_aimDirection * dashDistance, dashDistance);
        Debug.DrawLine(indicator.transform.position, hit.point, Color.white);
        Debug.Log(hit.collider);

        


        switch (_playerNumber)
        {
            case 1:
                if (Input.GetAxis("RTriggerP" + _playerNumber) > 0 && dashTimer == 0)
                {
                    if (!dashlineP1.gameObject.activeSelf)
                        dashlineP1.gameObject.SetActive(true);

                    if (!dashlineP1Collider.gameObject.activeSelf)
                        dashlineP1Collider.gameObject.SetActive(true);

                    //Destroy(currentDashLine);

                  
                   
                    
                    dashTimer = dashCooldown;
                    oldPosition = transform.position;

                    transform.Translate( (Vector3)_aimDirection * dashDistance);
                    // GetComponent<Rigidbody2D>().velocity=(Vector3)_aimDirection * dashDistance;

                   


                    newPosition = transform.position;
                    
                    dashlineP1.SetPosition(0, oldPosition);
                    dashlineP1.SetPosition(1, newPosition);

                    Vector2[] colliderPointsP1;

                    colliderPointsP1 = dashlineP1Collider.GetComponent<EdgeCollider2D>().points;
                    colliderPointsP1[0] = oldPosition;
                    colliderPointsP1[1] = newPosition;
                    dashlineP1Collider.GetComponent<EdgeCollider2D>().points = colliderPointsP1;




                }
                else
                {
                   
                    dashTimer -= Time.deltaTime;
                }
                break;
                ////Player 2 Movement
            case 2:
                if (Input.GetAxis("RTriggerP" + _playerNumber) > 0 && dashTimer == 0)
                {
                    if (!dashlineP2.gameObject.activeSelf)
                        dashlineP2.gameObject.SetActive(true);

                    if (!dashlineP2Collider.gameObject.activeSelf)
                        dashlineP2Collider.gameObject.SetActive(true);

                    //Destroy(currentDashLine);


                    dashTimer = dashCooldown;
                    oldPosition = transform.position;

                    transform.position += (Vector3)_aimDirection * dashDistance;

                    newPosition = transform.position;

                    dashlineP2.SetPosition(0, oldPosition);
                    dashlineP2.SetPosition(1, newPosition);

                    Vector2[] colliderPointsP2;

                    colliderPointsP2 = dashlineP2Collider.GetComponent<EdgeCollider2D>().points;
                    colliderPointsP2[0] = oldPosition;
                    colliderPointsP2[1] = newPosition;
                    dashlineP2Collider.GetComponent<EdgeCollider2D>().points = colliderPointsP2;




                }
                else
                {
                    dashTimer -= Time.deltaTime;
                }
                break;

        }

        if (dashTimer < 0)
        {
            dashTimer = 0;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }


    }
       
    
}
