using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    int _playerNumber;

    float _verticalAxes;
    float _horizontalAxes;
    float _turnV;
    float _turnH;
    Quaternion _aimRotation;
    

    Vector2 _aimDirection;
    float _aimAngle;
    public float movementSpeed;
    public float rotationSpeed;

    [Header("DEBUG")]
    public bool keyboardMovement = false;
   



    private void Start()
    {
        string[] controller = Input.GetJoystickNames();
        for(int i = 0; i < controller.Length; i++)
        {
            print(i + " " + controller[i]);
        }
        if (_playerNumber == 0)
            _playerNumber++;
    }
    private void FixedUpdate()
    {

        if (keyboardMovement)
        {
            _verticalAxes = Input.GetAxis("VerticalP" + _playerNumber+"K") * movementSpeed * Time.deltaTime;
            _horizontalAxes = Input.GetAxis("HorizontalP" + _playerNumber+"K")  * movementSpeed * Time.deltaTime;
            
        }
        else
        {

            _verticalAxes = Input.GetAxis("VerticalP" + _playerNumber) * movementSpeed * Time.deltaTime;
            _horizontalAxes = Input.GetAxis("HorizontalP" + _playerNumber) * movementSpeed * Time.deltaTime;

            _turnV = Input.GetAxis("VerticalRightStickP" + _playerNumber);
            _turnH = Input.GetAxis("HorizontalRightStickP" + _playerNumber);

            _aimDirection = new Vector2(_turnH, _turnV);
            _aimAngle = Mathf.Atan2(_turnH, _turnV) * Mathf.Rad2Deg;



        }
     
        transform.Translate(_horizontalAxes, _verticalAxes, 0, Space.World);
        if (_turnV != 0 && _turnH != 0)

        {
            _aimRotation = Quaternion.AngleAxis(_aimAngle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, _aimRotation, rotationSpeed * Time.deltaTime);
        }
        //Debug.Log(Input.GetAxis("VerticalRightStickP" + _playerNumber) + "|"+Input.GetAxis("HorizontalRightStickP" + _playerNumber);

    }
    public int getPlayerNumber()
    {
        return _playerNumber;
    }

    public void setPlayerNumber(int playerNumber)
    {
        _playerNumber = playerNumber;
    }

}
