using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using FreeLookCamera;
using UI;
using System.Collections.Generic;

// This class stores the currently played unit and allows to enable player controls for units or disable them
public class PlayerManager : MonoBehaviour
{
    //public FreeLookCam m_FreeLookCam;

    // private GameObject[] PlayerUnits;
    private List <GameObject> PlayerUnits = new List<GameObject>();
    private int CurrentTarget = 0;
    // [HideInInspector] public bool m_Active;
    private GameObject ActiveTarget;
    private bool ActiveTargetSet = false;
    private bool Pause = false;
    private bool MapActive = false;
    private bool DamageControl = false;
    private bool FreeCamera = false;
    public FreeLookCam m_FreeLookCamera;
    public Camera m_MapCamera;
    private GameManager GameManager;
    private WorldUnitsManager.Teams PlayerTeam;
    private UIManager UIManager;
    private MapManager MapManager;
    private UnitsUIManager UnitsUIManager;

    private void Init() {
        // Debug.Log ("INIT");
        PlayerUnits.Clear();
        PlayerUnits = new List<GameObject>();
        ActiveTarget = null;
        ActiveTargetSet = false;
        CurrentTarget = 0;
        MapManager = GetComponent<MapManager>();
        MapManager.SetMapCamera(m_MapCamera);
        UIManager = GetComponent<UIManager>();
        UIManager.SetPlayerManager(this);
        UIManager.SetFreeLookCamera(m_FreeLookCamera);
        UnitsUIManager = GetComponent<UnitsUIManager>();
        UnitsUIManager.Init(this, m_MapCamera);
        UnitsUIManager.SetPlayerTag(PlayerTeam);
        // FindAllPossibleTargets();
        // UnitsUIManager.KillAllInstances();
        // UnitsUIManager.Init();
        // Debug.Log ("PlayerUnits.Count : "+ PlayerUnits.Count);
        SetEnabledUnit();
        // UIManager.Reset();                // If the level has restarted, set the unit as not dead (no other check)
    }

    protected void Update() {
        // Debug.Log ("Playable units : "+ PlayerUnits.Count);
        if (ActiveTarget == null && !ActiveTargetSet) {
            // SetNextTarget();
            SetEnabledUnit();
        }
        if (!Pause) {     
            if (Input.GetButtonDown ("HideUI"))
                SetHideUI();
            if (Input.GetButtonDown ("OpenMap"))
                SetMap();

            if (Input.GetButtonDown ("FreeCamera"))
                SetFreeCamera();

            if (Input.GetButtonDown ("SetNextUnit"))
                SetNextTarget();
            if (Input.GetButtonDown ("SetPreviousUnit"))
                SetPreviousTarget();
            //Debug.Log ("Current target : "+ CurrentTarget);
        }

        if (Input.GetButtonDown ("PauseMenu"))
            SetPause();
    }

    private void SetHideUI(){
        // This is used to hide all UI elements
        m_FreeLookCamera.SetHideUI();
        UIManager.SetHideUI();
    }

    private void SetFreeCamera(){
        FreeCamera = !FreeCamera;
        // This is used to allow the player to not play (good riddance)
        m_FreeLookCamera.SetFreeCamera(FreeCamera);
        UIManager.SetFreeCamera(FreeCamera);

        if (ActiveTarget.GetComponent<TurretManager>()) {
            ActiveTarget.GetComponent<TurretManager>().SetFreeCamera(FreeCamera);
        }
    }

    public void SetPause() {
        Pause = !Pause;
        UIManager.SetPauseUI(Pause);
        if (Pause) {
            Time.timeScale = Mathf.Approximately(Time.timeScale, 0.0f) ? 1.0f : 0.0f;                   
        } else {
            Time.timeScale = Mathf.Approximately(Time.timeScale, 1.0f) ? 0.0f : 1.0f;
        }
        if (ActiveTarget.GetComponent<ShipController>()) {
            ActiveTarget.GetComponent<ShipController>().SetPause(Pause);
        }
        CheckCameraRotation();
    }

    private void SetNextTarget() {
        //If we overflow, get back to the beginning
        if (CurrentTarget >= (PlayerUnits.Count-1)) {
            CurrentTarget = 0;
        } else {
            CurrentTarget += 1;
        }
        // Debug.Log ("Playable units - SetNextTarget : "+ PlayerUnits.Count);
        // Debug.Log ("CurrentTarget - SetNextTarget : "+ CurrentTarget);
        
        //enable or disable user inputs for units disabled.
        if (PlayerUnits.Count > 1 || !ActiveTargetSet)
            SetEnabledUnit();
    }
    private void SetPreviousTarget() {
        if (CurrentTarget <= 0) {
            CurrentTarget = PlayerUnits.Count-1;
        } else {
            CurrentTarget -= 1;
        }
        // Debug.Log ("Playable units - SetPreviousTarget : "+ PlayerUnits.Count);
        // Debug.Log ("CurrentTarget - SetPreviousTarget : "+ CurrentTarget);

        if (PlayerUnits.Count > 1 || !ActiveTargetSet)
            SetEnabledUnit();
    }

