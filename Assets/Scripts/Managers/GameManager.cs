using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Game Manager deals with major game workings, like checking any unit spawn, current game mode...
public class GameManager : MonoBehaviour {

    public enum GameModes {
        Duel,
        Points,
        Custom
    }
    [Header("Select game mode :")]
    public GameModes m_GameMode;
    private GameModesManager CurrentGameMode;
    [Header("Player(s) :")]
    public PlayerManagerList[] m_Players;
    [HideInInspector] public List<PlayerManager> PlayersManager;


    // Command Points : 
    protected bool CommandPointSystem;              // Does the game use point system (for the spawners)
    protected int MaxTeamCommandPoints = 0;
    protected int AlliesTeamCurrentCommandPoints = 0;
    protected int AxisTeamCurrentCommandPoints = 0;
    // Victory Points : 
    protected int RequiredVictoryPointsToWin = 0;
    protected int AlliesTeamCurrentVictoryPoints = 0;
    protected int AxisTeamCurrentVictoryPoints = 0;


    private int TeamAlliesUnits;
    private int TeamOppositionUnits;  // The use of Opposition instead of Axis is to prevent confusion between TeamAlliesUnits and TeamAxisUnits

    private void Start() {
        // Creation of the full unit list, this may move if other files are made for the process !
        // WorldUnitsManager.BuildFirstLoad();
        // Debug.Log (GlobalStart.GlobalUnitList[0]);
        // Set each PlayerManager
        foreach (PlayerManagerList player in m_Players) {
            PlayersManager.Add(player.m_Player.GetComponent<PlayerManager>());
            //Give the correct team to the player
            player.m_Player.GetComponent<PlayerManager>().SetPlayerTeam(player.m_PlayerTeam);
        }
        foreach (PlayerManager playerManager in PlayersManager) {
            playerManager.SetGameManager(this);
            playerManager.Reset();
            playerManager.SetPlayerCanvas(GameObject.Find("UICanvas"), GameObject.Find("UIMapCanvas"));
        }

        // Everything is set, now use only the GameMode selected
        if (m_GameMode == GameModes.Duel) {
            CurrentGameMode = GameObject.Find("GameModes").GetComponent<GameModeDuel>();
        } else if (m_GameMode == GameModes.Points) {
            CurrentGameMode = GameObject.Find("GameModes").GetComponent<GameModePoints>();
        } else if (m_GameMode == GameModes.Custom) {
            CurrentGameMode = GameObject.Find("GameModes").GetComponent<GameModeCustom>();
        } else {
            Debug.Log ("No suitable file found for the selected scenario !");
        }
        CurrentGameMode.SetGameManager(this);
        CurrentGameMode.Begin();
    }

    public void UnitSpawned(GameObject unitGameObject, CompiledTypes.Teams.RowValues unitTeam) {
        // Debug.Log ("UnitSpawned : "+unitTeam);
        WorldUnitsManager.CreateNewUnitMapModel(unitGameObject, unitTeam);  // Ultimately, this should disappear.

        foreach (var playerManager in PlayersManager) {
           playerManager.UnitSpawned(unitGameObject, unitTeam); 
        }

        UpdateScore(unitGameObject, unitTeam, true);
        UpdateGameModeGameplay();
        UpdateGameModeMessage();
    }
    public void UnitDead(GameObject unitGameObject, CompiledTypes.Teams.RowValues unitTeam, bool unitActive) {
        foreach (var playerManager in PlayersManager) {
           playerManager.UnitDead(unitGameObject, unitTeam, unitActive); 
        }

        UpdateScore(unitGameObject, unitTeam, false);
        UpdateGameModeGameplay();
        UpdateGameModeMessage();
    }

