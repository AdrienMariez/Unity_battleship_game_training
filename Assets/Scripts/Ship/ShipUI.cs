using UnityEngine;
using UnityEngine.UI;
using Crest;

public class ShipUI : MonoBehaviour {

    private bool Active;
    private bool MapActive;
    private Transform ShipUITransform;
    private Text ShipUIText;
    private GameObject ShipUISliderObject;
    private Slider ShipUISlider;
    private Transform CameraPosition;
    // private float StartingHealth;
    // private float CurrentHealth;


    private void Start() {
        CameraPosition = GameObject.Find("MainCamera").transform;
        ShipUITransform = this.gameObject.transform.GetChild(1).transform;
        ShipUIText = this.gameObject.transform.GetChild(1).GetChild(0).GetComponent<Text>();
        ShipUISliderObject = ShipUITransform.GetChild(1).gameObject;
        ShipUISlider = ShipUITransform.GetChild(1).GetComponent<Slider>();
        ShipUIText.text = this.name;
    }

    private void FixedUpdate() {
        if (Active) {
            // ShipUIText.text = string.Format("{2}\n{0}/{1}", CurrentHealth,StartingHealth,this.name);
            ShipUITransform.LookAt(CameraPosition.position);
        }
    }

    public void SetStartingHealth(float FullHP) {
        // StartingHealth = FullHP;
        ShipUISlider.maxValue = FullHP;
        ShipUISlider.value = FullHP;
    }
    public void SetCurrentHealth(float HP) {
        // CurrentHealth = HP;
        ShipUISlider.value = HP;
    }
    public void SetMap(bool map) {
        MapActive = map;
    }
    public void SetActive(bool activate) {
        Active = activate;
        ShipUIText.enabled = Active;
        ShipUISliderObject.SetActive(Active);
    }
}