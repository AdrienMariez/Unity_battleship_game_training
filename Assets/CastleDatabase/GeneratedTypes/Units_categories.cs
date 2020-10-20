
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
public string ScriptPaths;
public string FileName;

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
ScriptPaths = node["ScriptPaths"];
FileName = node["FileName"];

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