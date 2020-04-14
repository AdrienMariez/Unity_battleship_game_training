using System;
using UnityEngine;


// Lack of "Monobehaviour" indicates that we are creating a new object class in this file.
// Serializable : makes new classes appear in the inspector
[Serializable]
public class WorldSingleShipUnit {
    public GameObject m_UnitPrefab;
    public WorldUnitsManager.ShipSubCategories m_UnitCategory;
    public WorldUnitsManager.SimpleTeams m_Team;
    public GameManager.Nations m_Nation;
    public int m_UnitCost;
}