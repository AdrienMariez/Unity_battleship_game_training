using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingAI : UnitAIController {
    private BuildingController BuildingController;
    public override void BeginOperations () {

        // Still need the specific unit Controller for specific methods
        BuildingController = GetComponent<BuildingController>();

        // Building...can't move.
        UnitCanMove = false;
        // UnitsAICurrentState for buildings is set in base.Awake()
        base.BeginOperations();
    }
}