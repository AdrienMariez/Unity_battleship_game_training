using System;
using UnityEngine;
using FreeLookCamera;
using System.Collections;
using System.Collections.Generic;

public class PlaneWeaponsManager : MonoBehaviour {
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
    private List <TurretManager.TurretStatusType> TurretStatus = new List<TurretManager.TurretStatusType>();

    private bool AIControl = false;
    private bool AIHasATarget = false; public void SetAIHasTarget(bool hasTarget) { AIHasATarget = hasTarget; SetPlayerControl(); }
    private Vector3 AITargetPosition;
    private float AITargetRange; public void SetAITargetRange(float targetRange) { AITargetRange = targetRange; TargetRange = targetRange; }
    

    private List <WeaponsListItem> TurretList = new List<WeaponsListItem>();
    public class WeaponsListItem {
        private GameObject _weaponGameObject;  public GameObject GetWeaponGameObject(){ return _weaponGameObject; } public void SetWeaponGameObject(GameObject _g){ _weaponGameObject = _g; }
        private PlaneWeapon _planeWeapon;  public PlaneWeapon GetPlaneWeapon(){ return _planeWeapon; } public void SetPlaneWeapon(PlaneWeapon _s){ _planeWeapon = _s; }
    }

    public void AddNewWeapon(GameObject weapon) {
        WeaponsListItem _weapon = new WeaponsListItem{};
        _weapon.SetWeaponGameObject(weapon);
        _weapon.SetPlaneWeapon(weapon.GetComponent<PlaneWeapon>());
        TurretList.Add(_weapon);
    }

