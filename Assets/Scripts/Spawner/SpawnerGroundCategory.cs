using System;
using UnityEngine;


// Lack of "Monobehaviour" indicates that we are creating a new object class in this file.
// Serializable : makes new classes appear in the inspector
[Serializable]
public class SpawnerGroundCategory {

    public WorldUnitsManager.GroundSubCategories m_GroundCategory;

    public void Setup () {
    }
}