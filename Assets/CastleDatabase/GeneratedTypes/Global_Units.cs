
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class Global_Units
    {
        public string id;
public bool Isavariant;
public List<UnitVariantReference> UnitVariantReferenceList = new List<UnitVariantReference>();
public string UnitName;
public string UnitPath;
public string UnitModel;
public Units_sub_categories UnitCategory;
public Countries UnitNation;
public List<UnitScore> UnitScoreList = new List<UnitScore>();
public List<UnitCameraParameters> UnitCameraParametersList = new List<UnitCameraParameters>();
public float UnitHealth;
public float UnitMass;
public List<UnitDamagecontrol> UnitDamagecontrolList = new List<UnitDamagecontrol>();
public List<Buoyancy> BuoyancyList = new List<Buoyancy>();
public List<Unitweapons> UnitweaponsList = new List<Unitweapons>();

        public enum RowValues { 
fuso, 
takao, 
fusoUSA, 
takaoUSA, 
TrainingCarrier, 
TrainingCarrierUSA, 
ShipyardJapan, 
ShipyardUSA
 } 
        public Global_Units (CastleDBParser.RootNode root, RowValues line) 
        {
            SimpleJSON.JSONNode node = root.GetSheetWithName("Global_Units").Rows[(int)line];
id = node["id"];
Isavariant = node["Isavariant"].AsBool;
foreach(var item in node["UnitVariantReference"]) { UnitVariantReferenceList.Add(new UnitVariantReference(root, item));}
UnitName = node["UnitName"];
UnitPath = node["UnitPath"];
UnitModel = node["UnitModel"];
UnitCategory = new CompiledTypes.Units_sub_categories(root,CompiledTypes.Units_sub_categories.GetRowValue(node["UnitCategory"]));
UnitNation = new CompiledTypes.Countries(root,CompiledTypes.Countries.GetRowValue(node["UnitNation"]));
foreach(var item in node["UnitScore"]) { UnitScoreList.Add(new UnitScore(root, item));}
foreach(var item in node["UnitCameraParameters"]) { UnitCameraParametersList.Add(new UnitCameraParameters(root, item));}
UnitHealth = node["UnitHealth"].AsFloat;
UnitMass = node["UnitMass"].AsFloat;
foreach(var item in node["UnitDamagecontrol"]) { UnitDamagecontrolList.Add(new UnitDamagecontrol(root, item));}
foreach(var item in node["Buoyancy"]) { BuoyancyList.Add(new Buoyancy(root, item));}
foreach(var item in node["Unitweapons"]) { UnitweaponsList.Add(new Unitweapons(root, item));}

        }  
        
public static Global_Units.RowValues GetRowValue(string name)
{
    var values = (RowValues[])Enum.GetValues(typeof(RowValues));
    for (int i = 0; i < values.Length; i++)
    {
        if(values[i].ToString() == name)
        {
            return values[i];
        }
    }
    return values[0];
}
    }
}