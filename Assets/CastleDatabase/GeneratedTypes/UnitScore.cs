
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class UnitScore
    {
        public int Commandpoints;
public int Victorypoints;
public int SquadSize;

         
        public UnitScore (CastleDBParser.RootNode root, SimpleJSON.JSONNode node) 
        {
            Commandpoints = node["Commandpoints"].AsInt;
Victorypoints = node["Victorypoints"].AsInt;
SquadSize = node["SquadSize"].AsInt;

        }  
        
    }
}