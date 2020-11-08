
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class DuelSpawnPoints
    {
        public string DuelSpawnPointName;
public Units_categories DuelSpawnPointCategory;

         
        public DuelSpawnPoints (CastleDBParser.RootNode root, SimpleJSON.JSONNode node) 
        {
            DuelSpawnPointName = node["DuelSpawnPointName"];
DuelSpawnPointCategory = new CompiledTypes.Units_categories(root,CompiledTypes.Units_categories.GetRowValue(node["DuelSpawnPointCategory"]));

        }  
        
    }
}