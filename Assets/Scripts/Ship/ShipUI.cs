using UnityEngine;
using UnityEngine.UI;

public class ShipUI : MonoBehaviour {
    private bool Dead = false;
    private bool Active = false;
    private bool MapActive = false;
    private GameObject PlayerCanvas;
    private GameObject PlayerMapCanvas;
    private Camera Cam;
    private Camera MapCam;

    public GameObject m_UnitName;
        private GameObject UnitNameInstance;
    public GameObject m_UnitDistance;
        private GameObject UnitDistanceInstance;
    public GameObject m_UnitHealth;
        private GameObject UnitHealthInstance;
    public GameObject m_MapUnitName;
        private GameObject MapUnitNameInstance;
    public GameObject m_MapUnitDistance;
        private GameObject MapUnitDistanceInstance;
    public GameObject m_MapUnitHealth;
        private GameObject MapUnitHealthInstance;

    private string Name;
    private string Team;
    private string DistanceString;
    private float MaximumHealth;
    private float CurrentHealth;

    const string RangeDisplayMeter = "{0} m";
    const string RangeDisplayKilometer = "{0} km";

    // [Header("Debug")]
    //     public bool debug = false;


    public void Init() {
        // ShipUIText.text = this.name;
        Cam = GameObject.Find("MainCamera").GetComponentInChildren<Camera>();
        MapCam = GameObject.Find("MapCamera").GetComponentInChildren<Camera>();
    }

    private void FixedUpdate() {
        if (!Active && MapActive && !Dead){
            DistanceString = "Played unit";
            // Debug.Log (Name+" - DistanceString -"+DistanceString);
        } else if (Active && !Dead || MapActive && !Dead) {
            float distance = (transform.position - Cam.transform.position).magnitude;
            if (distance > 999) {
                distance = (Mathf.Round(distance / 100)) / 10f;
                DistanceString = string.Format(RangeDisplayKilometer, distance);
            } else {
                DistanceString = string.Format(RangeDisplayMeter, Mathf.Round(distance));
            }
        }
        // Debug.Log (" - Name : "+Name+" - Active : "+Active+" - MapActive : "+MapActive+" - Dead : "+Dead);

        if (Active){
            Vector3 screenPos = Cam.WorldToScreenPoint(transform.position);
            Vector3 vectorName = new Vector2(screenPos.x, (screenPos.y + 50f));
            Vector3 vectorDistance = new Vector2(screenPos.x, (screenPos.y + 30f));
            Vector3 vectorHealth = new Vector2(screenPos.x, (screenPos.y + 10f));
            UnitNameInstance.transform.position  = vectorName;
            UnitDistanceInstance.transform.position  = vectorDistance;
            UnitHealthInstance.transform.position  = vectorHealth;

            UnitDistanceInstance.GetComponent<Text>().text = DistanceString;
        }
        if (MapActive){
            Vector3 screenMapPos = MapCam.WorldToScreenPoint(transform.position);
            Vector3 vectorMapName = new Vector2(screenMapPos.x, (screenMapPos.y + 50f));
            Vector3 vectorMapDistance = new Vector2(screenMapPos.x, (screenMapPos.y + 30f));
            Vector3 vectorMapHealth = new Vector2(screenMapPos.x, (screenMapPos.y + 10f));
            MapUnitNameInstance.transform.position  = vectorMapName;
            MapUnitDistanceInstance.transform.position  = vectorMapDistance;
            MapUnitHealthInstance.transform.position  = vectorMapHealth;

            MapUnitDistanceInstance.GetComponent<Text>().text = DistanceString;
        }
    }

    private void SetDisplay() {
        if (!Dead) {
            if (Active) {
                if (!UnitNameInstance)
                    DisplayUI();
            } else {
                DestroyUI();
            }
            if (!MapUnitNameInstance)
                    DisplayMapUI();
        } else {
            DestroyUI();
            DestroyMapUI();
        }
    }

