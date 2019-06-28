using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    int _playerNumber;

    float _verticalAxes;
    float _horizontalAxes;
    public float movementSpeed;

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
            transform.Translate(_horizontalAxes, _verticalAxes, 0);
        }
        else
        {

            _verticalAxes = Input.GetAxis("VerticalP" + _playerNumber) * movementSpeed * Time.deltaTime;
            _horizontalAxes = Input.GetAxis("HorizontalP" + _playerNumber) * movementSpeed * Time.deltaTime;
            transform.Translate(_horizontalAxes, _verticalAxes, 0);


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

}
