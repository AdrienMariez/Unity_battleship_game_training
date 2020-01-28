using UnityEngine;
using UnityEngine.UI;

public class ShipUI : MonoBehaviour {

    private bool Active = true;
    private Canvas ShipUIObj;
    private Transform ShipUITransform;
    private Text ShipUIDistance;
    private Text ShipUIText;
    private Slider ShipUISlider;
    private Transform CameraPosition;

    const string RangeDisplayMeter = "{0} m";
    const string RangeDisplayKilometer = "{0} km";

    // [Header("Debug")]
    //     public bool debug = false;


    public void Init() {
        CameraPosition = GameObject.Find("MainCamera").transform;
        ShipUIObj = this.gameObject.transform.GetChild(1).GetComponent<Canvas>();
        ShipUITransform = this.gameObject.transform.GetChild(1).transform;
        ShipUIDistance = ShipUITransform.Find("UnitDistance").GetComponent<Text>();
        ShipUIText = ShipUITransform.Find("UnitName").GetComponent<Text>();
        ShipUISlider = ShipUITransform.Find("HealthSlider").GetComponent<Slider>();
        ShipUIText.text = this.name;
    }

    private void FixedUpdate() {
        // if (debug)
        //     Debug.Log (this.name+" - ShipUISlider -"+ShipUISlider);
        if (Active) {
            float distance = (ShipUITransform.position - CameraPosition.position).magnitude;
            if (distance > 999) {
                distance = (Mathf.Round(distance / 100)) / 10f;
                ShipUIDistance.text = string.Format(RangeDisplayKilometer, distance);
            } else {
                ShipUIDistance.text = string.Format(RangeDisplayMeter, distance);
            }
            ShipUITransform.LookAt(CameraPosition.position);
        }
    }

    public void SetStartingHealth(float FullHP) {
        // Debug.Log (this.name+" - ShipUISlider -"+ShipUISlider);
        ShipUISlider.maxValue = FullHP;
        ShipUISlider.value = FullHP;
    }
    public void SetCurrentHealth(float HP) { ShipUISlider.value = HP; }
    public void SetName(string name) { ShipUIText.text = name; }
    public void SetActive(bool activate) {
        Active = activate;
        ShipUIObj.enabled = Active;
    }
}