using UnityEngine;
using UnityEngine.UI;

public class ShipUI : MonoBehaviour {

    private bool Active = true;
    private Transform ShipUITransform;
    private Text ShipUIDistance;
    private Text ShipUIText;
    private GameObject ShipUISliderObject;
    private Slider ShipUISlider;
    private Transform CameraPosition;

    // [Header("Debug")]
    //     public bool debug = false;


    public void Init() {
        CameraPosition = GameObject.Find("MainCamera").transform;
        ShipUITransform = this.gameObject.transform.GetChild(1).transform;
        ShipUIText = this.gameObject.transform.GetChild(1).GetChild(0).GetComponent<Text>();
        ShipUISliderObject = ShipUITransform.GetChild(1).gameObject;
        ShipUISlider = ShipUITransform.GetChild(1).GetComponent<Slider>();
        ShipUIText.text = this.name;
    }

    private void FixedUpdate() {
        // if (debug)
        //     Debug.Log (this.name+" - ShipUISlider -"+ShipUISlider);
        if (Active) {
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
        ShipUIText.enabled = Active;
        ShipUISliderObject.SetActive(Active);
    }
}