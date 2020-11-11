using System.Collections;
using UnityEngine;

public class GameModeDuel : GameModesManager {
    // public UnitManager[] m_Units;               // Public unit list. There could be multiple of those, in theory

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
        // public static  List<MenuButtonsControl.SpawnPointDuel> _tempSpawnPointsDuel = null;
        if (LoadingData.SpawnPointsDuel != null){
            foreach (MenuButtonsControl.SpawnPointDuel spawnPoint in LoadingData.SpawnPointsDuel) {
                if ( spawnPoint.GetUnit() == null) {
                    continue;
                }
                // Debug.Log (spawnPoint.GetUnit().GetUnitName());
                UnitManager _unitManager = new UnitManager{};
                _unitManager.SetUnit(spawnPoint.GetUnit());
                _unitManager.SetCustomName(spawnPoint.GetUnit().GetUnitName());
                _unitManager.SetUnitCanMove(spawnPoint.GetCanMove());
                _unitManager.SetUnitCanShoot(spawnPoint.GetCanShoot());
                _unitManager.SetUnitCanSpawn(spawnPoint.GetCanSpawn());
                foreach (GameObject sp in GameManager.m_SpawnPoints) {
                    if (spawnPoint.GetSpawnPointDB().DuelSpawnPointName == sp.name) {
                        _unitManager.SetSpawnPoint(sp.transform);
                    }
                }
                UnitList.Add(_unitManager);
                // _unitManager.SetSpawnPoint(spawnPoint.GetSpawnPointDB().DuelSpawnPointName);
            }
        }
        LoadingData.CleanData();

        // Once the units have been created and the camera is using them as targets, start the game.
        StartCoroutine (GameLoop ());
    }
    protected override bool ObectiveAccomplished() {
        // BASIC : If there are still playable units or enemy units...
        bool obectiveAccomplishedForOneSide = false;
        if (GameManager.GetTeamAlliesUnits() == 0 || GameManager.GetTeamOppositionUnits() == 0) {
            obectiveAccomplishedForOneSide = true;
        }
        return obectiveAccomplishedForOneSide;
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
        // The main message could be updated here if needed
        GameManager.SetScoreMessage(GameMessage());
    }

    public override void UpdateGameplay() {
        base.UpdateGameplay();
        //Hi I am needed by GameManager, please do not mess with me.
        //Add specifics for gameplay when a unit dies/spawns here
    }
}