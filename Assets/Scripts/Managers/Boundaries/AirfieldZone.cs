
using System.Collections.Generic;
using UnityEngine;
public class AirfieldZone : MonoBehaviour {
    void OnTriggerEnter(Collider collider) {
        // Debug.Log("OnTriggerEnter");
        HitboxComponent targetHitboxComponent = collider.GetComponent<HitboxComponent> ();
        if (targetHitboxComponent != null) {
            targetHitboxComponent.InteractionWithAirfield(true);
        }

    }
    void OnTriggerExit(Collider collider) {
        // Debug.Log("OnTriggerExit");
        HitboxComponent targetHitboxComponent = collider.GetComponent<HitboxComponent> ();
        if (targetHitboxComponent != null) {
            targetHitboxComponent.InteractionWithAirfield(false);
        }
    }

}