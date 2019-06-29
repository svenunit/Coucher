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
        {
            Debug.Log(collision.GetComponent<Enemy>().Health);

            collision.GetComponent<Enemy>().OnHitByPlayerDash(1);
            Debug.Log(collision.GetComponent<Enemy>().Health);

        }

            Debug.Log(collision.tag+"|"+collision.name);
       
    }
}

