using UnityEngine;
using Crest;

public class ShipController : MonoBehaviour {
    [HideInInspector] public bool m_Active;
    // private BoatProbes m_Buoyancy;

    private void Start() {
        // m_Buoyancy = GameObject.Find("Buoyancy").GetComponent<BoatProbes>();
    }

    private void FixedUpdate() {
		// Debug.Log("m_Active :"+ m_Active);
		// Debug.Log("m_Buoyancy :"+ m_Buoyancy);
        // m_Buoyancy.GetComponent<BoatProbes>().m_Active = m_Active;
    }
}