using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CompiledTypes;

public class WorldUnitsManager : MonoBehaviour {

    // public WaveList.Wave[] waves;

    // [Header("Add all units of the game to this list !")]
    // public WorldSingleUnit[] m_WorldSingleUnit;

    // [Header("Add all turrets of the game to this list !")]
    // public WorldSingleTurret[] m_WorldSingleTurret;

    public TextAsset CastleDBAsset;
    private static CastleDB DB; public static CastleDB GetDB(){ return DB; }

    [Tooltip("What can be hit by ammo or more generally, what is considered a game element that can be damaged.")]
    public LayerMask m_WeaponHitMask;
    private static LayerMask HitMask; public static LayerMask GetHitMask(){ return HitMask; }
    [Tooltip("What can be collided ingame or more generally, what is considered an element that should be checked for any collision.")]
    public LayerMask m_WorldMask;
    private static LayerMask WorldMask; public static LayerMask GetWorldMask(){ return WorldMask; }


    private static GameManager GameManager; public static GameManager GetGameManager(){ return GameManager; } public static void SetGameManager(GameManager _s){ GameManager =_s; }

    private static bool FirstLoad = true; public static bool GetFirstLoad(){ return FirstLoad; }
    private void Awake() {
        if (FirstLoad){
            FirstLoad = false;
            DB = new CastleDB(CastleDBAsset);
            HitMask = m_WeaponHitMask;
            WorldMask = m_WorldMask;
            WorldSetUnits();
        }
    }
    // public static void BuildFirstLoad() {
    //     if (FirstLoad){
    //         FirstLoad = false;
    //         DB = new CastleDB(CastleDBAsset);
    //         HitMask = m_WeaponHitMask;
    //         WorldSetUnits();
    //     }
    // }
    
    static List<CompiledTypes.Units_sub_categories> SubCategoriesDB = new List<CompiledTypes.Units_sub_categories>();
    private static List<List<WorldSingleUnit>> UnitsBySubcategory = new List<List<WorldSingleUnit>>();
    public static List<List<WorldSingleUnit>> GetUnitsBySubcategory() { return UnitsBySubcategory; }

    static List<CompiledTypes.Units_categories> CategoriesDB = new List<CompiledTypes.Units_categories>();
    private static List<List<WorldSingleUnit>> UnitsByCategory = new List<List<WorldSingleUnit>>();
    public static List<List<WorldSingleUnit>> GetUnitsByCategory() { return UnitsByCategory; }


    private static List<WorldSingleAmmo> WorldAmmos = new List<WorldSingleAmmo>(); public static List<WorldSingleAmmo> GetWorldAmmos() { return WorldAmmos; }
    private static List<WorldSingleWeapon> WorldWeapons = new List<WorldSingleWeapon>(); public static List<WorldSingleWeapon> GetWorldWeapons() { return WorldWeapons; }
    
