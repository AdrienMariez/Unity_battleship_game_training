using UnityEngine;
using System.Collections;
using FreeLookCamera;

public class TurretRotation : MonoBehaviour
{    
    private bool Active;
    private bool Dead;

    [Header("Elements")]
        [Tooltip ("Point to which the cannon will point when in idle position")]public Transform m_IdlePointer;
        [Tooltip ("Audio played when the turret is rotating.")]public AudioClip m_TurretRotationAudio;
        [Tooltip ("PAxis of rotation for horizontal rotation of the turret.")]public Rigidbody TurretTurret;
        [Tooltip ("Axis of rotation for the elevation of the cannon.")]public Rigidbody TurretCannon;
        [Tooltip ("Direct parent of this turret, place the unit rigidbody here by default, but you can put a turret on top of another by placing the parent turret here. ")]public Rigidbody Parent;

        [Header("Horizontal rotation")]
        [Tooltip ("Rotation Speed. (°/s)")] public float rotationSpeed = 15.0f;

        [Tooltip("When true, turret rotates according to left/right traverse limits. When false, turret can rotate freely.")]
        public bool limitTraverse = false;
        [Tooltip("When traverse is limited, how many degrees to the left the turret can turn.")]
        [Range(0.0f, 180.0f)]
        public float leftTraverse = 60.0f;
        private float localLeftTraverse;
        [Tooltip("When traverse is limited, how many degrees to the right the turret can turn.")]
        [Range(0.0f, 180.0f)]
        public float rightTraverse = 60.0f; 
        private float localRightTraverse;
        public FireZonesManager[] NoFireZones;
        private bool MapActive;

    [Header("Vertical elevation")]
        [Tooltip ("Maximum elevation Speed. (Degree per Second)")] public float elevationSpeed = 15.0f;

        [Tooltip("Maximum elevation (degrees) of the turret. 90° is horizontal.")]
        [Range(0.0f, 180.0f)]
        public float upTraverse = 200.0f;
        [Tooltip("Depression (degrees) of the turret.  90° is horizontal.")]
        [Range(0.0f, 180.0f)]
        public float downTraverse = 80.0f;
        public ElevationZonesManager[] ElevationZones;

    [Header("Debug")]
        public bool debug = false;

    [Tooltip ("Position/rotation of the direct parent")] private Vector3 parentEulerAngles;
    [Tooltip ("initial rotation of the turret")] private float TurretEulerAngle;
    private float targetAng;
    private float currentAng;
    private float targetAngElev;
    private float currentAngElev;
    private GameObject CameraPivot;
    private FreeLookCam FreeLookCam;
    private Vector3 TargetPosition;
    private float CameraPercentage;
    private TurretFireManager TurretFireManager;
    private bool PreventFireHoriz = false;
    private bool PreventFireVert = false;
    private float TotalAngleElevRatio;
    private float CurrentAngleElevRatio;
    [HideInInspector] public float CurrentAnglePercentage;



    private void Awake(){
        FreeLookCam = GameObject.Find("FreeLookCameraRig").GetComponent<FreeLookCam>();
        CameraPivot = GameObject.Find("CameraPivot");
        TurretFireManager = GetComponent<TurretFireManager>();
        parentEulerAngles = Parent.transform.rotation.eulerAngles;
        TurretEulerAngle = TurretTurret.transform.localRotation.eulerAngles.y;
        localLeftTraverse = leftTraverse + TurretEulerAngle;
        if (localLeftTraverse > 360)
            localLeftTraverse -= 360;
        localRightTraverse = 360 - rightTraverse + TurretEulerAngle;
        if (localRightTraverse > 360)
            localRightTraverse -= 360;

        TotalAngleElevRatio = upTraverse-downTraverse;

        currentAng = TurretTurret.transform.localRotation.eulerAngles.y;
        
        // if (debug) {
        //     Debug.Log("TurretEulerAngle: " + TurretEulerAngle);
        //     Debug.Log("leftTraverse: " + leftTraverse);
        //     Debug.Log("rightTraverse: " + rightTraverse);
        //     Debug.Log("localLeftTraverse: " + localLeftTraverse);
        //     Debug.Log("localRightTraverse: " + localRightTraverse);
            // Debug.Log("m_IdlePosition: " + m_IdlePosition);
        // }
    }

