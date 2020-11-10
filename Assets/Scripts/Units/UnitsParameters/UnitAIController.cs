using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitAIController : UnitParameter {
    protected bool AIActive = true;
    protected CompiledTypes.Teams Team; public void SetUnitTeam(CompiledTypes.Teams team){ Team = team; }
    protected string Name; public void SetName(string name) { Name = name; } // For debug purposes
    protected bool Stressed;              // Maybe this will have to change, if stressed, the unit has found a possible target and will fight it
    protected float TurnInputLimit = 0;
    protected float MaxTurretsRange; public void SetMaxTurretRange(float maxTurretsRange) { MaxTurretsRange = maxTurretsRange; CheckState(); }
    protected GameObject TargetUnit;
    protected int PlayerTargetUnitIndex = 0;
    protected GameObject PlayerSetTargetUnit;
    protected UnitMasterController UnitMasterController;
    protected TurretManager TurretManager;
    protected List <GameObject> EnemyUnitsList = new List<GameObject>();

    public virtual void SetAITurnInputValue(float turnInputValue){}

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

    [Header("What is this particular model allowed to do ?")]
    public bool UnitCanMove = true;
    public bool UnitCanShoot = true;
    public bool UnitCanSpawn = true;

    public virtual void BeginOperations () {
        UnitMasterController = GetComponent<UnitMasterController>();
        // GetTargets();

        // Pre set the AI if needed
        if (!UnitCanMove && !UnitCanShoot) {
            UnitsAICurrentState = UnitsAIStates.NoAI;
        } else if (!UnitCanMove && !UnitCanShoot) {
            UnitsAICurrentState = UnitsAIStates.Idle;
        }
    }
    void Start () {
        // CheckMoveAbility();
        CheckShootAbility();
        CheckSpawnAbility();

        StartCoroutine(AIOrdersLoop());
    }
    public virtual void SetAIFromUnitManager(bool unitCanMove, bool unitCanShoot, bool unitCanSpawn) {      // Start from a spawn point
        UnitCanMove = unitCanMove;
        UnitCanShoot = unitCanShoot;
        UnitCanSpawn = unitCanSpawn;
        // if (unitCanMove) {
        //     CheckMoveAbility();
        // }
        if (unitCanShoot) {
            CheckShootAbility();
        }
        if (unitCanSpawn) {
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
                SetPlayerSetTarget();
            }
        }   
    }
    IEnumerator AIOrdersLoop(){
        while (true) {
            yield return new WaitForSeconds(0.3f);
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
                TurretManager.SetAIHasTarget(true);     // Allowed to engage target
            } else {
                TurretManager.SetAIHasTarget(false);    // If the target is too far
            }
        }else {
            if (UnitCanShoot) {
                TurretManager.SetAIHasTarget(false);        // If there is no target
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

    public void SetAIActive(bool activate) {
        // Debug.Log("Unit : "+ Name +" - SetAIActive = "+ activate);
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

    protected virtual bool TrySpawn() {
        if (AIActive && UnitsAICurrentState != UnitsAIStates.NoAI && UnitCanSpawn) {
            return true;
        } else {
            return false;
        }
    }
    protected void SpawnNewUnit() {
        // Debug.Log("SpawnNewUnit");
        SpawnerScriptToAttach spawnerScript = GetComponent<SpawnerScriptToAttach>();
        int listCount = spawnerScript.GetTeamedSpawnableUnitsList().Count - 1;
        if (listCount < 0 ) { return; } // Check if list is populated
        int unitChosen = Random.Range(0, listCount);
        // Debug.Log(unitChosen +" / "+listCount);
        if (spawnerScript.TrySpawnUnit(spawnerScript.GetTeamedSpawnableUnitsList()[unitChosen])) {
            spawnerScript.SpawnUnit(spawnerScript.GetTeamedSpawnableUnitsList()[unitChosen]);
        }
        // foreach (WorldSingleUnit singleUnit in spawnerScript.GetTeamedSpawnableUnitsList()) {
        //     Debug.Log(singleUnit.GetUnitName());
        // }
    }
}