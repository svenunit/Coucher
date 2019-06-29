using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashlineBehaviour : MonoBehaviour
{
    private void Awake()
    {
       

       
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
            collision.GetComponent<Enemy>().Stun();
    }
}

