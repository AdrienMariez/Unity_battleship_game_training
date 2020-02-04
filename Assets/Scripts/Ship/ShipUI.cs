using UnityEngine;
using UnityEngine.UI;

public class ShipUI : MonoBehaviour {
    private bool Dead = false;
    private bool Active = true;
    private bool MapActive = false;
    private GameObject PlayerCanvas;
    private GameObject PlayerMapCanvas;
    private Camera Cam;
    private Camera MapCam;

    public GameObject m_UnitName;
        private GameObject m_UnitNameInstance;
    public GameObject m_UnitDistance;
        private GameObject m_UnitDistanceInstance;
    public GameObject m_UnitHealth;
        private GameObject m_UnitHealthInstance;
    public GameObject m_MapUnitName;
        private GameObject m_MapUnitNameInstance;
    public GameObject m_MapUnitDistance;
        private GameObject m_MapUnitDistanceInstance;
    public GameObject m_MapUnitHealth;
        private GameObject m_MapUnitHealthInstance;

    private string Name;

    const string RangeDisplayMeter = "{0} m";
    const string RangeDisplayKilometer = "{0} km";

    private Vector3 ScreenPos;

    // [Header("Debug")]
    //     public bool debug = false;


    public void Init() {
        // ShipUIText.text = this.name;
        Cam = GameObject.Find("MainCamera").GetComponentInChildren<Camera>();
        MapCam = GameObject.Find("MapCamera").GetComponentInChildren<Camera>();
    }

    private void FixedUpdate() {
        // if (debug)
        //     Debug.Log (this.name+" - ShipUISlider -"+ShipUISlider);
        // if (Active) {
        //     float distance = (ShipUITransform.position - CameraPosition.position).magnitude;
        //     if (distance > 999) {
        //         distance = (Mathf.Round(distance / 100)) / 10f;
        //         ShipUIDistance.text = string.Format(RangeDisplayKilometer, distance);
        //         ShipMapUIDistance.text = string.Format(RangeDisplayKilometer, distance);
        //     } else {
        //         ShipUIDistance.text = string.Format(RangeDisplayMeter, Mathf.Round(distance));
        //         ShipMapUIDistance.text = string.Format(RangeDisplayMeter, Mathf.Round(distance));
        //     }
        //     ShipUITransform.LookAt(CameraPosition.position);
        //     ShipMapUITransform.eulerAngles = new Vector3(-90.0f, 180.0f, 0.0f);
        // }
        // if (Active) {
        //     m_UnitNameInstance.transform.position = Cam.WorldToScreenPoint(transform.position);
        //     m_UnitDistanceInstance.transform.position = Cam.WorldToScreenPoint(transform.position);
        //     m_UnitHealthInstance.transform.position = Cam.WorldToScreenPoint(transform.position);
        // }
        // if (MapActive) {
        //     m_MapUnitNameInstance.transform.position = MapCam.WorldToScreenPoint(transform.position);
        //     m_MapUnitDistanceInstance.transform.position = MapCam.WorldToScreenPoint(transform.position);
        //     m_MapUnitHealthInstance.transform.position = MapCam.WorldToScreenPoint(transform.position);
        // }
        ScreenPos = Cam.WorldToScreenPoint(transform.position);
        Debug.Log(Name + " is " + ScreenPos.x + " pixels from the left");
        Debug.Log(Name + " is " + ScreenPos.y + " pixels from the top");
        Debug.Log(Name + " is " + ScreenPos.y + " pixels from the camera");
        if (m_UnitNameInstance)
            m_UnitNameInstance.transform.position = Cam.WorldToScreenPoint(this.transform.position);
        if (m_MapUnitNameInstance)
            m_MapUnitNameInstance.transform.position = MapCam.WorldToScreenPoint(this.transform.position);
    }

    private void SetDisplay() {
        if (!Dead) {
            if (Active) {
                if (!m_UnitNameInstance)
                    DisplayUI();
            } else {
                DestroyUI();
            }
            if (!m_MapUnitNameInstance)
                    DisplayMapUI();
        } else {
            DestroyUI();
            DestroyMapUI();
        }
    }

    private void DisplayUI() {
        m_UnitNameInstance = Instantiate(m_UnitName, PlayerCanvas.transform);
        m_UnitDistanceInstance = Instantiate(m_UnitDistance, PlayerCanvas.transform);
        m_UnitHealthInstance = Instantiate(m_UnitHealth, PlayerCanvas.transform);

        m_UnitNameInstance.GetComponent<Text>().text = Name;
    }

    private void DestroyUI() {
        if (m_UnitNameInstance)
            Destroy (m_UnitNameInstance);
        if (m_UnitDistanceInstance)
            Destroy (m_UnitDistanceInstance);
        if (m_UnitHealthInstance)
            Destroy (m_UnitHealthInstance);
    }

    /*private void SetMapDisplay() {
        if (MapActive) {
            DisplayMapUI();
        } else {
            DestroyMapUI();
        }
    }*/

    private void DisplayMapUI() {
        m_MapUnitNameInstance = Instantiate(m_MapUnitName, PlayerMapCanvas.transform);
        m_MapUnitDistanceInstance = Instantiate(m_MapUnitDistance, PlayerMapCanvas.transform);
        m_MapUnitHealthInstance = Instantiate(m_MapUnitHealth, PlayerMapCanvas.transform);

        m_MapUnitNameInstance.GetComponent<Text>().text = Name;
    }

    private void DestroyMapUI() {
        if (m_MapUnitNameInstance)
            Destroy (m_MapUnitNameInstance);
        if (m_MapUnitDistanceInstance)
            Destroy (m_MapUnitDistanceInstance);
        if (m_MapUnitHealthInstance)
            Destroy (m_MapUnitHealthInstance);
    }

    public void SetStartingHealth(float FullHP) {
        // Debug.Log (this.name+" - ShipUISlider -"+ShipUISlider);
        // ShipUISlider.maxValue = FullHP;
        // ShipUISlider.value = FullHP;
        
        // ShipMapUISlider.maxValue = FullHP;
        // ShipMapUISlider.value = FullHP;
    }
    public void SetCurrentHealth(float HP) {
        // ShipUISlider.value = HP;
        // ShipMapUISlider.value = HP;
    }
    public void SetName(string name) {
        Name = name;
        if (m_UnitNameInstance)
            m_UnitNameInstance.GetComponent<Text>().text = Name;
        if (m_MapUnitNameInstance)
            m_MapUnitNameInstance.GetComponent<Text>().text = Name;
    }
    public void SetPlayerCanvas(GameObject playerCanvas, GameObject playerMapCanvas){
        PlayerCanvas = playerCanvas;
        PlayerMapCanvas = playerMapCanvas;
    }

    public void SetActive(bool activate) {
        Active = activate;
        SetDisplay();

        // ShipUIObj.enabled = Active;
    }
    public void SetMapActive(bool activate) {
        // MapActive = activate;
        // SetMapDisplay();
    }
    public void SetDead() {
        Dead = true;
        SetDisplay();

        // ShipUIObj.enabled = Active;
    }
}