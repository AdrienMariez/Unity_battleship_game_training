using System;
using UnityEngine;

[Serializable]
public class FireZonesManager {
    [Tooltip("Beginning of no fire zone")]
    [Range(0.0f, 360.0f)]
    public float ZoneBegin = 160.0f;

    [Tooltip("End of no fire zone")]
    [Range(0.0f, 360.0f)]
    public float ZoneEnd = 200.0f;
}