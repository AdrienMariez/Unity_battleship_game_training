using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class HardPointComponent : MonoBehaviour {

    [Header("Turrets parameters")]
    [Header("Turrets horizontal rotation limitations")]

        [Tooltip("When true, turret rotates according to left/right traverse limits. When false, turret can rotate freely.")]
            public bool m_LimitTraverse = false;
        [Tooltip("When traverse is limited, how many degrees to the left the turret can turn.")]
            [Range(0.0f, 180.0f)]
            public float m_LeftTraverse = 60.0f;
        [Tooltip("When traverse is limited, how many degrees to the right the turret can turn.")]
            [Range(0.0f, 180.0f)]
            public float m_RightTraverse = 60.0f; 
        public FireZonesManager[] m_NoFireZones;

    [Header("Turrets vertical rotation limitations")]
        public ElevationZonesManager[] m_ElevationZones;

    [Header("Other Parameters")]

    [Tooltip("Prefab used")]
        public GameObject m_Prefab;


    // private void Start () { }
    // private void FixedUpdate(){ }

    public void SetUpHardPointComponent(WorldSingleUnit.UnitHardPoint hardPointElement, HardPointComponent hardPointComponent, Transform hardPointTransform, UnitMasterController unitMasterController){
        if (hardPointElement.GetHardpointType() == CompiledTypes.HardPoints.RowValues.ShipFunnel) {
            SetUpShipFunnel(hardPointElement, hardPointComponent, hardPointTransform);
        } else if (hardPointElement.GetHardpointType() == CompiledTypes.HardPoints.RowValues.PlanePropeller) {
            SetUpPlanePropeller(m_Prefab, hardPointTransform, unitMasterController);
        }
        // Debug.Log ("Hardpoint set.  hardpoint id :"+ hardPointElement.GetHardPointID());
    }
    public static void SetUpWeaponHardPoint(WorldSingleWeapon weapon, HardPointComponent hardPointComponent, Transform hardPointTransform, TurretManager turretManager){
        // MODEL
            GameObject turretInstance =
                Instantiate (weapon.GetWeaponPrefab(), hardPointTransform);

        // Add sound
            GameObject audioPrefab = (Resources.Load("Prefabs/Objects/TurretAudioSource", typeof(GameObject))) as GameObject;
            GameObject turretRotationSoundInstance =
                Instantiate (audioPrefab, turretInstance.transform);
            GameObject turretFireSoundInstance =
                Instantiate (audioPrefab, turretInstance.transform);

        // Build/find each script
            TurretRotation turretRotation = turretInstance.AddComponent<TurretRotation>();
            TurretHealth turretHealth = turretInstance.AddComponent<TurretHealth>();
            TurretFireManager turretFireManager = turretInstance.GetComponent<TurretFireManager>();

            turretManager.AddNewWeapon(turretInstance);

        // Turret Fire Manager
            turretFireManager.SetAmmoRef(weapon.GetAmmoRef());
            turretFireManager.SetWeaponRoles(weapon.GetWeaponRoles());
            turretFireManager.SetMaxRange(weapon.GetWeaponMaxRange());
            turretFireManager.SetMinRange(weapon.GetWeaponMinRange());
            turretFireManager.SetMuzzleVelocity(weapon.GetWeaponMuzzleVelocity());
            turretFireManager.SetReloadTime(weapon.GetWeaponReloadTime());
            turretFireManager.SetPrecision(weapon.GetWeaponPrecision());

        // Turret Rotation
            turretRotation.SetRotationSpeed(weapon.GetWeaponRotationSpeed());
            turretRotation.SetElevationSpeed(weapon.GetWeaponElevationSpeed());
            turretRotation.SetElevationMax(weapon.GetWeaponMaxVerticaltraverse());
            turretRotation.SetElevationMin(weapon.GetWeaponMinVerticaltraverse());

            turretRotation.SetLimitTraverse(hardPointComponent.m_LimitTraverse);
            turretRotation.SetLeftTraverse(hardPointComponent.m_LeftTraverse);
            turretRotation.SetRightTraverse(hardPointComponent.m_RightTraverse);
            turretRotation.SetElevationZones(hardPointComponent.m_ElevationZones);
            turretRotation.SetNoFireZones(hardPointComponent.m_NoFireZones);

            
            turretRotation.BeginOperations(hardPointTransform, turretRotationSoundInstance, turretFireManager);
            turretFireManager.BeginOperations(turretFireSoundInstance);
            turretHealth.BeginOperations(turretManager, turretRotation, turretFireManager);
    }

    public static void SetUpPlaneWeaponHardPoint(WorldSingleWeapon weapon, HardPointComponent hardPointComponent, Transform hardPointTransform, PlaneWeaponsManager planeWeaponsManager){
        // MODEL
            GameObject turretInstance =
                Instantiate (weapon.GetWeaponPrefab(), hardPointTransform);

        // Add sound
            GameObject audioPrefab = (Resources.Load("Prefabs/Objects/TurretAudioSource", typeof(GameObject))) as GameObject;
            GameObject turretFireSoundInstance =
                Instantiate (audioPrefab, turretInstance.transform);

        // Build/find each script
            PlaneWeapon planeWeapon = turretInstance.GetComponent<PlaneWeapon>();

            planeWeaponsManager.AddNewWeapon(turretInstance);

        // Turret Fire Manager
            planeWeapon.SetAmmoRef(weapon.GetAmmoRef());
            planeWeapon.SetWeaponRoles(weapon.GetWeaponRoles());
            planeWeapon.SetMaxRange(weapon.GetWeaponMaxRange());
            planeWeapon.SetMinRange(weapon.GetWeaponMinRange());
            planeWeapon.SetMuzzleVelocity(weapon.GetWeaponMuzzleVelocity());
            planeWeapon.SetReloadTime(weapon.GetWeaponReloadTime());
            planeWeapon.SetPrecision(weapon.GetWeaponPrecision());
            
            planeWeapon.BeginOperations(turretFireSoundInstance);
    }

    public static void SetUpShipFunnel(WorldSingleUnit.UnitHardPoint hardPointElement, HardPointComponent hardPointComponent, Transform hardPointTransform){
        
    }

    public static void SetUpPlanePropeller(GameObject propellerPrefab, Transform hardPointTransform, UnitMasterController unitMasterController){
        // MODEL
            GameObject propellerInstance =
                Instantiate (propellerPrefab, hardPointTransform);

        AircraftPropellerAnimator animatorScript = propellerInstance.GetComponent<AircraftPropellerAnimator>();

        // animatorScript

        unitMasterController.AddPropellerAnimator(animatorScript);
        // Debug.Log ("SetUpPlanePropeller set.");
    }

}