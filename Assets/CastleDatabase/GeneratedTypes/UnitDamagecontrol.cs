
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class UnitDamagecontrol
    {
        public float Repairrate;
public int Repaircrew;

         
        public UnitDamagecontrol (CastleDBParser.RootNode root, SimpleJSON.JSONNode node) 
        {
            Repairrate = node["Repairrate"].AsFloat;
Repaircrew = node["Repaircrew"].AsInt;

        }  
        
    }
}