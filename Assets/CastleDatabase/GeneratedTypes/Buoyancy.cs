
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class Buoyancy
    {
        public List<Center_of_mass> Center_of_massList = new List<Center_of_mass>();
public List<Force_points> Force_pointsList = new List<Force_points>();
public float Model_width;
public float Vertical_drag;
public float Engine_power;
public float Rotation_time;

         
        public Buoyancy (CastleDBParser.RootNode root, SimpleJSON.JSONNode node) 
        {
            foreach(var item in node["Center_of_mass"]) { Center_of_massList.Add(new Center_of_mass(root, item));}
foreach(var item in node["Force_points"]) { Force_pointsList.Add(new Force_points(root, item));}
Model_width = node["Model_width"].AsFloat;
Vertical_drag = node["Vertical_drag"].AsFloat;
Engine_power = node["Engine_power"].AsFloat;
Rotation_time = node["Rotation_time"].AsFloat;

        }  
        
    }
}