using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingController : UnitMasterController {
    // [Header("Building units elements : ")]

    protected void Awake() {
        // base.SpawnUnit();
    }

    // private bool ActionPaused = false;
    // private bool ActionPaused2 = false;
    public override void FixedUpdate() {
		// Debug.Log("Active :"+ Active);

        // The following is used to kill an active ship for debug purposes
        // if (Active && !Dead) {
        //     if (Input.GetAxis ("VerticalShip") == 1){
        //         CallDeath();
        //     }
        // }
        // kills all inactive ships for debug purposes
        // if (!Active && !ActionPaused) {
        //     ActionPaused = !ActionPaused;
        //     StartCoroutine(PauseAction());
        // }
        // if (!Active && ActionPaused2 && !Dead) {
        //     CallDeath();
        // }
    }
    
    // IEnumerator PauseAction(){
    //     yield return new WaitForSeconds(3f);
    //     ActionPaused2= true;
    // }
    

    // ALL OVERRIDES METHODS

    // Turrets
    public override void SetSingleTurretStatus(TurretManager.TurretStatusType status, int turretNumber){
        if (PlayerManager != null) { PlayerManager.SetSingleTurretStatus(status, turretNumber); }
    }
    public override void SendPlayerShellToUI(GameObject shellInstance){
        if (PlayerManager != null) { PlayerManager.SendPlayerShellToUI(shellInstance); }
    }

    // UI
    public override float GetStartingHealth() { return(Health.GetStartingHealth()); }
    public override float GetCurrentHealth() { return(Health.GetCurrentHealth()); }

    // Damage control
    public override void ModuleDestroyed(ElementType elementType) {
        // Debug.Log("ElementType :"+ ElementType);
        // Status : 0 : fixed and running / 1 : damaged / 2 : dead
        if (elementType == ElementType.ammo) {
            Health.AmmoExplosion();
        } else if (elementType == ElementType.fuel) {
            Health.StartFire();
        }
    }
    public override void ModuleRepaired(ElementType elementType) {
        if (elementType == ElementType.fuel) {
            Health.EndFire();
        }
    }
    // public override void CallDeath() {
    //     base.CallDeath();    
    // }

}