    private void WorldSetUnits() {

        // BUILD ALL AMMO
        foreach (CompiledTypes.Ammos ammo in DB.Ammos.GetAll()) {
            WorldSingleAmmo newAmmo = new WorldSingleAmmo();
            newAmmo.SetAmmo(ammo);
            WorldAmmos.Add(newAmmo);
            // Debug.Log (ammo.id +" is set");
        }

        // BUILD ALL WEAPONS
        foreach (CompiledTypes.Weapons weapon in DB.Weapons.GetAll()) {
            WorldSingleWeapon newWeapon = new WorldSingleWeapon();
            newWeapon.SetWeapon(weapon);
            WorldWeapons.Add(newWeapon);
            // Debug.Log (weapon.id +" is set");
        }


        // BUILD ALL UNITS
        foreach (CompiledTypes.Units_sub_categories category in DB.Units_sub_categories.GetAll()) {
            SubCategoriesDB.Add(category);
        }

        foreach (CompiledTypes.Units_sub_categories subCategory in SubCategoriesDB) {
            List<WorldSingleUnit> categoryObjects = new List<WorldSingleUnit>();
            foreach (CompiledTypes.Global_Units unit in DB.Global_Units.GetAll()) {
                if (unit.Isavariant && unit.UnitCategory.id.ToString() == "Empty") {
                    CompiledTypes.Global_Units masterUnitReference = unit.UnitVariantReferenceList[0].UnitVariantRef;
                    if (masterUnitReference.UnitCategory.id == subCategory.id) {
                        WorldSingleUnit newUnit = new WorldSingleUnit();
                        newUnit.SetUnit(unit);
                        categoryObjects.Add(newUnit);
                    }
                } else if (unit.UnitCategory.id == subCategory.id) {
                    WorldSingleUnit newUnit = new WorldSingleUnit();
                    newUnit.SetUnit(unit);
                    categoryObjects.Add(newUnit);
                }
            }
            UnitsBySubcategory.Add(categoryObjects);
            // Debug.Log ("loop in WUM");
        }

        // Build UNIT LIST BY CATEGORY
        // foreach (CompiledTypes.Units_categories category in DB.Units_categories.GetAll()) {
        //     CategoriesDB.Add(category);
        // }
        // foreach (CompiledTypes.Units_categories category in CategoriesDB) {
        //     List<CompiledTypes.Global_Units> categoryObjects = new List<CompiledTypes.Global_Units>();
        //     foreach (CompiledTypes.Global_Units unit in DB.Global_Units.GetAll()) {
        //         if (unit.Isavariant && unit.UnitCategory.id.ToString() == "Empty") {
        //             CompiledTypes.Global_Units masterUnitReference = unit.UnitVariantReferenceList[0].UnitVariantRef;
        //             if (masterUnitReference.UnitCategory.Category.id == category.id) {
        //                 categoryObjects.Add(unit);
        //             }
        //         } else if (unit.UnitCategory.Category.id == category.id) {
        //             categoryObjects.Add(unit);
        //         }
        //     }
        //     UnitsDBByCategory.Add(categoryObjects);
        // }

        foreach (CompiledTypes.Units_categories category in DB.Units_categories.GetAll()) {
            CategoriesDB.Add(category);
        }

        foreach (CompiledTypes.Units_categories category in CategoriesDB) {
            List<WorldSingleUnit> categoryObjects = new List<WorldSingleUnit>();
            foreach (List<WorldSingleUnit> subCategory in UnitsBySubcategory) {
                foreach (WorldSingleUnit unit in subCategory) {
                    // Debug.Log (" unit "+unit.GetUnitName());
                    if (unit.GetUnitCategory_DB().id == category.id) {
                        categoryObjects.Add(unit);
                    } else {
                        break;
                    }
                }
            }
            UnitsByCategory.Add(categoryObjects);
        }


        // Debug.Log ("WorldSingleUnit built");

        // This is to view each element and its category.
        // foreach (List<WorldSingleUnit> category in UnitsBySubcategory) {
        //     foreach (WorldSingleUnit unit in category) {
        //         Debug.Log (unit.GetUnitSubCategory()+" / "+unit.GetUnitName()+" / "+unit.GetUnitTeam());
        //     }
        // }
    }

