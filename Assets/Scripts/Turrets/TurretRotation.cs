﻿using UnityEngine;
using System.Collections;

public class TurretRotation : MonoBehaviour
{ 
    private bool PlayerControl = false;
    private bool Dead;

    [Header("Elements")]
        [Tooltip ("Audio played when the turret is rotating.")]
            public AudioClip m_TurretRotationAudio;
        [Tooltip ("Axis of rotation for the elevation of the cannon.")]
            public Rigidbody m_TurretCannon;

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
    [Tooltip ("initial elevation of the turret")] private float TurretEulerElevAngle;
    private float TargetAng;
    private float CurrentAng;
    private float TargetAngElev;
    private float CurrentAngElev;
    private Vector3 TargetPosition;
    private float ElevationRatio;
    private TurretFireManager TurretFireManager;
    private bool PreventFireHoriz = false;
    private bool PreventFireVert = false;

    private bool AIControl = false;
    private bool ActionPaused = false;
    private bool TurretSleep = true;

    private Transform ParentTransform;
    private Transform Transform;
    private Vector3 PositionSafeguard;

    public void BeginOperations(Transform parentTransform){
        ParentTransform = parentTransform.parent;
        Transform = parentTransform;
        PositionSafeguard = parentTransform.position;


        ParentEulerAngles = parentTransform.rotation.eulerAngles;
        TurretEulerAngle = Transform.localRotation.eulerAngles.y;
        TurretEulerElevAngle = 90;
        LocalLeftTraverse = m_LeftTraverse + TurretEulerAngle;
        if (LocalLeftTraverse > 360)
            LocalLeftTraverse -= 360;
        LocalRightTraverse = 360 - m_RightTraverse + TurretEulerAngle;
        if (LocalRightTraverse > 360)
            LocalRightTraverse -= 360;


        CurrentAng = Transform.localRotation.eulerAngles.y;                  // Gets start angle horizontal

        CurrentAngElev = m_TurretCannon.transform.localRotation.eulerAngles.x;              // Gets start angle vertical

        StartCoroutine(PositionSafeguardLoop());
        
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

        if (!Dead) {
            // Debug.DrawRay(m_TurretCannon.transform.position, TargetPosition - m_TurretCannon.transform.position , Color.yellow);
            // if (debug) { Debug.Log("TargetPosition: " + TargetPosition); }
            TurretRotate();

            CannonElevation();
        } else {
            TurretStatic();
        }

        // Check if anything can prevent the turret from firing
        if (PreventFireHoriz || PreventFireVert){
            TurretFireManager.SetPreventFire(true);
        } else{
            TurretFireManager.SetPreventFire(false);
        }

        // if (debug) {
        //     Debug.Log("PreventFireHoriz: " + PreventFireHoriz+"PreventFireVert: " + PreventFireVert);
        // }
            // if (debug) {
                // Debug.Log("current: " + currentAngElev);
                // Debug.Log("percent sent: " + CurrentAngleElevRatio);
            //     Debug.Log("PreventFireHoriz: " + PreventFireHoriz+"PreventFireVert: " + PreventFireVert);
            // }
        
        // Reassign the new parent angle for future TurretRotate()
        ParentEulerAngles = ParentTransform.rotation.eulerAngles;

        if (!ActionPaused) {
            StartCoroutine(PauseAction());
        }
    }
    IEnumerator PositionSafeguardLoop(){
        while (true) {
            yield return new WaitForSeconds(2f);
            Transform.localPosition = PositionSafeguard;
        }
    }

    IEnumerator PauseAction(){
        ActionPaused = true;
        yield return new WaitForSeconds(0.1f);
        ActionPaused = false;
    }