    /* Those methods are not used but could be to allow to disable all turrets of a unit if needed
        private void OnEnable () { 
        }
        private void OnDisable () {
        }
    */

    private void TurretAudio() {
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
        if (!Active) {
            TargetPosition = m_IdlePointer.transform.position;
        } else {
            if (!Input.GetButton ("FreeCamera") && !MapActive) {
                // Manual control activated

                // Get the angle of the camera here
                CameraPercentage = FreeLookCam.GetTiltPercentage();
                // Get target point position here
                TargetPosition = FreeLookCam.GetTargetPosition();
            } 
        }

        if (!Dead) {
            TurretRotate();
            CannonElevation();
        } else {
            TurretStatic();
        }

        // Check if anything can prevent the turret from firing
        if (PreventFireHoriz || PreventFireVert || MapActive){
            TurretFireManager.SetPreventFire(true);
        } else{
            TurretFireManager.SetPreventFire(false);

            // if (debug) {
                // Debug.Log("current: " + currentAngElev);
                // Debug.Log("total: " + TotalAngleElevRatio);
                // Debug.Log("percent sent: " + CurrentAngleElevRatio);
            // }
            CurrentAnglePercentage = CurrentAngleElevRatio * 100 / (TotalAngleElevRatio);
        }
        
        // Reassign the new parent angle for future TurretRotate()
        parentEulerAngles = Parent.transform.rotation.eulerAngles;
    }

    private void TurretRotate() {
        // Get parent current rotation rate
        float parentRotationAng = Parent.transform.rotation.eulerAngles.y-parentEulerAngles.y;
        if (parentRotationAng<-360)
            parentRotationAng += 360;
        else if (parentRotationAng>360)
            parentRotationAng -= 360;


        float TargetAngleWorld = Quaternion.FromToRotation(Vector3.forward, TargetPosition - TurretTurret.transform.position).eulerAngles.y;

        targetAng = TargetAngleWorld - parentEulerAngles.y;
        
        if (targetAng<0)
            targetAng += 360;
        else if (targetAng>360)
            targetAng -= 360;


        // currentAng = TurretTurret.transform.localRotation.eulerAngles.y;

        // if (debug) { Debug.Log("parentRotationAng = "+ parentRotationAng); }
        
        // Rotate
        currentAng = BuildRotation(currentAng,targetAng);

        // Check if the turret is hitting a limitation
        if (limitTraverse) {
            currentAng = CheckLimitTraverse(currentAng);
        }

        // Add parent rotation rate to the new current angle so that a rotating tank can turn its turret while rotating himself
        currentAng += parentRotationAng;


        if (currentAng<0)
            currentAng += 360;
        if (currentAng>360)
            currentAng -= 360;

        // Check if the turret is in a no-fire zone
        PreventFireHoriz = CheckNoFireZones(currentAng,targetAng);

        // if (debug) { Debug.Log("currentAng = "+ currentAng); Debug.Log("targetAng = "+ targetAng); }

        // Update the turret angle
        TurretTurret.transform.localRotation = Quaternion.Euler (new Vector3 (0.0f, currentAng, 0.0f));

        Debug.DrawLine(TurretTurret.transform.position, TargetPosition, Color.green);
    }

