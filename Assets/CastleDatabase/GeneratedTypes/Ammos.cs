
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class Ammos
    {
        public string id;
public string Name;
public List<AmmoModelPath> AmmoModelPathList = new List<AmmoModelPath>();
public List<AmmoFX> AmmoFXList = new List<AmmoFX>();
public float Weight;
public float DamageMax;
public float DamageMin;
public float ArmorPenetration;
public float ExplosionRadius;
public float MaxLifeTime;

        public enum RowValues { 
ShellJapan356mm, 
ShellJapan200mm, 
ShellJapan152mm, 
ShellJapan120mm, 
TorpedoJapan610mm, 
AirMGAmmo
 } 
        public Ammos (CastleDBParser.RootNode root, RowValues line) 
        {
            SimpleJSON.JSONNode node = root.GetSheetWithName("Ammos").Rows[(int)line];
id = node["id"];
Name = node["Name"];
foreach(var item in node["AmmoModelPath"]) { AmmoModelPathList.Add(new AmmoModelPath(root, item));}
foreach(var item in node["AmmoFX"]) { AmmoFXList.Add(new AmmoFX(root, item));}
Weight = node["Weight"].AsFloat;
DamageMax = node["DamageMax"].AsFloat;
DamageMin = node["DamageMin"].AsFloat;
ArmorPenetration = node["ArmorPenetration"].AsFloat;
ExplosionRadius = node["ExplosionRadius"].AsFloat;
MaxLifeTime = node["MaxLifeTime"].AsFloat;

        }  
        
public static Ammos.RowValues GetRowValue(string name)
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