
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class MapSize
    {
        public int Top;
public int Bottom;
public int Left;
public int Right;

         
        public MapSize (CastleDBParser.RootNode root, SimpleJSON.JSONNode node) 
        {
            Top = node["Top"].AsInt;
Bottom = node["Bottom"].AsInt;
Left = node["Left"].AsInt;
Right = node["Right"].AsInt;

        }  
        
    }
}