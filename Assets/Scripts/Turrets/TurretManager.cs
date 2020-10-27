using System;
using UnityEngine;
using FreeLookCamera;
using System.Collections;
using System.Collections.Generic;

public class TurretManager : MonoBehaviour {
    public GameObject[] m_Turrets;
    private List <GameObject> AllTurrets = new List<GameObject>();
    private bool Active = false;
    private bool Dead = false;
    private bool Pause = false;
    private bool Map = false;
    private bool DamageControl = false;
    private bool FreeCamera = false;
    private bool PlayerControl = false;
    private PlayerManager PlayerManager;
    private FreeLookCam FreeLookCam;
    private UnitMasterController UnitMasterController;
    private TurretFireManager.TurretRole CurrentControlledTurretRole;
    [Header("Artillery")]
        // private List <GameObject> ArtilleryTurrets;
        private List <GameObject> NavalArtilleryTurrets = new List<GameObject>();
        private List <GameObject> ArtilleryTurrets = new List<GameObject>();
        private float ElevationRatio;               // % of elevation needed
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

    public void AddNewWeapon(GameObject weapon) {
        AllTurrets.Add(weapon);
    }

    public void BeginOperations(UnitMasterController unitController){
        UnitMasterController = unitController;
        float MaxR;
        float MinR;
        foreach (GameObject turret in AllTurrets) {
            TotalTurrets++;
            MaxR = turret.GetComponent<TurretFireManager>().GetMaxRange();
            MinR = turret.GetComponent<TurretFireManager>().GetMinRange();
            if (MaxR > MaxRange)
                MaxRange = MaxR;
            if (MinR < MinRange)
                MinRange = MinR;
            turret.GetComponent<TurretFireManager>().SetTurretManager(this);
            turret.GetComponent<TurretRotation>().SetTurretFireManager(turret.GetComponent<TurretFireManager>());
            TurretFireManager.TurretRole[] availableRoles = turret.GetComponent<TurretFireManager>().GetTurretRoles(); 
            
            foreach (TurretFireManager.TurretRole role in availableRoles) {
                if (role == TurretFireManager.TurretRole.NavalArtillery) {
                    NavalArtilleryTurrets.Add(turret);
                } else if (role == TurretFireManager.TurretRole.Artillery) {
                    ArtilleryTurrets.Add(turret);
                } else if (role == TurretFireManager.TurretRole.AA) {
                    AATurrets.Add(turret);
                } else if (role == TurretFireManager.TurretRole.Torpedo) {
                    TorpedoTurrets.Add(turret);
                } else if (role == TurretFireManager.TurretRole.DepthCharge) {
                    DepthChargeTurrets.Add(turret);
                }
            }
        }
        // ReinitializeCurrentWeaponSelected();
        // Debug.Log(ArtilleryTurrets.Count + " : ArtilleryTurrets");
        WorkingTurrets = TotalTurrets;

        UnitMasterController.SetTotalTurrets(TotalTurrets);
        UnitMasterController.SetMaxTurretRange(MaxRange);

        SetPlayerControl();
    }

    private void FixedUpdate() {
        if (PlayerControl) {
            // Debug.Log("if (PlayerControl) {");
            CheckForTurretSwitch();
            if (CurrentControlledTurretRole == TurretFireManager.TurretRole.NavalArtillery) {
                ElevationRatio = FreeLookCam.GetTiltPercentage();                                               // Get the angle of the camera here
                TargetRange = ((MaxRange - MinRange) / 100 * ElevationRatio) + MinRange;
                TargetPosition = FreeLookCam.GetRaycastAbstractTargetPosition();                                // Long range raycast
            } else {
                TargetRange = FreeLookCam.GetRaycastRange();                                                    // Get the distance with the raycast point
                TargetPosition = FreeLookCam.GetRaycastTargetPosition();                                        // Short range raycast
                ElevationRatio = Mathf.Round((TargetRange - MinRange) * 100 / (MaxRange - MinRange));
            }
            // Debug.Log(ElevationRatio + " : ElevationRatio");

            foreach (GameObject turret in AllTurrets) {
                turret.GetComponent<TurretFireManager>().SetTargetRange(TargetRange);
                turret.GetComponent<TurretFireManager>().SetTargetPosition(TargetPosition);
                turret.GetComponent<TurretRotation>().SetTargetPosition(TargetPosition);
                turret.GetComponent<TurretRotation>().SetElevationRatio(ElevationRatio);
            }
        }
        if (!PlayerControl && AIControl) {
            // Debug.Log("if (!PlayerControl && AIControl) {");
            ElevationRatio = Mathf.Round((AITargetRange - MinRange) * 100 / (MaxRange - MinRange));
            // ElevationRatio = (Mathf.Round(AITargetRange * 100 / (MaxRange - MinRange)));
            TargetRange = AITargetRange;                                                        // So the UI Knows the current AI range ?
            // Debug.Log("ElevationRatio = "+ ElevationRatio);

            foreach (GameObject turret in AllTurrets) {
                turret.GetComponent<TurretFireManager>().SetTargetRange(AITargetRange);
                turret.GetComponent<TurretFireManager>().SetTargetPosition(AITargetPosition);
                turret.GetComponent<TurretRotation>().SetElevationRatio(ElevationRatio);
                turret.GetComponent<TurretRotation>().SetTargetPosition(AITargetPosition);
            }
        }
        
        // Debug.Log("TargetRange = "+ TargetRange);
    }

