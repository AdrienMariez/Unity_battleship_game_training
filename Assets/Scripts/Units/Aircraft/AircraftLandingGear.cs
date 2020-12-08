using System;
using UnityEngine;
    public class AircraftLandingGear : MonoBehaviour {

        private enum GearState {
            Raised = -1,
            Lowered = 1,
            Neutral = 0
        }

        // The landing gear can be raised and lowered at differing altitudes.
        // The gear is only lowered when descending, and only raised when climbing.

        // this script detects the raise/lower condition and sets a parameter on
        // the animator to actually play the animation to raise or lower the gear.
        private GearState m_State = GearState.Raised;
        private Animator m_Animator;
        private Rigidbody m_Rigidbody;
        private AircraftController m_Plane;
        private bool InAirportZone = false;
        


        public void BeginOperations(AircraftController unitController, Rigidbody rb) {
            m_Plane = unitController;
            m_Rigidbody = rb;
            m_Animator = GetComponent<Animator>();
        }

        private void Update() {
            // float speed = Mathf.Abs (m_Rigidbody.velocity.x) + Mathf.Abs (m_Rigidbody.velocity.y) + Mathf.Abs (m_Rigidbody.velocity.z);
        }

        public void SetInAirfieldZone(bool action) {
            InAirportZone = action;
            SetCurrentAnimationState();
        }

        private void SetCurrentAnimationState() {
            if (InAirportZone) {
                if (m_State != GearState.Lowered) {
                    // Landing gear is out
                    // Debug.Log ("Airfield ''o'' expected");
                    m_State = GearState.Lowered;
                    m_Animator.SetInteger("GearState", (int) m_State);
                }
            } else {
                if (m_State != GearState.Raised) {
                    // Landing gear is folded inside
                    // Debug.Log ("Airfield ..o..");
                    m_State = GearState.Raised;
                    m_Animator.SetInteger("GearState", (int) m_State);
                }
            }
        }
    }
