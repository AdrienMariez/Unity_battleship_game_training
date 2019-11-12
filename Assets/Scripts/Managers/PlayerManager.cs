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

    private void Start()
    {
        FindAllPossibleTargets();
        SetEnabledUnit(PlayerUnits.Length);
    }

    protected void Update()
    {
        if (m_Active)
        {
            // Look if the unit is changed, otherwise, do nothing
            if (Input.GetButtonDown ("SetNextUnit")){
                SetNextTarget();
            }
            if (Input.GetButtonDown ("SetPreviousUnit")){
                SetPreviousTarget();
            }
        }
        //Debug.Log ("Current target : "+ CurrentTarget);
    }

    private void SetNextTarget()
    {
        //check all playable units
        FindAllPossibleTargets();

        //If we overflow, get back to the beginning
        if (CurrentTarget >= (PlayerUnits.Length-1))
        {
            CurrentTarget = 0;
        }
        else
        {
            CurrentTarget += 1;
        }
        
        //enable or disable user inputs for units disabled.
        SetEnabledUnit(PlayerUnits.Length);
    }
    private void SetPreviousTarget()
    {
        FindAllPossibleTargets();

        if (CurrentTarget <= 0)
        {
            CurrentTarget = PlayerUnits.Length-1;
        }
        else
        {
            CurrentTarget -= 1;
        }
        SetEnabledUnit(PlayerUnits.Length);
    }

    private void FindAllPossibleTargets()
    {
        // The check to look if any playable is spawned during the game is made only if the player tries to switch unit
        PlayerUnits = GameObject.FindGameObjectsWithTag("Player");
        // Debug.Log ("Playable units : "+ PlayerUnits.Length);
    }

    private void SetEnabledUnit(int PlayerUnitsLength)
    {
        ActiveTarget = PlayerUnits[CurrentTarget];
        // Debug.Log ("ActiveTarget : "+ ActiveTarget);

        for (int i = 0; i < PlayerUnitsLength; i++)
        {
            // If it's a tank :
            if (PlayerUnits[i].GetComponent<TankMovement>())
            {
                if (i == CurrentTarget)
                {
                    PlayerUnits[i].GetComponent<TankMovement>().m_Active = true;
                    PlayerUnits[i].GetComponent<TankShooting>().m_Active = true;
                    //Debug.Log ("Current target is a tank : "+ PlayerUnits[CurrentTarget].GetComponent<TankMovement>());
                }
                else
                {
                    PlayerUnits[i].GetComponent<TankMovement>().m_Active = false;
                    PlayerUnits[i].GetComponent<TankShooting>().m_Active = false;
                }
            }
            else if (PlayerUnits[i].GetComponent<AircraftController>())
            {
                if (i == CurrentTarget)
                {
                    PlayerUnits[i].GetComponent<AircraftUserControl4Axis>().m_Active = true;
                    //Debug.Log ("Current target is a plane : "+ PlayerUnits[CurrentTarget].GetComponent<AircraftUserControl4Axis>());
                }
                else
                {
                    PlayerUnits[i].GetComponent<AircraftUserControl4Axis>().m_Active = false;
                }
            }
            /*
            else if (PlayerUnits[i].GetComponent<ShipMovement>())
            {
                if (i == CurrentTarget)
                {
                    PlayerUnits[i].GetComponent<ShipMovement>().m_Active = true;
                    //Debug.Log ("Current target is a plane : "+ PlayerUnits[CurrentTarget].GetComponent<AircraftUserControl4Axis>());
                }
                else
                {
                    PlayerUnits[i].GetComponent<ShipMovement>().m_Active = false;
                }
            }
            */
            else if (PlayerUnits[i].GetComponent<ShipBehaviour>())
            {
                if (i == CurrentTarget)
                {
                    PlayerUnits[i].GetComponent<ShipBehaviour>().m_Active = true;
                }
                else
                {
                    PlayerUnits[i].GetComponent<ShipBehaviour>().m_Active = false;
                }
            }
        }
        //Debug.Log ("Current target for player manager : "+ PlayerUnits[CurrentTarget]);
    }
}