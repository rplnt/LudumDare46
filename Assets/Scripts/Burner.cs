using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burner : MonoBehaviour {

    public float mass;
    public float cooling;
    public bool startOnFire;

    public bool OnFire { get; private set; } = false;
    public bool Burned { get { return mass <= 0.0f; } }

    float currentHeat;
    float rateOfFire;

    GameObject fire;

    public WindController wind { get; private set; }

    private void Start() {
        if (startOnFire) OnFire = true;

        fire = transform.Find("Fire").gameObject;
        fire.SetActive(OnFire);

        GameObject player = GameObject.FindWithTag("Player");
        wind = player.GetComponent<WindController>();
        if (wind == null) Debug.LogError(this.name + ": Cannot find player");
    }

    private void Update() {
        if (OnFire) {
            Burn();
        }

        if (OnFire && Burned) {
            Burnout();
        }

        if (!OnFire && !Burned && currentHeat > 1.0f) {
            Ignite();
        }

        currentHeat = Mathf.Clamp01(currentHeat - cooling * Time.deltaTime);
    }

    public void AddHeat(float heat) {
        if (Burned) return;
        if (OnFire) {
            Burn();
            return;
        }

        Debug.Log("Heating up +" + heat);
        currentHeat += heat;
    }

    private void Ignite() {
        Debug.Log("Ignited");
        fire.SetActive(true);
        OnFire = true;
    }

    private void Burn() {
        // TODO rate of fire
        rateOfFire = 1.0f;
        mass -= rateOfFire * Time.deltaTime;
    }

    private void Burnout() {
        OnFire = false;
        fire.SetActive(false);

        transform.Find("Body").gameObject.SetActive(false);
    }
}
