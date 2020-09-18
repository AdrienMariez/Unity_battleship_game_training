using System.Collections;
using UnityEngine;

public class GameModesManager : MonoBehaviour {
    bool CustomScenario = false;
    CustomScenariosManager CustomScenariosManager;
    public virtual void SetGameManager(GameManager gameManager) {
        if (GetComponent<CustomScenariosManager>()) {
            CustomScenario = true;
            CustomScenariosManager = GetComponent<CustomScenariosManager>();
            CustomScenariosManager.SetGameManager(gameManager);
        } else {
            CustomScenario = false;
            // Debug.Log ("No custom scenario called");
        }
    }
    public virtual void Begin() {
        if (CustomScenario)
            CustomScenariosManager.Begin();
    }
    public virtual void UpdateMessage() {
        if (CustomScenario)
            CustomScenariosManager.UpdateMessage();
        // Generic interactive stuff
    }
    public virtual void UpdateGameplay() {
        if (CustomScenario)
            CustomScenariosManager.UpdateGameplay();
        // Generic interactive stuff
    }
}