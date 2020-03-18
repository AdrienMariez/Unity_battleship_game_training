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
    private bool UnitCurrentlyPlayed = false;
    private GameObject EnemyTargetUnit;
    private bool UnitCurrentlyTargeted = false;
    // private bool ActionPaused = false;
    
    private string DistanceString;

    private GameObject UIName;
    private GameObject UIDistance;
    private GameObject UIHealth;
    private GameObject UIPointer;

    private float MaximumHealth;
    private float CurrentHealth;
    const string RangeDisplayMeter = "{0} m";
    const string RangeDisplayKilometer = "{0} km";


    // IEnumerator PauseAction(){
    //     // Coroutine created to prevent too much calculus for ship behaviour
    //     ActionPaused = true;
    //     yield return new WaitForSeconds(0.5f);
    //     ActionPaused = false;
    // }
    public void InitializeUIModule(Camera cam, GameObject unit, UnitsUIManager unitsUIManager) {
        // Debug.Log ("InitializeUIModule");
        Cam = cam;
        Unit = unit;
        UIName = this.transform.Find("Name").gameObject;
        UIDistance = this.transform.Find("Distance").gameObject;
        UIHealth = this.transform.Find("Health").gameObject;
        UIPointer = this.transform.Find("Pointer").gameObject;
        MaximumHealth = this.transform.Find("Health").GetComponent<Slider>().maxValue;
        CurrentHealth = this.transform.Find("Health").GetComponent<Slider>().value;
        UnitsUIManager = unitsUIManager;
        StartCoroutine(PauseActionName());
    }
    private bool NameActionPaused = false;
    IEnumerator PauseActionName(){
        // Coroutine created to prevent too much calculus for ship behaviour
        NameActionPaused = true;
        yield return new WaitForSeconds(0.1f);
        UIName.GetComponent<Text>().text = Unit.name;
        NameActionPaused = false;
    }

    protected void FixedUpdate() {
        // Debug.Log ("Unit : "+ Unit+" - Cam : "+ Cam);
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
            // Debug.Log (this.gameObject.name+"  - screenPos : " + screenPos);
            if (UnitCurrentlyTargeted) {
                UIName.SetActive(true);
                UIDistance.SetActive(true);
                UIHealth.SetActive(true);
                UIPointer.SetActive(false);
                UpdateDistanceText();
            } else if (screenPos.x >= (0.45f*Screen.width) && screenPos.x <= (0.55f*Screen.width) && screenPos.y >= (0.35f*Screen.height) && screenPos.y <= (0.75f*Screen.height)){
                // Debug.Log (this.gameObject.name+"  - screenPos x : " + screenPos.x+"  - screenPos  y: " + screenPos.y);
                UIName.SetActive(false);
                UIDistance.SetActive(true);
                UIHealth.SetActive(true);
                UIPointer.SetActive(false);

                // Update distance text
                if (!UnitCurrentlyPlayed) {
                    UpdateDistanceText();
                } else {
                    UIDistance.GetComponent<Text>().text = "Played unit";
                }
            } else {
                UIName.SetActive(false);
                UIDistance.SetActive(false);
                UIHealth.SetActive(false);
                UIPointer.SetActive(true);
            }
        } else {
            // put the UI away if it is not visible
            this.transform.position  = new Vector2(0.0f, 3000f);
        }
    }

    private void UpdateDistanceText(){
        if (!DistanceActionPaused) {
            float distance = (Unit.transform.position - Cam.transform.position).magnitude;
            if (distance > 999) {
                distance = (Mathf.Round(distance / 100)) / 10f;
                DistanceString = string.Format(RangeDisplayKilometer, distance);
            } else {
                DistanceString = string.Format(RangeDisplayMeter, Mathf.Round(distance));
            }
            UIDistance.GetComponent<Text>().text = DistanceString;
            StartCoroutine(PauseActionDistance());
        }
    }
    private bool DistanceActionPaused = false;
    IEnumerator PauseActionDistance(){
        // Coroutine created to prevent too much calculus for ship behaviour
        DistanceActionPaused = true;
        yield return new WaitForSeconds(0.1f);
        UIName.GetComponent<Text>().text = Unit.name;
        DistanceActionPaused = false;
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

    public void SetEnemyTargetUnit(GameObject targetUnit){
        EnemyTargetUnit = targetUnit;
        if (EnemyTargetUnit == Unit) {
            UnitCurrentlyTargeted = true;
            // UIName.GetComponent<Text>().text = Unit.name;
        } else {
            UnitCurrentlyTargeted = false;
        }
        // Debug.Log (" UnitCurrentlyPlayed : " + UnitCurrentlyPlayed);
    }

    public void SetCurrentHealth(float HP, Color barColor) {
        UIHealth.transform.GetComponent<Slider>().value = HP;
        UIHealth.transform.Find("FillArea").Find("Fill").GetComponent<Image>().color = barColor;
    }

    public void SetDead() {
        UIName.GetComponent<Text>().color = Color.grey;
        UIDistance.GetComponent<Text>().color = Color.grey;
        UIPointer.GetComponent<Image>().color = Color.grey;
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