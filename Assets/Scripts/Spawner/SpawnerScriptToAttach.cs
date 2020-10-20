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
    protected GameManager GameManager;
    protected PlayerManager PlayerManager;


    // UI
    private GameObject SpawnerUI;
    private GameObject SpawnMenuInstance;
    private GameObject SpawnListContainerInstance;
    private float SpawnerSpacing;


    void Start() {
        SpawnerUI = WorldUIVariables.GetSpawnerUI();
        SpawnerSpacing = WorldUIVariables.GetSpawnerSpacing();
        UnitController = GetComponent<UnitMasterController>();
        CreateSpawnList();
    }
    private void CreateSpawnList() {
        foreach (List<WorldSingleUnit> subCategory in WorldUnitsManager.GetUnitsBySubcategory()) {
            for (int i=0; i < subCategory.Count; i++) {
                // Debug.Log(subCategory[0].GetUnitName());

                if (subCategory[0] != null) {                                                       // Check the first element of each category, if it is good !
                    foreach (var categorySelected in m_SpawnableCategories) {
                        if (subCategory[i].GetUnitSubCategory() == categorySelected.m_UnitSubCategory.ToString()) {
                            SpawnableUnitsList.Add(subCategory[i]);
                        }
                    }
                } else {
                    break;                                                                          // If nothing is in the list (as intended, stop checking)
                }
            }
        }
        StartCoroutine(SpawnPauseLogic());
    }
    IEnumerator SpawnPauseLogic(){
        yield return new WaitForSeconds(0.3f);
        CreateTeamedSpawnList();
    }
    public void CreateTeamedSpawnList() {
        //This method should be called if a building is captured

        TeamedSpawnableUnitsList = new List<WorldSingleUnit>();             // Clean the list

        if (SpawnableUnitsList.Count == 0) { return; }
        // The enemy AI bugs a bit on this part apparently, need to make appropriate checks.
        // Debug.Log(UnitController.m_UnitName +" - SpawnableUnitsList.Count "+ SpawnableUnitsList.Count);

        foreach (WorldSingleUnit singleUnit in SpawnableUnitsList) {
            // Debug.Log(UnitController.m_UnitName +" - UnitController.GetTeam() "+ UnitController.GetTeam());
            if (singleUnit.GetUnitTeam() == UnitController.GetTeam().ToString()) {
                // Debug.Log (singleUnit.GetUnitName());
                TeamedSpawnableUnitsList.Add(singleUnit);                   // Populate the list
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
            if (singleUnit.GetUnitTeam() == UnitController.GetTeam().ToString()) {
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
        // TeamedSpawnableUnitsList = new List<WorldSingleUnit>();
        // foreach (WorldSingleUnit singleUnit in SpawnableUnitsList) {
        //     if (singleUnit.GetUnitTeam() == UnitController.GetTeam()) {
        //         // Debug.Log (singleUnit.GetUnitName());
        //         TeamedSpawnableUnitsList.Add(singleUnit);
        //     }
        // }
        CreateUnitSpawnerListDisplay();
    }
    private void CreateUnitSpawnerListDisplay() {
        foreach (WorldSingleUnit singleUnit in TeamedSpawnableUnitsList) {
            GameObject listElement = Instantiate(WorldUIVariables.GetSpawnerUnitSelect(), SpawnListContainerInstance.transform);
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
        if (GameManager == null) {
            
        }
        if (GameManager.GetCommandPointSystem()) {
            string text;
            text = TeamedSpawnableUnitsList[i].GetUnitName() +" - / - "+ TeamedSpawnableUnitsList[i].GetUnitCommandPointsCost() +"\n";
            SpawnListContainerInstance.transform.GetChild(i).transform.GetChild(0).GetComponentInChildren<Text>().text = text;
        } else {
            SpawnListContainerInstance.transform.GetChild(i).transform.GetChild(0).GetComponentInChildren<Text>().text = TeamedSpawnableUnitsList[i].GetUnitName();
        }

        Button spawnerButton = SpawnListContainerInstance.transform.GetChild(i).transform.GetChild(0).GetComponent<Button>();
		// spawnerButton.onClick.AddListener(SpawnUnit);
        spawnerButton.onClick.AddListener(() => { TrySpawn(unit); });
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

    protected void TrySpawn (WorldSingleUnit unit) {
        if (TrySpawnUnit(unit)) {
            SpawnUnit(unit);
        }
    }
    Vector3 SpawnPosition;
    public bool TrySpawnUnit (WorldSingleUnit unit) {

        bool trySpawn1 = false;
        bool trySpawn2 = false;
        // Checks if gameplay allows spawn
        if (GameManager.GetCommandPointSystem()) {
            if (unit.GetUnitTeam() == CompiledTypes.Teams.RowValues.Allies.ToString()) {
                if ((GameManager.GetAlliesTeamCurrentCommandPoints() - unit.GetUnitCommandPointsCost()) >= 0){
                    trySpawn2 = true;
                } else {
                    return false;
                }
            } else if (unit.GetUnitTeam() == CompiledTypes.Teams.RowValues.Axis.ToString()) {
                if ((GameManager.GetAxisTeamCurrentCommandPoints() - unit.GetUnitCommandPointsCost()) >= 0){
                    trySpawn2 = true;
                } else {
                    return false;
                }
            }
        } else {
            // When using slots, this will be changed.
            trySpawn2 = true;
        }

        SpawnPosition = m_ShipSpawnPosition.position;

        for (int i = 0; i <= 30; i++) { // Try 30 times to spawn the unit (if it can't with 30 tries, it is deduced there is no place !)
            SpawnPosition = m_ShipSpawnPosition.position;
            SpawnPosition.x = m_ShipSpawnPosition.position.x + Random.Range(-500, 500);
            SpawnPosition.z = m_ShipSpawnPosition.position.z + Random.Range(-500, 500);
            Collider[] hitColliders = Physics.OverlapSphere(SpawnPosition, 100f);
            if (hitColliders.Length == 0) {
                trySpawn1 = true; //Spawn location correct !
                break;
            }
        }


        if (trySpawn1 && trySpawn2) {
            return true;
        } else {
            if (!trySpawn1) {
                Debug.Log("No spawn location available !");
            }
            if (!trySpawn2) {
                Debug.Log("No points available !");
            }
            return false;
        }
    }
    public void SpawnUnit (WorldSingleUnit unit) {
        // GameObject spawnedUnitInstance =
        //     Instantiate(unit.GetUnitModel(), SpawnPosition, m_ShipSpawnPosition.rotation);

        WorldUnitsManager.BuildUnit(unit, SpawnPosition, m_ShipSpawnPosition.rotation);
    }

    private void SwitchSpawnMenu() {
        SpawnMenuOpen = !SpawnMenuOpen;
        if (PlayerManager != null) {   
            PlayerManager.SetSpawnerMenu(SpawnMenuOpen);
        }
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
    // public void SetUnitController(UnitMasterController unitController) {
    //     UnitController = unitController;
    // }
    public List<WorldSingleUnit> GetTeamedSpawnableUnitsList() { return TeamedSpawnableUnitsList; }
}