    private float BuildRotation(float CurrentAngle, float TargetAngle){
        if (!limitTraverse) {
            if (CurrentAngle < TargetAngle && CurrentAngle+180 > targetAng || CurrentAngle > TargetAngle && CurrentAngle > TargetAngle+180) {
                CurrentAngle += rotationSpeed * Time.fixedDeltaTime;
            } else {
                CurrentAngle -= rotationSpeed * Time.fixedDeltaTime;
            }
        } else {
            float Median;
            if (localLeftTraverse > localRightTraverse) {
                Median = ((localRightTraverse + localLeftTraverse)/2) - 180;
                if (CurrentAngle < TargetAngle && TargetAngle > Median || CurrentAngle > TargetAngle && TargetAngle < Median) {
                    CurrentAngle += rotationSpeed * Time.fixedDeltaTime;
                } else {
                    CurrentAngle -= rotationSpeed * Time.fixedDeltaTime;
                }
            } else {
                Median = (localRightTraverse + localLeftTraverse)/2;
                if (CurrentAngle < TargetAngle && TargetAngle < Median || CurrentAngle < TargetAngle && CurrentAngle > Median || CurrentAngle > TargetAngle && CurrentAngle >= Median && TargetAngle < Median) {
                    CurrentAngle += rotationSpeed * Time.fixedDeltaTime;
                } else {
                    CurrentAngle -= rotationSpeed * Time.fixedDeltaTime;
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

    private bool CheckNoFireZones(float CurrentAngle, float TargetAngle){
        // This fuction disables the firing capacity of the turrets on certain conditions.
        bool PreventFire = false;

        // Check first if any no fire zones are implemented, and if the turret is in it
        if (NoFireZones.Length > 0) {
            for (int i = 0; i < NoFireZones.Length; i++) {
                if (NoFireZones[i].ZoneBegin > NoFireZones[i].ZoneEnd) {
                    if (CurrentAngle > NoFireZones[i].ZoneBegin || CurrentAngle < NoFireZones[i].ZoneEnd) {
                        PreventFire = true;
                    }
                } else if (CurrentAngle > NoFireZones[i].ZoneBegin && CurrentAngle < NoFireZones[i].ZoneEnd ) {
                    PreventFire = true;
                }
            }
        }

        // if (debug) {
        //     Debug.Log("TargetAngleMax = "+ TargetAngleMax);
        //     Debug.Log("currentAngElev = "+ currentAngElev);
        //     Debug.Log("TargetAngleMin = "+ TargetAngleMin);
        // }

        float TargetAngleMax = TargetAngle + 4;
        // if (TargetAngleMax>360)
        //     TargetAngleMax -= 360;
        float TargetAngleMin = TargetAngle - 4;
        // if (TargetAngleMin<0)
        //     TargetAngleMin += 360;
        // Check if the turret is close to the firing targeting point
        if (CurrentAngle > TargetAngleMax || CurrentAngle < TargetAngleMin){
            PreventFire = true;
        }

        return PreventFire;
    }

    private void CannonElevation() {

        // Get parent current rotation rate
        float parentRotationAng = Parent.transform.rotation.eulerAngles.x-parentEulerAngles.x;

        float TargetAngleWorld = Quaternion.FromToRotation(Vector3.forward, TargetPosition - TurretCannon.transform.position).eulerAngles.x;

        currentAngElev = TurretCannon.transform.localRotation.eulerAngles.x;

        /* legacy code
            targetAngElev = TargetAngleWorld + parentEulerAngles.x;
            targetAngElev = TargetAngleWorld;
            if (currentAngElev > 180 && targetAngElev > 180 && currentAngElev > targetAngElev || currentAngElev < 180 && targetAngElev < 180 && currentAngElev > targetAngElev|| currentAngElev < 180 && targetAngElev > 180) {
                currentAngElev -= elevationSpeed * Time.fixedDeltaTime;
            } else {
                currentAngElev += elevationSpeed * Time.fixedDeltaTime;
            }
        */

        targetAngElev = (CameraPercentage * (upTraverse - downTraverse) / 100) + downTraverse;

        // Transform the elevation variable into the same axis than the limitations
        if (currentAngElev > 180)
            currentAngElev -= 360;
        currentAngElev += 180;
        currentAngElev = 360 - currentAngElev;
        currentAngElev -= 90;

        // if (debug) { Debug.Log("targetAngElev --- = "+ targetAngElev); }
        // if (debug) { Debug.Log("currentAngElev = "+ currentAngElev); }


        if (currentAngElev > targetAngElev) {
            currentAngElev -= elevationSpeed * Time.fixedDeltaTime;
        } else {
            currentAngElev += elevationSpeed * Time.fixedDeltaTime;
        }

        PreventFireVert = CheckNoFireVertical(currentAngElev,targetAngElev);
        
        // Transform back the elevation variable into the same axis than the limitations
        currentAngElev += 90;
        currentAngElev = 360 - currentAngElev;
        currentAngElev -= 180;
        if (currentAngElev < 0)
            currentAngElev += 360;


        currentAngElev += parentRotationAng;

        currentAngElev = CheckLimitElevation(currentAngElev);

        if (currentAngElev<0)
            currentAngElev += 360;
        if (currentAngElev>360)
            currentAngElev -= 360;

        // if (debug) {
        //     Debug.Log("targetAngElev = "+targetAngElev);
        //     Debug.Log("currentAngElev = "+currentAngElev);
        // }

        // Turn turret towards camera facing
        TurretCannon.transform.localRotation = Quaternion.Euler (new Vector3 (currentAngElev, 0.0f, 0.0f));

        // Debug.DrawRay(TurretCannon.transform.position, TurretCannon.transform.TransformDirection(Vector3.forward) * 1000, Color.red);
        
    }

    private float CheckLimitElevation(float currentElevation){
        float localUpTraverse = upTraverse;
        float localDownTraverse = downTraverse;

        // Transform the elevation variable into the same axis than the limitations
        if (currentElevation > 180)
            currentElevation -= 360;
        currentElevation += 180;
        currentElevation = 360 - currentElevation;
        currentElevation -= 90;

        // Check the elevations zones, and update the limit values if needed
        if (ElevationZones.Length > 0){
            for (int i = 0; i < ElevationZones.Length; i++) {
                if (ElevationZones[i].ZoneBegin > ElevationZones[i].ZoneEnd) {
                    if (currentAng > ElevationZones[i].ZoneBegin || currentAng < ElevationZones[i].ZoneEnd) {
                        if (ElevationZones[i].OverrideMaxElev){
                            localUpTraverse = ElevationZones[i].UpTraverse;
                        }
                        if (ElevationZones[i].OverrideMinElev) {
                            localDownTraverse = ElevationZones[i].DownTraverse;
                        }
                    }
                } else if (currentAng > ElevationZones[i].ZoneBegin && currentAng < ElevationZones[i].ZoneEnd ) {
                    if (ElevationZones[i].OverrideMaxElev){
                            localUpTraverse = ElevationZones[i].UpTraverse;
                        }
                        if (ElevationZones[i].OverrideMinElev) {
                            localDownTraverse = ElevationZones[i].DownTraverse;
                        }
                }
            }
        }

        // Implement limitations, clamp if the limitation is close
        if (currentElevation > (localUpTraverse+1)){
            currentElevation -= 2 * elevationSpeed * Time.fixedDeltaTime;
        }
        else if (currentElevation >= localUpTraverse){
            currentElevation = localUpTraverse;
        }
        else if (currentElevation < (localDownTraverse-1)){
            currentElevation += 2 * elevationSpeed * Time.fixedDeltaTime;
        }
        else if (currentElevation <= localDownTraverse){
            currentElevation = localDownTraverse;
        }

        CurrentAngleElevRatio = currentElevation - downTraverse;

        currentElevation += 90;
        currentElevation = 360 - currentElevation;
        currentElevation -= 180;
        if (currentElevation < 0)
            currentElevation += 360;
        
        return currentElevation;
    }

    private bool CheckNoFireVertical(float currentAngElev, float targetAngElev){
        // This fuction disables the firing capacity of the turrets on certain conditions.
        bool PreventFire = false;

        if (currentAngElev > targetAngElev+180) {
            targetAngElev += 360;
        }else if (currentAngElev+180 < targetAngElev) {
            targetAngElev -= 360;
        }

        float TargetAngleMax = targetAngElev + 4;
        float TargetAngleMin = targetAngElev - 4;

        // Check if the turret is close to the firing targeting point
        if (currentAngElev > TargetAngleMax || currentAngElev < TargetAngleMin){
            PreventFire = true;
        }
        // if (debug && PreventFire) {
        //     Debug.Log("currentAngElev = "+ currentAngElev);
        //     Debug.Log("targetAngElev = "+ targetAngElev);
        // }

        return PreventFire;
    }

    private void TurretStatic() {
        // Update the turret angle
        TurretTurret.transform.localRotation = Quaternion.Euler (new Vector3 (0.0f, currentAng, 0.0f));
    }

    public void SetActive(bool activate) {
        Active = activate;
    }
    public void SetMap(bool map) {
        MapActive = map;
    }

    public void SetTurretDeath(bool IsShipDead) {
        Dead = IsShipDead;
    }
}