    private void CheckForTurretSwitch() {
        if (Input.GetButtonDown ("SetWeaponNavalArtillery") && CurrentControlledTurretRole != TurretFireManager.TurretRole.NavalArtillery && NavalArtilleryTurrets.Count > 0){
            CurrentControlledTurretRole = TurretFireManager.TurretRole.NavalArtillery;
            PlayerManager.SetPlayerTurretRole(CurrentControlledTurretRole);
            SetPlayerControl();
            // Debug.Log ("CurrentControlledTurretRole : "+ CurrentControlledTurretRole);
        }
        if (Input.GetButtonDown ("SetWeaponArtillery") && CurrentControlledTurretRole != TurretFireManager.TurretRole.Artillery && ArtilleryTurrets.Count > 0){
            CurrentControlledTurretRole = TurretFireManager.TurretRole.Artillery;
            PlayerManager.SetPlayerTurretRole(CurrentControlledTurretRole);
            SetPlayerControl();
            // Debug.Log ("CurrentControlledTurretRole : "+ CurrentControlledTurretRole);
        }
        if (Input.GetButtonDown ("SetWeaponAA") && CurrentControlledTurretRole != TurretFireManager.TurretRole.AA && AATurrets.Count > 0){
            CurrentControlledTurretRole = TurretFireManager.TurretRole.AA;
            PlayerManager.SetPlayerTurretRole(CurrentControlledTurretRole);
            SetPlayerControl();
            // Debug.Log ("CurrentControlledTurretRole : "+ CurrentControlledTurretRole);
        }
        if (Input.GetButtonDown ("SetWeaponTorpedoes") && CurrentControlledTurretRole != TurretFireManager.TurretRole.Torpedo && TorpedoTurrets.Count > 0){
            CurrentControlledTurretRole = TurretFireManager.TurretRole.Torpedo;
            PlayerManager.SetPlayerTurretRole(CurrentControlledTurretRole);
            SetPlayerControl();
        }
    }
    private void ReinitializeCurrentWeaponSelected() {
        if (NavalArtilleryTurrets.Count > 0) {
            CurrentControlledTurretRole = TurretFireManager.TurretRole.NavalArtillery;
            PlayerManager.SetPlayerTurretRole(CurrentControlledTurretRole);
        } else if (ArtilleryTurrets.Count > 0) {
            CurrentControlledTurretRole = TurretFireManager.TurretRole.Artillery;
            PlayerManager.SetPlayerTurretRole(CurrentControlledTurretRole);
        } else if (AATurrets.Count > 0) {
            CurrentControlledTurretRole = TurretFireManager.TurretRole.AA;
            PlayerManager.SetPlayerTurretRole(CurrentControlledTurretRole);
        } else if (TorpedoTurrets.Count > 0) {
            CurrentControlledTurretRole = TurretFireManager.TurretRole.Torpedo;
            PlayerManager.SetPlayerTurretRole(CurrentControlledTurretRole);
        } else if (DepthChargeTurrets.Count > 0) {
            CurrentControlledTurretRole = TurretFireManager.TurretRole.DepthCharge;
            PlayerManager.SetPlayerTurretRole(CurrentControlledTurretRole);
        } else {
            PlayerManager.SetPlayerTurretRole(TurretFireManager.TurretRole.None);
        }
        // Debug.Log ("CurrentControlledTurretRole : "+ CurrentControlledTurretRole);
    }


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
        // Debug.Log("PlayerControl : "+ PlayerControl);
        // Debug.Log("AIControl : "+ AIControl);
        int number = 0;
        foreach (GameObject turret in AllTurrets){
            TurretFireManager.TurretRole[] availableRoles = turret.GetComponent<TurretFireManager>().GetTurretRoles();
            bool matchFound = false;
            foreach (TurretFireManager.TurretRole role in availableRoles) {
                if (role == CurrentControlledTurretRole) {
                    matchFound = true;                              // Try to find a single match for what we look for.
                    // return;                                      // No return here or it stops the AllTurrets loop (why ?)
                }
            }

            if (matchFound) {                               // If this was in the availableRoles loop, it would be ignored by further roles. Better do it once, it's cleaner.
                turret.GetComponent<TurretRotation>().SetCurrentControlledTurretRole(CurrentControlledTurretRole);
                turret.GetComponent<TurretRotation>().SetPlayerControl(PlayerControl);
                turret.GetComponent<TurretFireManager>().SetCurrentControlledTurretRole(CurrentControlledTurretRole);
                turret.GetComponent<TurretFireManager>().SetPlayerControl(PlayerControl);
                turret.GetComponent<TurretFireManager>().SetTurretUIActive(true);
                turret.GetComponent<TurretFireManager>().SetTurretNumber(number);
                number++;
            } else {
                turret.GetComponent<TurretRotation>().SetPlayerControl(false);
                turret.GetComponent<TurretFireManager>().SetPlayerControl(false);
                turret.GetComponent<TurretFireManager>().SetTurretUIActive(false);
            }
            turret.GetComponent<TurretRotation>().SetAIControl(AIControl);
            turret.GetComponent<TurretFireManager>().SetAIControl(AIControl);
        }
    }

    public void SetActive(bool activate) {
        Active = activate;
        foreach (GameObject turret in AllTurrets) {
            turret.GetComponent<TurretFireManager>().SetActive(activate);
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
        foreach (GameObject turret in AllTurrets) {
            turret.GetComponent<TurretRotation>().SetPause(Pause);
        }
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
        foreach (GameObject turret in AllTurrets) {
            turret.GetComponent<TurretHealth>().SetRepairRate(Rate);
        }
    }
    public void SetTurretRepairRate(float Rate) {
        foreach (GameObject turret in AllTurrets) {
            turret.GetComponent<TurretHealth>().SetTurretRepairRate(Rate);
        }
    }
    public void SetSingleTurretDeath(bool isTurretDead){
        if (isTurretDead) {
            WorkingTurrets--;
        } else{
            WorkingTurrets++;
        }
        UnitMasterController.SetDamagedTurrets(TotalTurrets - WorkingTurrets);
    }
    public void SetDeath(bool IsShipDead) {
        Dead = IsShipDead;
        foreach (GameObject turret in AllTurrets) {
            turret.GetComponent<TurretHealth>().SetShipDeath(IsShipDead);
            turret.GetComponent<TurretRotation>().SetTurretDeath(IsShipDead);
            turret.GetComponent<TurretFireManager>().SetTurretDeath(IsShipDead);
        }
    }

    public void SetSingleTurretStatus(TurretStatusType status, int turretNumber){
        if (Active)
            if (!PlayerControl && !AIControl & status == TurretStatusType.Ready) {     // If no one is in control of the turrets, keep them red
                UnitMasterController.SetSingleTurretStatus(TurretStatusType.PreventFire, turretNumber);
            } else {
                UnitMasterController.SetSingleTurretStatus(status, turretNumber);
            }
    }
    public void SendPlayerShellToUI(GameObject shellInstance){
        if (Active)
            UnitMasterController.SendPlayerShellToUI(shellInstance);
    }

    public bool GetIsEmpty() {
        if (AllTurrets.Count > 0) {
            return false;
        } else {
            return true;
        }
    }
    
    public TurretFireManager.TurretRole GetCurrentTurretRole() { return CurrentControlledTurretRole; }
    public void FeedbackShellHit(bool armorPenetrated) {
        UnitMasterController.FeedbackShellHit(armorPenetrated);
    }
    
    public List <TurretStatusType> GetTurretsStatus() {
        TurretStatus.Clear();
        foreach (GameObject turret in AllTurrets) {
            TurretFireManager.TurretRole[] availableRoles = turret.GetComponent<TurretFireManager>().GetTurretRoles();
            bool canLoop = true;                                                                   // If turret found once don't add it twice ! (in case of turret having twice the same available role in the DB)
            foreach (TurretFireManager.TurretRole role in availableRoles) {
                if (role == CurrentControlledTurretRole && canLoop) {
                    TurretStatus.Add(turret.GetComponent<TurretFireManager>().GetTurretStatus());
                    canLoop = false;
                }
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
        // if (!Active) {
        //     Debug.Log ("AITargetPosition : "+ AITargetPosition);
        // }
    }
    public void SetAITargetRange(float targetRange) {
        AITargetRange = targetRange;
        TargetRange = targetRange;
        // Debug.Log ("AITargetRange : "+ AITargetRange);
    }
    public void SetAIHasTarget(bool hasTarget) { AIHasATarget = hasTarget; SetPlayerControl(); }

    [Serializable] public class singleTurret {      // All variable elements for each turret
        public GameObject m_TurretModel;
        [Tooltip("When true, turret rotates according to left/right traverse limits. When false, turret can rotate freely.")]
            public bool m_LimitTraverse = false;
        [Tooltip("When traverse is limited, how many degrees to the left the turret can turn.")]
            [Range(0.0f, 180.0f)]
            public float m_LeftTraverse = 60.0f;
            private float LocalLeftTraverse;
        [Tooltip("When traverse is limited, how many degrees to the right the turret can turn.")]
            [Range(0.0f, 180.0f)]
            public float m_RightTraverse = 60.0f; 
            private float LocalRightTraverse;
        public FireZonesManager[] m_NoFireZones;
        [Header("Vertical elevation")]
            public ElevationZonesManager[] m_ElevationZones;
    }
}