    private void UpdateScore(GameObject unitGameObject, CompiledTypes.Teams.RowValues unitTeam, bool positive) {
        // positive stands for " does the score need too be updated in a positive (unit spawned) or a negative way ?"
        if (unitTeam == CompiledTypes.Teams.RowValues.Allies) {
            if (positive) {                                                                                                                 // If unit is spawned... 
                TeamAlliesUnits += 1;                                                                                                           // Unit counter goes up
                AlliesTeamCurrentCommandPoints -= unitGameObject.GetComponent<UnitMasterController>().GetUnitCommandPointsCost();               // Unit price is deduced
            } else {                                                                                                                        // If unit is killed...
                TeamAlliesUnits -= 1;                                                                                                           // Unit counter goes down
                AlliesTeamCurrentCommandPoints += unitGameObject.GetComponent<UnitMasterController>().GetUnitCommandPointsCost();               // Unit price is refund
                AxisTeamCurrentVictoryPoints += unitGameObject.GetComponent<UnitMasterController>().GetUnitVictoryPointsValue();                // Enemy is credited with the points.
            }
        } else if (unitTeam == CompiledTypes.Teams.RowValues.Axis) {
            if (positive) {
                TeamOppositionUnits += 1; 
                AxisTeamCurrentCommandPoints -= unitGameObject.GetComponent<UnitMasterController>().GetUnitCommandPointsCost();
            } else {
                TeamOppositionUnits -= 1; 
                AxisTeamCurrentCommandPoints += unitGameObject.GetComponent<UnitMasterController>().GetUnitCommandPointsCost();
                AlliesTeamCurrentVictoryPoints += unitGameObject.GetComponent<UnitMasterController>().GetUnitVictoryPointsValue(); 
            }
        }
    }

    private void UpdateGameModeGameplay() {
        CurrentGameMode.UpdateGameplay();
    }
    private void UpdateGameModeMessage() {
        CurrentGameMode.UpdateMessage();
    }
    //Message system
    public void SetScoreMessage(string message) {
        foreach (var playerManager in PlayersManager) {
           playerManager.SetScoreMessage(message); 
        }
    }
    public void SetScoreMessageTeam() {
        foreach (var playerManager in PlayersManager) {
            playerManager.SetScoreMessage(CurrentGameMode.GameMessageTeam(playerManager.GetPlayerTeam()));
        }
    }

    public int GetTeamAlliesUnits() { return TeamAlliesUnits; }
    public int GetTeamOppositionUnits() { return TeamOppositionUnits; }

    
    public int GetMaxTeamCommandPoints() { return MaxTeamCommandPoints; }
    public int GetAlliesTeamCurrentCommandPoints() { return AlliesTeamCurrentCommandPoints; }
    public int GetAxisTeamCurrentCommandPoints() { return AxisTeamCurrentCommandPoints; }
    public int GetRequiredVictoryPointsToWin() { return RequiredVictoryPointsToWin; }
    public int GetAlliesTeamCurrentVictoryPoints() { return AlliesTeamCurrentVictoryPoints; }
    public int GetAxisTeamCurrentVictoryPoints() { return AxisTeamCurrentVictoryPoints; }
    
    public void SetMaxTeamCommandPoints(int value) { MaxTeamCommandPoints = value; }

    public void SetRequiredVictoryPointsToWin(int value) { RequiredVictoryPointsToWin = value; }
    // public void SetTeamsCommandPoints(int value) {
    //     AlliesTeamCurrentCommandPoints = value;
    //     AxisTeamCurrentCommandPoints += value;
    // }
    // public void SetAlliesVictoryPoints(int value) { AlliesTeamCurrentVictoryPoints = value; }
    // public void SetAxisVictoryPoints(int value) { AxisTeamCurrentVictoryPoints += value; }

    public void SetCommandPointSystem(bool scenarioUsesCommandPoints) {
        CommandPointSystem = scenarioUsesCommandPoints;
    }
    public bool GetCommandPointSystem() {
        return CommandPointSystem;
    }

    public void ResetCounters() {
        TeamAlliesUnits = 0;
        TeamOppositionUnits = 0;
        AlliesTeamCurrentCommandPoints = MaxTeamCommandPoints;
        AlliesTeamCurrentVictoryPoints = 0;
        AxisTeamCurrentCommandPoints = MaxTeamCommandPoints;
        AxisTeamCurrentVictoryPoints = 0;
    }

    public PlayerManagerList[] GetPlayerManagerList() {
        return m_Players;
    }

    public void ResetPlayerManager() {
        foreach (var playerManager in PlayersManager) {
           playerManager.Reset(); 
        }
    }

    

    public void EndGame() {
        // string sceneName = "MainMenuScene3d";
        // SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        LoadingData.sceneToLoad = "MainMenuScene3d";
        SceneManager.LoadScene("Loading");
    }
}