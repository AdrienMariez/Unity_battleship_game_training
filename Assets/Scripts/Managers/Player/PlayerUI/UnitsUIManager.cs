using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class UnitsUIManager : MonoBehaviour {
    private PlayerManager PlayerManager;
    private Camera Cam;
    private Camera MapCam;
    private GameObject PlayerCanvas; public void SetPlayerCanvas(GameObject playerCanvas, GameObject playerMapCanvas){ PlayerCanvas = playerCanvas; PlayerMapCanvas  = playerMapCanvas;}
    private GameObject PlayerMapCanvas;
    private CompiledTypes.Teams PlayerTeam; public void SetPlayerTag(CompiledTypes.Teams playerTeam) { PlayerTeam = playerTeam; }

    private List <PlayerSideUnitsUI> UnitsUIList = new List<PlayerSideUnitsUI>();
    public class PlayerSideUnitsUI {
        private GameObject _unitModel; public GameObject GetUnitModel(){ return _unitModel; } public void SetUnitModel(GameObject _g){ _unitModel = _g; }
        private UnitMasterController _unitController; public UnitMasterController GetUnitController(){ return _unitController; } public void SetUnitController(UnitMasterController _s){ _unitController = _s; }
        private UnitUIManager _unitUI; public UnitUIManager GetUnitUI(){ return _unitUI; } public void SetUnitUI(UnitUIManager _s){ _unitUI = _s; }
        private UnitMapUIManager _unitUIMap; public UnitMapUIManager GetUnitUIMap(){ return _unitUIMap; } public void SetUnitUIMap(UnitMapUIManager _s){ _unitUIMap = _s; }
        protected UnitSelectionManager _unitSelector; public UnitSelectionManager GetUnitSelector(){ return _unitSelector; } public void SetUnitSelectionManager(UnitSelectionManager _s){ _unitSelector = _s; } 
    }


    private List <GameObject> EnemyUnitList = new List<GameObject>();
    private GameObject TempUI;
    // private GameObject ActiveUnit;

    public void Init(PlayerManager playerManager, Camera camera){
        UnitsUIList = new List<PlayerSideUnitsUI>();
        PlayerManager = playerManager;
        MapCam = camera;
        Cam = GameObject.Find("MainCamera").GetComponentInChildren<Camera>();
    }

    public void SpawnUnit(UnitMasterController unitController, CompiledTypes.Teams team){
        // Build enemy unit list
        // Why is this in UI ?
        if (PlayerTeam.id == WorldUnitsManager.GetDB().Teams.Allies.id) {
            if (team.id == WorldUnitsManager.GetDB().Teams.Axis.id) {
                EnemyUnitList.Add(unitController.GetUnitModel());
                // PlayerManager.SendEnemiesToPlayerUnits(EnemyUnitList);
            }
        } else if (PlayerTeam.id == WorldUnitsManager.GetDB().Teams.Axis.id) {
            if (team.id == WorldUnitsManager.GetDB().Teams.Allies.id) {
                EnemyUnitList.Add(unitController.GetUnitModel());
                // PlayerManager.SendEnemiesToPlayerUnits(EnemyUnitList);
            }
        }
        PlayerManager.SendEnemiesToPlayerUnits(EnemyUnitList);

        // Build new UI set for this unit for a single player manager cameras
        PlayerSideUnitsUI _unit = new PlayerSideUnitsUI{};
            _unit.SetUnitModel(unitController.GetUnitModel());
            _unit.SetUnitController(unitController);
            _unit.SetUnitUI(CreateUnitDisplay(_unit));
            _unit.SetUnitUIMap(CreateUnitMapDisplay(_unit));
            _unit.SetUnitSelectionManager(CreateUnitUnitSelectionManager(_unit));
        UnitsUIList.Add(_unit);

        // Debug.Log ("SpawnUnit : "+ unitController.GetUnitName());
    }
    private UnitUIManager CreateUnitDisplay(PlayerSideUnitsUI unit){
        // Debug.Log ("CreateGameDisplay : "+ UnitList.Count);
        if (unit.GetUnitModel() == null) {return null;}
        // Debug.Log ("name : "+ unitGameObject.name);
        GameObject _tempUIObject = Instantiate(WorldUIVariables.GetUnitUI(), PlayerCanvas.transform);
        unit.GetUnitController().SetUIElement(_tempUIObject);
        UnitUIManager _tempUI = _tempUIObject.GetComponent<UnitUIManager>();
        _tempUI.InitializeUIModule(Cam, unit.GetUnitModel(), unit, this);
        // UnitUIList.Add(_tempUI);
        return _tempUI;
    }
    private UnitMapUIManager CreateUnitMapDisplay(PlayerSideUnitsUI unit){
        // Debug.Log ("CreateGameDisplay : "+ UnitList.Count);
        if (unit.GetUnitModel() == null) {return null;}
        // Debug.Log ("name : "+ unitGameObject.name);
        GameObject _tempMapUIObject = Instantiate(WorldUIVariables.GetUnitMapUI(), PlayerMapCanvas.transform);
        unit.GetUnitController().SetUIMapElement(_tempMapUIObject);
        UnitMapUIManager _tempMapUI = _tempMapUIObject.GetComponent<UnitMapUIManager>();
        _tempMapUI.InitializeUIModule(MapCam, unit.GetUnitModel(), this);
        // UnitUIList.Add(_tempUI);
        return _tempMapUI;
    }
    private UnitSelectionManager CreateUnitUnitSelectionManager(PlayerSideUnitsUI unit){
        // Debug.Log ("CreateGameDisplay : "+ UnitList.Count);
        if (unit.GetUnitModel() == null) {return null;}
        // Debug.Log ("name : "+ unitGameObject.name);
        UnitSelectionManager _selectionManager = unit.GetUnitModel().AddComponent<UnitSelectionManager>();
            _selectionManager.SetUnitController(unit.GetUnitController());
            _selectionManager.SetPlayerManager(PlayerManager);
        // UnitUIList.Add(_tempUI);
        return _selectionManager;
    }

    public void SetPlayedUnit(UnitMasterController activeUnitController, bool isActive){
        foreach (PlayerSideUnitsUI unit in UnitsUIList) {
            if (unit.GetUnitController() == activeUnitController) {
                // Debug.Log ("SetPlayedUnit : "+ activeUnitController.GetUnitName());
                unit.GetUnitUI().SetPlayerUnit(isActive);
                unit.GetUnitUIMap().SetPlayerUnit(isActive);
            }
        }
    }

    public void SetHighlightedUnit(UnitMasterController highlightedUnitController, bool isHighlighted){
        foreach (PlayerSideUnitsUI unit in UnitsUIList) {
            if (unit.GetUnitController() == highlightedUnitController) {
                // Debug.Log ("SetHighlightedUnit : "+ activeUnitController.GetUnitName());
                unit.GetUnitUIMap().SetHighlightedUnit(isHighlighted);
            }
        }
    }

    public void SetMap(bool mapActive) {
        foreach (PlayerSideUnitsUI unit in UnitsUIList) {
            unit.GetUnitSelector().SetMap(mapActive);
        }
    }

    public void SetCurrentEnemyTarget(GameObject targetUnit) {
        foreach (PlayerSideUnitsUI unit in UnitsUIList) {
            unit.GetUnitUI().SetEnemyTargetUnit(targetUnit);
            unit.GetUnitUIMap().SetEnemyTargetUnit(targetUnit);
        }
    }

    public void RemoveUnit(UnitMasterController unitController, CompiledTypes.Teams team){
        if (PlayerTeam.id == WorldUnitsManager.GetDB().Teams.Allies.id) {
            if (team.id == WorldUnitsManager.GetDB().Teams.Axis.id) {
                EnemyUnitList.Remove(unitController.GetUnitModel());
            }
        } else if (PlayerTeam.id == WorldUnitsManager.GetDB().Teams.Axis.id) {
            if (team.id == WorldUnitsManager.GetDB().Teams.Allies.id) {
                EnemyUnitList.Remove(unitController.GetUnitModel());
            }
        }
        PlayerManager.SendEnemiesToPlayerUnits(EnemyUnitList);
    }

    public void RemoveUIForSingleUnit(PlayerSideUnitsUI unitUI){
        UnitsUIList.Remove(unitUI);
    }

    public void KillAllInstances() {
        // Debug.Log ("KillAllInstances");
        foreach (PlayerSideUnitsUI unit in UnitsUIList) {
            unit.GetUnitController().KillAllUIInstances();
            unit.GetUnitUI().Destroy();
            unit.GetUnitUIMap().Destroy();
        }
        UnitsUIList.Clear();
        UnitsUIList = new List<PlayerSideUnitsUI>();
        EnemyUnitList.Clear();
        EnemyUnitList = new List<GameObject>();
        PlayerManager.SendEnemiesToPlayerUnits(EnemyUnitList);
    }
}