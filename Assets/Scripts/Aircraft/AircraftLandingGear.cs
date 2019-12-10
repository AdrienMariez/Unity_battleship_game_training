using System;
using UnityEngine;
    public class AircraftLandingGear : MonoBehaviour {

        private enum GearState {
            Raised = -1,
            Lowered = 1
        }

        // The landing gear can be raised and lowered at differing altitudes.
        // The gear is only lowered when descending, and only raised when climbing.

        // this script detects the raise/lower condition and sets a parameter on
        // the animator to actually play the animation to raise or lower the gear.

        public float LandingZoneDistance = 200;

        private GearState m_State = GearState.Lowered;
        private Animator m_Animator;
        private Rigidbody m_Rigidbody;
        private AircraftController m_Plane;
        [HideInInspector] public GameObject[] landingZones;

        // Use this for initialization
        private void Start() {
            m_Plane = GetComponent<AircraftController>();
            m_Animator = GetComponent<Animator>();
            m_Rigidbody = GetComponent<Rigidbody>();
        }

        private void Update() {
            float speed = Mathf.Abs (m_Rigidbody.velocity.x) + Mathf.Abs (m_Rigidbody.velocity.y) + Mathf.Abs (m_Rigidbody.velocity.z);

            if (speed < 70) {
                //Debug.Log ("speed < 70");
                landingZones = GameObject.FindGameObjectsWithTag("LandingZone");
                bool landingZoneInProximity = false;
                foreach (var zone in landingZones) {
                    if ( (m_Rigidbody.transform.position - zone.transform.position).magnitude < LandingZoneDistance) {
                        landingZoneInProximity = true;
                    }
                }
                if (landingZoneInProximity){
                    // Debug.Log ("landingZoneInProximity = "+landingZoneInProximity);
                    if (m_State == GearState.Raised){
                        m_State = GearState.Lowered;
                    }
                } else {
                    m_State = GearState.Raised;
                }
            } else if (m_State == GearState.Lowered) {
                m_State = GearState.Raised;
            }

            // Debug.Log ("m_State = "+m_State);

            // set the parameter on the animator controller to trigger the appropriate animation
            m_Animator.SetInteger("GearState", (int) m_State);
        }
    }
