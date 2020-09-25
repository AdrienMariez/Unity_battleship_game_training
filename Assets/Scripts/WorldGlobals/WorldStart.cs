using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldStart : MonoBehaviour {
    // Hi I am completely useless for the time being...
    private static bool FirstLoad = true;
    public static void WorldSetUnits() {
        // Debug.Log ("WorldSetUnits");
        // WorldUnitsManager = m_GlobalSharedVariables.GetComponent<WorldUnitsManager>();
        // WorldUnitsManager.WorldSetMapInstances();
        // Debug.Log ("Test");
    }

    public static void WorldSetFirstLoad(bool firstLoad) {
        FirstLoad = firstLoad;
    }
    public static bool WorldGetFirstLoad() {
        // Was this scene the first to load since the game was opened ?
        return FirstLoad;
    }
}