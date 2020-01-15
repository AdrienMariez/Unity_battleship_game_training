using UnityEngine;
using FreeLookCamera;

public class TurretManager : MonoBehaviour
{    
    public GameObject[] m_Turrets;
    private bool Active = false;
    private bool Dead = false;
    private bool Map = false;
    private bool DamageControl = false;
    private bool FreeCamera = false;
    private bool PlayerControl = false;
    private FreeLookCam FreeLookCam;
    private float CameraPercentage;
    private float TargetRange;
    private Vector3 TargetPosition;
    private float MaxRange = -1;
    private float MinRange = 100000;

    private void Start() {
        FreeLookCam = GameObject.Find("FreeLookCameraRig").GetComponent<FreeLookCam>();
        float MaxR;
        float MinR;
        for (int i = 0; i < m_Turrets.Length; i++){
            MaxR = m_Turrets[i].GetComponent<TurretFireManager>().GetMaxRange();
            MinR = m_Turrets[i].GetComponent<TurretFireManager>().GetMinRange();
            if (MaxR > MaxRange)
                MaxRange = MaxR;
            if (MinR < MinRange)
                MinRange = MinR;
        }
    }

    private void FixedUpdate() {
        if (Active && Input.GetButtonDown ("FreeCamera")) { 
            SetFreeCamera(true);
        }
        if (Active && Input.GetButtonUp ("FreeCamera")) { 
            SetFreeCamera(false);
        }


        if (PlayerControl) {
            // Get the angle of the camera here
            CameraPercentage = FreeLookCam.GetTiltPercentage();

            TargetRange = ((MaxRange - MinRange) / 100 * CameraPercentage) + MinRange;
            TargetPosition = FreeLookCam.GetTargetPosition();

            for (int i = 0; i < m_Turrets.Length; i++){
                m_Turrets[i].GetComponent<TurretFireManager>().SetTargetRange(TargetRange);
                m_Turrets[i].GetComponent<TurretRotation>().SetCameraPercentage(CameraPercentage);
                m_Turrets[i].GetComponent<TurretRotation>().SetTargetPosition(TargetPosition);
            }
        }

        /*if (PlayerControl) {
            // TODO put here the manager to switch between turrets types
        }*/
        
        // Debug.Log("TargetRange = "+ TargetRange);
    }

    private void SetPlayerControl(){
        if (Active && !Map && !DamageControl && !Dead && !FreeCamera) {
            PlayerControl = true;
        } else {
            PlayerControl = false;
        }
        for (int i = 0; i < m_Turrets.Length; i++) {
            m_Turrets[i].GetComponent<TurretRotation>().SetPlayerControl(PlayerControl);
            m_Turrets[i].GetComponent<TurretFireManager>().SetPlayerControl(PlayerControl);
        }
    }

    public void SetActive(bool activate) {
        Active = activate;
        SetPlayerControl();
    }
    public void SetMap(bool map) {
        Map = map;
        SetPlayerControl();
    }
    public void SetDamageControl(bool damageControl) {
        DamageControl = damageControl;
        SetPlayerControl();
    }
    public void SetFreeCamera(bool freeCam) {
        FreeCamera = freeCam;
        SetPlayerControl();
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
        Dead = IsShipDead;
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