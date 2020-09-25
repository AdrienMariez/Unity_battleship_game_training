using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipAI : UnitAIController {
    protected ShipController ShipController;
    public UnitsAIStates ShipAISpawnState = UnitsAIStates.Patrol;
    protected override void Awake () {
        UnitsAICurrentState = ShipAISpawnState;
        // Still need the specific unit Controller for specific methods
        ShipController = GetComponent<ShipController>();
        base.Awake();
    }
    // UnitsAIStates
    // UnitsAICurrentState
    protected override void CheckState() {
        // if (UnitsAICurrentState == UnitsAIStates.NoAI) {
        //     return;
        // }
        // If there is a target
        if (TargetUnit != null && UnitCanShoot) {
            // I'm not sure about this one... The logic is : if it's one of the correct states, check if the target is in range or not. Act accordingly
            if (UnitsAICurrentState == UnitsAIStates.ApproachTarget || UnitsAICurrentState == UnitsAIStates.CircleTarget || UnitsAICurrentState == UnitsAIStates.Patrol) {
                if ((gameObject.transform.position - TargetUnit.transform.position).magnitude > MaxTurretsRange) {
                    UnitsAICurrentState = UnitsAIStates.ApproachTarget;
                } else {
                    UnitsAICurrentState = UnitsAIStates.CircleTarget;
                }
            }
        } else {
            if (UnitsAICurrentState == UnitsAIStates.ApproachTarget || UnitsAICurrentState == UnitsAIStates.CircleTarget || UnitsAICurrentState == UnitsAIStates.Patrol) {
                UnitsAICurrentState = UnitsAIStates.Patrol;
            }
        }

        base.CheckState();

        // Debug.Log("Unit : "+ Name +" - TargetUnit = "+ TargetUnit +" - UnitsAICurrentState = "+ UnitsAICurrentState);
        // Debug.Log("Unit : "+ Name +" - TargetUnit = "+ TargetUnit +" - AIState = "+ AIState);
        // Debug.Log("Unit : "+ Name +" - magnitude = "+ (gameObject.transform.position - TargetUnit.transform.position).magnitude +" - MaxTurretsRange = "+ MaxTurretsRange);
    }

    // Possible Actions
    protected override void PatrolAction(){
        // Not done at all
        ShipController.SetAISpeed(4);
        ShipController.SetAIturn(0);
    }
    protected override void CircleTargetAction(){
        ShipController.SetAISpeed(4);

        Vector3 targetDir = gameObject.transform.position - TargetUnit.transform.position;
        Vector3 forward = gameObject.transform.forward;
        float angle = Vector3.SignedAngle(targetDir, forward, Vector3.up);

        if (angle > 95 && angle > 0 && TurnInputLimit < 1 || angle > -85 && angle < 0 && TurnInputLimit < 1) {
            ShipController.SetAIturn(-0.5f);
        } else if (angle < 85  && angle > 0 && TurnInputLimit > -1 || angle < -95 && angle < 0 && TurnInputLimit > -1) {
            ShipController.SetAIturn(0.5f);
        } else {
            ShipController.SetAIturn(0);
        }
    }
    protected override void ApproachTargetAction(){
                // Debug.Log("ApproachTarget");
        ShipController.SetAISpeed(4);

        Vector3 targetDir = gameObject.transform.position - TargetUnit.transform.position;
        Vector3 forward = gameObject.transform.forward;
        float angle = Vector3.SignedAngle(targetDir, forward, Vector3.up);

        // Not tested !
        if (angle > 5 && angle < 180 && TurnInputLimit < 1) {
            ShipController.SetAIturn(-0.5f);
        } else if (angle < -5 && angle > -180 && TurnInputLimit > -1) {
            ShipController.SetAIturn(0.5f);
        } else {
            ShipController.SetAIturn(0);
        }
        // Debug.Log("angle : "+ angle);
    }
    protected override void IdleAction(){
        ShipController.SetAISpeed(0);
        ShipController.SetAIturn(0);
    }
    protected override void FollowWayPointsAction(){

    }
    protected override void FleeAction(){

    }
    protected override void BackToBaseAction(){

    }
    protected override void NoAIAction(){

    }

    // Unit Manager sent info
    public override void SetAITurnInputValue(float turnInputValue){ TurnInputLimit = turnInputValue; }
}