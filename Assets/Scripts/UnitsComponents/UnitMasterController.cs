using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMasterController : MonoBehaviour {
    [Header("Global units elements : ")]
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
    protected WorldUnitsManager.Teams Team;
    protected TurretManager Turrets;
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
    public virtual bool GetDeath() { return(false); }
    public virtual int GetCurrentSpeedStep() { return(0); }

    // Damage control
    public virtual void ApplyDamage(float damage) { }
    public virtual void ModuleDestroyed(ElementType elementType) { }
    public virtual void ModuleRepaired(ElementType elementType) { }
    public virtual void BuoyancyCompromised(ElementType ElementType, float damage) { }
    public virtual void SendHitInfoToDamageControl (bool armorPenetrated) { }
    public virtual void SetDamageControlEngineCount(){ }
    public virtual float GetRepairRate(){ return(0f); }
    public virtual void DestroyUnit() {
        Destroy(gameObject);
    }

    // Main Gameplay
    public virtual void SetActive(bool active) {}
    public virtual void SetPause(bool pause) {}
    public virtual void SetMap(bool mapActive) {}
    public virtual void SetTag(WorldUnitsManager.Teams team){
        gameObject.tag = team.ToString("g");
        Team = team;
    }
    public virtual void SetName(string name){
        gameObject.name = name;
    }
    public virtual void SetNewEnemyList(List <GameObject> unitsObjectList) {}
    public void SetPlayerManager(PlayerManager playerManager) {
        PlayerManager = playerManager;
        // PlayerManager.UnitSpawned(this.gameObject, Team, m_UnitType);
        if (GetComponent<TurretManager>())
            Turrets.SetPlayerManager(PlayerManager);
    }
    public void SetGameManager(GameManager gameManager){
        GameManager = gameManager;
    }
    public GameObject GetUnitMapModel() {
        return m_UnitMapModel;
    }
    public void ResumeStart() {
        if (GameManager != null) {
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