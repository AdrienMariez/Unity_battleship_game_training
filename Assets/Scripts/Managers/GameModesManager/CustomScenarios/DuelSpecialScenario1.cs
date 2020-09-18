using System.Collections;
using UnityEngine;

public class DuelSpecialScenario1 : CustomScenariosManager {
    // Put only one of those files in a scene !
    private GameManager GameManager;

    public override void SetGameManager(GameManager gameManager) {}
    public override void Begin() {
        Debug.Log ("DuelSpecialScenario1 called");
    }
    public override void UpdateMessage() {
        // Generic interactive stuff
    }
    public override void UpdateGameplay() {
        // Generic interactive stuff
    }
}