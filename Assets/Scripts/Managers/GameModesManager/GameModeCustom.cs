using System.Collections;
using UnityEngine;

public class GameModeCustom : GameModesManager {

    public override void SetGameManager(GameManager gameManager) {
        base.SetGameManager(gameManager);
    }
    public override void Begin() {
        base.Begin();
    }


    public override void UpdateMessage() { 
        //Hi I am needed by GameManager, please do not mess with me.
        base.UpdateMessage();
    }

    public override void UpdateGameplay() {
        base.UpdateMessage();
        //Hi I am needed by GameManager, please do not mess with me.
    }
}