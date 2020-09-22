using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMasterController : MonoBehaviour {
    [Header("Global units elements : ")]
    // Same as WorldSingleUnit !
    public string m_UnitName;
    public WorldUnitsManager.UnitCategories m_UnitCategory;
    public WorldUnitsManager.UnitSubCategories m_UnitSubCategory;
    public WorldUnitsManager.SimpleTeams m_Team;
    public WorldUnitsManager.Nations m_Nation;
    public int m_UnitCost;
    public int m_UnitPointValue;
    [Tooltip("Place here the prefab of the map model")] public GameObject m_UnitMapModel;


    protected bool Active = false;
    protected bool Dead = false;
    protected WorldUnitsManager.Teams Team;  // Ok this is weird, there should be only One Team OR m_Team but fixing this now will ruin all test scenario.
    protected TurretManager Turrets;
    private SpawnerScriptToAttach Spawner;
    protected GameManager GameManager;
    protected PlayerManager PlayerManager;

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
    // Turrets
    public virtual void SetTotalTurrets(int turrets) { }
    public virtual void SetMaxTurretRange(float maxRange) { }
    public virtual void SetDamagedTurrets(int turrets) { }
    public virtual void SetSingleTurretStatus(TurretManager.TurretStatusType status, int turretNumber){ }
    public virtual void SendPlayerShellToUI(GameObject shellInstance){ }
    public virtual void FeedbackShellHit (bool armorPenetrated) { }

    // UI
    public virtual void SetUIElement(GameObject tempUI) {}
    public virtual void SetUIMapElement(GameObject tempUI) {}
    public virtual void KillAllUIInstances() { }
    public virtual float GetStartingHealth() { return(0f); }
    public virtual float GetCurrentHealth() { return(0f); }
    public virtual bool GetDeath() { return(Dead); }
    public virtual int GetCurrentSpeedStep() { return(0); }

    // Damage control
    public virtual void ApplyDamage(float damage) { }
    public virtual void ModuleDestroyed(ElementType elementType) { }
    public virtual void ModuleRepaired(ElementType elementType) { }
    public virtual void BuoyancyCompromised(ElementType ElementType, float damage) { }
    public virtual void SendHitInfoToDamageControl (bool armorPenetrated) { }
    public virtual void SetDamageControlEngineCount(){ }
    public virtual float GetRepairRate(){ return(0f); }
    public virtual void SetDamageControl(bool damageControl) {
        if (GetComponent<TurretManager>())
            Turrets.SetDamageControl(damageControl);
        if (PlayerManager != null)
            PlayerManager.SetDamageControl(damageControl);
    }
    public virtual void CallDeath() {
        Dead = true;
        if (GetComponent<TurretManager>())
            Turrets.SetDeath(true);
        if (GameManager)
            GameManager.UnitDead(this.gameObject, Team, Active);
        if (GetComponent<SpawnerScriptToAttach>())
            GetComponent<SpawnerScriptToAttach>().SetDeath(true);

        tag = "Untagged";
    }
    public virtual void DestroyUnit() {
        Destroy(gameObject);
    }

    // Main Gameplay
    public void SpawnUnit() {
        if (GetComponent<TurretManager>())
            Turrets = GetComponent<TurretManager>();
    }
    public virtual void SetActive(bool activate) {
        Active = activate;
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
    public virtual void SetTag(WorldUnitsManager.Teams team){
        gameObject.tag = team.ToString("g");
        Team = team;
    }
    public WorldUnitsManager.SimpleTeams GetTeam() { return m_Team; }
    public virtual void SetName(string name){ gameObject.name = name; }
    public virtual void SetNewEnemyList(List <GameObject> unitsObjectList) {}
    public void SetPlayerManager(PlayerManager playerManager) {
        PlayerManager = playerManager;
        if (GetComponent<TurretManager>())
            Turrets.SetPlayerManager(PlayerManager);
        if (GetComponent<SpawnerScriptToAttach>()){
            GetComponent<SpawnerScriptToAttach>().SetPlayerManager(PlayerManager);
            GetComponent<SpawnerScriptToAttach>().SetUnitController(this);
        }
    }
    public void SetGameManager(GameManager gameManager){
        GameManager = gameManager;
    }
    public GameObject GetUnitMapModel() {
        return m_UnitMapModel;
    }
    public void ResumeStart() {
        if (GameManager != null) {
            // This will get removed in time, the Team and m_Team are conflicting now for test scenarios
            if (Team != null) { GameManager.UnitSpawned(this.gameObject, Team); }
            else{ GameManager.UnitSpawnedConvertFromSimpleTeam(this.gameObject, m_Team); }
        }
        else if (GameObject.Find("GameManager") != null) {
            GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            if (Team != null) { GameManager.UnitSpawned(this.gameObject, Team); }
            else{ GameManager.UnitSpawnedConvertFromSimpleTeam(this.gameObject, m_Team); }
        }
    }
}