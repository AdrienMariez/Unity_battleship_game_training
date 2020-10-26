
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class FX
    {
        public FX_Storage FXShooting;
public AudioFX_Storage ShootingSound;

         
        public FX (CastleDBParser.RootNode root, SimpleJSON.JSONNode node) 
        {
            FXShooting = new CompiledTypes.FX_Storage(root,CompiledTypes.FX_Storage.GetRowValue(node["FXShooting"]));
ShootingSound = new CompiledTypes.AudioFX_Storage(root,CompiledTypes.AudioFX_Storage.GetRowValue(node["ShootingSound"]));

        }  
        
    }
}