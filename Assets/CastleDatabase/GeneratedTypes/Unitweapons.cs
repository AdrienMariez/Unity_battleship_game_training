
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class Unitweapons
    {
        public Weapons WeaponRef;
public int HardPoint;
public List<Position> PositionList = new List<Position>();
public bool Mirror;
public float Left_traverse_limitation;
public float Right_traverse_limitation;
public List<No_fire_zones> No_fire_zonesList = new List<No_fire_zones>();
public List<Elevation_zones> Elevation_zonesList = new List<Elevation_zones>();

         
        public Unitweapons (CastleDBParser.RootNode root, SimpleJSON.JSONNode node) 
        {
            WeaponRef = new CompiledTypes.Weapons(root,CompiledTypes.Weapons.GetRowValue(node["WeaponRef"]));
HardPoint = node["HardPoint"].AsInt;
foreach(var item in node["Position"]) { PositionList.Add(new Position(root, item));}
Mirror = node["Mirror"].AsBool;
Left_traverse_limitation = node["Left_traverse_limitation"].AsFloat;
Right_traverse_limitation = node["Right_traverse_limitation"].AsFloat;
foreach(var item in node["No_fire_zones"]) { No_fire_zonesList.Add(new No_fire_zones(root, item));}
foreach(var item in node["Elevation_zones"]) { Elevation_zonesList.Add(new Elevation_zones(root, item));}

        }  
        
    }
}