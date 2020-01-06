using UnityEngine;

public class TurretManager : MonoBehaviour
{    
    public GameObject[] m_Turrets;
    private bool Active;
    private float TurretsRepairCrew;
    private float TargetRange;

    private void Start() {
    }

    private void FixedUpdate() {
        /*if (Active) {
            // TODO put here the manager to switch between turrets types
        }*/
        
        for (int i = 0; i < m_Turrets.Length; i++){
            if (m_Turrets[i].GetComponent<TurretFireManager>().m_DirectorTurret) {
                TargetRange = m_Turrets[i].GetComponent<TurretFireManager>().GetTargetRange(); 
            } else {
                m_Turrets[i].GetComponent<TurretFireManager>().SetTargetRange(TargetRange);
            }
        }
        // Debug.Log("TargetRange = "+ TargetRange);
    }

    public void SetActive(bool activate) {
        Active = activate;
        for (int i = 0; i < m_Turrets.Length; i++){
            m_Turrets[i].GetComponent<TurretRotation>().SetActive(Active);
            m_Turrets[i].GetComponent<TurretFireManager>().SetActive(Active);
        }
    }
    public void SetMap(bool map) {
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