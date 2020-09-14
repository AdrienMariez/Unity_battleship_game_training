using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Monobehaviour marks that this script extends an existing class
public class GameManager : MonoBehaviour {

    public enum GameModes {
        Duel,
        Points
    }
    [Header("Player UI")]
    private GameModeDuel GameModeDuel;
    private GameModePoints GameModePoints;

    public GameModes m_GameMode;

    public GameObject m_Player;
    [Header("For single Player : ")]
    public WorldUnitsManager.Teams m_PlayerTeam;

    [Header("Gameplay options")]

    private int PlayableUnits;
    private int EnemiesUnits;
    private int WinsAllies;                     // How many Allies round victories this far ?
    private int WinsAxis;                       // How many Axis round victories this far ?
    private WorldUnitsManager.Teams RoundWinner;                  // Who won this particular round ?
    private WorldUnitsManager.Teams GameWinner;                   // Who won the whole game ?

    private WorldUnitsManager WorldUnitsManager;
    private PlayerManager PlayerManager;
    private GameObject PlayerCanvas;
    private GameObject PlayerMapCanvas;

    private void Start() {
        //Find game modes
        GameModeDuel = GameObject.Find("GameModes").GetComponent<GameModeDuel>();
        GameModePoints = GameObject.Find("GameModes").GetComponent<GameModePoints>();

        GameModeDuel.SetGameManager(this);

        WorldUnitsManager = GameObject.Find("GlobalSharedVariables").GetComponent<WorldUnitsManager>();
        PlayerManager = m_Player.GetComponent<PlayerManager>();
        PlayerManager.SetGameManager(this);
        PlayerManager.Reset();
        PlayerCanvas = GameObject.Find("UICanvas");
        PlayerMapCanvas = GameObject.Find("UIMapCanvas");

        PlayerManager.SetPlayerCanvas(PlayerCanvas, PlayerMapCanvas);

        // Everything is set, now use only the GameMode selected
        if (m_GameMode == GameModes.Duel) {
            GameModeDuel.BeginDuel();
        } else if (m_GameMode == GameModes.Points) {

        }
    }


    public WorldUnitsManager.Teams GetPlayer(){
        return m_PlayerTeam;
    }

    public void ShipSpawned(GameObject unitGameObject, WorldUnitsManager.Teams unitTeam, WorldUnitsManager.ShipSubCategories unitType) {
        WorldUnitsManager.CreateShipElement(unitGameObject, unitTeam, unitType);
        UnitSpawned(unitGameObject, unitTeam);
    }
    public void BuildingSpawned(GameObject unitGameObject, WorldUnitsManager.Teams unitTeam, WorldUnitsManager.BuildingSubCategories unitType) {
        WorldUnitsManager.CreateBuildingElement(unitGameObject, unitTeam, unitType);
        UnitSpawned(unitGameObject, unitTeam);
    }
    private void UnitSpawned(GameObject unitGameObject, WorldUnitsManager.Teams unitTeam) {
        if (m_PlayerTeam == WorldUnitsManager.Teams.Allies) {
            if (unitTeam == WorldUnitsManager.Teams.Allies) {
                PlayableUnits += 1;
            } else if (unitTeam == WorldUnitsManager.Teams.Axis || unitTeam == WorldUnitsManager.Teams.AxisAI) {
                EnemiesUnits += 1;
            }
        } else if (m_PlayerTeam == WorldUnitsManager.Teams.Axis) {
            if (unitTeam == WorldUnitsManager.Teams.Axis) {
                PlayableUnits += 1;
            } else if (unitTeam == WorldUnitsManager.Teams.Allies || unitTeam == WorldUnitsManager.Teams.AlliesAI) {
                EnemiesUnits += 1;
            }
        }
        PlayerManager.UnitSpawned(unitGameObject, unitTeam);
        UpdateGameModeMessage();
        UpdateGameModeGameplay();
    }
    public void UnitDead(GameObject unitGameObject, WorldUnitsManager.Teams unitTeam, bool unitActive) {
        if (m_PlayerTeam == WorldUnitsManager.Teams.Allies) {
            if (unitTeam == WorldUnitsManager.Teams.Allies) {
                PlayableUnits -= 1;
            } else if (unitTeam == WorldUnitsManager.Teams.Axis || unitTeam == WorldUnitsManager.Teams.AxisAI) {
                EnemiesUnits -= 1;
            }
        } else if (m_PlayerTeam == WorldUnitsManager.Teams.Axis) {
            if (unitTeam == WorldUnitsManager.Teams.Axis) {
                PlayableUnits -= 1;
            } else if (unitTeam == WorldUnitsManager.Teams.Allies || unitTeam == WorldUnitsManager.Teams.AlliesAI) {
                EnemiesUnits -= 1;
            }
        }
        PlayerManager.UnitDead(unitGameObject, unitTeam, unitActive);
        UpdateGameModeMessage();
        UpdateGameModeGameplay();
    }

    //Message system
    public void SetScoreMessage(string message) {
        PlayerManager.SetScoreMessage(message);
    }
    private void UpdateGameModeMessage() {
        if (m_GameMode == GameModes.Duel) {
            GameModeDuel.UpdateMessage();
        } else if (m_GameMode == GameModes.Points) {

        }
    }

    private void UpdateGameModeGameplay() {
        if (m_GameMode == GameModes.Duel) {
            GameModeDuel.UpdateGameplay();
        } else if (m_GameMode == GameModes.Points) {

        }
    }

    public void SetPlayableUnits(int x) { PlayableUnits = x; }
    public int GetPlayableUnits() { return PlayableUnits; }
    public void SetEnemiesUnits(int x) { EnemiesUnits = x; }
    public int GetEnemiesUnits() { return EnemiesUnits; }
    public WorldUnitsManager.Teams GetSoloPlayerTeam() { return m_PlayerTeam; }
    public PlayerManager GetPlayerManager() { return PlayerManager; }
    public void ResetPlayerManager() { PlayerManager.Reset(); }
    public void EndGame() {
        string sceneName = "MainMenuScene3d";
        // Application.LoadLevel(sceneName);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}