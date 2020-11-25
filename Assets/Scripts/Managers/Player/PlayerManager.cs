using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using FreeLookCamera;
using UI;
using System.Collections.Generic;

// This class stores the currently played unit and allows to enable player controls for units or disable them
public class PlayerManager : MonoBehaviour {
    private List <PlayerUnit> PlayerUnitList = new List<PlayerUnit>();
    public class PlayerUnit {
        private GameObject _unitModel; public GameObject GetUnitModel(){ return _unitModel; } public void SetUnitModel(GameObject _g){ _unitModel = _g; }
        private UnitMasterController _unitController; public UnitMasterController GetUnitController(){ return _unitController; } public void SetUnitController(UnitMasterController _s){ _unitController = _s; }
        private bool _unitActive; public bool GetUnitActive(){ return _unitActive; } public void SetUnitActive(bool _b){ _unitActive = _b; }

        private UnitUIManager _unitUI; public UnitUIManager GetUnitUI(){ return _unitUI; } public void SetUnitUI(UnitUIManager _s){ _unitUI = _s; }
        private UnitMapUIManager _unitUIMap; public UnitMapUIManager GetUnitUIMap(){ return _unitUIMap; } public void SetUnitUIMap(UnitMapUIManager _s){ _unitUIMap = _s; }
    }


    private int PlayerUnitCurrentIndex = 0;
    private PlayerUnit CurrentPlayerControlledUnit;

    private bool Pause = false;                                         // Pause should be a global var, not a player-local one !
    private bool MapActive = false;
    private bool DamageControl = false;
    private bool SpawnerMenu = false;
    private bool FreeCamera = false;
    public FreeLookCam m_FreeLookCamera; public FreeLookCam GetFreeLookCam(){ return m_FreeLookCamera; }
    private Camera MapCamera;
    private GameManager GameManager; public void SetGameManager(GameManager gameManager) { GameManager = gameManager; }
    private CompiledTypes.Teams PlayerTeam; public void SetPlayerTeam (CompiledTypes.Teams _t) { PlayerTeam = _t;} public CompiledTypes.Teams GetPlayerTeam() { return PlayerTeam; }
    private UIManager UIManager;
    private MapManager MapManager;
    private UnitsUIManager UnitsUIManager;

    private void InitPlayerFromGameManager() {
        // Debug.Log ("InitPlayerFromGameManager");
        PlayerUnitList.Clear();
        PlayerUnitList = new List<PlayerUnit>();
        CurrentPlayerControlledUnit = null;
        PlayerUnitCurrentIndex = 0;

        MapCamera = Instantiate(WorldGlobals.GetMapCamera(), this.transform).GetComponent<Camera>();
        MapManager = GetComponent<MapManager>();
        MapManager.SetMapCamera(MapCamera);
        MapManager.InitMapFromPlayerManager(GameManager);
        UIManager = GetComponent<UIManager>();
        UIManager.SetPlayerManager(this);
        UIManager.SetFreeLookCamera(m_FreeLookCamera);
        UnitsUIManager = GetComponent<UnitsUIManager>();
        UnitsUIManager.Init(this, MapCamera);
        UnitsUIManager.SetPlayerTag(PlayerTeam);
        // FindAllPossibleTargets();
        // UnitsUIManager.KillAllInstances();
        // UnitsUIManager.Init();
        // Debug.Log ("PlayerUnitList.Count : "+ PlayerUnitList.Count);
        // SwitchSelectedUnitByIndex();
        // UIManager.Reset();                // If the level has restarted, set the unit as not dead (no other check)
    }

    protected void Update() {
        // Debug.Log ("Playable units : "+ PlayerUnitList.Count);
        // if (CurrentPlayerControlledUnit == null) {
        //     SetNextTarget();
        // }
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

        UIManager.SetAimerPosition(m_FreeLookCamera.GetRaycastScreenPosition());
    }

    private void SetHideUI(){
        // This is used to hide all UI elements
        // m_FreeLookCamera.SetHideUI();
        UIManager.SetHideUI();
    }

