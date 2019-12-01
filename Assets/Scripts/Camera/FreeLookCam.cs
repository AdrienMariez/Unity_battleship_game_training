using System;
using UnityEngine;

namespace FreeLookCamera
{
    public class FreeLookCam : MonoBehaviour
    {
        // This script is designed to be placed on the root object of a camera rig,
        // comprising 3 gameobjects, each parented to the next:

        // 	Camera Rig
        // 		Pivot
        // 			Camera

        public enum UpdateType { // The available methods of updating are:
            FixedUpdate, // Update in FixedUpdate (for tracking rigidbodies).
            LateUpdate, // Update in LateUpdate. (for tracking objects that are moved in Update)
            ManualUpdate, // user must call to update camera
        }

        [HideInInspector] protected Transform m_Target;            // The target object to follow
        protected Rigidbody targetRigidbody;
        protected Camera m_Cam; // Main camera
        protected Transform m_Pivot; // the point at which the camera points to
        protected Transform m_Axis; // the point at which the camera pivots around
        protected Vector3 m_LastTargetPosition;

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
            protected GameObject m_RaycastProjector;
        public float RaycastRange = 1000;
        public RaycastHit RaycastHit;
        protected GameObject m_TargetCircle;
        [HideInInspector] public Vector3 m_TargetPosition;

        private float m_LookAngle;                    // The rig's y axis rotation.
        private float m_TiltAngle;                    // The pivot's x axis rotation.
		private Vector3 m_AxisEulers;
		private Quaternion m_AxisTargetRot;
		private Quaternion m_TransformTargetRot;
        [HideInInspector] public GameObject ActiveTarget;
        private PlayerManager PlayerManager;

        protected virtual void Start() {
            if (m_Target == null) return;
            targetRigidbody = m_Target.GetComponent<Rigidbody>();

			m_AxisEulers = m_Axis.rotation.eulerAngles;

	        m_AxisTargetRot = m_Axis.transform.localRotation;
			m_TransformTargetRot = transform.localRotation;

            ActiveTarget = GameObject.Find("GameManager").GetComponent<PlayerManager>().ActiveTarget;
        }

        private void Awake(){
            m_Cam = GetComponentInChildren<Camera>();
            m_Pivot = GetComponentInChildren<Camera>().transform.parent;
            m_Axis = m_Pivot.parent;

            m_RaycastProjector = GameObject.Find("RaycastProjector");
            m_TargetCircle = GameObject.Find("TargetCircle");
        }

        private void FixedUpdate() { 
                FollowTarget(Time.deltaTime);
        }

