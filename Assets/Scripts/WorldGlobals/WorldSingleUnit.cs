using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class WorldSingleUnit {
    // Each parameter is built at the first loading of the game from each prefab added in WorldUnitsManager.
    private GameObject UnitPrefab; public GameObject GetUnitModel(){ return UnitPrefab; }
    private CompiledTypes.Global_Units UnitReference_DB; public CompiledTypes.Global_Units GetUnitReference_DB(){ return UnitReference_DB; }

    private string UnitName; public string GetUnitName(){ return UnitName; }
    // CATEGORIES
        private CompiledTypes.Units_categories.RowValues UnitCategory; public CompiledTypes.Units_categories.RowValues GetUnitCategory(){ return UnitCategory; }
        private CompiledTypes.Units_sub_categories.RowValues UnitSubCategory; public CompiledTypes.Units_sub_categories.RowValues GetUnitSubCategory(){ return UnitSubCategory; }
        private CompiledTypes.Units_sub_categories UnitSubCategory_DB; public CompiledTypes.Units_sub_categories GetUnitSubCategory_DB(){ return UnitSubCategory_DB; }
        private GameObject UnitDeathFX; public GameObject GetUnitDeathFX(){ return UnitDeathFX; }         // If there were multiple of those, there could be multiple death FX
    private CompiledTypes.Countries Nation; public CompiledTypes.Countries GetUnitNation(){ return Nation; }
    private CompiledTypes.Teams.RowValues Team; public CompiledTypes.Teams.RowValues GetUnitTeam(){ return Team; }
    private CompiledTypes.Teams Team_DB;
    // SCORES
        private int UnitCommandPointsCost; public int GetUnitCommandPointsCost(){ return UnitCommandPointsCost; }
        private int UnitVictoryPointsValue; public int GetUnitVictoryPointsValue(){ return UnitVictoryPointsValue; }

    // CAMERA
        private float CameraDistance, CameraHeight, CameraCameraOffset;
        public float GetUnitCameraDistance(){ return CameraDistance; } public float GetUnitCameraHeight(){ return CameraHeight; }public float GetUnitCameraCameraOffset(){ return CameraCameraOffset; }
    // HEALTH
        private float UnitHealth; public float GetUnitHealth(){ return UnitHealth; }

    private bool UnitSet = false;

    public void SetUnit(CompiledTypes.Global_Units unit) {
        if (UnitSet) {
            return;                                 // Prevent the setting to be done multiple times.
        } else {
            UnitSet = true;
        }

        CompiledTypes.Global_Units masterUnitReference = unit;
        if (unit.UnitVariantReferenceList.Count > 0) {
            masterUnitReference = unit.UnitVariantReferenceList[0].UnitVariantRef;      // Set variant master
        }

        // DB REFERENCE
            UnitReference_DB = unit;

        // MODEL
            if (unit.Isavariant && String.IsNullOrEmpty(unit.UnitPath)) {
                UnitPrefab = (Resources.Load(masterUnitReference.UnitPath+"/"+masterUnitReference.UnitModel, typeof(GameObject))) as GameObject;
            } else if (!String.IsNullOrEmpty(unit.UnitPath)) {
                UnitPrefab = (Resources.Load(unit.UnitPath+"/"+unit.UnitModel, typeof(GameObject))) as GameObject;
            }
            if (UnitPrefab == null) {
                Debug.Log (" A unit was implemented without a model ! Unit id :"+ unit.id);
                UnitPrefab = WorldUIVariables.GetErrorModel();
            }

        // NAME
            if (unit.Isavariant && String.IsNullOrEmpty(unit.UnitName)) {
                UnitName = masterUnitReference.UnitName;
            } else if (!String.IsNullOrEmpty(unit.UnitName)) {
                UnitName = unit.UnitName;
            }

        // CATEGORY && SUBCATEGORY && SUBCATEGORY RELATED FX
            // Debug.Log (UnitName+"?UnitCategoryEmpty? : "+String.IsNullOrEmpty(unit.UnitCategory.id)+" - ?unit.Isavariant? :  "+unit.Isavariant+" - unit.UnitCategory.id :  "+unit.UnitCategory.id);

            if (unit.Isavariant && unit.UnitCategory.id.ToString() == "Empty") {
                foreach (CompiledTypes.Units_sub_categories subCat in WorldUnitsManager.GetDB().Units_sub_categories.GetAll()) {
                    if (subCat.id == masterUnitReference.UnitCategory.id) {
                        // Debug.Log ("case1 found for "+UnitName);
                        UnitSubCategory_DB = subCat;
                        string subCatString = subCat.id.ToString();
                        UnitSubCategory = (CompiledTypes.Units_sub_categories.RowValues)System.Enum.Parse( typeof(CompiledTypes.Units_sub_categories.RowValues), subCatString);

                        string catString = subCat.Category.id.ToString();
                        UnitCategory = (CompiledTypes.Units_categories.RowValues)System.Enum.Parse( typeof(CompiledTypes.Units_categories.RowValues), catString );

                        string FXString = "FX/"+masterUnitReference.UnitCategory.DeathFX.FXPath+"/"+masterUnitReference.UnitCategory.DeathFX.FXPrefab;
                        UnitDeathFX = (Resources.Load(FXString, typeof(GameObject))) as GameObject;
                    }
                }
            } else if (unit.UnitCategory.id.ToString() != "Empty") {
                foreach (CompiledTypes.Units_sub_categories subCat in WorldUnitsManager.GetDB().Units_sub_categories.GetAll()) {
                    if (subCat.id == unit.UnitCategory.id) {
                        // Debug.Log ("case2 found for "+UnitName);
                        UnitSubCategory_DB = subCat;
                        string subCatString = subCat.id.ToString();
                        UnitSubCategory = (CompiledTypes.Units_sub_categories.RowValues)System.Enum.Parse( typeof(CompiledTypes.Units_sub_categories.RowValues), subCatString);
                        
                        string catString = subCat.Category.id.ToString();
                        UnitCategory = (CompiledTypes.Units_categories.RowValues)System.Enum.Parse( typeof(CompiledTypes.Units_categories.RowValues), catString );
                        
                        string FXString = "FX/"+unit.UnitCategory.DeathFX.FXPath+"/"+unit.UnitCategory.DeathFX.FXPrefab;
                        UnitDeathFX = (Resources.Load(FXString, typeof(GameObject))) as GameObject;
                    }
                }
            } else {
                Debug.Log ("No category found for "+UnitName);
            }

            if (UnitDeathFX == null) {
                Debug.Log (" A unit was implemented without a model ! Unit id :"+ unit.id);
                UnitDeathFX = WorldUIVariables.GetErrorModel();
            }
            // Debug.Log (UnitName+"UnitSubCategory : "+UnitSubCategory+" - UnitCategory :  "+UnitCategory+" - UnitDeathFX :  "+UnitDeathFX);
        // END CATEGORY

        // NATION
            if (unit.Isavariant && String.IsNullOrEmpty(unit.UnitNation.id)) {
                Nation = masterUnitReference.UnitNation;
                Team_DB = masterUnitReference.UnitNation.Team;
            } else if (!String.IsNullOrEmpty(unit.UnitNation.id)) {
                Nation = unit.UnitNation;
                Team_DB = unit.UnitNation.Team;
            }
            foreach (CompiledTypes.Teams.RowValues team in Enum.GetValues(typeof(CompiledTypes.Teams.RowValues))) {
                if (team.ToString() == Team_DB.id) {
                    Team = team;
                }
            }
            // Debug.Log (UnitName+" WSU Team : "+Team);
        // END NATION

        // SCORES
            if (unit.Isavariant && unit.UnitScoreList.Count == 0) {
                UnitCommandPointsCost = masterUnitReference.UnitScoreList[0].Commandpoints;
                UnitVictoryPointsValue = masterUnitReference.UnitScoreList[0].Victorypoints;
            } else if (unit.UnitCameraParametersList.Count > 0) {
                UnitCommandPointsCost = unit.UnitScoreList[0].Commandpoints;
                UnitVictoryPointsValue = unit.UnitScoreList[0].Victorypoints;
            }

        // CAMERA
            if (unit.Isavariant && unit.UnitCameraParametersList.Count == 0) {
                CameraDistance = masterUnitReference.UnitCameraParametersList[0].Distance;
                CameraHeight = masterUnitReference.UnitCameraParametersList[0].Height;
                CameraCameraOffset = masterUnitReference.UnitCameraParametersList[0].Lateraloffset;
            } else if (unit.UnitCameraParametersList.Count > 0) {
                CameraDistance = unit.UnitCameraParametersList[0].Distance;
                CameraHeight = unit.UnitCameraParametersList[0].Height;
                CameraCameraOffset = unit.UnitCameraParametersList[0].Lateraloffset;
            }

        // HEALTH
            if (unit.Isavariant && unit.UnitHealth == 0) {
                UnitHealth = masterUnitReference.UnitHealth;
            } else if (unit.UnitHealth > 0) {
                UnitHealth = unit.UnitHealth;
            } else {
                Debug.Log (UnitName);
            }

        // DAMAGE CONTROL
            if (unit.Isavariant && unit.UnitDamagecontrolList.Count == 0) {
                if (masterUnitReference.BuoyancyList.Count > 0) {
                    SetBuoyancy(masterUnitReference);
                }
            } else if (unit.BuoyancyList.Count > 0) {
                SetBuoyancy(unit);
            }

        // RIGID BODY
            if (unit.Isavariant && unit.UnitMass == 0) {
                SetRigidBody(masterUnitReference);
            } else if (unit.UnitMass > 0) {
                SetRigidBody(unit);
            } else {
                Debug.Log (UnitName+ " No rigid body set ?");
            }
        
        // WEAPONS
            if (unit.Isavariant && unit.UnitweaponsList.Count == 0) {
                if (masterUnitReference.UnitweaponsList.Count > 0) {
                    SetWeapons(masterUnitReference);
                }
            } else if (unit.UnitweaponsList.Count > 0) {
                SetWeapons(unit);
            }

        // SHIP BUOYANCY
            if (unit.Isavariant && unit.BuoyancyList.Count == 0) {
                if (masterUnitReference.BuoyancyList.Count > 0) {
                    SetDamageControl(masterUnitReference);
                }
            } else if (unit.BuoyancyList.Count > 0) {
                SetDamageControl(unit);
            }

        // foreach (var category in WorldUnitsManager.GetDB().Units_categories.GetAll()) {
        //     if (category.id == UnitCategory) {   
                // string controller = category.ScriptPaths+"/"+category.FileName+"Controller";
                // UnitPrefab.AddComponent(Type.GetType(controller));
                // UnitPrefab.AddComponent<ScriptName>();
        //     }
        // }

        // Debug.Log (CameraHeight+" / "+CameraDistance);
    }

// RIGID BODY
    private float RigidBodyUnitMass; public float GetRigidBodyMass(){ return RigidBodyUnitMass; }
    private float RigidBodyCategoryDrag; public float GetRigidBodyDrag(){ return RigidBodyCategoryDrag; }
    private float RigidBodyCategoryAngularDrag; public float GetRigidBodyAngularDrag(){ return RigidBodyCategoryAngularDrag; }
    private bool RigidBodyCategoryUseGravity; public bool GetRigidBodyUseGravity(){ return RigidBodyCategoryUseGravity; }
    private bool RigidBodyCategoryIsKinematic; public bool GetRigidBodyIsKinematic(){ return RigidBodyCategoryIsKinematic; }
    private bool RigidBodyCategoryFreezePosition; public bool GetRigidBodyFreezePosition(){ return RigidBodyCategoryFreezePosition; }
    private bool RigidBodyCategoryFreezeRotation; public bool GetRigidBodyFreezeRotation(){ return RigidBodyCategoryFreezeRotation; }
    private void SetRigidBody(CompiledTypes.Global_Units unit) {
        // If there is no rigidbody to the prefab, adding one now
        if (!UnitPrefab.GetComponent<Rigidbody>()) { UnitPrefab.AddComponent<Rigidbody>(); }

        RigidBodyUnitMass = unit.UnitMass;
        RigidBodyCategoryDrag = UnitSubCategory_DB.Category.RigidBodyDataList[0].CategoryDrag;
        RigidBodyCategoryAngularDrag = UnitSubCategory_DB.Category.RigidBodyDataList[0].CategoryAngularDrag;
        RigidBodyCategoryUseGravity = UnitSubCategory_DB.Category.RigidBodyDataList[0].CatUseGravity;
        RigidBodyCategoryIsKinematic = UnitSubCategory_DB.Category.RigidBodyDataList[0].CatIsKinematic;
        RigidBodyCategoryFreezePosition = UnitSubCategory_DB.Category.RigidBodyDataList[0].CatFreezePosition;
        RigidBodyCategoryFreezeRotation = UnitSubCategory_DB.Category.RigidBodyDataList[0].CatFreezeRotation;
    }

// WEAPONS
    private List<TurretWeapon> TurretWeaponList = new List<TurretWeapon>(); public List<TurretWeapon> GetTurretWeaponList(){ return TurretWeaponList; }
    private void SetWeapons(CompiledTypes.Global_Units unit) {
        foreach (CompiledTypes.Unitweapons weapon in unit.UnitweaponsList) {
            TurretWeapon _weapon = new TurretWeapon{};

            // Find & set variant master
                CompiledTypes.Weapons weaponReference = weapon.WeaponRef;
                CompiledTypes.Weapons masterWeaponReference = weaponReference;
                if (weapon.WeaponRef.WeaponVariantReferenceList.Count > 0) {
                    masterWeaponReference = weapon.WeaponRef.WeaponVariantReferenceList[0].WeaponVariantRef;
                }

            // TurretManager

                if (weaponReference.Isavariant && weaponReference.ModelPathList.Count == 0) {
                    if (masterWeaponReference.ModelPathList.Count > 0) {
                        _weapon._turretPrefab = (Resources.Load(masterWeaponReference.ModelPathList[0].AmmoPath+"/"+masterWeaponReference.ModelPathList[0].AmmoPath, typeof(GameObject))) as GameObject;
                    }
                } else if (weaponReference.ModelPathList.Count > 0) {
                    _weapon._turretPrefab = (Resources.Load(weaponReference.ModelPathList[0].AmmoPath+"/"+weaponReference.ModelPathList[0].AmmoPath, typeof(GameObject))) as GameObject;
                }

                if (_weapon._turretPrefab == null) {
                    Debug.Log (" A turret was implemented without a model ! turret id :"+ weapon.WeaponRef.id);
                    _weapon._turretPrefab = WorldUIVariables.GetErrorModel();
                }

            // TurretHealth
                if (weaponReference.Isavariant && weaponReference.Health != masterWeaponReference.Health) {
                    _weapon._healthTurretHealth = masterWeaponReference.Health;
                } else {
                    _weapon._healthTurretHealth = weaponReference.Health;
                }
                if (weaponReference.Isavariant && weaponReference.Armor != masterWeaponReference.Armor) {
                    _weapon._healthTurretArmor = masterWeaponReference.Armor;
                } else {
                    _weapon._healthTurretArmor = weaponReference.Armor;
                }
            // TurretFireManager
                if (weaponReference.Isavariant && weaponReference.WeaponRolesList != masterWeaponReference.WeaponRolesList) {
                    foreach (CompiledTypes.WeaponRoles role in masterWeaponReference.WeaponRolesList) {
                        _weapon._fireAvailableWeaponRoles.Add(role);
                    }
                } else {
                    foreach (CompiledTypes.WeaponRoles role in weaponReference.WeaponRolesList) {
                        _weapon._fireAvailableWeaponRoles.Add(role);
                    }
                }
                
                if (weaponReference.Isavariant && weaponReference.Ammo != masterWeaponReference.Ammo) {
                    _weapon._fireManagerAmmo = masterWeaponReference.Ammo;
                } else {
                    _weapon._fireManagerAmmo = weaponReference.Ammo;
                }
                // Transform[] _weapon._fireManagerFireMuzzles;
            //     _weapon._fireManageFireFx;
            //     _weapon._fireManagerShootingAudio;
            //     _weapon._fireManagerFireClip;
            //     _weapon._fireManagerMaxRange;
            //     _weapon._fireManagerMinRange;
            //     _weapon._fireManagerMuzzleVelocity;
            //     _weapon._fireManagerReloadTime;
            //     _weapon._fireManagerPrecision;
            // // TurretRotation
            //     _weapon._rotationAudio;
            //     _weapon._rotationHorizAxis;
            //     _weapon._rotationElevationAxis;
            //     _weapon._rotationParent;
            //     _weapon._rotationSpeed;
            //     _weapon._rotationLimitTraverse;
            //     _weapon._rotationLeftTraverse;
            //     _weapon._rotationRightTraverse;
            //     // public FireZonesManager[] _weapon._rotationNoFireZones;
            //     _weapon._rotationElevationSpeed;
            //     _weapon._rotationUpTraverse;
                // _weapon._rotationDownTraverse;
                // public ElevationZonesManager[] _weapon._rotationElevationZones;
        }
    }

    public class TurretWeapon {
        // TurretManager
            public GameObject _turretPrefab;
        // TurretHealth
            public float _healthTurretHealth;
            public float _healthTurretArmor;
        // TurretFireManager
            public List<CompiledTypes.WeaponRoles> _fireAvailableWeaponRoles = new List<CompiledTypes.WeaponRoles>();
            public CompiledTypes.Ammos _fireManagerAmmo;
            public Transform[] _fireManagerFireMuzzles;
            public GameObject _fireManageFireFx;
            public AudioSource _fireManagerShootingAudio;
            public AudioClip _fireManagerFireClip;
            public float _fireManagerMaxRange;
            public float _fireManagerMinRange;
            public float _fireManagerMuzzleVelocity;
            public float _fireManagerReloadTime;
            public float _fireManagerPrecision;
        // TurretRotation
            public AudioClip _rotationAudio;
            public Rigidbody _rotationHorizAxis;
            public Rigidbody _rotationElevationAxis;
            public Rigidbody _rotationParent;
            public float _rotationSpeed;
            public bool _rotationLimitTraverse;
            public float _rotationLeftTraverse;
            public float _rotationRightTraverse;
            public FireZonesManager[] _rotationNoFireZones;
            public float _rotationElevationSpeed;
            public float _rotationUpTraverse;
            public float _rotationDownTraverse;
            public ElevationZonesManager[] _rotationElevationZones;
        
    }
    
// DAMAGE CONTROL
    private bool DamageControlExists = false; public bool GetDamageControlExists(){ return DamageControlExists; }
    private float DamageControlRepairRate; public float GetDamageControlRepairRate(){ return DamageControlRepairRate; }
    private int DamageControlRepairCrew; public int GetDamageControlRepairCrew(){ return DamageControlRepairCrew; }
    private void SetDamageControl(CompiledTypes.Global_Units unit) {
        DamageControlExists = true;
        DamageControlRepairRate = unit.UnitDamagecontrolList[0].Repairrate;
        DamageControlRepairCrew = unit.UnitDamagecontrolList[0].Repaircrew;
    }
    
// SHIP BUOYANCY
    private Vector3 BuoyancyUnitCenterOfMass; public Vector3 GetBuoyancyUnitCenterOfMass(){ return BuoyancyUnitCenterOfMass; }
    private List<Vector3> BuoyancyForcePointsList = new List<Vector3>();public List<Vector3> GetBuoyancyForcePointsList(){ return BuoyancyForcePointsList; }
    private float BuoyancyModelWidth; public float GetBuoyancyModelWidth(){ return BuoyancyModelWidth; }
    private float BuoyancyVerticalDrag; public float GetBuoyancyVerticalDrag(){ return BuoyancyVerticalDrag; }
    private float BuoyancyEnginePower; public float GetBuoyancyEnginePower(){ return BuoyancyEnginePower; }
    private float BuoyancyRotationTime; public float GetBuoyancyRotationTime(){ return BuoyancyRotationTime; }
    private void SetBuoyancy(CompiledTypes.Global_Units unit) {
        BuoyancyUnitCenterOfMass = new Vector3(unit.BuoyancyList[0].Center_of_massList[0].X, unit.BuoyancyList[0].Center_of_massList[0].Y, unit.BuoyancyList[0].Center_of_massList[0].Z);

        foreach (CompiledTypes.Force_points point in unit.BuoyancyList[0].Force_pointsList) {
            // Instead of having to painstakingly add each point to the DB, we put will only enter one point and clone it on the X & Z axis when needed.
            Vector3 _point = new Vector3(point.X, point.Y, point.Z);
            BuoyancyForcePointsList.Add(_point);
            if (point.X != 0 && point.Z != 0) {
                Vector3 _point1 = new Vector3(-point.X, point.Y, point.Z);
                BuoyancyForcePointsList.Add(_point1);
                Vector3 _point2 = new Vector3(point.X, point.Y, -point.Z);
                BuoyancyForcePointsList.Add(_point2);
                Vector3 _point3 = new Vector3(-point.X, point.Y, -point.Z);
                BuoyancyForcePointsList.Add(_point3);
            } else if (point.X != 0) {
                Vector3 _point1 = new Vector3(-point.X, point.Y, point.Z);
                BuoyancyForcePointsList.Add(_point1);
            } else if ( point.Z != 0 ) {
                Vector3 _point1 = new Vector3(point.X, point.Y, -point.Z);
                BuoyancyForcePointsList.Add(_point1);
            }
        }

        BuoyancyModelWidth = unit.BuoyancyList[0].Model_width;

        BuoyancyVerticalDrag = unit.BuoyancyList[0].Vertical_drag;

        BuoyancyEnginePower = unit.BuoyancyList[0].Engine_power;

        BuoyancyRotationTime = unit.BuoyancyList[0].Rotation_time;
    }
}