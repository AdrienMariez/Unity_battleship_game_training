using UnityEngine;
public class HitboxComponent : MonoBehaviour {

    [Tooltip("Initial HP of the element")]
    public float m_ElementHealth = 100.0f;
    [Tooltip("Armor of the element (equivalent in rolled steel mm)")]
    public float m_ElementArmor = 100.0f;
    [Tooltip("Type of element")]
    public ShipController.ElementType m_ElementType = ShipController.ElementType.hull;

    [Header("Debug")]
        public bool debug = false;
    [HideInInspector] public float m_CurrentHealth;
    [HideInInspector] public  bool m_Dead;


    private ShipController ShipController;


    private void Start () {
        m_CurrentHealth = m_ElementHealth;
        m_Dead = false;
        InitializeModules();
    }

    public void InitializeModules () {
        // Find ShipController
        ShipController = transform.parent.parent.parent.GetComponent<ShipController>();

        // Depending of the ElementType, send it to the ShipController
        if (m_ElementType == ShipController.ElementType.engine){
            ShipController.engine = true;
            ShipController.engineCount += 1;
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
        // This directly transfers damage to modules to the unit itself
        else if (m_CurrentHealth > 0 && !m_Dead) {
            ModuleDamaged(amount);
        }
        else if (m_Dead) {
            RepairModule();
        }

        if (debug){
            Debug.Log("amount = "+ amount);
            Debug.Log("m_ElementType = "+ m_ElementType);
            Debug.Log("m_CurrentHealth = "+ m_CurrentHealth);
        }
    }

    private void ModuleDamaged (float damage) {
        ShipController.ApplyDamage(damage);
        if (m_ElementType == ShipController.ElementType.underwaterFrontLeft || m_ElementType == ShipController.ElementType.underwaterFrontRight || m_ElementType == ShipController.ElementType.underwaterBackLeft || m_ElementType == ShipController.ElementType.underwaterBackRight) {
            ShipController.BuoyancyCompromised(m_ElementType);
        }
        // ShipController.ModuleDamaged(m_ElementType);
    }

    private void ModuleDestroyed () {
        m_Dead = true;
        ShipController.ModuleDestroyed(m_ElementType);
    }

    private void RepairModule () {
        // If the module is destroyed, repair it to full health while keeping it disabled as long as it's not fully repaired
        float ModuleRepairRate;
        // If the module type is either engine, steering or a turret, accelerate the repair time of the module with damage control teams
        if (m_ElementType == ShipController.ElementType.engine || m_ElementType == ShipController.ElementType.steering) {
            ModuleRepairRate = ShipController.RepairRate * ShipController.EngineRepairCrew * Time.deltaTime;
        }else if (m_ElementType == ShipController.ElementType.turret) {
            ModuleRepairRate = ShipController.RepairRate * ShipController.TurretsRepairCrew * Time.deltaTime;
        } else {
            ModuleRepairRate = ShipController.RepairRate * Time.deltaTime;
        }
        m_CurrentHealth += ModuleRepairRate;

        // Stop repair and reactivate the module when full health is back
        if (m_CurrentHealth >= m_ElementHealth) {
            m_CurrentHealth = m_ElementHealth;
            m_Dead = false;
        }
    }
}