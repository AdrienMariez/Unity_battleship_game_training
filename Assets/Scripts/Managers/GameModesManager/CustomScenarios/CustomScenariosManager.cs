using System.Collections;
using UnityEngine;

public class CustomScenariosManager : MonoBehaviour {
    public virtual void SetGameManager(GameManager gameManager) {}
    public virtual void Begin() {}
    public virtual void UpdateMessage() {
        // Generic interactive stuff
    }
    public virtual void UpdateGameplay() {
        // Generic interactive stuff
    }
}