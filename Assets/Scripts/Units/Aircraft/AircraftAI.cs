using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AircraftAI : UnitAIController {
    protected AircraftController AircraftController;
    public UnitsAIStates AircraftAISpawnState = UnitsAIStates.Patrol;
    protected float RotationSafeDistance;  // this var is used to determine if a waypoint is inside a turning arc of a full speed ship
    public override void BeginOperations (bool aiMove, bool aiShoot, bool aiSpawn) {
        UnitsAICurrentState = AircraftAISpawnState;
        // Still need the specific unit Controller for specific methods
        AircraftController = GetComponent<AircraftController>();
        // RotationSafeDistance = ShipController.GetUnitWorldSingleUnit().GetBuoyancyRotationTime() * 6;
        // Debug.Log("Unit : "+ Name +"RotationSafeDistance: " + RotationSafeDistance);
        base.BeginOperations(aiMove, aiShoot, aiSpawn);
    }
    // UnitsAIStates
    // UnitsAICurrentState
    protected override void CheckState() {
        // if (UnitsAICurrentState == UnitsAIStates.NoAI) {
        //     return;
        // }
        // If there is a target
        if (Waypoints.Count > 0 && UsesWaypoints && UnitCanMove) {
            UnitsAICurrentState = UnitsAIStates.FollowWayPoints;
        } else if (UnitsAICurrentState == UnitsAIStates.Takeoff) {
            //Don't touch anything if the plane is taking off
        } else if (TargetUnit != null && UnitCanShoot) {
            // I'm not sure about this one... The logic is : if it's one of the correct states, check if the target is in range or not. Act accordingly
            if (UnitsAICurrentState == UnitsAIStates.ApproachTarget || UnitsAICurrentState == UnitsAIStates.CircleTarget || UnitsAICurrentState == UnitsAIStates.Patrol) {
                if ((gameObject.transform.position - TargetUnit.transform.position).magnitude > MaxTurretsRange) {
                    UnitsAICurrentState = UnitsAIStates.ApproachTarget;
                    // Debug.Log("Unit : "+ Name +" UnitsAICurrentState = "+ UnitsAICurrentState);
                } else {
                    UnitsAICurrentState = UnitsAIStates.CircleTarget;
                    // Debug.Log("Unit : "+ Name +" UnitsAICurrentState = "+ UnitsAICurrentState);
                }
            }
        } else {
            if (UnitsAICurrentState == UnitsAIStates.ApproachTarget || UnitsAICurrentState == UnitsAIStates.CircleTarget || UnitsAICurrentState == UnitsAIStates.Patrol) {
                UnitsAICurrentState = UnitsAIStates.Patrol;
                // Debug.Log("Unit : "+ Name +" UnitsAICurrentState = "+ UnitsAICurrentState);
            } else {
                UnitsAICurrentState = UnitsAIStates.Idle;
                // Debug.Log("Unit : "+ Name +" UnitsAICurrentState = "+ UnitsAICurrentState);
            }
        }

        // if (UnitsAICurrentState != UnitsAIStates.Idle && UnitsAICurrentState != UnitsAIStates.NoAI) {
        //     if (!UnitCanMove) {
        //         UnitsAICurrentState = UnitsAIStates.Idle;
        //         // Debug.Log("Unit : "+ Name +" UnitsAICurrentState = "+ UnitsAICurrentState);
        //     }
        // }

        base.CheckState();

        // if (AIActive) {
        //     Debug.Log("Unit : "+ Name +" - TargetUnit = "+ TargetUnit +" - UnitsAICurrentState = "+ UnitsAICurrentState);
        // }

        // Debug.Log("Unit : "+ Name +" - magnitude = "+ (gameObject.transform.position - TargetUnit.transform.position).magnitude +" - MaxTurretsRange = "+ MaxTurretsRange);
    }

    // Possible Actions
    protected override void PatrolAction(){
        // Not done at all
        AircraftController.SetAISpeed(1);
        // ShipController.SetAIturn(0);
    }
    protected override void CircleTargetAction(){
        AircraftController.SetAISpeed(1);

        Vector3 targetDir = gameObject.transform.position - TargetUnit.transform.position;
        Vector3 forward = gameObject.transform.forward;
        float angle = Vector3.SignedAngle(targetDir, forward, Vector3.up);

        if (angle > 95 && angle > 0 && TurnInputLimit < 1 || angle > -85 && angle < 0 && TurnInputLimit < 1) {
            // ShipController.SetAIturn(-0.5f);
        } else if (angle < 85  && angle > 0 && TurnInputLimit > -1 || angle < -95 && angle < 0 && TurnInputLimit > -1) {
            // ShipController.SetAIturn(0.5f);
        } else {
            // ShipController.SetAIturn(0);
        }
    }
    protected override void ApproachTargetAction(){
                // Debug.Log("ApproachTarget");
        AircraftController.SetAISpeed(1);

        Vector3 targetDir = gameObject.transform.position - TargetUnit.transform.position;
        Vector3 forward = gameObject.transform.forward;
        float angle = Vector3.SignedAngle(targetDir, forward, Vector3.up);

        // Not tested !
        if (angle > 5 && angle < 180 && TurnInputLimit < 1) {
            // ShipController.SetAIturn(-0.5f);
        } else if (angle < -5 && angle > -180 && TurnInputLimit > -1) {
            // ShipController.SetAIturn(0.5f);
        } else {
            // ShipController.SetAIturn(0);
        }
        // Debug.Log("angle : "+ angle);
    }
    protected override void IdleAction(){
        AircraftController.SetAISpeed(0);
        // ShipController.SetAIturn(0);
    }
    protected override void FollowWayPointsAction(){
        // Debug.Log("Unit : "+ Name +" - UnitsAICurrentState = "+ UnitsAICurrentState);
        AircraftController.SetAISpeed(1);
        if (!UsesWaypoints) {
            return;
        }

        Vector3 targetDir = gameObject.transform.position - Waypoints[0];
        Vector3 forward = gameObject.transform.forward;
        float angle = Vector3.SignedAngle(targetDir, forward, Vector3.up);

        float distance = (gameObject.transform.position -  Waypoints[0]).magnitude;
        // Debug.Log("distance : " + distance);

        if (distance > RotationSafeDistance && angle > 0 && angle < 178) {                                  // If destination is far and on the right
            // ShipController.SetAIturn(0.5f);
            // ShipController.SetAISpeed(4);
            // Debug.Log("far right - " + distance);
            // Debug.Log("case 1 - angle : " + angle +" - TurnInputLimit - " + TurnInputLimit);
        } else if (distance > RotationSafeDistance && angle < -0 && angle > -178) {                         // If destination is far and on the left
            // ShipController.SetAIturn(-0.5f);
            // ShipController.SetAISpeed(4);
            // Debug.Log("far left - " + distance);
            // Debug.Log("case 2 - angle : " + angle +" - TurnInputLimit - " + TurnInputLimit);
        } else if (distance > RotationSafeDistance ) {                                                                       // If far
            // ShipController.SetAIturn(0);
            // Debug.Log("far - " + distance);
            // Debug.Log("case 3 - angle : " + angle +" - TurnInputLimit - " + TurnInputLimit);
        } else if (angle > 0 && angle < 120) {                                                              // If destination is close and extremely on the right
            // ShipController.SetAIturn(0.5f);
            // ShipController.SetAISpeed(1);
            // Debug.Log("close Far right - " + distance);
            // Debug.Log("case 3 - angle : " + angle +" - TurnInputLimit - " + TurnInputLimit);
        } else if (angle < -0 && angle > -120) {                                                            // If destination is close and extremely on the left
            // ShipController.SetAIturn(-0.5f);
            // ShipController.SetAISpeed(1);
            // Debug.Log("close Far left - " + distance);
            // Debug.Log("case 4 - angle : " + angle +" - TurnInputLimit - " + TurnInputLimit);
        } else if (angle > 0 && angle < 178) {                                                              // If destination is close and on the right
            // ShipController.SetAIturn(0.5f);
            // ShipController.SetAISpeed(2);
            // Debug.Log("close right - " + distance);
            // Debug.Log("case 3 - angle : " + angle +" - TurnInputLimit - " + TurnInputLimit);
        } else if (angle <= -0 && angle > -178) {                                                            // If destination is close and on the left
            // ShipController.SetAIturn(-0.5f);
            // ShipController.SetAISpeed(2);
            // Debug.Log("close left - " + distance);
            // Debug.Log("case 4 - angle : " + angle +" - TurnInputLimit - " + TurnInputLimit);
        } else {                                                                                            // If close
            // ShipController.SetAISpeed(4);
            // ShipController.SetAIturn(0);
            // Debug.Log("close - " + distance);
            // Debug.Log("else - angle : " + angle +" - TurnInputLimit - " + TurnInputLimit);
        }
        // if (angle < 5 && angle > 180 && TurnInputLimit > -1) {
        //     ShipController.SetAIturn(-0.5f);
        //     Debug.Log("case 1 - angle : " + angle);
        // } else if (angle > -5 && angle < -180 && TurnInputLimit < 1) {
        //     Debug.Log("case 2 - angle : " + angle);
        //     ShipController.SetAIturn(0.5f);
        // } else {
        //     Debug.Log("else - angle : " + angle);
        //     ShipController.SetAIturn(0);
        // }
    }
    protected override void FleeAction(){
        AircraftController.SetAISpeed(1);
    }
    protected override void BackToBaseAction(){
        AircraftController.SetAISpeed(1);
    }
    protected override void TakeoffAction(){
        // Debug.Log("Unit : "+ Name +" - UnitsAICurrentState = "+ UnitsAICurrentState);
        AircraftController.SetAISpeed(1);
        AircraftController.SetAIPitch(-1);
        // Take off; wait for X time then switch ai for what is specified as the basic AI.
        //It's logical that there can be no planes flying after a takeoff that won't try to fly so do not care about NoAI edgecase.
        StartCoroutine(TakeoffActionPauseLogic());
        // UnitsAICurrentState = AircraftAISpawnState;
    }
    IEnumerator TakeoffActionPauseLogic(){
        yield return new WaitForSeconds(AircraftController.m_TimeBeforePlayerControl);
        TakeoffActionEnd();
    }
    protected void TakeoffActionEnd(){
        AircraftController.SetPhysicalAsNormal();
        AircraftController.SetAIPitch(0);
        UnitsAICurrentState = AircraftAISpawnState;
        CheckState();
    }
    protected override void LandingAction(){
        AircraftController.SetAISpeed(0);
    }
    protected override void NoAIAction(){
        AircraftController.SetAISpeed(0);
    }


    public override void SetNewMoveLocation(Vector3 waypointPosition, MapManager.RaycastHitType raycastHitType){
        base.SetNewMoveLocation(waypointPosition, raycastHitType);
        // if (raycastHitType == MapManager.RaycastHitType.Sea) {
        //     // Debug.Log(UnitMasterController.GetUnitName() +", ShipAI : SetNewMoveLocation : " + waypointPosition);
        //     base.SetNewMoveLocation(waypointPosition, raycastHitType);
        // } else {
        //     // Debug.Log(UnitMasterController.GetUnitName() +", ShipAI : not water, do not continue ! ");
        //     return;
        // }
    }
    // Unit Manager sent info
    public override void SetAITurnInputValue(float turnInputValue){ TurnInputLimit = turnInputValue; }

    public override void SetAIActive(bool activate) {
        base.SetAIActive(activate);
        CheckState();
    }
    public override void SetStaging(bool staging) {
        if (staging) {
            UnitsAICurrentState = UnitsAIStates.NoAI; 
        } else {
            UnitsAICurrentState = UnitsAIStates.Takeoff;
        }

        // Debug.Log("Unit : "+ Name +" - UnitsAICurrentState = "+ UnitsAICurrentState);
    }
}