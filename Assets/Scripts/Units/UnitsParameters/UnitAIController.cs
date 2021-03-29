using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitAIController : UnitParameter {
    protected bool AIActive = true;
    protected CompiledTypes.Teams Team; public void SetUnitTeam(CompiledTypes.Teams team){ Team = team; }
    protected string Name; public void SetName(string name) { Name = name; } // For debug purposes
    protected bool Stressed;              // Maybe this will have to change, if stressed, the unit has found a possible target and will fight it
    protected float TurnInputLimit = 0;
    protected float MaxTurretsRange; public void SetMaxTurretRange(float maxTurretsRange) { MaxTurretsRange = maxTurretsRange; }
    protected GameObject TargetUnit;
    protected int PlayerTargetUnitIndex = 0;
    protected GameObject PlayerSetTargetUnit;
    protected UnitMasterController UnitMasterController;
    protected TurretManager TurretManager;
    protected SpawnerScriptToAttach SpawnerScript;
    protected PlaneWeaponsManager PlaneWeaponsManager; public void SetPlaneWeaponsManager(PlaneWeaponsManager _s){ PlaneWeaponsManager = _s; }
    protected List <GameObject> EnemyUnitsList = new List<GameObject>();

    public virtual void SetAITurnInputValue(float turnInputValue){}

    public enum UnitsAIStates {      // This will be the State Machine used globally for all units
        Patrol,                         // Follow a waypoint until a unit is seen
        CircleTarget,                   // turn aroung a position
        ApproachTarget,                 // Go to set position
        Follow,                // Get into a formation with a fellow Unit
        Idle,                           // Wait until new orders
        FollowWayPoints,                // Follow a set of coordinates
        Flee,                           // Go away from a point
        BackToBase,                      // Go back to ally point
        NoAI,
        Takeoff,
        Landing
    }
    protected UnitsAIStates UnitsAICurrentState = UnitsAIStates.Patrol;

    protected UnitMasterController FollowedUnit;
    protected Vector3 FollowedUnitSelfPositionning;         // The position the unit will try to maintain relative to the followed unit
    protected bool FollowsUnit = false;
    protected List <Vector3> Waypoints = new List<Vector3>();
    protected bool UsesWaypoints = false;

    [Header("What is this particular model allowed to do ?")]
    protected bool UnitCanMove = true; 
    protected bool UnitCanShoot = true; 
    protected bool UnitCanSpawn = true;
    protected bool ChidrenCanMove = true; public bool GetChidrenCanMove() { return ChidrenCanMove; }
    protected bool ChidrenCanShoot = true; public bool GetChidrenCanShoot() { return ChidrenCanShoot; }
    protected bool ChidrenCanSpawn = true; public bool GetChidrenCanSpawn() { return ChidrenCanSpawn; }

    public virtual void BeginOperations (bool aiMove, bool aiShoot, bool aiSpawn) {
        // Debug.Log ("BeginOperations "+Name+" Sent AI : "+aiMove+" : "+aiShoot+" : "+aiSpawn);
        UnitCanMove = aiMove;
        UnitCanShoot = aiShoot;
        UnitCanSpawn = aiSpawn;
        ChidrenCanMove = aiMove;        // Children AI options should be untouched so they can transmit their parameters to future spawned units
        ChidrenCanShoot = aiShoot;
        ChidrenCanSpawn = aiSpawn;
        // Debug.Log ("BeginOperations "+Name+" Sent Chidren AI : "+ChidrenCanMove+" : "+ChidrenCanShoot+" : "+ChidrenCanSpawn);
        UnitMasterController = GetComponent<UnitMasterController>();
        // GetTargets();

        // Pre set the AI if needed
        if (!UnitCanMove && !UnitCanShoot) {
            UnitsAICurrentState = UnitsAIStates.NoAI;
        } else if (!UnitCanMove && !UnitCanShoot) {
            UnitsAICurrentState = UnitsAIStates.Idle;
        }
        // Check if the AI is able to do set orders
        if (UnitCanShoot) {
            CheckShootAbility();
        }
        if (UnitCanSpawn) {
            CheckSpawnAbility();
        }
        // Debug.Log ("BeginOperations "+Name+" AI : "+UnitCanMove+" : "+UnitCanShoot+" : "+UnitCanSpawn);
        StartCoroutine(AIOrdersLoop());
    }
    public virtual void SetAIFromUnitManager(bool aiMove, bool aiShoot, bool aiSpawn) {      // Start from a spawn point
        UnitCanMove = aiMove;
        UnitCanShoot = aiShoot;
        UnitCanSpawn = aiSpawn;
        // if (unitCanMove) {
        //     CheckMoveAbility();
        // }
        if (UnitCanShoot) {
            CheckShootAbility();
        }
        if (UnitCanSpawn) {
            CheckSpawnAbility();
        }
    }
    // protected void CheckMoveAbility() {
    // }
    protected void CheckShootAbility() {
        if (!GetComponent<TurretManager>()) {
            // Debug.Log("Unit : "+ Name +"can't shoot");
            UnitCanShoot = false;
        }
    }
    protected void CheckSpawnAbility() {
        if (!GetComponent<SpawnerScriptToAttach>()) {
            // Debug.Log("Unit : "+ Name +"can't spawn");
            UnitCanSpawn = false;
        } else {
            SpawnerScript = GetComponent<SpawnerScriptToAttach>();
            StartCoroutine(TrySpawnLoop());
        }
    }

    protected virtual void FixedUpdate() {
        // if (UnitsAICurrentState == UnitsAIStates.NoAI) {
        //     return;
        // }
        if (Stressed && TargetUnit) {
            SetAITargetRange();
        }
        // Debug.Log("Unit : "+ Name +" - SpawningPaused = "+ SpawningPaused +" - OrdersPaused = "+ OrdersPaused);
        // Debug.Log("Unit : "+ Name +" - TargetUnit = "+ TargetUnit +" - AIState = "+ AIState);
        // Debug.Log("Unit : "+ Name +" - AIActive = "+ AIActive);
        // Debug.Log("Unit : "+ Name +" - TargetUnit = "+ TargetUnit +" - UnitsAICurrentState = "+ UnitsAICurrentState +" - AIActive = "+ AIActive);
        if (!AIActive) {
            // If player controlled
            if (Input.GetButtonUp ("SetNewTarget")) {
                // Debug.Log("SetNewTarget");
                ChangePlayerTargetIndex();
                SetPlayerSetTargetByIndex();
            }
        }
        if (UnitsAICurrentState == UnitsAIStates.FollowWayPoints && UsesWaypoints) {
            float distance = (gameObject.transform.position -  Waypoints[0]).magnitude;
            if (distance < 50) {
                MoveCheckPointReached();
            }
        }
    }
    protected virtual IEnumerator AIOrdersLoop(){
        while (true) {
            yield return new WaitForSeconds(4f);
            if (AIActive) {
                // Debug.Log("PauseAIOrders");
                // If AI controlled
                if (TargetUnit == null &&  UnitCanShoot) {
                    SetNewTarget();
                }
                CheckState();
            }
        }
    }
    IEnumerator TrySpawnLoop(){
        while (true) {
            yield return new WaitForSeconds(5f);
            if (TrySpawn()) {
                SpawnNewUnit();
            }
        }
    }

    protected void GetTargets(){
        Debug.Log ("GetTargets called in UnitAIController - WARNING ! This should be used with precaution !");
        // Debug.Log("Unit : "+ Name +" - Team = "+ Team.id);
        Stressed = false;
        TargetUnit = null;
        float range = 0f;

        if (Team.id == WorldUnitsManager.GetDB().Teams.Allies.id) {
            CompiledTypes.Teams[] tagsToTarget = { WorldUnitsManager.GetDB().Teams.Axis };
            foreach (CompiledTypes.Teams tag in tagsToTarget) {
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
        } else if (Team.id == WorldUnitsManager.GetDB().Teams.Axis.id) {
            CompiledTypes.Teams[] tagsToTarget = { WorldUnitsManager.GetDB().Teams.Allies };
            foreach (CompiledTypes.Teams tag in tagsToTarget) {
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
        CheckIfTargetExists();
    }
    protected void CheckIfTargetExists() {
        if (EnemyUnitsList.Contains(PlayerSetTargetUnit)) {
            // Debug.Log("Case 1 : Unit : "+ Name +" - Target exists ! = "+ TargetUnit);
            UnitMasterController.SetCurrentTarget(PlayerSetTargetUnit);
            return;
        } else {
            // Debug.Log("Case 2 : Unit : "+ Name +" - Target exists ! = "+ TargetUnit);
            // If unit is played or not supposed to be targeting, change behaviour here
            PlayerSetTargetUnit = null;
            TargetUnit = null;
            UnitMasterController.SetCurrentTarget(PlayerSetTargetUnit);
            SetNewTarget();
        }
    }
    protected virtual void SetNewTarget() {
        // Debug.Log("Unit : "+ Name +" - Team.id = "+ Team.id);
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
        // UnitMasterController.SetCurrentTarget(TargetUnit);
        // Debug.Log("EnemyUnitsList : "+ EnemyUnitsList.Count);
        // Debug.Log("TargetUnit : "+ TargetUnit);    

        CheckState();
    }
    protected void SetPlayerSetTargetByIndex() {
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
        UnitMasterController.SetCurrentTarget(PlayerSetTargetUnit);
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
    public virtual void SetPlayerSetTargetByController(UnitMasterController targetedUnitController) {
        // An attack order set by the map
        // Debug.Log("EnemyUnitsList[x]"+EnemyUnitsList[PlayerTargetUnitIndex]);
        // Debug.Log("PlayerSetTargetUnit"+PlayerSetTargetUnit);
        // if (UnitCanShoot) {   
            for (int i = 0; i < EnemyUnitsList.Count; i++) {
                if (targetedUnitController.GetUnitModel() == EnemyUnitsList[i]) {
                    PlayerTargetUnitIndex = i; 
                }
            }
            PlayerSetTargetUnit = targetedUnitController.GetUnitModel();
            TargetUnit = PlayerSetTargetUnit;
            
            UnitMasterController.SetCurrentTarget(PlayerSetTargetUnit);
            CheckState();
        // }
    }
    public virtual void CleanPlayerSetAttackTarget() {
        // Player Set Target removed from the map
        // if (UnitCanShoot) {  
            PlayerSetTargetUnit = null;
            SetNewTarget();
            UnitMasterController.SetCurrentTarget(null);
        // }
    }
    public virtual void SetFollowedUnit(UnitMasterController followedUnitController) {
        // A move order set by the map towards an allied unit
        // Debug.Log("EnemyUnitsList[x]"+EnemyUnitsList[PlayerTargetUnitIndex]);
            CleanMoveOrders();
            FollowsUnit = true;
            FollowedUnit = followedUnitController;
            UnitMasterController.AICallbackCurrentFollowedUnit(followedUnitController);
            CheckState();
        // }
    }
    public virtual void SetNewMoveLocation(Vector3 waypointPosition, MapManager.RaycastHitType raycastHitType) {
        // A move order set by the map, overrides check if the location is for the unit category
        // if (UnitCanMove) {                   // A unit with AI disabled for moving can still be ordered to move !
            Waypoints.Add(waypointPosition);
            UsesWaypoints = true;
            UnitMasterController.AICallbackCurrentWaypoints(Waypoints);
            CheckState();
        // }
    }
    public virtual void CleanMoveOrders() {
        // All move orders removed from the map
        if (UnitCanMove) {
            Waypoints.Clear();
            UsesWaypoints = false;
            UnitMasterController.AICallbackCurrentWaypoints(Waypoints);

            FollowsUnit = false;
            FollowedUnit = null;
            UnitMasterController.AICallbackCurrentFollowedUnit(FollowedUnit);

            CheckState();
        }
    }
    protected void MoveCheckPointReached() {
        // Debug.Log("Checkpoint reached ! Timer : "+Time.time);
        if (Waypoints.Count > 1) {
            Waypoints.Remove(Waypoints[0]);
            // Debug.Log(" case 1");
        } else {
            Waypoints.Clear();
            UsesWaypoints = false;
            // Debug.Log(" case 2");
        }
        UnitMasterController.AICallbackCurrentWaypoints(Waypoints);
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
            case UnitsAIStates.Follow:
                FollowAction();
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
            case UnitsAIStates.Takeoff:
                TakeoffAction();
                break;
            case UnitsAIStates.Landing:
                LandingAction();
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
                TurretManager.SetAIHasTarget(true);     // Allowed to engage target
            } else {
                // Debug.Log("Unit : "+ Name +" - TurretManager : "+TurretManager);
                TurretManager.SetAIHasTarget(false);    // If the target is too far
            }
        }else {
            if (UnitCanShoot) {
                TurretManager.SetAIHasTarget(false);        // If there is no target
            }
        }
    }

    protected virtual void PatrolAction(){ }
    protected virtual void CircleTargetAction(){ }
    protected virtual void ApproachTargetAction(){ }
    protected virtual void FollowAction(){ }
    protected virtual void IdleAction(){ }
    protected virtual void FollowWayPointsAction(){ }
    protected virtual void FleeAction(){ }
    protected virtual void BackToBaseAction(){ }
    protected virtual void TakeoffAction(){ }
    protected virtual void LandingAction(){ }
    protected virtual void NoAIAction(){ }

    public virtual void SetAIActive(bool activate) {
        // Debug.Log("Unit : "+ Name +" - SetAIActive = "+ activate);
        // If player control, AI inactive
        AIActive = activate;
        UnitMasterController.SetCurrentTarget(PlayerSetTargetUnit);
    }
    public virtual void SetStaging(bool staging) { }
    public void SetTurretManager(TurretManager turretManager){
        TurretManager = turretManager;
        // If nothing is found in TurretManager, set the AI to not use shooting behaviours.
        if (TurretManager.GetIsEmpty()){
            UnitCanShoot = false;
        }
        // TurretManagerPresent = true;
    }

    protected virtual bool TrySpawn() {
        if (AIActive && UnitsAICurrentState != UnitsAIStates.NoAI && UnitCanSpawn) {
            return true;
        } else {
            return false;
        }
    }
    protected void SpawnNewUnit() {
        // Debug.Log("SpawnNewUnit");
        int listCount = SpawnerScript.GetTeamedSpawnableUnitsList().Count - 1;
        if (listCount < 0 ) { return; } // Check if list is populated
        int unitChosen = Random.Range(0, listCount);
        // Debug.Log(unitChosen +" / "+listCount);
        if (SpawnerScript.StagingUnitList.Count == 0) {
            SpawnerScript.TrySpawn(SpawnerScript.GetTeamedSpawnableUnitsList()[unitChosen], true);
        }
        // if (SpawnerScript.TrySpawnUnit(SpawnerScript.GetTeamedSpawnableUnitsList()[unitChosen])) {
        //     SpawnerScript.SpawnUnit(SpawnerScript.GetTeamedSpawnableUnitsList()[unitChosen], ChidrenCanMove, ChidrenCanShoot, ChidrenCanSpawn);
        // }
        // foreach (WorldSingleUnit singleUnit in SpawnerScript.GetTeamedSpawnableUnitsList()) {
        //     Debug.Log(singleUnit.GetUnitName());
        // }
    }
}