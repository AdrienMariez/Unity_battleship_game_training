using UnityEngine;
using Crest;

public class ShipController : MonoBehaviour {
    [Tooltip("Components (game object with collider + Hitbox Component script)")]
    public GameObject[] m_ShipComponents;

    [HideInInspector] public bool m_Active;
    [HideInInspector] public bool m_Dead;

    private ShipBuoyancy Buoyancy;
    private ShipMovement Movement;
    private ShipHealth Health;
    private TurretManager Turrets;
    private ShipDamageControl DamageControl;

    [HideInInspector] public bool engine = false;
    [HideInInspector] public float engineCount = 0;

    [HideInInspector] public float RepairRate;
    [HideInInspector] public float EngineRepairCrew;
    [HideInInspector] public float FireRepairCrew;
    [HideInInspector] public float WaterRepairCrew;
    [HideInInspector] public float TurretsRepairCrew;

    public enum ElementType {
        hull,
        engine,
        steering,
        ammo,
        fuel,
        turret,
        underwaterFrontLeft,
        underwaterFrontRight,
        underwaterBackLeft,
        underwaterBackRight
    }

    private void Start() {
        m_Dead = false;
        Buoyancy = GetComponent<ShipBuoyancy>();
        Movement = GetComponent<ShipMovement>();
        Health = GetComponent<ShipHealth>();
        if (GetComponent<ShipDamageControl>()) {
            DamageControl = GetComponent<ShipDamageControl>();
            RepairRate = DamageControl.RepairRate;
        }
        if (GetComponent<TurretManager>()) {
            Turrets = GetComponent<TurretManager>();
            Turrets.RepairRate = RepairRate;
            Turrets.TurretsRepairCrew = TurretsRepairCrew;
        }
    }

    private void FixedUpdate() {
		// Debug.Log("m_Active :"+ m_Active);
		// Debug.Log("m_Buoyancy :"+ m_Buoyancy);
        if (!m_Dead) {
            Movement.m_Active = m_Active;
            Turrets.m_Active = m_Active;
        } else {
            // Prevent any action from the ship once it is dead
            Buoyancy.m_Dead = true;
            Movement.m_Dead = true;
            Turrets.m_Dead = true;
        }
    }

    public void ApplyDamage(float damage) {
        Health.ApplyDamage(damage);
    }

    public void BuoyancyCompromised(ElementType ElementType) {
        //If a water tight compartment is hit, apply effects here
    }

    public void ModuleDestroyed(ElementType ElementType) {
        // Debug.Log("ElementType :"+ ElementType);
    }

    public void CallDeath() {
        m_Dead = true;
        tag = "Untagged";

        // Sink the ship
        Buoyancy.Sink(2f);
    }
}