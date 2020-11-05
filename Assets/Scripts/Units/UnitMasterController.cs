using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMasterController : MonoBehaviour {
    [Header("Global units elements : ")]
    // Same as WorldSingleUnit !

    protected WorldSingleUnit UnitWorldSingleUnit;
    protected CompiledTypes.Global_Units UnitReference_DB;

    protected string UnitName;
    protected CompiledTypes.Units_categories.RowValues UnitCategory;
    protected CompiledTypes.Units_sub_categories.RowValues UnitSubCategory;
    protected CompiledTypes.Units_sub_categories UnitSubCategory_DB;
    protected CompiledTypes.Countries Nation;
    protected CompiledTypes.Teams.RowValues Team;
    protected int UnitCommandPointsCost;
    protected int UnitVictoryPointsValue;

    protected List<HitboxComponent> UnitComponents = new List<HitboxComponent>();
    protected float RepairRate = 0;  public float GetRepairRate(){ return RepairRate; }

    protected bool Active = false;
    protected bool Dead = false;
    private SpawnerScriptToAttach Spawner;
    protected GameManager GameManager;
    protected PlayerManager PlayerManager;

    protected UnitAIController UnitAI;
    protected UnitUI UnitUI;
    protected UnitHealth Health;
    protected TurretManager Turrets;
    private GameObject EnemyTargetUnit;

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
    // Spawn
    private void Start() {
        StartCoroutine(SpawnPauseLogic());
    }
    IEnumerator SpawnPauseLogic(){
        yield return new WaitForSeconds(0.3f);
        ResumeStart();
    }
    public void ResumeStart() {
        if (GameManager != null) {
            // This will get removed in time, the Team and m_Team are conflicting now for test scenarios
            GameManager.UnitSpawned(this.gameObject, Team);
        }
        else if (GameObject.Find("GameManager") != null) {
            GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            GameManager.UnitSpawned(this.gameObject, Team);
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
        Debug.Log("CallDeath");
        Dead = true;
        UnitUI.SetDead();
        if (GetComponent<TurretManager>())
            Turrets.SetDeath(true);
        if (GameManager)
            GameManager.UnitDead(this.gameObject, Team, Active);
        if (GetComponent<SpawnerScriptToAttach>())
            GetComponent<SpawnerScriptToAttach>().SetDeath(true);

        tag = "Untagged";
    }
    public virtual void DestroyUnit() {
        if (GetComponent<TurretManager>())
            Turrets.SetDeath(true);
        UnitUI.KillAllUIInstances();
        if (GameManager) {
            GameManager.UnitDead(this.gameObject, Team, Active);
        }
        Destroy(gameObject);
    }

    // Artificial Intelligence
    public virtual void SetNewEnemyList(List <GameObject> enemiesUnitsObjectList) {
        UnitAI.SetNewEnemyList(enemiesUnitsObjectList);
    }
    public void SetCurrentTarget(GameObject targetUnit) {
        EnemyTargetUnit = targetUnit;
        if (Active) {
            PlayerManager.SendCurrentEnemyTarget(targetUnit);
        }
    }

    // Main Gameplay
    public virtual void SetUnitFromWorldUnitsManager(WorldSingleUnit unit) {
        // Sets the basic unit info from WorldUnitsManager and the corresponding WorldSingleUnit info.
        // Debug.Log ("SetPlayerManager" +unit);

        // Set all common parameters
            UnitName = unit.GetUnitName();
                gameObject.name = UnitName;
            UnitCategory = unit.GetUnitCategory();
            UnitSubCategory = unit.GetUnitSubCategory();
            UnitSubCategory_DB = unit.GetUnitSubCategory_DB();
            Nation = unit.GetUnitNation();
            Team = unit.GetUnitTeam();
                gameObject.tag = Team.ToString();
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
            UnitUI.SetUnitTeam(Team);

        // Set AI
            UnitAI = GetComponent<UnitAIController>();
            UnitAI.BeginOperations();
            UnitAI.SetUnitTeam(Team);
            UnitAI.SetName(UnitName);

        // Set turrets
            if (unit.GetWeaponExists()) {
                this.gameObject.AddComponent<TurretManager>();
                Turrets = GetComponent<TurretManager>();
                UnitAI.SetTurretManager(Turrets);
            }

        // Set HardPoints
            foreach (WorldSingleUnit.UnitHardPoint hardPointElement in unit.GetUnitHardPointList()) {
                // this.transform.Find("HardPoints").transform.Find(hardPointElement.GetHardPointID().ToString()).GetComponent<HardPointComponent>().SetUpHardPointComponent(hardPointElement);
                Transform hardPointTransform = this.transform.Find("HardPoints").transform.Find(hardPointElement.GetHardPointID()).transform;
                HardPointComponent hardPointComponent = hardPointTransform.GetComponent<HardPointComponent>();

                if (hardPointElement.GetHardpointType() == CompiledTypes.HardPoints.RowValues.Weapon) {
                    HardPointComponent.SetUpWeaponHardPoint(hardPointElement.GetWeapon(), hardPointComponent, hardPointTransform, Turrets);
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
            if (GetComponent<TurretManager>()) {
                // Debug.Log("yes"+ Turrets);
                Turrets.BeginOperations(this);
            }
    }

    public virtual void SetActive(bool activate) {
        Active = activate;
        UnitAI.SetAIActive(!Active);
        if (GetComponent<TurretManager>())
            Turrets.SetActive(Active);
        if (GetComponent<SpawnerScriptToAttach>())
            GetComponent<SpawnerScriptToAttach>().SetActive(Active);
    }
    public virtual void SetPause(bool pause) {
        if (GetComponent<TurretManager>())
            Turrets.SetPause();
    }
    public virtual void SetMap(bool mapActive) {}
    public CompiledTypes.Teams.RowValues GetTeam() { return Team; }

    public void SetPlayerManager(PlayerManager playerManager) {
        // Debug.Log ("SetPlayerManager" +UnitName);
        PlayerManager = playerManager;
        if (GetComponent<TurretManager>())
            Turrets.SetPlayerManager(PlayerManager);
        if (GetComponent<SpawnerScriptToAttach>()){
            GetComponent<SpawnerScriptToAttach>().SetPlayerManager(PlayerManager);
            // GetComponent<SpawnerScriptToAttach>().SetUnitController(this);
        }
    }
    public void SetGameManager(GameManager gameManager){
        // Debug.Log ("SetGameManager" +UnitName);
        GameManager = gameManager;
        if (GetComponent<SpawnerScriptToAttach>()){
            GetComponent<SpawnerScriptToAttach>().SetGameManager(GameManager);
        }
    }
    public CompiledTypes.Units_categories.RowValues GetUnitCategory() {
        return UnitCategory;
    }
    public CompiledTypes.Units_sub_categories.RowValues GetUnitSubCategory() {
        return UnitSubCategory;
    }
    public CompiledTypes.Units_sub_categories GetUnitSubCategory_DB() {
        return UnitSubCategory_DB;
    }
    public int GetUnitCommandPointsCost() {
        return UnitCommandPointsCost;
    }
    public int GetUnitVictoryPointsValue() {
        return UnitVictoryPointsValue;
    }
}