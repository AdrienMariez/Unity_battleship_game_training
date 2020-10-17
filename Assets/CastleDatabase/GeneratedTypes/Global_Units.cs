
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
public string UnitName;
public string UnitModel;
public Units_sub_categories UnitCategory;
public Countries UnitNation;
public List<UnitScore> UnitScoreList = new List<UnitScore>();
public List<UnitCameraParameters> UnitCameraParametersList = new List<UnitCameraParameters>();
public int UnitHealth;
public List<UnitDamagecontrol> UnitDamagecontrolList = new List<UnitDamagecontrol>();
public List<Buoyancy> BuoyancyList = new List<Buoyancy>();
public float Unitmass;
public List<Unitweapons> UnitweaponsList = new List<Unitweapons>();

        public enum RowValues { 
fuso, 
takao
 } 
        public Global_Units (CastleDBParser.RootNode root, RowValues line) 
        {
            SimpleJSON.JSONNode node = root.GetSheetWithName("Global_Units").Rows[(int)line];
id = node["id"];
UnitName = node["UnitName"];
UnitModel = node["UnitModel"];
UnitCategory = new CompiledTypes.Units_sub_categories(root,CompiledTypes.Units_sub_categories.GetRowValue(node["UnitCategory"]));
UnitNation = new CompiledTypes.Countries(root,CompiledTypes.Countries.GetRowValue(node["UnitNation"]));
foreach(var item in node["UnitScore"]) { UnitScoreList.Add(new UnitScore(root, item));}
foreach(var item in node["UnitCameraParameters"]) { UnitCameraParametersList.Add(new UnitCameraParameters(root, item));}
UnitHealth = node["UnitHealth"].AsInt;
foreach(var item in node["UnitDamagecontrol"]) { UnitDamagecontrolList.Add(new UnitDamagecontrol(root, item));}
foreach(var item in node["Buoyancy"]) { BuoyancyList.Add(new Buoyancy(root, item));}
Unitmass = node["Unitmass"].AsFloat;
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