using System;
using UnityEngine;

public class WorldUIVariables : MonoBehaviour {
    [Header("Player UI")]
        public GameObject m_TankUI;
        public GameObject m_PlaneUI;
        public GameObject m_ShipUI;
        public GameObject m_BuildingUI;
        public GameObject m_PlayerMapUI;
        public GameObject m_TurretUI;
        public GameObject m_PauseMenu;
    [Header("Turrets status icons")]
        public GameObject m_TurretStatusSprites;
        public float m_IconsSpacing = 22;
    [Header("Spawner UI")]
        public GameObject m_SpawnerUI;
        public GameObject m_SpawnerUnitSelect;
        public float m_SpawnerSpacing = 22;
    [Header("Shell Camera")]
        public GameObject m_ShellCamera;
        public float m_TimeToDestroyCamera = 3;
    [Header("Units UI")]
        public GameObject m_UnitUI;
        public GameObject m_UnitMapUI;
    [Header("Damage Control UI")]
        public GameObject m_ShipDamageControlUI;
        public GameObject m_ShipDamageControlAlertUI;
    [Header("Map Models")]
        public MapModelsList[] m_MapModels;
        public GameObject m_MapErrorModel;

/////////////////////////////////////////////////////////

    [Header("Static layer UI")]
        private static GameObject TankUI;
        private static GameObject PlaneUI;
        private static GameObject ShipUI;
        private static GameObject BuildingUI;
        private static GameObject PlayerMapUI;
        private static GameObject TurretUI;
        private static GameObject PauseMenu;
    [Header("Static Turrets status icons")]
        private static GameObject TurretStatusSprites;
        private static float IconsSpacing = 22;
    [Header("Static Spawner UI")]
        private static GameObject SpawnerUI;
        private static GameObject SpawnerUnitSelect;
        private static float SpawnerSpacing = 22;
    [Header("Static Shell Camera")]
        private static GameObject ShellCamera;
        private static float TimeToDestroyCamera = 3;
    [Header("Static Units UI")]
        private static GameObject UnitUI;
        private static GameObject UnitMapUI;
    [Header("Static Damage Control UI")]
        private static GameObject ShipDamageControlUI;
        private static GameObject ShipDamageControlAlertUI;
    [Header("Static Map Models")]
        private static MapModelsList[] MapModels;
        private static GameObject MapErrorModel;

    private static bool FirstLoad = true;
    private void Start() {
        if (FirstLoad){
            FirstLoad = false;
            WorldSetUI();
        }
    }
    private void WorldSetUI() {
        TankUI = m_TankUI;
        PlaneUI = m_PlaneUI;
        ShipUI = m_ShipUI;
        BuildingUI = m_BuildingUI;
        PlayerMapUI = m_PlayerMapUI;
        TurretUI = m_TurretUI;
        PauseMenu = m_PauseMenu;

        TurretStatusSprites = m_TurretStatusSprites;
        IconsSpacing = m_IconsSpacing;

        SpawnerUI = m_SpawnerUI;
        SpawnerUnitSelect = m_SpawnerUnitSelect;
        SpawnerSpacing = m_SpawnerSpacing;

        ShellCamera = m_ShellCamera;
        TimeToDestroyCamera = m_TimeToDestroyCamera;

        UnitUI = m_UnitUI;
        UnitMapUI = m_UnitMapUI;

        ShipDamageControlUI = m_ShipDamageControlUI;
        ShipDamageControlAlertUI = m_ShipDamageControlAlertUI;

        MapModels = m_MapModels;
        MapErrorModel = m_MapErrorModel;
    }
    public static GameObject GetTankUI() { return TankUI; }
    public static GameObject GetPlaneUI() { return PlaneUI; }
    public static GameObject GetShipUI() { return ShipUI; }
    public static GameObject GetBuildingUI() { return BuildingUI; }
    public static GameObject GetPlayerMapUI() { return PlayerMapUI; }
    public static GameObject GetTurretUI() { return TurretUI; }
    public static GameObject GetPauseMenu() { return PauseMenu; }

    public static GameObject GetTurretStatusSprites() { return TurretStatusSprites; }
    public static float GetIconsSpacing() { return IconsSpacing; }

    public static GameObject GetSpawnerUI() { return SpawnerUI; }
    public static GameObject GetSpawnerUnitSelect() { return SpawnerUnitSelect; }
    public static float GetSpawnerSpacing() { return SpawnerSpacing; }

    public static GameObject GetShellCamera() { return ShellCamera; }
    public static float GetTimeToDestroyCamera() { return TimeToDestroyCamera; }

    public static GameObject GetShipDamageControlUI() { return ShipDamageControlUI; }
    public static GameObject GetShipDamageControlAlertUI() { return ShipDamageControlAlertUI; }

    public static GameObject GetUnitUI() { return UnitUI; }
    public static GameObject GetUnitMapUI() { return UnitMapUI; }

    public static GameObject BuildMapModel(WorldUnitsManager.UnitSubCategories unitCategory) {
        string unitCat = unitCategory.ToString();
        foreach (var mapModel in MapModels) {
            if (mapModel.m_Name == unitCat) {
                return mapModel.m_MapModel;
            }
        }
        return MapErrorModel;
    }

    [Serializable]
    public class MapModelsList {
        public string m_Name;

        public GameObject m_MapModel;
    }

}