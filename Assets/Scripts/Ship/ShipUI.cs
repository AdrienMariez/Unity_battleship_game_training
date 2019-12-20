using UnityEngine;
using UnityEngine.UI;
using Crest;

public class ShipUI : MonoBehaviour {

    private bool Active;
    private bool MapActive;
    private Transform ShipUITransform;
    private Text ShipUIText;
    private Transform CameraPosition;


    private void Start() {
        CameraPosition = GameObject.Find("MainCamera").transform;
        ShipUITransform = this.gameObject.transform.GetChild(1).GetChild(0).transform;
        ShipUIText = this.gameObject.transform.GetChild(1).GetChild(0).GetComponent<Text>();
        ShipUIText.text = this.name;
    }

    private void FixedUpdate() {
        if (Active)
            ShipUITransform.LookAt(CameraPosition.position);
    }

    public void SetMap(bool map) {
        MapActive = map;
    }
    public void SetActive(bool activate) {
        Active = activate;
        ShipUIText.enabled = Active;
    }
}