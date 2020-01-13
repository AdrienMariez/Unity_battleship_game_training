using UnityEngine;
using Crest;

public class ShipDamageControl : MonoBehaviour {
    private bool Active = false;
    public float RepairRate = 3;
    public float RepairCrew = 3;

    private ShipController ShipController;

    private float EngineRepairCrew;
    private float FireRepairCrew;
    private float WaterRepairCrew;
    private float TurretsRepairCrew;

    private bool DmgCtrlOpen = false;

    private void Awake() {
        EngineRepairCrew = 1;
        FireRepairCrew = 1;
        WaterRepairCrew = 1;
        TurretsRepairCrew = 1;

        ShipController = GetComponent<ShipController>();
        TurretsRepairCrew += RepairCrew;

        ShipController.SetDamageControlEngine(EngineRepairCrew);
        ShipController.SetDamageControlFire(FireRepairCrew);
        ShipController.SetDamageControlWater(WaterRepairCrew);
        ShipController.SetDamageControlTurrets(TurretsRepairCrew);
    }

    protected void Update() {
        if (Active) {
            if (Input.GetButtonDown ("OpenDamageControl")) {
                SetOpenDmgCtrl(!DmgCtrlOpen);
            }
        }
        if (DmgCtrlOpen) {
            // Update damage control panel ?
        }
    }

    protected void SetOpenDmgCtrl(bool open) {
        DmgCtrlOpen = open;
    }

    public void SetActive(bool activate) {
        Active = activate;
    }

    public float GetRepairRate() {
        return RepairRate;
    }
}