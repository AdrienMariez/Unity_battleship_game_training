
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class WeaponModelPath
    {
        public string TurretPath;
public string TurretModel;

         
        public WeaponModelPath (CastleDBParser.RootNode root, SimpleJSON.JSONNode node) 
        {
            TurretPath = node["TurretPath"];
TurretModel = node["TurretModel"];

        }  
        
    }
}