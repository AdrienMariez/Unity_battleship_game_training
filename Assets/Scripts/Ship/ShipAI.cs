using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipAI : MonoBehaviour {
    private bool AIActive = true;
    private string Team;
    private string Name;                // For debug purposes
    private bool Stressed;              // Maybe this will have to change, if stressed, the unit has found a possible target and will fight it
    private float TurnInputLimit = 0;
    private float MaxTurretsRange;
    private GameObject TargetUnit;
    private ShipController ShipController;
    private TurretManager TurretManager;
    // private bool TurretManagerPresent = false;
    private List <GameObject> EnemyUnitsList = new List<GameObject>();
    public enum ShipMoveStates {
        Patrol,
        CircleTarget,
        ApproachTarget,
        Idle,
        NoAI
    }
    public ShipMoveStates AIState;

    private void Awake () {
        ShipController = GetComponent<ShipController>();
        StartCoroutine(PauseOrders());
        // GetTargets();
    }

    private void FixedUpdate() {
        if (Stressed && TargetUnit) {
            SetAITargetRange();
        }
        // Debug.Log("Unit : "+ Name +" - TargetUnit = "+ TargetUnit +" - AIState = "+ AIState);
        // Debug.Log("Stressed : "+ Stressed +" - TargetUnit = "+ TargetUnit);
        // Debug.Log("Unit : "+ Name +" - TargetUnit = "+ TargetUnit +" - AIState = "+ AIState +" - AIActive = "+ AIActive);
        if (AIActive && !OrdersPaused) {
            // Debug.Log("AIState : "+ AIState);
            if (AIState == ShipMoveStates.ApproachTarget && TargetUnit == null || AIState == ShipMoveStates.CircleTarget && TargetUnit == null) {
                SetNewTarget();
                return;
            }
            CheckState();
            StartCoroutine(PauseOrders());
            
        }
    }
    private bool OrdersPaused = false;
    IEnumerator PauseOrders(){
        // Coroutine created to prevent too much calculus for ship behaviour
        OrdersPaused = true;
        yield return new WaitForSeconds(0.3f);
        OrdersPaused = false;
    }

    private void GetTargets(){
        // Debug.Log("Unit : "+ Name +" - Team = "+ Team);
        Stressed = false;
        TargetUnit = null;
        float range = 0f;

        if (Team == "Allies" || Team == "AlliesAI") {
            string[] tagsToTarget = { "Axis", "AxisAI" };
            foreach (string tag in tagsToTarget) {
                GameObject[] possibleTargetUnits = GameObject.FindGameObjectsWithTag (tag);
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
        } else if (Team == "Axis" || Team == "AxisAI") {
            string[] tagsToTarget = { "Allies", "AlliesAI" };
            foreach (string tag in tagsToTarget) {
                GameObject[] possibleTargetUnits = GameObject.FindGameObjectsWithTag (tag);
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
        Debug.Log("Unit : "+ Name +" - TargetUnit = "+ TargetUnit);
    }

    private void SetAITargetRange(){
        float distance = (gameObject.transform.position - TargetUnit.transform.position).magnitude;
        TurretManager.SetAITargetRange(distance);
        TurretManager.SetAITargetToFireOn(TargetUnit.transform.position);
    }

    private void NoMove() {
        ShipController.SetAISpeed(0);
        ShipController.SetAIturn(0);
    }

    private void RotateTargetAndFire(){
        // Debug.Log("RotateTargetAndFire");
        // For the moment, just circle the Target at full speed
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
        // SetAITargetRange();
        // TurretManager.SetAITargetToFireOn(TargetUnit.transform.position);
        // Debug.Log("angle : "+ angle);
    }
    private void ApproachTarget(){
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

    private void IdleGoForward(){
        ShipController.SetAISpeed(4);
        ShipController.SetAIturn(0);
    }

    public void SetNewEnemyList(List <GameObject> enemiesUnitsObjectList){
        EnemyUnitsList = enemiesUnitsObjectList;
        StartCoroutine(PauseOrders());
        CheckIfTargetExists();
    }
    private void CheckIfTargetExists() {
        if (EnemyUnitsList.Contains(TargetUnit)) {
            return;
        } else if(AIActive) {
            // If unit is played or not supposed to be targeting, change behaviour here
            SetNewTarget();
        }
    }
    private void SetNewTarget() {
        // Debug.Log("Unit : "+ Name +" - Team = "+ Team);
        TargetUnit = null;
        float range = 0f;
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
        ShipController.SetCurrentTarget(TargetUnit);
        // Debug.Log("EnemyUnitsList : "+ EnemyUnitsList.Count);
        // Debug.Log("TargetUnit : "+ TargetUnit);
        

        CheckState();
    }

    private void CheckState() {
        Stressed = false;
        if (TargetUnit != null && TurretManager) {
            if (AIState == ShipMoveStates.NoAI) {
                TurretManager.SetAIHasTarget(false);
                NoMove();
            } else if (AIState == ShipMoveStates.Idle) {
                TurretManager.SetAIHasTarget(true);
                NoMove();
            } else if ((gameObject.transform.position - TargetUnit.transform.position).magnitude > MaxTurretsRange) {
                AIState = ShipMoveStates.ApproachTarget;
                TurretManager.SetAIHasTarget(false);
                ApproachTarget();
            } else {
                AIState = ShipMoveStates.CircleTarget; Stressed = true;
                TurretManager.SetAIHasTarget(true);
                RotateTargetAndFire();
            }
        } else {
            if (AIState == ShipMoveStates.NoAI || AIState == ShipMoveStates.Idle) {
                AIState = AIState;
                NoMove();
            } else {
                AIState = ShipMoveStates.Patrol;
                IdleGoForward();
            }
            if (TurretManager) {TurretManager.SetAIHasTarget(false);}
        }

        // Debug.Log("Unit : "+ Name +" - TargetUnit = "+ TargetUnit +" - AIState = "+ AIState);
        // Debug.Log("Unit : "+ Name +" - magnitude = "+ (gameObject.transform.position - TargetUnit.transform.position).magnitude +" - MaxTurretsRange = "+ MaxTurretsRange);
    }

    public void SetUnitTeam(string team){ Team = team; }
    public void SetName(string name) {
        Name = name;
        // GetTargets();
    }
    public void SetAIActive(bool activate) {
        // If player control, AI inactive
        AIActive = activate;
        ShipController.SetCurrentTarget(TargetUnit);
        // if (AIActive)
        //     GetTargets();
    }
    public void SetAITurnInputValue(float turnInputValue){ TurnInputLimit = turnInputValue; }
    public void SetTurretManager(TurretManager turretManager){
        TurretManager = turretManager;
        // TurretManagerPresent = true;
    }
    public void SetMaxTurretRange(float maxTurretsRange) { MaxTurretsRange = maxTurretsRange; CheckState(); }
}