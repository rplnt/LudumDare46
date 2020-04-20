using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TreeSpawner : MonoBehaviour {
    public GameObject tree;
    public int count;
    public Bounds bounds;

    float noiseOffset;

    [Range(0.0f, 1.0f)]
    public float spawnCutoff;
    [Range(0.0f, 1.0f)]
    public float noiseScale;

    private void Awake() {
    }

    private void Start() {
        noiseOffset = Random.Range(100.0f, 1000.0f);
        SpawnTrees();
    }

    void SpawnTrees() {
        Debug.Log("Spawning " + count + " trees.");
        int spawned = 0;
        int missed = 0;

        Collider[] c = new Collider[1];
        while (spawned < count && missed < count) {
            Vector3 pos = GetSpawnCandidate();

            // avoid spawning over other trees
            if (Physics.OverlapSphereNonAlloc(pos, 4.25f, c, LayerMask.GetMask("Trees")) > 0) {
                missed++;
                continue;
            }
            
            GameObject go = Instantiate(tree, pos, Quaternion.identity, transform);
            float scale = Random.Range(0.8f, 1.3f);
            go.transform.localScale = new Vector3(scale, scale, scale);
            go.name = "Tree [" + (int)pos.x + ", " + (int)pos.z + "]";
            go.transform.localRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            Burner burner = go.GetComponent<Burner>();
            if (burner == null) Debug.LogError("Tree without a burner: " + go.name);
            burner.mass = burner.mass + Random.Range(0.0f, 10.0f) * scale;
            spawned++;
        }
        Debug.Log("Spawned " + spawned + " trees (" + missed + " misses)");
    }

    Vector3 GetSpawnCandidate() {
        for (int n=0;n<100;n++) {
            float x = Random.Range(bounds.min.x, bounds.max.x);
            float z = Random.Range(bounds.min.z, bounds.max.z);
            float y = 0.0f;

            // avoid obstacles (water)
            if (Physics.Raycast(new Vector3(x, 10, z), Vector3.down, out RaycastHit hitInfo, 20.0f, ~LayerMask.NameToLayer("Ground"))) {
                if (hitInfo.transform.CompareTag("Water")) {
                    continue;
                }
                y = hitInfo.point.y;
            }

            float value = Mathf.PerlinNoise(noiseOffset + x * noiseScale, noiseOffset + z * noiseScale);

            // increase chance of spawn with each attempt until we pick anything
            if ((value + n * 0.02) > Random.Range(spawnCutoff, 1.0f)) {
                return new Vector3(x, y, z);
            }
        }

        Debug.LogError("Can't find spawn position (invalid noise settings?");
        return Vector3.zero;
    }
}