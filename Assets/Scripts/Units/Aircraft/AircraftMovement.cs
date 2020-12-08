using UnityEngine;

public class AircraftMovement : UnitMovement {

    // reference to the aeroplane that we're controlling
    private AircraftController PlaneController;
    private float m_Throttle;
    private bool m_AirBrakes;
    private float m_Yaw;
    private float roll;
    private float pitch;
    private bool FreeCam = false;

    [HideInInspector] public bool m_Active; 


    // private void Awake(){
    //     // Set up the reference to the aeroplane controller.
    //     PlaneController = GetComponent<AircraftController>();   
    //     FreeCam = false;
    // }

    public void BeginOperations(AircraftController unitController) {
        PlaneController = unitController;
        // Debug.Log("AircraftMovement BeginOperations"+ PlaneController);
    }

    private void FixedUpdate() {
        if (Active && !MapActive && !Dead) {
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
            // Debug.Log("m_Throttle : " + m_Throttle +"m_Yaw : " + m_Yaw);
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
        PlaneController.Move(roll, pitch, m_Yaw, m_Throttle, m_AirBrakes);
    }

    public void SetFreeCamera(bool freeCam){ FreeCam = freeCam; }
}