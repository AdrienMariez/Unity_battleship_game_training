
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class Score
    {
        public int Commandpoints;
public int Victorypoints;

         
        public Score (CastleDBParser.RootNode root, SimpleJSON.JSONNode node) 
        {
            Commandpoints = node["Commandpoints"].AsInt;
Victorypoints = node["Victorypoints"].AsInt;

        }  
        
    }
}