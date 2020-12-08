using System;
using UnityEngine;
using FreeLookCamera;
using System.Collections;
using System.Collections.Generic;

public class TurretManager : MonoBehaviour {
    private bool Active = false;
    private bool Dead = false;
    private bool Pause = false;
    private bool Map = false; public void SetMap(bool map) { Map = map; SetPlayerControl(); }
    private bool DamageControl = false; public void SetDamageControl(bool damageControl) { DamageControl = damageControl; SetPlayerControl(); }
    private bool FreeCamera = false; public void SetFreeCamera(bool freeCamera) { FreeCamera = freeCamera; SetPlayerControl(); }
    private bool PlayerControl = false;
    private PlayerManager PlayerManager; public void SetPlayerManager(PlayerManager playerManager){ PlayerManager = playerManager; FreeLookCam = PlayerManager.GetFreeLookCam(); }
    private FreeLookCam FreeLookCam;
    private UnitMasterController UnitMasterController;
    private CompiledTypes.Weapons_roles.RowValues CurrentControlledWeaponRole; public CompiledTypes.Weapons_roles.RowValues GetCurrentTurretRole() { return CurrentControlledWeaponRole; }
    private List<CompiledTypes.Weapons_roles.RowValues> UnitWeaponRoleList = new List<CompiledTypes.Weapons_roles.RowValues>(); public List<CompiledTypes.Weapons_roles.RowValues> GetUnitWeaponRoleList(){ return UnitWeaponRoleList; }

    [Header("Artillery")]
        private float ElevationRatio;               // % of elevation needed
        private float TargetRange; public float GetTargetRange() { return TargetRange; } 
        private Vector3 TargetPosition;
        private float MaxRange = -1;
        private float MinRange = 100000;

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
    private bool AIHasATarget = false; public void SetAIHasTarget(bool hasTarget) { AIHasATarget = hasTarget; SetPlayerControl(); }
    private Vector3 AITargetPosition;
    private float AITargetRange; public void SetAITargetRange(float targetRange) { AITargetRange = targetRange; TargetRange = targetRange; }
    

    private List <TurretListItem> TurretList = new List<TurretListItem>();
    public class TurretListItem {
        private GameObject _weaponGameObject;  public GameObject GetWeaponGameObject(){ return _weaponGameObject; } public void SetWeaponGameObject(GameObject _g){ _weaponGameObject = _g; }
        private TurretRotation _weaponTurretRotation;  public TurretRotation GetTurretRotation(){ return _weaponTurretRotation; } public void SetTurretRotation(TurretRotation _s){ _weaponTurretRotation = _s; }
        private TurretFireManager _weaponTurretFireManager;  public TurretFireManager GetTurretFireManager(){ return _weaponTurretFireManager; } public void SetTurretFireManager(TurretFireManager _s){ _weaponTurretFireManager = _s; }
        private TurretHealth _weaponTurretHealth;  public TurretHealth GetTurretHealth(){ return _weaponTurretHealth; } public void SetTurretHealth(TurretHealth _s){ _weaponTurretHealth = _s; }

        // private int _weaponCount;  public int GetWeaponCount(){ return _weaponCount; } public void AddWeaponCount(){ _weaponCount += 1; }public void SetWeaponCount(int _i){ _weaponCount = _i; }
    }

    public void AddNewWeapon(GameObject weapon) {
        TurretListItem _turret = new TurretListItem{};
        _turret.SetWeaponGameObject(weapon);
        _turret.SetTurretRotation(weapon.GetComponent<TurretRotation>());
        _turret.SetTurretFireManager(weapon.GetComponent<TurretFireManager>());
        _turret.SetTurretHealth(weapon.GetComponent<TurretHealth>());
        TurretList.Add(_turret);
    }