    private void DisplayUI() {
        Active = true;
        UnitNameInstance = Instantiate(m_UnitName, PlayerCanvas.transform);
        UnitDistanceInstance = Instantiate(m_UnitDistance, PlayerCanvas.transform);
        UnitHealthInstance = Instantiate(m_UnitHealth, PlayerCanvas.transform);

        if (Team == "Allies" || Team == "AlliesAI") {
            UnitNameInstance.GetComponent<Text>().color = Color.blue;
            UnitDistanceInstance.GetComponent<Text>().color = Color.blue;
        } else if (Team == "Axis" || Team == "AxisAI") {
            UnitNameInstance.GetComponent<Text>().color = Color.red;
            UnitDistanceInstance.GetComponent<Text>().color = Color.red;
        } else{
            UnitNameInstance.GetComponent<Text>().color = Color.yellow;
            UnitDistanceInstance.GetComponent<Text>().color = Color.yellow;
        }

        UnitNameInstance.GetComponent<Text>().text = Name;
        UnitHealthInstance.GetComponent<Slider>().maxValue = MaximumHealth;
        UnitHealthInstance.GetComponent<Slider>().value = CurrentHealth;
    }

    private void DestroyUI() {
        Active = false;
        if (UnitNameInstance)
            Destroy (UnitNameInstance);
        if (UnitDistanceInstance)
            Destroy (UnitDistanceInstance);
        if (UnitHealthInstance)
            Destroy (UnitHealthInstance);
    }

    private void DisplayMapUI() {
        MapActive = true;
        MapUnitNameInstance = Instantiate(m_MapUnitName, PlayerMapCanvas.transform);
        MapUnitDistanceInstance = Instantiate(m_MapUnitDistance, PlayerMapCanvas.transform);
        MapUnitHealthInstance = Instantiate(m_MapUnitHealth, PlayerMapCanvas.transform);

        if (Team == "Allies" || Team == "AlliesAI") {
            MapUnitNameInstance.GetComponent<Text>().color = Color.blue;
            MapUnitDistanceInstance.GetComponent<Text>().color = Color.blue;
        } else if (Team == "Axis" || Team == "AxisAI") {
            MapUnitNameInstance.GetComponent<Text>().color = Color.red;
            MapUnitDistanceInstance.GetComponent<Text>().color = Color.red;
        } else{
            MapUnitNameInstance.GetComponent<Text>().color = Color.yellow;
            MapUnitDistanceInstance.GetComponent<Text>().color = Color.yellow;
        }

        MapUnitNameInstance.GetComponent<Text>().text = Name;
        MapUnitHealthInstance.GetComponent<Slider>().maxValue = MaximumHealth;
        MapUnitHealthInstance.GetComponent<Slider>().value = CurrentHealth;
    }

    private void DestroyMapUI() {
        MapActive = false;
        if (MapUnitNameInstance)
            Destroy (MapUnitNameInstance);
        if (MapUnitDistanceInstance)
            Destroy (MapUnitDistanceInstance);
        if (MapUnitHealthInstance)
            Destroy (MapUnitHealthInstance);
    }

    public void SetStartingHealth(float FullHP) {
        // ShipUISlider.maxValue = FullHP;
        // ShipUISlider.value = FullHP;
        MaximumHealth = FullHP;
        CurrentHealth = FullHP;
    }
    public void SetCurrentHealth(float HP) {
        // ShipUISlider.value = HP;
        // ShipMapUISlider.value = HP;
        CurrentHealth = HP;
        if (UnitHealthInstance)
            UnitHealthInstance.GetComponent<Slider>().value = CurrentHealth;
        if (MapUnitHealthInstance)
            MapUnitHealthInstance.GetComponent<Slider>().value = CurrentHealth;
    }
    public void SetName(string name) {
        Name = name;
        if (UnitNameInstance)
            UnitNameInstance.GetComponent<Text>().text = Name;
        if (MapUnitNameInstance)
            MapUnitNameInstance.GetComponent<Text>().text = Name;
    }
    public void SetUnitTeam(string team){ Team = team; }
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