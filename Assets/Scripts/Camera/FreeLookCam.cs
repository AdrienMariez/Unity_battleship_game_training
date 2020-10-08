using System;
using UnityEngine;

namespace FreeLookCamera {
    public class FreeLookCam : MonoBehaviour {
        // This script is designed to be placed on the root object of a camera rig,
        // comprising 3 gameobjects, each parented to the next:

        // 	Camera Rig
        // 		Pivot
        // 			Camera
        private Camera Cam; // Main camera
        private Transform Pivot; // the point at which the camera points to
        private Transform Axis; // the point at which the camera pivots around

        [Header("Camera")]
            [Tooltip("Field of View")] [Range(1f, 150f)] [SerializeField] private float m_FieldOfView = 60f;
            [Tooltip("How fast the rig will move to keep up with the target's position.")] [SerializeField] private float m_MoveSpeed = 20f;
            [Tooltip("How fast the rig will rotate from user input.")] [Range(0f, 10f)] [SerializeField] private float m_TurnSpeed = 0.02f;
            [Tooltip("How much smoothing to apply to the turn input, to reduce mouse-turn jerkiness")] [SerializeField] private float m_TurnSmoothing = 0f;
            [Tooltip("The maximum vertical value for the rotation of the pivot.")] [SerializeField] private float m_TiltMax = 75f;
            [Tooltip("The minimum vertical value for the rotation of the pivot.")] [SerializeField] private float m_TiltMin = 45f;
        [Header("Focused camera")]
            [Tooltip("Field of View when focused")] [Range(1f, 150f)] [SerializeField] private float m_FieldOfViewFocus = 40f;
            [Tooltip("Multiplier for the Turn speed when focused")] [Range(0f, 1f)] [SerializeField] private float m_TurnSpeedFocus = 0.2f;
        [Header("Basic position values")]
            [Tooltip("Those variables should be overwritten for each unit, but basic static values are stored here.")]
            public float m_CameraDistance = 12, m_CameraHeight = 2, m_CameraLateralOffset = 0;
            private float CameraDistance, CameraHeight, CameraLateralOffset;        // X, Y, Z

        [Header("Raycast")]
            [Tooltip("Layer to filter what the raycast will hit.")] public LayerMask RaycastLayerMask;
            private GameObject m_RaycastProjector;
        public RaycastHit RaycastHit;

        private float LookAngle;                    // The rig's y axis rotation.
        private float TiltAngle;                    // The pivot's x axis rotation.
        private float CameraTiltPercentage;
		private Vector3 AxisEulers;
		private Quaternion AxisTargetRot;
		private Quaternion TransformTargetRot;
        private bool AllowCameraRotation = true;
        private bool FreeCamera = false;
        private bool DisplayUI = true;

        private float TiltMin, TiltMax;                                         // Current used Tilt limitations

        private Vector3 RaycastTargetPosition;                                  // Point set by the camera raycast
        private Vector3 RaycastAbstractTargetPosition;                          // Point set very far by the raycast
        private float RaycastRange;                                             // Distance from unit to point
        private GameObject ActivePlayerUnit;                                    // Current controlled unit
        private Transform ActivePlayerUnitTransform;                                    // Current controlled unit transform
        private WorldUnitsManager.UnitCategories ActivePlayerUnitCategory;
        private TurretFireManager.TurretRole CurrentControlledTurretRole;

        protected virtual void Start() {
			AxisEulers = Axis.rotation.eulerAngles;

	        AxisTargetRot = Axis.transform.localRotation;
			TransformTargetRot = transform.localRotation;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            CameraDistance = m_CameraDistance;
            CameraHeight = m_CameraHeight;
            CameraLateralOffset = m_CameraLateralOffset;
        }

        private void Awake(){
            Cam = GetComponentInChildren<Camera>();
            Pivot = GetComponentInChildren<Camera>().transform.parent;
            Axis = Pivot.parent;

            m_RaycastProjector = GameObject.Find("RaycastProjector");
        }

