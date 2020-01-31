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
    private bool Pause = false;
    private bool MapActive = false;
    private bool DamageControl = false;
    public FreeLookCam m_FreeLookCamera;
    public Camera m_MapCamera;
    private GameManager GameManager;
    private GameManager.Teams PlayerTeam;
    private UIManager UIManager;
    private MapManager MapManager;

    private void Start() {
        GameManager = GetComponent<GameManager>();
        PlayerTeam = GameManager.GetPlayer();
        MapManager = GetComponent<MapManager>();
        MapManager.SetMapCamera(m_MapCamera);
        UIManager = GetComponent<UIManager>();
        UIManager.SetPlayerManager(this);
        UIManager.SetFreeLookCamera(m_FreeLookCamera);
        FindAllPossibleTargets();
        SetEnabledUnit(PlayerUnits.Length);
        UIManager.SetCurrentUnitDead(false);                // If the level has restarted, set the unit as not dead (no other check)
    }

    protected void Update() {
        if (ActiveTarget == null)
            SetNextTarget();
        if (!Pause) {     
            if (Input.GetButtonDown ("HideUI"))
                SetHideUI();
            if (Input.GetButtonDown ("OpenMap"))
                SetMap();

            if (Input.GetButtonDown ("SetNextUnit"))
                SetNextTarget();
            if (Input.GetButtonDown ("SetPreviousUnit"))
                SetPreviousTarget();
            //Debug.Log ("Current target : "+ CurrentTarget);
        }

        if (Input.GetButtonDown ("PauseMenu"))
            SetPause();
    }

    private void SetHideUI(){
        // This is used to hide all UI elements
        m_FreeLookCamera.SetHideUI();
        UIManager.SetHideUI();
    }

    public void SetPause() {
        Pause = !Pause;
        UIManager.SetPauseUI(Pause);
        if (Pause) {
            Time.timeScale = Mathf.Approximately(Time.timeScale, 0.0f) ? 1.0f : 0.0f;                   
        } else {
            Time.timeScale = Mathf.Approximately(Time.timeScale, 1.0f) ? 0.0f : 1.0f;
        }
        if (ActiveTarget.GetComponent<ShipController>()) {
            ActiveTarget.GetComponent<ShipController>().SetPause(Pause);
        }
        CheckCameraRotation();
    }

    private void SetNextTarget() {
        //check all playable units
        FindAllPossibleTargets();

        //If we overflow, get back to the beginning
        if (CurrentTarget >= (PlayerUnits.Length-1)) {
            CurrentTarget = 0;
        } else {
            CurrentTarget += 1;
        }
        
        //enable or disable user inputs for units disabled.
        SetEnabledUnit(PlayerUnits.Length);
    }
    private void SetPreviousTarget() {
        FindAllPossibleTargets();

        if (CurrentTarget <= 0) {
            CurrentTarget = PlayerUnits.Length-1;
        } else {
            CurrentTarget -= 1;
        }
        SetEnabledUnit(PlayerUnits.Length);
    }

    private void FindAllPossibleTargets() {
        // The check to look if any playable is spawned during the game is made only if the player tries to switch unit
        // PlayerUnits = GameObject.FindGameObjectsWithTag("Player");
        PlayerUnits = GameObject.FindGameObjectsWithTag(PlayerTeam.ToString("g"));
        // Debug.Log ("Playable units : "+ PlayerUnits.Length);
    }

    private void SetEnabledUnit(int PlayerUnitsLength) {
        ActiveTarget = PlayerUnits[CurrentTarget];
        // Debug.Log ("ActiveTarget : "+ ActiveTarget);
        // Debug.Log ("CurrentTarget : "+ CurrentTarget);
        // Debug.Log ("PlayerUnits.Length : "+ PlayerUnits.Length);
        UIManager.SetTargetType("Unknown");

        for (int i = 0; i < PlayerUnitsLength; i++){
            // If it's a tank :
            if (PlayerUnits[i].GetComponent<TankMovement>()) {
                if (i == CurrentTarget) {
                    PlayerUnits[i].GetComponent<TankMovement>().SetActive(true);
                    PlayerUnits[i].GetComponent<TurretManager>().SetActive(true);
                    UIManager.SetTargetType("Tank");
                    //Debug.Log ("Current target is a tank : "+ PlayerUnits[CurrentTarget].GetComponent<TankMovement>());
                } else {
                    PlayerUnits[i].GetComponent<TankMovement>().SetActive(false);
                    PlayerUnits[i].GetComponent<TurretManager>().SetActive(false);
                }
            }
            else if (PlayerUnits[i].GetComponent<AircraftController>()) {
                if (i == CurrentTarget) {
                    PlayerUnits[i].GetComponent<AircraftUserControl4Axis>().m_Active = true;
                    UIManager.SetTargetType("Aircraft");
                    // Debug.Log ("Current target is a plane");
                } else {
                    PlayerUnits[i].GetComponent<AircraftUserControl4Axis>().m_Active = false;
                }
            }
            else if (PlayerUnits[i].GetComponent<ShipController>()) {
                if (i == CurrentTarget) {
                    PlayerUnits[i].GetComponent<ShipController>().SetActive(true);
                    PlayerUnits[i].GetComponent<ShipController>().SetMap(MapActive);
                    UIManager.SetTargetType("Ship");
                } else {
                    PlayerUnits[i].GetComponent<ShipController>().SetActive(false);
                } 
            }
        }
        m_FreeLookCamera.SetActiveTarget(ActiveTarget);
        UIManager.SetActiveTarget(ActiveTarget);
        //Debug.Log ("Current target for player manager : "+ PlayerUnits[CurrentTarget]);
    }

    private void CheckCameraRotation(){
        if (MapActive || DamageControl || Pause) {
            m_FreeLookCamera.SetRotation(false);
            m_FreeLookCamera.SetMouse(true);
        } else {
            m_FreeLookCamera.SetRotation(true);
            m_FreeLookCamera.SetMouse(false);
        }
    }

    public void SetPlayer(GameManager.Teams PlayerTeam){}

    public void SetScoreMessage(string message) { UIManager.SetScoreMessage(message); }
    public void SetMap() {
        MapActive = !MapActive;
        if (MapActive)
            SetEnabledUnit(PlayerUnits.Length);

        UIManager.SetMap(MapActive);

        MapManager.SetInitialPosition(ActiveTarget);
        MapManager.SetMap(MapActive);

        m_MapCamera.enabled = MapActive;

        if (ActiveTarget.GetComponent<ShipController>()) {
            ActiveTarget.GetComponent<ShipController>().SetMap(MapActive);
        }
        CheckCameraRotation();
    }
    public void SetDamageControl(bool damageControl){
        DamageControl = damageControl;
        CheckCameraRotation();
    }
    public void SetCurrentUnitHealth(float health){ UIManager.SetCurrentUnitHealth(health); }
    public void SetCurrentUnitDead(bool isUnitDead){ UIManager.SetCurrentUnitDead(isUnitDead); }
    public void ChangeSpeedStep(int currentSpeedStep){ UIManager.ChangeSpeedStep(currentSpeedStep); }
    public void SetRotationInput(float rotation){ UIManager.SetRotationInput(rotation); }
    public void SetTurretStatus(string status){ UIManager.SetTurretStatus(status); }

    public void Reset(){Start();}
    public void EndGame(){
        if (Pause){
            SetPause();
        }
        GameManager.EndGame();
    }
}