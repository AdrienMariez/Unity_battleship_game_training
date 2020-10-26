
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class Weapons
    {
        public string id;
public string Name;
public bool Isavariant;
public List<WeaponVariantReference> WeaponVariantReferenceList = new List<WeaponVariantReference>();
public List<ModelPath> ModelPathList = new List<ModelPath>();
public List<FX> FXList = new List<FX>();
public float Health;
public float Armor;
public List<WeaponRoles> WeaponRolesList = new List<WeaponRoles>();
public Ammos Ammo;
public float Max_range;
public float Min_range;
public float Muzzle_velocity;
public float Reload_time;
public int Precision;
public float Rotation_speed;
public float Elevation_speed;
public float Max_vertical_traverse;
public float Min_vertical_traverse;

        public enum RowValues { 
JapanBB356mmA, 
JapanBB356mmB, 
JapanCA200mmA, 
JapanCA200mmB, 
JapanCA200mmY, 
Japan120mmType10AATwin, 
JapanBBCasemate120mm, 
JapanTorpedo4CA
 } 
        public Weapons (CastleDBParser.RootNode root, RowValues line) 
        {
            SimpleJSON.JSONNode node = root.GetSheetWithName("Weapons").Rows[(int)line];
id = node["id"];
Name = node["Name"];
Isavariant = node["Isavariant"].AsBool;
foreach(var item in node["WeaponVariantReference"]) { WeaponVariantReferenceList.Add(new WeaponVariantReference(root, item));}
foreach(var item in node["ModelPath"]) { ModelPathList.Add(new ModelPath(root, item));}
foreach(var item in node["FX"]) { FXList.Add(new FX(root, item));}
Health = node["Health"].AsFloat;
Armor = node["Armor"].AsFloat;
foreach(var item in node["WeaponRoles"]) { WeaponRolesList.Add(new WeaponRoles(root, item));}
Ammo = new CompiledTypes.Ammos(root,CompiledTypes.Ammos.GetRowValue(node["Ammo"]));
Max_range = node["Max_range"].AsFloat;
Min_range = node["Min_range"].AsFloat;
Muzzle_velocity = node["Muzzle_velocity"].AsFloat;
Reload_time = node["Reload_time"].AsFloat;
Precision = node["Precision"].AsInt;
Rotation_speed = node["Rotation_speed"].AsFloat;
Elevation_speed = node["Elevation_speed"].AsFloat;
Max_vertical_traverse = node["Max_vertical_traverse"].AsFloat;
Min_vertical_traverse = node["Min_vertical_traverse"].AsFloat;

        }  
        
public static Weapons.RowValues GetRowValue(string name)
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