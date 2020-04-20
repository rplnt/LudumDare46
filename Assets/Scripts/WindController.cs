using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindController : MonoBehaviour {

    public float radius;
    public float movementSpeed;
    public float rotationSpeed;

    public float minRadius, maxRadius;
    public float baseWindPower;

    public float Power { get { return baseWindPower + baseWindPower * (maxRadius / radius); } }
    public Vector3 Direction { get { return transform.forward; } }

    float sphereColliderBonus = 3.0f;
    SphereCollider sphere;
    ParticleSystem.ShapeModule visual;

    Bounds worldBounds;

    private void Start() {
        sphere = GetComponent<SphereCollider>();
        visual = GetComponentInChildren<ParticleSystem>().shape;

        sphere.radius = radius + sphereColliderBonus;
        FindBounds();
    }

    private void Update() {
        Rotate();
        Move();
        Zoom();
    }

    void FindBounds() {
        GameObject go = GameObject.FindWithTag("World");
        GameObject world = go.transform.Find("Plane").gameObject;

        MeshRenderer mr = world.GetComponent<MeshRenderer>();
        worldBounds = mr.bounds;
    }

    void Move() {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        transform.Translate(new Vector3(x * movementSpeed * Time.deltaTime, 0, y * movementSpeed * Time.deltaTime), Space.World);

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, worldBounds.min.x, worldBounds.max.x),
            transform.position.y,
            Mathf.Clamp(transform.position.z, worldBounds.min.z, worldBounds.max.z)
        );
    }

    void Rotate() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo)) {
            Vector3 target = new Vector3(hitInfo.point.x, transform.position.y, hitInfo.point.z);
            Debug.DrawLine(transform.position, transform.forward * 10.0f, Color.blue);
            Debug.DrawLine(transform.position, target, Color.red);
            Vector3 direction = Vector3.RotateTowards(transform.forward, target - transform.position, Mathf.Deg2Rad * rotationSpeed * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    void Zoom (){
        float scroll = -Input.mouseScrollDelta.y;
        float newRadius = Mathf.Clamp(radius + ((radius/10.0f) * scroll), minRadius, maxRadius);

        radius = newRadius;
        sphere.radius = newRadius + sphereColliderBonus;
        visual.radius = newRadius;        

        //transform.localScale = new Vector3(transform.localScale.x + zoom, transform.localScale.y + zoom, transform.localScale.z + zoom);
    }
}
