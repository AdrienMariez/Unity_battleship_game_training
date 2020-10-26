
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class AudioFX_Storage
    {
        public string id;
public string SoundFXPath;
public string SoundFXPrefab;

        public enum RowValues { 
musicA_calm, 
musicA_stress, 
ShellExplosion, 
ShotFiring, 
TankExplosion
 } 
        public AudioFX_Storage (CastleDBParser.RootNode root, RowValues line) 
        {
            SimpleJSON.JSONNode node = root.GetSheetWithName("AudioFX_Storage").Rows[(int)line];
id = node["id"];
SoundFXPath = node["SoundFXPath"];
SoundFXPrefab = node["SoundFXPrefab"];

        }  
        
public static AudioFX_Storage.RowValues GetRowValue(string name)
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