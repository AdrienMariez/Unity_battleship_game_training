
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class Scenarios
    {
        public string id;
public string ScenarioScene;
public List<ScenarioGameModes> ScenarioGameModesList = new List<ScenarioGameModes>();
public List<DuelSpawnPoints> DuelSpawnPointsList = new List<DuelSpawnPoints>();

        public enum RowValues { 
A, 
B, 
C
 } 
        public Scenarios (CastleDBParser.RootNode root, RowValues line) 
        {
            SimpleJSON.JSONNode node = root.GetSheetWithName("Scenarios").Rows[(int)line];
id = node["id"];
ScenarioScene = node["ScenarioScene"];
foreach(var item in node["ScenarioGameModes"]) { ScenarioGameModesList.Add(new ScenarioGameModes(root, item));}
foreach(var item in node["DuelSpawnPoints"]) { DuelSpawnPointsList.Add(new DuelSpawnPoints(root, item));}

        }  
        
public static Scenarios.RowValues GetRowValue(string name)
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