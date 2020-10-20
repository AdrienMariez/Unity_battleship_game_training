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


    // public enum SimpleTeams {
    //     Allies,
    //     Axis,
    //     NeutralAI
    // }
    // public enum Teams {
    //     Allies,
    //     AlliesAI,
    //     Axis,
    //     AxisAI,
    //     NeutralAI
    // }
    // public enum Nations {
    //     US,
    //     Japan,
    //     GB,
    //     Germany,
    //     USSR,
    //     China,
    //     France
    // }
    // Australia,China,France,GreatBritain,Germany,Italy,Japan,NewZealand,USA,USSR
    // public enum UnitCategories {
    //     ship,
    //     submarine,
    //     plane,
    //     ground,
    //     building
    // }
    // See https://en.wikipedia.org/wiki/Hull_classification_symbol
    // public enum UnitSubCategories {
    //     ShipBattleship,ShipCarrier,ShipCruiser,ShipDestroyer,
    //     SubmarineSubmarine,
    //     PlaneFighter,PlaneLightBomber,PlaneBomber,
    //     GroundTank,
    //     BuildingLandBase,BuildingShipyard,BuildingAirfield
    // }

    // Carrier,Battleship,Cruiser,Destroyer,Submarine,Fighter,LightBomber,Bomber,Tank,LandBase,Shipyard,Airfield
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
                if (unit.Isavariant && String.IsNullOrEmpty(unit.UnitCategory.id)) {
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
        }





        // Old system using m_WorldSingleUnit
        /*foreach(UnitSubCategories category in Enum.GetValues(typeof(UnitSubCategories))) {
            SubCategories.Add(category);
        }

        foreach (UnitSubCategories category in SubCategories) {
            List<WorldSingleUnit> categoryObjects = new List<WorldSingleUnit>();
            foreach (WorldSingleUnit unit in m_WorldSingleUnit) {
                if (unit.m_UnitPrefab.GetComponent<UnitMasterController>()) {
                    if (unit.m_UnitPrefab.GetComponent<UnitMasterController>().m_UnitSubCategory == category) {
                        unit.SetUnit();
                        categoryObjects.Add(unit);
                        // Debug.Log (category +" - "+ unit.m_UnitPrefab);
                    }
                } else {
                    Debug.Log (" A unit without a UnitMasterController was found ! It will not be used in the listings ! Error situated in : "+ unit.m_UnitPrefab);
                }
            }
            UnitsBySubcategory.Add(categoryObjects);
        }*/

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
    }
    public static void SetUnitList() {
        // This is set once in the whole game session !
        // WorldSetMapInstances();
        // Debug.Log ("Test");
    }*/

    protected void Update() { }

    public static GameObject BuildUnit(WorldSingleUnit unit, Vector3 spawnPosition, Quaternion spawnRotation) {
        // MODEL
            GameObject spawnedUnitInstance =
                Instantiate(unit.GetUnitModel(), spawnPosition, spawnRotation);

        // SCRIPTS
            // MANAGER
                string UnitControllerScript = unit.GetUnitReference_DB().UnitCategory.Category.FileName +"Controller";
                UnitMasterController UnitController = spawnedUnitInstance.AddComponent(Type.GetType(UnitControllerScript)) as UnitMasterController;

            // CAMERA
                TargetCameraParameters TCP = spawnedUnitInstance.AddComponent<TargetCameraParameters>();

        // NAME
        

        // CATEGORY && SUBCATEGORY

        // NATION

        // SCORES

        // CAMERA
            TCP.SetCameraDistance(unit.GetUnitCameraDistance());
            TCP.SetCameraHeight(unit.GetUnitCameraHeight());
            TCP.SetCameraLateralOffset(unit.GetUnitCameraCameraOffset());
        




        // WorldUnitsManager.CreateNewUnitMapModel(spawnedUnitInstance, unit.GetUnitTeam());        // Commented for now, as
        return spawnedUnitInstance;
    }

    public static void CreateNewUnitMapModel(GameObject unitGameObject, CompiledTypes.Teams.RowValues team) {
        //Create the map element corresponding to the unit
        GameObject tempModel = Instantiate(WorldUIVariables.BuildMapModel(unitGameObject.GetComponent<UnitMasterController>().GetUnitSubCategory()), unitGameObject.transform);
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