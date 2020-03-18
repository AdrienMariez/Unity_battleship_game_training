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
    private GameObject PlayerCanvas;
    private GameObject PlayerMapCanvas;
    private GameManager.Teams PlayerTeam;
    private List <GameObject> UnitList = new List<GameObject>();
    private List <GameObject> UnitUIList = new List<GameObject>();
    private List <GameObject> UnitUIMapList = new List<GameObject>();
    private List <GameObject> EnemyUnitList = new List<GameObject>();
    private GameObject TempUI;
    private GameObject ActiveUnit;

    public GameObject m_UnitUI;
    public GameObject m_UnitMapUI;
    public void Init(PlayerManager playerManager, Camera camera){
        PlayerManager = playerManager;
        MapCam = camera;
        Cam = GameObject.Find("MainCamera").GetComponentInChildren<Camera>();
    }
    public void SetPlayerCanvas(GameObject playerCanvas, GameObject playerMapCanvas){ PlayerCanvas = playerCanvas; PlayerMapCanvas  = playerMapCanvas;}

    public void SetPlayerTag(GameManager.Teams playerTeam) { PlayerTeam = playerTeam; }

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

    public void SpawnUnit(GameObject unitGameObject, GameManager.Teams team){
        if (PlayerTeam == GameManager.Teams.Allies) {
            if (team == GameManager.Teams.Axis || team == GameManager.Teams.AxisAI) {
                EnemyUnitList.Add(unitGameObject);
                PlayerManager.SendEnemiesToPlayerUnits(EnemyUnitList);
            }
        } else if (PlayerTeam == GameManager.Teams.Axis) {
            if (team == GameManager.Teams.Allies || team == GameManager.Teams.AlliesAI) {
                EnemyUnitList.Add(unitGameObject);
                PlayerManager.SendEnemiesToPlayerUnits(EnemyUnitList);
            }
        }
        UnitList.Add(unitGameObject);
        CreateUnitDisplay(unitGameObject);
        CreateUnitMapDisplay(unitGameObject);
    }
    private void CreateUnitDisplay(GameObject unitGameObject){
        // Debug.Log ("CreateGameDisplay : "+ UnitList.Count);
        if (unitGameObject == null) {return;}
        // Debug.Log ("name : "+ item.name);
        TempUI = Instantiate(m_UnitUI, PlayerCanvas.transform);
        if (unitGameObject.GetComponent<ShipController>()){
            unitGameObject.GetComponent<ShipUI>().SetUIElement(TempUI);
        }
        TempUI.GetComponent<UnitUIManager>().InitializeUIModule(Cam, unitGameObject, this);
        UnitUIList.Add(TempUI);
    }
    private void CreateUnitMapDisplay(GameObject unitGameObject){
        if (unitGameObject == null) {return;}
        TempUI = Instantiate(m_UnitMapUI, PlayerMapCanvas.transform);
        if (unitGameObject.GetComponent<ShipController>()){
            unitGameObject.GetComponent<ShipUI>().SetUIMapElement(TempUI);
        }
        TempUI.GetComponent<UnitMapUIManager>().InitializeUIModule(MapCam, unitGameObject, this);
        UnitUIMapList.Add(TempUI);
    }
    public void RemoveUnit(GameObject unitGameObject, GameManager.Teams team){
        if (PlayerTeam == GameManager.Teams.Allies) {
            if (team == GameManager.Teams.Axis || team == GameManager.Teams.AxisAI) {
                EnemyUnitList.Remove(unitGameObject);
                PlayerManager.SendEnemiesToPlayerUnits(EnemyUnitList);
            }
        } else if (PlayerTeam == GameManager.Teams.Axis) {
            if (team == GameManager.Teams.Allies || team == GameManager.Teams.AlliesAI) {
                EnemyUnitList.Remove(unitGameObject);
                PlayerManager.SendEnemiesToPlayerUnits(EnemyUnitList);
            }
        }
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
            if (item.GetComponent<ShipController>()){
                item.GetComponent<ShipUI>().KillAllUIInstances();
            }
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
        UnitList = new List<GameObject>();
        UnitUIList = new List<GameObject>();
        UnitUIMapList = new List<GameObject>();
    }
}