    private void SetFreeCamera(){
        FreeCamera = !FreeCamera;
        // This is used to allow the player to not play (good riddance)
        m_FreeLookCamera.SetFreeCamera(FreeCamera);
        UIManager.SetFreeCamera(FreeCamera);

        CurrentPlayerControlledUnit.GetUnitController().SetFreeCamera(FreeCamera);
    }

    public void SetPause() {
        Pause = !Pause;
        UIManager.SetPauseUI(Pause);
        // if (Pause) {
        //     Time.timeScale = Mathf.Approximately(Time.timeScale, 0.0f) ? 1.0f : 0.0f;                   
        // } else {
        //     Time.timeScale = Mathf.Approximately(Time.timeScale, 1.0f) ? 0.0f : 1.0f;
        // }
        if (Pause) {
            Time.timeScale = 0f;               
        } else {
            Time.timeScale = 1f;   
        }
        // Debug.Log ("Pause : "+ Pause);
        if (CurrentPlayerControlledUnit != null)
            CurrentPlayerControlledUnit.GetUnitController().SetPause(Pause);
        CheckCameraRotation();
    }

    private void SetNextTarget() {
        //If we overflow, get back to the beginning
        if (PlayerUnitCurrentIndex >= (PlayerUnitList.Count-1)) {
            PlayerUnitCurrentIndex = 0;
        } else {
            PlayerUnitCurrentIndex += 1;
        }

        // Debug.Log ("Playable units - SetNextTarget : "+ PlayerUnitList.Count);
        // Debug.Log ("PlayerUnitCurrentIndex - SetNextTarget : "+ PlayerUnitCurrentIndex);
        
        if (VerifyPlayerUnitsListIntegrity())
            SwitchSelectedUnitByIndex(PlayerUnitCurrentIndex);
    }
    private void SetPreviousTarget() {
        if (PlayerUnitCurrentIndex <= 0 || PlayerUnitCurrentIndex > PlayerUnitList.Count-1) {
            PlayerUnitCurrentIndex = PlayerUnitList.Count-1;
        } else {
            PlayerUnitCurrentIndex -= 1;
        }
        // Debug.Log ("Playable units - SetPreviousTarget : "+ PlayerUnitList.Count);
        // Debug.Log ("PlayerUnitCurrentIndex - SetPreviousTarget : "+ PlayerUnitCurrentIndex);

        if (VerifyPlayerUnitsListIntegrity())
            SwitchSelectedUnitByIndex(PlayerUnitCurrentIndex);
    }

    public void UnitSpawned(UnitMasterController unitController, CompiledTypes.Teams team) {
        // Debug.Log ("UnitSpawned : "+ unitGameObject.name+" - "+team.id);
        // Debug.Log ("PlayerTeam : "+ PlayerTeam.id);
        if (team.id == PlayerTeam.id) {
            PlayerUnit _unit = new PlayerUnit{};
                _unit.SetUnitModel(unitController.GetUnitModel());
                _unit.SetUnitController(unitController);
                _unit.SetUnitActive(false);
            PlayerUnitList.Add(_unit);

            
            _unit.GetUnitController().SetPlayerManager(this);
        }
        UnitsUIManager.SpawnUnit(unitController, team);


        // Late check : if no units were available and a player owned unit spawns, set it as controlled
        if (team.id == PlayerTeam.id) {
            if (CurrentPlayerControlledUnit == null) {
                SwitchSelectedUnitByIndex(0);
            }
        }
        // Debug.Log ("Playable units - UnitSpawned: "+ PlayerUnitList.Count);
        // foreach (var unit in PlayerUnitList) {
        //     Debug.Log ("Playable units : "+ unit);
        // }

    }
    public void UnitDead(UnitMasterController unitController, CompiledTypes.Teams unitTeam, bool unitActive) {
        // Debug.Log ("UnitDead : "+ unitGameObject.name);

        if (unitTeam.id == PlayerTeam.id) {
            UnitsUIManager.RemoveUnit(unitController, unitTeam);
            foreach (PlayerUnit unit in PlayerUnitList) {
                if (unit.GetUnitController() == unitController) {
                    if (unit.GetUnitActive()) {
                        SetCurrentUnitDead(true);
                    }
                    PlayerUnitList.Remove(unit);
                }
            }

        }
        // Debug.Log ("Playable units - UnitDead : "+ PlayerUnitList.Count);
    }

