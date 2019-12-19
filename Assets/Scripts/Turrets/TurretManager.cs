using UnityEngine;

public class TurretManager : MonoBehaviour
{    
    public GameObject[] m_Turrets;
    [HideInInspector] public bool m_Active;
    private float TurretsRepairCrew;
    private float TargetRange;
    private bool MapActive;

    private void Start() {
    }

    private void FixedUpdate() {
        if (m_Active) {
            
        }
        
        for (int i = 0; i < m_Turrets.Length; i++){
            if (m_Turrets[i].GetComponent<TurretFireManager>().m_DirectorTurret) {
                TargetRange = m_Turrets[i].GetComponent<TurretFireManager>().GetTargetRange(); 
            } else {
                m_Turrets[i].GetComponent<TurretFireManager>().SetTargetRange(TargetRange);
            }
            m_Turrets[i].GetComponent<TurretRotation>().m_Active = m_Active;
            m_Turrets[i].GetComponent<TurretFireManager>().m_Active = m_Active;
        }
        // Debug.Log("TargetRange = "+ TargetRange);
    }

    public void SetMap(bool map) {
        MapActive = map;
        for (int i = 0; i < m_Turrets.Length; i++) {
            m_Turrets[i].GetComponent<TurretRotation>().SetMap(map);
        }
    }
    public void SetRepairRate(float Rate) {
        for (int i = 0; i < m_Turrets.Length; i++){
            m_Turrets[i].GetComponent<TurretHealth>().SetRepairRate(Rate);
        }
    }
    public void SetTurretRepairRate(float Rate) {
        for (int i = 0; i < m_Turrets.Length; i++){
            m_Turrets[i].GetComponent<TurretHealth>().SetTurretRepairRate(Rate);
        }
    }
    public void SetDeath(bool IsShipDead) {
        for (int i = 0; i < m_Turrets.Length; i++){
            m_Turrets[i].GetComponent<TurretHealth>().SetTurretDeath(IsShipDead);
            m_Turrets[i].GetComponent<TurretRotation>().SetTurretDeath(IsShipDead);
            m_Turrets[i].GetComponent<TurretFireManager>().SetTurretDeath(IsShipDead);
        }
    }

    public GameObject[] GetTurrets() {
        return m_Turrets;
    }
    public float GetTargetRange() {
        return TargetRange;
    }
}