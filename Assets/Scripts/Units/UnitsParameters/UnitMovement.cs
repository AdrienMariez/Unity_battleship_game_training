using UnityEngine;
using System.Collections;

public class UnitMovement : UnitParameter {
    protected bool Active = false; 
    protected bool Dead = false;
    protected bool MapActive = false;
    public virtual void SetActive(bool activate) {
        Active = activate;
    }
    public virtual void SetDead(bool death) {
        Dead = death;
    }
    public virtual void SetMap(bool map) {
        MapActive = map;
    }
}