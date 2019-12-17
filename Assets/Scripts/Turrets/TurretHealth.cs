using UnityEngine;
public class TurretHealth : MonoBehaviour {

    [Tooltip("Initial HP of the turret")]
    public float m_ElementHealth = 100.0f;
    [Tooltip("Armor of the turret (equivalent in rolled steel mm)")]
    public float m_ElementArmor = 100.0f;

    [Header("Debug")]
        public bool debug = false;
    private float CurrentHealth;
    private float RepairRate;
    private float TurretsRepairCrew;
    [HideInInspector] public bool Dead;


    private void Start () {
        CurrentHealth = m_ElementHealth;
        Dead = false;
    }

    private void FixedUpdate(){
        if (Dead) {
            RepairModule();
        }
    }

    public void TakeDamage (float amount) {
        // Reduce current health by the amount of damage done.
        CurrentHealth -= amount;

        if (CurrentHealth < 0)
            CurrentHealth = 0;

        if (CurrentHealth == 0 && !Dead) {
            ModuleDestroyed();
        }

        // if (debug){
        //     Debug.Log("turret damage taken = "+ amount);
        //     Debug.Log("turret current health = "+ CurrentHealth);
        // }
    }

    private void ModuleDestroyed () {
        Dead = true;
        // ShipController.ModuleDestroyed(m_ElementType);
    }

    private void RepairModule () {
        // If the module is destroyed, repair it to full health while keeping it disabled as long as it's not fully repaired
        float ModuleRepairRate = RepairRate * TurretsRepairCrew * Time.deltaTime;
        CurrentHealth += ModuleRepairRate;

        // Stop repair and reactivate the module when full health is back
        if (CurrentHealth >= m_ElementHealth) {
            CurrentHealth = m_ElementHealth;
            Dead = false;
        }
        // if (debug){
        //     Debug.Log("Repairing... = "+ CurrentHealth);
        // }
    }

    public void SetRepairRate(float Rate) {
        RepairRate = Rate;
    }
    public void SetTurretRepairRate(float Rate) {
        TurretsRepairCrew = Rate;
    }

    public void SetTurretDeath(bool IsShipDead) {
        Dead = IsShipDead;
        if (IsShipDead) {
            SetRepairRate(0f);
            SetTurretRepairRate(0f);
        }
    }
}