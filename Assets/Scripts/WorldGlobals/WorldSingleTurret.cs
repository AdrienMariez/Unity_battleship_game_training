using System;
using UnityEngine;

[Serializable]
public class WorldSingleTurret {
    // Each parameter is built at the first loading of the game from each prefab added in WorldUnitsManager.
    // Those 
    public GameObject m_TurretPrefab;

    // Copied from WorldSingleUnit
    /*private string UnitName;
    private WorldUnitsManager.UnitCategories UnitCategory;
    private WorldUnitsManager.UnitSubCategories UnitSubCategory;
    private WorldUnitsManager.SimpleTeams Team;
    private WorldUnitsManager.Nations Nation;
    private int UnitCommandPointsCost;
    private int UnitVictoryPointsValue;

    public void SetUnit() {
        UnitName = m_UnitPrefab.GetComponent<UnitMasterController>().m_UnitName;
        UnitCategory = m_UnitPrefab.GetComponent<UnitMasterController>().m_UnitCategory;
        UnitSubCategory = m_UnitPrefab.GetComponent<UnitMasterController>().m_UnitSubCategory;
        Team = m_UnitPrefab.GetComponent<UnitMasterController>().m_Team;
        Nation = m_UnitPrefab.GetComponent<UnitMasterController>().m_Nation;
        UnitCommandPointsCost = m_UnitPrefab.GetComponent<UnitMasterController>().m_UnitCommandPointsCost;
        UnitVictoryPointsValue = m_UnitPrefab.GetComponent<UnitMasterController>().m_UnitVictoryPointsValue;
    }
    public string GetUnitName(){ return UnitName; }
    public WorldUnitsManager.UnitCategories GetUnitCategory(){ return UnitCategory; }
    public WorldUnitsManager.UnitSubCategories GetUnitSubCategory(){ return UnitSubCategory; }
    public WorldUnitsManager.SimpleTeams GetUnitTeam(){ return Team; }
    public WorldUnitsManager.Nations GetUnitNation(){ return Nation; }
    public int GetUnitCommandPointsCost(){ return UnitCommandPointsCost; }
    public int GetUnitVictoryPointsValue(){ return UnitVictoryPointsValue; }*/
}