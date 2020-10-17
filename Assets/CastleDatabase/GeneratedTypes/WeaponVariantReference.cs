
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class WeaponVariantReference
    {
        public Weapons WeaponVariantRef;

         
        public WeaponVariantReference (CastleDBParser.RootNode root, SimpleJSON.JSONNode node) 
        {
            WeaponVariantRef = new CompiledTypes.Weapons(root,CompiledTypes.Weapons.GetRowValue(node["WeaponVariantRef"]));

        }  
        
    }
}