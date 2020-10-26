
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class AmmoModelPath
    {
        public string AmmoPath;
public string AmmoModel;

         
        public AmmoModelPath (CastleDBParser.RootNode root, SimpleJSON.JSONNode node) 
        {
            AmmoPath = node["AmmoPath"];
AmmoModel = node["AmmoModel"];

        }  
        
    }
}