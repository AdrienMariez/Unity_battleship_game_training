using UnityEngine;
using System.Collections;

public class SpawnerScriptToAttach : MonoBehaviour {

    [Tooltip("Ships categories spawnable")]
    public SpawnerShipCategory[] m_ShipsCategories;
    public Transform m_ShipSpawnPosition;

    [Tooltip("Submarines categories spawnable")]
    public SpawnerSubmarineCategory[] m_SubmarinesCategories;
    public Transform m_SubmarineSpawnPosition;

    [Tooltip("Planes categories spawnable")]
    public SpawnerPlaneCategory[] m_PlanesCategories;
    public Transform m_PlanesSpawnPosition;

    [Tooltip("Ground units categories spawnable")]
    public SpawnerGroundCategory[] m_GroundCategories;
    public Transform m_GroundSpawnPosition;

    private bool Active;
    private bool Dead;

    private WorldUIVariables WorldUIVariables;
    private WorldUnitsManager WorldUnitsManager;
    private GameObject SpawnerUI;

    void Start() {
        WorldUIVariables worldUIVariables = GameObject.Find("GlobalSharedVariables").GetComponent<WorldUIVariables>();
        SpawnerUI = worldUIVariables.m_SpawnerUI;
        WorldUnitsManager = GameObject.Find("GlobalSharedVariables").GetComponent<WorldUnitsManager>();
    }

    protected void Update() {
        if (Input.GetButtonDown ("SpawnMenu") && Active && !Dead) {
            SpawnUnit();
            // Debug.Log("SpawnMenu pushed, show spawn list !");
        }
    }

    protected void CreateSpawnList () {
        // for (int i = 0; i < TurretStatus.Count; i++) {
        //     GameObject turret = Instantiate(TurretStatusSprites, DisplayTurretsStatus.transform);
        // }
    }
    protected void SpawnUnit () {
        GameObject spawnedUnitInstance =
                Instantiate (WorldUnitsManager.m_WorldShipsUnits[0].m_UnitPrefab, m_ShipSpawnPosition.position, m_ShipSpawnPosition.rotation);
        // WorldUnitsManager.m_WorldShipsUnits
        // foreach (var category in m_ShipsCategories) {
            
        // }
    }

    public void SetActive(bool active) {
        Active = active;
    }
    public void SetDeath(bool dead) {
        Dead = dead;
    }
}