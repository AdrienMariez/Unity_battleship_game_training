using UnityEngine;
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

    public static void SetUpHardPointComponent(WorldSingleUnit.UnitHardPoint hardPointElement,HardPointComponent hardPointComponent, Transform hardPointTransform){
        if (hardPointElement.GetHardpointType() == CompiledTypes.HardPoints.RowValues.ShipFunnel) {
            SetUpShipFunnel(hardPointElement,hardPointComponent, hardPointTransform);
        }
        // Debug.Log ("Hardpoint set.  hardpoint id :"+ hardPointElement.GetHardPointID());
    }
    public static void SetUpWeaponHardPoint(WorldSingleUnit.UnitHardPoint hardPointElement,HardPointComponent hardPointComponent, Transform hardPointTransform, TurretManager turretManager){
        // Debug.Log("SetUpWeaponHardPoint");
        // Find & set variant master
            CompiledTypes.Weapons weaponReference = hardPointElement.GetWeaponType();
            CompiledTypes.Weapons masterWeaponReference = weaponReference;
            if (weaponReference.Isavariant && weaponReference.WeaponVariantReferenceList.Count > 0) {
                masterWeaponReference = weaponReference.WeaponVariantReferenceList[0].WeaponVariantRef;
            }
            // else { Debug.Log (weaponReference.id); }


        // Set Prefab
            GameObject weaponPrefab = null;
            if (weaponReference.Isavariant && weaponReference.WeaponModelPathList.Count == 0) {
                if (masterWeaponReference.WeaponModelPathList.Count > 0) {
                    weaponPrefab = (Resources.Load("Prefabs/Weapons/"+masterWeaponReference.WeaponModelPathList[0].TurretPath+"/"+masterWeaponReference.WeaponModelPathList[0].TurretModel, typeof(GameObject))) as GameObject;
                }
            } else if (weaponReference.WeaponModelPathList.Count > 0) {
                weaponPrefab = (Resources.Load("Prefabs/Weapons/"+weaponReference.WeaponModelPathList[0].TurretPath+"/"+weaponReference.WeaponModelPathList[0].TurretModel, typeof(GameObject))) as GameObject;
            }
            if (weaponPrefab == null) {
                Debug.Log (" A turret was implemented without a model ! turret id :"+ weaponReference.id);
                weaponPrefab = WorldUIVariables.GetErrorModel();
            }

        // MODEL
            GameObject turretInstance =
                Instantiate (weaponPrefab, hardPointTransform);

            turretManager.AddNewWeapon(turretInstance);

        // Add sound
            GameObject audioPrefab = (Resources.Load("Prefabs/Objects/TurretAudioSource", typeof(GameObject))) as GameObject;
            GameObject turretRotationSoundInstance =
                Instantiate (audioPrefab, turretInstance.transform);
            GameObject turretFireSoundInstance =
                Instantiate (audioPrefab, turretInstance.transform);

        // Build/find each script
            TurretRotation turretRotation = turretInstance.AddComponent<TurretRotation>();
            TurretFireManager turretFireManager = turretInstance.GetComponent<TurretFireManager>();
            TurretHealth turretHealth = turretInstance.AddComponent<TurretHealth>();

        // Turret Fire Manager
            if (weaponReference.Isavariant && weaponReference.Max_range == 0) {
                turretFireManager.SetMaxRange(masterWeaponReference.Max_range);
            } else {
                turretFireManager.SetMaxRange(weaponReference.Max_range);
            }
            if (weaponReference.Isavariant && weaponReference.Min_range == 0) {
                turretFireManager.SetMinRange(masterWeaponReference.Min_range);
            } else {
                turretFireManager.SetMinRange(weaponReference.Min_range);
            }
            if (weaponReference.Isavariant && weaponReference.Muzzle_velocity == 0) {
                turretFireManager.SetMuzzleVelocity(masterWeaponReference.Muzzle_velocity);
            } else {
                turretFireManager.SetMuzzleVelocity(weaponReference.Muzzle_velocity);
            }
            if (weaponReference.Isavariant && weaponReference.Reload_time == 0) {
                turretFireManager.SetReloadTime(masterWeaponReference.Reload_time);
            } else {
                turretFireManager.SetReloadTime(weaponReference.Reload_time);
            }
            if (weaponReference.Isavariant && weaponReference.Precision == 0) {
                turretFireManager.SetPrecision(masterWeaponReference.Reload_time);
            } else {
                turretFireManager.SetPrecision(weaponReference.Precision);
            }

        // Turret Rotation
            if (weaponReference.Isavariant && weaponReference.Rotation_speed == 0) {
                turretRotation.SetRotationSpeed(masterWeaponReference.Rotation_speed);
            } else {
                turretRotation.SetRotationSpeed(weaponReference.Rotation_speed);
            }
            turretRotation.SetLimitTraverse(hardPointComponent.m_LimitTraverse);
            turretRotation.SetLeftTraverse(hardPointComponent.m_LeftTraverse);
            turretRotation.SetRightTraverse(hardPointComponent.m_RightTraverse);
            turretRotation.SetElevationZones(hardPointComponent.m_ElevationZones);
            turretRotation.SetNoFireZones(hardPointComponent.m_NoFireZones);
            if (weaponReference.Isavariant && weaponReference.Elevation_speed == 0) {
                turretRotation.SetElevationSpeed(masterWeaponReference.Elevation_speed);
            } else {
                turretRotation.SetElevationSpeed(weaponReference.Elevation_speed);
            }
            if (weaponReference.Isavariant && weaponReference.Max_vertical_traverse == 0) {
                turretRotation.SetElevationMax(masterWeaponReference.Max_vertical_traverse);
            } else {
                turretRotation.SetElevationMax(weaponReference.Max_vertical_traverse);
            }
            if (weaponReference.Isavariant && weaponReference.Min_vertical_traverse == 0) {
                turretRotation.SetElevationMin(masterWeaponReference.Min_vertical_traverse);
            } else {
                turretRotation.SetElevationMin(weaponReference.Min_vertical_traverse);
            }

        // Set all FX
            if (weaponReference.Isavariant && weaponReference.WeaponFXList.Count == 0) {
                if (masterWeaponReference.WeaponFXList.Count > 0) {
                    turretRotation.SetTurretRotationAudio((AudioClip) Resources.Load("Sounds/"+masterWeaponReference.WeaponFXList[0].RotationSound.SoundFXPath+""+masterWeaponReference.WeaponFXList[0].RotationSound.SoundFXPrefab));
                    turretFireManager.SetFireAudio((AudioClip) Resources.Load("Sounds/"+masterWeaponReference.WeaponFXList[0].ShootingSound.SoundFXPath+""+masterWeaponReference.WeaponFXList[0].ShootingSound.SoundFXPrefab));
                    turretFireManager.SetFireFx((GameObject) Resources.Load("FX/"+masterWeaponReference.WeaponFXList[0].FXShooting.FXPath+""+masterWeaponReference.WeaponFXList[0].FXShooting.FXPrefab));
                }
            } else if (weaponReference.WeaponFXList.Count > 0) {
                turretRotation.SetTurretRotationAudio((AudioClip) Resources.Load("Sounds/"+weaponReference.WeaponFXList[0].RotationSound.SoundFXPath+""+weaponReference.WeaponFXList[0].RotationSound.SoundFXPrefab));
                turretFireManager.SetFireAudio((AudioClip) Resources.Load("Sounds/"+weaponReference.WeaponFXList[0].ShootingSound.SoundFXPath+""+weaponReference.WeaponFXList[0].ShootingSound.SoundFXPrefab));
                turretFireManager.SetFireFx((GameObject) Resources.Load("FX/"+weaponReference.WeaponFXList[0].FXShooting.FXPath+""+weaponReference.WeaponFXList[0].FXShooting.FXPrefab));
            }
            if (turretRotation.GetTurretRotationAudio() == null) {
                Debug.Log (" No TurretRotationAudio found  or"+ weaponReference.id);
                turretRotation.SetTurretRotationAudio(WorldUIVariables.GetErrorSound());
            }
            if (turretFireManager.GetFireAudio() == null) {
                Debug.Log (" No FireAudio found  or"+ weaponReference.id);
                turretFireManager.SetFireAudio(WorldUIVariables.GetErrorSound());
            }
            // else {
            //     Debug.Log (turretFireManager.GetFireAudio());
            // }
            
            turretRotation.BeginOperations(hardPointTransform, turretRotationSoundInstance, turretFireManager);
            turretFireManager.BeginOperations(turretRotation, turretFireSoundInstance);

        // Health
            if (weaponReference.Isavariant && weaponReference.Armor == 0) {
                turretHealth.SetElementArmor(masterWeaponReference.Armor);
            } else {
                turretHealth.SetElementArmor(weaponReference.Armor);
            }
            if (weaponReference.Isavariant && weaponReference.Health == 0) {
                turretHealth.SetStartingHealth(masterWeaponReference.Health);
            } else {
                turretHealth.SetStartingHealth(weaponReference.Health);
            }
            turretHealth.BeginOperations(turretManager, turretRotation, turretFireManager);


    }

    public static void SetUpShipFunnel(WorldSingleUnit.UnitHardPoint hardPointElement,HardPointComponent hardPointComponent, Transform hardPointTransform){
        
    }

}