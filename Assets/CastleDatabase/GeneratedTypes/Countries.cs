
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class Countries
    {
        public string id;
public string Name;
public Teams Team;
public string Flag;

        public enum RowValues { 
australia, 
china, 
france, 
greatbritain, 
germany, 
italy, 
japan, 
newzealand, 
usa, 
ussr
 } 
        public Countries (CastleDBParser.RootNode root, RowValues line) 
        {
            SimpleJSON.JSONNode node = root.GetSheetWithName("Countries").Rows[(int)line];
id = node["id"];
Name = node["Name"];
Team = new CompiledTypes.Teams(root,CompiledTypes.Teams.GetRowValue(node["Team"]));
Flag = node["Flag"];

        }  
        
public static Countries.RowValues GetRowValue(string name)
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