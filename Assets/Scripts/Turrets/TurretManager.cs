using UnityEngine;

public class TurretManager : MonoBehaviour
{    
    [HideInInspector] public bool m_Active;
    [HideInInspector] public bool m_Dead;
    public GameObject[] m_Turrets;
    private float targetRange;

    private void Start (){
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
            if (!m_Dead) {
                m_Turrets[i].GetComponent<TurretRotation>().m_Active = m_Active;
                m_Turrets[i].GetComponent<TurretFireManager>().m_Active = m_Active;  
            } else {
                m_Turrets[i].GetComponent<TurretRotation>().m_Dead = true;
                m_Turrets[i].GetComponent<TurretFireManager>().m_Dead = true;  
            }
        }
    }
}