
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class Weapons_types
    {
        public string id;
public string Name;

        public enum RowValues { 
artillery, 
antiairartillery, 
antiair, 
torpedoes
 } 
        public Weapons_types (CastleDBParser.RootNode root, RowValues line) 
        {
            SimpleJSON.JSONNode node = root.GetSheetWithName("Weapons_types").Rows[(int)line];
id = node["id"];
Name = node["Name"];

        }  
        
public static Weapons_types.RowValues GetRowValue(string name)
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