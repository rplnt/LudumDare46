using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindController : MonoBehaviour {

    public float radius;

    public float Power { get { return radius / 2.0f; } }
    public Vector3 Direction { get { return transform.forward; } }

    private void Update() {
        DrawDebug();
    }

    void DrawDebug() {
        Debug.DrawLine(transform.position, transform.position + transform.forward * radius);
        //Debug.DrawRay(transform.position, transform.forward);
    }
}