        protected void Update() {
            // Debug.Log ("m_Axis   : "+ m_Axis);
            if (ActivePlayerUnit != null) {
                FollowTarget(Time.deltaTime);

                if (Input.GetButton ("FocusCamera")){
                    Cam.fieldOfView = m_FieldOfViewFocus;
                    Cam.transform.localRotation = Quaternion.Euler(-20, 0, 0);
                } else {
                    Cam.fieldOfView = m_FieldOfView;
                    Cam.transform.localRotation = Quaternion.Euler(0, 0, 0);
                }

                if (ActivePlayerUnitCategory == WorldUnitsManager.UnitCategories.plane && !FreeCamera) {
                    FollowPlaneMovement();                          // If it's a plane. Keep the camera behind the unit.
                } else if (AllowCameraRotation) {
                    HandleRotationMovement();                       // Allow free cam if the camera can turn
                }

                TargetRangeRayCast();

                if (CurrentControlledTurretRole == TurretFireManager.TurretRole.NavalArtillery) {
                    TargetRangeCameraTilt();
                }
                
                // Debug.Log ("RaycastTargetPosition : "+ RaycastTargetPosition);
                

                
                // Debug.Log("targetDistance = "+ targetDistance);
            }
        }
        protected void TargetRangeCameraTilt() {
            CameraTiltPercentage = 100 - (((TiltAngle - m_TiltMin) * 100) / (m_TiltMax - m_TiltMin));
        }
        protected void TargetRangeRayCast() {
            Ray ray = new Ray(m_RaycastProjector.transform.position, m_RaycastProjector.transform.forward);
            Plane hPlane = new Plane(Vector3.up, Vector3.zero);         // This is a plane at 0,0,0 which simulates the sea collision model
            float distance = 0; 

            if (Physics.Raycast(ray, out RaycastHit, Mathf.Infinity, RaycastLayerMask)) {               // If a collision model is hit
                Debug.DrawRay(m_RaycastProjector.transform.position + (m_RaycastProjector.transform.forward * m_CameraDistance), m_RaycastProjector.transform.TransformDirection(Vector3.forward) * RaycastHit.distance, Color.yellow);
                RaycastTargetPosition = RaycastHit.point;
            } else if (hPlane.Raycast(ray, out distance)) {                                             // If the "water" is hit
                Debug.DrawRay(m_RaycastProjector.transform.position + (m_RaycastProjector.transform.forward * m_CameraDistance), m_RaycastProjector.transform.TransformDirection(Vector3.forward) * distance, Color.red);
                RaycastTargetPosition = m_RaycastProjector.transform.position + m_RaycastProjector.transform.TransformDirection(Vector3.forward) * distance;
            } else {                                                                                    // If it is in the sky
                Debug.DrawRay(m_RaycastProjector.transform.position + (m_RaycastProjector.transform.forward * m_CameraDistance), m_RaycastProjector.transform.TransformDirection(Vector3.forward) * 100000, Color.white);
                RaycastTargetPosition = m_RaycastProjector.transform.position + m_RaycastProjector.transform.TransformDirection(Vector3.forward) * 100000;
            }
            RaycastAbstractTargetPosition = m_RaycastProjector.transform.position + m_RaycastProjector.transform.TransformDirection(Vector3.forward) * 100000;

            // void Update(){
            // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // // create a plane at 0,0,0 whose normal points to +Y:
            // Plane hPlane = new Plane(Vector3.up, Vector3.zero);
            // // Plane.Raycast stores the distance from ray.origin to the hit point in this variable:
            // float distance = 0; 
            // // if the ray hits the plane...
            // if (hPlane.Raycast(ray, out distance)){
            //     // get the hit point:
            //     temp.transform.position = ray.GetPoint(distance);
            // }
            // }

            BuildRaycastRange();
        }

        protected void BuildRaycastRange() {
            RaycastRange = (ActivePlayerUnitTransform.position - RaycastTargetPosition).magnitude;
        }

        public virtual void SetTarget(Transform newTransform) {
            ActivePlayerUnitTransform = newTransform;
        }

        protected void FollowTarget(float deltaTime) {
            // Move the rig towards target position.
            transform.position = Vector3.Lerp(transform.position, ActivePlayerUnitTransform.position, deltaTime * m_MoveSpeed);
        }

        private void HandleRotationMovement() {
			if(Time.timeScale < float.Epsilon)
			return;

            // Read the user input
            var x = Input.GetAxis("Mouse X");
            var y = Input.GetAxis("Mouse Y");
            float turnSpeed = m_TurnSpeed;
            

            if (Input.GetButton ("FocusCamera"))
                turnSpeed = m_TurnSpeed * m_TurnSpeedFocus;
            

            // Adjust the look angle by an amount proportional to the turn speed and horizontal input.
            LookAngle += x * turnSpeed * Time.deltaTime;

            // Rotate the rig (the root object) around Y axis only:
            TransformTargetRot = Quaternion.Euler(0f, LookAngle, 0f);

            // on platforms with a mouse, we adjust the current angle based on Y mouse input and turn speed
            TiltAngle -= y * turnSpeed * Time.deltaTime;


            // and make sure the new value is within the tilt range
            TiltAngle = Mathf.Clamp(TiltAngle, m_TiltMin, m_TiltMax);

            // Tilt input around X is applied to the pivot (the child of this object)
			AxisTargetRot = Quaternion.Euler(TiltAngle, AxisEulers.y , AxisEulers.z);

			if (m_TurnSmoothing > 0)
			{
				Axis.localRotation = Quaternion.Slerp(Axis.localRotation, AxisTargetRot, m_TurnSmoothing * Time.deltaTime);
				transform.localRotation = Quaternion.Slerp(transform.localRotation, TransformTargetRot, m_TurnSmoothing * Time.deltaTime);
			} else {
				Axis.localRotation = AxisTargetRot;
				transform.localRotation = TransformTargetRot;
			}
        }

