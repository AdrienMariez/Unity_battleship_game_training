using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class StackComponentSmoke {
    [Tooltip("Select the prefab used for this funnel")]
    public GameObject m_SmokePrefab;
    // private GameObject SmokeIdleInstance;
    private GameObject SmokeInstance;
    [Tooltip("Select the position of the funnel")]
    public Transform m_StackPosition;
    private bool StackPlaying = true;
    private UnityEngine.ParticleSystem.MinMaxCurve InitialRate;
    public void SetStackInstance(GameObject gameobj) {
        SmokeInstance = gameobj;
        var emission = SmokeInstance.GetComponent<ParticleSystem>().emission;
        InitialRate = emission.rateOverTime.constant;
    }
    public void StackInstanceStart(){
        if (!StackPlaying) {
            StackPlaying = !StackPlaying;
            SmokeInstance.GetComponent<ParticleSystem>().Play();
        }
    }
    public void StackInstanceStop(){
        if (StackPlaying) {
            StackPlaying = !StackPlaying;
            SmokeInstance.GetComponent<ParticleSystem>().Stop();
        }
    }
    public void InstantiateMoveSet(float multiplier){
        var emission = SmokeInstance.GetComponent<ParticleSystem>().emission;
        // emission.rateOverTime = 1;
        var rate = emission.rateOverTime;
        rate = multiplier * InitialRate.constant;
        emission.rateOverTime = rate;
    }
    

    public GameObject GetSmokePrefab () {
        return m_SmokePrefab;
    }
    public Transform GetStackPosition () {
        return m_StackPosition;
    }
}