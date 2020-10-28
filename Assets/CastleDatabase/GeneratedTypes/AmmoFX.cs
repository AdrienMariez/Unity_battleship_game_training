
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class AmmoFX
    {
        public FX_Storage FXExplosion;
public FX_Storage FXWaterHit;

         
        public AmmoFX (CastleDBParser.RootNode root, SimpleJSON.JSONNode node) 
        {
            FXExplosion = new CompiledTypes.FX_Storage(root,CompiledTypes.FX_Storage.GetRowValue(node["FXExplosion"]));
FXWaterHit = new CompiledTypes.FX_Storage(root,CompiledTypes.FX_Storage.GetRowValue(node["FXWaterHit"]));

        }  
        
    }
}