
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class UnitModelPath
    {
        public string UnitPath;
public string UnitModel;

         
        public UnitModelPath (CastleDBParser.RootNode root, SimpleJSON.JSONNode node) 
        {
            UnitPath = node["UnitPath"];
UnitModel = node["UnitModel"];

        }  
        
    }
}