    private void FindAllPossibleTargets() {
        // The check to look if any playable is spawned during the game is made only if the player tries to switch unit
        // PlayerUnits = GameObject.FindGameObjectsWithTag(PlayerTeam.ToString("g"));
        Debug.Log ("FindAllPossibleTargets called in PlayerManager - WARNING ! This should be used with precaution !");
        PlayerUnits = new List<GameObject>();
        PlayerUnits.AddRange(GameObject.FindGameObjectsWithTag(PlayerTeam.ToString("g")));
        // Debug.Log ("Playable units - FindAllPossibleTargets : "+ PlayerUnits.Count + " - ActiveTargetSet : "+ ActiveTargetSet);
    }
    public void UnitSpawned(GameObject unitGameObject, WorldUnitsManager.Teams team) {
        // Debug.Log ("UnitSpawned : "+ unitGameObject.name);
        UnitsUIManager.SpawnUnit(unitGameObject, team);
        if (team == PlayerTeam) {
            PlayerUnits.Add(unitGameObject);
        }
        // Debug.Log ("Playable units - UnitSpawned: "+ PlayerUnits.Count);
        // foreach (var unit in PlayerUnits) {
        //     Debug.Log ("Playable units : "+ unit);
        // }
    }
    public void UnitDead(GameObject unitGameObject, WorldUnitsManager.Teams unitTeam, bool unitActive) {
        // Debug.Log ("UnitDead : "+ unitGameObject.name);
        PlayerUnits.Remove(unitGameObject);
        UnitsUIManager.RemoveUnit(unitGameObject, unitTeam);
        if (unitTeam == PlayerTeam && unitActive) {
            SetCurrentUnitDead(true);
        }
        // Debug.Log ("Playable units - UnitDead : "+ PlayerUnits.Count);
    }

    public void SendEnemiesToPlayerUnits(List <GameObject> enemiesUnitsObjectList) {
        // Debug.Log ("enemiesUnitsObjectList : "+ enemiesUnitsObjectList.Count);
        // if (PlayerUnits.Count > 0) {
            foreach (var unit in PlayerUnits) {
                // Debug.Log ("Playable units : "+ unit);
                if (unit.GetComponent<ShipController>()) {
                    unit.GetComponent<ShipAI>().SetNewEnemyList(enemiesUnitsObjectList);
                }
            }
        // }
        
        // This should be changed for multiplayer
        // if (PlayerUnits.Count > 0) {
            foreach (var unit in enemiesUnitsObjectList) {
                // Debug.Log ("Enemy units : "+ unit);
                if (unit.GetComponent<ShipController>()) {
                    unit.GetComponent<ShipAI>().SetNewEnemyList(PlayerUnits);
                }
            }
        // }
        
        // Debug.Log ("Playable units : "+ PlayerUnits.Count);
    }

    public void SendCurrentEnemyTarget(GameObject targetUnit) {
        UnitsUIManager.SetCurrentEnemyTarget(targetUnit);
    }

    private void SetEnabledUnit() {
        if (PlayerUnits.Count == 0){
            // Debug.Log ("Case 1");
            ActiveTargetSet = false;
            return;
        } else if (PlayerUnits.Count == 1) {
            // Debug.Log ("Case 2");
            ActiveTargetSet = true;
        } else if (PlayerUnits[CurrentTarget] == null) {
            // Debug.Log ("Case 3");
            ActiveTargetSet = false;
            SetNextTarget();
            return;
        } else {
            // Debug.Log ("Case 4");
            ActiveTargetSet = true;
        }
        ActiveTarget = PlayerUnits[CurrentTarget];
        // Debug.Log ("ActiveTarget : "+ ActiveTarget);
        // Debug.Log ("CurrentTarget : "+ CurrentTarget);
        // Debug.Log ("PlayerUnits.Length : "+ PlayerUnits.Length);
        UIManager.SetTargetType("Unknown");

        for (int i = 0; i < PlayerUnits.Count; i++){
            if (PlayerUnits[i] == null){ continue; }
            // If it's a tank :
            if (PlayerUnits[i].GetComponent<TankMovement>()) {
                if (i == CurrentTarget) {
                    PlayerUnits[i].GetComponent<TankMovement>().SetActive(true);
                    PlayerUnits[i].GetComponent<TurretManager>().SetActive(true);
                    UIManager.SetTargetType("Tank");
                    //Debug.Log ("Current target is a tank : "+ PlayerUnits[CurrentTarget].GetComponent<TankMovement>());
                } else {
                    PlayerUnits[i].GetComponent<TankMovement>().SetActive(false);
                    PlayerUnits[i].GetComponent<TurretManager>().SetActive(false);
                }
            }
            else if (PlayerUnits[i].GetComponent<AircraftController>()) {
                if (i == CurrentTarget) {
                    PlayerUnits[i].GetComponent<AircraftUserControl4Axis>().m_Active = true;
                    UIManager.SetTargetType("Aircraft");
                    // Debug.Log ("Current target is a plane");
                } else {
                    PlayerUnits[i].GetComponent<AircraftUserControl4Axis>().m_Active = false;
                }
            }
            else if (PlayerUnits[i].GetComponent<ShipController>()) {
                if (i == CurrentTarget) {
                    PlayerUnits[i].GetComponent<ShipController>().SetActive(true);
                    PlayerUnits[i].GetComponent<ShipController>().SetMap(MapActive);
                    UIManager.SetTargetType("Ship");
                } else {
                    PlayerUnits[i].GetComponent<ShipController>().SetActive(false);
                } 
            }
            else if (PlayerUnits[i].GetComponent<BuildingController>()) {
                if (i == CurrentTarget) {
                    PlayerUnits[i].GetComponent<BuildingController>().SetActive(true);
                    PlayerUnits[i].GetComponent<BuildingController>().SetMap(MapActive);
                    UIManager.SetTargetType("Building");
                } else {
                    PlayerUnits[i].GetComponent<BuildingController>().SetActive(false);
                } 
            }
        }
        m_FreeLookCamera.SetActiveTarget(ActiveTarget);
        UIManager.SetActiveTarget(ActiveTarget);
        UnitsUIManager.SetPlayedUnit(ActiveTarget);
        UIManager.SetCurrentUnitDead(false);
        MapManager.SetPlayedUnit(ActiveTarget);

        if (ActiveTarget.GetComponent<TurretManager>()) {
            ActiveTarget.GetComponent<TurretManager>().SetFreeCamera(FreeCamera);
        }
        // Debug.Log ("Current target for player manager : "+ ActiveTarget);
    }

