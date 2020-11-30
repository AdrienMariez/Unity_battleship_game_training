// using System;
// using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Game Manager deals with major game workings, like checking any unit spawn, current game mode...
public class GameManager : MonoBehaviour {

    public GameObject m_Ocean;

    [Header("Select game mode :")]
        private CompiledTypes.GameModes SelectedGameMode;
        private GameModesManager CurrentGameMode;
    [Header("SpawnPoints :")]
        public GameObject[] m_SpawnPoints;
    [Header("Player(s) :")]
        public GameObject m_PlayerObject;
        public PlayerManagerList[] m_Players; // The process of managing multiple players should be changed. Put in standby while it isn't used.
        [HideInInspector] public List<PlayerManager> PlayersManager;
    [Header("World Global  Variables:")]
        public GameObject m_WorldGlobals;


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

    // Player common parameters :
        // Cameras
            protected float MapCameraMaxSize; public float GetMapCameraMaxSize() { return MapCameraMaxSize; }
            protected float MapCameraPosTop; public float GetMapCameraPosTop() { return MapCameraPosTop; }
            protected float MapCameraPosBottom; public float GetMapCameraPosBottom() { return MapCameraPosBottom; }
            protected float MapCameraPosLeft; public float GetMapCameraPosLeft() { return MapCameraPosLeft; }
            protected float MapCameraPosRight; public float GetCameraPosRight() { return MapCameraPosRight; }


    private void Start() {
        // Include & set world globals
            if (WorldUIVariables.GetFirstLoad() || WorldUnitsManager.GetFirstLoad()){
                // Debug.Log ("m_WorldGlobals");
                Instantiate(m_WorldGlobals);
            }
            WorldUnitsManager.SetGameManager(this);

        // Create map elements
            Transform _gameBoundariesHolder = GameObject.Find("GameBoundaries").transform;
            Transform _gameBoundaries = _gameBoundariesHolder.Find("GameBoundary").transform;
            Transform _gameBoundariesKill = _gameBoundariesHolder.Find("BoundaryKillZone").transform;

            Transform _mapPatternHolder = Instantiate(WorldGlobals.GetMapPattern(), GameObject.Find("GameObjects").transform).transform;
                _mapPatternHolder.position = _gameBoundaries.position;
            GameObject _mapPattern = GameObject.Find("MapSeaGameArea");
                _mapPattern.transform.localScale = new Vector3(_gameBoundaries.localScale.x / 10, 1.0f, _gameBoundaries.localScale.z / 10);
                Material _mapMaterial = _mapPattern.GetComponent<MeshRenderer>().material;
                    _mapMaterial.mainTextureScale = new Vector2(_mapPattern.transform.localScale.x / 100, _mapPattern.transform.localScale.z / 100);
            GameObject _mapPatternForbiddenZone = GameObject.Find("MapSeaOutOfBounds");
                _mapPatternForbiddenZone.transform.localScale = new Vector3(_gameBoundariesKill.localScale.x / 10, 1.0f, _gameBoundariesKill.localScale.z / 10);
                Material _mapForbiddenZoneMaterial = _mapPatternForbiddenZone.GetComponent<MeshRenderer>().material;
                    _mapForbiddenZoneMaterial.mainTextureScale = new Vector2(_mapPatternForbiddenZone.transform.localScale.x / 100, _mapPatternForbiddenZone.transform.localScale.z / 100);
            
            GameObject _mapPatternKillZone = GameObject.Find("MapSeaKillZone");
                _mapPatternKillZone.transform.localScale = new Vector3(_gameBoundariesKill.localScale.x , 1.0f, _gameBoundariesKill.localScale.z);
                Material _mapKillZoneMaterial = _mapPatternKillZone.GetComponent<MeshRenderer>().material;
                    _mapKillZoneMaterial.mainTextureScale = new Vector2(_mapPatternForbiddenZone.transform.localScale.x / 10, _mapPatternForbiddenZone.transform.localScale.z / 10);



        // If scenario is called from main menu, stop all action
            if (LoadingData.InMenu == true) {
                return;
            }

        // Common Map Parameters
            // Map Camera
                float _height = _gameBoundariesKill.localScale.x;
                float _width =_gameBoundariesKill.localScale.z;
                float _mapCameraMaxSize = _height;
                if (_width > _mapCameraMaxSize) {
                    _mapCameraMaxSize = _width;
                }
                MapCameraMaxSize = _mapCameraMaxSize / 2;

                MapCameraPosTop = _gameBoundariesKill.position.z + _gameBoundariesKill.localScale.z / 2;
                MapCameraPosBottom = _gameBoundariesKill.position.z - _gameBoundariesKill.localScale.z / 2;
                MapCameraPosLeft = _gameBoundariesKill.position.x - _gameBoundariesKill.localScale.x / 2;
                MapCameraPosRight = _gameBoundariesKill.position.x + _gameBoundariesKill.localScale.x / 2;
                // Debug.Log ("MapCameraPosTop" + MapCameraPosTop + "MapCameraPosBottom" + MapCameraPosBottom + "MapCameraPosLeft" + MapCameraPosLeft + "MapCameraPosRight" + MapCameraPosRight);

        // Set each PlayerManager
            foreach (PlayerManagerList player in m_Players) {
                player.SetPlayerObject(Instantiate(m_PlayerObject));
                if (LoadingData.PlayerTeam == null) {
                    // Debug.Log ("Basic Allies selected");
                    player.SetPlayerTeam(WorldUnitsManager.GetDB().Teams.Allies);
                } else {
                    // Debug.Log (LoadingData.PlayerTeam.id + "selected");
                    player.SetPlayerTeam(LoadingData.PlayerTeam);
                }
                
                PlayersManager.Add(player.GetPlayerObject().GetComponent<PlayerManager>());
                //Give the correct team to the player
                player.GetPlayerObject().GetComponent<PlayerManager>().SetPlayerTeam(player.GetPlayerTeam());
            }
            foreach (PlayerManager playerManager in PlayersManager) {
                playerManager.SetGameManager(this);
                playerManager.ResetPlayerFromGameManager();
                // TODO Each canvas should be instanciated instead of created...
                playerManager.SetPlayerCanvas(GameObject.Find("UICanvas"), GameObject.Find("UIMapCanvas"));
            }
        // Include ocean
            Instantiate(m_Ocean);

        // GameMode
            if (LoadingData.CurrentGameMode == null) {
                SelectedGameMode = WorldUnitsManager.GetDB().GameModes.duel;
            } else {
                SelectedGameMode = LoadingData.CurrentGameMode;
            }
            // Debug.Log (SelectedGameMode.id);

            if (SelectedGameMode.id == WorldUnitsManager.GetDB().GameModes.duel.id) {
                CurrentGameMode = GameObject.Find("GameModes").GetComponent<GameModeDuel>();
            } else if (SelectedGameMode.id == WorldUnitsManager.GetDB().GameModes.points.id) {
                CurrentGameMode = GameObject.Find("GameModes").GetComponent<GameModePoints>();
            } else if (SelectedGameMode.id == WorldUnitsManager.GetDB().GameModes.custom.id) {
                CurrentGameMode = GameObject.Find("GameModes").GetComponent<GameModeCustom>();
            } else {
                Debug.Log ("No suitable file found for the selected scenario !");
            }
            CurrentGameMode.SetGameManager(this);
            CurrentGameMode.Begin();
    }

