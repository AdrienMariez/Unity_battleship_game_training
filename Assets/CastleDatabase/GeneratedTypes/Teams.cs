
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class Teams
    {
        public string id;
public string Name;
public float TeamColorRed;
public float TeamColorGreen;
public float TeamColorBlue;

        public enum RowValues { 
Allies, 
Axis, 
Neutral
 } 
        public Teams (CastleDBParser.RootNode root, RowValues line) 
        {
            SimpleJSON.JSONNode node = root.GetSheetWithName("Teams").Rows[(int)line];
id = node["id"];
Name = node["Name"];
TeamColorRed = node["TeamColorRed"].AsFloat;
TeamColorGreen = node["TeamColorGreen"].AsFloat;
TeamColorBlue = node["TeamColorBlue"].AsFloat;

        }  
        
public static Teams.RowValues GetRowValue(string name)
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