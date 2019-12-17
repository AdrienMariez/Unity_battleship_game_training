using System;
using UnityEngine;

namespace FreeLookCamera {
    public class FreeLookCam : MonoBehaviour {
        // This script is designed to be placed on the root object of a camera rig,
        // comprising 3 gameobjects, each parented to the next:

        // 	Camera Rig
        // 		Pivot
        // 			Camera

        private Transform Target;            // The target object to follow
        private Rigidbody targetRigidbody;
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
            [Tooltip("Those variables should be overrriden for each unit, but basic static values are stored here.")] public float m_CameraDistance = 12;  // x
            public float m_CameraHeight = 2;    // y
            public float m_CameraLateralOffset = 0;   // z

        [Header("Raycast")]
            [Tooltip("Layer to filter what the raycast will hit.")] public LayerMask RaycastLayerMask;
            private GameObject m_RaycastProjector;
        public RaycastHit RaycastHit;
        private GameObject TargetCircle;
        private Vector3 TargetPosition;

        private float LookAngle;                    // The rig's y axis rotation.
        private float TiltAngle;                    // The pivot's x axis rotation.
		private Vector3 AxisEulers;
		private Quaternion AxisTargetRot;
		private Quaternion TransformTargetRot;
        private GameObject ActiveTarget;
        private PlayerManager PlayerManager;

        protected virtual void Start() {
            if (Target == null) return;
            targetRigidbody = Target.GetComponent<Rigidbody>();

			AxisEulers = Axis.rotation.eulerAngles;

	        AxisTargetRot = Axis.transform.localRotation;
			TransformTargetRot = transform.localRotation;
        }

        private void Awake(){
            Cam = GetComponentInChildren<Camera>();
            Pivot = GetComponentInChildren<Camera>().transform.parent;
            Axis = Pivot.parent;

            m_RaycastProjector = GameObject.Find("RaycastProjector");
            TargetCircle = GameObject.Find("TargetCircle");
        }

        private void FixedUpdate() { 
                FollowTarget(Time.deltaTime);
        }

        protected void Update() {
            // Debug.Log ("m_Axis   : "+ m_Axis);

            // Set camera position relative to the target
            if (ActiveTarget.GetComponent<TargetCameraParameters>()) {
                m_CameraDistance = ActiveTarget.GetComponent<TargetCameraParameters>().m_CameraDistance;
                m_CameraHeight = ActiveTarget.GetComponent<TargetCameraParameters>().m_CameraHeight;
                m_CameraLateralOffset = ActiveTarget.GetComponent<TargetCameraParameters>().m_CameraLateralOffset;
            } else {
                m_CameraDistance = 12;
                m_CameraHeight = 2;
                m_CameraLateralOffset = 0;
            }

            Pivot.localPosition = new Vector3(m_CameraLateralOffset, m_CameraHeight, -m_CameraDistance);

            if (Input.GetButton ("FocusCamera")){
                Cam.fieldOfView = m_FieldOfViewFocus;
                Cam.transform.localRotation = Quaternion.Euler(-20, 0, 0);
            } else {
                Cam.fieldOfView = m_FieldOfView;
                Cam.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }

            if (ActiveTarget.GetComponent<AircraftController>() && Input.GetButton ("FreeCamera") || !ActiveTarget.GetComponent<AircraftController>()) {
                // If it's a plane and free look is activated OR if it's anything but a plane, allow free cam
                HandleRotationMovement();
            } else {
                // Otherwise, it's a plane. Keep the camera behind the unit.
                FollowPlaneMovement();
            }
            
            // Debug.Log ("m_RaycastPoint : "+ m_RaycastProjector);
            
            if (Physics.Raycast(m_RaycastProjector.transform.position + (m_RaycastProjector.transform.forward * m_CameraDistance), m_RaycastProjector.transform.TransformDirection(Vector3.forward), out RaycastHit, Mathf.Infinity, RaycastLayerMask)) {
                Debug.DrawRay(m_RaycastProjector.transform.position + (m_RaycastProjector.transform.forward * m_CameraDistance), m_RaycastProjector.transform.TransformDirection(Vector3.forward) * RaycastHit.distance, Color.yellow);
                TargetCircle.transform.position = RaycastHit.point;
                TargetPosition = RaycastHit.point;
            } else {
                Debug.DrawRay(m_RaycastProjector.transform.position + (m_RaycastProjector.transform.forward * m_CameraDistance), m_RaycastProjector.transform.TransformDirection(Vector3.forward) * 100000, Color.white);
                TargetCircle.transform.position = m_RaycastProjector.transform.position + m_RaycastProjector.transform.TransformDirection(Vector3.forward) * 100000;
                TargetPosition = m_RaycastProjector.transform.position + m_RaycastProjector.transform.TransformDirection(Vector3.forward) * 100000;
            }
        }

        public virtual void SetTarget(Transform newTransform) {
            Target = newTransform;
        }

        // public Transform GetTarget {
        //     get { return Target; }
        // }

        protected virtual void FollowTarget(float deltaTime) {
            // if (Target == null) 
                // ActiveTarget = GameObject.Find("GameManager").GetComponent<PlayerManager>().ActiveTarget;

            // Move the rig towards target position.
            Target = ActiveTarget.transform;

            transform.position = Vector3.Lerp(transform.position, Target.position, deltaTime * m_MoveSpeed);
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
            TiltAngle = Mathf.Clamp(TiltAngle, -m_TiltMin, m_TiltMax);

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
            Vector3 planeeulerAngles = Target.rotation.eulerAngles;

            // Give it to the local camera rig transform
            transform.rotation = Quaternion.Euler(Target.rotation.eulerAngles.x, Target.rotation.eulerAngles.y, Target.rotation.eulerAngles.z);
        }

        public void SetActiveTarget(GameObject TargetSent) {
            ActiveTarget = TargetSent;
            Target = ActiveTarget.transform;
        }
        public Vector3 GetTargetPosition() {
            return TargetPosition;
        }
    }
}