using UnityEngine;

public class ShipAI : MonoBehaviour {

    private Vector3 AIGroundTargetPosition;

    private void Awake () {

    }

    private void FixedUpdate(){

    }

    private Vector3 AIGroundTarget(){
        // Find all possible targets here
        return AIGroundTargetPosition;
    }

    public Vector3 GetAIGroundTargetPosition(){
        AIGroundTargetPosition = AIGroundTarget();
        return AIGroundTargetPosition;
    }
}