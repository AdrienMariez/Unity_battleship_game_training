using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using FreeLookCamera;
using Crest;

namespace UI {
    public class UIManager : MonoBehaviour {
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
            const string TurretsTargetRangeDisplay = "Targeting range : {0}";

        private GameObject ActiveTarget;
        private string TargetType;
        float StartingHP = -200;
        private bool DisplayGameUI = true;
        private bool DisplayMapUI = false;

        private GameObject[] Turrets;
        private FreeLookCam FreeLookCam;

        private void Start() {
            FreeLookCam = GameObject.Find("FreeLookCameraRig").GetComponent<FreeLookCam>();
        }

        private void Update() {
            if (DisplayGameUI) {
                float CurrentHP = -200;
                float SpeedStep = -200;
                float CurrentSpeed = Mathf.Round(ActiveTarget.GetComponent<Rigidbody>().velocity.magnitude);
                float CurrentRotation = -200;
                string TurretStatus = "";
                float TurretTargetRange = -200;


                if (TargetType == "Tank") {
                    if (ActiveTarget.GetComponent<TankHealth>())
                        CurrentHP = Mathf.Round(ActiveTarget.GetComponent<TankHealth>().GetCurrentHealth());
                } else if (TargetType == "Aircraft") {
                    if (ActiveTarget.GetComponent<AircraftHealth>())
                        CurrentHP = Mathf.Round(ActiveTarget.GetComponent<AircraftHealth>().GetCurrentHealth());
                } else if (TargetType == "Ship") {
                    if (ActiveTarget.GetComponent<ShipHealth>())
                        CurrentHP = Mathf.Round(ActiveTarget.GetComponent<ShipHealth>().GetCurrentHealth());
                    if (ActiveTarget.GetComponent<ShipMovement>()){
                        SpeedStep = ActiveTarget.GetComponent<ShipMovement>().GetCurrentSpeedStep();
                        CurrentRotation = Mathf.Round(ActiveTarget.GetComponent<ShipMovement>().GetLocalRealRotation());
                    }
                }

                if (ActiveTarget.GetComponent<TurretManager>()){
                    TurretTargetRange = Mathf.Round(FreeLookCam.GetTargetPointRange());
                    for (int i = 0; i < Turrets.Length; i++){
                        TurretStatus += Turrets[i].GetComponent<TurretFireManager>().GetTurretStatus();
                        if (TurretTargetRange > 99000f) {
                            if (Turrets[i].GetComponent<TurretFireManager>().m_DirectorTurret)
                            TurretTargetRange = Mathf.Round(Turrets[i].GetComponent<TurretFireManager>().GetTargetRange());
                        }
                        // if (Turrets[i].GetComponent<TurretFireManager>().m_DirectorTurret)
                        //     TurretTargetRange = Turrets[i].GetComponent<TurretFireManager>().GetTargetRange();
                    }
                }

                m_UnitName.enabled = true;

                if (CurrentHP < 0 || StartingHP < 0) {
                    m_UnitHP.enabled = false;
                } else {
                    m_UnitHP.enabled = true;
                    m_UnitHP.text = string.Format(HPDisplay, CurrentHP, StartingHP); 
                }

                if (SpeedStep == -200) {
                    m_ShipSpeedStep.enabled = false;
                } else {
                    m_ShipSpeedStep.enabled = true;
                    m_ShipSpeedStep.text = string.Format(ShipSpeedStepDisplay, SpeedStep);
                }

                if (CurrentSpeed == -2000) {
                    m_ShipCurrentSpeed.enabled = false;
                } else {
                    m_ShipCurrentSpeed.enabled = true;
                    m_ShipCurrentSpeed.text = string.Format(ShipCurrentSpeedDisplay, CurrentSpeed); 
                }

                if (CurrentRotation == -200) {
                    m_ShipTurningSpeed.enabled = false;
                } else {
                    m_ShipTurningSpeed.enabled = true;
                    m_ShipTurningSpeed.text = string.Format(ShipTurningSpeedDisplay, CurrentRotation); 
                }

                if (!ActiveTarget.GetComponent<TurretManager>()) {
                    m_TurretsStatus.enabled = false;
                    m_TurretsTargetRange.enabled = false;
                } else {
                    m_TurretsStatus.enabled = true;
                    m_TurretsStatus.text = string.Format(TurretsStatusDisplay, TurretStatus); 
                    m_TurretsTargetRange.enabled = true;
                    m_TurretsTargetRange.text = string.Format(TurretsTargetRangeDisplay, TurretTargetRange); 
                }
            } else {
                m_UnitName.enabled = false;
                m_UnitHP.enabled = false;
                m_ShipSpeedStep.enabled = false;
                m_ShipCurrentSpeed.enabled = false;
                m_ShipTurningSpeed.enabled = false;
                m_TurretsStatus.enabled = false;
                m_TurretsTargetRange.enabled = false;
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
            StartingHP = -200;

            if (TargetType == "Tank") {
                if (ActiveTarget.GetComponent<TankHealth>())
                    StartingHP = ActiveTarget.GetComponent<TankHealth>().GetStartingHealth();
            } else if (TargetType == "Aircraft") {
                if (ActiveTarget.GetComponent<AircraftHealth>())
                    StartingHP = ActiveTarget.GetComponent<AircraftHealth>().GetStartingHealth();
            } else if (TargetType == "Ship") {
                if (ActiveTarget.GetComponent<ShipHealth>())
                    StartingHP = ActiveTarget.GetComponent<ShipHealth>().GetStartingHealth();
            }
        }
        
        public void SetMap(bool map) {
            DisplayGameUI = !map;
            DisplayMapUI = map;
        }
    }
}