using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Lack of "Monobehaviour" indicates that we are creating a new object class in this file.
// Serializable : makes new classes appear in the inspector
[Serializable]
public class UnitManager {

    // public CompiledTypes.Teams.RowValues m_Team;            //  (obsolete)
    // public CompiledTypes.Countries.RowValues m_Nation;      //  (obsolete)
    // public GameObject m_UnitPrefab;                         // The unit prefab (obsolete)

    private WorldSingleUnit _unit;  public WorldSingleUnit GetUnit(){ return _unit; } public void SetUnit(WorldSingleUnit _SWU ){ _unit = _SWU; }
    private string _customName; public string GetCustomName(){ return _customName; } public void SetCustomName(string _s ){ _customName = _s; }

    private bool _unitCanMove; public bool GetUnitCanMove(){ return _unitCanMove; } public void SetUnitCanMove(bool _b ){ _unitCanMove = _b; }
    private bool _unitCanShoot; public bool GetUnitCanShoot(){ return _unitCanShoot; } public void SetUnitCanShoot(bool _b ){ _unitCanShoot = _b; }
    private bool _unitCanSpawn; public bool GetUnitCanSpawn(){ return _unitCanSpawn; } public void SetUnitCanSpawn(bool _b ){ _unitCanSpawn = _b; }

    private Transform _spawnPoint; public Transform GetSpawnPoint(){ return _spawnPoint; } public void SetSpawnPoint(Transform _t ){ _spawnPoint = _t; }

    private UnitMasterController UnitController;
    private bool UnitFromScenario = true; public void SetUnitFromScenario(bool _b ){ UnitFromScenario = _b; }           // Is the unit set from the scenario parameters or not ?
    [Header("Custom Fixed Unit data :")]
    public CompiledTypes.Global_Units.RowValues m_Unit;         // The unit itself
    public string m_UnitName;
    // // public bool m_UseSpawnpoint;                            // Should the game use a spawnpoint for spawning ?
    public Transform m_SpawnPoint;                          // The position and direction the unit will have when it spawns.
    [Header("AI control :")]
    public bool m_UnitCanMove = true;
    public bool m_UnitCanShoot = true;
    public bool m_UnitCanSpawn = true;

    public void VerifyData () {
        if (!UnitFromScenario) {
            return;
        } else {
            _unitCanMove = m_UnitCanMove;
            _unitCanShoot = m_UnitCanShoot;
            _unitCanSpawn = m_UnitCanSpawn;
            _spawnPoint = m_SpawnPoint;
            foreach (List<WorldSingleUnit> subCategory in WorldUnitsManager.GetUnitsBySubcategory()) {
                foreach (WorldSingleUnit worldUnit in subCategory) {
                    if (m_Unit.ToString() == worldUnit.GetUnitReference_DB().id.ToString()) {
                        // Debug.Log (unit.GetUnit().ToString()+" / "+worldUnit.GetUnitReference_DB().id.ToString());
                        _unit = worldUnit;
                    }
                }
            }
            if (!String.IsNullOrEmpty(m_UnitName)) {
                _customName = m_UnitName;
            } else {
                _customName = _unit.GetUnitName();
            }
        }
    }

    public void Setup () {
        // Instance.transform.position = _spawnPoint.position;
        // Instance.transform.rotation = _spawnPoint.rotation;

        // Instance.SetActive(false);
        // Instance.SetActive(true);
    }


    // Used during the phases of the game where the player shouldn't be able to control their unit.
    public void DisableControl () {
    }


    // Used during the phases of the game where the player should be able to control their units.
    public void EnableControl () {
    }


    // Used at the start of each round to put the unit into it's default state.
    public void Reset () {
        // Instance.transform.position = _spawnPoint.position;
        // Instance.transform.rotation = _spawnPoint.rotation;

        // Instance.SetActive(false);
        // Instance.SetActive(true);
    }

    public void SetInstance(UnitMasterController unitController) {
        UnitController = unitController;
        UnitController.SetUnitName(_customName);
        // UnitController.SetSpawnSource(null, true);
        UnitController.SetAsSquadLeader();
        UnitController.InitSquad(null);
        
        // if (Instance.GetComponent<UnitAIController>()) {
        //     Instance.GetComponent<UnitAIController>().SetAIFromUnitManager(_unitCanMove, _unitCanShoot, _unitCanSpawn);
        // }
    }
    public void Destroy() {
        if (UnitController) {
            UnitController.DestroyUnit();
        }
    }

    public void SetUnactive() {
        UnitController.SetActive(false);
    }

    // public void SetGameManager(GameManager gameManager) { 
    //     // Debug.Log (gameManager+"-"+Instance);
    //     // Instance.GetComponent<UnitMasterController>().SetGameManager(gameManager);
    // }
    // public void SetPlayerManager(PlayerManager playerManager) {
    //     // Debug.Log (playerManager+"-"+Instance);
    //     // Instance.GetComponent<UnitMasterController>().SetPlayerManager(playerManager);
    // }
}