using System;
using UnityEngine;

//[RequireComponent(typeof (AeroplaneController))]
public class AircraftUserControl4Axis : MonoBehaviour
{
    // these max angles are only used on mobile, due to the way pitch and roll input are handled
    public float maxRollAngle = 80;
    public float maxPitchAngle = 80;

    // reference to the aeroplane that we're controlling
    private AircraftController Aeroplane;
    private float m_Throttle;
    private bool m_AirBrakes;
    private float m_Yaw;
    private float roll;
    private float pitch;
    private bool FreeCam;

    [HideInInspector] public bool m_Active; 


    private void Awake(){
        // Set up the reference to the aeroplane controller.
        Aeroplane = GetComponent<AircraftController>();   
        FreeCam = false;
    }


    private void FixedUpdate() {
        if (Input.GetButtonDown ("FreeCamera"))
            SetFreeCam();
        if (m_Active) {
            // Read input for the pitch, yaw, roll and throttle of the aeroplane.
            if (FreeCam){
                roll = 100;
                pitch = 0;
            } else {
                roll = Input.GetAxis("Mouse X");
                pitch = Input.GetAxis("Mouse Y");
            }
            m_AirBrakes = Input.GetButton("FireMainPlane");
            m_Yaw = Input.GetAxis("HorizontalPlane");
            m_Throttle = Input.GetAxis("VerticalPlane");
        } else {
            // Disable user input if the unit isn't player controlled
            // Obviously if the plane isn't player controlled, there will be an AI needed
            roll = Input.GetAxis("Submit");
            pitch = Input.GetAxis("Submit");
            m_AirBrakes = Input.GetButton("Submit");
            m_Yaw = Input.GetAxis("Submit");
            m_Throttle = Input.GetAxis("Submit");
        }

        // Pass the input to the aeroplane
        Aeroplane.Move(roll, pitch, m_Yaw, m_Throttle, m_AirBrakes);
    }

    private void SetFreeCam(){ FreeCam = !FreeCam; }
}