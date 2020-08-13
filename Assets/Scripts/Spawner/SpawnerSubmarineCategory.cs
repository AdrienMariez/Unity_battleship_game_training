using System;
using UnityEngine;


// Lack of "Monobehaviour" indicates that we are creating a new object class in this file.
// Serializable : makes new classes appear in the inspector
[Serializable]
public class SpawnerSubmarineCategory {

    public WorldUnitsManager.SubmarineSubCategories m_SubmarineCategory;

    public void Setup () {
    }
}