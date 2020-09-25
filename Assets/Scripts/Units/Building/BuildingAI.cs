using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingAI : UnitAIController {
    private BuildingController BuildingController;
    protected override void Awake () {

        // Still need the specific unit Controller for specific methods
        BuildingController = GetComponent<BuildingController>();

        // Building...can't move.
        UnitCanMove = false;
        // UnitsAICurrentState for buildings is set in base.Awake()
        base.Awake();
    }
    protected override void CheckState() {
        Stressed = false;
        // CHECK IF CAN SHOOT
        if (TargetUnit != null && UnitCanShoot && UnitsAICurrentState != UnitsAIStates.NoAI && (gameObject.transform.position - TargetUnit.transform.position).magnitude < MaxTurretsRange) {
            // In this case, there is a target and we can shoot it.
            Stressed = true;
            TurretManager.SetAIHasTarget(true);
        } else {
            Stressed = false;
            TurretManager.SetAIHasTarget(false);
        }

        base.CheckState();
        // Debug.Log("Unit : "+ Name +" - TargetUnit = "+ TargetUnit +" - AIState = "+ AIState);
        // Debug.Log("Unit : "+ Name +" - magnitude = "+ (gameObject.transform.position - TargetUnit.transform.position).magnitude +" - MaxTurretsRange = "+ MaxTurretsRange);
    }
    protected override void IdleAction(){
        
    }
    protected override void NoAIAction(){

    }
}