        protected void Update() {
            // Debug.Log ("m_Axis   : "+ m_Axis);
            ActiveTarget = GameObject.Find("GameManager").GetComponent<PlayerManager>().ActiveTarget;

            // Set camera position relative to the target
            if (ActiveTarget.GetComponent<TargetCameraParameters>())
            {
                m_CameraDistance = ActiveTarget.GetComponent<TargetCameraParameters>().m_CameraDistance;
                m_CameraHeight = ActiveTarget.GetComponent<TargetCameraParameters>().m_CameraHeight;
                m_CameraLateralOffset = ActiveTarget.GetComponent<TargetCameraParameters>().m_CameraLateralOffset;
            } else {
                m_CameraDistance = 12;
                m_CameraHeight = 2;
                m_CameraLateralOffset = 0;
            }

            m_Pivot.localPosition = new Vector3(m_CameraLateralOffset, m_CameraHeight, -m_CameraDistance);

            if (Input.GetButton ("FocusCamera")){
                m_Cam.fieldOfView = m_FieldOfViewFocus;
            } else {
                m_Cam.fieldOfView = m_FieldOfView;
            }

            if (ActiveTarget.GetComponent<AircraftController>() && Input.GetButton ("FreeCamera") || !ActiveTarget.GetComponent<AircraftController>()) {
                // If it's a plane and free look is activated OR if it's anything but a plane, allow free cam
                HandleRotationMovement();
            } else {
                // Otherwise, it's a plane. Keep the camera behind the unit.
                FollowPlaneMovement();
            }


            if (Input.GetButtonDown ("SetNextUnit") || Input.GetButtonDown ("SetPreviousUnit")){
                ActiveTarget = GameObject.Find("GameManager").GetComponent<PlayerManager>().ActiveTarget;
                //Debug.Log ("Current Target : "+ CurrentTarget);
            }
            
            // Debug.Log ("m_RaycastPoint : "+ m_RaycastProjector);
            
            if (Physics.Raycast(m_RaycastProjector.transform.position + (m_RaycastProjector.transform.forward * m_CameraDistance), m_RaycastProjector.transform.TransformDirection(Vector3.forward), out RaycastHit, Mathf.Infinity, RaycastLayerMask)) {
                Debug.DrawRay(m_RaycastProjector.transform.position + (m_RaycastProjector.transform.forward * m_CameraDistance), m_RaycastProjector.transform.TransformDirection(Vector3.forward) * RaycastHit.distance, Color.yellow);
                m_TargetCircle.transform.position = RaycastHit.point;
                m_TargetPosition = RaycastHit.point;
            } else {
                Debug.DrawRay(m_RaycastProjector.transform.position + (m_RaycastProjector.transform.forward * m_CameraDistance), m_RaycastProjector.transform.TransformDirection(Vector3.forward) * 1000, Color.white);
                m_TargetCircle.transform.position = m_RaycastProjector.transform.position + m_RaycastProjector.transform.TransformDirection(Vector3.forward) * 1000;
                m_TargetPosition = m_RaycastProjector.transform.position + m_RaycastProjector.transform.TransformDirection(Vector3.forward) * 1000;
            }
        }

        public virtual void SetTarget(Transform newTransform) {
            m_Target = newTransform;
        }

        public Transform Target {
            get { return m_Target; }
        }

        protected virtual void FollowTarget(float deltaTime) {
            if (m_Target == null) 
                ActiveTarget = GameObject.Find("GameManager").GetComponent<PlayerManager>().ActiveTarget;

            // Move the rig towards target position.
            m_Target = ActiveTarget.transform;

            transform.position = Vector3.Lerp(transform.position, m_Target.position, deltaTime*m_MoveSpeed);
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
            m_LookAngle += x * turnSpeed * Time.deltaTime;

            // Rotate the rig (the root object) around Y axis only:
            m_TransformTargetRot = Quaternion.Euler(0f, m_LookAngle, 0f);

            // on platforms with a mouse, we adjust the current angle based on Y mouse input and turn speed
            m_TiltAngle -= y * turnSpeed * Time.deltaTime;


            // and make sure the new value is within the tilt range
            m_TiltAngle = Mathf.Clamp(m_TiltAngle, -m_TiltMin, m_TiltMax);

            // Tilt input around X is applied to the pivot (the child of this object)
			m_AxisTargetRot = Quaternion.Euler(m_TiltAngle, m_AxisEulers.y , m_AxisEulers.z);

			if (m_TurnSmoothing > 0)
			{
				m_Axis.localRotation = Quaternion.Slerp(m_Axis.localRotation, m_AxisTargetRot, m_TurnSmoothing * Time.deltaTime);
				transform.localRotation = Quaternion.Slerp(transform.localRotation, m_TransformTargetRot, m_TurnSmoothing * Time.deltaTime);
			} else {
				m_Axis.localRotation = m_AxisTargetRot;
				transform.localRotation = m_TransformTargetRot;
			}
        }

        private void FollowPlaneMovement() {
			if(Time.timeScale < float.Epsilon)
			return;
            // Get target plane rotation
            Vector3 planeeulerAngles = m_Target.rotation.eulerAngles;

            // Give it to the local camera rig transform
            transform.rotation = Quaternion.Euler(m_Target.rotation.eulerAngles.x, m_Target.rotation.eulerAngles.y, m_Target.rotation.eulerAngles.z);
        }
    }
}