
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class No_fire_zones
    {
        public float ZoneBegin;
public float ZoneEnd;

         
        public No_fire_zones (CastleDBParser.RootNode root, SimpleJSON.JSONNode node) 
        {
            ZoneBegin = node["ZoneBegin"].AsFloat;
ZoneEnd = node["ZoneEnd"].AsFloat;

        }  
        
    }
}