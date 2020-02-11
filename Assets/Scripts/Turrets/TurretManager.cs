using UnityEngine;
using FreeLookCamera;
using System.Collections;
using System.Collections.Generic;

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
    private bool ActionPaused = false;
    private PlayerManager PlayerManager;
    private FreeLookCam FreeLookCam;
    private ShipController ShipController;
    private TurretFireManager.TurretType CurrentControlledTurretType;
    [Header("Artillery")]
        // private List <GameObject> ArtilleryTurrets;
        private List <GameObject> ArtilleryTurrets = new List<GameObject>();
        private float CameraPercentage;
        private float TargetRange;
        private Vector3 TargetPosition;
        private float MaxRange = -1;
        private float MinRange = 100000;
    [Header("AA")]
        private List <GameObject> AATurrets = new List<GameObject>();
    [Header("Torpedoes")]
        private List <GameObject> TorpedoTurrets = new List<GameObject>();
    [Header("DepthCharge")]
        private List <GameObject> DepthChargeTurrets = new List<GameObject>();

    private int TotalTurrets = 0;
    private int WorkingTurrets;
    public enum TurretStatusType {
        Ready,
        Reloading,
        PreventFire,
        Dead
    }
    private List <TurretStatusType> TurretStatus = new List<TurretStatusType>();

    private bool AIControl = false;
    private bool AIHasATarget = false;
    private Vector3 AITargetPosition;
    private float AITargetRange;

    private void Start() {
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
            m_Turrets[i].GetComponent<TurretFireManager>().SetTurretManager(this);
            // m_Turrets[i].GetComponent<TurretFireManager>().SetTurretNumber(i);
            if (m_Turrets[i].GetComponent<TurretFireManager>().GetTurretType() == TurretFireManager.TurretType.Artillery) {
                ArtilleryTurrets.Add(m_Turrets[i]);
            } else if (m_Turrets[i].GetComponent<TurretFireManager>().GetTurretType() == TurretFireManager.TurretType.ArtilleryAA) {
                ArtilleryTurrets.Add(m_Turrets[i]);
                AATurrets.Add(m_Turrets[i]);
            } else if (m_Turrets[i].GetComponent<TurretFireManager>().GetTurretType() == TurretFireManager.TurretType.AA) {
                AATurrets.Add(m_Turrets[i]);
            } else if (m_Turrets[i].GetComponent<TurretFireManager>().GetTurretType() == TurretFireManager.TurretType.Torpedo) {
                TorpedoTurrets.Add(m_Turrets[i]);
            } else if (m_Turrets[i].GetComponent<TurretFireManager>().GetTurretType() == TurretFireManager.TurretType.DepthCharge) {
                DepthChargeTurrets.Add(m_Turrets[i]);
            }
        }
        // ReinitializeCurrentWeaponSelected();
        // Debug.Log(ArtilleryTurrets.Count + " : ArtilleryTurrets");
        WorkingTurrets = TotalTurrets;
        if (GetComponent<ShipController>())
            ShipController.SetTotalTurrets(TotalTurrets);
        SetPlayerControl();
    }

    private void FixedUpdate() {
        if (PlayerControl) {
            CheckForTurretSwitch();

            // Get the angle of the camera here
            CameraPercentage = FreeLookCam.GetTiltPercentage();
            // Debug.Log(CameraPercentage + " : CameraPercentage");

            TargetRange = ((MaxRange - MinRange) / 100 * CameraPercentage) + MinRange;
            TargetPosition = FreeLookCam.GetTargetPosition();

            for (int i = 0; i < m_Turrets.Length; i++){
                m_Turrets[i].GetComponent<TurretFireManager>().SetTargetRange(TargetRange);
                m_Turrets[i].GetComponent<TurretRotation>().SetCameraPercentage(CameraPercentage);
                m_Turrets[i].GetComponent<TurretRotation>().SetTargetPosition(TargetPosition);
            }
            // StartCoroutine(PauseAction());
        }
        if (!PlayerControl && AIControl) {
            float fakeCameraPercentage = (Mathf.Round(AITargetRange * 100 / (MaxRange - MinRange)));
            // TurretStatus = "F - ";
            // Debug.Log("fakeCameraPercentage = "+ fakeCameraPercentage);

            for (int i = 0; i < m_Turrets.Length; i++){
                m_Turrets[i].GetComponent<TurretFireManager>().SetTargetRange(AITargetRange);
                m_Turrets[i].GetComponent<TurretRotation>().SetCameraPercentage(fakeCameraPercentage);
                m_Turrets[i].GetComponent<TurretRotation>().SetTargetPosition(AITargetPosition);
            }
            // StartCoroutine(PauseAction());
        }
        
        // Debug.Log("TargetRange = "+ TargetRange);
    }

    // IEnumerator PauseAction(){
    //     ActionPaused = true;
    //     yield return new WaitForSeconds(0.01f);
    //     ActionPaused = false;
    // }
    private void CheckForTurretSwitch() {
        if (Input.GetButtonDown ("SetWeaponArtillery") && CurrentControlledTurretType != TurretFireManager.TurretType.Artillery && ArtilleryTurrets.Count > 0){
            CurrentControlledTurretType = TurretFireManager.TurretType.Artillery;
            PlayerManager.SetPlayerUITurretType(CurrentControlledTurretType);
            SetPlayerControl();
        }
        if (Input.GetButtonDown ("SetWeaponAA") && CurrentControlledTurretType != TurretFireManager.TurretType.AA && AATurrets.Count > 0){
            CurrentControlledTurretType = TurretFireManager.TurretType.AA;
            PlayerManager.SetPlayerUITurretType(CurrentControlledTurretType);
            SetPlayerControl();
        }
        if (Input.GetButtonDown ("SetWeaponTorpedoes") && CurrentControlledTurretType != TurretFireManager.TurretType.Torpedo && TorpedoTurrets.Count > 0){
            CurrentControlledTurretType = TurretFireManager.TurretType.Torpedo;
            PlayerManager.SetPlayerUITurretType(CurrentControlledTurretType);
            SetPlayerControl();
        }
    }
    private void ReinitializeCurrentWeaponSelected() {
        if (ArtilleryTurrets.Count > 0) {
            CurrentControlledTurretType = TurretFireManager.TurretType.Artillery;
            PlayerManager.SetPlayerUITurretType(CurrentControlledTurretType);
        } else if (AATurrets.Count > 0) {
            CurrentControlledTurretType = TurretFireManager.TurretType.AA;
            PlayerManager.SetPlayerUITurretType(CurrentControlledTurretType);
        } else if (TorpedoTurrets.Count > 0) {
            CurrentControlledTurretType = TurretFireManager.TurretType.Torpedo;
            PlayerManager.SetPlayerUITurretType(CurrentControlledTurretType);
        } else if (DepthChargeTurrets.Count > 0) {
            CurrentControlledTurretType = TurretFireManager.TurretType.DepthCharge;
            PlayerManager.SetPlayerUITurretType(CurrentControlledTurretType);
        }
        // Debug.Log ("CurrentControlledTurretType : "+ CurrentControlledTurretType);
    }
    // private void CreatePlayerTurretsgroup(){
    //     for (int i = 0; i < m_Turrets.Length; i++){
    //         if (m_Turrets[i].GetComponent<TurretFireManager>().GetTurretType() == CurrentControlledTurretType){
    //             TurretStatus.Add(m_Turrets[i].GetComponent<TurretFireManager>().GetTurretStatus());
    //         }
    //     }     
    // }

    private void SetPlayerControl(){
        if (Active && !Map && !DamageControl && !Dead && !FreeCamera && !Pause) {
            PlayerControl = true;
            AIControl = false;
        } else {
            PlayerControl = false;
            if (AIHasATarget) {
                AIControl = true;
            } else {
                AIControl = false;
            }
        }
        int number = 0;
        for (int i = 0; i < m_Turrets.Length; i++) {
            if ((CurrentControlledTurretType == TurretFireManager.TurretType.Artillery || CurrentControlledTurretType == TurretFireManager.TurretType.AA) && m_Turrets[i].GetComponent<TurretFireManager>().GetTurretType() == TurretFireManager.TurretType.ArtilleryAA) {
                m_Turrets[i].GetComponent<TurretRotation>().SetPlayerControl(PlayerControl);
                m_Turrets[i].GetComponent<TurretFireManager>().SetPlayerControl(PlayerControl);
                m_Turrets[i].GetComponent<TurretFireManager>().SetTurretUIActive(true);
                m_Turrets[i].GetComponent<TurretFireManager>().SetTurretNumber(number);
                number++;
            } else if (m_Turrets[i].GetComponent<TurretFireManager>().GetTurretType() == CurrentControlledTurretType) {
                m_Turrets[i].GetComponent<TurretRotation>().SetPlayerControl(PlayerControl);
                m_Turrets[i].GetComponent<TurretFireManager>().SetPlayerControl(PlayerControl);
                m_Turrets[i].GetComponent<TurretFireManager>().SetTurretUIActive(true);
                m_Turrets[i].GetComponent<TurretFireManager>().SetTurretNumber(number);
                number++;
            } else {
                m_Turrets[i].GetComponent<TurretRotation>().SetPlayerControl(false);
                m_Turrets[i].GetComponent<TurretFireManager>().SetPlayerControl(false);
                m_Turrets[i].GetComponent<TurretFireManager>().SetTurretUIActive(false);
            }
            m_Turrets[i].GetComponent<TurretRotation>().SetAIControl(AIControl);
            m_Turrets[i].GetComponent<TurretFireManager>().SetAIControl(AIControl);
        }
    }

    public void SetActive(bool activate) {
        Active = activate;
        for (int i = 0; i < m_Turrets.Length; i++){
            m_Turrets[i].GetComponent<TurretFireManager>().SetActive(activate);
        }
        SetPlayerControl();
        if (Active)
            ReinitializeCurrentWeaponSelected();
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
    public void SetPlayerManager(PlayerManager playerManager){
        PlayerManager = playerManager;
        FreeLookCam = PlayerManager.GetFreeLookCam();
    }
    public void SetFreeCamera(bool freeCamera) {
        FreeCamera = freeCamera;
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

    public void SetSingleTurretStatus(TurretStatusType status, int turretNumber){
        if (Active && GetComponent<ShipController>())
                ShipController.SetSingleTurretStatus(status, turretNumber);
    }

    public GameObject[] GetTurrets() {
        return m_Turrets;
    }
    public TurretFireManager.TurretType GetCurrentTurretType() {
        return CurrentControlledTurretType;
    }
    
    public List <TurretStatusType> GetTurretsStatus() {
        TurretStatus.Clear();
        for (int i = 0; i < m_Turrets.Length; i++) {
            if ((CurrentControlledTurretType == TurretFireManager.TurretType.Artillery || CurrentControlledTurretType == TurretFireManager.TurretType.AA) && m_Turrets[i].GetComponent<TurretFireManager>().GetTurretType() == TurretFireManager.TurretType.ArtilleryAA) {
                TurretStatus.Add(m_Turrets[i].GetComponent<TurretFireManager>().GetTurretStatus());
            } else if (m_Turrets[i].GetComponent<TurretFireManager>().GetTurretType() == CurrentControlledTurretType) {
                TurretStatus.Add(m_Turrets[i].GetComponent<TurretFireManager>().GetTurretStatus());
            }
        }
        // Debug.Log ("TurretStatus : "+ TurretStatus);
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
    public void SetAITargetRange(float targetRange) { AITargetRange = targetRange; TargetRange = targetRange;}
    public void SetAIHasTarget(bool hasTarget) { AIHasATarget = hasTarget; SetPlayerControl(); }
}