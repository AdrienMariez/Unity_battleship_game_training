using UnityEngine;
using System.Collections;

public class ShipAI : MonoBehaviour {
    private bool AIActive = true;
    private string Team;
    private string Name;                // For debug purposes
    private float TurnInputLimit = 0;
    private bool PauseRotation = true;
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
    }

    private void FixedUpdate(){
        if (AIActive && PauseRotation) {
            RotateTarget();
            if (TurretManager) {
                TurretManager.SetAITargetToFireOn(TargetUnit);
            }
            StartCoroutine(PauseRotate());
        }
    }
    IEnumerator PauseRotate(){
        // Coroutine created to prevent too much calculus for ship behaviour
        PauseRotation = false;
        yield return new WaitForSeconds(3);
        PauseRotation = true;
    }

    private void GetTargets(){
        // Debug.Log("Unit : "+ Name +" - Team = "+ Team);

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
        }
        if (Team == "Axis" || Team == "AxisAI") {
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
        // Debug.Log("Unit : "+ Name +" - TargetUnit = "+ TargetUnit);
    }

    private void RotateTarget(){
        // For the moment, just circle the Target
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

    private void AISwitchMoveState(){
        // For the moment, just go at full speed
        ShipController.SetAISpeed(4);
    }

    public void SetUnitTeam(string team){ Team = team; }
    public void SetName(string name) { Name = name; GetTargets(); }
    public void SetAIActive(bool activate) {
        AIActive = activate;
        if (AIActive) {
            GetTargets();
            AISwitchMoveState();
        }
    }
    public void SetAITurnInputValue(float turnInputValue){ TurnInputLimit = turnInputValue; }
    public void SetTurretManager(TurretManager turretManager){ TurretManager = turretManager; TurretManagerPresent = true; }
}