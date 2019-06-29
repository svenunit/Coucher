using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelExit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D c)
    {
        PlayerInput player = c.GetComponent<PlayerInput>();
        if (player != null)
        {
            
        }
    }
}
