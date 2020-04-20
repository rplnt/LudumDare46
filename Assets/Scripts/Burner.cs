using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burner : MonoBehaviour {

    public float mass;
    public float cooling;
    public bool startOnFire;

    public bool OnFire { get; private set; } = false;
    public bool Burned { get { return mass <= 0.0f; } }

    public float currentHeat;

    public bool inWind;

    GameObject fire;
    Material body;
    Color bodyColor;
    Material trunk;

    bool flickering = false;
    float heatFlickerTimer = 0.0f;

    public WindController wind { get; private set; }
    public GameManager gm { get; private set; }

    private void Start() {
        gm = FindObjectOfType<GameManager>();
        gm.RegisterTree();

        currentHeat = 0;
        if (startOnFire) {
            gm.TreeOnFire();
            OnFire = true;
        }

        fire = transform.Find("Fire").gameObject;
        fire.SetActive(OnFire);

        GameObject player = GameObject.FindWithTag("Player");
        wind = player.GetComponent<WindController>();
        if (wind == null) Debug.LogError(this.name + ": Cannot find player");



        // find tree body
        MeshRenderer mr = transform.Find("Model").GetComponent<MeshRenderer>();
        List<Material> mats = new List<Material>();
        mr.GetMaterials(mats);
        foreach(Material mat in mats) {
            if (mat.name.Contains("Tree")) {
                body = mat;
            }
            if (mat.name.Contains("Trunk")) {
                trunk = mat;
            }
        }

        ChangeBaseColor();
    }

    void ChangeBaseColor() {
        float h, s, v, noise;
        Color color = body.GetColor("_BaseColor");
        Color.RGBToHSV(color, out h, out s, out v);
        noise = Mathf.PerlinNoise(0.08f * transform.position.x, 0.08f * transform.position.z);
        bodyColor = Color.HSVToRGB(((h + h + noise) / 3.0f), s, v);
        body.SetColor("_BaseColor", bodyColor);
    }

    private void Update() {
        if (OnFire) {
            Burn();
        }

        if (OnFire && Burned) {
            Burnout();
        }

        if (!OnFire && !Burned && currentHeat >= 1.0f) {
            Ignite();
        }

        if (!Burned) {
            Cool();
            HeatFlicker();
        }
    }

    void Cool() {
        float bonus = 1.0f + (inWind ? wind.Power / 40.0f : 0.0f);
        currentHeat = Mathf.Clamp01(currentHeat - bonus * cooling * Time.deltaTime);
    }

    public void AddHeat(float heat) {
        if (Burned) return;
        if (OnFire) {
            mass -= heat;
            return;
        }

        currentHeat += heat;
    }

    private void Ignite() {
        fire.SetActive(true);
        OnFire = true;
        flickering = false;

        gm.TreeOnFire();
    }

    private void Burn() {
        mass -= Time.deltaTime;
    }

    private void HeatFlicker() {
        if (currentHeat <= 0.0f) {
            if (flickering) {
                body.SetColor("_BaseColor", bodyColor);
                flickering = false;
            }
            return;
        }
        flickering = true;
        Color redish = new Color(Mathf.Clamp01(bodyColor.r + currentHeat / 2.0f), bodyColor.g, bodyColor.b);
        body.SetColor("_BaseColor", redish);
    }

    private void Burnout() {
        OnFire = false;
        fire.SetActive(false);
        flickering = false;
        gm.TreeBurned();

        body.SetColor("_BaseColor", new Color(0.1f, 0.1f, 0.1f));
        trunk.SetColor("_BaseColor", new Color(0.1f, 0.1f, 0.1f));
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
