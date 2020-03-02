using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class UnitUIManager : MonoBehaviour {

    private Camera Cam;
    private GameObject Unit;
    private GameObject ActiveUnit;
    private GameObject EnemyTargetUnit;
    private bool UnitCurrentlyPlayed = false;
    private bool ActionPaused = false;
    private bool ShortActionPaused = false;
    private bool BehindCamera = false;  // This will maybe have to go
    private string DistanceString;
    const string RangeDisplayMeter = "{0} m";
    const string RangeDisplayKilometer = "{0} km";


    IEnumerator PauseAction(){
        // Coroutine created to prevent too much calculus for ship behaviour
        ActionPaused = true;
        yield return new WaitForSeconds(0.5f);
        ActionPaused = false;
    }
    IEnumerator PauseActionShort(){
        // Coroutine created to prevent too much calculus for ship behaviour
        ShortActionPaused = true;
        yield return new WaitForSeconds(0.1f);
        ShortActionPaused = false;
    }
    public void InitializeUIModule(Camera cam, GameObject unit) {
        Cam = cam;
        Unit = unit;
    }

    protected void FixedUpdate() {
        // Update distances
        // TODO Don't update if it's not visible
        if (!UnitCurrentlyPlayed) {
            float distance = (Unit.transform.position - Cam.transform.position).magnitude;
            if (distance > 999) {
                distance = (Mathf.Round(distance / 100)) / 10f;
                DistanceString = string.Format(RangeDisplayKilometer, distance);
            } else {
                DistanceString = string.Format(RangeDisplayMeter, Mathf.Round(distance));
            }
            this.transform.Find("Distance").GetComponent<Text>().text = DistanceString;
        } else {
            this.transform.Find("Distance").GetComponent<Text>().text = "Played unit";
        }

        var heading = Unit.transform.position - Cam.transform.position;

        
        if (Vector3.Dot(Cam.transform.forward, heading) > 0 && !UnitCurrentlyPlayed) {
            // Debug.Log (this.gameObject.name+" calculus : " + Vector3.Dot(Cam.transform.forward, heading));
            Vector3 screenPos = Cam.WorldToScreenPoint(Unit.transform.position);
            Vector3 updatedPos = new Vector2(screenPos.x, (screenPos.y + 30f));
            this.transform.position  = updatedPos;
        } else {
            this.transform.position  = new Vector2(0.0f, 3000f);
        }
    }

    public void SetPlayerUnit(GameObject activeUnit){
        ActiveUnit = activeUnit;
        if (ActiveUnit == Unit) {
            UnitCurrentlyPlayed = true;
        } else {
            UnitCurrentlyPlayed = false;
        }
    }
}