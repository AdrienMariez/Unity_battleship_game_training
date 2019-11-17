using UnityEngine;

public class TurretManager : MonoBehaviour
{    
    [HideInInspector] public bool m_Active;
    public GameObject[] m_Turrets;

    private void FixedUpdate() {
        // Get vehicle rotation
        Vector3 unitEulerAngles = this.transform.rotation.eulerAngles;
        for (int i = 0; i < m_Turrets.Length; i++){
            m_Turrets[i].GetComponent<TurretRotation>().m_Active = m_Active;
            m_Turrets[i].GetComponent<TurretRotation>().unitEulerAngles = unitEulerAngles;
            m_Turrets[i].GetComponent<TurretFireManager>().m_Active = m_Active;
        }
    }
}