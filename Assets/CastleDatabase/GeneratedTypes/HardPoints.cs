
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class HardPoints
    {
        public string id;

        public enum RowValues { 
Weapon, 
ShipFunnel, 
ShipPropeller, 
PlanePropeller
 } 
        public HardPoints (CastleDBParser.RootNode root, RowValues line) 
        {
            SimpleJSON.JSONNode node = root.GetSheetWithName("HardPoints").Rows[(int)line];
id = node["id"];

        }  
        
public static HardPoints.RowValues GetRowValue(string name)
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