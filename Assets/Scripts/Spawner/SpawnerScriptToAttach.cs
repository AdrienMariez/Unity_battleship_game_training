using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [HideInInspector] public List<WorldSingleUnit> SpawnableUnitsList;

    [HideInInspector] public List<WorldSingleUnit> TeamedSpawnableUnitsList;

    UnitMasterController UnitController;
    private bool Active;
    private bool Dead;
    private bool SpawnMenuOpen = false;

    // Globals
    private WorldUIVariables WorldUIVariables;
    private WorldUnitsManager WorldUnitsManager;
    protected GameManager GameManager;
    protected PlayerManager PlayerManager;


    // UI
    private GameObject SpawnerUI;
    private GameObject SpawnMenuInstance;
    private GameObject SpawnListContainerInstance;
    private float SpawnerSpacing;


    void Start() {
        WorldUIVariables = GameObject.Find("GlobalSharedVariables").GetComponent<WorldUIVariables>();
        WorldUnitsManager = GameObject.Find("GlobalSharedVariables").GetComponent<WorldUnitsManager>();
        SpawnerUI = WorldUIVariables.m_SpawnerUI;
        SpawnerSpacing = WorldUIVariables.SpawnerSpacing;
        // UnitController = GetComponent<UnitMasterController>();
        CreateSpawnList();
    }
    private void CreateSpawnList () {
        foreach (List<WorldSingleUnit> subCategory in WorldUnitsManager.GetUnitsBySubcategory()) {
            for (int i=0; i < subCategory.Count; i++) {
                // Debug.Log(subCategory[0].GetUnitName());

                if (subCategory[0] != null) {
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
        if (Input.GetButtonDown ("SpawnMenu") && SpawnableUnitsList.Count > 0 && Active && !Dead) {
            SwitchSpawnMenu();
            if (SpawnMenuOpen) {
                if(TryOpenSpawnMenu()) { OpenSpawnMenu(); } // Only case that opens the menu
                else { SwitchSpawnMenu(); }
            } else { CloseSpawnMenu(); }
            // SpawnUnit();
            // Debug.Log("SpawnMenu pushed, show spawn list !");
        }
    }

    private bool TryOpenSpawnMenu(){
        // Verify first if a unit complies with what we want to see (a unit in the list for the correct team), otherwise, keep the menu shut !
        // bool success = false;
        foreach (WorldSingleUnit singleUnit in SpawnableUnitsList) {
            if (singleUnit.GetUnitTeam() == UnitController.GetTeam()) {
                // success = true;
                // break;
                return true;
            }
        }
        // if (success) { return true; }
        // else { return false; }
        return false;
    }
    private void OpenSpawnMenu(){
        // Debug.Log ("Spawn menu open and ready !");
        // Debug.Log (UnitController.m_UnitName);

        SpawnMenuInstance = Instantiate(SpawnerUI);
        SpawnListContainerInstance = SpawnMenuInstance.transform.Find("SpawnListContainer").gameObject;
        
        // Clean and populate the list
        TeamedSpawnableUnitsList = new List<WorldSingleUnit>();
        foreach (WorldSingleUnit singleUnit in SpawnableUnitsList) {
            if (singleUnit.GetUnitTeam() == UnitController.GetTeam()) {
                // Debug.Log (singleUnit.GetUnitName());
                TeamedSpawnableUnitsList.Add(singleUnit);
            }
        }
        CreateUnitSpawnerListDisplay();
    }
    private void CreateUnitSpawnerListDisplay() {
        foreach (var singleUnit in TeamedSpawnableUnitsList) {
            GameObject listElement = Instantiate(WorldUIVariables.m_SpawnerUnitSelect, SpawnListContainerInstance.transform);
        }

        float position = 0;
        if (TeamedSpawnableUnitsList.Count % 2 == 0) {
            position = (TeamedSpawnableUnitsList.Count*SpawnerSpacing)/2 - (SpawnerSpacing/2);
        } else {
            position = (TeamedSpawnableUnitsList.Count*SpawnerSpacing)/2;
        }

        for (int i = 0; i < TeamedSpawnableUnitsList.Count; i++) {
            if (!SpawnListContainerInstance) {
                continue;
            }
            if (SpawnListContainerInstance.transform.GetChild(i) == null)
                continue;
            Vector3 positionning = SpawnListContainerInstance.transform.GetChild(i).transform.position;
            positionning.y = position;
            positionning.x = 0;
            SpawnListContainerInstance.transform.GetChild(i).transform.localPosition = positionning;
            position -= SpawnerSpacing;
            CreateSingleUnitSpawnerListDisplay(TeamedSpawnableUnitsList[i], i);
        }
    }
    private void CreateSingleUnitSpawnerListDisplay(WorldSingleUnit unit, int i) {
        //This will come in use when fancy cards will be made, I'm sure...
        if (GameManager.GetCommandPointSystem()) {
            string text;
            text = TeamedSpawnableUnitsList[i].GetUnitName() +" - / - "+ TeamedSpawnableUnitsList[i].GetUnitCommandPointsCost() +"\n";
            SpawnListContainerInstance.transform.GetChild(i).transform.GetChild(0).GetComponentInChildren<Text>().text = text;
        } else {
            SpawnListContainerInstance.transform.GetChild(i).transform.GetChild(0).GetComponentInChildren<Text>().text = TeamedSpawnableUnitsList[i].GetUnitName();
        }

        Button spawnerButton = SpawnListContainerInstance.transform.GetChild(i).transform.GetChild(0).GetComponent<Button>();
		// spawnerButton.onClick.AddListener(SpawnUnit);
        spawnerButton.onClick.AddListener(() => { SpawnUnit(unit); });
    }
    private void CloseSpawnMenu(){
        // Debug.Log ("Spawn menu closed !");
        if (SpawnMenuInstance) {
            foreach (Transform child in SpawnListContainerInstance.transform) {
                GameObject.Destroy(child.gameObject);
            }
            Destroy (SpawnMenuInstance);
        }
    }

    protected void SpawnUnit (WorldSingleUnit unit) {
        Vector3 spawnPosition = m_ShipSpawnPosition.position;

        bool trySpawn1 = false;
        bool trySpawn2 = false;
        for (int i = 0; i <= 30; i++) { // Try 30 times to spawn the unit (if it can't with 30 tries, it is deduced there is no place !)
            spawnPosition = m_ShipSpawnPosition.position;
            spawnPosition.x = m_ShipSpawnPosition.position.x + Random.Range(-500, 500);
            spawnPosition.z = m_ShipSpawnPosition.position.z + Random.Range(-500, 500);
            Collider[] hitColliders = Physics.OverlapSphere(spawnPosition, 100f);
            if (hitColliders.Length == 0) {
                trySpawn1 = true; //Spawn location correct !
                break;
            }
        }

        if (GameManager.GetCommandPointSystem()) {
            if (unit.GetUnitTeam() == WorldUnitsManager.SimpleTeams.Allies) {
                if ((GameManager.GetAlliesTeamCurrentCommandPoints() - unit.GetUnitCommandPointsCost()) >= 0){
                    trySpawn2 = true;
                }
            } else if (unit.GetUnitTeam() == WorldUnitsManager.SimpleTeams.Axis) {
                if ((GameManager.GetAxisTeamCurrentCommandPoints() - unit.GetUnitCommandPointsCost()) >= 0){
                    trySpawn2 = true;
                }
            }
        } else {
            // When using slots, this will be changed.
            trySpawn2 = true;
        }

        if (trySpawn1 && trySpawn2) {
            GameObject spawnedUnitInstance =
                Instantiate (unit.m_UnitPrefab, spawnPosition, m_ShipSpawnPosition.rotation);
        } else {
            if (!trySpawn1) {
                Debug.Log("No spawn location available !");
            }
            if (!trySpawn2) {
                Debug.Log("No points available !");
            }
        }


        // if (hitColliders.Length > 0) {
        //     Debug.Log("Unit in the way !");
        // } else {
        //     Debug.Log("Spawn !");
        //     GameObject spawnedUnitInstance =
        //         Instantiate (WorldUnitsManager.m_WorldSingleUnit[0].m_UnitPrefab, spawnPosition, m_ShipSpawnPosition.rotation);
        // }
    }

    private void SwitchSpawnMenu() {
        SpawnMenuOpen = !SpawnMenuOpen;
        PlayerManager.SetSpawnerMenu(SpawnMenuOpen);
    }
        
    public void SetActive(bool active) {
        Active = active;
        if (SpawnMenuOpen) {
            SwitchSpawnMenu();
            if (!Active) {
                CloseSpawnMenu();
            }
        }
    }
    public void SetDeath(bool dead) {
        Dead = dead;
        if (SpawnMenuOpen) {
            SwitchSpawnMenu();
            if (Dead) {
                CloseSpawnMenu();
            }
        }
    }
    public void SetGameManager(GameManager gameManager) {
        GameManager = gameManager;
    }
    public void SetPlayerManager(PlayerManager playerManager) {
        PlayerManager = playerManager;
    }
    public void SetUnitController(UnitMasterController unitController) {
        UnitController = unitController;
    }
}