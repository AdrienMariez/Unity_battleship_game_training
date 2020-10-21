
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class FX_Storage
    {
        public string id;
public string FXPath;
public string FXPrefab;

        public enum RowValues { 
DamageExplosionLargePlaceholder, 
DamageSmokeEmitterLarge, 
DamageSmokeEmitterMedium, 
DamageSteamEmitterLarge, 
ShellExplosionLargePlaceholder, 
ShellExplosionMediumPlaceholder, 
ShellExplosionSmallPlaceholder, 
ShellWaterHitLargePlaceholder, 
ShellWaterHitMediumPlaceholder, 
ShellWaterHitSmallPlaceholder, 
ShellWaterHitTorpedoPlaceholder, 
StackSmokeEmitterLarge, 
StackSmokeEmitterMedium
 } 
        public FX_Storage (CastleDBParser.RootNode root, RowValues line) 
        {
            SimpleJSON.JSONNode node = root.GetSheetWithName("FX_Storage").Rows[(int)line];
id = node["id"];
FXPath = node["FXPath"];
FXPrefab = node["FXPrefab"];

        }  
        
public static FX_Storage.RowValues GetRowValue(string name)
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