    private void TurretRotate() {
        // float parentRotationAng = ParentTransform.rotation.eulerAngles.y-ParentEulerAngles.y;     // Get parent current rotation rate
        // if (parentRotationAng<-360)                                                                  // Works but apparently useless
        //     parentRotationAng += 360;
        // else if (parentRotationAng>360)
        //     parentRotationAng -= 360;


        float targetAngleWorld = Quaternion.FromToRotation(Vector3.forward, TargetPosition - Transform.position).eulerAngles.y;

        TargetAng = targetAngleWorld - ParentEulerAngles.y;

        if (TurretSleep) {
            TargetAng = TurretEulerAngle;
        }
        
        if (TargetAng<0)
            TargetAng += 360;
        else if (TargetAng>360)
            TargetAng -= 360;


        // currentAng = Transform.localRotation.eulerAngles.y;

        // if (debug) { Debug.Log("TurretSleep = "+ TurretSleep); }

        // Add parent rotation rate to the new current angle so that a rotating unit can turn its turret while rotating himself
        // Tests indicated this was not neccessary...
        // CurrentAng += parentRotationAng * Time.fixedDeltaTime;
        
        // Rotate
        CurrentAng = BuildRotation(CurrentAng, TargetAng);

        // Check if the turret is hitting a limitation
        if (m_LimitTraverse) {
            CurrentAng = CheckLimitTraverse(CurrentAng);
        }

        if (CurrentAng<0)
            CurrentAng += 360;
        if (CurrentAng>360)
            CurrentAng -= 360;

        // Check if the turret is in a no-fire zone
        PreventFireHoriz = CheckNoFireZones(CurrentAng,TargetAng);

        // Last check to see if the turret is very close to the target angle, to avoid unwanted shaking
        if (TurretSleep && TargetAng > 359.5 && CurrentAng < 0.5 || TurretSleep && TargetAng < 0.5 && CurrentAng > 359.5) {
            return;
        }
        if (TargetAng >= CurrentAng && TargetAng <= (CurrentAng+0.1)  || TargetAng <= CurrentAng && TargetAng >= (CurrentAng-0.1)) {
            // if (debug) { Debug.Log("Vertical Close");}
            return;
        }

        // if (debug) { Debug.Log("CurrentAng = "+ CurrentAng+"TargetAng = "+ TargetAng);}

        // Update the turret angle
        Transform.localRotation = Quaternion.Euler (new Vector3 (0.0f, CurrentAng, 0.0f));

        // Debug.DrawLine(Transform.position, TargetPosition, Color.green);
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
            foreach (FireZonesManager fireZone in m_NoFireZones) {
                if (fireZone.ZoneBegin > fireZone.ZoneEnd) {
                    if (CurrentAngle > fireZone.ZoneBegin || CurrentAngle < fireZone.ZoneEnd) {
                        PreventFire = true;
                    }
                } else if (CurrentAngle > fireZone.ZoneBegin && CurrentAngle < fireZone.ZoneEnd ) {
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
        CurrentAngElev = m_TurretCannon.transform.localRotation.eulerAngles.x;

        float parentRotationAng = ParentTransform.rotation.eulerAngles.x - ParentEulerAngles.x;      // Get parent current rotation rate

        float targetAngleWorld = Quaternion.FromToRotation(Vector3.forward, TargetPosition - Transform.position).eulerAngles.x;      // Gets the x angle between parent and target.

        if (targetAngleWorld > 180)                                                                     // Transform the world angle variable into the same axis than the limitations
            targetAngleWorld -= 360;
        targetAngleWorld += 180;
        targetAngleWorld = 360 - targetAngleWorld;
        targetAngleWorld -= 180;

        if (ElevationRatio + 20 > 100)          // Gives a little boost to the elevation to prevent shells trajectories too flat or ending in the water. May be upgraded to better account for smaller trajectories
            ElevationRatio = 100;
        else
            ElevationRatio += 20;

        TargetAngElev = (ElevationRatio * (m_UpTraverse - m_DownTraverse) / 100) + m_DownTraverse;      // Build traverse with the range required

        // if (debug) { Debug.Log("targetAngleWorld --- = "+ targetAngleWorld); }

        TargetAngElev += targetAngleWorld;                                                              // Add the angle to the target

        if (TurretSleep) {
            TargetAngElev = TurretEulerElevAngle;
        }

        if (CurrentAngElev > 180)                                                                       // Transform the elevation variable into the same axis than the limitations
            CurrentAngElev -= 360;
        CurrentAngElev += 180;
        CurrentAngElev = 360 - CurrentAngElev;
        CurrentAngElev -= 90;

        if (TargetAngElev >= CurrentAngElev && TargetAngElev <= (CurrentAngElev+0.5) || TargetAngElev <= CurrentAngElev && TargetAngElev >= (CurrentAngElev-0.5)) {
            // if (debug) { Debug.Log("Elevation close !"); }
            return;                                                                                     // If the angles are very close, prevent turning
        }

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

        

        // Create a vector between the current position and the target
        //     Vector3 targetDir = TargetPosition - m_TurretCannon.transform.position;
        // // Get the angle between the facing of the current position and the new vector
        //     float signedAngle = Vector3.SignedAngle(targetDir, m_TurretCannon.transform.forward, Vector3.forward);
        // if (debug) { Debug.Log("signedAngle = "+ signedAngle); }   
        
        CurrentAngElev += 90;                                                                           // Transform back the elevation variable into the same axis than the limitations
        CurrentAngElev = 360 - CurrentAngElev;
        CurrentAngElev -= 180;
        if (CurrentAngElev < 0)
            CurrentAngElev += 360;


        CurrentAngElev += parentRotationAng;                                                            // Add parent current rotation rate

        CurrentAngElev = CheckLimitElevation(CurrentAngElev);

        if (CurrentAngElev < 0)
            CurrentAngElev += 360;
        if (CurrentAngElev > 360)
            CurrentAngElev -= 360;

        // if (debug) { Debug.Log("TargetAngElev = "+TargetAngElev+"CurrentAngElev = "+CurrentAngElev); }

        m_TurretCannon.transform.localRotation = Quaternion.Euler (new Vector3 (CurrentAngElev, 0.0f, 0.0f));   // Set correct current angle to the cannons axis

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
        //     Debug.Log("currentAngElev = "+ currentAngElev+"- targetAngElev = "+ targetAngElev);
        // }

        return PreventFire;
    }
    private bool CheckNoFireVerticalArtillery(float currentAngElev, float targetAngElev){
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
        //     Debug.Log("currentAngElev = "+ currentAngElev+"- targetAngElev = "+ targetAngElev);
        // }

        return PreventFire;
    }
    private bool CheckNoFireVerticalAA(float currentAngElev, float targetAngElev){
        // This fuction disables the firing capacity of the turrets on certain conditions.
        bool PreventFire = false;

        float targetAngleMax = targetAngElev + 4;
        float targetAngleMin = targetAngElev - 4;

        // Check if the turret is close to the firing targeting point
        if (currentAngElev > targetAngleMax || currentAngElev < targetAngleMin){
            PreventFire = true;
        }
        // if (debug) {
        //     Debug.Log("currentAngElev = "+ currentAngElev+"- targetAngElev = "+ targetAngElev);
        // }

        return PreventFire;
    }


    private void TurretStatic() {
        // Update the turret angle
        Transform.localRotation = Quaternion.Euler (new Vector3 (0.0f, CurrentAng, 0.0f));
    }

    public void SetTurretFireManager(TurretFireManager turretFireManager){ TurretFireManager = turretFireManager; }
    public void SetPlayerControl(bool playerControl) { PlayerControl = playerControl; SetTurretSleep(); }
    public void SetAIControl(bool aiControl) { AIControl = aiControl; SetTurretSleep(); }
    private void SetTurretSleep() {
        if (!PlayerControl && !AIControl){
            TurretSleep = true;
            // TargetPosition = m_IdlePointer.transform.position;
        } else {
            TurretSleep = false;
        }
        // if (debug) { Debug.Log("SetTurretSleep = "+ TurretSleep +" - "+ PlayerControl+" / "+ AIControl); }

    }
    public void SetAIGroundTargetPosition(Vector3 groundTargetPosition) {  }
    public void SetTurretDeath(bool IsShipDead) { Dead = IsShipDead; }
    public void SetElevationRatio(float percentage) {
        percentage = (Mathf.Round(percentage));
        if (ElevationRatio != percentage)
            ElevationRatio = percentage;
    }
    public void SetTargetPosition(Vector3 position) { TargetPosition = position; }

    private TurretFireManager.TurretRole CurrentControlledTurretRole;
    public void SetCurrentControlledTurretRole(TurretFireManager.TurretRole currentControlledTurretRole) { CurrentControlledTurretRole = currentControlledTurretRole; }
}