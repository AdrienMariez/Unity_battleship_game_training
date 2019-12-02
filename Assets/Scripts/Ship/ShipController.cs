using UnityEngine;
using Crest;

public class ShipController : MonoBehaviour {
    [HideInInspector] public bool m_Active;
    private ShipBuoyancy m_Buoyancy;
    private ShipMovement m_Movement;

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