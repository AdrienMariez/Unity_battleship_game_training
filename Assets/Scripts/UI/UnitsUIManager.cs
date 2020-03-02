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
    private GameManager GameManager;
    private bool ActionPaused = false;
    private bool ShortActionPaused = false;
    private bool MapActive = false;
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
        yield return new WaitForSeconds(0.5f);
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
        if (GameObject.Find("MainCamera"))
            Cam = GameObject.Find("MainCamera").GetComponentInChildren<Camera>();

        GameManager = GetComponent<GameManager>();

        GetUnits();
        SetDisplayStatus();
    }

    private void GetUnits() {
        GameObject[] units;

        units = GameObject.FindGameObjectsWithTag("Allies");
        UnitList.AddRange(units);
        units = GameObject.FindGameObjectsWithTag("AlliesAI");
        UnitList.AddRange(units);
        units = GameObject.FindGameObjectsWithTag("Axis");
        UnitList.AddRange(units);
        units = GameObject.FindGameObjectsWithTag("AxisAI");
        UnitList.AddRange(units);
        units = GameObject.FindGameObjectsWithTag("NeutralAI");
        UnitList.AddRange(units);
        // Debug.Log ("Units : "+ UnitList.Count);
    }
    private void SetDisplayStatus(){
        CreateGameDisplay();
        CreateMapDisplay();
        // if (!MapActive) {
        //     CreateGameDisplay();
        //     DestroyMapDisplay();
        // } else if (MapActive) {
        //     DestroyGameDisplay();
        //     CreateMapDisplay();
        // }
    }

    private void CreateGameDisplay(){
        // Debug.Log ("CreateGameDisplay : "+ m_UnitUI);
        foreach (var item in UnitList) {
            TempUI = Instantiate(m_UnitUI, PlayerCanvas.transform);
            if (item.GetComponent<ShipController>()){
                item.GetComponent<ShipUI>().SetUIElement(TempUI);
            }
            TempUI.GetComponent<UnitUIManager>().InitializeUIModule(Cam, item);
            UnitUIList.Add(TempUI);
            //send TempUI to the unit so it can update data itself
        }
    }
    private void DestroyGameDisplay(){

    }
    private void CreateMapDisplay(){
        // foreach (var item in UnitList) {
        //     TempUI = Instantiate(m_UnitMapUI, PlayerCanvas.transform);
        //     TempUI.gameObject.name = item.name;
        //     UnitUIMapList.Add(TempUI);
        // }
    }
    private void DestroyMapDisplay(){

    }

    protected void FixedUpdate() {
        // foreach (var item in UnitUIList) {
            
        // }
        // foreach (var item in UnitUIMapList) {
            
        // }
    }

    public void SetMapCamera(Camera camera){ MapCam = camera; }
    public void SetPlayerCanvas(GameObject playerCanvas, GameObject playerMapCanvas){ PlayerCanvas = playerCanvas; PlayerMapCanvas  = playerMapCanvas;}

    public void SetPlayedUnit(GameObject activeUnit){
        // Debug.Log ("SetPlayedUnit : "+ activeUnit);
        ActiveUnit = activeUnit;
        foreach (var item in UnitUIList) {
            item.GetComponent<UnitUIManager>().SetPlayerUnit(ActiveUnit);
        }
    }

    public void SetMap(bool active) {
        MapActive = active;
        SetDisplayStatus();
    }
}