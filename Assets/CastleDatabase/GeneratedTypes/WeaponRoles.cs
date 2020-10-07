
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class WeaponRoles
    {
        public Weapons_roles Weaponrole;

         
        public WeaponRoles (CastleDBParser.RootNode root, SimpleJSON.JSONNode node) 
        {
            Weaponrole = new CompiledTypes.Weapons_roles(root,CompiledTypes.Weapons_roles.GetRowValue(node["Weaponrole"]));

        }  
        
    }
}