using UnityEngine;
using System.Collections;

public class TurretRotation : MonoBehaviour
{ 
    private bool PlayerControl = false;
    private bool Dead;

    [Header("Elements")]
        [Tooltip ("Point to which the cannon will point when in idle position")]
            public Transform m_IdlePointer;
        [Tooltip ("Audio played when the turret is rotating.")]
            public AudioClip m_TurretRotationAudio;
        [Tooltip ("Axis of rotation for horizontal rotation of the turret.")]
            public Rigidbody m_TurretTurret;
        [Tooltip ("Axis of rotation for the elevation of the cannon.")]
            public Rigidbody m_TurretCannon;
        [Tooltip ("Direct parent of this turret, place the unit rigidbody here by default, but you can put a turret on top of another by placing the parent turret here. ")]
            public Rigidbody m_Parent;

        [Header("Horizontal rotation")]
        [Tooltip ("Rotation Speed. (°/s)")]
            public float m_RotationSpeed = 15.0f;

        [Tooltip("When true, turret rotates according to left/right traverse limits. When false, turret can rotate freely.")]
            public bool m_LimitTraverse = false;
        [Tooltip("When traverse is limited, how many degrees to the left the turret can turn.")]
            [Range(0.0f, 180.0f)]
            public float m_LeftTraverse = 60.0f;
            private float LocalLeftTraverse;
        [Tooltip("When traverse is limited, how many degrees to the right the turret can turn.")]
            [Range(0.0f, 180.0f)]
            public float m_RightTraverse = 60.0f; 
            private float LocalRightTraverse;
        public FireZonesManager[] m_NoFireZones;

    [Header("Vertical elevation")]
        [Tooltip ("Maximum elevation Speed. (Degree per Second)")] public float m_ElevationSpeed = 15.0f;

        [Tooltip("Maximum elevation (degrees) of the turret. 90° is horizontal.")]
        [Range(0.0f, 180.0f)]
        public float m_UpTraverse = 110.0f;
        [Tooltip("Depression (degrees) of the turret.  90° is horizontal.")]
        [Range(0.0f, 180.0f)]
        public float m_DownTraverse = 80.0f;
        public ElevationZonesManager[] m_ElevationZones;

    [Header("Debug")]
        public bool debug = false;

    [Tooltip ("Position/rotation of the direct parent")] private Vector3 ParentEulerAngles;
    [Tooltip ("initial rotation of the turret")] private float TurretEulerAngle;
    private float TargetAng;
    private float CurrentAng;
    private float TargetAngElev;
    private float CurrentAngElev;
    private Vector3 TargetPosition;
    private float CameraPercentage;
    private TurretFireManager TurretFireManager;
    private bool PreventFireHoriz = false;
    private bool PreventFireVert = false;
    private float TotalAngleElevRatio;
    private float CurrentAngleElevRatio;

    private bool AIControl = false;
    private bool ActionPaused = false;

    private void Awake(){
        TurretFireManager = GetComponent<TurretFireManager>();
        ParentEulerAngles = m_Parent.transform.rotation.eulerAngles;
        TurretEulerAngle = m_TurretTurret.transform.localRotation.eulerAngles.y;
        LocalLeftTraverse = m_LeftTraverse + TurretEulerAngle;
        if (LocalLeftTraverse > 360)
            LocalLeftTraverse -= 360;
        LocalRightTraverse = 360 - m_RightTraverse + TurretEulerAngle;
        if (LocalRightTraverse > 360)
            LocalRightTraverse -= 360;

        TotalAngleElevRatio = m_UpTraverse-m_DownTraverse;

        CurrentAng = m_TurretTurret.transform.localRotation.eulerAngles.y;
        
        // if (debug) {
        //     Debug.Log("TurretEulerAngle: " + TurretEulerAngle);
        //     Debug.Log("m_LeftTraverse: " + m_LeftTraverse);
        //     Debug.Log("m_RightTraverse: " + m_RightTraverse);
        //     Debug.Log("LocalLeftTraverse: " + LocalLeftTraverse);
        //     Debug.Log("localRightTraverse: " + localRightTraverse);
            // Debug.Log("m_IdlePosition: " + m_IdlePosition);
        // }
    }

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
        if (!PlayerControl && !AIControl){
            TargetPosition = m_IdlePointer.transform.position;
        }