    public void SendEnemiesToPlayerUnits(List <GameObject> enemiesUnitsObjectList) {
        // Debug.Log ("enemiesUnitsObjectList : "+ enemiesUnitsObjectList.Count);
        foreach (PlayerUnit unit in PlayerUnitList) {
            // Debug.Log ("Playable units : "+ unit);
            unit.GetUnitController().SetNewEnemyList(enemiesUnitsObjectList);
        }
        
        // This should be changed for multiplayer
            List <GameObject> _playerUnits = new List<GameObject>();
            foreach (PlayerUnit unit in PlayerUnitList) {
                _playerUnits.Add(unit.GetUnitModel());
            }
            foreach (var unit in enemiesUnitsObjectList) {
                // Debug.Log ("Enemy units : "+ unit);
                unit.GetComponent<UnitMasterController>().SetNewEnemyList(_playerUnits);
            }
        
        // Debug.Log ("Playable units : "+ PlayerUnitList.Count);
    }

    public void SendCurrentEnemyTarget(GameObject targetUnit) {
        UnitsUIManager.SetCurrentEnemyTarget(targetUnit);
    }

    private bool VerifyPlayerUnitsListIntegrity() {
        if (PlayerUnitList.Count == 0){                                                        // If no unit available
            // Debug.Log ("Case 1");
            return false;
        } else if (PlayerUnitList.Count == 1) {                                                // If only one unit available
            // Debug.Log ("Case 2");
            if (PlayerUnitList[0].GetUnitActive()) {
                return true;
            } else {
                SetNextTarget();
                return false;
            }
        } else if (PlayerUnitList.Count < PlayerUnitCurrentIndex) {                                     // If index overflowed
            // Debug.Log ("Case 3");
            SetNextTarget();
            return false;
        } else {                                                                               
            // Debug.Log ("Case 4");
            return true;
        }
    }
    private void SwitchSelectedUnitByIndex(int index) {
        // Debug.Log ("SwitchSelectedUnitByIndex");
        // CHECK IF CURRENT UNIT IS NOT ALREADY SET (in case there is only one playable unit)
        if (PlayerUnitList[index] == CurrentPlayerControlledUnit) {
            return;
        }
        // DEAL WITH PREVIOUS UNIT
        foreach (PlayerUnit unit in PlayerUnitList) {
            if (unit.GetUnitActive()) {
                unit.SetUnitActive(false);
                unit.GetUnitController().SetActive(false);
                UnitsUIManager.SetPlayedUnit(unit.GetUnitController(), false);
            }
        }

        // SET CURRENT UNIT
        if (PlayerUnitList[index] != null) {
            CurrentPlayerControlledUnit = PlayerUnitList[index];
            CurrentPlayerControlledUnit.SetUnitActive(true);
            CurrentPlayerControlledUnit.GetUnitController().SetActive(true);
            CurrentPlayerControlledUnit.GetUnitController().SetMap(MapActive);
            UIManager.SetCurrentPlayedUnitCategory(CurrentPlayerControlledUnit.GetUnitController().GetUnitCategory());
            CurrentPlayerControlledUnit.GetUnitController().SetFreeCamera(FreeCamera);
        } else {
            SetNextTarget();
            return;
        }

        // SEND DATA NEEDED
        m_FreeLookCamera.SetActiveTarget(CurrentPlayerControlledUnit.GetUnitModel(), PlayerUnitList[PlayerUnitCurrentIndex].GetUnitController());
        UIManager.SetActiveTarget(CurrentPlayerControlledUnit.GetUnitModel(), PlayerUnitList[PlayerUnitCurrentIndex].GetUnitController());
        // UnitsUIManager.SetPlayedUnit(CurrentPlayerControlledUnit.GetUnitModel());
        UnitsUIManager.SetPlayedUnit(CurrentPlayerControlledUnit.GetUnitController(), true);
        UIManager.SetCurrentUnitDead(false);
        MapManager.MoveCameraToUnit(CurrentPlayerControlledUnit.GetUnitModel().transform);

    }
    public void SwitchSelectedUnitByController(UnitMasterController unitController) {
        for (int i = 0; i < PlayerUnitList.Count; i++) {
            if (unitController == PlayerUnitList[i].GetUnitController()) {
                PlayerUnitCurrentIndex = i;
                if (VerifyPlayerUnitsListIntegrity())
                    SwitchSelectedUnitByIndex(PlayerUnitCurrentIndex);
                return;
            }
        }
    }

