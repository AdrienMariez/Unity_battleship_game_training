using UnityEngine;
using FreeLookCamera;

public class TurretManager : MonoBehaviour
{    
    public GameObject[] m_Turrets;
    private bool Active = false;
    private bool Dead = false;
    private bool Pause = false;
    private bool Map = false;
    private bool DamageControl = false;
    private bool FreeCamera = false;
    private bool PlayerControl = false;
    private FreeLookCam FreeLookCam;
    private ShipController ShipController;
    private float CameraPercentage;
    private float TargetRange;
    private Vector3 TargetPosition;
    private float MaxRange = -1;
    private float MinRange = 100000;

    private int TotalTurrets = 0;
    private int WorkingTurrets;
    string TurretStatus = "";

    private bool AIControl = false;
    private Vector3 AITargetPosition;
    private float AITargetRange;

    private void Start() {
        FreeLookCam = GameObject.Find("FreeLookCameraRig").GetComponent<FreeLookCam>();
        if (GetComponent<ShipController>())
            ShipController = GetComponent<ShipController>();
        float MaxR;
        float MinR;
        for (int i = 0; i < m_Turrets.Length; i++){
            TotalTurrets++;
            MaxR = m_Turrets[i].GetComponent<TurretFireManager>().GetMaxRange();
            MinR = m_Turrets[i].GetComponent<TurretFireManager>().GetMinRange();
            if (MaxR > MaxRange)
                MaxRange = MaxR;
            if (MinR < MinRange)
                MinRange = MinR;
            m_Turrets[i].GetComponent<TurretHealth>().SetTurretManager(this);
        }
        WorkingTurrets = TotalTurrets;
        if (GetComponent<ShipController>())
            ShipController.SetTotalTurrets(TotalTurrets);
        SetPlayerControl();
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
            TurretStatus = "";
            for (int i = 0; i < m_Turrets.Length; i++){
                m_Turrets[i].GetComponent<TurretFireManager>().SetTargetRange(TargetRange);
                m_Turrets[i].GetComponent<TurretRotation>().SetCameraPercentage(CameraPercentage);
                m_Turrets[i].GetComponent<TurretRotation>().SetTargetPosition(TargetPosition);
                TurretStatus += m_Turrets[i].GetComponent<TurretFireManager>().GetTurretStatus();
            }
            if (GetComponent<ShipController>())
                ShipController.SetTurretStatus(TurretStatus);
        }
        if (!PlayerControl && AIControl) {
            float fakeCameraPercentage = AITargetRange * 100 / (MaxRange - MinRange);
            // Debug.Log("AITargetUnit.transform.position = "+ AITargetUnit.transform.position);
            for (int i = 0; i < m_Turrets.Length; i++){
                m_Turrets[i].GetComponent<TurretFireManager>().SetTargetRange(AITargetRange);
                m_Turrets[i].GetComponent<TurretRotation>().SetCameraPercentage(fakeCameraPercentage);
                m_Turrets[i].GetComponent<TurretRotation>().SetTargetPosition(AITargetPosition);
            }
        }

        /*if (PlayerControl) {
            // TODO put here the manager to switch between turrets types
        }*/
        
        // Debug.Log("TargetRange = "+ TargetRange);
    }

    private void SetPlayerControl(){
        if (Active && !Map && !DamageControl && !Dead && !FreeCamera && !Pause) {
            PlayerControl = true;
            AIControl = false;
        } else {
            PlayerControl = false;
            if (AITargetRange < MaxRange && AITargetRange > MinRange) {
                AIControl = true;
            } else {
                AIControl = false;
            }
        }
        for (int i = 0; i < m_Turrets.Length; i++) {
            m_Turrets[i].GetComponent<TurretRotation>().SetPlayerControl(PlayerControl);
            m_Turrets[i].GetComponent<TurretFireManager>().SetPlayerControl(PlayerControl);

            m_Turrets[i].GetComponent<TurretRotation>().SetAIControl(AIControl);
            m_Turrets[i].GetComponent<TurretFireManager>().SetAIControl(AIControl);
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
    public void SetPause() {
        Pause = !Pause;
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
    public void SetSingleTurretDeath(bool isTurretDead){
        if (isTurretDead) {
            WorkingTurrets--;
        } else{
            WorkingTurrets++;
        }
        if (GetComponent<ShipController>())
            ShipController.SetDamagedTurrets(TotalTurrets - WorkingTurrets);
    }
    public void SetDeath(bool IsShipDead) {
        Dead = IsShipDead;
        for (int i = 0; i < m_Turrets.Length; i++){
            m_Turrets[i].GetComponent<TurretHealth>().SetShipDeath(IsShipDead);
            m_Turrets[i].GetComponent<TurretRotation>().SetTurretDeath(IsShipDead);
            m_Turrets[i].GetComponent<TurretFireManager>().SetTurretDeath(IsShipDead);
        }
    }

    public GameObject[] GetTurrets() {
        return m_Turrets;
    }
    public string GetTurretStatus() {
        return TurretStatus;
    }
    public float GetTargetRange() {
        return TargetRange;
    }

    public void SetAITargetToFireOn(Vector3 targetPosition) {
        AITargetPosition = targetPosition;
        // A bit of cheating here, before a correct fake camera angle can be implemented
        targetPosition.y += 500;
        AITargetPosition = targetPosition;
    }
    public void SetAITargetRange(float targetRange) {
        AITargetRange = targetRange;
    }
}