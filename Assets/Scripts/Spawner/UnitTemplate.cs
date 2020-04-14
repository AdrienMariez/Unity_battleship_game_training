using System;
using UnityEngine;


// Lack of "Monobehaviour" indicates that we are creating a new object class in this file.
// Serializable : makes new classes appear in the inspector
[Serializable]
public class UnitTemplate {

    public WorldUnitsManager.Teams m_Team;
    public WorldUnitsManager.Nations m_Nation;
    public GameObject m_UnitPrefab;                         // The unit itself
    public string m_UnitName;
    public bool m_UseSpawnpoint;                            // Should the game use a spawnpoint for spawning ?
    public Transform m_SpawnPoint;                          // The position and direction the unit will have when it spawns.
    private GameObject Instance;                          // A reference to the instance of the tank when it is created.
    public enum UnitType {
        hull,
        engine,
        steering,
        ammo,
        fuel,
        turret,
        underwaterFrontLeft,
        underwaterFrontRight,
        underwaterBackLeft,
        underwaterBackRight,
        armorPlate
    }


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

        //send tag to ships
        if (Instance.GetComponent<ShipController>()) {
            Instance.GetComponent<ShipController>().SetTag(m_Team);
            Instance.GetComponent<ShipController>().SetName(m_UnitName);
            // Debug.Log("Instance :"+ Instance);
        } else {
            // Todo : change this for different unit types
            Instance.gameObject.tag = m_Team.ToString("g");
        }
    }
    public void Destroy() {
        if (Instance) {
            if (Instance.GetComponent<ShipController>()) {
                Instance.GetComponent<ShipController>().DestroyUnit();
            }
        }
    }

    public void SetUnactive() {
        if (Instance.GetComponent<ShipController>()){
            Instance.GetComponent<ShipController>().SetActive(false);
        }
    }

    public void SetGameManager(GameManager gameManager) { 
        if (Instance.GetComponent<ShipController>()){
            Instance.GetComponent<ShipController>().SetGameManager(gameManager);
        }
    }
    public void SetPlayerManager(PlayerManager playerManager) {
        if (Instance.GetComponent<ShipController>()){
            Instance.GetComponent<ShipController>().SetPlayerManager(playerManager);
        }
    }

    public void SetUnitName() { 
        if (Instance.GetComponent<ShipController>()){
            Instance.GetComponent<ShipController>().SetName(m_UnitName);
        }
    }
}