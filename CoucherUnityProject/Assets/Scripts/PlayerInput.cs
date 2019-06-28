using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    int PlayerNumber;

    float _verticalAxes;
    float _horizontalAxes;
    public float movementSpeed;



    private void Start()
    {
        string[] controller = Input.GetJoystickNames();
        for(int i = 0; i < controller.Length; i++)
        {
            print(i + " " + controller[i]);
        }
    }
    private void FixedUpdate()
    {
        _verticalAxes = Input.GetAxis("VerticalP" + PlayerNumber)*movementSpeed*Time.deltaTime;
        _horizontalAxes = Input.GetAxis("HorizontalP" + PlayerNumber)*movementSpeed*Time.deltaTime;

        transform.Translate(_horizontalAxes, _verticalAxes,0);

    }
}
