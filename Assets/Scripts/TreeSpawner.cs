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
        Collider[] c = new Collider[1];
        int spawned = 0;
        for(int n=0;n<count;n++) {
            Vector3 pos = GetSpawnCandidate();
            if (Physics.OverlapSphereNonAlloc(pos, 4.25f, c) > 0) {
                continue;
            }

            Instantiate(tree, pos, Quaternion.Euler(0, Random.Range(0, 360), 0), transform);
            spawned++;
        }
        Debug.Log("Spawning " + spawned + " trees.");
    }

    Vector3 GetSpawnCandidate() {
        for (int n=0;n<100;n++) {
            float x = Random.Range(bounds.min.x, bounds.max.x);
            float y = Random.Range(bounds.min.z, bounds.max.z);
            float value = Mathf.PerlinNoise(noiseOffset + x * noiseScale, noiseOffset + y * noiseScale);
            
            // increase chance of spawn with each attempt until we pick anything
            if ((value + n * 0.01) > Random.Range(spawnCutoff, 1.0f)) {
                return new Vector3(x, 0, y);
            }
        }

        Debug.LogError("Can't find spawn position (invalid noise settings?");
        return Vector3.zero;
    }
}