using UnityEngine;
using System.Collections;

public class WorldUnitsManager : MonoBehaviour {

    // public WorldCategoriesUnitsManager[] m_UnitsCategories;
    // public WorldSingleUnitManager[] m_WorldUnits;

    // TODO See if useless :
    /*public WorldSingleShipUnit[] m_WorldShipsUnits;
    public WorldSingleSubmarineUnit[] m_WorldSubmarinesUnits;
    public WorldSinglePlaneUnit[] m_WorldPlanesUnits;
    public WorldSingleGroundUnit[] m_WorldGroundUnits;
        public enum ShipSubCategories {
        battleship,
        carrier,
        cruiser,
        destroyer
    }
    [Header("Ships map models")]
        public GameObject ShipBattleship;
        public GameObject ShipCarrier, ShipCruiser, ShipDestroyer;
    public enum SubmarineSubCategories {
        submarine
    }
    [Header("Submarines map models")]
        public GameObject SubmarineSubmarine;
    public enum PlaneSubCategories {
        fighter,
        lightBomber,
        Bomber
    }
    [Header("Planes map models")]
        public GameObject PlaneFighter;
        public GameObject PlaneLightBomber, PlaneBomber;
    public enum GroundSubCategories {
        GroundTank
    }
    [Header("Ground map models")]
        public GameObject GroundTank;
    public enum BuildingSubCategories {
        landBase,
        shipyard,
        airfield
    }
    [Header("Buildings map models")]
        public GameObject BuildingLandBase;
        public GameObject BuildingShipyard, BuildingAirfield;*/
    // END TODO


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

    private static bool FirstLoad = true;
    void Start() {
        if (FirstLoad){
            FirstLoad = false;
            Debug.Log ("WorldSetUnits");
        }
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