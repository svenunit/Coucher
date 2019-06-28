using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private void Update()
    {
        if (tag == "Player1")
            transform.position = new Vector2(Input.GetAxis("HorizontalP1"), Input.GetAxis("VerticalP1"));

        Debug.Log(Input.GetAxis("HorizontalP1"));
    }
}
