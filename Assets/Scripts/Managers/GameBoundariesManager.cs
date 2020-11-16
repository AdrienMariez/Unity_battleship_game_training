
using System.Collections.Generic;
using UnityEngine;
public class GameBoundariesManager : MonoBehaviour {
    void OnTriggerEnter(Collider collider) {
        // Debug.Log("OnTriggerEnter");
        HitboxComponent targetHitboxComponent = collider.GetComponent<HitboxComponent> ();
        if (targetHitboxComponent != null) {
            targetHitboxComponent.InteractionWithGameBoundaries(true);
        }

    }
    void OnTriggerExit(Collider collider) {
        // Debug.Log("OnTriggerExit");
        HitboxComponent targetHitboxComponent = collider.GetComponent<HitboxComponent> ();
        if (targetHitboxComponent != null) {
            targetHitboxComponent.InteractionWithGameBoundaries(false);
        }
    }

}