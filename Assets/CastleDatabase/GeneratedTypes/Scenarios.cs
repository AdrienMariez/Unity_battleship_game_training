
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
public GameModesFlag GameModes;
[FlagsAttribute] public enum GameModesFlag { duel = 1,points = 2,custom = 4 }
        public enum RowValues { 
A, 
B, 
C, 
D, 
E
 } 
        public Scenarios (CastleDBParser.RootNode root, RowValues line) 
        {
            SimpleJSON.JSONNode node = root.GetSheetWithName("Scenarios").Rows[(int)line];
id = node["id"];
ScenarioScene = node["ScenarioScene"];
GameModes = (GameModesFlag)node["GameModes"].AsInt;

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