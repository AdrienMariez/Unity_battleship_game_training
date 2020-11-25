
using System.Collections.Generic;
using UnityEngine;
public class UnitSelectionManager : UnitParameter {
    bool SelectorActive = false;                                                                                // If this isn't set, do not let the selection process work
    bool MapActive = false; public virtual void SetMap(bool mapActive) { MapActive = mapActive; }
    PlayerManager PlayerManager;
        public void SetPlayerManager(PlayerManager _s){ PlayerManager = _s; TryData(); }
    UnitMasterController UnitController;
        public void SetUnitController(UnitMasterController _s){ UnitController = _s; TryData(); }


    private void TryData() {
        if (UnitController != null && PlayerManager != null) {
            SelectorActive = true;
        }
    }
    void OnMouseEnter() { 
        if (SelectorActive && MapActive) {
            // Debug.Log("Mouse entered "+ UnitController.GetUnitName());
            PlayerManager.HighlightUnitByMap(UnitController, true);
        }
    }
    void OnMouseExit() {
        if (SelectorActive && MapActive) {
            // Debug.Log("Mouse exited "+ UnitController.GetUnitName());
            PlayerManager.HighlightUnitByMap(UnitController, false);
        }
    }
    // void OnMouseOver() {
    //     if (SelectorActive && MapActive) {
    //         Debug.Log("Mouse over "+ UnitController.GetUnitName());
    //     }
    // }
    void OnMouseDown() {
        if (SelectorActive && MapActive) {
            // Debug.Log("FAKE Mouse click on "+ UnitController.GetUnitName());
            if (PlayerManager.GetPlayerTeam().id == UnitController.GetTeam().id) {
                // Debug.Log("Mouse click on "+ UnitController.GetUnitName());
                PlayerManager.SwitchSelectedUnitByController(UnitController);
            }
        }
    }

}