using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class UnitMapUIManager : MonoBehaviour {

    private Camera MapCam;
    private GameObject Unit;
    private GameObject ActiveUnit;
    private GameObject EnemyTargetUnit;
    private bool UnitCurrentlyPlayed = false;
    private bool ActionPaused = false;
    private bool ShortActionPaused = false;
    private bool BehindCamera = false;  // This will maybe have to go
    private string DistanceString;

    private float MaximumHealth;
    private float CurrentHealth;
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
        MapCam = cam;
        Unit = unit;
        MaximumHealth = this.transform.Find("Health").GetComponent<Slider>().maxValue;
        CurrentHealth = this.transform.Find("Health").GetComponent<Slider>().value;
    }

    protected void FixedUpdate() {
        // Debug.Log (this.gameObject.name+" calculus : " + Vector3.Dot(MapCam.transform.forward, heading));
        Vector3 screenPos = MapCam.WorldToScreenPoint(Unit.transform.position);
        Vector3 updatedPos = new Vector2(screenPos.x, (screenPos.y + 30f));
        this.transform.position  = updatedPos;

        // Update distance text
        if (!UnitCurrentlyPlayed) {
            float distance = (Unit.transform.position - MapCam.transform.position).magnitude;
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
    }

    public void SetPlayerUnit(GameObject activeUnit){
        ActiveUnit = activeUnit;
        if (ActiveUnit == Unit) {
            UnitCurrentlyPlayed = true;
        } else {
            UnitCurrentlyPlayed = false;
        }
    }

    public void SetCurrentHealth(float HP, Color barColor) {
        this.transform.Find("Health").GetComponent<Slider>().value = HP;
        this.transform.Find("Health").Find("FillArea").Find("Fill").GetComponent<Image>().color = barColor;
    }

    public void SetDead() {
        this.transform.Find("Name").GetComponent<Text>().color = Color.grey;
        this.transform.Find("Distance").GetComponent<Text>().color = Color.grey;
        StartCoroutine(WaitForDestroy());
        // Destroy (this.gameObject);
    }
    IEnumerator WaitForDestroy(){
        yield return new WaitForSeconds(5f);
        Destroy (this.gameObject);
    }
}