    public void BeginOperations(UnitMasterController unitController){
        UnitMasterController = unitController;
        float MaxR;
        float MinR;
        foreach (TurretListItem turret in TurretList) {
            TotalTurrets++;
            MaxR = turret.GetTurretFireManager().GetMaxRange();
            MinR = turret.GetTurretFireManager().GetMinRange();
            if (MaxR > MaxRange)
                MaxRange = MaxR;
            if (MinR < MinRange)
                MinRange = MinR;
            turret.GetTurretFireManager().SetTurretManager(this);
            
            foreach (CompiledTypes.Weapons_roles.RowValues role in turret.GetTurretFireManager().GetWeaponRoles()) {
                if (!UnitWeaponRoleList.Contains(role)) {
                    UnitWeaponRoleList.Add(role);
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
            if (CurrentControlledWeaponRole == CompiledTypes.Weapons_roles.RowValues.NavalArtillery) {
                ElevationRatio = FreeLookCam.GetTiltPercentage();                                               // Get the angle of the camera here
                TargetRange = ((MaxRange - MinRange) / 100 * ElevationRatio) + MinRange;
                TargetPosition = FreeLookCam.GetRaycastAbstractTargetPosition();                                // Long range raycast
            } else {
                TargetRange = FreeLookCam.GetRaycastRange();                                                    // Get the distance with the raycast point
                TargetPosition = FreeLookCam.GetRaycastTargetPosition();                                        // Short range raycast
                ElevationRatio = Mathf.Round((TargetRange - MinRange) * 100 / (MaxRange - MinRange));
            }
            // Debug.Log(ElevationRatio + " : ElevationRatio");

            foreach (TurretListItem turret in TurretList) {
                turret.GetTurretFireManager().SetTargetRange(TargetRange);
                turret.GetTurretFireManager().SetTargetPosition(TargetPosition);
                turret.GetTurretRotation().SetTargetPosition(TargetPosition);
                turret.GetTurretRotation().SetElevationRatio(ElevationRatio);
            }
        }
        if (!PlayerControl && AIControl) {
            // Debug.Log("if (!PlayerControl && AIControl) {");
            ElevationRatio = Mathf.Round((AITargetRange - MinRange) * 100 / (MaxRange - MinRange));
            // ElevationRatio = (Mathf.Round(AITargetRange * 100 / (MaxRange - MinRange)));
            TargetRange = AITargetRange;                                                        // So the UI Knows the current AI range ?
            // Debug.Log("ElevationRatio = "+ ElevationRatio);
            foreach (TurretListItem turret in TurretList) {
                turret.GetTurretFireManager().SetTargetRange(AITargetRange);
                turret.GetTurretFireManager().SetTargetPosition(AITargetPosition);
                turret.GetTurretRotation().SetElevationRatio(ElevationRatio);
                turret.GetTurretRotation().SetTargetPosition(AITargetPosition);
            }
        }
        
        // Debug.Log("TargetRange = "+ TargetRange);
    }

    private void CheckForTurretSwitch() {
        if (Input.GetButtonDown ("SetWeaponNavalArtillery") && CurrentControlledWeaponRole != CompiledTypes.Weapons_roles.RowValues.NavalArtillery && UnitWeaponRoleList.Contains(CompiledTypes.Weapons_roles.RowValues.NavalArtillery)){
            CurrentControlledWeaponRole = CompiledTypes.Weapons_roles.RowValues.NavalArtillery;
            PlayerManager.SetPlayerTurretRole(CurrentControlledWeaponRole);
            SetPlayerControl();
            // Debug.Log ("CurrentControlledWeaponRole : "+ CurrentControlledWeaponRole);
        }
        if (Input.GetButtonDown ("SetWeaponArtillery") && CurrentControlledWeaponRole != CompiledTypes.Weapons_roles.RowValues.Artillery && UnitWeaponRoleList.Contains(CompiledTypes.Weapons_roles.RowValues.Artillery)){
            CurrentControlledWeaponRole = CompiledTypes.Weapons_roles.RowValues.Artillery;
            PlayerManager.SetPlayerTurretRole(CurrentControlledWeaponRole);
            SetPlayerControl();
            // Debug.Log ("CurrentControlledWeaponRole : "+ CurrentControlledWeaponRole);
        }
        if (Input.GetButtonDown ("SetWeaponAA") && CurrentControlledWeaponRole != CompiledTypes.Weapons_roles.RowValues.AntiAir && UnitWeaponRoleList.Contains(CompiledTypes.Weapons_roles.RowValues.AntiAir)){
            CurrentControlledWeaponRole = CompiledTypes.Weapons_roles.RowValues.AntiAir;
            PlayerManager.SetPlayerTurretRole(CurrentControlledWeaponRole);
            SetPlayerControl();
            // Debug.Log ("CurrentControlledWeaponRole : "+ CurrentControlledWeaponRole);
        }
        if (Input.GetButtonDown ("SetWeaponTorpedoes") && CurrentControlledWeaponRole != CompiledTypes.Weapons_roles.RowValues.Torpedo && UnitWeaponRoleList.Contains(CompiledTypes.Weapons_roles.RowValues.Torpedo)){
            CurrentControlledWeaponRole = CompiledTypes.Weapons_roles.RowValues.Torpedo;
            PlayerManager.SetPlayerTurretRole(CurrentControlledWeaponRole);
            SetPlayerControl();
        }
    }
    private void ReinitializeCurrentWeaponSelected() {
        foreach (CompiledTypes.Weapons_roles.RowValues role in UnitWeaponRoleList) {
            // When a unit is put active, one role is selected.
            // The first role is always selected. The order of the roles is set in DB by the order of UnitHardPoint in Global_Units AND in WeaponRoles in Weapons.
            // In the DB The first role in a weapon will be prioritized. The first weapon of a unit will be prioritized. Watch as these orders have consequences on gameplay.
            CurrentControlledWeaponRole = role;
            PlayerManager.SetPlayerTurretRole(CurrentControlledWeaponRole);
            break;
        }
        // Debug.Log ("CurrentControlledWeaponRole : "+ CurrentControlledWeaponRole);
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
        foreach (TurretListItem turret in TurretList) {
            List<CompiledTypes.Weapons_roles.RowValues> availableWeaponRoles = turret.GetTurretFireManager().GetWeaponRoles();
            bool matchFound = false;
            foreach (CompiledTypes.Weapons_roles.RowValues role in availableWeaponRoles) {
                if (role == CurrentControlledWeaponRole) {
                    matchFound = true;                              // Try to find a single match for what we look for.
                    // return;                                      // No return here or it stops the TurretList loop (why ?)
                }
            }

            if (matchFound) {                               // If this was in the availableRoles loop, it would be ignored by further roles. Better do it once, it's cleaner.
                // turret.GetComponent<TurretRotation>().SetCurrentControlledWeaponRole(CurrentControlledWeaponRole);
                turret.GetTurretRotation().SetPlayerControl(PlayerControl);
                turret.GetTurretFireManager().SetWeaponCurrentRole(CurrentControlledWeaponRole);
                turret.GetTurretFireManager().SetPlayerControl(PlayerControl);
                turret.GetTurretFireManager().SetTurretUIActive(true);
                turret.GetTurretFireManager().SetTurretNumber(number);
                number++;
            } else {
                turret.GetTurretRotation().SetPlayerControl(false);
                turret.GetTurretFireManager().SetPlayerControl(false);
                turret.GetTurretFireManager().SetTurretUIActive(false);
            }
            turret.GetTurretRotation().SetAIControl(AIControl);
            turret.GetTurretFireManager().SetAIControl(AIControl);
        }
    }

    public void SetActive(bool activate) {
        Active = activate;
        foreach (TurretListItem turret in TurretList) {
            turret.GetTurretFireManager().SetActive(activate);
        }
        SetPlayerControl();
        if (Active)
            ReinitializeCurrentWeaponSelected();
    }
    public void SetPause() {
        Pause = !Pause;
        SetPlayerControl();
        foreach (TurretListItem turret in TurretList) {
            turret.GetTurretRotation().SetPause(Pause);
        }
    }
    public void SetRepairRate(float Rate) {
        foreach (TurretListItem turret in TurretList) {
            turret.GetTurretHealth().SetRepairRate(Rate);
        }
    }
    public void SetTurretRepairRate(float Rate) {
        foreach (TurretListItem turret in TurretList) {
            turret.GetTurretHealth().SetTurretRepairRate(Rate);
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
        foreach (TurretListItem turret in TurretList) {
            turret.GetTurretHealth().SetShipDeath(IsShipDead);
            turret.GetTurretRotation().SetTurretDeath(IsShipDead);
            turret.GetTurretFireManager().SetTurretDeath(IsShipDead);
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
        if (TurretList.Count > 0) {
            return false;
        } else {
            return true;
        }
    }
    public void FeedbackShellHit(bool armorPenetrated) {
        UnitMasterController.FeedbackShellHit(armorPenetrated);
    }
    
    public List <TurretStatusType> GetTurretsStatus() {
        TurretStatus.Clear();
        foreach (TurretListItem turret in TurretList) {
            List<CompiledTypes.Weapons_roles.RowValues> availableWeaponRoles = turret.GetTurretFireManager().GetWeaponRoles();                                                                 
            foreach (CompiledTypes.Weapons_roles.RowValues role in availableWeaponRoles) {
                if (role == CurrentControlledWeaponRole) {
                    TurretStatus.Add(turret.GetTurretFireManager().GetTurretStatus());
                    break;  // If turret found once don't add it twice ! (in case of turret having twice the same available role in the DB)
                }
            }
        }
        // Debug.Log ("TurretStatus : "+ TurretStatus);
        return TurretStatus;
    }

    public void SetAITargetToFireOn(Vector3 targetPosition) {
        AITargetPosition = targetPosition;
        // if (!Active) {
        //     Debug.Log ("AITargetPosition : "+ AITargetPosition);
        // }
    }
}