
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class Units_sub_categories
    {
        public string id;
public string Classification;
public string Name;
public Units_categories Category;
public string MapModel;
public FX_Storage DeathExplosion;

        public enum RowValues { 
Empty, 
Battleship, 
Carrier, 
Heavy_Cruiser, 
Light_Cruiser, 
Destroyer, 
Attack_Submarine, 
Fighter, 
Light_Bomber, 
Heavy_Bomber, 
Tank, 
Airfield, 
Shipyard, 
LandBase
 } 
        public Units_sub_categories (CastleDBParser.RootNode root, RowValues line) 
        {
            SimpleJSON.JSONNode node = root.GetSheetWithName("Units_sub_categories").Rows[(int)line];
id = node["id"];
Classification = node["Classification"];
Name = node["Name"];
Category = new CompiledTypes.Units_categories(root,CompiledTypes.Units_categories.GetRowValue(node["Category"]));
MapModel = node["MapModel"];
DeathExplosion = new CompiledTypes.FX_Storage(root,CompiledTypes.FX_Storage.GetRowValue(node["DeathExplosion"]));

        }  
        
public static Units_sub_categories.RowValues GetRowValue(string name)
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