
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class Weapons_roles
    {
        public string id;
public string Name;

        public enum RowValues { 
artillery, 
navalartillery, 
antiair, 
torpedoes
 } 
        public Weapons_roles (CastleDBParser.RootNode root, RowValues line) 
        {
            SimpleJSON.JSONNode node = root.GetSheetWithName("Weapons_roles").Rows[(int)line];
id = node["id"];
Name = node["Name"];

        }  
        
public static Weapons_roles.RowValues GetRowValue(string name)
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