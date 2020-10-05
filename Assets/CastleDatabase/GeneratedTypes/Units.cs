
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class Units
    {
        public string id;
public string Name;
public string Model;
public Units_sub_categories Category;
public Countries Nation;
public List<Score> ScoreList = new List<Score>();
public List<Camera> CameraList = new List<Camera>();
public int Health;
public List<Damagecontrol> DamagecontrolList = new List<Damagecontrol>();
public List<Buoyancy> BuoyancyList = new List<Buoyancy>();
public float Unitmass;
public List<Unitweapons> UnitweaponsList = new List<Unitweapons>();

        public enum RowValues { 
fuso, 
takao
 } 
        public Units (CastleDBParser.RootNode root, RowValues line) 
        {
            SimpleJSON.JSONNode node = root.GetSheetWithName("Units").Rows[(int)line];
id = node["id"];
Name = node["Name"];
Model = node["Model"];
Category = new CompiledTypes.Units_sub_categories(root,CompiledTypes.Units_sub_categories.GetRowValue(node["Category"]));
Nation = new CompiledTypes.Countries(root,CompiledTypes.Countries.GetRowValue(node["Nation"]));
foreach(var item in node["Score"]) { ScoreList.Add(new Score(root, item));}
foreach(var item in node["Camera"]) { CameraList.Add(new Camera(root, item));}
Health = node["Health"].AsInt;
foreach(var item in node["Damagecontrol"]) { DamagecontrolList.Add(new Damagecontrol(root, item));}
foreach(var item in node["Buoyancy"]) { BuoyancyList.Add(new Buoyancy(root, item));}
Unitmass = node["Unitmass"].AsFloat;
foreach(var item in node["Unitweapons"]) { UnitweaponsList.Add(new Unitweapons(root, item));}

        }  
        
public static Units.RowValues GetRowValue(string name)
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