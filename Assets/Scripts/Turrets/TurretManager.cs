using UnityEngine;

public class TurretManager : MonoBehaviour
{    
    public GameObject[] m_Turrets;
    [HideInInspector] public bool m_Active;
    [HideInInspector] public bool m_Dead;
    [HideInInspector] public float RepairRate;
    [HideInInspector] public float TurretsRepairCrew;
    private float targetRange;

    private void Start() {
        for (int i = 0; i < m_Turrets.Length; i++){
            m_Turrets[i].GetComponent<TurretHealth>().RepairRate = RepairRate;
            m_Turrets[i].GetComponent<TurretHealth>().TurretsRepairCrew = TurretsRepairCrew;
        }
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
            if (m_Dead || m_Turrets[i].GetComponent<TurretHealth>().m_Dead) {
                m_Turrets[i].GetComponent<TurretRotation>().m_Dead = true;
                m_Turrets[i].GetComponent<TurretFireManager>().m_Dead = true;  
            } else {
                m_Turrets[i].GetComponent<TurretRotation>().m_Dead = false;
                m_Turrets[i].GetComponent<TurretFireManager>().m_Dead = false;  
                m_Turrets[i].GetComponent<TurretRotation>().m_Active = m_Active;
                m_Turrets[i].GetComponent<TurretFireManager>().m_Active = m_Active;  
            }
        }
    }
}