using System.Collections;
using UnityEngine;

public class GameModePoints : GameModesManager {
    public UnitManager[] m_Units;               // Public unit list. There could be multiple of those, in theory

    [Header("Gameplay options : ")]
    public int m_NumRoundsToWin = 5;            // The number of rounds a single player has to win to win the game. Overrides NumRoundsToWin in GameModeDuel to allow multiple rounds games.
    public float m_StartDelay = 3f;             // The delay between the start of RoundStarting and RoundPlaying phases.
    public float m_EndDelay = 3f;               // The delay between the end of RoundPlaying and RoundEnding phases.

    public override void Begin() {
        base.Begin();
        
        // Create the delays so they only have to be made once.
        StartWait = new WaitForSeconds (m_StartDelay);
        EndWait = new WaitForSeconds (m_EndDelay);

        // Set the real Round to win in the parent class
        NumRoundsToWin = m_NumRoundsToWin;

        // Send all units to the GameModesManager list.
        Units = m_Units;

        // foreach (var units in Units) {
        //     Debug.Log ("Unit : "+ units.m_UnitPrefab);
        // }

        // Once the units have been created and the camera is using them as targets, start the game.
        StartCoroutine (GameLoop ());
    }

    // Message system
    /*protected override string StartMessage() {
        string message;
        // Override here the message shown on screen at the beginning of a round / of gameplay
        return message;
    }*/
    /*protected override string GameMessage() {
        string message;
        // Override here the message shown on screen during the duration of a round / of gameplay
        return message;
    }*/
    /*protected override string EndMessage() {
        string message;
        // Override here the message shown on screen at the end of a round / of gameplay
        return message;
    }*/

    public override void UpdateMessage() { 
        //Hi I am needed by GameManager, please do not mess with me.
        base.UpdateMessage();
        GameManager.SetScoreMessage(GameMessage());
    }

    public override void UpdateGameplay() {
        base.UpdateMessage();
        //Hi I am needed by GameManager, please do not mess with me.
        //Add specifics for gameplay when a unit dies/spawns here
    }
}