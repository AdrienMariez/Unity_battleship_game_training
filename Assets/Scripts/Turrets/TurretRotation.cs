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

    [Tooltip("When true, turret rotates according to left/right traverse limits. When false, turret can rotate freely.")]
    public bool limitTraverse = false;
    [Tooltip("When traverse is limited, how many degrees to the left the turret can turn.")]
    [Range(0.0f, 180.0f)]
    public float leftTraverse = 60.0f;
    [Tooltip("When traverse is limited, how many degrees to the right the turret can turn.")]
    [Range(0.0f, 180.0f)]
    public float rightTraverse = 60.0f;
    private float localLeftTraverse;
    private float localRightTraverse;

    public bool debug = false;

    private float speedRate;
    [Tooltip ("Position/rotation of the direct parent")] private Vector3 parentEulerAngles;
    [Tooltip ("initial rotation of the turret")] private float TurretEulerAngle;
    private float targetAng;
    private float currentAng;
    private GameObject CameraPivot;

    // [HideInInspector] public Vector3 m_TargetPosition;
    private FreeLookCam FreeLookCam;


    private void Awake(){
        FreeLookCam = GameObject.Find("FreeLookCameraRig").GetComponent<FreeLookCam>();
        CameraPivot = GameObject.Find("CameraPivot");
        parentEulerAngles = Parent.transform.rotation.eulerAngles;
        TurretEulerAngle = TurretTurret.transform.localRotation.eulerAngles.y;
        localLeftTraverse = leftTraverse + TurretEulerAngle;
        if (localLeftTraverse>360)
            localLeftTraverse -= 360;
        localRightTraverse = 360 - rightTraverse + TurretEulerAngle;
        if (localRightTraverse>360)
            localRightTraverse -= 360;
        // if (debug) {
        //     Debug.Log("TurretEulerAngle: " + TurretEulerAngle);
        //     Debug.Log("leftTraverse: " + leftTraverse);
        //     Debug.Log("rightTraverse: " + rightTraverse);
        //     Debug.Log("localLeftTraverse: " + localLeftTraverse);
        //     Debug.Log("localRightTraverse: " + localRightTraverse);
        // }
    }

    /* Those methods are not used but could be to allow to disable all turrets of a unit if needed
        private void OnEnable () { 
        }
        private void OnDisable () {
        }
    */


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
        if (parentRotationAng<-360)
            parentRotationAng += 360;
        else if (parentRotationAng>360)
            parentRotationAng -= 360;

        /*
            //This allows to align with camera
            // Get Camera rotation
            Vector3 cameraEulerAngles = CameraPivot.transform.rotation.eulerAngles;

            targetAng = cameraEulerAngles.y-parentEulerAngles.y;
            if (targetAng<0)
                targetAng += 360;
        */


        float TargetAngleWorld = Quaternion.FromToRotation(Vector3.forward, FreeLookCam.m_TargetPosition - TurretTurret.transform.position).eulerAngles.y;

        targetAng = TargetAngleWorld - parentEulerAngles.y;
        if (targetAng<0)
            targetAng += 360;
        else if (targetAng>360)
            targetAng -= 360;

        // Debug.DrawRay(TurretTurret.transform.position, TurretCannon.transform.TransformDirection(new Vector3(0f,targetAng,0f)) * 1000, Color.green);
        // if (debug) {
        //     Debug.Log("targetAng: " + targetAng);
        // }

        currentAng = TurretTurret.transform.localRotation.eulerAngles.y;
        // if (currentAng>360)
        //     currentAng -= 360;

        // if (debug) { Debug.Log("currentAng 1 = "+ currentAng); }
        
        // Calculate Turn Rate.
        float targetSpeedRate = Mathf.Lerp (0.0f, 1.0f, Mathf.Abs (targetAng) / (rotationSpeed * Time.fixedDeltaTime + bufferAngle)) * Mathf.Sign (targetAng);
        
        // Calculate Rate
        speedRate = Mathf.MoveTowardsAngle (speedRate, targetSpeedRate, Time.fixedDeltaTime / acceleration_Time);
        
        // Rotate
        currentAng = BuildRotation(currentAng,targetAng);

        // if (debug) {
        //     Debug.Log("TurretEulerAngles = "+TurretEulerAngle);
        // }

        // Add parent rotation rate to the new current angle
        // Why is it no longer necessary ? No idea ! Caused bugs with reversed turrets
        // currentAng += parentRotationAng;

        // if (debug) {
        //     Debug.Log("currentAng 2 = "+ currentAng);
        //     Debug.Log("parentRotationAng = "+ parentRotationAng);
        // }

        if (limitTraverse) {
            currentAng = CheckLimitTraverse(currentAng);
        }

        // if (debug) {
        //     // Debug.Log("targetAng: " + targetAng);
        //     Debug.Log("currentAng 3 = "+ currentAng);
        // }

        // Update the turret angle
        TurretTurret.transform.localRotation = Quaternion.Euler (new Vector3 (0.0f, currentAng, 0.0f));

        // Debug.Log("currentAng = "+currentAng);
    }

    private float BuildRotation(float CurrentAngle, float TargetAngle){
        if (!limitTraverse) {
            if (CurrentAngle < TargetAngle && CurrentAngle+180 > targetAng || CurrentAngle > TargetAngle && CurrentAngle > TargetAngle+180) {
                CurrentAngle += rotationSpeed * speedRate * Time.fixedDeltaTime;
            } else {
                CurrentAngle -= rotationSpeed * speedRate * Time.fixedDeltaTime;
            }
        } else {
            float Median;
            if (localLeftTraverse > localRightTraverse) {
                Median = ((localRightTraverse + localLeftTraverse)/2) - 180;
                if (CurrentAngle < TargetAngle && TargetAngle > Median || CurrentAngle > TargetAngle && TargetAngle < Median) {
                    CurrentAngle += rotationSpeed * speedRate * Time.fixedDeltaTime;
                } else {
                    CurrentAngle -= rotationSpeed * speedRate * Time.fixedDeltaTime;
                }
            } else {
                Median = (localRightTraverse + localLeftTraverse)/2;
                if (CurrentAngle < TargetAngle && TargetAngle < Median || CurrentAngle < TargetAngle && CurrentAngle > Median || CurrentAngle > TargetAngle && CurrentAngle >= Median && TargetAngle < Median) {
                    CurrentAngle += rotationSpeed * speedRate * Time.fixedDeltaTime;
                } else {
                    CurrentAngle -= rotationSpeed * speedRate * Time.fixedDeltaTime;
                }
            }
        }
        return CurrentAngle;
    }

    private float CheckLimitTraverse(float CurrentAngle){
        if (localLeftTraverse > localRightTraverse) {
            if (CurrentAngle >= localLeftTraverse) {
                CurrentAngle = localLeftTraverse;
            }
            if (CurrentAngle <= localRightTraverse) {
                CurrentAngle = localRightTraverse;
            }
        } else {
            if (CurrentAngle >= localLeftTraverse && CurrentAngle <= localRightTraverse) {
                if (CurrentAngle < (localLeftTraverse + 1)) {
                    CurrentAngle = localLeftTraverse;
                } else if (CurrentAngle > (localRightTraverse - 1)) {
                    CurrentAngle = localRightTraverse;
                }
            }
        }
        return CurrentAngle;
    }

    private void CannonElevation() {
        // Get parent current rotation rate
        float parentRotationAng = Parent.transform.rotation.eulerAngles.x-parentEulerAngles.x;



        // Get Camera rotation
        // Vector3 cameraEulerAngles = CameraPivot.transform.rotation.eulerAngles;
        // targetAng = cameraEulerAngles.x-parentEulerAngles.x;
        // if (targetAng<0)
        //     targetAng += 360;

        float TargetAngleWorld = Quaternion.FromToRotation(Vector3.forward, FreeLookCam.m_TargetPosition - TurretTurret.transform.position).eulerAngles.x;

        targetAng = TargetAngleWorld - parentEulerAngles.x;
        if (targetAng<0)
            targetAng += 360;

        // targetAng += parentRotationAng;

        // Turn turret towards camera facing
        TurretCannon.transform.localRotation = Quaternion.Euler(targetAng, 0.0f, 0.0f);

        Debug.DrawRay(TurretCannon.transform.position, TurretCannon.transform.TransformDirection(Vector3.forward) * 1000, Color.red);
    }
}