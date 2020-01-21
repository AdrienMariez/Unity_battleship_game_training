using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using FreeLookCamera;
using Crest;

namespace UI {
    public class UIManager : MonoBehaviour {
        [Header("Units UI")]
        public Text m_UnitName;
        public Text m_UnitHP;
            const string HPDisplay = "{0}/{1} HP";
        public Text m_ShipSpeedStep;
            const string ShipSpeedStepDisplay = "Speed Order : {0}";
        public Text m_ShipCurrentSpeed;
            const string ShipCurrentSpeedDisplay = "{0} km/h";
        public Text m_ShipTurningSpeed;
            const string ShipTurningSpeedDisplay = "Direction : {0}";
        public Text m_TurretsStatus;
            const string TurretsStatusDisplay = "{0}";
        public Text m_TurretsTargetRange;
            const string TurretsTargetRangeDisplayMeter = "Targeting range : {0} m";
            const string TurretsTargetRangeDisplayKilometer = "Targeting range : {0} km";

        private GameObject ActiveTarget;
        private string TargetType;
        private float StartingHP;
        private float SpeedStep;
        private float CurrentRotation;
        private float CurrentHP;
        private float TurretTargetRange;
        private string TurretStatus = "";
        private bool CurrentUnitDead = false;
        private bool DisplayGameUI = true;
        private bool DisplayMapUI = false;
        private bool DisplayUI = true;

        private GameObject[] Turrets;
        private FreeLookCam FreeLookCam;

        private void Update() {
            if (DisplayGameUI) {
                float CurrentSpeed = Mathf.Round(ActiveTarget.GetComponent<Rigidbody>().velocity.magnitude);
                m_ShipCurrentSpeed.text = string.Format(ShipCurrentSpeedDisplay, CurrentSpeed);

                if (TargetType == "Tank") {
                    CurrentHP = Mathf.Round(ActiveTarget.GetComponent<TankHealth>().GetCurrentHealth());
                    if (ActiveTarget.GetComponent<TurretManager>()) {
                        TurretStatus = ActiveTarget.GetComponent<TurretManager>().GetTurretStatus();
                        m_TurretsStatus.text = string.Format(TurretsStatusDisplay, TurretStatus);
                    }
                } else if (TargetType == "Aircraft") {
                    CurrentHP = Mathf.Round(ActiveTarget.GetComponent<AircraftHealth>().GetCurrentHealth());
                }

                if (ActiveTarget.GetComponent<TurretManager>()) {
                    TurretTargetRange = ActiveTarget.GetComponent<TurretManager>().GetTargetRange();
                    if (TurretTargetRange > 999) {
                        TurretTargetRange = (Mathf.Round(TurretTargetRange / 100)) / 10f;
                        m_TurretsTargetRange.text = string.Format(TurretsTargetRangeDisplayKilometer, TurretTargetRange);
                    } else {
                        m_TurretsTargetRange.text = string.Format(TurretsTargetRangeDisplayMeter, TurretTargetRange);
                    }
                }
            }
        }

        public void SetHideUI(){
            DisplayUI = !DisplayUI;
            SetDisplayGameUI();
        }

        private void SetDisplayGameUI(){
            m_UnitName.enabled = false;
            m_UnitHP.enabled = false;
            m_ShipSpeedStep.enabled = false;
            m_ShipCurrentSpeed.enabled = false;
            m_ShipTurningSpeed.enabled = false;
            m_TurretsStatus.enabled = false;
            m_TurretsTargetRange.enabled = false;

            // Debug.Log (DisplayGameUI+" - "+DisplayMapUI+" - "+DisplayUI);

            if (DisplayGameUI && !DisplayMapUI && DisplayUI){
                m_UnitName.enabled = true;
                m_UnitHP.enabled = true;
                if (TargetType == "Tank") {
                    
                } else if (TargetType == "Aircraft") {
                    
                } else if (TargetType == "Ship") {
                    m_ShipSpeedStep.enabled = true;
                    m_ShipCurrentSpeed.enabled = true;
                    m_ShipTurningSpeed.enabled = true;
                }
                if (ActiveTarget.GetComponent<TurretManager>()) {
                    m_TurretsStatus.enabled = true;
                    m_TurretsTargetRange.enabled = true;
                }
            }

        }

        public void SetTargetType(string Type) {
            TargetType = Type;
        }

        public void SetActiveTarget(GameObject Target) {
            ActiveTarget = Target;
            m_UnitName.text = ActiveTarget.name;
            if (ActiveTarget.GetComponent<TurretManager>()) {
                Turrets = ActiveTarget.GetComponent<TurretManager>().GetTurrets();
            }

            if (TargetType == "Tank") {
                if (ActiveTarget.GetComponent<TankHealth>())
                    StartingHP = ActiveTarget.GetComponent<TankHealth>().GetStartingHealth();
            } else if (TargetType == "Aircraft") {
                if (ActiveTarget.GetComponent<AircraftHealth>())
                    StartingHP = ActiveTarget.GetComponent<AircraftHealth>().GetStartingHealth();
            } else if (TargetType == "Ship") {
                StartingHP = ActiveTarget.GetComponent<ShipHealth>().GetStartingHealth();
                SetCurrentUnitHealth(ActiveTarget.GetComponent<ShipHealth>().GetCurrentHealth());
                ChangeSpeedStep(ActiveTarget.GetComponent<ShipMovement>().GetCurrentSpeedStep());
                CurrentUnitDead = ActiveTarget.GetComponent<ShipController>().GetDeath();
            }
            SetDisplayGameUI();
        }
        
        public void SetFreeLookCamera(FreeLookCam freeLookCam){ FreeLookCam = freeLookCam; }
        public void SetMap(bool map) {
            DisplayMapUI = map;
            if (!CurrentUnitDead)
                DisplayGameUI = !map;
            SetDisplayGameUI();
        }
        public void SetCurrentUnitHealth(float health){ CurrentHP = health; m_UnitHP.text = string.Format(HPDisplay, CurrentHP, StartingHP); }
        public void ChangeSpeedStep(int currentSpeedStep){ SpeedStep = currentSpeedStep; m_ShipSpeedStep.text = string.Format(ShipSpeedStepDisplay, SpeedStep); }
        public void SetRotationInput(float rotation){ CurrentRotation = rotation; m_ShipTurningSpeed.text = string.Format(ShipTurningSpeedDisplay, CurrentRotation); }
        public void SetTurretStatus(string status){ TurretStatus = status; m_TurretsStatus.text = string.Format(TurretsStatusDisplay, TurretStatus); }
        public void SetCurrentUnitDead(bool isUnitDead) {
            // If CurrentUnitDead == true, the game Display should not be shown ! Only the map should work.
            CurrentUnitDead = isUnitDead;
            DisplayGameUI = !isUnitDead;
            SetDisplayGameUI();
        }
    }
}