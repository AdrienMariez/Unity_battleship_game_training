using System;
using UnityEngine;

public class WorldUIVariables : MonoBehaviour {
    [Header("Menu UI")]
        public GameObject m_SpawnPointUI; private static GameObject SpawnPointUI; public static GameObject GetSpawnPointUI() { return SpawnPointUI; }
    [Header("Player UI")]
        public GameObject m_TankUI; private static GameObject TankUI; public static GameObject GetTankUI() { return TankUI; }
        public GameObject m_PlaneUI; private static GameObject PlaneUI;public static GameObject GetPlaneUI() { return PlaneUI; }
        public GameObject m_ShipUI; private static GameObject ShipUI;public static GameObject GetShipUI() { return ShipUI; }
        public GameObject m_BuildingUI; private static GameObject BuildingUI;public static GameObject GetBuildingUI() { return BuildingUI; }
        public GameObject m_PlayerMapUI; private static GameObject PlayerMapUI;public static GameObject GetPlayerMapUI() { return PlayerMapUI; }
        public GameObject m_TurretUI; private static GameObject TurretUI;public static GameObject GetTurretUI() { return TurretUI; }
        public GameObject m_PauseMenu; private static GameObject PauseMenu;public static GameObject GetPauseMenu() { return PauseMenu; }
    [Header("Turrets status icons")]
        public GameObject m_TurretStatusSprites; private static GameObject TurretStatusSprites; public static GameObject GetTurretStatusSprites() { return TurretStatusSprites; }
        public float m_IconsSpacing = 22; private static float IconsSpacing = 22; public static float GetIconsSpacing() { return IconsSpacing; }
    [Header("Spawner UI")]
        public GameObject m_SpawnerUI; private static GameObject SpawnerUI; public static GameObject GetSpawnerUI() { return SpawnerUI; }
        public GameObject m_SpawnerUnitSelect; private static GameObject SpawnerUnitSelect; public static GameObject GetSpawnerUnitSelect() { return SpawnerUnitSelect; }
        public float m_SpawnerSpacing = 22; private static float SpawnerSpacing = 22; public static float GetSpawnerSpacing() { return SpawnerSpacing; }
    [Header("Shell Decal")]
        public GameObject m_ShellDecal; private static GameObject ShellDecal; public static GameObject GetShellDecal() { return ShellDecal; }
    [Header("Units UI")]
        public GameObject m_UnitUI; private static GameObject UnitUI; public static GameObject GetUnitUI() { return UnitUI; }
        public GameObject m_UnitMapUI; private static GameObject UnitMapUI; public static GameObject GetUnitMapUI() { return UnitMapUI; }
    [Header("Map UI")]
        public GameObject m_MapOrderCanvas; private static GameObject MapOrderCanvas; public static GameObject GetMapOrderCanvas() { return MapOrderCanvas; }
        public GameObject m_MapOrderModel; private static GameObject MapOrderModel; public static GameObject GetMapOrderModel() { return MapOrderModel; }
    [Header("Damage Control UI")]
        public GameObject m_ShipDamageControlUI; private static GameObject ShipDamageControlUI; public static GameObject GetShipDamageControlUI() { return ShipDamageControlUI; }
        public GameObject m_ShipDamageControlAlertUI; private static GameObject ShipDamageControlAlertUI; public static GameObject GetShipDamageControlAlertUI() { return ShipDamageControlAlertUI; }

/////////////////////////////////////////////////////////   
    private static bool FirstLoad = true; public static bool GetFirstLoad() { return FirstLoad; }
    private void Awake() {
        if (FirstLoad){
            FirstLoad = false;
            // Debug.Log ("WorldUIVariables FirstLoad");
            WorldSetUI();
        }
    }
    private void WorldSetUI() {
        // Menu UI
            SpawnPointUI = m_SpawnPointUI; if (SpawnPointUI == null) { Debug.Log ("No SpawnPointUI found."); }
        // Player UI
            TankUI = m_TankUI; if (TankUI == null) { Debug.Log ("No TankUI found."); }
            PlaneUI = m_PlaneUI; if (PlaneUI == null) { Debug.Log ("No PlaneUI found."); }
            ShipUI = m_ShipUI; if (ShipUI == null) { Debug.Log ("No ShipUI found."); }
            BuildingUI = m_BuildingUI; if (BuildingUI == null) { Debug.Log ("No BuildingUI found."); }
            PlayerMapUI = m_PlayerMapUI; if (PlayerMapUI == null) { Debug.Log ("No PlayerMapUI found."); }
            TurretUI = m_TurretUI; if (TurretUI == null) { Debug.Log ("No TurretUI found."); }
            PauseMenu = m_PauseMenu; if (PauseMenu == null) { Debug.Log ("No PauseMenu found."); }

        // Turrets status icons
            TurretStatusSprites = m_TurretStatusSprites; if (TurretStatusSprites == null) { Debug.Log ("No TurretStatusSprites found."); }
            IconsSpacing = m_IconsSpacing;

        // Spawner UI
            SpawnerUI = m_SpawnerUI; if (SpawnerUI == null) { Debug.Log ("No SpawnerUI found."); }
            SpawnerUnitSelect = m_SpawnerUnitSelect; if (SpawnerUnitSelect == null) { Debug.Log ("No SpawnerUnitSelect found."); }
            SpawnerSpacing = m_SpawnerSpacing;

        // Shell Decal
            ShellDecal = m_ShellDecal; if (ShellDecal == null) { Debug.Log ("No ShellDecal found."); }

        // Units UI
            UnitUI = m_UnitUI; if (UnitUI == null) { Debug.Log ("No UnitUI found."); }
            UnitMapUI = m_UnitMapUI; if (UnitMapUI == null) { Debug.Log ("No UnitMapUI found."); }

        // Map
            MapOrderCanvas = m_MapOrderCanvas; if (MapOrderCanvas == null) { Debug.Log ("No MapOrderCanvas found."); }
            MapOrderModel = m_MapOrderModel; if (MapOrderModel == null) { Debug.Log ("No MapOrderModel found."); }

        // Damage Control UI
            ShipDamageControlUI = m_ShipDamageControlUI; if (ShipDamageControlUI == null) { Debug.Log ("No ShipDamageControlUI found."); }
            ShipDamageControlAlertUI = m_ShipDamageControlAlertUI; if (ShipDamageControlAlertUI == null) { Debug.Log ("No ShipDamageControlAlertUI found."); }
    }
}