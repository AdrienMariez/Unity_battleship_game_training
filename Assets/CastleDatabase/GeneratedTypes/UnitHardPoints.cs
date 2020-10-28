
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class UnitHardPoints
    {
        public string HardPointId;
public HardPoints HardPointType;
public bool IsTransferedToVariants;
public bool IsMirrored;
public Weapons WeaponType;

         
        public UnitHardPoints (CastleDBParser.RootNode root, SimpleJSON.JSONNode node) 
        {
            HardPointId = node["HardPointId"];
HardPointType = new CompiledTypes.HardPoints(root,CompiledTypes.HardPoints.GetRowValue(node["HardPointType"]));
IsTransferedToVariants = node["IsTransferedToVariants"].AsBool;
IsMirrored = node["IsMirrored"].AsBool;
WeaponType = new CompiledTypes.Weapons(root,CompiledTypes.Weapons.GetRowValue(node["WeaponType"]));

        }  
        
    }
}