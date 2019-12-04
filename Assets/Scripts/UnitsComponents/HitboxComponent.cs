using UnityEngine;
public class HitboxComponent : MonoBehaviour {

    internal enum ElementType {
        hull,
        engine,
        ammo,
        fuel,
        underwater
    }

    [Tooltip("Initial HP of the element")]
    public float m_ElementHealth = 100.0f;
    [Tooltip("Type of element")]
    [SerializeField] private ElementType m_ElementType = ElementType.hull;

    [Header("Debug")]
        public bool debug = false;

    [HideInInspector] public float m_CurrentHealth;

    private void Start () {
        m_CurrentHealth = m_ElementHealth;
    }

    public void TakeDamage (float amount) {
        // Reduce current health by the amount of damage done.
        m_CurrentHealth -= amount;
        if (m_CurrentHealth < 0)
            m_CurrentHealth = 0;

        if (debug){
            Debug.Log("amount = "+ amount);
            Debug.Log("m_ElementType = "+ m_ElementType);
            Debug.Log("m_CurrentHealth = "+ m_CurrentHealth);
        }

        // CheckDeath ();
    }
}