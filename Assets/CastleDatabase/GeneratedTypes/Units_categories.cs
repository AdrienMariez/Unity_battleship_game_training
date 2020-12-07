
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class Units_categories
    {
        public string id;
public string Name;
public List<Scripts> ScriptsList = new List<Scripts>();
public string FileName;
public List<RigidBodyData> RigidBodyDataList = new List<RigidBodyData>();

        public enum RowValues { 
ship, 
submarine, 
aircraft, 
ground, 
building
 } 
        public Units_categories (CastleDBParser.RootNode root, RowValues line) 
        {
            SimpleJSON.JSONNode node = root.GetSheetWithName("Units_categories").Rows[(int)line];
id = node["id"];
Name = node["Name"];
foreach(var item in node["Scripts"]) { ScriptsList.Add(new Scripts(root, item));}
FileName = node["FileName"];
foreach(var item in node["RigidBodyData"]) { RigidBodyDataList.Add(new RigidBodyData(root, item));}

        }  
        
public static Units_categories.RowValues GetRowValue(string name)
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