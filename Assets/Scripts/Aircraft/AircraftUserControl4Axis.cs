using System;
using UnityEngine;

//[RequireComponent(typeof (AeroplaneController))]
public class AircraftUserControl4Axis : MonoBehaviour
{
    // these max angles are only used on mobile, due to the way pitch and roll input are handled
    public float maxRollAngle = 80;
    public float maxPitchAngle = 80;

    // reference to the aeroplane that we're controlling
    private AircraftController m_Aeroplane;
    private float m_Throttle;
    private bool m_AirBrakes;
    private float m_Yaw;
    float roll;
    float pitch;

    [HideInInspector] public bool m_Active; 


    private void Awake()
    {
        // Set up the reference to the aeroplane controller.
        m_Aeroplane = GetComponent<AircraftController>();
    }


    private void FixedUpdate()
    {
        if (m_Active)
        {
            // Read input for the pitch, yaw, roll and throttle of the aeroplane.
            if (Input.GetButton ("FreeCamera")){
                roll = Input.GetAxis("Empty");
                pitch = Input.GetAxis("Empty");
            }
            else
            {
                roll = Input.GetAxis("Mouse X");
                pitch = Input.GetAxis("Mouse Y");
            }
            m_AirBrakes = Input.GetButton("FireMainPlane");
            m_Yaw = Input.GetAxis("HorizontalPlane");
            m_Throttle = Input.GetAxis("VerticalPlane");
        }
        else
        {
            // Disable user input if the unit isn't player controlled
            roll = Input.GetAxis("Empty");
            pitch = Input.GetAxis("Empty");
            m_AirBrakes = Input.GetButton("Empty");
            m_Yaw = Input.GetAxis("Empty");
            m_Throttle = Input.GetAxis("Empty");
        }
        #if MOBILE_INPUT
            AdjustInputForMobileControls(ref roll, ref pitch, ref m_Throttle);
        #endif
        // Pass the input to the aeroplane
        m_Aeroplane.Move(roll, pitch, m_Yaw, m_Throttle, m_AirBrakes);
    }


    private void AdjustInputForMobileControls(ref float roll, ref float pitch, ref float throttle)
    {
        // because mobile tilt is used for roll and pitch, we help out by
        // assuming that a centered level device means the user
        // wants to fly straight and level!

        // this means on mobile, the input represents the *desired* roll angle of the aeroplane,
        // and the roll input is calculated to achieve that.
        // whereas on non-mobile, the input directly controls the roll of the aeroplane.

        float intendedRollAngle = roll*maxRollAngle*Mathf.Deg2Rad;
        float intendedPitchAngle = pitch*maxPitchAngle*Mathf.Deg2Rad;
        roll = Mathf.Clamp((intendedRollAngle - m_Aeroplane.RollAngle), -1, 1);
        pitch = Mathf.Clamp((intendedPitchAngle - m_Aeroplane.PitchAngle), -1, 1);
    }
}