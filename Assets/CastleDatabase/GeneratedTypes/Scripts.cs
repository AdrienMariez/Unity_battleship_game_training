
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class Scripts
    {
        public int LoadOrder;
public string ScriptName;

         
        public Scripts (CastleDBParser.RootNode root, SimpleJSON.JSONNode node) 
        {
            LoadOrder = node["LoadOrder"].AsInt;
ScriptName = node["ScriptName"];

        }  
        
    }
}