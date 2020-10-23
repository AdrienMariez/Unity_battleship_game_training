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
    private CompiledTypes.Units_sub_categories UnitSubCategory_DB;
    private GameObject UnitDeathFX;         // If there were multiple of those, there could be multiple death FX
    private CompiledTypes.Countries Nation;
    private CompiledTypes.Teams.RowValues Team;
    private CompiledTypes.Teams Team_DB;
    private int UnitCommandPointsCost;
    private int UnitVictoryPointsValue;

    private float CameraDistance;
    private float CameraHeight;
    private float CameraCameraOffset;

    private float UnitHealth;

    private float UnitMass;

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
            }
            if (UnitPrefab == null) {
                Debug.Log (" A unit was implemented without a model ! Unit id :"+ unit.id);
                UnitPrefab = WorldUIVariables.GetErrorModel();
            }

        // NAME
            if (unit.Isavariant && String.IsNullOrEmpty(unit.UnitName)) {
                UnitName = masterUnitReference.UnitName;
            } else if (!String.IsNullOrEmpty(unit.UnitName)) {
                UnitName = unit.UnitName;
            }

        // CATEGORY && SUBCATEGORY && SUBCATEGORY RELATED FX
            // Debug.Log (UnitName+"?UnitCategoryEmpty? : "+String.IsNullOrEmpty(unit.UnitCategory.id)+" - ?unit.Isavariant? :  "+unit.Isavariant+" - unit.UnitCategory.id :  "+unit.UnitCategory.id);

            if (unit.Isavariant && unit.UnitCategory.id.ToString() == "Empty") {
                foreach (CompiledTypes.Units_sub_categories subCat in WorldUnitsManager.GetDB().Units_sub_categories.GetAll()) {
                    if (subCat.id == masterUnitReference.UnitCategory.id) {
                        // Debug.Log ("case1 found for "+UnitName);
                        UnitSubCategory_DB = subCat;
                        string subCatString = subCat.id.ToString();
                        UnitSubCategory = (CompiledTypes.Units_sub_categories.RowValues)System.Enum.Parse( typeof(CompiledTypes.Units_sub_categories.RowValues), subCatString);

                        string catString = subCat.Category.id.ToString();
                        UnitCategory = (CompiledTypes.Units_categories.RowValues)System.Enum.Parse( typeof(CompiledTypes.Units_categories.RowValues), catString );

                        string FXString = "FX/"+masterUnitReference.UnitCategory.DeathExplosion.FXPath+"/"+masterUnitReference.UnitCategory.DeathExplosion.FXPrefab;
                        UnitDeathFX = (Resources.Load(FXString, typeof(GameObject))) as GameObject;
                    }
                }
            } else if (unit.UnitCategory.id.ToString() != "Empty") {
                foreach (CompiledTypes.Units_sub_categories subCat in WorldUnitsManager.GetDB().Units_sub_categories.GetAll()) {
                    if (subCat.id == unit.UnitCategory.id) {
                        // Debug.Log ("case2 found for "+UnitName);
                        UnitSubCategory_DB = subCat;
                        string subCatString = subCat.id.ToString();
                        UnitSubCategory = (CompiledTypes.Units_sub_categories.RowValues)System.Enum.Parse( typeof(CompiledTypes.Units_sub_categories.RowValues), subCatString);
                        
                        string catString = subCat.Category.id.ToString();
                        UnitCategory = (CompiledTypes.Units_categories.RowValues)System.Enum.Parse( typeof(CompiledTypes.Units_categories.RowValues), catString );
                        
                        string FXString = "FX/"+unit.UnitCategory.DeathExplosion.FXPath+"/"+unit.UnitCategory.DeathExplosion.FXPrefab;
                        UnitDeathFX = (Resources.Load(FXString, typeof(GameObject))) as GameObject;
                    }
                }
            } else {
                Debug.Log ("No category found for "+UnitName);
            }
            // Debug.Log (UnitName+"UnitSubCategory : "+UnitSubCategory+" - UnitCategory :  "+UnitCategory+" - UnitDeathFX :  "+UnitDeathFX);
        // END CATEGORY

        // NATION
            if (unit.Isavariant && String.IsNullOrEmpty(unit.UnitNation.id)) {
                Nation = masterUnitReference.UnitNation;
                Team_DB = masterUnitReference.UnitNation.Team;
            } else if (!String.IsNullOrEmpty(unit.UnitNation.id)) {
                Nation = unit.UnitNation;
                Team_DB = unit.UnitNation.Team;
            }
            foreach (CompiledTypes.Teams.RowValues team in Enum.GetValues(typeof(CompiledTypes.Teams.RowValues))) {
                if (team.ToString() == Team_DB.id) {
                    Team = team;
                }
            }
            // Debug.Log (UnitName+" WSU Team : "+Team);
        // END NATION

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

        // HEALTH
            if (unit.Isavariant && unit.UnitHealth == 0) {
                UnitHealth = masterUnitReference.UnitHealth;
            } else if (unit.UnitHealth > 0) {
                UnitHealth = unit.UnitHealth;
            } else {
                Debug.Log (UnitName);
            }

        // DAMAGE CONTROL


        // RIGID BODY
            if (unit.Isavariant && unit.UnitMass == 0) {
                UnitMass = masterUnitReference.UnitMass;
            } else if (unit.UnitMass > 0) {
                UnitMass = unit.UnitMass;
            } else {
                Debug.Log (UnitName);
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
    public void SetUnitCategory(CompiledTypes.Units_categories.RowValues unitCategory){ UnitCategory = unitCategory; }
    public void SetUnitSubCategory(CompiledTypes.Units_sub_categories.RowValues unitSubCategory){ UnitSubCategory = unitSubCategory; }
    public CompiledTypes.Units_sub_categories.RowValues GetUnitSubCategory(){ return UnitSubCategory; }
    public CompiledTypes.Units_sub_categories GetUnitSubCategory_DB(){ return UnitSubCategory_DB; }
    public GameObject GetUnitDeathFX(){ return UnitDeathFX; }
    public CompiledTypes.Countries GetUnitNation(){ return Nation; }
    public CompiledTypes.Teams.RowValues GetUnitTeam(){ return Team; }
    public int GetUnitCommandPointsCost(){ return UnitCommandPointsCost; }
    public int GetUnitVictoryPointsValue(){ return UnitVictoryPointsValue; }
    public float GetUnitCameraDistance(){ return CameraDistance; }
    public float GetUnitCameraHeight(){ return CameraHeight; }
    public float GetUnitCameraCameraOffset(){ return CameraCameraOffset; }
    public float GetUnitHealth(){ return UnitHealth; }

    public float GetUnitMass(){ return UnitMass; }
}