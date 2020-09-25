using System;
using UnityEngine;

[Serializable]
public class WorldSingleUnit {
    // Each parameter is built at the first loading of the game from each prefab added in WorldUnitsManager.
    // Those 
    public GameObject m_UnitPrefab;

    // Same as in UnitMasterController !!
    private string UnitName;
    private WorldUnitsManager.UnitCategories UnitCategory;
    private WorldUnitsManager.UnitSubCategories UnitSubCategory;
    private WorldUnitsManager.SimpleTeams Team;
    private WorldUnitsManager.Nations Nation;
    private int UnitCost;
    private int UnitPointValue;

    public void SetUnit() {
        UnitName = m_UnitPrefab.GetComponent<UnitMasterController>().m_UnitName;
        UnitCategory = m_UnitPrefab.GetComponent<UnitMasterController>().m_UnitCategory;
        UnitSubCategory = m_UnitPrefab.GetComponent<UnitMasterController>().m_UnitSubCategory;
        Team = m_UnitPrefab.GetComponent<UnitMasterController>().m_Team;
        Nation = m_UnitPrefab.GetComponent<UnitMasterController>().m_Nation;
        UnitCost = m_UnitPrefab.GetComponent<UnitMasterController>().m_UnitCost;
        UnitPointValue = m_UnitPrefab.GetComponent<UnitMasterController>().m_UnitPointValue;
    }
    public string GetUnitName(){ return UnitName; }
    public WorldUnitsManager.UnitCategories GetUnitCategory(){ return UnitCategory; }
    public WorldUnitsManager.UnitSubCategories GetUnitSubCategory(){ return UnitSubCategory; }
    public WorldUnitsManager.SimpleTeams GetUnitTeam(){ return Team; }
    public WorldUnitsManager.Nations GetUnitNation(){ return Nation; }
    public int GetUnitCost(){ return UnitCost; }
    public int GetUnitPointValue(){ return UnitPointValue; }
}