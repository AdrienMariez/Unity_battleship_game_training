using UnityEngine;
public class TurretHealth : MonoBehaviour {

    [Tooltip("Initial HP of the turret")]
    public float m_ElementHealth = 100.0f;
    [Tooltip("Armor of the turret (equivalent in rolled steel mm)")]
    public float m_ElementArmor = 100.0f;

    [Header("Debug")]
        public bool debug = false;
    [HideInInspector] public float m_CurrentHealth;
    [HideInInspector] public float RepairRate;
    [HideInInspector] public float TurretsRepairCrew;
    [HideInInspector] public bool m_Dead;


    private void Start () {
        m_CurrentHealth = m_ElementHealth;
        m_Dead = false;
    }

    private void FixedUpdate(){
        if (m_Dead) {
            RepairModule();
        }
    }

    public void TakeDamage (float amount) {
        // Reduce current health by the amount of damage done.
        m_CurrentHealth -= amount;

        if (m_CurrentHealth < 0)
            m_CurrentHealth = 0;

        if (m_CurrentHealth == 0 && !m_Dead) {
            ModuleDestroyed();
        }

        // if (debug){
        //     Debug.Log("turret damage taken = "+ amount);
        //     Debug.Log("turret current health = "+ m_CurrentHealth);
        // }
    }

    private void ModuleDestroyed () {
        m_Dead = true;
        // ShipController.ModuleDestroyed(m_ElementType);
    }

    private void RepairModule () {
        // If the module is destroyed, repair it to full health while keeping it disabled as long as it's not fully repaired
        float ModuleRepairRate = RepairRate * TurretsRepairCrew * Time.deltaTime;
        m_CurrentHealth += ModuleRepairRate;

        // Stop repair and reactivate the module when full health is back
        if (m_CurrentHealth >= m_ElementHealth) {
            m_CurrentHealth = m_ElementHealth;
            m_Dead = false;
        }
        // if (debug){
        //     Debug.Log("Repairing... = "+ m_CurrentHealth);
        // }
    }
}