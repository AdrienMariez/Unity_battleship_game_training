
using System.Collections.Generic;
using UnityEngine;
public class KillZoneBoundariesManager : MonoBehaviour {
    void OnTriggerExit(Collider collider) {
        // Debug.Log("OnTriggerExit");
        HitboxComponent targetHitboxComponent = collider.GetComponent<HitboxComponent> ();
        if (targetHitboxComponent != null) {
            targetHitboxComponent.InteractionWithKillZoneBoundaries();
        }
    }
}