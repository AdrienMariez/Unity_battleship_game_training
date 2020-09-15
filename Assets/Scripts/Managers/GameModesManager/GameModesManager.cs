using System.Collections;
using UnityEngine;

public class GameModesManager : MonoBehaviour {
    public virtual void Begin() {}
    public virtual void SetGameManager(GameManager gameManager) {}
    public virtual void UpdateMessage() {
        // Generic interactive stuff
    }
    public virtual void UpdateGameplay() {
        // Generic interactive stuff
    }
}