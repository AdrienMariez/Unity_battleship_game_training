using UnityEngine;
using System.Collections;

public class WorldUnitsManager : MonoBehaviour {

    public WorldCategoriesUnitsManager[] m_UnitsCategories;

    public enum UnitCategories {
        ship,
        submarine,
        plane,
        ground
    }

    void Start() {
    }

    protected void Update() {
        
    }
}