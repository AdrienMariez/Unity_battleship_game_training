using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class WorldSingleUnit {
    // Each parameter is built at the first loading of the game from each prefab added in WorldUnitsManager.
    // Those 
    // public GameObject m_UnitPrefab;
    private GameObject UnitPrefab;
    private CompiledTypes.Global_Units UnitReference_DB;

    // Same as in UnitMasterController !!
    private string UnitName;
    private string UnitCategory;
    private string UnitSubCategory;
    private string Nation;
    private string Team;
    private int UnitCommandPointsCost;
    private int UnitVictoryPointsValue;

    private float CameraDistance;
    private float CameraHeight;
    private float CameraCameraOffset;

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
            } else { Debug.Log (" A unit was implemented without a model ! Unit id :"+ unit.id); }

        // NAME
            if (unit.Isavariant && String.IsNullOrEmpty(unit.UnitName)) {
                UnitName = masterUnitReference.UnitName;
            } else if (!String.IsNullOrEmpty(unit.UnitName)) {
                UnitName = unit.UnitName;
            }

        // CATEGORY && SUBCATEGORY
            if (unit.Isavariant && String.IsNullOrEmpty(unit.UnitCategory.id)) {
                UnitCategory = masterUnitReference.UnitCategory.Category.id;
                UnitSubCategory = masterUnitReference.UnitCategory.id;
            } else if (!String.IsNullOrEmpty(unit.UnitCategory.id)) {
                UnitCategory = unit.UnitCategory.Category.id;
                UnitSubCategory = unit.UnitCategory.id;
            }

        // NATION
            if (unit.Isavariant && String.IsNullOrEmpty(unit.UnitNation.id)) {
                Nation = masterUnitReference.UnitNation.id;
                Team = masterUnitReference.UnitNation.Team.id;
            } else if (!String.IsNullOrEmpty(unit.UnitNation.id)) {
                Nation = unit.UnitNation.id;
                Team = unit.UnitNation.Team.id;
            }

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



        // foreach (var category in WorldUnitsManager.GetDB().Units_categories.GetAll()) {
        //     if (category.id == UnitCategory) {   
                // string controller = category.ScriptPaths+"/"+category.FileName+"Controller";
                // UnitPrefab.AddComponent(Type.GetType(controller));
                // UnitPrefab.AddComponent<ScriptName>();
        //     }
        // }

        // Debug.Log (CameraHeight+" / "+CameraDistance);
    }
    public GameObject GetUnitModel(){ return UnitPrefab; }
    public CompiledTypes.Global_Units GetUnitReference_DB(){ return UnitReference_DB; }
    public string GetUnitName(){ return UnitName; }
    public string GetUnitCategory(){ return UnitCategory; }
    public string GetUnitSubCategory(){ return UnitSubCategory; }
    public string GetUnitTeam(){ return Team; }
    public string GetUnitNation(){ return Nation; }
    public int GetUnitCommandPointsCost(){ return UnitCommandPointsCost; }
    public int GetUnitVictoryPointsValue(){ return UnitVictoryPointsValue; }
    public float GetUnitCameraDistance(){ return CameraDistance; }
    public float GetUnitCameraHeight(){ return CameraHeight; }
    public float GetUnitCameraCameraOffset(){ return CameraCameraOffset; }
}