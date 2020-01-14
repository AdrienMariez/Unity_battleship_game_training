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
        TurretsRepairCrew += RepairCrew;

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
        m_DamageControlInstance = Instantiate(m_DamageControlUI);
        m_DamageControlInstance.transform.Find("UnitName").GetComponent<Text>().text = UnitName;
        m_DamageControlInstance.transform.Find("EngineCrewCount").GetComponent<Text>().text = EngineRepairCrew.ToString("g");
        m_DamageControlInstance.transform.Find("FireCrewCount").GetComponent<Text>().text = FireRepairCrew.ToString("g");
        m_DamageControlInstance.transform.Find("WaterCrewCount").GetComponent<Text>().text = WaterRepairCrew.ToString("g");
        m_DamageControlInstance.transform.Find("TurretsCrewCount").GetComponent<Text>().text = TurretsRepairCrew.ToString("g");
        m_DamageControlInstance.transform.Find("UnsetCrewCount").GetComponent<Text>().text = UnsetCrew.ToString("g");
    }

    protected void CloseDmgCtrl() {
        if (m_DamageControlInstance)
            Destroy (m_DamageControlInstance);
    } 

    public void SetOpenDmgCtrl(bool open) {
        DmgCtrlOpen = open;
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