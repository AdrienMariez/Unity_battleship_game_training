using UnityEngine;
public class HitboxComponent : MonoBehaviour {

    [Tooltip("Initial HP of the element")]
    public float m_ElementHealth = 100.0f;
    [Tooltip("Type of element")]
    public ShipController.ElementType m_ElementType = ShipController.ElementType.hull;

    [Header("Debug")]
        public bool debug = false;
    [HideInInspector] public float m_CurrentHealth;
    private bool m_Dead;


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
            ModuleDamaged();
        }
        // This directly transfers damage to modules to the unit itself
        TransferDamage(amount);

        if (debug){
            // Debug.Log("amount = "+ amount);
            // Debug.Log("m_ElementType = "+ m_ElementType);
            // Debug.Log("m_CurrentHealth = "+ m_CurrentHealth);
        }
    }

    private void TransferDamage (float damage) {
        ShipController.ApplyDamage(damage);
    }

    private void ModuleDamaged () {
        m_Dead = true;
        ShipController.ModuleDamaged(m_ElementType);
    }
}