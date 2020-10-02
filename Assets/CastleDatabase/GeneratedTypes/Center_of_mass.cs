
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class Center_of_mass
    {
        public float X;
public float Y;
public float Z;

         
        public Center_of_mass (CastleDBParser.RootNode root, SimpleJSON.JSONNode node) 
        {
            X = node["X"].AsFloat;
Y = node["Y"].AsFloat;
Z = node["Z"].AsFloat;

        }  
        
    }
}