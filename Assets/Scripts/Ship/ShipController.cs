using UnityEngine;
using Crest;

public class ShipController : MonoBehaviour {
    [Tooltip("Components (game object with collider + Hitbox Component script)")]
    public GameObject[] m_ShipComponents; 
    [HideInInspector] public bool m_Active;
    private ShipBuoyancy m_Buoyancy;
    private ShipMovement m_Movement;
    [HideInInspector] public bool engine = false;
    [HideInInspector] public float engineCount;
    [HideInInspector] public float hullCount;
    [HideInInspector] public float ammoCount;
    [HideInInspector] public float fuelCount;
    [HideInInspector] public float underwaterCount;
    [HideInInspector] public float underwater2Count;

    private void Start() {
        m_Buoyancy = GetComponent<ShipBuoyancy>();
        m_Movement = GetComponent<ShipMovement>();
    }

    private void FixedUpdate() {
		// Debug.Log("m_Active :"+ m_Active);
		// Debug.Log("m_Buoyancy :"+ m_Buoyancy);
        m_Buoyancy.m_Active = m_Active;
        m_Movement.m_Active = m_Active;
    }
}