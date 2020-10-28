using System;
using UnityEngine;

[Serializable]
public class WorldSingleWeapon {
    // Each parameter is built at the first loading of the game from each prefab added in WorldUnitsManager.
    private CompiledTypes.Weapons WeaponReference_DB; public CompiledTypes.Weapons GetWeaponReference_DB(){ return WeaponReference_DB; }
    private GameObject WeaponPrefab; public GameObject GetWeaponPrefab(){ return WeaponPrefab; }
    private WorldSingleAmmo AmmoRef; public WorldSingleAmmo GetAmmoRef(){ return AmmoRef; }
    // Health
        private float WeaponHealth; public float GetWeaponHealth(){ return WeaponHealth; }
        private float WeaponArmor; public float GetWeaponArmor(){ return WeaponArmor; }
    private float WeaponMaxRange; public float GetWeaponMaxRange(){ return WeaponMaxRange; }
    // Fire manager
        private float WeaponMinRange; public float GetWeaponMinRange(){ return WeaponMinRange; }
        private float WeaponMuzzleVelocity; public float GetWeaponMuzzleVelocity(){ return WeaponMuzzleVelocity; }
        private float WeaponReloadTime; public float GetWeaponReloadTime(){ return WeaponReloadTime; }
        private int WeaponPrecision; public int GetWeaponPrecision(){ return WeaponPrecision; }
        private AudioClip FireSound; public AudioClip GetFireSound(){ return FireSound; }
        private GameObject FireFXPrefab; public GameObject GetFireFXPrefab(){ return FireFXPrefab; }
    // Rotation
        private float WeaponRotationSpeed; public float GetWeaponRotationSpeed(){ return WeaponRotationSpeed; }
        private float WeaponElevationSpeed; public float GetWeaponElevationSpeed(){ return WeaponElevationSpeed; }
        private float WeaponMaxVerticaltraverse; public float GetWeaponMaxVerticaltraverse(){ return WeaponMaxVerticaltraverse; }
        private float WeaponMinVerticaltraverse; public float GetWeaponMinVerticaltraverse(){ return WeaponMinVerticaltraverse; }
        private AudioClip RotationSound ; public AudioClip GetRotationSound(){ return RotationSound; }


