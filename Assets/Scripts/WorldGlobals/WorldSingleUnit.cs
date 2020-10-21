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
    private CompiledTypes.Units_categories.RowValues UnitCategory;
    private CompiledTypes.Units_sub_categories.RowValues UnitSubCategory;
    private CompiledTypes.Countries.RowValues Nation;
    private CompiledTypes.Teams.RowValues Team;
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
                foreach (CompiledTypes.Units_categories.RowValues row in Enum.GetValues(typeof(CompiledTypes.Units_categories.RowValues))) {
                    if (row.ToString() == masterUnitReference.UnitCategory.Category.id) {
                        UnitCategory = row;
                    }
                }
                foreach (CompiledTypes.Units_sub_categories.RowValues row in Enum.GetValues(typeof(CompiledTypes.Units_sub_categories.RowValues))) {
                    if (row.ToString() == masterUnitReference.UnitCategory.id) {
                        UnitSubCategory = row;
                    }
                }
            } else if (!String.IsNullOrEmpty(unit.UnitCategory.id)) {
                foreach (CompiledTypes.Units_categories.RowValues row in Enum.GetValues(typeof(CompiledTypes.Units_categories.RowValues))) {
                    if (row.ToString() == unit.UnitCategory.Category.id) {
                        UnitCategory = row;
                    }
                }
                foreach (CompiledTypes.Units_sub_categories.RowValues row in Enum.GetValues(typeof(CompiledTypes.Units_sub_categories.RowValues))) {
                    if (row.ToString() == unit.UnitCategory.id) {
                        UnitSubCategory = row;
                    }
                }
                // UnitCategory = unit.UnitCategory.Category.id;
                // UnitSubCategory = unit.UnitCategory.id;
            }

        // NATION
            if (unit.Isavariant && String.IsNullOrEmpty(unit.UnitNation.id)) {
                foreach (CompiledTypes.Countries.RowValues row in Enum.GetValues(typeof(CompiledTypes.Countries.RowValues))) {
                    if (row.ToString() == masterUnitReference.UnitNation.id) {
                        Nation = row;
                    }
                }
                foreach (CompiledTypes.Teams.RowValues row in Enum.GetValues(typeof(CompiledTypes.Teams.RowValues))) {
                    if (row.ToString() == masterUnitReference.UnitNation.Team.id) {
                        Team = row;
                    }
                }
            } else if (!String.IsNullOrEmpty(unit.UnitNation.id)) {
                foreach (CompiledTypes.Countries.RowValues row in Enum.GetValues(typeof(CompiledTypes.Countries.RowValues))) {
                    if (row.ToString() == unit.UnitNation.id) {
                        Nation = row;
                    }
                }
                foreach (CompiledTypes.Teams.RowValues row in Enum.GetValues(typeof(CompiledTypes.Teams.RowValues))) {
                    if (row.ToString() == unit.UnitNation.Team.id) {
                        Team = row;
                    }
                }
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
    public CompiledTypes.Units_categories.RowValues GetUnitCategory(){ return UnitCategory; }
    public CompiledTypes.Units_sub_categories.RowValues GetUnitSubCategory(){ return UnitSubCategory; }
    public CompiledTypes.Countries.RowValues GetUnitNation(){ return Nation; }
    public CompiledTypes.Teams.RowValues GetUnitTeam(){ return Team; }
    public int GetUnitCommandPointsCost(){ return UnitCommandPointsCost; }
    public int GetUnitVictoryPointsValue(){ return UnitVictoryPointsValue; }
    public float GetUnitCameraDistance(){ return CameraDistance; }
    public float GetUnitCameraHeight(){ return CameraHeight; }
    public float GetUnitCameraCameraOffset(){ return CameraCameraOffset; }
}