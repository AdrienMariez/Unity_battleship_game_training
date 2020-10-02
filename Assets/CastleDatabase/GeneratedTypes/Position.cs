
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class Position
    {
        public float Pos_x;
public float Pos_y;
public float Pos_z;
public float Rotation_x;
public float Rotation_y;
public float Rotation_z;

         
        public Position (CastleDBParser.RootNode root, SimpleJSON.JSONNode node) 
        {
            Pos_x = node["Pos_x"].AsFloat;
Pos_y = node["Pos_y"].AsFloat;
Pos_z = node["Pos_z"].AsFloat;
Rotation_x = node["Rotation_x"].AsFloat;
Rotation_y = node["Rotation_y"].AsFloat;
Rotation_z = node["Rotation_z"].AsFloat;

        }  
        
    }
}