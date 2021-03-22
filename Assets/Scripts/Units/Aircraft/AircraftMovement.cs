using UnityEngine;

public class AircraftMovement : UnitMovement {

    // reference to the aeroplane that we're controlling
    private AircraftController PlaneController;
    private float Throttle = 0;
    private bool AirBrakes = false;
    private float Yaw;
    private float roll;
    private float pitch;
    private bool FreeCam = false; public void SetFreeCamera(bool _b){ FreeCam = _b; }

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
            AirBrakes = Input.GetButton("FireMainPlane");
            Yaw = Input.GetAxis("HorizontalPlane");
            Throttle = Input.GetAxis("VerticalPlane");
            // Debug.Log("Throttle : " + Throttle +"Yaw : " + Yaw);
        } else {
            // Disable user input if the unit isn't player controlled
            // Obviously if the plane isn't player controlled, there will be an AI needed
            // roll = Input.GetAxis("Submit");
            // pitch = Input.GetAxis("Submit");
            // AirBrakes = Input.GetButton("Submit");
            // Yaw = Input.GetAxis("Submit");
            // Throttle = Input.GetAxis("Submit");
        }

        // Pass the input to the aeroplane
        PlaneController.Move(roll, pitch, Yaw, Throttle, AirBrakes);
    }

    public void SetAISpeed(int speedProportion){ 
        Throttle = speedProportion;
        // Debug.Log("SetAISpeed - Throttle : " + speedProportion);
    }
    public void SetAIPitch(int pitchProportion){ 
        pitch = pitchProportion;
        // Debug.Log("SetAISpeed - Throttle : " + speedProportion);
    }
}