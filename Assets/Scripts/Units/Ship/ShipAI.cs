using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipAI : UnitAIController {
    protected ShipController ShipController;
    public UnitsAIStates ShipAISpawnState = UnitsAIStates.Patrol;
    protected float RotationSafeDistance;  // this var is used to determine if a waypoint is inside a turning arc of a full speed ship
    public override void BeginOperations (bool aiMove, bool aiShoot, bool aiSpawn) {
        UnitsAICurrentState = ShipAISpawnState;
        // Still need the specific unit Controller for specific methods
        ShipController = GetComponent<ShipController>();
        RotationSafeDistance = ShipController.GetUnitWorldSingleUnit().GetBuoyancyRotationTime() * 6;
        // Debug.Log("Unit : "+ Name +"RotationSafeDistance: " + RotationSafeDistance);
        base.BeginOperations(aiMove, aiShoot, aiSpawn);
    }
    // UnitsAIStates
    // UnitsAICurrentState
    protected override void CheckState() {
        // if (UnitsAICurrentState == UnitsAIStates.NoAI) {
        //     return;
        // }
        if (FollowedUnit != null && FollowsUnit) {
            UnitsAICurrentState = UnitsAIStates.Follow;
        } else if (Waypoints.Count > 0 && UsesWaypoints) {
            UnitsAICurrentState = UnitsAIStates.FollowWayPoints;
        } else if (TargetUnit != null && UnitCanMove && UnitCanShoot) {
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
            } else {
                UnitsAICurrentState = UnitsAIStates.Idle;
            }
        }

        // if (UnitsAICurrentState != UnitsAIStates.Idle && UnitsAICurrentState != UnitsAIStates.NoAI) {
        //     if (!UnitCanMove) {
        //         UnitsAICurrentState = UnitsAIStates.Idle;
        //     }
        // }

        base.CheckState();

        // Debug.Log("Unit : "+ Name +" - TargetUnit = "+ TargetUnit +" - UnitsAICurrentState = "+ UnitsAICurrentState);
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
    protected override void FollowAction(){
        if (!FollowsUnit) {
            return;
        }
        Vector3 targetDir = transform.position - FollowedUnit.GetUnitModel().transform.position;
        Vector3 forward = transform.forward;
        float angle = Vector3.SignedAngle(targetDir, forward, Vector3.up);

        float distance = (transform.position -  FollowedUnit.GetUnitModel().transform.position).magnitude;
        // Debug.Log("distance : " + distance);

        if (distance > RotationSafeDistance && angle > 0 && angle < 178) {                                  // If destination is far and on the right
            ShipController.SetAIturn(0.5f);
            ShipController.SetAISpeed(4);
            // Debug.Log("far right - " + distance);
            // Debug.Log("case 1 - angle : " + angle +" - TurnInputLimit - " + TurnInputLimit);
        } else if (distance > RotationSafeDistance && angle < -0 && angle > -178) {                         // If destination is far and on the left
            ShipController.SetAIturn(-0.5f);
            ShipController.SetAISpeed(4);
            // Debug.Log("far left - " + distance);
            // Debug.Log("case 2 - angle : " + angle +" - TurnInputLimit - " + TurnInputLimit);
        } else if (distance > RotationSafeDistance ) {                                                                       // If far
            ShipController.SetAIturn(0);
            // Debug.Log("far - " + distance);
            // Debug.Log("case 3 - angle : " + angle +" - TurnInputLimit - " + TurnInputLimit);
        } else if (distance < RotationSafeDistance / 2) {                                                              // If destination is VERY close 
            ShipController.SetAIturn(0);
            ShipController.SetAISpeed(0);
            // Debug.Log("very close - " + distance);
            // Debug.Log("case 3 - angle : " + angle +" - TurnInputLimit - " + TurnInputLimit);
        } else if (angle > 0 && angle < 120) {                                                              // If destination is close and extremely on the right
            ShipController.SetAIturn(0.5f);
            ShipController.SetAISpeed(1);
            // Debug.Log("close Far right - " + distance);
            // Debug.Log("case 3 - angle : " + angle +" - TurnInputLimit - " + TurnInputLimit);
        } else if (angle < -0 && angle > -120) {                                                            // If destination is close and extremely on the left
            ShipController.SetAIturn(-0.5f);
            ShipController.SetAISpeed(1);
            // Debug.Log("close Far left - " + distance);
            // Debug.Log("case 4 - angle : " + angle +" - TurnInputLimit - " + TurnInputLimit);
        } else if (angle > 0 && angle < 178) {                                                              // If destination is close and on the right
            ShipController.SetAIturn(0.5f);
            ShipController.SetAISpeed(2);
            // Debug.Log("close right - " + distance);
            // Debug.Log("case 3 - angle : " + angle +" - TurnInputLimit - " + TurnInputLimit);
        } else if (angle <= -0 && angle > -178) {                                                            // If destination is close and on the left
            ShipController.SetAIturn(-0.5f);
            ShipController.SetAISpeed(2);
            // Debug.Log("close left - " + distance);
            // Debug.Log("case 4 - angle : " + angle +" - TurnInputLimit - " + TurnInputLimit);
        } else {                                                                                            // If close
            ShipController.SetAISpeed(4);
            ShipController.SetAIturn(0);
            // Debug.Log("close - " + distance);
            // Debug.Log("else - angle : " + angle +" - TurnInputLimit - " + TurnInputLimit);
        }
    }
    protected override void IdleAction(){
        ShipController.SetAISpeed(0);
        ShipController.SetAIturn(0);
    }
    protected override void FollowWayPointsAction(){
        // Debug.Log("Unit : "+ Name +" - UnitsAICurrentState = "+ UnitsAICurrentState);
        if (!UsesWaypoints) {
            return;
        }

        Vector3 targetDir = transform.position - Waypoints[0];
        Vector3 forward = transform.forward;
        float angle = Vector3.SignedAngle(targetDir, forward, Vector3.up);

        float distance = (transform.position -  Waypoints[0]).magnitude;
        // Debug.Log("distance : " + distance);

        if (distance > RotationSafeDistance && angle > 0 && angle < 178) {                                  // If destination is far and on the right
            ShipController.SetAIturn(0.5f);
            ShipController.SetAISpeed(4);
            // Debug.Log("far right - " + distance);
            // Debug.Log("case 1 - angle : " + angle +" - TurnInputLimit - " + TurnInputLimit);
        } else if (distance > RotationSafeDistance && angle < -0 && angle > -178) {                         // If destination is far and on the left
            ShipController.SetAIturn(-0.5f);
            ShipController.SetAISpeed(4);
            // Debug.Log("far left - " + distance);
            // Debug.Log("case 2 - angle : " + angle +" - TurnInputLimit - " + TurnInputLimit);
        } else if (distance > RotationSafeDistance ) {                                                                       // If far
            ShipController.SetAIturn(0);
            // Debug.Log("far - " + distance);
            // Debug.Log("case 3 - angle : " + angle +" - TurnInputLimit - " + TurnInputLimit);
        } else if (angle > 0 && angle < 120) {                                                              // If destination is close and extremely on the right
            ShipController.SetAIturn(0.5f);
            ShipController.SetAISpeed(1);
            // Debug.Log("close Far right - " + distance);
            // Debug.Log("case 3 - angle : " + angle +" - TurnInputLimit - " + TurnInputLimit);
        } else if (angle < -0 && angle > -120) {                                                            // If destination is close and extremely on the left
            ShipController.SetAIturn(-0.5f);
            ShipController.SetAISpeed(1);
            // Debug.Log("close Far left - " + distance);
            // Debug.Log("case 4 - angle : " + angle +" - TurnInputLimit - " + TurnInputLimit);
        } else if (angle > 0 && angle < 178) {                                                              // If destination is close and on the right
            ShipController.SetAIturn(0.5f);
            ShipController.SetAISpeed(2);
            // Debug.Log("close right - " + distance);
            // Debug.Log("case 3 - angle : " + angle +" - TurnInputLimit - " + TurnInputLimit);
        } else if (angle <= -0 && angle > -178) {                                                            // If destination is close and on the left
            ShipController.SetAIturn(-0.5f);
            ShipController.SetAISpeed(2);
            // Debug.Log("close left - " + distance);
            // Debug.Log("case 4 - angle : " + angle +" - TurnInputLimit - " + TurnInputLimit);
        } else {                                                                                            // If close
            ShipController.SetAISpeed(4);
            ShipController.SetAIturn(0);
            // Debug.Log("close - " + distance);
            // Debug.Log("else - angle : " + angle +" - TurnInputLimit - " + TurnInputLimit);
        }
    }
    protected override void FleeAction(){

    }
    protected override void BackToBaseAction(){

    }
    protected override void NoAIAction(){

    }

    public override void SetFollowedUnit(UnitMasterController followedUnitController) {
        if (followedUnitController.GetUnitCategory() == CompiledTypes.Units_categories.RowValues.ship) {
            base.SetFollowedUnit(followedUnitController);
        }
    }
    public override void SetNewMoveLocation(Vector3 waypointPosition, MapManager.RaycastHitType raycastHitType){
        if (raycastHitType == MapManager.RaycastHitType.Sea) {
            // Debug.Log(UnitMasterController.GetUnitName() +", ShipAI : SetNewMoveLocation : " + waypointPosition);
            base.SetNewMoveLocation(waypointPosition, raycastHitType);
        } else {
            // Debug.Log(UnitMasterController.GetUnitName() +", ShipAI : not water, do not continue ! ");
            return;
        }
    }
    // Unit Manager sent info
    public override void SetAITurnInputValue(float turnInputValue){ TurnInputLimit = turnInputValue; }

    public override void SetStaging(bool staging) {
        if (staging) {
            UnitsAICurrentState = UnitsAIStates.NoAI; 
        } else {
            UnitsAICurrentState = ShipAISpawnState; 
        }
        // Debug.Log("Unit : "+ Name +" - UnitsAICurrentState = "+ UnitsAICurrentState);
    }
}