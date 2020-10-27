using UnityEngine;
public class TurretHealth : MonoBehaviour {
    private float ElementArmor; public void SetElementArmor(float elementArmor){ ElementArmor = elementArmor; } public float GetElementArmor(){ return ElementArmor; }
    private float ElementStartingHealth;  public void SetStartingHealth(float startingHealth){ ElementStartingHealth = startingHealth; } public float GetStartingHealth(){ return ElementStartingHealth; }
    private float CurrentHealth;
    private float RepairRate;
    private float TurretsRepairCrew;
    private TurretFireManager TurretFireManager;
    private TurretRotation TurretRotation;
    private bool Dead = false;
    private bool ShipDead = false;
    private TurretManager TurretManager;

    public void BeginOperations(TurretManager turretManager, TurretRotation turretRotation, TurretFireManager turretFireManager){
        TurretManager = turretManager;
        TurretRotation = turretRotation;
        TurretFireManager = turretFireManager;

        CurrentHealth = ElementStartingHealth;
    }

    private void FixedUpdate(){
        if (Dead && !ShipDead) {
            RepairModule();
        }
    }

    public void TakeDamage (float amount) {
        // Reduce current health by the amount of damage done.
        CurrentHealth -= amount;

        if (CurrentHealth < 0)
            CurrentHealth = 0;

        if (CurrentHealth == 0 && !Dead) {
            TurretDestroyed();
        }

        // if (debug){
        //     Debug.Log("turret damage taken = "+ amount);
        //     Debug.Log("turret current health = "+ CurrentHealth);
        // }
    }

    private void TurretDestroyed () {
        Dead = true;
        TurretFireManager.SetTurretDeath(true);
        TurretRotation.SetTurretDeath(true);
        TurretManager.SetSingleTurretDeath(Dead);
    }

    private void RepairModule () {
        // If the module is destroyed, repair it to full health while keeping it disabled as long as it's not fully repaired
        float ModuleRepairRate = RepairRate * (TurretsRepairCrew + 1) * Time.deltaTime;
        CurrentHealth += ModuleRepairRate;

        // Stop repair and reactivate the module when full health is back
        if (CurrentHealth >= ElementStartingHealth) {
            TurretRepaired();
        }
        // if (debug){
        //     Debug.Log("Repairing... = "+ CurrentHealth);
        // }
    }

    private void TurretRepaired(){
        CurrentHealth = ElementStartingHealth;
        Dead = false;
        TurretFireManager.SetTurretDeath(false);
        TurretRotation.SetTurretDeath(false);
        TurretManager.SetSingleTurretDeath(Dead);
    }

    public void SetRepairRate(float Rate) {
        RepairRate = Rate;
    }
    public void SetTurretRepairRate(float Rate) {
        TurretsRepairCrew = Rate;
    }

    public void SetShipDeath(bool IsShipDead) {
        ShipDead = IsShipDead;
        if (IsShipDead) {
            SetRepairRate(0f);
            SetTurretRepairRate(0f);
        } else {
            CurrentHealth = ElementStartingHealth;
        }
    }
}