using System;
using UnityEngine;


    public class FreeLookCam : MonoBehaviour
    {
        // This script is designed to be placed on the root object of a camera rig,
        // comprising 3 gameobjects, each parented to the next:

        // 	Camera Rig
        // 		Pivot
        // 			Camera

        // From AbstractTargetFollower
        public enum UpdateType // The available methods of updating are:
        {
            FixedUpdate, // Update in FixedUpdate (for tracking rigidbodies).
            LateUpdate, // Update in LateUpdate. (for tracking objects that are moved in Update)
            ManualUpdate, // user must call to update camera
        }

        [HideInInspector] protected Transform m_Target;            // The target object to follow
        [SerializeField] private UpdateType m_UpdateType;         // stores the selected update type
        protected Rigidbody targetRigidbody;
        // END From AbstractTargetFollower

        // From PivotBasedCameraRig
        protected Transform m_Cam; // the transform of the camera
        protected Transform m_Pivot; // the point at which the camera points to
        protected Transform m_Axis; // the point at which the camera pivots around
        protected Vector3 m_LastTargetPosition;
        // END From PivotBasedCameraRig


        [SerializeField] private float m_MoveSpeed = 20f;                      // How fast the rig will move to keep up with the target's position.
        [Range(0f, 10f)] [SerializeField] private float m_TurnSpeed = 0.02f;   // How fast the rig will rotate from user input.
        [SerializeField] private float m_TurnSmoothing = 0f;                // How much smoothing to apply to the turn input, to reduce mouse-turn jerkiness
        [SerializeField] private float m_TiltMax = 75f;                       // The maximum value of the x axis rotation of the pivot.
        [SerializeField] private float m_TiltMin = 45f;                       // The minimum value of the x axis rotation of the pivot.
        [SerializeField] private bool m_LockCursor = true;                   // Whether the cursor should be hidden and locked.

        private float m_LookAngle;                    // The rig's y axis rotation.
        private float m_TiltAngle;                    // The pivot's x axis rotation.
        // private const float k_LookDistance = 100f;    // How far in front of the pivot the character's look target is.
		private Vector3 m_AxisEulers;
		private Quaternion m_AxisTargetRot;
		private Quaternion m_TransformTargetRot;

        [HideInInspector] public GameObject ActiveTarget;
        public float m_CameraDistance = 12;  // x
        public float m_CameraHeight = 2;    // y
        public float m_CameraLateralOffset = 0;   // z

        private PlayerManager PlayerManager;

        protected virtual void Start() {
            // From AbstractTargetFollower
            if (m_Target == null) return;
            targetRigidbody = m_Target.GetComponent<Rigidbody>();
            // END From AbstractTargetFollower
            // From PivotBasedCameraRig
            m_Cam = GetComponentInChildren<Camera>().transform;
            m_Pivot = m_Cam.parent;
            m_Axis = m_Pivot.parent;

            // END From PivotBasedCameraRig

            // Lock or unlock the cursor.
            
            Cursor.lockState = m_LockCursor ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !m_LockCursor;
			m_AxisEulers = m_Axis.rotation.eulerAngles;

	        m_AxisTargetRot = m_Axis.transform.localRotation;
			m_TransformTargetRot = transform.localRotation;

            ActiveTarget = GameObject.Find("GameManager").GetComponent<PlayerManager>().ActiveTarget;
        }

        private void FixedUpdate() {   
            // From AbstractTargetFollower
            // we update from here if updatetype is set to Fixed, or in auto mode,
            // if the target has a rigidbody, and isn't kinematic.
            if (m_UpdateType == UpdateType.FixedUpdate)
            {
                FollowTarget(Time.deltaTime);
            }
        }

        private void LateUpdate() {
            // From AbstractTargetFollower
            if (m_UpdateType == UpdateType.LateUpdate)
            {
                FollowTarget(Time.deltaTime);
            }
        }

        public void ManualUpdate() {
            // we update from here if updatetype is set to Late, or in auto mode,
            // if the target does not have a rigidbody, or - does have a rigidbody but is set to kinematic.
            if (m_UpdateType == UpdateType.ManualUpdate)
            {
                FollowTarget(Time.deltaTime);
            }
        }

        protected void Update() {
            // Debug.Log ("m_Axis   : "+ m_Axis);


            Color color = new Color(1.0f, 1.0f, 1.0f);
            Debug.DrawLine(Vector3.zero, new Vector3(0, 5, 0), color);

            ActiveTarget = GameObject.Find("GameManager").GetComponent<PlayerManager>().ActiveTarget;

            // Set camera position relative to the target
            if (ActiveTarget.GetComponent<TargetCameraParameters>())
            {
                m_CameraDistance = ActiveTarget.GetComponent<TargetCameraParameters>().m_CameraDistance;
                m_CameraHeight = ActiveTarget.GetComponent<TargetCameraParameters>().m_CameraHeight;
                m_CameraLateralOffset = ActiveTarget.GetComponent<TargetCameraParameters>().m_CameraLateralOffset;
            }
            else
            {
                m_CameraDistance = 12;
                m_CameraHeight = 2;
                m_CameraLateralOffset = 0;
            }

            m_Pivot.localPosition = new Vector3(m_CameraLateralOffset, m_CameraHeight, -m_CameraDistance);


            if (ActiveTarget.GetComponent<AircraftController>() && Input.GetButton ("FreeCamera") || !ActiveTarget.GetComponent<AircraftController>())
            {
                // If it's a plane and free look is activated OR if it's anything but a plane, allow free cam
                HandleRotationMovement();
            }
            else
            {
                // Otherwise, it's a plane. Keep the camera behind the unit.
                FollowPlaneMovement();
            }

            if (m_LockCursor && Input.GetMouseButtonUp(0))
            {
                Cursor.lockState = m_LockCursor ? CursorLockMode.Locked : CursorLockMode.None;
                Cursor.visible = !m_LockCursor;
            }


            if (Input.GetButtonDown ("SetNextUnit") || Input.GetButtonDown ("SetPreviousUnit")){
                ActiveTarget = GameObject.Find("GameManager").GetComponent<PlayerManager>().ActiveTarget;
                //Debug.Log ("Current Target : "+ CurrentTarget);
            }
        }


        private void OnDisable() {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public virtual void SetTarget(Transform newTransform) {
            // From AbstractTargetFollower
            m_Target = newTransform;
        }


        public Transform Target {
            // From AbstractTargetFollower
            get { return m_Target; }
        }

        protected virtual void FollowTarget(float deltaTime) {
            if (m_Target == null)
            {
                ActiveTarget = GameObject.Find("GameManager").GetComponent<PlayerManager>().ActiveTarget;
                // CurrentTarget = GameObject.Find("GameManager").GetComponent<PlayerManager>().CurrentTarget;
            }

            //Those lines DO NOT EXECUTE in Start()
            //I think a lot of lines in this file could be ditched entirely, cleanup todo later.
            m_Cam = GetComponentInChildren<Camera>().transform;
            m_Pivot = m_Cam.parent;
            m_Axis = m_Pivot.parent;

            // Move the rig towards target position.

            //PlayerUnits[CurrentTarget].EnableControl();
            m_Target = ActiveTarget.transform;
            //Debug.Log ("Current target for camera   : "+ PlayerUnits[CurrentTarget]);
            

            transform.position = Vector3.Lerp(transform.position, m_Target.position, deltaTime*m_MoveSpeed);
        }


        private void HandleRotationMovement() {
			if(Time.timeScale < float.Epsilon)
			return;

            // Read the user input
            var x = Input.GetAxis("Mouse X");
            var y = Input.GetAxis("Mouse Y");

            // Adjust the look angle by an amount proportional to the turn speed and horizontal input.
            m_LookAngle += x*m_TurnSpeed * Time.deltaTime;

            // Rotate the rig (the root object) around Y axis only:
            m_TransformTargetRot = Quaternion.Euler(0f, m_LookAngle, 0f);
            // on platforms with a mouse, we adjust the current angle based on Y mouse input and turn speed
            m_TiltAngle -= y*m_TurnSpeed * Time.deltaTime;
            // and make sure the new value is within the tilt range
            m_TiltAngle = Mathf.Clamp(m_TiltAngle, -m_TiltMin, m_TiltMax);

            // Tilt input around X is applied to the pivot (the child of this object)
			m_AxisTargetRot = Quaternion.Euler(m_TiltAngle, m_AxisEulers.y , m_AxisEulers.z);

			if (m_TurnSmoothing > 0)
			{
				m_Axis.localRotation = Quaternion.Slerp(m_Axis.localRotation, m_AxisTargetRot, m_TurnSmoothing * Time.deltaTime);
				transform.localRotation = Quaternion.Slerp(transform.localRotation, m_TransformTargetRot, m_TurnSmoothing * Time.deltaTime);
			}
			else
			{
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
