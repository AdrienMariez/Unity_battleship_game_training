using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMasterController : MonoBehaviour {
    [Header("Global units elements : ")]
    // Same as WorldSingleUnit !
    protected string UnitName;
    protected CompiledTypes.Units_categories.RowValues UnitCategory;
    protected CompiledTypes.Units_sub_categories.RowValues UnitSubCategory;
    protected CompiledTypes.Countries.RowValues Nation;
    protected int UnitCommandPointsCost;
    protected int UnitVictoryPointsValue;

    protected List<HitboxComponent> UnitComponents = new List<HitboxComponent>();
    protected float RepairRate;

    protected bool Active = false;
    protected bool Dead = false;
    protected CompiledTypes.Teams.RowValues Team;
    protected TurretManager Turrets;
    private SpawnerScriptToAttach Spawner;
    protected GameManager GameManager;
    protected PlayerManager PlayerManager;

    protected UnitAIController UnitAI;
    protected UnitUI UnitUI;
    protected UnitHealth Health;
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
        Health.ApplyDamage(damage);
        float currentHealth = Health.GetCurrentHealth();
        UnitUI.SetCurrentHealth(currentHealth);
    }
    public void SetCurrentHealth(float health){ if (Active && !Dead) PlayerManager.SetCurrentUnitHealth(health); }
    public virtual void ModuleDestroyed(ElementType elementType) { }
    public virtual void ModuleRepaired(ElementType elementType) { }
    public virtual void BuoyancyCompromised(ElementType ElementType, float damage) { }
    public virtual void SendHitInfoToDamageControl (bool armorPenetrated) { }
    public virtual void SetDamageControlEngineCount(){ }
    public float GetRepairRate(){ return RepairRate; }
    public virtual void SetDamageControl(bool damageControl) {
        if (GetComponent<TurretManager>())
            Turrets.SetDamageControl(damageControl);
        if (PlayerManager != null)
            PlayerManager.SetDamageControl(damageControl);
    }
    public virtual void CallDeath() {
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
        // Debug.Log ("SetPlayerManager" +UnitName);

        UnitName = unit.GetUnitName();
            gameObject.name = UnitName;
            UnitAI.SetName(UnitName);
        UnitCategory = unit.GetUnitCategory();
        UnitSubCategory = unit.GetUnitSubCategory();
        Nation = unit.GetUnitNation();
        Team = unit.GetUnitTeam();
            gameObject.tag = Team.ToString();
            UnitAI.SetUnitTeam(Team);
            UnitUI.SetUnitTeam(Team);

        UnitCommandPointsCost = unit.GetUnitCommandPointsCost();
        UnitVictoryPointsValue = unit.GetUnitVictoryPointsValue();
    }
    public void SpawnUnit() {

        // Set Health
            Health = GetComponent<UnitHealth>();
            Health.BeginOperations(this);
            float HP = Health.GetStartingHealth();

        // Set UI
            UnitUI = GetComponent<UnitUI>();
            UnitUI.SetStartingHealth(HP);
            UnitUI.SetCurrentHealth(HP);
        // Set turrets
            if (GetComponent<TurretManager>())
                Turrets = GetComponent<TurretManager>();
        // Set AI
            UnitAI = GetComponent<UnitAIController>();
            UnitAI.BeginOperations();
            if (GetComponent<TurretManager>())
                UnitAI.SetTurretManager(Turrets);

        // Find and set components
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
            GetComponent<SpawnerScriptToAttach>().SetGameManager(GameManager);
            GetComponent<SpawnerScriptToAttach>().SetPlayerManager(PlayerManager);
            // GetComponent<SpawnerScriptToAttach>().SetUnitController(this);
        }
    }
    public void SetGameManager(GameManager gameManager){
        GameManager = gameManager;
    }
    public CompiledTypes.Units_categories.RowValues GetUnitCategory() {
        return UnitCategory;
    }
    public CompiledTypes.Units_sub_categories.RowValues GetUnitSubCategory() {
        return UnitSubCategory;
    }
    public int GetUnitCommandPointsCost() {
        return UnitCommandPointsCost;
    }
    public int GetUnitVictoryPointsValue() {
        return UnitVictoryPointsValue;
    }
}