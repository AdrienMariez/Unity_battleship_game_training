using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnerScriptToAttach : MonoBehaviour {
    [Tooltip("Spawnableunits, by category")]
    public SpawnerUnitCategory[] m_SpawnableCategories;
    [Tooltip("Ships units spawnpoint")]
    public Transform m_ShipSpawnPosition;
    public UnitPath[] m_ShipSpawnPath;

    [Tooltip("Submarines units spawnpoint")]
    public Transform m_SubmarineSpawnPosition;
    public UnitPath[] m_SubmarineSpawnPath;

    [Tooltip("Planes units spawnpoint")]
    public Transform m_PlaneSpawnPosition;
    // public Transform[] m_PlaneSpawnPath2;
    public UnitPath[] m_PlaneSpawnPath;
    [Serializable] public class UnitPath {
        public Transform _transform;
        public float _speed;
    }

    [Tooltip("Ground units spawnpoint")]
    public Transform m_GroundSpawnPosition;
    public UnitPath[] m_GroundSpawnPath;


    [HideInInspector] public List<WorldSingleUnit> SpawnableUnitsList;

    [HideInInspector] public List<WorldSingleUnit> TeamedSpawnableUnitsList; public List<WorldSingleUnit> GetTeamedSpawnableUnitsList() { return TeamedSpawnableUnitsList; }

    protected bool StagingListInUse = false;
    [HideInInspector] public List<WorldSingleUnit> StagingUnitList;

    private List<Squad> SquadSpawnedList = new List<Squad>();
    protected bool SpawningListInUse = false;
    public class Squad {
        // Spawning process
        private WorldSingleUnit _unitWorldSingleUnit;  public WorldSingleUnit GetUnitWorldSingleUnit(){ return _unitWorldSingleUnit; } public void SetUnitWorldSingleUnit(WorldSingleUnit _wsu){ _unitWorldSingleUnit = _wsu; }
        private int _leftToSpawn;  public int GetLeftToSpawn(){ return _leftToSpawn; } public void SetLeftToSpawn(int _i){ _leftToSpawn = _i; }
        // Further gameplay
        private List<UnitMasterController> _squadUnitsList = new List<UnitMasterController>(); public List<UnitMasterController> GetSquadUnitsList() { return _squadUnitsList; }
        private UnitMasterController _squadLeader; public UnitMasterController GetSquadLeader(){ return _squadLeader; } 
            public void SetSquadLeader(UnitMasterController _umc){
                _squadLeader = _umc;
                if (_squadLeader != null) { _squadLeader.SetAsSquadLeader(); }
                foreach (UnitMasterController unit in _squadUnitsList) {
                    if (unit != _squadLeader) {
                        unit.UpdateSquadLeader(_squadLeader);
                    }
                }
            }
        private bool _isAlive;  public bool GetIsAlive(){ return _isAlive; } public void SetIsAlive(bool _b){ _isAlive = _b; }
        public void InitSquad(UnitMasterController spawner) { if (_squadLeader != null) { _squadLeader.InitSquad(spawner); } }
    }

    protected bool UnitsInSpawningAnimationListInUse = false;
    private List<UnitInSpawningAnimation> UnitsInSpawningAnimationList = new List<UnitInSpawningAnimation>();
    public class UnitInSpawningAnimation {
        private UnitMasterController _unit;  public UnitMasterController GetUnit(){ return _unit; } public void SetUnit(UnitMasterController _umc){ _unit = _umc; }
        private List<UnitPath> _pathPoints = new List<UnitPath>(); public List<UnitPath> GetPath() { return _pathPoints; } public void SetPath(List<UnitPath> _l){ _pathPoints = _l; }
        private Squad _squad;  public Squad GetSquad(){ return _squad; } public void SetSquad(Squad _c){ _squad = _c; }
    }

    protected UnitMasterController UnitController; public UnitMasterController GetUnitMasterController() {return UnitController; }
    protected UnitAIController AIController;
    private bool Active;
    private bool Dead;
    private bool SpawnMenuOpen = false;

    // Globals
    protected GameManager GameManager; public void SetGameManager(GameManager gameManager) { GameManager = gameManager; }
    protected PlayerManager PlayerManager;
    public void SetPlayerManager(PlayerManager playerManager) {
        PlayerManager = playerManager;

        // In the future, just sending a new PlayerManager could rebuild a new CreateTeamedSpawnList() here
    }
    
    public void BeginOperations(UnitMasterController unitController, UnitAIController aiController) {
        UnitController = unitController;
        AIController = aiController;

        SpawnerSpacing = WorldUIVariables.GetSpawnerSpacing();
        CreateSpawnList();
    }


    // UI
    private GameObject SpawnMenuInstance;
    private GameObject SpawnListContainerInstance;
    private float SpawnerSpacing;

    private void CreateSpawnList() {
        foreach (List<WorldSingleUnit> subCategory in WorldUnitsManager.GetUnitsBySubcategory()) {
            for (int i=0; i < subCategory.Count; i++) {
                // Debug.Log(subCategory[0].GetUnitName());

                if (subCategory[0] != null) {                                                       // Check the first element of each category, if it is good !
                    foreach (SpawnerUnitCategory categorySelected in m_SpawnableCategories) {
                        if (subCategory[i].GetUnitSubCategory() == categorySelected.m_UnitSubCategory) {
                            SpawnableUnitsList.Add(subCategory[i]);
                            // Debug.Log(subCategory[i].GetUnitName());
                        }
                    }
                }
                //else { break; }                                                                         // Don't break, it will prevent the check of any OTHER category
                
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
            // Debug.Log(UnitController.GetTeam().id);
            // Debug.Log(singleUnit.GetUnitName() +" - "+ singleUnit.GetUnitTeam_DB().id);
            if (singleUnit.GetUnitTeam_DB().id == UnitController.GetTeam().id) {
                // Debug.Log (UnitController.GetUnitName()+ " - " +singleUnit.GetUnitName());
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
            // Debug.Log("SpawnMenu pushed, show spawn list !");
        }
    }

    protected void FixedUpdate() {
        // List<UnitInSpawningAnimation> UnitsInSpawningAnimationList
        if (UnitsInSpawningAnimationListInUse) {
            List<UnitInSpawningAnimation> _unitsToRemoveFromList = new List<UnitInSpawningAnimation>();
            foreach (UnitInSpawningAnimation unit in UnitsInSpawningAnimationList) {

                if (unit.GetUnit().GetDead()) {
                    unit.GetUnit().GetUnitModel().transform.parent = null;
                    unit.GetUnit().SetStaging(false, true);
                    _unitsToRemoveFromList.Add(unit);
                    continue;
                }

                Transform targetPath = unit.GetPath()[0]._transform;
                // Debug.Log(GetComponent<Rigidbody>().velocity.magnitude);
                // Debug.Log(unit.GetUnit().GetUnitMaxSpeed());
                float targetPathSpeedPropotion = unit.GetPath()[0]._speed;
                if (targetPathSpeedPropotion < 20) {
                    targetPathSpeedPropotion = 20;
                }

                float _unitSpeed = unit.GetUnit().GetUnitMaxSpeed() * (targetPathSpeedPropotion / 100);
                // Debug.Log(_unitSpeed);
                float _speed =  (_unitSpeed + GetComponent<Rigidbody>().velocity.magnitude) * Time.deltaTime;
                // float _speed =  10 * Time.deltaTime;
                unit.GetUnit().GetUnitModel().transform.position = Vector3.MoveTowards(unit.GetUnit().GetUnitModel().transform.position, targetPath.position, _speed);

                float step = 50.0f * Time.deltaTime;
                unit.GetUnit().GetUnitModel().transform.rotation = Quaternion.RotateTowards(unit.GetUnit().GetUnitModel().transform.rotation, targetPath.rotation, step);

                // Check if the position of the cube and sphere are approximately equal.
                if (Vector3.Distance(unit.GetUnit().GetUnitModel().transform.position, targetPath.position) < 1) {
                    // Waypoint is reached, remove it from list and go to next or stop if no more paths.
                    unit.GetPath().Remove(unit.GetPath()[0]);
                    // Debug.Log("Waypoint reached !");
                    if (unit.GetPath().Count <= 0) {
                        // Debug.Log("Waypoint list ended !");
                        // Path is ended, give full control of the unit to the game process
                        if (unit.GetSquad().GetSquadLeader() == null) {
                            unit.GetSquad().SetSquadLeader(unit.GetUnit());
                            unit.GetSquad().InitSquad(UnitController);
                            // unit.GetUnit().SetSpawnSource(this, true);
                        } else {
                            unit.GetUnit().UpdateSquadLeader(unit.GetSquad().GetSquadLeader());
                        }
                        // else {
                        //     unit.GetUnit().SetSpawnSource(this, false);
                        // }
                        unit.GetUnit().SetSquad(unit.GetSquad());
                        // unit.GetUnit().GetUnitModel().transform.parent = null;
                        unit.GetUnit().SetStaging(false, true);
                        _unitsToRemoveFromList.Add(unit);
                    }
                }
            }
            if (_unitsToRemoveFromList.Count > 0) {
                foreach (UnitInSpawningAnimation unit in _unitsToRemoveFromList) {
                    UnitsInSpawningAnimationList.Remove(unit);
                }
            }
        }
    }

    private bool TryOpenSpawnMenu(){
        // Verify first if a unit complies with what we want to see (a unit in the list for the correct team), otherwise, keep the menu shut !
        foreach (WorldSingleUnit singleUnit in SpawnableUnitsList) {
            if (singleUnit.GetUnitTeam_DB().id == UnitController.GetTeam().id) {
                return true;
            }
        }
        return false;
    }
    private void OpenSpawnMenu(){
        // Debug.Log ("Spawn menu open and ready for "+UnitController.GetUnitName());

        SpawnMenuInstance = Instantiate(WorldUIVariables.GetSpawnerUI());
        SpawnListContainerInstance = SpawnMenuInstance.transform.Find("SpawnListContainer").gameObject;

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
        spawnerButton.onClick.AddListener(() => { TrySpawn(unit, true); });
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

    public void TrySpawn (WorldSingleUnit unit, bool firstPass) {
        // if (TrySpawnUnit(unit, firstPass)) {
        if (TrySpawnSquad(unit)) {
            CreateNewSquad(unit);
            // SpawnUnit(unit, AIController.GetChidrenCanMove(), AIController.GetChidrenCanShoot(), AIController.GetChidrenCanSpawn());
        }
    }
    Vector3 SpawnPosition;
    Quaternion SpawnRotation;
    protected bool TrySpawnSquad (WorldSingleUnit unit) {
        if (GameManager == null) {
            return false;
        }

        // Checks if gameplay allows spawn
        if (GameManager.GetCommandPointSystem()) {
            if (unit.GetUnitTeam() == CompiledTypes.Teams.RowValues.Allies) {
                if ((GameManager.GetAlliesTeamCurrentCommandPoints() - unit.GetUnitCommandPointsCost()) >= 0){
                    return true;
                } else {
                    return false;
                }
            } else if (unit.GetUnitTeam() == CompiledTypes.Teams.RowValues.Axis) {
                if ((GameManager.GetAxisTeamCurrentCommandPoints() - unit.GetUnitCommandPointsCost()) >= 0){
                    return true;
                } else {
                    return false;
                }
            }
        } else {
            // When using slots, this will be changed.
            return true;
        }
        return false;
    }

    protected void CreateNewSquad(WorldSingleUnit unit) {
        Squad _newSquad = new Squad{};
            _newSquad.SetUnitWorldSingleUnit(unit);
            _newSquad.SetLeftToSpawn(unit.GetSquadCount());     // Add each squad member to the count left to spawn
            // _newSquad.SetLeftToSpawn(1);                     // For test purposes, forces one model max for any squad.
        SquadSpawnedList.Add(_newSquad);

        if (SpawningListInUse == false) {
            SpawningListInUse = true;                           // Activate spawn list
            StartCoroutine(SpawnLoop());
        }

    }
    IEnumerator SpawnLoop(){
        while (SpawningListInUse) {
            bool _spawnerHasUnitsToSpawn = false;
            foreach (Squad squad in SquadSpawnedList) {
                if (squad.GetLeftToSpawn() > 0) {
                    if (TrySpawnUnitPosition(squad.GetUnitWorldSingleUnit())) {
                        UnitMasterController _unit = SpawnUnit(squad.GetUnitWorldSingleUnit(), AIController.GetChidrenCanMove(), AIController.GetChidrenCanShoot(), AIController.GetChidrenCanSpawn());
                        squad.GetSquadUnitsList().Add(_unit);

                        // UnitsInSpawningAnimationListInUse
                        bool _animUsed = false;
                        UnitPath[] _spawnPath = m_PlaneSpawnPath;
                        if (squad.GetUnitWorldSingleUnit().GetUnitCategory_DB().id == WorldUnitsManager.GetDB().Units_categories.ship.id) {
                                
                        } else if (squad.GetUnitWorldSingleUnit().GetUnitCategory_DB().id == WorldUnitsManager.GetDB().Units_categories.submarine.id) {

                        } else if (squad.GetUnitWorldSingleUnit().GetUnitCategory_DB().id == WorldUnitsManager.GetDB().Units_categories.aircraft.id) {
                            if (m_PlaneSpawnPath.Length > 0) {
                                _animUsed = true;
                                _spawnPath = m_PlaneSpawnPath;
                            }
                        } else if (squad.GetUnitWorldSingleUnit().GetUnitCategory_DB().id == WorldUnitsManager.GetDB().Units_categories.ground.id) {

                        }

                        if (_animUsed) {
                            _unit.GetUnitModel().transform.parent = this.gameObject.transform;
                            _unit.SetStaging(true, true);

                            UnitInSpawningAnimation _newAnimElement = new UnitInSpawningAnimation{};
                                _newAnimElement.SetUnit(_unit);
                                List<UnitPath> _pathList = new List<UnitPath>();
                                foreach (UnitPath path in _spawnPath) {
                                    _pathList.Add(path);
                                }
                                _newAnimElement.SetPath(_pathList);
                                _newAnimElement.SetSquad(squad);
                            UnitsInSpawningAnimationList.Add(_newAnimElement);

                            UnitsInSpawningAnimationListInUse = true;
                        } else {
                            if (squad.GetSquadLeader() == null) {
                                squad.SetSquadLeader(_unit);
                                squad.InitSquad(UnitController);
                            } else {
                                _unit.UpdateSquadLeader(squad.GetSquadLeader());
                            }
                        }
                        _unit.SetSquad(squad);

                        squad.SetIsAlive(true);
                        squad.SetLeftToSpawn(squad.GetLeftToSpawn()-1);
                    }
                    _spawnerHasUnitsToSpawn = true;
                }
            }
            if (!_spawnerHasUnitsToSpawn) {
                SpawningListInUse = false;
            }
            yield return new WaitForSeconds(3f);
        }
    }
    protected bool TrySpawnUnitPosition (WorldSingleUnit unit) {
        bool trySpawnPosition = false;

        if (unit.GetUnitCategory_DB().id == WorldUnitsManager.GetDB().Units_categories.ship.id) {
            var _spawn = TryPosition(m_ShipSpawnPosition, unit.GetUnitSize());
            if (_spawn.Item2 == true) {
                SpawnPosition = _spawn.Item1.position;
                SpawnRotation = _spawn.Item1.rotation;
                trySpawnPosition = true;
            }
        } else if (unit.GetUnitCategory_DB().id == WorldUnitsManager.GetDB().Units_categories.submarine.id) {
            var _spawn = TryPosition(m_SubmarineSpawnPosition, unit.GetUnitSize());
            if (_spawn.Item2 == true) {
                SpawnPosition = _spawn.Item1.position;
                SpawnRotation = _spawn.Item1.rotation;
                trySpawnPosition = true;
            }
        } else if (unit.GetUnitCategory_DB().id == WorldUnitsManager.GetDB().Units_categories.aircraft.id) {
            var _spawn = TryPositionSingleLocation(m_PlaneSpawnPosition, unit.GetUnitSize(), WorldUnitsManager.GetPlaneSpawnMask());
            if (_spawn.Item2 == true) {
                SpawnPosition = _spawn.Item1.position;
                SpawnRotation = _spawn.Item1.rotation;
                trySpawnPosition = true;
            }
        } else if (unit.GetUnitCategory_DB().id == WorldUnitsManager.GetDB().Units_categories.ground.id) {
            var _spawn = TryPositionSingleLocation(m_GroundSpawnPosition, unit.GetUnitSize(), WorldUnitsManager.GetHitMask());
            if (_spawn.Item2 == true) {
                SpawnPosition = _spawn.Item1.position;
                SpawnRotation = _spawn.Item1.rotation;
                trySpawnPosition = true;
            }
        } else {
            var _spawn = TryPosition(m_GroundSpawnPosition, unit.GetUnitSize());
            if (_spawn.Item2 == true) {
                SpawnPosition = _spawn.Item1.position;
                SpawnRotation = _spawn.Item1.rotation;
                trySpawnPosition = true;
            }
        }


        if (trySpawnPosition) {
            return true;
        } else {
            return false;
        }
    }

    public static (Transform, bool) TryPosition (Transform transform, float unitSize) {
        Transform _transform = transform;
        Collider[] hitColliders = Physics.OverlapSphere(_transform.position, unitSize, WorldUnitsManager.GetHitMask());
        if (hitColliders.Length == 0) {
            return (_transform, true);                        // Try initial position first (Item2 = true means position is clear)
        }

        for (int i = 0; i <= 30; i++) { // Try 30 times to spawn the unit (if it can't with 30 tries, it is deduced there is no place !)
            // Debug.Log("TryPosition loop !");
             Vector3 _p = transform.position;
            _p.x = transform.position.x + UnityEngine.Random.Range(-500, 500);
            _p.z = transform.position.z + UnityEngine.Random.Range(-500, 500);
            transform.position = _p;
            Collider[] _hitColliders = Physics.OverlapSphere (_transform.position, unitSize, WorldUnitsManager.GetHitMask());
            if (_hitColliders.Length == 0) {
                return (_transform, true);
            }
        }
        // Debug.Log("TryPosition found ! Original _x : " + _x +" current x : "+ position.x + "Original _z : " + _z +" current z : "+ position.z);
        return (_transform, false);                    // If no position is found after 30 tries, it is not spawned.
    }
    public static (Transform, bool) TryPositionSingleLocation (Transform transform, float unitSize, LayerMask layerMask) {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, unitSize, layerMask);
        if (hitColliders.Length == 0) {
            return (transform, true);                        // Try initial position first (Item2 = true means position is clear)
        }

        // Debug.Log("TryPosition found ! Original _x : " + _x +" current x : "+ position.x + "Original _z : " + _z +" current z : "+ position.z);
        return (transform, false);                    // If no position is found, it is not spawned.
    }

    public UnitMasterController SpawnUnit (WorldSingleUnit unit, bool aiMove, bool aiShoot, bool aiSpawn) {
        // GameObject spawnedUnitInstance =
        //     Instantiate(unit.GetUnitModel(), SpawnPosition, m_ShipSpawnPosition.rotation);

        UnitMasterController _spawnedUnitController = WorldUnitsManager.BuildUnit(unit, SpawnPosition, SpawnRotation, aiMove, aiShoot, aiSpawn);
        // Debug.Log ("SpawnUnit AI : "+aiMove+" : "+aiShoot+" : "+aiSpawn);
        // _spawnedUnitController.SetSpawnSource(this);
        return _spawnedUnitController;
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


    public void AddSquadMate (UnitMasterController unit) {
        Debug.Log("AddSquadMate");

    }
}