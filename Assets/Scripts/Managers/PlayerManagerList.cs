using System;
using UnityEngine;


// Lack of "Monobehaviour" indicates that we are creating a new object class in this file.
// Serializable : makes new classes appear in the inspector
[Serializable]
public class PlayerManagerList {

    public GameObject m_Player;
    public CompiledTypes.Teams.RowValues m_PlayerTeam;

    public void Setup () {
    }
}