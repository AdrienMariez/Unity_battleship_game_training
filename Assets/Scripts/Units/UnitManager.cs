using System;
using UnityEngine;


// Lack of "Monobehaviour" indicates that we are creating a new object class in this file.
// Serializable : makes new classes appear in the inspector
[Serializable]
public class UnitManager {

    public CompiledTypes.Teams.RowValues m_Team;            //  (obsolete)
    public CompiledTypes.Countries.RowValues m_Nation;      //  (obsolete)
    public GameObject m_UnitPrefab;                         // The unit prefab (obsolete)
    public CompiledTypes.Global_Units.RowValues m_Unit;         // The unit itself
    public string m_UnitName;
    // public bool m_UseSpawnpoint;                            // Should the game use a spawnpoint for spawning ?
    public Transform m_SpawnPoint;                          // The position and direction the unit will have when it spawns.
    [Header("AI control :")]
    public bool UnitCanMove = true;
    public bool UnitCanShoot = true;
    public bool UnitCanSpawn = true;
    private GameObject Instance;                          // A reference to the instance of the tank when it is created.
    // [HideInInspector] public int m_Wins;                    // The number of wins this player has so far.

    public CompiledTypes.Global_Units.RowValues GetUnit() { return m_Unit; }
    public void Setup () {
        Instance.transform.position = m_SpawnPoint.position;
        Instance.transform.rotation = m_SpawnPoint.rotation;

        Instance.SetActive(false);
        Instance.SetActive(true);
    }


    // Used during the phases of the game where the player shouldn't be able to control their unit.
    public void DisableControl () {
    }


    // Used during the phases of the game where the player should be able to control their units.
    public void EnableControl () {
    }


    // Used at the start of each round to put the unit into it's default state.
    public void Reset () {
        Instance.transform.position = m_SpawnPoint.position;
        Instance.transform.rotation = m_SpawnPoint.rotation;

        Instance.SetActive(false);
        Instance.SetActive(true);
    }

    public void SetInstance(GameObject gameobj) {
        Instance = gameobj;
        if (Instance.GetComponent<UnitAIController>()) {
            Instance.GetComponent<UnitAIController>().SetAIFromUnitManager(UnitCanMove, UnitCanShoot, UnitCanSpawn);
        }
    }
    public void Destroy() {
        if (Instance) {
            Instance.GetComponent<UnitMasterController>().DestroyUnit();
        }
    }

    public void SetUnactive() {
        Instance.GetComponent<UnitMasterController>().SetActive(false);
    }

    public void SetGameManager(GameManager gameManager) { 
        // Debug.Log (gameManager+"-"+Instance);
        Instance.GetComponent<UnitMasterController>().SetGameManager(gameManager);
    }
    public void SetPlayerManager(PlayerManager playerManager) {
        Instance.GetComponent<UnitMasterController>().SetPlayerManager(playerManager);
    }
}