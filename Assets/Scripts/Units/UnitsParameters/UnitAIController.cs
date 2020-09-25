using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitAIController : MonoBehaviour {
    protected bool AIActive = true;
    protected WorldUnitsManager.Teams Team;
    protected string Name;                // For debug purposes
    protected bool Stressed;              // Maybe this will have to change, if stressed, the unit has found a possible target and will fight it
    protected float TurnInputLimit = 0;
    protected float MaxTurretsRange;
    protected GameObject TargetUnit;
    protected int PlayerTargetUnitIndex = 0;
    protected GameObject PlayerSetTargetUnit;
    protected UnitMasterController UnitMasterController;
    protected TurretManager TurretManager;
    protected List <GameObject> EnemyUnitsList = new List<GameObject>();

    public enum UnitsAIStates {      // This will be the State Machine used globally for all units
        Patrol,                         // Follow a waypoint until a unit is seen
        CircleTarget,                   // turn aroung a position
        ApproachTarget,                 // Go to set position
        Idle,                           // Wait until new orders
        FollowWayPoints,                // Follow a set of coordinates
        Flee,                           // Go away from a point
        BackToBase,                      // Go back to ally point
        NoAI
    }
    protected UnitsAIStates UnitsAICurrentState = UnitsAIStates.Patrol;
    public bool UnitCanMove = true;
    public bool UnitCanShoot = true;
    public bool UnitCanSpawn = true;

    protected virtual void Awake () {
        UnitMasterController = GetComponent<UnitMasterController>();
        StartCoroutine(PauseOrders());
        // GetTargets();

        if (!GetComponent<TurretManager>()) {
            UnitCanShoot = false;
        }
        if (!GetComponent<SpawnerScriptToAttach>()) {
            UnitCanSpawn = false;
        }

        // Pre set the AI if needed
        if (!UnitCanMove && !UnitCanShoot) {
            UnitsAICurrentState = UnitsAIStates.NoAI;
        } else if (!UnitCanMove && !UnitCanShoot) {
            UnitsAICurrentState = UnitsAIStates.Idle;
        }
    }

    protected virtual void FixedUpdate() {
        // if (UnitsAICurrentState == UnitsAIStates.NoAI) {
        //     return;
        // }

        if (Stressed && TargetUnit) {
            SetAITargetRange();
        }
        // Debug.Log("Unit : "+ Name +" - TargetUnit = "+ TargetUnit +" - AIState = "+ AIState);
        // Debug.Log("Unit : "+ Name +" - AIActive = "+ AIActive);
        // Debug.Log("Unit : "+ Name +" - TargetUnit = "+ TargetUnit +" - UnitsAICurrentState = "+ UnitsAICurrentState +" - AIActive = "+ AIActive);
        if (AIActive) {
            // If AI controlled
            // Debug.Log("AIState : "+ AIState);
            if (TargetUnit == null &&  UnitCanShoot) {
                SetNewTarget();
                return;
            }
            CheckState();
            StartCoroutine(PauseOrders());
            
        } else if (!AIActive && !PlayerOrdersPaused) {
            // If player controlled
            if (Input.GetButtonDown ("SetNewTarget")) {
                // Debug.Log("SetNewTarget");
                ChangePlayerTargetIndex();
                SetPlayerSetTarget();
                StartCoroutine(PausePlayerOrders());
            }
        }
    }
    protected bool OrdersPaused = false;
    IEnumerator PauseOrders(){
        // Coroutine created to prevent too much calculus for unit behaviour
        OrdersPaused = true;
        yield return new WaitForSeconds(0.3f);
        OrdersPaused = false;
    }
    protected bool PlayerOrdersPaused = false;
    IEnumerator PausePlayerOrders(){
        // Coroutine created to prevent too much calculus for unit behaviour
        PlayerOrdersPaused = true;
        yield return new WaitForSeconds(0.3f);
        PlayerOrdersPaused = false;
    }

    protected void GetTargets(){
        Debug.Log ("GetTargets called in UnitAIController - WARNING ! This should be used with precaution !");
        // Debug.Log("Unit : "+ Name +" - Team = "+ Team);
        Stressed = false;
        TargetUnit = null;
        float range = 0f;

        if (Team == WorldUnitsManager.Teams.Allies || Team == WorldUnitsManager.Teams.AlliesAI) {
            WorldUnitsManager.Teams[] tagsToTarget = { WorldUnitsManager.Teams.Axis, WorldUnitsManager.Teams.AxisAI };
            foreach (WorldUnitsManager.Teams tag in tagsToTarget) {
                GameObject[] possibleTargetUnits = GameObject.FindGameObjectsWithTag (tag.ToString());
                foreach (GameObject gameObj in possibleTargetUnits) {
                    float distance = (gameObject.transform.position - gameObj.transform.position).magnitude;
                    if (range == 0) {
                        range = distance;
                        TargetUnit = gameObj;
                    } else if (distance < range) {
                            TargetUnit = gameObj;
                    }
                }
            }
        } else if (Team == WorldUnitsManager.Teams.Axis || Team == WorldUnitsManager.Teams.AxisAI) {
            WorldUnitsManager.Teams[] tagsToTarget = { WorldUnitsManager.Teams.Allies, WorldUnitsManager.Teams.AlliesAI };
            foreach (WorldUnitsManager.Teams tag in tagsToTarget) {
                GameObject[] possibleTargetUnits = GameObject.FindGameObjectsWithTag (tag.ToString());
                foreach (GameObject gameObj in possibleTargetUnits) {
                    float distance = (gameObject.transform.position - gameObj.transform.position).magnitude;
                    if (range == 0) {
                        range = distance;
                        TargetUnit = gameObj;
                    } else if (distance < range) {
                            TargetUnit = gameObj;
                    }
                }
            }
        }

        // If a target was found, stress the unit and activate turrets AI
        if (TargetUnit != null && TurretManager){
            // Debug.Log("Unit : "+ Name +" - TargetUnit : "+ TargetUnit);
            Stressed = true;
            if (TurretManager)
                TurretManager.SetAIHasTarget(true);
        } else {
            // Debug.Log("Unit : "+ Name);
            if (TurretManager)
                TurretManager.SetAIHasTarget(false);
        }
        // Debug.Log("Unit : "+ Name +" - TargetUnit = "+ TargetUnit);
    }

    protected void SetAITargetRange(){
        float distance = (gameObject.transform.position - TargetUnit.transform.position).magnitude;
        if (TurretManager != null) {
            TurretManager.SetAITargetRange(distance);
            TurretManager.SetAITargetToFireOn(TargetUnit.transform.position);   
        }
    }

    public void SetNewEnemyList(List <GameObject> enemiesUnitsObjectList){
        EnemyUnitsList = enemiesUnitsObjectList;
        // Debug.Log("Unit : "+ Name +" - EnemyUnitsList = "+ EnemyUnitsList.Count);
        StartCoroutine(PauseOrders());
        CheckIfTargetExists();
    }
    protected void CheckIfTargetExists() {
        if (EnemyUnitsList.Contains(TargetUnit)) {
            // Debug.Log("Unit : "+ Name +" - Target exists ! = "+ TargetUnit);
            return;
        } else {
            // If unit is played or not supposed to be targeting, change behaviour here
            PlayerSetTargetUnit = null;
            TargetUnit = null;
            UnitMasterController.SetCurrentTarget(TargetUnit);
            SetNewTarget();
        }
    }
    protected void SetNewTarget() {
        // Debug.Log("Unit : "+ Name +" - Team = "+ Team);
        TargetUnit = null;
        float range = 0f;
        if (EnemyUnitsList.Count > 0) {
            foreach (var enemyUnit in EnemyUnitsList) {
                // Debug.Log("enemyUnit : "+ enemyUnit);
                float distance = (gameObject.transform.position - enemyUnit.transform.position).magnitude;
                if (range == 0) {
                    range = distance;
                    TargetUnit = enemyUnit;
                } else if (distance < range) {
                    TargetUnit = enemyUnit;
                }
            }
        }
        UnitMasterController.SetCurrentTarget(TargetUnit);
        // Debug.Log("EnemyUnitsList : "+ EnemyUnitsList.Count);
        // Debug.Log("TargetUnit : "+ TargetUnit);    

        CheckState();
    }
    protected void ChangePlayerTargetIndex() {
        if (PlayerTargetUnitIndex >= (EnemyUnitsList.Count-1)) {
            PlayerTargetUnitIndex = 0;
        } else {
            PlayerTargetUnitIndex += 1;
        }
        // Debug.Log ("Targetable units : "+ EnemyUnitsList.Count);
        // Debug.Log ("PlayerTargetUnitIndex : "+ PlayerTargetUnitIndex);
    }
    protected void SetPlayerSetTarget() {
        // Debug.Log("EnemyUnitsList[x]"+EnemyUnitsList[PlayerTargetUnitIndex]);
        // Debug.Log("PlayerSetTargetUnit"+PlayerSetTargetUnit);
        if (PlayerSetTargetUnit == null) {
            PlayerSetTargetUnit = null;
        }
        if (PlayerTargetUnitIndex > (EnemyUnitsList.Count-1)) {
            ChangePlayerTargetIndex();
        }
        PlayerSetTargetUnit = EnemyUnitsList[PlayerTargetUnitIndex];
        TargetUnit = PlayerSetTargetUnit;
        UnitMasterController.SetCurrentTarget(TargetUnit);
        CheckState();
    }

    // AI ACTIONS
    protected virtual void CheckState() {
        /*Patrol,                         // Follow a waypoint until a unit is seen
        CircleTarget,                   // turn aroung a position
        ApproachTarget,                 // Go to set position
        Idle,                           // Wait until new orders
        FollowWayPoints,                // Follow a set of coordinates
        Flee,                           // Go away from a point
        BackToBase,                      // Go back to ally point
        NoAI*/
        CheckIfCanShoot();
        switch (UnitsAICurrentState){
            case UnitsAIStates.Patrol:
                PatrolAction();
                break;
            case UnitsAIStates.CircleTarget:
                CircleTargetAction();
                break;
            case UnitsAIStates.ApproachTarget:
                ApproachTargetAction();
                break;
            case UnitsAIStates.Idle:
                IdleAction();
                break;
            case UnitsAIStates.FollowWayPoints:
                FollowWayPointsAction();
                break;
            case UnitsAIStates.Flee:
                FleeAction();
                break;
            case UnitsAIStates.BackToBase:
                BackToBaseAction();
                break;
            case UnitsAIStates.NoAI:
                NoAIAction();
                break;
            default:
                NoAIAction();
                break;
        }
    }
    protected virtual void CheckIfCanShoot() {
        Stressed = false;
        // CHECK IF CAN SHOOT
        if (TargetUnit != null && UnitCanShoot && UnitsAICurrentState != UnitsAIStates.NoAI) {
            if ((gameObject.transform.position - TargetUnit.transform.position).magnitude < MaxTurretsRange) {
                // In this case, there is a target and we can shoot it.
                // Debug.Log("Unit : "+ Name +" - is ready to shoot");
                Stressed = true;
                TurretManager.SetAIHasTarget(true);
            } else {
                TurretManager.SetAIHasTarget(false);
            }
        }
    }

    protected virtual void PatrolAction(){

    }
    protected virtual void CircleTargetAction(){

    }
    protected virtual void ApproachTargetAction(){

    }
    protected virtual void IdleAction(){

    }
    protected virtual void FollowWayPointsAction(){

    }
    protected virtual void FleeAction(){

    }
    protected virtual void BackToBaseAction(){

    }
    protected virtual void NoAIAction(){

    }

    public void SetUnitTeam(WorldUnitsManager.Teams team){ Team = team; }
    public void SetName(string name) {
        Name = name;
        // GetTargets();
    }
    public void SetAIActive(bool activate) {
        // If player control, AI inactive
        AIActive = activate;
        UnitMasterController.SetCurrentTarget(TargetUnit);
    }
    public void SetTurretManager(TurretManager turretManager){
        TurretManager = turretManager;
        // If nothing is found in TurretManager, set the AI to not use shooting behaviours.
        if (TurretManager.GetIsEmpty()){
            UnitCanShoot = false;
        }
        // TurretManagerPresent = true;
    }
    public void SetMaxTurretRange(float maxTurretsRange) { MaxTurretsRange = maxTurretsRange; CheckState(); }
    public virtual void SetAITurnInputValue(float turnInputValue){}


    // Not yet used but will get some action soon
    protected virtual bool TrySpawn() {
        if (UnitsAICurrentState != UnitsAIStates.NoAI || UnitCanSpawn) {
            return true;
        } else { 
            return false;
        }
    }
}