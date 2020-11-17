using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldGlobals : MonoBehaviour {
    [Header("Shell Camera")]
        public GameObject m_ShellCamera; private static GameObject ShellCamera; public static GameObject GetShellCamera() { return ShellCamera; }
        public float m_TimeToDestroyCamera = 3; private static float TimeToDestroyCamera = 3; public static float GetTimeToDestroyCamera() { return TimeToDestroyCamera; }
    [Header("Menu Map Camera")]
        public GameObject m_MenuMapCamera; private static GameObject MenuMapCamera; public static GameObject GetMenuMapCamera() { return MenuMapCamera; }
    [Header("Map")]
        public GameObject m_MapPattern; private static GameObject MapPattern; public static GameObject GetMapPattern() { return MapPattern; }
        public GameObject m_MapCamera; private static GameObject MapCamera; public static GameObject GetMapCamera() { return MapCamera; }
    [Header("Gaming Area")]
        public int m_GamingAreaKillZonePadding = 200; private static int GamingAreaKillZonePadding = 200; public static int GetGamingAreaKillZonePadding() { return GamingAreaKillZonePadding; }
    [Header("Error Placeholders")]
        public GameObject m_ErrorModel; private static GameObject ErrorModel; public static GameObject GetErrorModel() { return ErrorModel; }
        public AudioClip m_ErrorSound; private static AudioClip ErrorSound; public static AudioClip GetErrorSound() { return ErrorSound; }

    /////////////////////////////////////////////////////////   
    private static bool FirstLoad = true; public static bool GetFirstLoad() { return FirstLoad; }
    private void Awake() {
        if (FirstLoad){
            FirstLoad = false;
            WorldSetGlobals();
        }
    }
    private void WorldSetGlobals() {
        // Shell Camera
            ShellCamera = m_ShellCamera; if (ShellCamera == null) { Debug.Log ("No ShellCamera found."); }
            TimeToDestroyCamera = m_TimeToDestroyCamera;
        // Menu Map Camera
            MenuMapCamera = m_MenuMapCamera; if (MenuMapCamera == null) { Debug.Log ("No MenuMapCamera found."); }
        // Map
            MapPattern = m_MapPattern; if (MapPattern == null) { Debug.Log ("No MapPattern found."); }
            MapCamera = m_MapCamera; if (MapCamera == null) { Debug.Log ("No MapCamera found."); }
        // Gaming Area
            GamingAreaKillZonePadding = m_GamingAreaKillZonePadding;
        // Error Models
            ErrorModel = m_ErrorModel; if (ErrorModel == null) { Debug.Log ("No ErrorModel found."); }
            ErrorSound = m_ErrorSound; if (ErrorSound == null) { Debug.Log ("No ErrorSound found."); }
    }

    public static GameObject BuildMapModel(CompiledTypes.Units_sub_categories unitCategory) {
        GameObject mapModel = (Resources.Load("Prefabs/MapModels/"+unitCategory.MapModel, typeof(GameObject))) as GameObject;
        if (mapModel != null) {
            // Debug.Log ("BuildMapModel Worked !");
            return mapModel;
        } else {
            // Debug.Log (" WUIV BuildMapModel NOT FOUND  "+unitCategory.id +" - "+ unitCategory.MapModel);
            return ErrorModel;
        }
    }
}