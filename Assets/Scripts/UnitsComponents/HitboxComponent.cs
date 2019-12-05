using UnityEngine;
public class HitboxComponent : MonoBehaviour {

    public enum ElementType {
        hull,
        engine,
        ammo,
        fuel,
        underwater
    }

    [Tooltip("Initial HP of the element")]
    public float m_ElementHealth = 100.0f;
    [Tooltip("Type of element")]
    public ElementType m_ElementType = ElementType.hull;

    [Header("Debug")]
        public bool debug = false;
    [HideInInspector] public float m_CurrentHealth;

    private ShipController ShipController;


    private void Start () {
        m_CurrentHealth = m_ElementHealth;
        ShipController = transform.parent.parent.parent.GetComponent<ShipController>();
        if (m_ElementType == ElementType.hull) {
            ShipController.hullCount += 1;
        }else if (m_ElementType == ElementType.engine){
            ShipController.engine = true;
            ShipController.engineCount += 1;
        }else if (m_ElementType == ElementType.ammo){
            ShipController.ammoCount += 1;
        }else if (m_ElementType == ElementType.fuel){
            ShipController.fuelCount += 1;
        }else if (m_ElementType == ElementType.underwater){
            ShipController.underwaterCount += 1;
        }else{
            Debug.Log("ERROR ! Unrecognized element type : "+ m_ElementType);
        }
    }

    public void TakeDamage (float amount) {
        // Reduce current health by the amount of damage done.
        m_CurrentHealth -= amount;
        if (m_CurrentHealth < 0)
            m_CurrentHealth = 0;

        if (debug){
            // Debug.Log("amount = "+ amount);
            // Debug.Log("m_ElementType = "+ m_ElementType);
            // Debug.Log("m_CurrentHealth = "+ m_CurrentHealth);
        }

        // CheckDeath ();
    }
}