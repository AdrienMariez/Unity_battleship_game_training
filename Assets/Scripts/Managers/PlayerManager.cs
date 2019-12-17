using System.Collections;
using UnityEngine;
// using UnityEngine.UI;
using UnityEngine.SceneManagement;
using FreeLookCamera;
using UI;

// This class stores the currently played unit and allows to enable player controls for units or disable them
public class PlayerManager : MonoBehaviour
{
    //public FreeLookCam m_FreeLookCam;

    private GameObject[] PlayerUnits;
    private int CurrentTarget = 0;
    // [HideInInspector] public bool m_Active;
    private GameObject ActiveTarget;
    private bool MapActive;
    private FreeLookCam FreeLookCamera;
    private UIManager UIManager;

    private void Start() {
        FreeLookCamera = GameObject.Find("FreeLookCameraRig").GetComponent<FreeLookCam>();
        UIManager = GetComponent<UIManager>();
        FindAllPossibleTargets();
        SetEnabledUnit(PlayerUnits.Length);
        MapActive = false;
    }

    protected void Update() {
        // Look if the unit is changed, otherwise, do nothing
        if (Input.GetButtonDown ("SetNextUnit")) {
            SetNextTarget();
        }
        if (Input.GetButtonDown ("SetPreviousUnit")) {
            SetPreviousTarget();
        }
        //Debug.Log ("Current target : "+ CurrentTarget);
    }

    private void SetNextTarget() {
        //check all playable units
        FindAllPossibleTargets();

        //If we overflow, get back to the beginning
        if (CurrentTarget >= (PlayerUnits.Length-1)) {
            CurrentTarget = 0;
        }
        else {
            CurrentTarget += 1;
        }
        
        //enable or disable user inputs for units disabled.
        SetEnabledUnit(PlayerUnits.Length);
    }
    private void SetPreviousTarget() {
        FindAllPossibleTargets();

        if (CurrentTarget <= 0) {
            CurrentTarget = PlayerUnits.Length-1;
        }
        else {
            CurrentTarget -= 1;
        }
        SetEnabledUnit(PlayerUnits.Length);
    }

    private void FindAllPossibleTargets() {
        // The check to look if any playable is spawned during the game is made only if the player tries to switch unit
        PlayerUnits = GameObject.FindGameObjectsWithTag("Player");
        // Debug.Log ("Playable units : "+ PlayerUnits.Length);
    }

    private void SetEnabledUnit(int PlayerUnitsLength) {
        ActiveTarget = PlayerUnits[CurrentTarget];
        // Debug.Log ("ActiveTarget : "+ ActiveTarget);
        // UIManager.SetTargetType("Unknown");

        for (int i = 0; i < PlayerUnitsLength; i++){
            // If it's a tank :
            if (PlayerUnits[i].GetComponent<TankMovement>()) {
                if (i == CurrentTarget) {
                    PlayerUnits[i].GetComponent<TankMovement>().m_Active = true;
                    PlayerUnits[i].GetComponent<TurretManager>().m_Active = true;
                    UIManager.SetTargetType("Tank");
                    //Debug.Log ("Current target is a tank : "+ PlayerUnits[CurrentTarget].GetComponent<TankMovement>());
                }
                else {
                    PlayerUnits[i].GetComponent<TankMovement>().m_Active = false;
                    PlayerUnits[i].GetComponent<TurretManager>().m_Active = false;
                }
            }
            else if (PlayerUnits[i].GetComponent<AircraftController>()) {
                if (i == CurrentTarget) {
                    PlayerUnits[i].GetComponent<AircraftUserControl4Axis>().m_Active = true;
                    UIManager.SetTargetType("Aircraft");
                    // Debug.Log ("Current target is a plane");
                }
                else {
                    PlayerUnits[i].GetComponent<AircraftUserControl4Axis>().m_Active = false;
                }
            }
            else if (PlayerUnits[i].GetComponent<ShipController>()) {
                if (i == CurrentTarget) {
                    PlayerUnits[i].GetComponent<ShipController>().m_Active = true;
                    UIManager.SetTargetType("Ship");
                }
                else {
                    PlayerUnits[i].GetComponent<ShipController>().m_Active = false;
                } 
            }
        }
        FreeLookCamera.SetActiveTarget(ActiveTarget);
        UIManager.SetActiveTarget(ActiveTarget);
        // UI.UITest.SetActiveTarget(ActiveTarget);
        //Debug.Log ("Current target for player manager : "+ PlayerUnits[CurrentTarget]);
    }

    public void SetMap(bool map) {
        MapActive = map;
        if (map)
            SetEnabledUnit(PlayerUnits.Length);

        UIManager.SetMap(map);

        if (ActiveTarget.GetComponent<ShipController>()) {
            ActiveTarget.GetComponent<ShipController>().SetMap(map);
        }
    }
}