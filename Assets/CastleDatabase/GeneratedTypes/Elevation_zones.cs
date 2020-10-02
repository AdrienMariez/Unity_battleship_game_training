
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class Elevation_zones
    {
        public float ZoneBegin;
public float ZoneEnd;
public float VerticalMaxOverride;
public float VerticalMinOverride;

         
        public Elevation_zones (CastleDBParser.RootNode root, SimpleJSON.JSONNode node) 
        {
            ZoneBegin = node["ZoneBegin"].AsFloat;
ZoneEnd = node["ZoneEnd"].AsFloat;
VerticalMaxOverride = node["VerticalMaxOverride"].AsFloat;
VerticalMinOverride = node["VerticalMinOverride"].AsFloat;

        }  
        
    }
}