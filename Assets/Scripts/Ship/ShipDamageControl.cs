using UnityEngine;
using Crest;

public class ShipDamageControl : MonoBehaviour {

    public float RepairRate = 3;
    public float RepairCrew = 3;

    private ShipController ShipController;

    private float EngineRepairCrew;
    private float FireRepairCrew;
    private float WaterRepairCrew;
    private float TurretsRepairCrew;

    private void Start() {
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

    public float GetRepairRate() {
        return RepairRate;
    }

}