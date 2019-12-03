using UnityEngine;

public class TurretManager : MonoBehaviour
{    
    [HideInInspector] public bool m_Active;
    public GameObject[] m_Turrets;
    // private float m_maxRangeGlobal = 1000000;
    // private float m_minRangeGlobal = 1000000;
    private float targetRange;

    private void Start (){
        // for (int i = 0; i < m_Turrets.Length; i++){
        //     if (m_maxRangeGlobal < m_Turrets[i].GetComponent<TurretFireManager>().m_MaxRange) {  
        //         m_maxRangeGlobal = m_Turrets[i].GetComponent<TurretFireManager>().m_MaxRange;
        //     }
        //     if (m_minRangeGlobal > m_Turrets[i].GetComponent<TurretFireManager>().m_MinRange) {  
        //         m_minRangeGlobal = m_Turrets[i].GetComponent<TurretFireManager>().m_MinRange;
        //     }
        // }
    }

    private void FixedUpdate() {
        // Get vehicle rotation
        // Vector3 unitEulerAngles = this.transform.rotation.eulerAngles;
        for (int i = 0; i < m_Turrets.Length; i++){
            if (m_Turrets[i].GetComponent<TurretFireManager>().m_DirectorTurret) {
                targetRange = m_Turrets[i].GetComponent<TurretFireManager>().targetRange; 
            } else {
                m_Turrets[i].GetComponent<TurretFireManager>().targetRange = targetRange;
            }
            m_Turrets[i].GetComponent<TurretRotation>().m_Active = m_Active;
            // m_Turrets[i].GetComponent<TurretRotation>().unitEulerAngles = unitEulerAngles;
            m_Turrets[i].GetComponent<TurretFireManager>().m_Active = m_Active;
        }
    }
}