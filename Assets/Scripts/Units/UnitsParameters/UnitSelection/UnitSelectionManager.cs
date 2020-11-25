
using System.Collections.Generic;
using UnityEngine;
public class UnitSelectionManager : UnitParameter {
    bool SelectorActive = false;                                                                                // If this isn't set, do not let the selection process work
    bool MapActive = false; public virtual void SetMap(bool mapActive) { MapActive = mapActive; }
    PlayerManager PlayerManager;
        public void SetPlayerManager(PlayerManager _s){ PlayerManager = _s; TryData(); }
    UnitMasterController UnitController;
        public void SetUnitController(UnitMasterController _s){ UnitController = _s; UnitController.SetUnitSelectionManager(this); TryData(); }


    private void TryData() {
        if (UnitController != null && PlayerManager != null) {
            SelectorActive = true;
        }
    }
    // void OnMouseEnter() { Debug.Log("Mouse entered GameObject."); }
    // void OnMouseExit() { Debug.Log("Mouse is no longer on GameObject."); }
    void OnMouseOver() {
        // Debug.Log("False mouse over "+ UnitController.GetUnitName());
        if (MapActive) {
            Debug.Log("Mouse over "+ UnitController.GetUnitName());
        }
    }
    void OnMouseDown() {
        if (SelectorActive && MapActive) {
            Debug.Log("Mouse click on "+ UnitController.GetUnitName());
        }
    }

}