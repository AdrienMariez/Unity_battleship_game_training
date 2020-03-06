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
    private TurretFireManager TurretFireManager;
    private TurretRotation TurretRotation;
    private bool Dead = false;
    private bool ShipDead = false;
    private TurretManager TurretManager;


    private void Start () {
        CurrentHealth = m_ElementHealth;
        TurretFireManager = GetComponent<TurretFireManager>();
        TurretRotation = GetComponent<TurretRotation>();
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
        if (CurrentHealth >= m_ElementHealth) {
            TurretRepaired();
        }
        // if (debug){
        //     Debug.Log("Repairing... = "+ CurrentHealth);
        // }
    }

    private void TurretRepaired(){
        CurrentHealth = m_ElementHealth;
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
            CurrentHealth = m_ElementHealth;
        }
    }
    public void SetTurretManager(TurretManager turretManager){ TurretManager = turretManager; }
    public float GetElementArmor(){ return m_ElementArmor; }
}