    public void HighlightUnitByMap(UnitMasterController highlightedUnitController, bool isHighlighted) {
        UnitsUIManager.SetHighlightedUnit(highlightedUnitController, isHighlighted);
    }



    public void SetPlayerCanvas(GameObject playerCanvas, GameObject playerMapCanvas){ UIManager.SetPlayerCanvas(playerCanvas); UnitsUIManager.SetPlayerCanvas(playerCanvas, playerMapCanvas); }
    
    public void SetScoreMessage(string message) { UIManager.SetScoreMessage(message); }
    public void SetMap() {
        MapActive = !MapActive;

        UIManager.SetMap(MapActive);
        UnitsUIManager.SetMap(MapActive);
        MapManager.SetMap(MapActive);

        MapCamera.enabled = MapActive;
        // Debug.Log("Player Map open : " + MapActive);
        CurrentPlayerControlledUnit.GetUnitController().SetMap(MapActive);
        SetOverlayUI();
        CheckCameraRotation();
    }
    
    public void SetDamageControl(bool damageControl){
        DamageControl = damageControl;
        SetOverlayUI();
        CheckCameraRotation();
    }
    public void SetSpawnerMenu(bool spawnerMenu){
        SpawnerMenu = spawnerMenu;
        SetOverlayUI();
        CheckCameraRotation();
    }
    private void SetOverlayUI(){
        if (MapActive || DamageControl || SpawnerMenu) {
            UIManager.SetOverlayUI(true);
        } else {
            UIManager.SetOverlayUI(false);
        }
    }
    private void CheckCameraRotation(){
        if (MapActive || DamageControl || Pause || SpawnerMenu) {
            m_FreeLookCamera.SetRotation(false);
            m_FreeLookCamera.SetMouse(true);
        } else {
            m_FreeLookCamera.SetRotation(true);
            m_FreeLookCamera.SetMouse(false);
        }
    }

    public void SetCurrentUnitHealth(float health){ UIManager.SetCurrentUnitHealth(health); }
    public void SetCurrentUnitDead(bool isUnitDead){ UIManager.SetCurrentUnitDead(isUnitDead); }
    public void ChangeSpeedStep(int currentSpeedStep){ UIManager.ChangeSpeedStep(currentSpeedStep); }
    public void SetRotationInput(float rotation){ UIManager.SetRotationInput(rotation); }
    public void SetSingleTurretStatus(TurretManager.TurretStatusType status, int turretNumber){ UIManager.SetSingleTurretStatus(status, turretNumber); }
    public void SendPlayerShellToUI(GameObject shellInstance){ UIManager.SendPlayerShellToUI(shellInstance); }
    public void SetPlayerTurretRole(CompiledTypes.Weapons_roles.RowValues currentControlledTurret) {
        UIManager.SetPlayerUITurretRole(currentControlledTurret);       // Send the current turret control to the AI
        m_FreeLookCamera.SetCurrentTurretRole(currentControlledTurret);
    }
    public void ShellFollowedByCameraDestroyed() { UIManager.ShellFollowedByCameraDestroyed(); }

    public void ResetPlayerFromGameManager(){
        PlayerUnitList.Clear();
        PlayerUnitList = new List<PlayerUnit>();
        CurrentPlayerControlledUnit = null;

        InitPlayerFromGameManager();
        UIManager.SetCurrentUnitDead(true);
        // Debug.Log ("Playable units - Reset : "+ PlayerUnitList.Count);
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