    public void BeginOperations(UnitMasterController unitController){
        UnitMasterController = unitController;
        float MaxR;
        float MinR;
        foreach (WeaponsListItem _weapon in TurretList) {
            TotalTurrets++;
            MaxR = _weapon.GetPlaneWeapon().GetMaxRange();
            MinR = _weapon.GetPlaneWeapon().GetMinRange();
            if (MaxR > MaxRange)
                MaxRange = MaxR;
            if (MinR < MinRange)
                MinRange = MinR;
            _weapon.GetPlaneWeapon().SetTurretManager(this);
            
            foreach (CompiledTypes.Weapons_roles.RowValues role in _weapon.GetPlaneWeapon().GetWeaponRoles()) {
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

            foreach (WeaponsListItem turret in TurretList) {
                turret.GetPlaneWeapon().SetTargetRange(TargetRange);
                turret.GetPlaneWeapon().SetTargetPosition(TargetPosition);
            }
        }
        if (!PlayerControl && AIControl) {
            // Debug.Log("if (!PlayerControl && AIControl) {");
            ElevationRatio = Mathf.Round((AITargetRange - MinRange) * 100 / (MaxRange - MinRange));
            // ElevationRatio = (Mathf.Round(AITargetRange * 100 / (MaxRange - MinRange)));
            TargetRange = AITargetRange;                                                        // So the UI Knows the current AI range ?
            // Debug.Log("ElevationRatio = "+ ElevationRatio);
            foreach (WeaponsListItem _weapon in TurretList) {
                _weapon.GetPlaneWeapon().SetTargetRange(AITargetRange);
                _weapon.GetPlaneWeapon().SetTargetPosition(AITargetPosition);
            }
        }
        
        // Debug.Log("TargetRange = "+ TargetRange);
    }

    private void CheckForTurretSwitch() {
        if (Input.GetButtonDown ("SetWeaponNavalArtillery") && CurrentControlledWeaponRole != CompiledTypes.Weapons_roles.RowValues.NavalArtillery && UnitWeaponRoleList.Contains(CompiledTypes.Weapons_roles.RowValues.NavalArtillery)){
            CurrentControlledWeaponRole = CompiledTypes.Weapons_roles.RowValues.NavalArtillery;
            // PlayerManager.SetPlayerTurretRole(CurrentControlledWeaponRole);
            SetPlayerControl();
            // Debug.Log ("CurrentControlledWeaponRole : "+ CurrentControlledWeaponRole);
        }
        if (Input.GetButtonDown ("SetWeaponArtillery") && CurrentControlledWeaponRole != CompiledTypes.Weapons_roles.RowValues.Artillery && UnitWeaponRoleList.Contains(CompiledTypes.Weapons_roles.RowValues.Artillery)){
            CurrentControlledWeaponRole = CompiledTypes.Weapons_roles.RowValues.Artillery;
            // PlayerManager.SetPlayerTurretRole(CurrentControlledWeaponRole);
            SetPlayerControl();
            // Debug.Log ("CurrentControlledWeaponRole : "+ CurrentControlledWeaponRole);
        }
        if (Input.GetButtonDown ("SetWeaponAA") && CurrentControlledWeaponRole != CompiledTypes.Weapons_roles.RowValues.AntiAir && UnitWeaponRoleList.Contains(CompiledTypes.Weapons_roles.RowValues.AntiAir)){
            CurrentControlledWeaponRole = CompiledTypes.Weapons_roles.RowValues.AntiAir;
            // PlayerManager.SetPlayerTurretRole(CurrentControlledWeaponRole);
            SetPlayerControl();
            // Debug.Log ("CurrentControlledWeaponRole : "+ CurrentControlledWeaponRole);
        }
        if (Input.GetButtonDown ("SetWeaponTorpedoes") && CurrentControlledWeaponRole != CompiledTypes.Weapons_roles.RowValues.Torpedo && UnitWeaponRoleList.Contains(CompiledTypes.Weapons_roles.RowValues.Torpedo)){
            CurrentControlledWeaponRole = CompiledTypes.Weapons_roles.RowValues.Torpedo;
            // PlayerManager.SetPlayerTurretRole(CurrentControlledWeaponRole);
            SetPlayerControl();
        }
    }
    private void ReinitializeCurrentWeaponSelected() {
        foreach (CompiledTypes.Weapons_roles.RowValues role in UnitWeaponRoleList) {
            // When a unit is put active, one role is selected.
            // The first role is always selected. The order of the roles is set in DB by the order of UnitHardPoint in Global_Units AND in WeaponRoles in Weapons.
            // In the DB The first role in a weapon will be prioritized. The first weapon of a unit will be prioritized. Watch as these orders have consequences on gameplay.
            CurrentControlledWeaponRole = role;
            // PlayerManager.SetPlayerTurretRole(CurrentControlledWeaponRole);
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
        // Debug.Log(Active+"-"+!Map+"-"+!DamageControl+"-"+!Dead+"-"+!FreeCamera+"-"+!Pause);
        int number = 0;
        foreach (WeaponsListItem _weapon in TurretList) {
            List<CompiledTypes.Weapons_roles.RowValues> availableWeaponRoles = _weapon.GetPlaneWeapon().GetWeaponRoles();
            bool matchFound = false;
            foreach (CompiledTypes.Weapons_roles.RowValues role in availableWeaponRoles) {
                if (role == CurrentControlledWeaponRole) {
                    matchFound = true;                              // Try to find a single match for what we look for.
                    // return;                                      // No return here or it stops the TurretList loop (why ?)
                }
            }

            if (matchFound) {                               // If this was in the availableRoles loop, it would be ignored by further roles. Better do it once, it's cleaner.
                _weapon.GetPlaneWeapon().SetWeaponCurrentRole(CurrentControlledWeaponRole);
                _weapon.GetPlaneWeapon().SetPlayerControl(PlayerControl);
                _weapon.GetPlaneWeapon().SetTurretUIActive(true);
                _weapon.GetPlaneWeapon().SetTurretNumber(number);
                number++;
            } else {
                _weapon.GetPlaneWeapon().SetPlayerControl(false);
                _weapon.GetPlaneWeapon().SetTurretUIActive(false);
            }
            _weapon.GetPlaneWeapon().SetAIControl(AIControl);
        }
    }

    public void SetActive(bool activate) {
        Active = activate;
        foreach (WeaponsListItem _weapon in TurretList) {
            _weapon.GetPlaneWeapon().SetActive(activate);
        }
        SetPlayerControl();
        if (Active)
            ReinitializeCurrentWeaponSelected();
    }
    public void SetPause() {
        Pause = !Pause;
        SetPlayerControl();
    }
    public void SetRepairRate(float Rate) {

    }
    public void SetTurretRepairRate(float Rate) {

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
        foreach (WeaponsListItem turret in TurretList) {
            turret.GetPlaneWeapon().SetTurretDeath(IsShipDead);
        }
    }

    public void SetSingleTurretStatus(TurretManager.TurretStatusType status, int turretNumber){
        if (Active)
            if (!PlayerControl && !AIControl & status == TurretManager.TurretStatusType.Ready) {     // If no one is in control of the turrets, keep them red
                UnitMasterController.SetSingleTurretStatus(TurretManager.TurretStatusType.PreventFire, turretNumber);
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
    
    public List <TurretManager.TurretStatusType> GetTurretsStatus() {
        TurretStatus.Clear();
        foreach (WeaponsListItem turret in TurretList) {
            List<CompiledTypes.Weapons_roles.RowValues> availableWeaponRoles = turret.GetPlaneWeapon().GetWeaponRoles();                                                                 
            foreach (CompiledTypes.Weapons_roles.RowValues role in availableWeaponRoles) {
                if (role == CurrentControlledWeaponRole) {
                    TurretStatus.Add(turret.GetPlaneWeapon().GetTurretStatus());
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