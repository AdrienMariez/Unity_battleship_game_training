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

    private Transform ShipMapUITransform;
    private Text ShipMapUIDistance;
    private Text ShipMapUIText;
    private Slider ShipMapUISlider;

    const string RangeDisplayMeter = "{0} m";
    const string RangeDisplayKilometer = "{0} km";

    // [Header("Debug")]
    //     public bool debug = false;


    public void Init() {
        CameraPosition = GameObject.Find("MainCamera").transform;
        ShipUIObj = this.gameObject.transform.GetChild(1).GetComponent<Canvas>();
        ShipUITransform = this.gameObject.transform.GetChild(1).GetChild(0).transform;
        ShipUIDistance = ShipUITransform.Find("UnitDistance").GetComponent<Text>();
        ShipUIText = ShipUITransform.Find("UnitName").GetComponent<Text>();
        ShipUISlider = ShipUITransform.Find("HealthSlider").GetComponent<Slider>();

        ShipMapUITransform = this.gameObject.transform.GetChild(1).GetChild(1).transform;
        ShipMapUIDistance = ShipMapUITransform.Find("MapUnitDistance").GetComponent<Text>();
        ShipMapUIText = ShipMapUITransform.Find("MapUnitName").GetComponent<Text>();
        ShipMapUISlider = ShipMapUITransform.Find("MapHealthSlider").GetComponent<Slider>();

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
                ShipMapUIDistance.text = string.Format(RangeDisplayKilometer, distance);
            } else {
                ShipUIDistance.text = string.Format(RangeDisplayMeter, distance);
                ShipMapUIDistance.text = string.Format(RangeDisplayMeter, distance);
            }
            ShipUITransform.LookAt(CameraPosition.position);
        }
    }

    public void SetStartingHealth(float FullHP) {
        // Debug.Log (this.name+" - ShipUISlider -"+ShipUISlider);
        ShipUISlider.maxValue = FullHP;
        ShipUISlider.value = FullHP;
        
        ShipMapUISlider.maxValue = FullHP;
        ShipMapUISlider.value = FullHP;
    }
    public void SetCurrentHealth(float HP) { ShipUISlider.value = HP; ShipMapUISlider.value = HP; }
    public void SetName(string name) { ShipUIText.text = name; ShipMapUIText.text = name; }
    public void SetActive(bool activate) {
        Active = activate;
        ShipUIObj.enabled = Active;
    }
}