    public void SetWeapon(CompiledTypes.Weapons weapon) {
        WeaponReference_DB = weapon;

        // Find & set variant master
            CompiledTypes.Weapons masterWeaponReference = weapon;
            if (weapon.Isavariant && weapon.WeaponVariantReferenceList.Count > 0) {
                masterWeaponReference = weapon.WeaponVariantReferenceList[0].WeaponVariantRef;
            }

        // Prefab
            if (weapon.Isavariant && weapon.WeaponModelPathList.Count == 0) {
                if (masterWeaponReference.WeaponModelPathList.Count > 0) {
                    WeaponPrefab = (Resources.Load("Prefabs/Weapons/"+masterWeaponReference.WeaponModelPathList[0].TurretPath+"/"+masterWeaponReference.WeaponModelPathList[0].TurretModel, typeof(GameObject))) as GameObject;
                }
            } else if (weapon.WeaponModelPathList.Count > 0) {
                WeaponPrefab = (Resources.Load("Prefabs/Weapons/"+weapon.WeaponModelPathList[0].TurretPath+"/"+weapon.WeaponModelPathList[0].TurretModel, typeof(GameObject))) as GameObject;
            }
            if (WeaponPrefab == null) {
                Debug.Log (" A turret was implemented without a model ! turret id :"+ weapon.id);
                WeaponPrefab = WorldUIVariables.GetErrorModel();
            }

        // Health
            if (weapon.Isavariant && weapon.Armor == 0) {
                WeaponArmor = masterWeaponReference.Armor;
            } else {
                WeaponArmor = masterWeaponReference.Armor;
            }
            if (weapon.Isavariant && weapon.Health == 0) {
                WeaponHealth = masterWeaponReference.Health;
            } else {
                WeaponHealth = masterWeaponReference.Health;
            }

        // Turret Fire Manager
            foreach (WorldSingleAmmo ammo in WorldUnitsManager.GetWorldAmmos()) {
                if (weapon.Isavariant && String.IsNullOrEmpty(weapon.Ammo.id)) {
                    if (ammo.GetAmmoReference_DB().id == masterWeaponReference.Ammo.id) {
                        AmmoRef = ammo;
                        break;
                    }
                } else if (!String.IsNullOrEmpty(weapon.Ammo.id)) {
                    if (ammo.GetAmmoReference_DB().id == weapon.Ammo.id) {
                        AmmoRef = ammo;
                        break;
                    }
                }
            }
            if (weapon.Isavariant && weapon.Max_range == 0) {
                WeaponMaxRange = masterWeaponReference.Max_range;
            } else {
                WeaponMaxRange = weapon.Max_range;
            }
            if (weapon.Isavariant && weapon.Min_range == 0) {
                WeaponMinRange = masterWeaponReference.Min_range;
            } else {
                WeaponMinRange = weapon.Min_range;
            }
            if (weapon.Isavariant && weapon.Muzzle_velocity == 0) {
                WeaponMuzzleVelocity = masterWeaponReference.Muzzle_velocity;
            } else {
                WeaponMuzzleVelocity = weapon.Muzzle_velocity;
            }
            if (weapon.Isavariant && weapon.Reload_time == 0) {
                WeaponReloadTime = masterWeaponReference.Reload_time;
            } else {
                WeaponReloadTime = weapon.Reload_time;
            }
            if (weapon.Isavariant && weapon.Precision == 0) {
                WeaponPrecision = masterWeaponReference.Precision;
            } else {
                WeaponPrecision = weapon.Precision;
            }

        // Turret Rotation
            if (weapon.Isavariant && weapon.Rotation_speed == 0) {
                WeaponRotationSpeed = masterWeaponReference.Rotation_speed;
            } else {
                WeaponRotationSpeed = weapon.Rotation_speed;
            }
            if (weapon.Isavariant && weapon.Elevation_speed == 0) {
                WeaponElevationSpeed = masterWeaponReference.Elevation_speed;
            } else {
                WeaponElevationSpeed = weapon.Elevation_speed;
            }
            if (weapon.Isavariant && weapon.Max_vertical_traverse == 0) {
                WeaponMaxVerticaltraverse = masterWeaponReference.Max_vertical_traverse;
            } else {
                WeaponMaxVerticaltraverse = weapon.Max_vertical_traverse;
            }
            if (weapon.Isavariant && weapon.Min_vertical_traverse == 0) {
                WeaponMinVerticaltraverse = masterWeaponReference.Min_vertical_traverse;
            } else {
                WeaponMinVerticaltraverse = weapon.Min_vertical_traverse;
            }

        // FX
            // FireSound
            // FireFXPrefab
            // RotationSound
            if (weapon.Isavariant && weapon.WeaponFXList.Count == 0) {
                if (masterWeaponReference.WeaponFXList.Count > 0) {
                    RotationSound = ((AudioClip) Resources.Load("Sounds/"+masterWeaponReference.WeaponFXList[0].RotationSound.SoundFXPath+""+masterWeaponReference.WeaponFXList[0].RotationSound.SoundFXPrefab));
                    FireSound = ((AudioClip) Resources.Load("Sounds/"+masterWeaponReference.WeaponFXList[0].ShootingSound.SoundFXPath+""+masterWeaponReference.WeaponFXList[0].ShootingSound.SoundFXPrefab));
                    FireFXPrefab = ((GameObject) Resources.Load("FX/"+masterWeaponReference.WeaponFXList[0].FXShooting.FXPath+""+masterWeaponReference.WeaponFXList[0].FXShooting.FXPrefab));
                }
            } else if (weapon.WeaponFXList.Count > 0) {
                RotationSound = ((AudioClip) Resources.Load("Sounds/"+weapon.WeaponFXList[0].RotationSound.SoundFXPath+""+weapon.WeaponFXList[0].RotationSound.SoundFXPrefab));
                FireSound = ((AudioClip) Resources.Load("Sounds/"+weapon.WeaponFXList[0].ShootingSound.SoundFXPath+""+weapon.WeaponFXList[0].ShootingSound.SoundFXPrefab));
                FireFXPrefab = ((GameObject) Resources.Load("FX/"+weapon.WeaponFXList[0].FXShooting.FXPath+""+weapon.WeaponFXList[0].FXShooting.FXPrefab));
            }
            if (RotationSound == null) {
                Debug.Log (" No TurretRotationAudio found  or"+ weapon.id);
                RotationSound = (WorldUIVariables.GetErrorSound());
            }
            if (FireSound == null) {
                Debug.Log (" No FireAudio found  or"+ weapon.id);
                FireSound = (WorldUIVariables.GetErrorSound());
            }
            if (FireFXPrefab == null) {
                Debug.Log (" No FireFXPrefab found  or"+ weapon.id);
                FireFXPrefab = (WorldUIVariables.GetErrorModel());
            }

            // Debug.Log (weapon.id +" is set");
    }
}