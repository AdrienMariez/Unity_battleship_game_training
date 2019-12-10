using System;
using UnityEngine;
    public class AircraftLandingGear : MonoBehaviour
    {

        private enum GearState
        {
            Raised = -1,
            Lowered = 1
        }

        // The landing gear can be raised and lowered at differing altitudes.
        // The gear is only lowered when descending, and only raised when climbing.

        // this script detects the raise/lower condition and sets a parameter on
        // the animator to actually play the animation to raise or lower the gear.

        public float raiseAtAltitude = 40;
        public float lowerAtAltitude = 40;

        private GearState m_State = GearState.Lowered;
        private Animator m_Animator;
        private Rigidbody m_Rigidbody;
        private AircraftController m_Plane;
        [HideInInspector] public GameObject[] landingZones;

        // Use this for initialization
        private void Start()
        {
            m_Plane = GetComponent<AircraftController>();
            m_Animator = GetComponent<Animator>();
            m_Rigidbody = GetComponent<Rigidbody>();
        }


        // Update is called once per frame
        private void Update()
        {
            // bool x = this.GetComponent<AircraftUserControl4Axis>().m_Active;

            float speed = Mathf.Abs (m_Rigidbody.velocity.x) + Mathf.Abs (m_Rigidbody.velocity.y) + Mathf.Abs (m_Rigidbody.velocity.z);

            if (speed < 70) {
                //Debug.Log ("speed < 70");
                landingZones = GameObject.FindGameObjectsWithTag("LandingZone");
                bool landingZoneInProximity = false;
                foreach (var zone in landingZones) {
                    if ( (m_Rigidbody.transform.position - zone.transform.position).magnitude < 200) {
                        landingZoneInProximity = true;
                    }
                }
                if (landingZoneInProximity){
                    Debug.Log ("landingZoneInProximity = "+landingZoneInProximity);
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

            // Debug.Log ("m_Plane.Altitude = "+m_Rigidbody.transform.position.y);
            // Debug.Log ("speed = "+speed);


            // if (m_State == GearState.Lowered && m_Plane.Altitude > raiseAtAltitude && m_Rigidbody.velocity.y > 0) {
            //     m_State = GearState.Raised;
            //     if (!m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Raised"))
            //         m_Animator.Play("Raised", 0, 0);
            // }

            // if (m_State == GearState.Raised && m_Plane.Altitude < lowerAtAltitude && m_Rigidbody.velocity.y < 0)
            // {
            //     m_State = GearState.Lowered;
            //     if (!m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Lowered"))
            //         m_Animator.Play("Lowered", 0, 0);
            // }

            Debug.Log ("m_State = "+m_State);

            // set the parameter on the animator controller to trigger the appropriate animation
            m_Animator.SetInteger("GearState", (int) m_State);
        }
    }
