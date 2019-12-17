using System.Collections;
using UnityEngine;
using UnityEngine.UI; 

namespace UI {
    public class UIManager : MonoBehaviour {
        public Text m_UnitName;
        public Text m_UnitHP;
            const string HPDisplay = "{0} HP";
        public Text m_ShipSpeedStep;
            const string ShipSpeedStepDisplay = "Speed Order : {0}";
        public Text m_ShipCurrentSpeed;
            const string ShipCurrentSpeedDisplay = "{0} km/h";
        public Text m_ShipTurningSpeed;
            const string ShipTurningSpeedDisplay = "Direction : {0}";
        private GameObject ActiveTarget;
        private string TargetType;
        private bool DisplayGameUI = true;
        private bool DisplayMapUI = false;

        private void Update() {
            if (DisplayGameUI) {
                float CurrentHP = -200;
                float SpeedStep = -200;
                float CurrentSpeed = -2000;
                float CurrentRotation = -200;

                if (TargetType == "Tank") {
                    if (ActiveTarget.GetComponent<TankHealth>())
                        CurrentHP = ActiveTarget.GetComponent<TankHealth>().GetCurrentHealth();
                } else if (TargetType == "Aircraft") {
                    if (ActiveTarget.GetComponent<AircraftHealth>())
                        CurrentHP = ActiveTarget.GetComponent<AircraftHealth>().GetCurrentHealth();
                } else if (TargetType == "Ship") {
                    if (ActiveTarget.GetComponent<ShipHealth>())
                        CurrentHP = ActiveTarget.GetComponent<ShipHealth>().GetCurrentHealth();
                    if (ActiveTarget.GetComponent<ShipMovement>()){
                        SpeedStep = ActiveTarget.GetComponent<ShipMovement>().GetCurrentSpeedStep();
                        CurrentSpeed = ActiveTarget.GetComponent<ShipMovement>().GetLocalRealSpeed();
                        CurrentRotation = ActiveTarget.GetComponent<ShipMovement>().GetLocalRealRotation();
                    }
                }

                m_UnitName.enabled = true;


                if (CurrentHP < 0) {
                    m_UnitHP.enabled = false;
                } else {
                    m_UnitHP.enabled = true;
                    m_UnitHP.text = string.Format(HPDisplay, CurrentHP); 
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
            } else {
                m_UnitName.enabled = false;
                m_UnitHP.enabled = false;
                m_ShipSpeedStep.enabled = false;
                m_ShipCurrentSpeed.enabled = false;
                m_ShipTurningSpeed.enabled = false;
            }
        }


        public void SetTargetType(string Type) {
            TargetType = Type;
        }

        public void SetActiveTarget(GameObject Target) {
            ActiveTarget = Target;
            m_UnitName.text = ActiveTarget.name;
        }
        
        public void SetMap(bool map) {
            DisplayGameUI = !map;
            DisplayMapUI = map;
        }
    }
}