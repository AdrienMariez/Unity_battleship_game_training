
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class RigidBodyData
    {
        public float CategoryDrag;
public float CategoryAngularDrag;
public bool CatUseGravity;
public bool CatIsKinematic;
public bool CatFreezePosition;
public bool CatFreezeRotation;

         
        public RigidBodyData (CastleDBParser.RootNode root, SimpleJSON.JSONNode node) 
        {
            CategoryDrag = node["CategoryDrag"].AsFloat;
CategoryAngularDrag = node["CategoryAngularDrag"].AsFloat;
CatUseGravity = node["CatUseGravity"].AsBool;
CatIsKinematic = node["CatIsKinematic"].AsBool;
CatFreezePosition = node["CatFreezePosition"].AsBool;
CatFreezeRotation = node["CatFreezeRotation"].AsBool;

        }  
        
    }
}