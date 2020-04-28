using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingAI : MonoBehaviour {
    private bool AIActive = true;
    private string Team;
    private string Name;                // For debug purposes
    private bool Stressed;              // Maybe this will have to change, if stressed, the unit has found a possible target and will fight it
    private float TurnInputLimit = 0;
    private float MaxTurretsRange;
    private GameObject TargetUnit;
    private int PlayerTargetUnitIndex = 0;
    private GameObject PlayerSetTargetUnit;
    private BuildingController BuildingController;
    private TurretManager TurretManager;
    // private bool TurretManagerPresent = false;
    private List <GameObject> EnemyUnitsList = new List<GameObject>();
    public enum BuildingAIStates {
        Fight,
        NoAI
    }
    public BuildingAIStates AIState;

    private void Awake () {
        BuildingController = GetComponent<BuildingController>();
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
            // If AI controlled
            // Debug.Log("AIState : "+ AIState);
            if (AIState == BuildingAIStates.Fight) {
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
    private bool OrdersPaused = false;
    IEnumerator PauseOrders(){
        // Coroutine created to prevent too much calculus for ship behaviour
        OrdersPaused = true;
        yield return new WaitForSeconds(0.3f);
        OrdersPaused = false;
    }
    private bool PlayerOrdersPaused = false;
    IEnumerator PausePlayerOrders(){
        // Coroutine created to prevent too much calculus for ship behaviour
        PlayerOrdersPaused = true;
        yield return new WaitForSeconds(0.3f);
        PlayerOrdersPaused = false;
    }

    private void GetTargets(){
        Debug.Log ("GetTargets called in ShipAI - WARNING ! This should be used with precaution !");
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
        // Debug.Log("Unit : "+ Name +" - TargetUnit = "+ TargetUnit);
    }

    private void SetAITargetRange(){
        float distance = (gameObject.transform.position - TargetUnit.transform.position).magnitude;
        TurretManager.SetAITargetRange(distance);
        TurretManager.SetAITargetToFireOn(TargetUnit.transform.position);
    }

    public void SetNewEnemyList(List <GameObject> enemiesUnitsObjectList){
        EnemyUnitsList = enemiesUnitsObjectList;
        // Debug.Log("Unit : "+ Name +" - EnemyUnitsList = "+ EnemyUnitsList.Count);
        StartCoroutine(PauseOrders());
        CheckIfTargetExists();
    }
    private void CheckIfTargetExists() {
        if (EnemyUnitsList.Contains(TargetUnit)) {
            // Debug.Log("Unit : "+ Name +" - Target exists ! = "+ TargetUnit);
            return;
        } else {
            // If unit is played or not supposed to be targeting, change behaviour here
            PlayerSetTargetUnit = null;
            TargetUnit = PlayerSetTargetUnit;
            BuildingController.SetCurrentTarget(TargetUnit);
            SetNewTarget();
        }
    }
    private void SetNewTarget() {
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
        BuildingController.SetCurrentTarget(TargetUnit);
        // Debug.Log("EnemyUnitsList : "+ EnemyUnitsList.Count);
        // Debug.Log("TargetUnit : "+ TargetUnit);
        

        CheckState();
    }
    private void ChangePlayerTargetIndex() {
        if (PlayerTargetUnitIndex >= (EnemyUnitsList.Count-1)) {
            PlayerTargetUnitIndex = 0;
        } else {
            PlayerTargetUnitIndex += 1;
        }
        // Debug.Log ("Targetable units : "+ EnemyUnitsList.Count);
        // Debug.Log ("PlayerTargetUnitIndex : "+ PlayerTargetUnitIndex);
    }
    private void SetPlayerSetTarget() {
        // Debug.Log("EnemyUnitsList[x]"+EnemyUnitsList[PlayerTargetUnitIndex]);
        // Debug.Log("PlayerSetTargetUnit"+PlayerSetTargetUnit);
        // if (EnemyUnitsList[PlayerTargetUnitIndex] == PlayerSetTargetUnit) {
        //     ChangePlayerTargetIndex();
        // }
        if (PlayerSetTargetUnit == null) {
            PlayerSetTargetUnit = null;
        }
        if (PlayerTargetUnitIndex > (EnemyUnitsList.Count-1)) {
            ChangePlayerTargetIndex();
        }
        PlayerSetTargetUnit = EnemyUnitsList[PlayerTargetUnitIndex];
        TargetUnit = PlayerSetTargetUnit;
        BuildingController.SetCurrentTarget(TargetUnit);
        CheckState();
    }

    private void CheckState() {
        Stressed = false;
        if (TargetUnit != null && TurretManager) {
            if (AIState == BuildingAIStates.NoAI) {
                TurretManager.SetAIHasTarget(false);
            } else if ((gameObject.transform.position - TargetUnit.transform.position).magnitude > MaxTurretsRange) {
                TurretManager.SetAIHasTarget(false);
            } else {
                Stressed = true;
                TurretManager.SetAIHasTarget(true);
            }
        } else {
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
        BuildingController.SetCurrentTarget(TargetUnit);
        // If player takes control, Player target becomes AI target ?
        // if (!AIActive)
        //     PlayerSetTargetUnit = TargetUnit;
        // if (AIActive)
        //     GetTargets();
    }
    public void SetTurretManager(TurretManager turretManager){
        TurretManager = turretManager;
        // TurretManagerPresent = true;
    }
    public void SetMaxTurretRange(float maxTurretsRange) { MaxTurretsRange = maxTurretsRange; CheckState(); }
}