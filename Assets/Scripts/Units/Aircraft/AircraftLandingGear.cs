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
        // private bool InAirportZone = true;
        


        public void BeginOperations(AircraftController unitController, Rigidbody rb) {
            m_Plane = unitController;
            m_Rigidbody = rb;
            m_Animator = GetComponent<Animator>();
            // m_Animator.SetInteger("GearState", (int) GearState.Raised);
        }

        // public void SetInAirfieldZone(bool action) {
        //     InAirportZone = action;
        //     // Unused...could be used to land planes
        // }

        public void SetGear(bool action) {
            if (action) {
                Debug.Log ("Airfield ''o'' expected");
                m_State = GearState.Lowered;
                m_Animator.SetInteger("GearState", (int) m_State);
            } else {
                Debug.Log ("Airfield ..o..");
                m_State = GearState.Raised;
                m_Animator.SetInteger("GearState", (int) m_State);
            }
        }
    }
