
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class ModelPath
    {
        public string AmmoPath;
public string AmmoModel;

         
        public ModelPath (CastleDBParser.RootNode root, SimpleJSON.JSONNode node) 
        {
            AmmoPath = node["AmmoPath"];
AmmoModel = node["AmmoModel"];

        }  
        
    }
}