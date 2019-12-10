using UnityEngine;
using Crest;

public class ShipDamageControl : MonoBehaviour {

    public float RepairRate = 3;
    public float RepairCrew = 3;

    private ShipController ShipController;

    [HideInInspector] public float EngineRepairCrew;
    [HideInInspector] public float FireRepairCrew;
    [HideInInspector] public float WaterRepairCrew;
    [HideInInspector] public float TurretsRepairCrew;

    private void Start() {
        EngineRepairCrew = 1;
        FireRepairCrew = 1;
        WaterRepairCrew = 1;
        TurretsRepairCrew = 1;

        ShipController = GetComponent<ShipController>();
        TurretsRepairCrew += RepairCrew;

        ShipController.EngineRepairCrew = EngineRepairCrew;
        ShipController.FireRepairCrew = FireRepairCrew;
        ShipController.WaterRepairCrew = WaterRepairCrew;
        ShipController.TurretsRepairCrew = TurretsRepairCrew;
    }

}