        private void FollowPlaneMovement() {
			if(Time.timeScale < float.Epsilon)
			return;
            // Get target plane rotation
            Vector3 planeeulerAngles = ActivePlayerUnitTransform.rotation.eulerAngles;

            // Give it to the local camera rig transform
            transform.rotation = Quaternion.Euler(ActivePlayerUnitTransform.rotation.eulerAngles.x, ActivePlayerUnitTransform.rotation.eulerAngles.y, ActivePlayerUnitTransform.rotation.eulerAngles.z);
        }

        public void SetFreeCamera(bool freeCamera) { FreeCamera = freeCamera; }
        public void SetRotation(bool set){ AllowCameraRotation = set; }
        public void SetMouse(bool set){
            if (set) {
                Cursor.lockState = CursorLockMode.None;
            } else {
                Cursor.lockState = CursorLockMode.Locked;
            }
            Cursor.visible = set;
        }

        public void SetActiveTarget(GameObject TargetSent) {
            ActivePlayerUnit = TargetSent;
            ActivePlayerUnitTransform = ActivePlayerUnit.transform;

            if (ActivePlayerUnit.GetComponent<UnitMasterController>()) {
                ActivePlayerUnitCategory = ActivePlayerUnit.GetComponent<UnitMasterController>().m_UnitCategory;
            }

            if (ActivePlayerUnit.GetComponent<TargetCameraParameters>()) {                                      // Set camera position relative to the target
                m_CameraDistance = ActivePlayerUnit.GetComponent<TargetCameraParameters>().m_CameraDistance;
                m_CameraHeight = ActivePlayerUnit.GetComponent<TargetCameraParameters>().m_CameraHeight;
                m_CameraLateralOffset = ActivePlayerUnit.GetComponent<TargetCameraParameters>().m_CameraLateralOffset;
            } else {
                m_CameraDistance = CameraDistance;
                m_CameraHeight = CameraHeight;
                m_CameraLateralOffset = CameraLateralOffset;
            }

            Pivot.localPosition = new Vector3(m_CameraLateralOffset, m_CameraHeight, -m_CameraDistance);
        }

        public void SetCurrentTurretRole(TurretFireManager.TurretRole currentControlledTurret) {
            CurrentControlledTurretRole = currentControlledTurret;
        }
        public Vector3 GetRaycastScreenPosition() {
            Vector3 screenPosition;
            if (CurrentControlledTurretRole == TurretFireManager.TurretRole.NavalArtillery) {       // If naval artillery is used, keep the pointer at horizon level far away
                screenPosition = RaycastAbstractTargetPosition;
                screenPosition.y = 0;
                return Cam.WorldToScreenPoint(screenPosition);
            } else{
                return Cam.WorldToScreenPoint(RaycastTargetPosition);                               // Else give the raycast hit point
            }
        }
        public Vector3 GetRaycastTargetPosition() {
            return RaycastTargetPosition;
        }
        public Vector3 GetRaycastAbstractTargetPosition() {
            return RaycastAbstractTargetPosition;
        }
        public float GetRaycastRange() {
            return RaycastRange;
        }

        public float GetTiltPercentage() {
            return CameraTiltPercentage;
        }
        // public float GetTargetPointRange() {
        //     float Range = Vector3.Distance(ActivePlayerUnitTransform.position, TargetPosition);
        //     return Range;
        // }
        /*public void SetHideUI(){
            DisplayUI = !DisplayUI;
            if (DisplayUI) {                                                         // This is supposed to allow only parts of the game to render.
                Cam.cullingMask |= 1 << LayerMask.NameToLayer("UI");                 // Commented to test, nothing changed ? Will maybe produce unexpected errors ?
            } else {
                Cam.cullingMask &=  ~(1 << LayerMask.NameToLayer("UI"));
            }
        }*/
    }
}