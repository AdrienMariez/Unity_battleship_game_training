using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class UnitMapUIManager : MonoBehaviour {

    private Camera MapCam;
    private UnitsUIManager UnitsUIManager;
    private GameObject Unit;
    private GameObject ActiveUnit;
    private bool UnitCurrentlyPlayed = false;
    private GameObject EnemyTargetUnit;
    // private bool UnitCurrentlyTargeted = false;
    // private bool ActionPaused = false;
    // private bool ShortActionPaused = false;
    private string DistanceString;

    private GameObject UIName;
    private GameObject UIDistance;
    private GameObject UIHealth;
        private Slider HealthBar;
        private Image HealthBarColor;

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
    // IEnumerator PauseActionShort(){
    //     // Coroutine created to prevent too much calculus for ship behaviour
    //     ShortActionPaused = true;
    //     yield return new WaitForSeconds(0.1f);
    //     ShortActionPaused = false;
    // }
    public void InitializeUIModule(Camera cam, GameObject unit, UnitsUIManager unitsUIManager) {
        MapCam = cam;
        Unit = unit;
        UIName = this.transform.Find("Name").gameObject;
        UIDistance = this.transform.Find("Distance").gameObject;

        UIHealth = this.transform.Find("Health").gameObject;
        HealthBar = UIHealth.transform.GetComponent<Slider>();
        HealthBarColor = UIHealth.transform.Find("FillArea").Find("Fill").GetComponent<Image>();
        
        if (unit.GetComponent<UnitHealth>()) {
            MaximumHealth = unit.GetComponent<UnitHealth>().GetStartingHealth();
            CurrentHealth = unit.GetComponent<UnitHealth>().GetCurrentHealth();
        }
        HealthBar.maxValue = MaximumHealth;
        HealthBar.value = CurrentHealth;
        UnitsUIManager = unitsUIManager;
    }

    protected void FixedUpdate() {
        if (Unit == null) {
            Destroy();
            return;
        }
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
            UIDistance.transform.GetComponent<Text>().text = DistanceString;
        } else {
            UIDistance.transform.GetComponent<Text>().text = "Played unit";
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

    public void SetEnemyTargetUnit(GameObject targetUnit){
        EnemyTargetUnit = targetUnit;
        // if (EnemyTargetUnit == Unit) {
        //     UnitCurrentlyTargeted = true;
        // } else {
        //     UnitCurrentlyTargeted = false;
        // }
        // Debug.Log (" UnitCurrentlyPlayed : " + UnitCurrentlyPlayed);
    }

    public void SetCurrentHealth(float HP, Color barColor) {
        HealthBar.value = HP;
        HealthBarColor.color = barColor;
    }

    public void SetDead() {
        UIName.transform.GetComponent<Text>().color = Color.grey;
        UIDistance.transform.GetComponent<Text>().color = Color.grey;
        UnitsUIManager.RemoveMapUIElement(this.gameObject);
        StartCoroutine(WaitForDestroy());
        // Destroy (this.gameObject);
    }
    IEnumerator WaitForDestroy(){
        yield return new WaitForSeconds(5f);
        Destroy();
    }
    public void Destroy() { Destroy (this.gameObject); }
}