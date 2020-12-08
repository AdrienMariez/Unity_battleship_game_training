using System;
using UnityEngine;

[Serializable]
public class ElevationZonesManager {
    [Tooltip("Beginning of the special elevation zone")]
    [Range(0.0f, 360.0f)]
    public float ZoneBegin = 160.0f;

    [Tooltip("End of the special elevation zone")]
    [Range(0.0f, 360.0f)]
    public float ZoneEnd = 200.0f;

    [Tooltip("if unchecked, this Up Traverse value will be ignored.")]
    public bool OverrideMaxElev = false;

    [Tooltip("Maximum elevation (degrees) of the turret in the defined zone. 90° is horizontal.")]
    [Range(0.0f, 180.0f)]
    public float UpTraverse = 210.0f;

    [Tooltip("if unchecked, this Down Traverse value will be ignored.")]
    public bool OverrideMinElev = false;

    [Tooltip("Depression (degrees) of the turret in the defined zone. 90° is horizontal.")]
    [Range(0.0f, 180.0f)]
    public float DownTraverse = 70.0f;
}