using UnityEngine;
using System.Collections;

public class SpawnerScriptToAttach : MonoBehaviour {

    [Tooltip("Ships categories spawnable")]
    public SpawnerUnitCategory[] m_ShipsCategories;
    public Transform m_ShipSpawnPosition;

    [Tooltip("Submarines categories spawnable")]
    public SpawnerUnitCategory[] m_SubmarinesCategories;
    public Transform m_SubmarineSpawnPosition;

    [Tooltip("Planes categories spawnable")]
    public SpawnerUnitCategory[] m_PlanesCategories;
    public Transform m_PlanesSpawnPosition;

    [Tooltip("Ground units categories spawnable")]
    public SpawnerUnitCategory[] m_GroundCategories;
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
        Vector3 spawnPosition = m_ShipSpawnPosition.position;

        bool trySpawn = false;
        for (int i = 0; i <= 30; i++) { // Try 30 times to spawn the unit (if it can't with 30 tries, it is deduced there is no place !)
            spawnPosition = m_ShipSpawnPosition.position;
            spawnPosition.x = m_ShipSpawnPosition.position.x + Random.Range(-500, 500);
            spawnPosition.z = m_ShipSpawnPosition.position.z + Random.Range(-500, 500);
            Collider[] hitColliders = Physics.OverlapSphere(spawnPosition, 100f);
            if (hitColliders.Length == 0) {
                trySpawn = true; //Spawn location correct !
                break;
            }
        }

        if (trySpawn) {
            GameObject spawnedUnitInstance =
                Instantiate (WorldUnitsManager.m_WorldSingleUnit[1].m_UnitPrefab, spawnPosition, m_ShipSpawnPosition.rotation);
        } else {
            Debug.Log("No spawn location available !");
        }


        // if (hitColliders.Length > 0) {
        //     Debug.Log("Unit in the way !");
        // } else {
        //     Debug.Log("Spawn !");
        //     GameObject spawnedUnitInstance =
        //         Instantiate (WorldUnitsManager.m_WorldSingleUnit[0].m_UnitPrefab, spawnPosition, m_ShipSpawnPosition.rotation);
        // }
    }

    public void SetActive(bool active) {
        Active = active;
    }
    public void SetDeath(bool dead) {
        Dead = dead;
    }
}