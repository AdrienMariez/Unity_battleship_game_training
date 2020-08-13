using System;
using UnityEngine;


// Lack of "Monobehaviour" indicates that we are creating a new object class in this file.
// Serializable : makes new classes appear in the inspector
[Serializable]
public class SpawnerPlaneCategory {

    public WorldUnitsManager.PlaneSubCategories m_PlaneCategory;

    public void Setup () {
    }
}