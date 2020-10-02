
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class Force_points
    {
        public float X;
public float Y;
public float Z;

         
        public Force_points (CastleDBParser.RootNode root, SimpleJSON.JSONNode node) 
        {
            X = node["X"].AsFloat;
Y = node["Y"].AsFloat;
Z = node["Z"].AsFloat;

        }  
        
    }
}