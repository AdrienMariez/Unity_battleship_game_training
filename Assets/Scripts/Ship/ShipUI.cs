using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShipUI : MonoBehaviour {
    private bool Dead = false;
    private bool Active = false;
    private bool MapActive = false;
    private bool ActionPaused = false;
    private bool ShortActionPaused = false;
    private bool BehindCamera = false;
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
        if (GameObject.Find("MainCamera"))
            Cam = GameObject.Find("MainCamera").GetComponentInChildren<Camera>();
        if (GameObject.Find("MapCamera"))
            MapCam = GameObject.Find("MapCamera").GetComponentInChildren<Camera>();
    }

    private void FixedUpdate() {
        if (!Active && MapActive && !Dead && !ActionPaused){
            DistanceString = "Played unit";
            // Debug.Log (Name+" - DistanceString -"+DistanceString);
        } else if (Active && !Dead && !ActionPaused || MapActive && !Dead && !ActionPaused) {
            float distance = (transform.position - Cam.transform.position).magnitude;
            if (distance > 999) {
                distance = (Mathf.Round(distance / 100)) / 10f;
                DistanceString = string.Format(RangeDisplayKilometer, distance);
            } else {
                DistanceString = string.Format(RangeDisplayMeter, Mathf.Round(distance));
            }
        }
        // Debug.Log (" - Name : "+Name+" - Active : "+Active+" - MapActive : "+MapActive+" - Dead : "+Dead);

        if (Active && !ShortActionPaused){
            var heading = transform.position - Cam.transform.position;
            if (Vector3.Dot(Cam.transform.forward, heading) > 0) {
                if (BehindCamera){
                    UnitNameInstance.SetActive(true);
                    UnitDistanceInstance.SetActive(true);
                    UnitHealthInstance.SetActive(true);
                    BehindCamera = false;
                }
                // Debug.Log (Name+" - UI is in front -"+heading);
                Vector3 screenPos = Cam.WorldToScreenPoint(transform.position);
                Vector3 vectorName = new Vector2(screenPos.x, (screenPos.y + 70f));
                Vector3 vectorDistance = new Vector2(screenPos.x, (screenPos.y + 50f));
                Vector3 vectorHealth = new Vector2(screenPos.x, (screenPos.y + 30f));
                UnitNameInstance.transform.position  = vectorName;
                UnitDistanceInstance.transform.position  = vectorDistance;
                UnitHealthInstance.transform.position  = vectorHealth;

                UnitDistanceInstance.GetComponent<Text>().text = DistanceString;
            } else {
                if (!BehindCamera){
                    UnitNameInstance.SetActive(false);
                    UnitDistanceInstance.SetActive(false);
                    UnitHealthInstance.SetActive(false);
                    BehindCamera = true;
                }
            }
        }
        if (MapActive && !ShortActionPaused){
            Vector3 screenMapPos = MapCam.WorldToScreenPoint(transform.position);
            Vector3 vectorMapName = new Vector2(screenMapPos.x, (screenMapPos.y + 50f));
            Vector3 vectorMapDistance = new Vector2(screenMapPos.x, (screenMapPos.y + 30f));
            Vector3 vectorMapHealth = new Vector2(screenMapPos.x, (screenMapPos.y + 10f));
            MapUnitNameInstance.transform.position  = vectorMapName;
            MapUnitDistanceInstance.transform.position  = vectorMapDistance;
            MapUnitHealthInstance.transform.position  = vectorMapHealth;

            MapUnitDistanceInstance.GetComponent<Text>().text = DistanceString;
        }
        if (!ActionPaused && !Dead)
            StartCoroutine(PauseAction());
        if (!ShortActionPaused && !Dead)
            StartCoroutine(PauseAction());
    }

    IEnumerator PauseAction(){
        // Coroutine created to prevent too much calculus for ship behaviour
        ActionPaused = true;
        yield return new WaitForSeconds(0.5f);
        ActionPaused = false;
    }

    IEnumerator PauseActionShort(){
        // Coroutine created to prevent too much calculus for ship behaviour
        ShortActionPaused = true;
        yield return new WaitForSeconds(0.1f);
        ShortActionPaused = false;
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
        // Debug.Log (Name+" - Team -"+Team);
        if (Team == "Allies") {
            UnitNameInstance.GetComponent<Text>().color = new Color(0f, 0.47f, 1f, 1f);
            UnitDistanceInstance.GetComponent<Text>().color = new Color(0f, 0.47f, 1f, 1f);
        } else if (Team == "AlliesAI") {
            UnitNameInstance.GetComponent<Text>().color = new Color(0f, 0.1f, 1f, 1f);
            UnitDistanceInstance.GetComponent<Text>().color = new Color(0f, 0.1f, 1f, 1f);
        }  else if (Team == "Axis") {
            UnitNameInstance.GetComponent<Text>().color = new Color(1f, 0.22f, 0.29f, 1f);
            UnitDistanceInstance.GetComponent<Text>().color = new Color(1f, 0.22f, 0.29f, 1f);
        }  else if (Team == "AxisAI") {
            UnitNameInstance.GetComponent<Text>().color = new Color(1f, 0.0f, 0.0f, 0.49f);
            UnitDistanceInstance.GetComponent<Text>().color = new Color(1f, 0.0f, 0.0f, 0.49f);
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

        if (Team == "Allies") {
            MapUnitNameInstance.GetComponent<Text>().color = new Color(0f, 0.47f, 1f, 1f);
            MapUnitDistanceInstance.GetComponent<Text>().color = new Color(0f, 0.47f, 1f, 1f);
        } else if (Team == "AlliesAI") {
            MapUnitNameInstance.GetComponent<Text>().color = new Color(0f, 0.1f, 1f, 1f);
            MapUnitDistanceInstance.GetComponent<Text>().color = new Color(0f, 0.1f, 1f, 1f);
        }  else if (Team == "Axis") {
            MapUnitNameInstance.GetComponent<Text>().color = new Color(1f, 0.22f, 0.29f, 1f);
            MapUnitDistanceInstance.GetComponent<Text>().color = new Color(1f, 0.22f, 0.29f, 1f);
        }  else if (Team == "AxisAI") {
            MapUnitNameInstance.GetComponent<Text>().color = new Color(1f, 0.0f, 0.0f, 0.49f);
            MapUnitDistanceInstance.GetComponent<Text>().color = new Color(1f, 0.0f, 0.0f, 0.49f);
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






    // private GameObject UIElement;
    // TRANSFORM THIS INTO AN ARRAY, WE INIT ALL DATA AT ONCE AND UPDATE ALL NEEDED INFO AT ONCE TOO

    public void SetUIElement(GameObject uiElement) {
        // UIElement = uiElement;
        uiElement.gameObject.name = Name;
        uiElement.transform.Find("Name").GetComponent<Text>().text = Name;
        SetColor(uiElement);
        uiElement.transform.Find("Health").GetComponent<Slider>().maxValue = MaximumHealth;
        uiElement.transform.Find("Health").GetComponent<Slider>().value = CurrentHealth;
    }
    private void SetColor(GameObject uiElement) {
        Color color;
        if (Team == "Allies") {
            color = new Color(0f, 0.47f, 1f, 1f);
        } else if (Team == "AlliesAI") {
            color = new Color(0f, 0.1f, 1f, 1f);
        }  else if (Team == "Axis") {
            color = new Color(1f, 0.22f, 0.29f, 1f);
        }  else if (Team == "AxisAI") {
            color = new Color(1f, 0.0f, 0.0f, 0.49f);
        } else{
            color = Color.yellow;
        }
        uiElement.transform.Find("Name").GetComponent<Text>().color = color;
        uiElement.transform.Find("Distance").GetComponent<Text>().color = color;
    }
}