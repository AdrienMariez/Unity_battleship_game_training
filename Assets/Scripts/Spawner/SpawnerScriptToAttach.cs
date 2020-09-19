using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnerScriptToAttach : MonoBehaviour {
    [Tooltip("Spawnableunits, by category")]
    public SpawnerUnitCategory[] m_SpawnableCategories;
    [Tooltip("Ships units spawnpoint")]
    public Transform m_ShipSpawnPosition;

    [Tooltip("Submarines units spawnpoint")]
    public Transform m_SubmarineSpawnPosition;

    [Tooltip("Planes units spawnpoint")]
    public Transform m_PlanesSpawnPosition;

    [Tooltip("Ground units spawnpoint")]
    public Transform m_GroundSpawnPosition;
    public List<WorldSingleUnit> SpawnableUnitsList;

    private bool Active;
    private bool Dead;

    private WorldUIVariables WorldUIVariables;
    private WorldUnitsManager WorldUnitsManager;
    private GameObject SpawnerUI;

    void Start() {
        WorldUIVariables worldUIVariables = GameObject.Find("GlobalSharedVariables").GetComponent<WorldUIVariables>();
        SpawnerUI = worldUIVariables.m_SpawnerUI;
        WorldUnitsManager = GameObject.Find("GlobalSharedVariables").GetComponent<WorldUnitsManager>();
        CreateSpawnList();
    }
    private void CreateSpawnList () {
        foreach (List<WorldSingleUnit> subCategory in WorldUnitsManager.GetUnitsBySubcategory()) {
            for (int i=0; i < subCategory.Count; i++) {
                // Debug.Log(subCategory[0].GetUnitName());

                if (subCategory[0] != null) {
                    List<WorldSingleUnit> fff;
                    foreach (var categorySelected in m_SpawnableCategories) {
                        // Check the first element of each category, if it is good !
                        if (subCategory[i].GetUnitSubCategory() == categorySelected.m_UnitSubCategory) {
                            SpawnableUnitsList.Add(subCategory[i]);
                        }
                    }
                } else {
                    // if nothing is in the list (as intended, stop checking)
                    break;
                }
            }
        }
    }

    protected void Update() {
        if (Input.GetButtonDown ("SpawnMenu") && Active && !Dead) {
            OpenSpawnMenu();
            SpawnUnit();
            // Debug.Log("SpawnMenu pushed, show spawn list !");
        }
    }

    private void OpenSpawnMenu(){
        foreach (WorldSingleUnit unit in SpawnableUnitsList) {
            Debug.Log (unit.GetUnitSubCategory());
            Debug.Log (unit.GetUnitName());
        }
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