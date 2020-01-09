using System;
using UnityEngine;


// Lack of "Monobehaviour" indicates that we are creating a new object class in this file.
// Serializable : makes new classes appear in the inspector
[Serializable]
public class UnitManager {

    public GameManager.Teams m_Team;
    public GameManager.Nations m_Nation;
    public GameObject m_UnitPrefab;                         // The unit itself
    public bool m_UseSpawnpoint;                            // Should the game use a spawnpoint for spawning ?
    public Transform m_SpawnPoint;                          // The position and direction the unit will have when it spawns.
    private GameObject m_Instance;                          // A reference to the instance of the tank when it is created.
    // [HideInInspector] public int m_Wins;                    // The number of wins this player has so far.


    public void Setup () {
    }


    // Used during the phases of the game where the player shouldn't be able to control their unit.
    public void DisableControl () {
    }


    // Used during the phases of the game where the player should be able to control their units.
    public void EnableControl () {
    }


    // Used at the start of each round to put the unit into it's default state.
    public void Reset () {
        m_Instance.transform.position = m_SpawnPoint.position;
        m_Instance.transform.rotation = m_SpawnPoint.rotation;

        m_Instance.SetActive (false);
        m_Instance.SetActive (true);
    }

    public void SetInstance(GameObject Object){
        m_Instance = Object;
        if (m_Instance.GetComponent<ShipController>()) {
            m_Instance.GetComponent<ShipController>().SetTag(m_Team);
            Debug.Log("m_Instance :"+ m_Instance);
        }
    }
}