using System;
using UnityEngine;


// Lack of "Monobehaviour" indicates that we are creating a new object class in this file.
// Serializable : makes new classes appear in the inspector
[Serializable]
public class WorldCategoriesUnitsManager {
    public string m_UnitCategoryName;
    public WorldUnitsManager.UnitCategories m_UnitCategory;
    public GameObject m_CategoryMapModel;
    public WorldSingleUnitManager[] m_Units;

}