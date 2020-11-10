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
    private List <GameObject> UnitList = new List<GameObject>();
    private List <GameObject> UnitUIList = new List<GameObject>();
    private List <GameObject> UnitUIMapList = new List<GameObject>();
    private List <GameObject> EnemyUnitList = new List<GameObject>();
    private GameObject TempUI;
    private GameObject ActiveUnit;

    public void Init(PlayerManager playerManager, Camera camera){
        PlayerManager = playerManager;
        MapCam = camera;
        Cam = GameObject.Find("MainCamera").GetComponentInChildren<Camera>();
    }

    public void SetPlayedUnit(GameObject activeUnit){
        // Debug.Log ("SetPlayedUnit : "+ activeUnit);
        ActiveUnit = activeUnit;
        foreach (var item in UnitUIList) {
            if (item == null) { continue; }
            else { item.GetComponent<UnitUIManager>().SetPlayerUnit(ActiveUnit); }
        }
        foreach (var item in UnitUIMapList) {
            if (item == null) { continue; }
            else { item.GetComponent<UnitMapUIManager>().SetPlayerUnit(ActiveUnit); }
        }
    }

    public void SpawnUnit(GameObject unitGameObject, CompiledTypes.Teams team){
        if (PlayerTeam.id == WorldUnitsManager.GetDB().Teams.Allies.id) {
            if (team.id == WorldUnitsManager.GetDB().Teams.Axis.id) {
                EnemyUnitList.Add(unitGameObject);
                // PlayerManager.SendEnemiesToPlayerUnits(EnemyUnitList);
            }
        } else if (PlayerTeam.id == WorldUnitsManager.GetDB().Teams.Axis.id) {
            if (team.id == WorldUnitsManager.GetDB().Teams.Allies.id) {
                EnemyUnitList.Add(unitGameObject);
                // PlayerManager.SendEnemiesToPlayerUnits(EnemyUnitList);
            }
        }
        PlayerManager.SendEnemiesToPlayerUnits(EnemyUnitList);
        UnitList.Add(unitGameObject);
        CreateUnitDisplay(unitGameObject);
        CreateUnitMapDisplay(unitGameObject);
    }
    private void CreateUnitDisplay(GameObject unitGameObject){
        // Debug.Log ("CreateGameDisplay : "+ UnitList.Count);
        if (unitGameObject == null) {return;}
        // Debug.Log ("name : "+ unitGameObject.name);
        TempUI = Instantiate(WorldUIVariables.GetUnitUI(), PlayerCanvas.transform);
        unitGameObject.GetComponent<UnitMasterController>().SetUIElement(TempUI);
        TempUI.GetComponent<UnitUIManager>().InitializeUIModule(Cam, unitGameObject, this);
        UnitUIList.Add(TempUI);
    }
    private void CreateUnitMapDisplay(GameObject unitGameObject){
        if (unitGameObject == null) {return;}
        TempUI = Instantiate(WorldUIVariables.GetUnitMapUI(), PlayerMapCanvas.transform);
        unitGameObject.GetComponent<UnitMasterController>().SetUIMapElement(TempUI);
        TempUI.GetComponent<UnitMapUIManager>().InitializeUIModule(MapCam, unitGameObject, this);
        UnitUIMapList.Add(TempUI);
    }
    public void RemoveUnit(GameObject unitGameObject, CompiledTypes.Teams team){
        if (PlayerTeam.id == WorldUnitsManager.GetDB().Teams.Allies.id) {
            if (team.id == WorldUnitsManager.GetDB().Teams.Axis.id) {
                EnemyUnitList.Remove(unitGameObject);
                // PlayerManager.SendEnemiesToPlayerUnits(EnemyUnitList);
            }
        } else if (PlayerTeam.id == WorldUnitsManager.GetDB().Teams.Axis.id) {
            if (team.id == WorldUnitsManager.GetDB().Teams.Allies.id) {
                EnemyUnitList.Remove(unitGameObject);
                // PlayerManager.SendEnemiesToPlayerUnits(EnemyUnitList);
            }
        }
        PlayerManager.SendEnemiesToPlayerUnits(EnemyUnitList);
        UnitList.Remove(unitGameObject);
    }
    public void RemoveUIElement(GameObject unitGameObject){
        UnitUIList.Remove(unitGameObject);
    }
    public void RemoveMapUIElement(GameObject unitGameObject){
        UnitUIMapList.Remove(unitGameObject);
    }

    public void SetCurrentEnemyTarget(GameObject targetUnit) {
        foreach (var item in UnitUIList) {
            if (item == null) { continue; }
            else { item.GetComponent<UnitUIManager>().SetEnemyTargetUnit(targetUnit); }
        }
        foreach (var item in UnitUIMapList) {
            if (item == null) { continue; }
            else { item.GetComponent<UnitMapUIManager>().SetEnemyTargetUnit(targetUnit); }
        }
    }

    public void KillAllInstances() {
        // Debug.Log ("KillAllInstances");
        foreach (var item in UnitList) {
            if (item == null) { continue; }
            item.GetComponent<UnitMasterController>().KillAllUIInstances();
        }
        foreach (var item in UnitUIList) {
            if (item == null) { continue; }
            item.GetComponent<UnitUIManager>().Destroy();
        }
        foreach (var item in UnitUIMapList) {
            if (item == null) { continue; }
            item.GetComponent<UnitMapUIManager>().Destroy();
        }
        UnitList.Clear();
        UnitUIList.Clear();
        UnitUIMapList.Clear();
        EnemyUnitList.Clear();
        UnitList = new List<GameObject>();
        UnitUIList = new List<GameObject>();
        UnitUIMapList = new List<GameObject>();
        EnemyUnitList = new List<GameObject>();
        PlayerManager.SendEnemiesToPlayerUnits(EnemyUnitList);
    }
}