    public static GameObject BuildUnit(WorldSingleUnit unit, Vector3 spawnPosition, Quaternion spawnRotation, bool aiMove, bool aiShoot, bool aiSpawn) {
        // MODEL
            GameObject spawnedUnitInstance =
                Instantiate(unit.GetUnitModel(), spawnPosition, spawnRotation);
        // RIGIDBODY
            Rigidbody RigidBody = spawnedUnitInstance.GetComponent<Rigidbody>();
            RigidBody.mass = unit.GetRigidBodyMass();
            RigidBody.drag = unit.GetRigidBodyDrag();
            RigidBody.angularDrag = unit.GetRigidBodyAngularDrag();
            RigidBody.useGravity = unit.GetRigidBodyUseGravity();
            RigidBody.isKinematic = unit.GetRigidBodyIsKinematic();
            RigidBody.interpolation = RigidbodyInterpolation.Interpolate;                                  // .None/Interpolate/Extrapolate Player controlled units should be in .Interpolate (less costly) or .Extrapolate (more accurate)
            RigidBody.collisionDetectionMode = CollisionDetectionMode.Continuous;                   // gameplay models should be in Continuous, bullets should be in .ContinuousDynamic for better results, by Unity documentation.
            if (unit.GetRigidBodyFreezePosition() && unit.GetRigidBodyFreezeRotation()) {
                RigidBody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            } else if (unit.GetRigidBodyFreezePosition()) {
                RigidBody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
            } else if (unit.GetRigidBodyFreezeRotation()) {
                RigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            }

        // SCRIPTS
            // UI
                UnitUI UnitUI = spawnedUnitInstance.AddComponent<UnitUI>();
            // HEALTH
                UnitHealth UnitHealth = spawnedUnitInstance.AddComponent<UnitHealth>();
                
            // CUSTOM SCRIPTS
            if (unit.GetUnitSubCategory_DB().Category.ScriptsList.Count > 0) {
                List<CompiledTypes.Scripts> sortedScriptList = new List<CompiledTypes.Scripts>(); 
                foreach (CompiledTypes.Scripts script in unit.GetUnitSubCategory_DB().Category.ScriptsList) {
                    sortedScriptList.Add(script);
                }
                sortedScriptList.Sort((IComparer<CompiledTypes.Scripts>)new sort());

                foreach (CompiledTypes.Scripts script in sortedScriptList) {
                    UnitParameter unitParameter = spawnedUnitInstance.AddComponent(Type.GetType(script.ScriptName)) as UnitParameter;
                    // spawnedUnitInstance.AddComponent<script.ScriptName>();
                }
            }

            // UNITCONTROLLER
                string unitControllerScript = unit.GetUnitSubCategory_DB().Category.FileName;
                UnitMasterController unitController = spawnedUnitInstance.AddComponent(Type.GetType(unitControllerScript)) as UnitMasterController;
                // Debug.Log("script Name for "+ unit.GetUnitName()+ " which is a " + unit.GetUnitReference_DB().UnitCategory.Category.id +"  is :"+ UnitControllerScript);


            // CAMERA
                TargetCameraParameters TCP = spawnedUnitInstance.AddComponent<TargetCameraParameters>();
                
        // Order is important, if Health is set after the unit controller, the health UI won't be correctly displayed 
        // HEALTH
            UnitHealth.SetCurrentHealth(unit.GetUnitHealth());
            UnitHealth.SetStartingHealth(unit.GetUnitHealth());
            UnitHealth.SetDeathFX(unit.GetUnitDeathFX());

        // CAMERA
            TCP.SetCameraDistance(unit.GetUnitCameraDistance());
            TCP.SetCameraHeight(unit.GetUnitCameraHeight());
            TCP.SetCameraLateralOffset(unit.GetUnitCameraCameraOffset());

        // COMMON DATA
            unitController.SetUnitModel(spawnedUnitInstance);
            unitController.SetUnitFromWorldUnitsManager(unit, aiMove, aiShoot, aiSpawn);

        // MAP
            GameObject mapUnitInstance =
                CreateNewUnitMapModel(spawnedUnitInstance, unit.GetUnitTeam_DB());
            unitController.SetMapModel(mapUnitInstance);


        return spawnedUnitInstance;
    }

    private class sort : IComparer<CompiledTypes.Scripts>{
        int IComparer<CompiledTypes.Scripts>.Compare(CompiledTypes.Scripts _scriptA, CompiledTypes.Scripts _scriptB) {
            int t1 = _scriptA.LoadOrder;
            int t2 = _scriptB.LoadOrder;
            return t1.CompareTo(t2);
        }
    }

    public static GameObject CreateNewUnitMapModel(GameObject unitGameObject, CompiledTypes.Teams team) {
        //Create the map element corresponding to the unit
        GameObject tempModel = Instantiate(WorldGlobals.BuildMapModel(unitGameObject.GetComponent<UnitMasterController>().GetUnitSubCategory_DB()), unitGameObject.transform);
        var Renderer = tempModel.GetComponent<Renderer>();
        Renderer.material.SetColor("_Color", SetColor(team));
        return tempModel;
    }

    public static Color SetColor(CompiledTypes.Teams team) {
        return new Color(team.TeamColorRed, team.TeamColorGreen, team.TeamColorBlue, 1f);
    }
}