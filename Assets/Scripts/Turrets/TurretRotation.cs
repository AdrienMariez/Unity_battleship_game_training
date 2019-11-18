using UnityEngine;
using FreeLookCamera;

public class TurretRotation : MonoBehaviour
{    
    [HideInInspector] public bool m_Active;
    [Tooltip ("Audio played when the turret is rotating.")]public AudioClip m_TurretRotationAudio;
    [Tooltip ("PAxis of rotation for horizontal rotation of the turret.")]public Rigidbody TurretTurret;
    [Tooltip ("Axis of rotation for the elevation of the cannon.")]public Rigidbody TurretCannon;
    [Tooltip ("Direct parent of this turret, place the unit rigidbody here by default, but you can put a turret on top of another by placing the parent turret here. ")]public Rigidbody Parent;

    [Tooltip ("Maximum Rotation Speed. (Degree per Second)")] public float rotationSpeed = 15.0f;
	[Tooltip ("Time to reach the maximum speed. (Sec)")] public float acceleration_Time = 0.2f;
	[Tooltip ("Angle range for slowing down. (Degree)")] public float bufferAngle = 5.0f;
    private float speedRate;
    // [HideInInspector] public Vector3 unitEulerAngles;
    [Tooltip ("Position/rotation of the direct parent")] private Vector3 parentEulerAngles;
    private float targetAng;
    private float currentAng;
    private GameObject CameraPivot;

    // [HideInInspector] public Vector3 m_TargetPosition;
    private FreeLookCam FreeLookCam;


    private void Awake(){
        FreeLookCam = GameObject.Find("FreeLookCameraRig").GetComponent<FreeLookCam>();
        CameraPivot = GameObject.Find("CameraPivot");
        parentEulerAngles = Parent.transform.rotation.eulerAngles;
    }

    /*Those method are not used but could be to allow to disable all turrets of a unit if needed
    private void OnEnable () { 
    }
    private void OnDisable () {
    }*/


    private void TurretAudio()
    {
        // TODO play turret rotation audio if the turret axis is moving
        /*

        //Abs (x) means that if x=-3, Abs(x)=3
        //if not moving or rotating
        if (Mathf.Abs (m_MovementInputValue) < 0.1f && Mathf.Abs (m_TurnInputValue) < 0.1f)
        {
            //if moving audio currently playing
             if (m_MovementAudio.clip == m_EngineDriving)
             {
                //switch playing clip
                m_MovementAudio.clip = m_EngineIdling;
                //randomize pitch
                m_MovementAudio.pitch = Random.Range (m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                //play the new audio
                m_MovementAudio.Play ();
             }
        }
        else
        {
            //if idling audio currently playing
            if (m_MovementAudio.clip == m_EngineIdling){
                //switch playing clip
                m_MovementAudio.clip = m_EngineDriving;
                //randomize pitch
                m_MovementAudio.pitch = Random.Range (m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                //play the new audio
                m_MovementAudio.Play ();
            }
        }
        */
    }


    private void FixedUpdate(){
        if (m_Active){
            if (!Input.GetButton ("FreeCamera"))
            {
                TurretRotate();
                CannonElevation();
            }
        // Reassign the new parent angle for future TurretRotate()
        parentEulerAngles = Parent.transform.rotation.eulerAngles;
        }
        
    }

    private void TurretRotate() {
        // Get parent current rotation rate
        float parentRotationAng = Parent.transform.rotation.eulerAngles.y-parentEulerAngles.y;

        /*
            //This allows to align with camera
            // Get Camera rotation
            Vector3 cameraEulerAngles = CameraPivot.transform.rotation.eulerAngles;

            targetAng = cameraEulerAngles.y-parentEulerAngles.y;
            if (targetAng<0)
                targetAng += 360;
        */


        float TargetAngleWold = Quaternion.FromToRotation(Vector3.forward, FreeLookCam.m_TargetPosition - TurretTurret.transform.position).eulerAngles.y;

        targetAng = TargetAngleWold - parentEulerAngles.y;
        if (targetAng<0)
            targetAng += 360;
        // Debug.Log("targetAng: " + targetAng);

        currentAng = TurretTurret.transform.localRotation.eulerAngles.y;
        // Debug.Log("currentAng = "+currentAng);
        
        // Calculate Turn Rate.
        float targetSpeedRate = Mathf.Lerp (0.0f, 1.0f, Mathf.Abs (targetAng) / (rotationSpeed * Time.fixedDeltaTime + bufferAngle)) * Mathf.Sign (targetAng);
        
        // Calculate Rate
        speedRate = Mathf.MoveTowardsAngle (speedRate, targetSpeedRate, Time.fixedDeltaTime / acceleration_Time);
        
        // Rotate
        if (Mathf.Abs (currentAng) < Mathf.Abs (targetAng) && Mathf.Abs (currentAng)+180 > Mathf.Abs (targetAng) || Mathf.Abs (currentAng) > Mathf.Abs (targetAng) && Mathf.Abs (currentAng) > Mathf.Abs (targetAng)+180) {
            currentAng += rotationSpeed * speedRate * Time.fixedDeltaTime;
        }
        else {
            currentAng -= rotationSpeed * speedRate * Time.fixedDeltaTime;
        }

        // Add parent rotation rate to the new current angle
        currentAng += parentRotationAng;

        // Update the turret angle
        TurretTurret.transform.localRotation = Quaternion.Euler (new Vector3 (0.0f, currentAng, 0.0f));
    }

    private void CannonElevation() {
        // Get parent current rotation rate
        float parentRotationAng = Parent.transform.rotation.eulerAngles.x-parentEulerAngles.x;



        // Get Camera rotation
        // Vector3 cameraEulerAngles = CameraPivot.transform.rotation.eulerAngles;
        // targetAng = cameraEulerAngles.x-parentEulerAngles.x;
        // if (targetAng<0)
        //     targetAng += 360;

        float TargetAngleWold = Quaternion.FromToRotation(Vector3.forward, FreeLookCam.m_TargetPosition - TurretTurret.transform.position).eulerAngles.x;

        targetAng = TargetAngleWold - parentEulerAngles.x;
        if (targetAng<0)
            targetAng += 360;

        // targetAng += parentRotationAng;

        // Turn turret towards camera facing
        TurretCannon.transform.localRotation = Quaternion.Euler(targetAng, 0.0f, 0.0f);

        Debug.DrawRay(TurretCannon.transform.position, TurretCannon.transform.TransformDirection(Vector3.forward) * 1000, Color.red);
    }
}