    private void CheckCameraRotation(){
        if (MapActive || DamageControl || Pause) {
            m_FreeLookCamera.SetRotation(false);
            m_FreeLookCamera.SetMouse(true);
        } else {
            m_FreeLookCamera.SetRotation(true);
            m_FreeLookCamera.SetMouse(false);
        }
    }

    // public void SetPlayer(WorldUnitsManager.Teams PlayerTeam){}
    public void SetPlayerCanvas(GameObject playerCanvas, GameObject playerMapCanvas){ UIManager.SetPlayerCanvas(playerCanvas); UnitsUIManager.SetPlayerCanvas(playerCanvas, playerMapCanvas); }

    // public void InitUnitsUI() { UnitsUIManager.Init(); }
    
    public void SetScoreMessage(string message) { UIManager.SetScoreMessage(message); }
    public void SetMap() {
        MapActive = !MapActive;
        if (MapActive)
            SetEnabledUnit();

        UIManager.SetMap(MapActive);

        MapManager.SetInitialPosition(ActiveTarget);
        MapManager.SetMap(MapActive);

        m_MapCamera.enabled = MapActive;

        if (ActiveTarget.GetComponent<ShipController>()) {
            ActiveTarget.GetComponent<ShipController>().SetMap(MapActive);
        }

        CheckCameraRotation();
    }
    public void SetGameManager(GameManager gameManager) {
        GameManager = gameManager;
        PlayerTeam = GameManager.GetPlayer();
    }
    public void SetDamageControl(bool damageControl){
        DamageControl = damageControl;
        UIManager.SetOverlayUI(DamageControl);
        CheckCameraRotation();
    }
    public void SetCurrentUnitHealth(float health){ UIManager.SetCurrentUnitHealth(health); }
    public void SetCurrentUnitDead(bool isUnitDead){ UIManager.SetCurrentUnitDead(isUnitDead); }
    public void ChangeSpeedStep(int currentSpeedStep){ UIManager.ChangeSpeedStep(currentSpeedStep); }
    public void SetRotationInput(float rotation){ UIManager.SetRotationInput(rotation); }
    public void SetSingleTurretStatus(TurretManager.TurretStatusType status, int turretNumber){ UIManager.SetSingleTurretStatus(status, turretNumber); }
    public void SendPlayerShellToUI(GameObject shellInstance){ UIManager.SendPlayerShellToUI(shellInstance); }
    public void SetPlayerUITurretType(TurretFireManager.TurretType currentControlledTurret) { UIManager.SetPlayerUITurretType(currentControlledTurret); }
    public void ShellFollowedByCameraDestroyed() { UIManager.ShellFollowedByCameraDestroyed(); }
    public FreeLookCam GetFreeLookCam(){ return m_FreeLookCamera; }

    public void Reset(){
        PlayerUnits.Clear();
        PlayerUnits = new List<GameObject>();
        ActiveTarget = null;
        Init();
        UIManager.SetCurrentUnitDead(true);
        // Debug.Log ("Playable units - Reset : "+ PlayerUnits.Count);
    }
    public void UnitsUIManagerKillAllInstances(){
        UnitsUIManager.KillAllInstances();
    }
    public void EndGame(){
        if (Pause){
            SetPause();
        }
        GameManager.EndGame();
    }
}