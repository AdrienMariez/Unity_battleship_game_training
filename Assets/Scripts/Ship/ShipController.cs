using UnityEngine;
using Crest;

public class ShipController : MonoBehaviour {
    [Tooltip("Components (game object with collider + Hitbox Component script)")]
    public GameObject[] m_ShipComponents; 
    [HideInInspector] public bool m_Active;
    [HideInInspector] public bool m_Dead;
    private ShipBuoyancy m_Buoyancy;
    private ShipMovement m_Movement;
    private ShipHealth m_Health;
    private TurretManager m_Turrets;
    [HideInInspector] public bool engine = false;
    [HideInInspector] public float engineCount = 0;

    public enum ElementType {
        hull,
        engine,
        ammo,
        fuel,
        underwater
    }

    private void Start() {
        m_Dead = false;
        m_Buoyancy = GetComponent<ShipBuoyancy>();
        m_Movement = GetComponent<ShipMovement>();
        m_Health = GetComponent<ShipHealth>();
        m_Turrets = GetComponent<TurretManager>();
    }

    private void FixedUpdate() {
		// Debug.Log("m_Active :"+ m_Active);
		// Debug.Log("m_Buoyancy :"+ m_Buoyancy);
        if (!m_Dead) {
            m_Movement.m_Active = m_Active;
            m_Turrets.m_Active = m_Active;
        } else {
            // Prevent any action from the ship once it is dead
            m_Buoyancy.m_Dead = true;
            m_Movement.m_Dead = true;
            m_Turrets.m_Dead = true;
        }
    }

    public void ApplyDamage(float damage) {
        m_Health.ApplyDamage(damage);
    }

    public void ModuleDamaged(ElementType ElementType) {
        // Debug.Log("ElementType :"+ ElementType);

    }

    public void CallDeath() {
        m_Dead = true;
        tag = "Untagged";

        // Sink the ship
        m_Buoyancy.Sink(2f);
    }
}