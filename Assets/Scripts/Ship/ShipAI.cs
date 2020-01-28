using UnityEngine;

public class ShipAI : MonoBehaviour {
    private bool AIActive = true;
    private Vector3 AIGroundTargetPosition;
    private string Team;
    private string Name;                // For debug purposes
    private GameObject TargetUnit;
    public enum ShipMoveStates {
        Patrol,
        Circle,
        approach
    }
    private void Awake () {
    }

    private void FixedUpdate(){
    }

    private void GetTargets(){
        Debug.Log("Unit : "+ Name +"Team = "+ Team);

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
        Debug.Log("Unit : "+ Name +"TargetUnit = "+ TargetUnit);
    }

    private void AISwitchMoveState(){
        // If 
    }

    public Vector3 GetAIGroundTargetPosition(){
        return AIGroundTargetPosition;
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
}