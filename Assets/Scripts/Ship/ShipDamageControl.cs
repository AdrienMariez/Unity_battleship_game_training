using UnityEngine;
using UnityEngine.UI;

public class ShipDamageControl : MonoBehaviour {
    private bool Active = false;
    private bool MapActive = false;
    public GameObject m_DamageControlUI;
    public float RepairRate = 3;
    public float RepairCrew = 3;

    private ShipController ShipController;

    private float EngineRepairCrew = 0;
    private float FireRepairCrew = 0;
    private float WaterRepairCrew = 0;
    private float TurretsRepairCrew = 0;
    private float UnsetCrew = 0;
    private string UnitName;

    private bool DmgCtrlOpen = false;
    private GameObject m_DamageControlInstance;


    private void Awake() {
        UnsetCrew = RepairCrew;

        ShipController = GetComponent<ShipController>();
        // TurretsRepairCrew += RepairCrew;

        ShipController.SetDamageControlEngine(EngineRepairCrew);
        ShipController.SetDamageControlFire(FireRepairCrew);
        ShipController.SetDamageControlWater(WaterRepairCrew);
        ShipController.SetDamageControlTurrets(TurretsRepairCrew);
    }

    protected void Update() {
        if (Active && !MapActive) {
            if (Input.GetButtonDown ("OpenDamageControl")) {
                // Debug.Log("DmgCtrlOpen :"+ DmgCtrlOpen);
                SetOpenDmgCtrl(!DmgCtrlOpen);
            }
        }
        if (DmgCtrlOpen) {
            // Update damage control panel ?
        }
    }
    protected void OpenDmgCtrl() {
        // Cursor.lockState = CursorLockMode.None;
        // Cursor.visible = true;

        m_DamageControlInstance = Instantiate(m_DamageControlUI);
        m_DamageControlInstance.transform.Find("UnitName").GetComponent<Text>().text = UnitName;
        m_DamageControlInstance.transform.Find("EngineCrewCount").GetComponent<Text>().text = EngineRepairCrew.ToString("g");
        m_DamageControlInstance.transform.Find("FireCrewCount").GetComponent<Text>().text = FireRepairCrew.ToString("g");
        m_DamageControlInstance.transform.Find("WaterCrewCount").GetComponent<Text>().text = WaterRepairCrew.ToString("g");
        m_DamageControlInstance.transform.Find("TurretsCrewCount").GetComponent<Text>().text = TurretsRepairCrew.ToString("g");
        m_DamageControlInstance.transform.Find("UnsetCrewCount").GetComponent<Text>().text = UnsetCrew.ToString("g");

        Button buttonEnginePos = m_DamageControlInstance.transform.Find("buttonEnginePos").GetComponent<Button>();
		buttonEnginePos.onClick.AddListener(ButtonEnginePosOnClick);

        CheckActiveButtons();
    }

    protected void CloseDmgCtrl() {
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;

        if (m_DamageControlInstance)
            Destroy (m_DamageControlInstance);
    } 

    protected void CheckActiveButtons() {
        if (EngineRepairCrew > 0) {
            m_DamageControlInstance.transform.Find("buttonEngineNeg").gameObject.SetActive(true);
        } else {
            m_DamageControlInstance.transform.Find("buttonEngineNeg").gameObject.SetActive(false);
        }

        if (FireRepairCrew > 0) {
            m_DamageControlInstance.transform.Find("buttonFireNeg").gameObject.SetActive(true);
        } else {
            m_DamageControlInstance.transform.Find("buttonFireNeg").gameObject.SetActive(false);
        }

        if (WaterRepairCrew > 0) {
            m_DamageControlInstance.transform.Find("buttonEngineNeg").gameObject.SetActive(true);
        } else {
            m_DamageControlInstance.transform.Find("buttonEngineNeg").gameObject.SetActive(false);
        }

        if (TurretsRepairCrew > 0) {
            m_DamageControlInstance.transform.Find("buttonTurretsNeg").gameObject.SetActive(true);
        } else {
            m_DamageControlInstance.transform.Find("buttonTurretsNeg").gameObject.SetActive(false);
        }

        if (UnsetCrew > 0) {
            m_DamageControlInstance.transform.Find("buttonEnginePos").gameObject.SetActive(true);
            m_DamageControlInstance.transform.Find("buttonFirePos").gameObject.SetActive(true);
            m_DamageControlInstance.transform.Find("buttonWaterPos").gameObject.SetActive(true);
            m_DamageControlInstance.transform.Find("buttonTurretsPos").gameObject.SetActive(true);
        } else {
            m_DamageControlInstance.transform.Find("buttonEnginePos").gameObject.SetActive(false);
            m_DamageControlInstance.transform.Find("buttonFirePos").gameObject.SetActive(false);
            m_DamageControlInstance.transform.Find("buttonWaterPos").gameObject.SetActive(false);
            m_DamageControlInstance.transform.Find("buttonTurretsPos").gameObject.SetActive(false);
        }
    }

    void ButtonEnginePosOnClick(){
        Debug.Log ("ButtonEnginePosOnClick");
    }

    public void SetOpenDmgCtrl(bool open) {
        DmgCtrlOpen = open;
        ShipController.SetDamageControl(DmgCtrlOpen);
        if (DmgCtrlOpen) {
            OpenDmgCtrl();
        } else {
            CloseDmgCtrl();
        }
    }
    public void SetActive(bool activate) {
        Active = activate;
        if (!Active)
            SetOpenDmgCtrl(false);
    }
    public void SetMap(bool map) {
        MapActive = map;
        if (MapActive)
            SetOpenDmgCtrl(false);
    }
    public void SetName(string name) { UnitName = name; }

    public float GetRepairRate() { return RepairRate; }
}