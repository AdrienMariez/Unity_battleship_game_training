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

    private int TeamAlliesUnits;
    private int TeamOppositionUnits;  // The use of Opposition instead of Axis is to prevent confusion between TeamAlliesUnits and TeamAxisUnits

    private WorldUnitsManager WorldUnitsManager;

    private void Start() {
        // Debug.Log (GlobalStart.GlobalUnitList[0]);
        WorldUnitsManager = GameObject.Find("GlobalSharedVariables").GetComponent<WorldUnitsManager>();
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

    public void UnitSpawnedConvertFromSimpleTeam(GameObject unitGameObject, WorldUnitsManager.SimpleTeams unitTeam) {
        // In case a unit is spawned from a SimpleTeams line.
        if (unitTeam == WorldUnitsManager.SimpleTeams.Allies) {
            UnitSpawned(unitGameObject, WorldUnitsManager.Teams.Allies);
        } else if(unitTeam == WorldUnitsManager.SimpleTeams.Axis) {
            UnitSpawned(unitGameObject, WorldUnitsManager.Teams.Axis);
        } else {
            UnitSpawned(unitGameObject, WorldUnitsManager.Teams.NeutralAI);
        }
    }
    public void UnitSpawned(GameObject unitGameObject, WorldUnitsManager.Teams unitTeam) {
        WorldUnitsManager.CreateNewUnit(unitGameObject, unitTeam);
        // There is no reason the AI units shouldn't be counted in unit count.
        if (unitTeam == WorldUnitsManager.Teams.Allies || unitTeam == WorldUnitsManager.Teams.AlliesAI) {
            TeamAlliesUnits += 1;
        } else if (unitTeam == WorldUnitsManager.Teams.Axis || unitTeam == WorldUnitsManager.Teams.AxisAI) {
            TeamOppositionUnits += 1;
        }
        // OSP
        // PlayerManager.UnitSpawned(unitGameObject, unitTeam);
        // CFM
        foreach (var playerManager in PlayersManager) {
           playerManager.UnitSpawned(unitGameObject, unitTeam); 
        }

        UpdateGameModeMessage();
        UpdateGameModeGameplay();
    }
    public void UnitDead(GameObject unitGameObject, WorldUnitsManager.Teams unitTeam, bool unitActive) {
        if (unitTeam == WorldUnitsManager.Teams.Allies || unitTeam == WorldUnitsManager.Teams.AlliesAI) {
            TeamAlliesUnits -= 1;
        } else if (unitTeam == WorldUnitsManager.Teams.Axis || unitTeam == WorldUnitsManager.Teams.AxisAI) {
            TeamOppositionUnits -= 1;
        }
        // OSP
        // PlayerManager.UnitDead(unitGameObject, unitTeam, unitActive);
        // CFM
        foreach (var playerManager in PlayersManager) {
           playerManager.UnitDead(unitGameObject, unitTeam, unitActive); 
        }

        UpdateGameModeMessage();
        UpdateGameModeGameplay();
    }

    //Message system
    public void SetScoreMessage(string message) {
        foreach (var playerManager in PlayersManager) {
           playerManager.SetScoreMessage(message); 
        }
    }
    private void UpdateGameModeMessage() {
        CurrentGameMode.UpdateMessage();
    }

    private void UpdateGameModeGameplay() {
        CurrentGameMode.UpdateGameplay();
    }

    public void SetTeamAlliesUnits(int x) { TeamAlliesUnits = x; }
    public int GetTeamAlliesUnits() { return TeamAlliesUnits; }
    public void SetTeamOppositionUnits(int x) { TeamOppositionUnits = x; }
    public int GetTeamOppositionUnits() { return TeamOppositionUnits; }

    public PlayerManagerList[] GetPlayerManagerList() {
        return m_Players;
    }

    public void ResetPlayerManager() {
        foreach (var playerManager in PlayersManager) {
           playerManager.Reset(); 
        }
    }
    public void EndGame() {
        string sceneName = "MainMenuScene3d";
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}