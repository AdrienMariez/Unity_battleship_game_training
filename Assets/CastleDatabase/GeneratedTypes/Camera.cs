
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class Camera
    {
        public float Distance;
public float Height;
public float Lateraloffset;

         
        public Camera (CastleDBParser.RootNode root, SimpleJSON.JSONNode node) 
        {
            Distance = node["Distance"].AsFloat;
Height = node["Height"].AsFloat;
Lateraloffset = node["Lateraloffset"].AsFloat;

        }  
        
    }
}