        // if (PlayerContro && !Dead) {
        if (!Dead) {
            TurretRotate();
            CannonElevation();
        }
        // else if (!Dead && ActionPaused) {
        //     CannonElevation();
        // }
        else {
            TurretStatic();
        }

        // Check if anything can prevent the turret from firing
        if (PreventFireHoriz || PreventFireVert){
            TurretFireManager.SetPreventFire(true);
        } else{
            TurretFireManager.SetPreventFire(false);

            // if (debug) {
                // Debug.Log("current: " + currentAngElev);
                // Debug.Log("total: " + TotalAngleElevRatio);
                // Debug.Log("percent sent: " + CurrentAngleElevRatio);
            // }
        }
        
        // Reassign the new parent angle for future TurretRotate()
        ParentEulerAngles = m_Parent.transform.rotation.eulerAngles;

        // Reposition the turret as it can be moved out of position
        if (!ActionPaused) {
            m_TurretTurret.transform.localPosition = new Vector3 (0.0f, 0.0f, 0.0f);
        }
        if (!ActionPaused) {
            StartCoroutine(PauseAction());
        }
    }

    IEnumerator PauseAction(){
        ActionPaused = true;
        yield return new WaitForSeconds(0.1f);
        ActionPaused = false;
    }

    private void TurretRotate() {
        // Get parent current rotation rate
        float parentRotationAng = m_Parent.transform.rotation.eulerAngles.y-ParentEulerAngles.y;
        if (parentRotationAng<-360)
            parentRotationAng += 360;
        else if (parentRotationAng>360)
            parentRotationAng -= 360;


        float TargetAngleWorld = Quaternion.FromToRotation(Vector3.forward, TargetPosition - m_TurretTurret.transform.position).eulerAngles.y;

        TargetAng = TargetAngleWorld - ParentEulerAngles.y;
        
        if (TargetAng<0)
            TargetAng += 360;
        else if (TargetAng>360)
            TargetAng -= 360;


        // currentAng = m_TurretTurret.transform.localRotation.eulerAngles.y;

        // if (debug) { Debug.Log("parentRotationAng = "+ parentRotationAng); }
        
        // Rotate
        CurrentAng = BuildRotation(CurrentAng,TargetAng);

        // Check if the turret is hitting a limitation
        if (m_LimitTraverse) {
            CurrentAng = CheckLimitTraverse(CurrentAng);
        }

        // Add parent rotation rate to the new current angle so that a rotating tank can turn its turret while rotating himself
        CurrentAng += parentRotationAng;


        if (CurrentAng<0)
            CurrentAng += 360;
        if (CurrentAng>360)
            CurrentAng -= 360;

        // Check if the turret is in a no-fire zone
        PreventFireHoriz = CheckNoFireZones(CurrentAng,TargetAng);

        // if (debug) { Debug.Log("currentAng = "+ currentAng); Debug.Log("TargetAng = "+ TargetAng); }

        // Update the turret angle
        m_TurretTurret.transform.localRotation = Quaternion.Euler (new Vector3 (0.0f, CurrentAng, 0.0f));

        Debug.DrawLine(m_TurretTurret.transform.position, TargetPosition, Color.green);
    }

    private float BuildRotation(float currentAngle, float targetAngle){
        if (!m_LimitTraverse) {
            if (currentAngle < targetAngle && currentAngle+180 > TargetAng || currentAngle > targetAngle && currentAngle > targetAngle+180) {
                currentAngle += m_RotationSpeed * Time.fixedDeltaTime;
            } else {
                currentAngle -= m_RotationSpeed * Time.fixedDeltaTime;
            }
        } else {
            float Median;
            if (LocalLeftTraverse > LocalRightTraverse) {
                Median = ((LocalRightTraverse + LocalLeftTraverse)/2) - 180;
                if (currentAngle < targetAngle && targetAngle > Median || currentAngle > targetAngle && targetAngle < Median) {
                    currentAngle += m_RotationSpeed * Time.fixedDeltaTime;
                } else {
                    currentAngle -= m_RotationSpeed * Time.fixedDeltaTime;
                }
            } else {
                Median = (LocalRightTraverse + LocalLeftTraverse)/2;
                if (currentAngle < targetAngle && targetAngle < Median || currentAngle < targetAngle && currentAngle > Median || currentAngle > targetAngle && currentAngle >= Median && targetAngle < Median) {
                    currentAngle += m_RotationSpeed * Time.fixedDeltaTime;
                } else {
                    currentAngle -= m_RotationSpeed * Time.fixedDeltaTime;
                }
            }
        }
        return currentAngle;
    }

    private float CheckLimitTraverse(float CurrentAngle){
        if (LocalLeftTraverse > LocalRightTraverse) {
            if (CurrentAngle >= LocalLeftTraverse) {
                CurrentAngle = LocalLeftTraverse;
            }
            if (CurrentAngle <= LocalRightTraverse) {
                CurrentAngle = LocalRightTraverse;
            }
        } else {
            if (CurrentAngle >= LocalLeftTraverse && CurrentAngle <= LocalRightTraverse) {
                if (CurrentAngle < (LocalLeftTraverse + 1)) {
                    CurrentAngle = LocalLeftTraverse;
                } else if (CurrentAngle > (LocalRightTraverse - 1)) {
                    CurrentAngle = LocalRightTraverse;
                }
            }
        }
        return CurrentAngle;
    }

    private bool CheckNoFireZones(float CurrentAngle, float TargetAngle){
        // This fuction disables the firing capacity of the turrets on certain conditions.
        bool PreventFire = false;

        // Check first if any no fire zones are implemented, and if the turret is in it
        if (m_NoFireZones.Length > 0) {
            for (int i = 0; i < m_NoFireZones.Length; i++) {
                if (m_NoFireZones[i].ZoneBegin > m_NoFireZones[i].ZoneEnd) {
                    if (CurrentAngle > m_NoFireZones[i].ZoneBegin || CurrentAngle < m_NoFireZones[i].ZoneEnd) {
                        PreventFire = true;
                    }
                } else if (CurrentAngle > m_NoFireZones[i].ZoneBegin && CurrentAngle < m_NoFireZones[i].ZoneEnd ) {
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
        float parentRotationAng = m_Parent.transform.rotation.eulerAngles.x - ParentEulerAngles.x;

        // float TargetAngleWorld = Quaternion.FromToRotation(Vector3.forward, TargetPosition - m_TurretCannon.transform.position).eulerAngles.x;

        CurrentAngElev = m_TurretCannon.transform.localRotation.eulerAngles.x;

        if (CameraPercentage + 30 > 100)
            CameraPercentage = 100;
        else
            CameraPercentage += 30;

        TargetAngElev = (CameraPercentage * (m_UpTraverse - m_DownTraverse) / 100) + m_DownTraverse;

        // Transform the elevation variable into the same axis than the limitations
        if (CurrentAngElev > 180)
            CurrentAngElev -= 360;
        CurrentAngElev += 180;
        CurrentAngElev = 360 - CurrentAngElev;
        CurrentAngElev -= 90;

        // if (debug) { Debug.Log("targetAngElev --- = "+ targetAngElev); }
        // if (debug) { Debug.Log("currentAngElev = "+ currentAngElev); }

        float speed = m_ElevationSpeed * Time.fixedDeltaTime;
        if (CurrentAngElev < TargetAngElev && CurrentAngElev + speed < TargetAngElev) {
            CurrentAngElev += speed;
        } else if (CurrentAngElev > TargetAngElev && CurrentAngElev - speed > TargetAngElev) {
            CurrentAngElev -= speed;
        } else {
            CurrentAngElev = TargetAngElev;
        }

        PreventFireVert = CheckNoFireVertical(CurrentAngElev,TargetAngElev);
        
        // Transform back the elevation variable into the same axis than the limitations
        CurrentAngElev += 90;
        CurrentAngElev = 360 - CurrentAngElev;
        CurrentAngElev -= 180;
        if (CurrentAngElev < 0)
            CurrentAngElev += 360;


        CurrentAngElev += parentRotationAng;

        CurrentAngElev = CheckLimitElevation(CurrentAngElev);

        if (CurrentAngElev < 0)
            CurrentAngElev += 360;
        if (CurrentAngElev > 360)
            CurrentAngElev -= 360;

        // if (debug) {
        //     Debug.Log("targetAngElev = "+TargetAngElev);
        //     Debug.Log("currentAngElev = "+CurrentAngElev);
        // }

        // Set correct current angle to the cannons axis
        m_TurretCannon.transform.localRotation = Quaternion.Euler (new Vector3 (CurrentAngElev, 0.0f, 0.0f));

        // Debug.DrawRay(m_TurretCannon.transform.position, m_TurretCannon.transform.TransformDirection(Vector3.forward) * 1000, Color.red);
        
    }

    private float CheckLimitElevation(float currentElevation){
        float localUpTraverse = m_UpTraverse;
        float localDownTraverse = m_DownTraverse;

        // Transform the elevation variable into the same axis than the limitations
        if (currentElevation > 180)
            currentElevation -= 360;
        currentElevation += 180;
        currentElevation = 360 - currentElevation;
        currentElevation -= 90;

        // Check the elevations zones, and update the limit values if needed
        if (m_ElevationZones.Length > 0){
            for (int i = 0; i < m_ElevationZones.Length; i++) {
                if (m_ElevationZones[i].ZoneBegin > m_ElevationZones[i].ZoneEnd) {
                    if (CurrentAng > m_ElevationZones[i].ZoneBegin || CurrentAng < m_ElevationZones[i].ZoneEnd) {
                        if (m_ElevationZones[i].OverrideMaxElev){
                            localUpTraverse = m_ElevationZones[i].UpTraverse;
                        }
                        if (m_ElevationZones[i].OverrideMinElev) {
                            localDownTraverse = m_ElevationZones[i].DownTraverse;
                        }
                    }
                } else if (CurrentAng > m_ElevationZones[i].ZoneBegin && CurrentAng < m_ElevationZones[i].ZoneEnd ) {
                    if (m_ElevationZones[i].OverrideMaxElev){
                            localUpTraverse = m_ElevationZones[i].UpTraverse;
                        }
                        if (m_ElevationZones[i].OverrideMinElev) {
                            localDownTraverse = m_ElevationZones[i].DownTraverse;
                        }
                }
            }
        }

        // Implement limitations, clamp if the limitation is close
        if (currentElevation > (localUpTraverse+1)){
            currentElevation -= 2 * m_ElevationSpeed * Time.fixedDeltaTime;
        }
        else if (currentElevation >= localUpTraverse){
            currentElevation = localUpTraverse;
        }
        else if (currentElevation < (localDownTraverse-1)){
            currentElevation += 2 * m_ElevationSpeed * Time.fixedDeltaTime;
        }
        else if (currentElevation <= localDownTraverse){
            currentElevation = localDownTraverse;
        }

        CurrentAngleElevRatio = currentElevation - m_DownTraverse;

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
        m_TurretTurret.transform.localRotation = Quaternion.Euler (new Vector3 (0.0f, CurrentAng, 0.0f));
    }
    public void SetPlayerControl(bool playerControl) { PlayerControl = playerControl; }
    public void SetAIControl(bool aiControl) { AIControl = aiControl; }
    public void SetAIGroundTargetPosition(Vector3 groundTargetPosition) {  }
    public void SetTurretDeath(bool IsShipDead) { Dead = IsShipDead; }
    public void SetCameraPercentage(float percentage) {
        percentage = (Mathf.Round(percentage));
        if (CameraPercentage != percentage)
            CameraPercentage = percentage;
    }
    public void SetTargetPosition(Vector3 position) { TargetPosition = position; }
}