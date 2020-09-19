using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldUnitsManager : MonoBehaviour {

    [Header("Add all units of the game to this list !")]
    public WorldSingleUnit[] m_WorldSingleUnit;

    public enum SimpleTeams {
        Allies,
        Axis,
        NeutralAI
    }
    public enum Teams {
        Allies,
        AlliesAI,
        Axis,
        AxisAI,
        NeutralAI
    }
    public enum Nations {
        US,
        Japan,
        GB,
        Germany,
        USSR,
        China,
        France
    }

    public enum UnitCategories {
        ship,
        submarine,
        plane,
        ground,
        building
    }
    public enum UnitSubCategories {
        ShipBattleship,ShipCarrier,ShipCruiser,ShipDestroyer,
        SubmarineSubmarine,
        PlaneFighter,PlaneLightBomber,PlaneBomber,
        GroundTank,
        BuildingLandBase,BuildingShipyard,BuildingAirfield
    }
        
    private GameObject TempModel;
    private GameObject TempMapModel;

    /*[Serializable] public class VariableHolder
    {
        public bool var1;
        public float var2 = 150f;
        public float var3 = 25f;
    }
 
    public VariableHolder instance = new VariableHolder();*/

    private static bool FirstLoad = true;
    private void Start() {
        if (FirstLoad){
            FirstLoad = false;
            WorldSetUnits();
        }
    }
    
    // List<StarSystem> starSystems = new List<StarSystem>();
    // List<Planet> planets = new List<Planet>();
    // List<Sat> satellites = new List<Sat>();
    static List<UnitSubCategories> SubCategories = new List<UnitSubCategories>();
    private static List<List<WorldSingleUnit>> UnitsBySubcategory = new List<List<WorldSingleUnit>>();

    public List<List<WorldSingleUnit>> GetUnitsBySubcategory() { return UnitsBySubcategory; }
    
    private void WorldSetUnits() {
        foreach(UnitSubCategories category in Enum.GetValues(typeof(UnitSubCategories))) {
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
        }

        // This is to view each element and its category.
        /*foreach (List<WorldSingleUnit> category in UnitsBySubcategory) {
            foreach (WorldSingleUnit unit in category) {
                Debug.Log (unit.GetUnitSubCategory());
                Debug.Log (unit.GetUnitName());
            }
        }*/
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

    public void CreateNewUnit(GameObject unitGameObject, Teams team) {
        //Create the map element corresponding to the unit
        TempModel = Instantiate(unitGameObject.GetComponent<UnitMasterController>().GetUnitMapModel(), unitGameObject.transform);
        var Renderer = TempModel.GetComponent<Renderer>();
        Renderer.material.SetColor("_Color", SetColor(team));
    }

    public static Color SetColor(WorldUnitsManager.Teams team) {
        Color color = Color.yellow;;
        if (team == WorldUnitsManager.Teams.Allies) {
            color = new Color(0f, 0.47f, 1f, 1f);
        } else if (team == WorldUnitsManager.Teams.AlliesAI) {
            color = new Color(0f, 0.1f, 1f, 1f);
        } else if (team == WorldUnitsManager.Teams.Axis) {
            color = new Color(1f, 0.22f, 0.29f, 1f);
        } else if (team == WorldUnitsManager.Teams.AxisAI) {
            color = new Color(1f, 0.0f, 0.0f, 0.49f);
        } else if (team == WorldUnitsManager.Teams.NeutralAI) {
            color = Color.yellow;
        }
        return color;
    }
}