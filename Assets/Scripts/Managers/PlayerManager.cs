using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// This class stores the currently played unit and allows to enable player controls for units or disable them
public class PlayerManager : MonoBehaviour
{
    //public FreeLookCam m_FreeLookCam;

    [HideInInspector] public GameObject[] PlayerUnits;
    [HideInInspector] public int CurrentTarget = 0;
    [HideInInspector] public bool m_Active;

    // This var is only to be sent to other scripts.
    [HideInInspector] public GameObject ActiveTarget;

    [HideInInspector] public bool m_MapActive;
    private bool _m_MapActive;

    private void Start() {
        FindAllPossibleTargets();
        SetEnabledUnit(PlayerUnits.Length);
        m_MapActive = false;
        _m_MapActive = false;
    }

    protected void Update() {
        // Look if the unit is changed, otherwise, do nothing
        if (Input.GetButtonDown ("SetNextUnit")) {
            SetNextTarget();
        }
        if (Input.GetButtonDown ("SetPreviousUnit")) {
            SetPreviousTarget();
        }
        if (m_MapActive != _m_MapActive) {
            SetEnabledUnit(PlayerUnits.Length);
            _m_MapActive = !_m_MapActive;
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

        for (int i = 0; i < PlayerUnitsLength; i++){
            if (PlayerUnits[i].GetComponent<TurretManager>()) {
                if (i == CurrentTarget && !m_MapActive) {
                    PlayerUnits[i].GetComponent<TurretManager>().m_Active = true;
                    // Debug.Log ("Current turrets activated : "+ PlayerUnits[CurrentTarget]);
                }
                else {
                    PlayerUnits[i].GetComponent<TurretManager>().m_Active = false;
                }
            }
            // If it's a tank :
            if (PlayerUnits[i].GetComponent<TankMovement>()) {
                if (i == CurrentTarget && !m_MapActive) {
                    PlayerUnits[i].GetComponent<TankMovement>().m_Active = true;
                    //Debug.Log ("Current target is a tank : "+ PlayerUnits[CurrentTarget].GetComponent<TankMovement>());
                }
                else {
                    PlayerUnits[i].GetComponent<TankMovement>().m_Active = false;
                }
            }
            else if (PlayerUnits[i].GetComponent<AircraftController>()) {
                if (i == CurrentTarget && !m_MapActive) {
                    PlayerUnits[i].GetComponent<AircraftUserControl4Axis>().m_Active = true;
                    //Debug.Log ("Current target is a plane : "+ PlayerUnits[CurrentTarget].GetComponent<AircraftUserControl4Axis>());
                }
                else {
                    PlayerUnits[i].GetComponent<AircraftUserControl4Axis>().m_Active = false;
                }
            }
            else if (PlayerUnits[i].GetComponent<ShipController>()) {
                if (i == CurrentTarget && !m_MapActive) {
                    PlayerUnits[i].GetComponent<ShipController>().m_Active = true;
                }
                else {
                    PlayerUnits[i].GetComponent<ShipController>().m_Active = false;
                }
            }
        }
        //Debug.Log ("Current target for player manager : "+ PlayerUnits[CurrentTarget]);
    }

}