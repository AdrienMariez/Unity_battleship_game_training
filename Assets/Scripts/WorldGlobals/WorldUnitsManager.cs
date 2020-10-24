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
    private static CastleDB DB;
    
    public static CastleDB GetDB(){ return DB; }

    private static bool FirstLoad = true;
    private void Start() {
        if (FirstLoad){
            FirstLoad = false;
            DB = new CastleDB(CastleDBAsset);
            WorldSetUnits();
        }
    }
    
    // static List<UnitSubCategories> SubCategories = new List<UnitSubCategories>();
    private static List<List<WorldSingleUnit>> UnitsBySubcategory = new List<List<WorldSingleUnit>>();
    public static List<List<WorldSingleUnit>> GetUnitsBySubcategory() { return UnitsBySubcategory; }

    static List<CompiledTypes.Units_sub_categories> SubCategoriesDB = new List<CompiledTypes.Units_sub_categories>();
    
    private void WorldSetUnits() {

        // Get one element from an asset
        // Units_categories ship = DB.Units_categories.ship;
        // string shipName = ship.Name;
        // Debug.Log (shipName);

        
        // Get all elements in a list 
        // foreach (CompiledTypes.Teams team in DB.Teams.GetAll()) {
        //     TeamsDB.Add(team);
        // }

        // foreach (CompiledTypes.Global_Units unit in DB.Global_Units.GetAll()) {
        //     Debug.Log (unit.id+" - "+unit.UnitName+" - "+unit.UnitNation.Team.Name);
        //     foreach (var weapon in unit.UnitweaponsList) {
        //         Debug.Log (" - "+weapon.Type.id);
        //     }
        // }

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

        // This is to view each element and its category.
        // foreach (List<WorldSingleUnit> category in UnitsBySubcategory) {
        //     foreach (WorldSingleUnit unit in category) {
        //         Debug.Log (unit.GetUnitSubCategory()+" / "+unit.GetUnitName()+" / "+unit.GetUnitTeam());
        //     }
        // }
    }

    /*public static void SetFirstLoad(bool firstLoad) {
        FirstLoad = firstLoad;
    }
    public static bool GetFirstLoad() {
        return FirstLoad;
    }*/

    protected void Update() { }

    public static GameObject BuildUnit(WorldSingleUnit unit, Vector3 spawnPosition, Quaternion spawnRotation) {
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
            RigidBody.interpolation = RigidbodyInterpolation.None;                                  // Player controlled units should be in .Interpolate (less costly) or .Extrapolate (more accurate)
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

        // COMMON DATA
            unitController.SetUnitFromWorldUnitsManager(unit);

        // HEALTH
            UnitHealth.SetCurrentHealth(unit.GetUnitHealth());
            UnitHealth.SetStartingHealth(unit.GetUnitHealth());
            UnitHealth.SetDeathFX(unit.GetUnitDeathFX());
        // CAMERA
            TCP.SetCameraDistance(unit.GetUnitCameraDistance());
            TCP.SetCameraHeight(unit.GetUnitCameraHeight());
            TCP.SetCameraLateralOffset(unit.GetUnitCameraCameraOffset());
        




        // WorldUnitsManager.CreateNewUnitMapModel(spawnedUnitInstance, unit.GetUnitTeam());        // Commented for now
        return spawnedUnitInstance;
    }

    private class sort : IComparer<CompiledTypes.Scripts>{
        int IComparer<CompiledTypes.Scripts>.Compare(CompiledTypes.Scripts _scriptA, CompiledTypes.Scripts _scriptB) {
            int t1 = _scriptA.LoadOrder;
            int t2 = _scriptB.LoadOrder;
            return t1.CompareTo(t2);
        }
    }

    public static void CreateNewUnitMapModel(GameObject unitGameObject, CompiledTypes.Teams.RowValues team) {
        //Create the map element corresponding to the unit
        GameObject tempModel = Instantiate(WorldUIVariables.BuildMapModel(unitGameObject.GetComponent<UnitMasterController>().GetUnitSubCategory_DB()), unitGameObject.transform);
        var Renderer = tempModel.GetComponent<Renderer>();
        Renderer.material.SetColor("_Color", SetColor(team));
    }

    public static Color SetColor(CompiledTypes.Teams.RowValues team) {
        Color color = Color.yellow;
        foreach (CompiledTypes.Teams globalTeam in DB.Teams.GetAll()) {
            if (team.ToString() == globalTeam.id) {
                color = new Color(globalTeam.TeamColorRed, globalTeam.TeamColorGreen, globalTeam.TeamColorBlue, 1f);
            }
        }
        return color;
    }
}