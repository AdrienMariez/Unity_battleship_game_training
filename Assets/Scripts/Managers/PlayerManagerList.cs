using System;
using UnityEngine;


// Lack of "Monobehaviour" indicates that we are creating a new object class in this file.
// Serializable : makes new classes appear in the inspector
[Serializable]
public class PlayerManagerList {

    private GameObject PlayerObject; public GameObject GetPlayerObject() { return PlayerObject; } public void SetPlayerObject (GameObject _g) { PlayerObject = _g; }
    public CompiledTypes.Teams.RowValues m_PlayerTeam;
    private CompiledTypes.Teams PlayerTeam; public CompiledTypes.Teams GetPlayerTeam() { return PlayerTeam; } public void SetPlayerTeam (CompiledTypes.Teams _t) { PlayerTeam = _t; }

    public void Setup () {
    }
}