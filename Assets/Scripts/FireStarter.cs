using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireStarter : MonoBehaviour {
    private bool inWind = false;

    private Collider[] neighbours;

    float cutOffAngle = 15.0f;
    float maxSpread = 10.0f;

    Burner burner;

    private void Start() {
        burner = GetComponent<Burner>();
        if (burner == null) Debug.LogError(this.name + ": Missing burner");
        neighbours = new Collider[20];
    }

    private void Update() {
        if (!inWind) return;
        if (!burner.OnFire) return;

        Ignite();

    }

    void Ignite() {
        float angle, dist, heat;
        Vector2 windDir2d, targetDir2d;
        Vector3 dir;

        float found = FindNeighbours(maxSpread);
        for (int i = 0; i < found; i++) {
            Collider c = neighbours[i];
            if (!c.CompareTag("Tree")) continue;
            if (c.gameObject == gameObject) continue;

            dir = c.transform.position - transform.position;
            targetDir2d = new Vector2(dir.x, dir.z);
            windDir2d = new Vector2(burner.wind.Direction.x, burner.wind.Direction.z);
            
            angle = Vector2.Angle(windDir2d, targetDir2d);
            if (angle > cutOffAngle) continue;

            dist = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(c.transform.position.x, c.transform.position.z));

            heat = dist * burner.wind.Power * Time.deltaTime;

            //Debug.Log(c.name + ": Distance:" + Vector3.Distance(transform.position, c.transform.position) + " Angle:" + angle);
            c.SendMessage("AddHeat", heat, SendMessageOptions.DontRequireReceiver);
        }
    }

    private int FindNeighbours(float radius) {
        return Physics.OverlapSphereNonAlloc(transform.position, radius, neighbours);
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) return;
        inWind = true;
    }

    private void OnTriggerExit(Collider other) {
        if (!other.CompareTag("Player")) return;
        inWind = false;
    }
}
