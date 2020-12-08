using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMasterController : MonoBehaviour {
    [Header("Global units elements : ")]
    // Same as WorldSingleUnit !

    protected WorldSingleUnit UnitWorldSingleUnit; public WorldSingleUnit GetUnitWorldSingleUnit() { return UnitWorldSingleUnit; }
    protected CompiledTypes.Global_Units UnitReference_DB;

    protected GameObject UnitModel; public void SetUnitModel(GameObject _g){ UnitModel = _g; } public GameObject GetUnitModel() { return UnitModel; }

    protected string UnitName;
        public void SetUnitName(string _s) { UnitName = _s; gameObject.name = _s; UnitUI.ChangeName(UnitName); } public string GetUnitName() { return UnitName; }
    protected CompiledTypes.Units_categories.RowValues UnitCategory; public CompiledTypes.Units_categories.RowValues GetUnitCategory() { return UnitCategory; }
    protected CompiledTypes.Units_sub_categories.RowValues UnitSubCategory; public CompiledTypes.Units_sub_categories.RowValues GetUnitSubCategory() { return UnitSubCategory; }
    protected CompiledTypes.Units_sub_categories UnitSubCategory_DB; public CompiledTypes.Units_sub_categories GetUnitSubCategory_DB() { return UnitSubCategory_DB; }
    protected CompiledTypes.Countries Nation;
    protected CompiledTypes.Teams.RowValues Team; 
    protected CompiledTypes.Teams UnitTeam; public CompiledTypes.Teams GetTeam() { return UnitTeam; }
    protected int UnitCommandPointsCost; public int GetUnitCommandPointsCost() { return UnitCommandPointsCost; }
    protected int UnitVictoryPointsValue; public int GetUnitVictoryPointsValue() { return UnitVictoryPointsValue; }

    private GameObject MapModel; public void SetMapModel(GameObject _g){ MapModel = _g; } public GameObject GetMapModel(){ return MapModel; }
    
    protected List<HitboxComponent> UnitComponents = new List<HitboxComponent>();
    protected float RepairRate = 0;  public float GetRepairRate(){ return RepairRate; }

    protected bool Active = false;
    protected bool Dead = false;
    protected bool InGameBoundaries = false;                // True : unit is within game boundaries / False : unit is out of game
    private SpawnerScriptToAttach Spawner;
    protected GameManager GameManager;
    protected PlayerManager PlayerManager;

    protected UnitAIController UnitAI; public UnitAIController GetUnitAI() { return UnitAI; }
    protected UnitUI UnitUI;
    protected UnitHealth Health; public UnitHealth GetUnitHealth() { return Health; }
    protected TurretManager Turrets;
    protected PlaneWeaponsManager PlaneWeapons;
    private GameObject PlayerSetEnemyTargetUnit;

    public enum ElementType {
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
    
    public virtual void FixedUpdate() {
        // MapModel.transform.position.y = 300;
        if ( !Dead && transform.position.y < 0 && MapModel != null) {
            MapModel.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }
    }
    // Turrets
    public virtual void SetTotalTurrets(int turrets) { }
    public virtual void SetMaxTurretRange(float maxRange) { UnitAI.SetMaxTurretRange(maxRange); }
    public virtual void SetDamagedTurrets(int turrets) { }
    public virtual void SetSingleTurretStatus(TurretManager.TurretStatusType status, int turretNumber){ }
    public virtual void SendPlayerShellToUI(GameObject shellInstance){ }
    public virtual void FeedbackShellHit (bool armorPenetrated) { }

    // UI
    public virtual void SetUIElement(GameObject uiElement) { UnitUI.SetUIElement(uiElement); }
    public virtual void SetUIMapElement(GameObject uiElement) { UnitUI.SetUIMapElement(uiElement); }
    public virtual void KillAllUIInstances() {  UnitUI.KillAllUIInstances();  }
    public virtual float GetStartingHealth() { return(0f); }
    public virtual float GetCurrentHealth() { return(0f); }
    public virtual bool GetDeath() { return(Dead); }
    public virtual int GetCurrentSpeedStep() { return(0); }

    // Damage control
    public virtual void ApplyDamage(float damage) { 
        // Debug.Log(UnitName+ "damage = "+ damage);
        Health.ApplyDamage(damage);
    }
    public void SetCurrentHealth(float health){
        if (Active && !Dead) PlayerManager.SetCurrentUnitHealth(health);
        UnitUI.SetCurrentHealth(Health.GetCurrentHealth());
    }
    public virtual void ModuleDestroyed(ElementType elementType) { }
    public virtual void ModuleRepaired(ElementType elementType) { }
    public virtual void BuoyancyCompromised(ElementType ElementType, float damage) { }
    public virtual void SendHitInfoToDamageControl (bool armorPenetrated) { }
    public virtual void SetDamageControlEngineCount(){ }
    public virtual void SetDamageControl(bool damageControl) {
        if (GetComponent<TurretManager>())
            Turrets.SetDamageControl(damageControl);
        if (PlayerManager != null)
            PlayerManager.SetDamageControl(damageControl);
    }
    public virtual void CallDeath() {
        // Debug.Log("CallDeath");
        Dead = true;
        UnitUI.SetDead();
        if (GetComponent<TurretManager>())
            Turrets.SetDeath(true);
        if (GameManager)
            GameManager.UnitDead(this, UnitTeam, Active);
        if (GetComponent<SpawnerScriptToAttach>())
            GetComponent<SpawnerScriptToAttach>().SetDeath(true);

        tag = "Untagged";
    }
    public virtual void DestroyUnit() {
        if (GetComponent<TurretManager>())
            Turrets.SetDeath(true);
        UnitUI.KillAllUIInstances();
        if (GameManager) {
            GameManager.UnitDead(this, UnitTeam, Active);
        }
        Destroy(gameObject);
    }

    // Artificial Intelligence
    public virtual void SetNewEnemyList(List <GameObject> enemiesUnitsObjectList) {
        UnitAI.SetNewEnemyList(enemiesUnitsObjectList);
    }
    public void SendAttackTargetToAI(UnitMasterController rightClickedUnitController){
        // An attack order set by the map
        if (rightClickedUnitController.GetUnitModel() != PlayerSetEnemyTargetUnit) { // If the target is not already selected
            UnitAI.SetPlayerSetTargetByController(rightClickedUnitController);
        }
    }
    public void CleanAttackTargetToAI() {
        // Player-set target removed from the map
        UnitAI.CleanPlayerSetAttackTarget();
    }
    public void SetCurrentTarget(GameObject targetUnit) {
        // A switch in the target via UnitAIController
        PlayerSetEnemyTargetUnit = targetUnit;
        if (Active) {
            PlayerManager.SendCurrentEnemyTarget(targetUnit);
        }
        if (PlayerManager != null) {
            if (targetUnit != null) {
                PlayerManager.SendUnitEnemyTarget(this, targetUnit);
            } else {
                PlayerManager.CleanUnitEnemyTarget(this);
            }
        }
        // Debug.Log ("SetCurrentTarget : "+ PlayerManager+" - "+ targetUnit);
        // PlayerManager.SendUnitEnemyTarget(this, targetUnit);
    }
    public void SendFollowTarget(UnitMasterController rightClickedUnitController){
        // A follow fellow unit order set by the map
    }
    public void SendNewMoveLocationToAI(Vector3 waypointPosition, MapManager.RaycastHitType raycastHitType) {
        // A move order set by the map
        UnitAI.SetNewMoveLocation(waypointPosition, raycastHitType);
    }
    public void CleanMoveOrdersToAI() {
        // All move orders removed from the map
        UnitAI.CleanMoveOrders();
    }
    public void AICallbackCurrentWaypoints(List <Vector3> waypoints) {
        // Update UI with this unit current move orders
        PlayerManager.SendUnitWaypoints(this, waypoints);
    }

    // Main Gameplay
    public virtual void SetUnitFromWorldUnitsManager(WorldSingleUnit unit, bool aiMove, bool aiShoot, bool aiSpawn) {
        // Sets the basic unit info from WorldUnitsManager and the corresponding WorldSingleUnit info.
        // Debug.Log ("SetUnitFromWorldUnitsManager "+unit.GetUnitName()+" AI : "+aiMove+" : "+aiShoot+" : "+aiSpawn);

        // Set all common parameters
            // Debug.Log ("SetUnitFromWorldUnitsManager - " +UnitName);
            UnitCategory = unit.GetUnitCategory();
            UnitSubCategory = unit.GetUnitSubCategory();
            UnitSubCategory_DB = unit.GetUnitSubCategory_DB();
            Nation = unit.GetUnitNation();
            Team = unit.GetUnitTeam();
            UnitTeam = unit.GetUnitTeam_DB();
                gameObject.tag = UnitTeam.id;
            UnitCommandPointsCost = unit.GetUnitCommandPointsCost();
            UnitVictoryPointsValue = unit.GetUnitVictoryPointsValue();
            RepairRate = unit.GetDamageControlRepairRate();

        UnitWorldSingleUnit = unit;
        UnitReference_DB = unit.GetUnitReference_DB();

        // Set Health
            Health = GetComponent<UnitHealth>();
            Health.BeginOperations(this, RepairRate);
            float HP = Health.GetStartingHealth();

        // Set UI
            UnitUI = GetComponent<UnitUI>();
            UnitUI.SetStartingHealth(HP);
            UnitUI.SetCurrentHealth(HP);
            UnitUI.SetUnitTeam(UnitTeam);

            SetUnitName(unit.GetUnitName());

        // Set AI
            UnitAI = GetComponent<UnitAIController>();
            UnitAI.SetUnitTeam(UnitTeam);
            UnitAI.SetName(UnitName);
            // UnitAI.SetName(unit.GetChildrenCanMove(), unit.GetChildrenCanShoot(), unit.GetChildrenCanSpawn());

        // Set turrets
            if (UnitWorldSingleUnit.GetWeaponExists()) {
                Turrets = this.gameObject.AddComponent<TurretManager>();
            }
            if (UnitWorldSingleUnit.GetPlaneWeaponExists()) {
                PlaneWeapons = this.gameObject.AddComponent<PlaneWeaponsManager>();
            }


        // Set HardPoints
            Transform parentHardPointTransform = this.transform.Find("HardPoints").transform;
            foreach (WorldSingleUnit.UnitHardPoint hardPointElement in unit.GetUnitHardPointList()) {
                Transform hardPointTransform = parentHardPointTransform.Find(hardPointElement.GetHardPointID()).transform;
                HardPointComponent hardPointComponent = hardPointTransform.GetComponent<HardPointComponent>();

                if (hardPointElement.GetHardpointType() == CompiledTypes.HardPoints.RowValues.Weapon) {
                    HardPointComponent.SetUpWeaponHardPoint(hardPointElement.GetWeapon(), hardPointComponent, hardPointTransform, Turrets);
                } else if (hardPointElement.GetHardpointType() == CompiledTypes.HardPoints.RowValues.PlaneWeapon) {
                    HardPointComponent.SetUpPlaneWeaponHardPoint(hardPointElement.GetWeapon(), hardPointComponent, hardPointTransform, PlaneWeapons);
                } else {
                    HardPointComponent.SetUpHardPointComponent(hardPointElement, hardPointComponent, hardPointTransform);
                }
                
                // Build the copy in the case the hardpoint is mirrored
                if (hardPointElement.GetIsMirrored()) {
                    GameObject hardPointCopy = new GameObject();
                    Transform hardPointTransformCopy = hardPointCopy.transform;
                    hardPointTransformCopy.parent = hardPointTransform.parent;
                    hardPointTransformCopy.localPosition = new Vector3 (-hardPointTransform.localPosition.x, hardPointTransform.localPosition.y, hardPointTransform.localPosition.z);
                    hardPointTransformCopy.localRotation = Quaternion.Euler (new Vector3 (hardPointTransform.localEulerAngles.x, -hardPointTransform.localEulerAngles.y, hardPointTransform.localEulerAngles.z));


                    HardPointComponent hardPointComponentCopy = hardPointCopy.AddComponent<HardPointComponent>();
                    hardPointComponentCopy.m_LimitTraverse = hardPointComponent.m_LimitTraverse;
                    hardPointComponentCopy.m_LeftTraverse = hardPointComponent.m_RightTraverse;
                    hardPointComponentCopy.m_RightTraverse = hardPointComponent.m_LeftTraverse;
                    hardPointComponentCopy.m_NoFireZones = hardPointComponent.m_NoFireZones;
                    hardPointComponentCopy.m_ElevationZones = hardPointComponent.m_ElevationZones;

                    hardPointComponentCopy.m_Prefab = hardPointComponent.m_Prefab;

                    if (hardPointElement.GetHardpointType() == CompiledTypes.HardPoints.RowValues.Weapon) {
                        HardPointComponent.SetUpWeaponHardPoint(hardPointElement.GetWeapon(), hardPointComponentCopy, hardPointTransformCopy, Turrets);
                    } else if (hardPointElement.GetHardpointType() == CompiledTypes.HardPoints.RowValues.PlaneWeapon) {
                    HardPointComponent.SetUpPlaneWeaponHardPoint(hardPointElement.GetWeapon(), hardPointComponent, hardPointTransform, PlaneWeapons);
                    } else {
                        HardPointComponent.SetUpHardPointComponent(hardPointElement, hardPointComponentCopy, hardPointTransformCopy);
                    }
                }
            }
            // Debug.Log("HardPoints set "+UnitName);

        // Find and set components
            if (this.gameObject.transform.childCount > 0){
                Transform componentsParents = this.gameObject.transform.Find("Model").Find("Colliders");
                // Debug.Log(componentsParents+" . "+UnitName);

                foreach (Transform component in componentsParents) {
                    if (component.GetComponent<HitboxComponent>()) {
                        UnitComponents.Add(component.GetComponent<HitboxComponent>());
                        component.GetComponent<HitboxComponent>().SetUnitController(this);
                        // Debug.Log(component.GetComponent<HitboxComponent>().m_ElementType);
                    }
                }
            }

        // Set turrets
            if (UnitWorldSingleUnit.GetWeaponExists()) {
                // Debug.Log("yes"+ Turrets);
                Turrets.BeginOperations(this);
                UnitAI.SetTurretManager(Turrets);
            }

        // Start AI after turrets were set
            UnitAI.BeginOperations(aiMove, aiShoot, aiSpawn);

        // Send to GameManager if any
            if (WorldUnitsManager.GetGameManager() != null) {
                // Debug.Log ("SetGameManager" +UnitName);
                GameManager = WorldUnitsManager.GetGameManager();
                GameManager.UnitSpawned(this, UnitTeam);
                if (GetComponent<SpawnerScriptToAttach>()){
                    GetComponent<SpawnerScriptToAttach>().SetGameManager(GameManager);
                }
            }

        // Check if unit is within game zone
            Collider[] colliders = Physics.OverlapSphere (transform.position, unit.GetUnitSize());
            bool isToKill = true;
            bool isOutOfGameZone = true;
            foreach (var collider in colliders) {
                if (collider.GetComponent<KillZoneBoundariesManager> () != null) {
                    isToKill = false;
                }
                if (collider.GetComponent<GameBoundariesManager> () != null) {
                    isOutOfGameZone = false;
                    // Debug.Log(UnitName+" has spawned inside the game zone !");
                }
            }
            if (isToKill) {
                Debug.Log(UnitName+ " was spawned outside of bounds and was subsequently destroyed");
                DestroyUnit();
                return;
            }
            if (isOutOfGameZone) {
                Debug.Log(UnitName+ " was spawned outside of the game zone");
                InGameBoundaries = true;
            }
    }

    public virtual void SetActive(bool activate) {
        Active = activate;
        UnitAI.SetAIActive(!Active);
        if (UnitWorldSingleUnit.GetWeaponExists())
            Turrets.SetActive(Active);
        if (GetComponent<SpawnerScriptToAttach>())
            GetComponent<SpawnerScriptToAttach>().SetActive(Active);
    }
    public virtual void SetPause(bool pause) {
        if (GetComponent<TurretManager>())
            Turrets.SetPause();
    }
    public virtual void SetMap(bool mapActive) {
        if (UnitWorldSingleUnit.GetWeaponExists()) {
            Turrets.SetMap(mapActive);
        }
        // Debug.Log("Unit Map open : " + mapActive);
    }

    public virtual void SetFreeCamera(bool freeCam) {
        if (UnitWorldSingleUnit.GetWeaponExists()) {
            Turrets.SetFreeCamera(freeCam);
        }
    }

    public virtual void SetInGameBoundaries(bool action) {
        // True : enters game boundaries / False : exits game boundaries
        // This method should just warn the player / AI that this unit is leaving the game
        if (InGameBoundaries != action) {
            InGameBoundaries = action;
            if (InGameBoundaries) {
                // Debug.Log(UnitName+" has has entered the game zone !");
            } else {
                Debug.Log(UnitName+" has exited the game zone !");
            }
        }
    }

    public virtual void SetInAirfieldZone(bool action) { }

    public void SetPlayerManager(PlayerManager playerManager) {
        // Debug.Log ("SetPlayerManager" +UnitName);
        PlayerManager = playerManager;
        if (GetComponent<TurretManager>())
            Turrets.SetPlayerManager(PlayerManager);
        if (GetComponent<SpawnerScriptToAttach>()){
            GetComponent<SpawnerScriptToAttach>().SetPlayerManager(PlayerManager);
            GetComponent<SpawnerScriptToAttach>().BeginOperations(this, UnitAI);
        }
    }
    // public void SetGameManager(GameManager gameManager){
    //     // Debug.Log ("SetGameManager" +UnitName);
    //     GameManager = gameManager;
    //     if (GetComponent<SpawnerScriptToAttach>()){
    //         GetComponent<SpawnerScriptToAttach>().SetGameManager(GameManager);
    //     }
    // }
}