    public void UnitSpawned(UnitMasterController unitController, CompiledTypes.Teams unitTeam) {
        // Debug.Log ("UnitSpawned : "+unitTeam.id);
        foreach (var playerManager in PlayersManager) {
           playerManager.UnitSpawned(unitController, unitTeam); 
        }

        UpdateScore(unitController, unitTeam, true);
        UpdateGameModeGameplay();
        UpdateGameModeMessage();
    }
    public void UnitDead(UnitMasterController unitController, CompiledTypes.Teams unitTeam, bool unitActive) {
        foreach (var playerManager in PlayersManager) {
           playerManager.UnitDead(unitController, unitTeam, unitActive); 
        }

        UpdateScore(unitController, unitTeam, false);
        UpdateGameModeGameplay();
        UpdateGameModeMessage();
    }

    private void UpdateScore(UnitMasterController unitController, CompiledTypes.Teams unitTeam, bool positive) {
        // positive stands for " does the score need too be updated in a positive (unit spawned) or a negative way ?"
        if (unitTeam.id == WorldUnitsManager.GetDB().Teams.Allies.id) {
            if (positive) {                                                                                                                 // If unit is spawned... 
                TeamAlliesUnits += 1;                                                                                                           // Unit counter goes up
                AlliesTeamCurrentCommandPoints -= unitController.GetUnitCommandPointsCost();                                                    // Unit price is deduced
            } else {                                                                                                                        // If unit is killed...
                TeamAlliesUnits -= 1;                                                                                                           // Unit counter goes down
                AlliesTeamCurrentCommandPoints += unitController.GetUnitCommandPointsCost();                                                    // Unit price is refund
                AxisTeamCurrentVictoryPoints += unitController.GetUnitVictoryPointsValue();                                                     // Enemy is credited with the points.
            }
        } else if (unitTeam.id == WorldUnitsManager.GetDB().Teams.Axis.id) {
            if (positive) {
                TeamOppositionUnits += 1; 
                AxisTeamCurrentCommandPoints -= unitController.GetUnitCommandPointsCost();
            } else {
                TeamOppositionUnits -= 1; 
                AxisTeamCurrentCommandPoints += unitController.GetUnitCommandPointsCost();
                AlliesTeamCurrentVictoryPoints += unitController.GetUnitVictoryPointsValue(); 
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
           playerManager.ResetPlayerFromGameManager(); 
        }
    }

    

    public void EndGame() {
        WorldUnitsManager.SetGameManager(null);
        LoadingData.CleanData();
        SceneManager.LoadScene("Loading");
    }
}