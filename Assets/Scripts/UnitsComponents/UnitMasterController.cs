using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMasterController : MonoBehaviour {
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
    public virtual void SetTag(WorldUnitsManager.Teams team){}
    public virtual void SetName(string name){}
    public virtual void SetNewEnemyList(List <GameObject> unitsObjectList) {}
    public virtual void SetPlayerManager(PlayerManager playerManager) {}
    public virtual void SetGameManager(GameManager gameManager){}
}