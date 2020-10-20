
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class UnitVariantReference
    {
        public Global_Units UnitVariantRef;

         
        public UnitVariantReference (CastleDBParser.RootNode root, SimpleJSON.JSONNode node) 
        {
            UnitVariantRef = new CompiledTypes.Global_Units(root,CompiledTypes.Global_Units.GetRowValue(node["UnitVariantRef"]));

        }  
        
    }
}