using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class UnitsUIManager : MonoBehaviour {

    private Camera Cam;
    private Camera MapCam;
    private GameObject PlayerCanvas;
    private GameObject PlayerMapCanvas;
    private bool ActionPaused = false;
    private bool ShortActionPaused = false;
    private List <GameObject> UnitList = new List<GameObject>();
    private List <GameObject> UnitUIList = new List<GameObject>();
    private List <GameObject> UnitUIMapList = new List<GameObject>();
    private GameObject TempUI;
    private GameObject ActiveUnit;

    public GameObject m_UnitUI;
    public GameObject m_UnitMapUI;

    IEnumerator PauseAction(){
        // Coroutine created to prevent too much calculus for ship behaviour
        ActionPaused = true;
        yield return new WaitForSeconds(4f);
        ActionPaused = false;
    }
    IEnumerator PauseActionShort(){
        // Coroutine created to prevent too much calculus for ship behaviour
        ShortActionPaused = true;
        yield return new WaitForSeconds(0.1f);
        ShortActionPaused = false;
    }
    public void Init() {
        // ShipUIText.text = this.name;
        // Debug.Log ("Init : UnitsUIManager");
        if (GameObject.Find("MainCamera"))
            Cam = GameObject.Find("MainCamera").GetComponentInChildren<Camera>();

        GetUnits();
        SetDisplayStatus();
    }

    private void GetUnits() {
        UnitList.AddRange(GameObject.FindGameObjectsWithTag("Allies"));
        UnitList.AddRange(GameObject.FindGameObjectsWithTag("AlliesAI"));
        UnitList.AddRange(GameObject.FindGameObjectsWithTag("Axis"));
        UnitList.AddRange(GameObject.FindGameObjectsWithTag("AxisAI"));
        UnitList.AddRange(GameObject.FindGameObjectsWithTag("NeutralAI"));
        // Debug.Log ("Units : "+ UnitList.Count);
    }
    private void SetDisplayStatus(){
        CreateGameDisplay();
        CreateMapDisplay();
    }
    private void CreateGameDisplay(){
        // Debug.Log ("CreateGameDisplay : "+ UnitList.Count);

        foreach (var item in UnitList) {
            if (item == null) {continue;}
            // Debug.Log ("name : "+ item.name);
            TempUI = Instantiate(m_UnitUI, PlayerCanvas.transform);
            if (item.GetComponent<ShipController>()){
                item.GetComponent<ShipUI>().SetUIElement(TempUI);
            }
            TempUI.GetComponent<UnitUIManager>().InitializeUIModule(Cam, item, this);
            UnitUIList.Add(TempUI);
        }
    }
    private void DestroyGameDisplay(){

    }
    private void CreateMapDisplay(){
        foreach (var item in UnitList) {
            if (item == null) {continue;}
            TempUI = Instantiate(m_UnitMapUI, PlayerMapCanvas.transform);
            if (item.GetComponent<ShipController>()){
                item.GetComponent<ShipUI>().SetUIMapElement(TempUI);
            }
            TempUI.GetComponent<UnitMapUIManager>().InitializeUIModule(MapCam, item, this);
            UnitUIMapList.Add(TempUI);
        }
    }
    private void DestroyMapDisplay(){

    }

    protected void FixedUpdate() {
        // if (!ActionPaused) {
        //     Debug.Log ("units listed : "+ UnitUIList.Count);
        //     StartCoroutine(PauseAction());
        // }
    }

    public void SetMapCamera(Camera camera){ MapCam = camera; }
    public void SetPlayerCanvas(GameObject playerCanvas, GameObject playerMapCanvas){ PlayerCanvas = playerCanvas; PlayerMapCanvas  = playerMapCanvas;}

    public void SetPlayedUnit(GameObject activeUnit){
        // Debug.Log ("SetPlayedUnit : "+ activeUnit);
        ActiveUnit = activeUnit;
        foreach (var item in UnitUIList) {
            if (item == null) { continue; }
            else { item.GetComponent<UnitUIManager>().SetPlayerUnit(ActiveUnit); }
        }
        foreach (var item in UnitUIMapList) {
            if (item == null) { continue; }
            else { item.GetComponent<UnitMapUIManager>().SetPlayerUnit(ActiveUnit); }
        }
    }

    public void RemoveUIElement(GameObject unitGameObject){
        UnitUIList.Remove(unitGameObject);
    }
    public void RemoveMapUIElement(GameObject unitGameObject){
        UnitUIMapList.Remove(unitGameObject);
    }

    public void KillAllInstances() {
        // Debug.Log ("KillAllInstances");
        foreach (var item in UnitList) {
            if (item == null) { continue; }
            if (item.GetComponent<ShipController>()){
                item.GetComponent<ShipUI>().KillAllUIInstances();
            }
        }
        foreach (var item in UnitUIList) {
            if (item == null) { continue; }
            item.GetComponent<UnitUIManager>().Destroy();
        }
        foreach (var item in UnitUIMapList) {
            if (item == null) { continue; }
            item.GetComponent<UnitMapUIManager>().Destroy();
        }
        UnitList.Clear();
        UnitUIList.Clear();
        UnitUIMapList.Clear();
        UnitList = new List<GameObject>();
        UnitUIList = new List<GameObject>();
        UnitUIMapList = new List<GameObject>();
    }
}