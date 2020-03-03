using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class UnitUIManager : MonoBehaviour {

    private Camera Cam;
    private UnitsUIManager UnitsUIManager;
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
    public void InitializeUIModule(Camera cam, GameObject unit, UnitsUIManager unitsUIManager) {
        // Debug.Log ("InitializeUIModule");
        Cam = cam;
        Unit = unit;
        MaximumHealth = this.transform.Find("Health").GetComponent<Slider>().maxValue;
        CurrentHealth = this.transform.Find("Health").GetComponent<Slider>().value;
        UnitsUIManager = unitsUIManager;
    }

    protected void FixedUpdate() {
        // Debug.Log ("Unit : "+ Unit+"Cam : "+ Cam);
        if (Unit == null) {
            Destroy();
            return;
        }
        var heading = Unit.transform.position - Cam.transform.position;

        if (Vector3.Dot(Cam.transform.forward, heading) > 0 && !UnitCurrentlyPlayed) {
            // Update position of UI if it is supposed to be visible
            // Debug.Log (this.gameObject.name+" calculus : " + Vector3.Dot(Cam.transform.forward, heading));
            Vector3 screenPos = Cam.WorldToScreenPoint(Unit.transform.position);
            Vector3 updatedPos = new Vector2(screenPos.x, (screenPos.y + 30f));
            this.transform.position  = updatedPos;

            // Update distance text
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
        } else {
            // put the UI away if it is not visible
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
        // Debug.Log (" UnitCurrentlyPlayed : " + UnitCurrentlyPlayed);
    }

    public void SetCurrentHealth(float HP, Color barColor) {
        this.transform.Find("Health").GetComponent<Slider>().value = HP;
        this.transform.Find("Health").Find("FillArea").Find("Fill").GetComponent<Image>().color = barColor;
    }

    public void SetDead() {
        this.transform.Find("Name").GetComponent<Text>().color = Color.grey;
        this.transform.Find("Distance").GetComponent<Text>().color = Color.grey;
        UnitsUIManager.RemoveUIElement(this.gameObject);
        StartCoroutine(WaitForDestroy());
    }
    IEnumerator WaitForDestroy(){
        yield return new WaitForSeconds(5f);
        // Destroy();
        Destroy (this.gameObject);
    }
    public void Destroy() { //Debug.Log ("Destroy");
    Destroy (this.gameObject); }
}