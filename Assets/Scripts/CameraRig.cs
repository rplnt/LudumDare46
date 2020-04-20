using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour {
    public Transform player;
    float offsetX, offsetZ;

    Bounds worldBounds;

    private void Start() {
        player = GameObject.FindWithTag("Player").transform;
        offsetX = player.position.x - transform.position.x;
        offsetZ = player.position.z - transform.position.z;

        FindBounds();
    }

    void FindBounds() {
        GameObject go = GameObject.FindWithTag("World");
        GameObject world = go.transform.Find("Plane").gameObject;

        MeshRenderer mr = world.GetComponent<MeshRenderer>();
        worldBounds = mr.bounds;
    }

    private void Update() {
        float x = Mathf.Clamp(player.position.x + offsetX, worldBounds.min.x + 52.0f, worldBounds.max.x - 52.0f);
        float z = Mathf.Clamp(player.position.z + offsetZ, worldBounds.min.z + 35.0f, worldBounds.max.z - 45.0f);
        transform.position = new Vector3(x, transform.position.y, z );
    }
}
