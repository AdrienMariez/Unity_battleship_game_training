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
            if (unit.Isavariant && unit.UnitModelPathList.Count == 0) {
                UnitPrefab = (Resources.Load("Prefabs/Units/"+masterUnitReference.UnitModelPathList[0].UnitPath+""+masterUnitReference.UnitModelPathList[0].UnitModel, typeof(GameObject))) as GameObject;
            } else if (unit.UnitCameraParametersList.Count > 0) {
                UnitPrefab = (Resources.Load("Prefabs/Units/"+unit.UnitModelPathList[0].UnitPath+""+unit.UnitModelPathList[0].UnitModel, typeof(GameObject))) as GameObject;
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
                UnitSubCategory_DB = masterUnitReference.UnitCategory;
                string subCatString = masterUnitReference.UnitCategory.id.ToString();
                UnitSubCategory = (CompiledTypes.Units_sub_categories.RowValues)System.Enum.Parse( typeof(CompiledTypes.Units_sub_categories.RowValues), subCatString);

                string catString = masterUnitReference.UnitCategory.Category.id.ToString();
                UnitCategory = (CompiledTypes.Units_categories.RowValues)System.Enum.Parse( typeof(CompiledTypes.Units_categories.RowValues), catString );

                string FXString = "FX/"+masterUnitReference.UnitCategory.DeathFX.FXPath+""+masterUnitReference.UnitCategory.DeathFX.FXPrefab;
                UnitDeathFX = (Resources.Load(FXString, typeof(GameObject))) as GameObject;
            } else if (unit.UnitCategory.id.ToString() != "Empty") {
                UnitSubCategory_DB = unit.UnitCategory;
                string subCatString = unit.UnitCategory.id.ToString();
                UnitSubCategory = (CompiledTypes.Units_sub_categories.RowValues)System.Enum.Parse( typeof(CompiledTypes.Units_sub_categories.RowValues), subCatString);
                
                string catString = unit.UnitCategory.Category.id.ToString();
                UnitCategory = (CompiledTypes.Units_categories.RowValues)System.Enum.Parse( typeof(CompiledTypes.Units_categories.RowValues), catString );
                
                string FXString = "FX/"+unit.UnitCategory.DeathFX.FXPath+""+unit.UnitCategory.DeathFX.FXPrefab;
                UnitDeathFX = (Resources.Load(FXString, typeof(GameObject))) as GameObject;
            } else {
                Debug.Log ("No category found for "+UnitName);
            }

            if (UnitDeathFX == null) {
                Debug.Log (" No UnitDeathFX found. Unit id :"+ unit.id);
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

        // HARDPOINTS
            if (unit.Isavariant && unit.UnitHardPointsList.Count == 0) {
                if (masterUnitReference.UnitweaponsList.Count > 0) {
                    SetHardPoints(masterUnitReference, false);
                }
            } else if (unit.Isavariant && unit.UnitHardPointsList.Count > 0) {
                SetHardPoints(masterUnitReference, true);
                SetHardPoints(unit, false);
            }else if (unit.UnitHardPointsList.Count > 0) {
                SetHardPoints(unit, false);
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

// HARDPOINTS
    private List<UnitHardPoint> UnitHardPointList = new List<UnitHardPoint>(); public List<UnitHardPoint> GetUnitHardPointList(){ return UnitHardPointList; }
    private bool WeaponExists = false; public bool GetWeaponExists(){ return WeaponExists; }
    private void SetHardPoints(CompiledTypes.Global_Units unit, bool isAParent) {
        foreach (CompiledTypes.UnitHardPoints hardpoint in unit.UnitHardPointsList) {
            if (isAParent && !hardpoint.IsTransferedToVariants) { return; }

            UnitHardPoint _hardpoint = new UnitHardPoint{};

            _hardpoint.SetHardPointID(hardpoint.HardPointId);
            string hardPointTypeString = hardpoint.HardPointType.id.ToString();
            CompiledTypes.HardPoints.RowValues hardPointType = (CompiledTypes.HardPoints.RowValues)System.Enum.Parse( typeof(CompiledTypes.HardPoints.RowValues), hardPointTypeString);
            if (hardPointType == CompiledTypes.HardPoints.RowValues.Weapon) {
                WeaponExists = true;
            }
            _hardpoint.SetHardpointType(hardPointType);
            _hardpoint.SetIsMirrored(hardpoint.IsMirrored);
            if (!String.IsNullOrEmpty(hardpoint.WeaponType.id)) {
                foreach (WorldSingleWeapon weapon in WorldUnitsManager.GetWorldWeapons()) {
                    if (hardpoint.WeaponType.id == weapon.GetWeaponReference_DB().id) {
                        _hardpoint.SetWeapon(weapon);
                        break;
                    }
                }
            }
            _hardpoint.SetWeaponType(hardpoint.WeaponType);

            // Test if hardpoint exists
            Transform HP = UnitPrefab.transform.Find("HardPoints").transform.Find(hardpoint.HardPointId.ToString());
            if (HP == null) {
                Debug.Log (" No such hardpoint.  hardpoint id :"+ hardpoint.HardPointId + " in " + unit.id);
                return;
            }
            HardPointComponent HPC = HP.GetComponent<HardPointComponent>();
            if (HPC == null) {
                Debug.Log ("hardpoint has no component.  hardpoint id :"+ hardpoint.HardPointId + " in " + unit.id);
                return;
            }

            UnitHardPointList.Add(_hardpoint);


            // if (UnitPrefab.transform.Find("HardPoints").transform.Find(hardpoint.HardPointId.ToString()).GetComponent<HardPointComponent>()) {             // If the hardpoint exists in the model, set in table
            //     UnitHardPointList.Add(_hardpoint);
            // } else {
            //     Debug.Log (" No such hardpoint.  hardpoint id :"+ hardpoint.HardPointId + " in " + unit.id);
            // }

        }
    }
    public class UnitHardPoint {
        private string _hardPointIdentifier;  public string GetHardPointID(){ return _hardPointIdentifier; } public void SetHardPointID(string _hpID){ _hardPointIdentifier = _hpID; }
        private CompiledTypes.HardPoints.RowValues _hardpointType;  public CompiledTypes.HardPoints.RowValues GetHardpointType(){ return _hardpointType; } public void SetHardpointType(CompiledTypes.HardPoints.RowValues _hpType){ _hardpointType = _hpType; }
        private bool _isMirrored;  public bool GetIsMirrored(){ return _isMirrored; } public void SetIsMirrored(bool _hpMirror){ _isMirrored = _hpMirror; }
        private WorldSingleWeapon _weapon;  public WorldSingleWeapon GetWeapon(){ return _weapon; } public void SetWeapon(WorldSingleWeapon _hpWeapon){ _weapon = _hpWeapon; }
        // To remove
        private CompiledTypes.Weapons _weaponType;  public CompiledTypes.Weapons GetWeaponType(){ return _weaponType; } public void SetWeaponType(CompiledTypes.Weapons _hpWeaponT){ _weaponType = _hpWeaponT; }
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