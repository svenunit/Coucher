using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashlineBehaviour : MonoBehaviour
{
    private void Awake()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();

        Mesh mesh = new Mesh();
        lineRenderer.BakeMesh(mesh, true);
        meshCollider.sharedMesh = mesh;

       
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.name);
    }
}

