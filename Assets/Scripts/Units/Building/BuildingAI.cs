using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingAI : UnitAIController {
    private BuildingController BuildingController;
    public override void BeginOperations (bool aiMove, bool aiShoot, bool aiSpawn) {

        // Still need the specific unit Controller for specific methods
        BuildingController = GetComponent<BuildingController>();

        // UnitsAICurrentState for buildings is set in base.Awake()
        base.BeginOperations(UnitCanMove, aiShoot, aiSpawn);
        
        // Building...can't move.
        UnitCanMove = false;
    }
}