
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class ScenarioGameModes
    {
        public GameModes ScenarioGameMode;

         
        public ScenarioGameModes (CastleDBParser.RootNode root, SimpleJSON.JSONNode node) 
        {
            ScenarioGameMode = new CompiledTypes.GameModes(root,CompiledTypes.GameModes.GetRowValue(node["ScenarioGameMode"]));

        }  
        
    }
}