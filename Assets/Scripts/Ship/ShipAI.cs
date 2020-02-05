using UnityEngine;
using System.Collections;

public class ShipAI : MonoBehaviour {
    private bool AIActive = true;
    private string Team;
    private string Name;                // For debug purposes
    private bool Stressed;              // Maybe this will have to change, if stressed, the unit has found a possible target and will fight it
    private float TurnInputLimit = 0;
    private bool PauseOrder = false;
    private GameObject TargetUnit;
    private ShipController ShipController;
    private TurretManager TurretManager;
    private bool TurretManagerPresent = false;
    public enum ShipMoveStates {
        Patrol,
        Circle,
        approach
    }
    private void Awake () {
        ShipController = GetComponent<ShipController>();
        // GetTargets();
    }

    private void FixedUpdate(){
        // If AI is fighting but its target is dead, find another one immediately
        if (Stressed && TargetUnit == null) {
            GetTargets();
        }
        // If AI is fighting, do business with the opposing ship 
        // Todo : add a check for if a target is found but out of range
        else if (Stressed && TargetUnit) {
            if (TurretManager)
                SetAITargetRange();
                TurretManager.SetAITargetToFireOn(TargetUnit.transform.position);
            if (!PauseOrder) {
                if (AIActive) {
                    RotateTarget();
                }
                GetTargets();
                StartCoroutine(PauseOrders());
            }
        }
        // If AI doesn't find any opponent, change stance
        else if (AIActive) {
            if (!PauseOrder && AIActive) 
            IdleGoForward();
            StartCoroutine(PauseOrders());
        }
    }
    IEnumerator PauseOrders(){
        // Coroutine created to prevent too much calculus for ship behaviour
        PauseOrder = true;
        yield return new WaitForSeconds(3);
        PauseOrder = false;
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
                TurretManager.SetAIHasTarget(true);
        }
        // Debug.Log("Unit : "+ Name +" - TargetUnit = "+ TargetUnit);
    }

    private void SetAITargetRange(){
        float distance = (gameObject.transform.position - TargetUnit.transform.position).magnitude;
        TurretManager.SetAITargetRange(distance);
    }

    private void RotateTarget(){
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
        // Debug.Log("angle : "+ angle);
    }

    private void IdleGoForward(){
        ShipController.SetAISpeed(4);
        ShipController.SetAIturn(0);
    }

    public void SetUnitTeam(string team){ Team = team; }
    public void SetName(string name) { Name = name; GetTargets(); }
    public void SetAIActive(bool activate) {
        AIActive = activate;
        // if (AIActive)
        //     GetTargets();
    }
    public void SetAITurnInputValue(float turnInputValue){ TurnInputLimit = turnInputValue; }
    public void SetTurretManager(TurretManager turretManager){ TurretManager = turretManager; TurretManagerPresent = true; }
}