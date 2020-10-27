using UnityEngine;
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
            


    private float RotationSpeed = 15.0f; public void SetRotationSpeed(float rotationSpeed){ RotationSpeed = rotationSpeed; }
    private bool LimitTraverse = false; public void SetLimitTraverse(bool limitTraverse){ LimitTraverse = limitTraverse; }
    private float LeftTraverse = 60.0f; public void SetLeftTraverse(float leftTraverse){ LeftTraverse = leftTraverse; }
    private float RightTraverse = 60.0f; public void SetRightTraverse(float rightTraverse){ RightTraverse = rightTraverse; }
    private float LocalLeftTraverse, LocalRightTraverse;

    private float ElevationSpeed = 15.0f; public void SetElevationSpeed(float elevationSpeed){ ElevationSpeed = elevationSpeed; }
    private float ElevationMax = 110.0f; public void SetElevationMax(float elevationMax){ ElevationMax = elevationMax; }
    private float ElevationMin = 80.0f; public void SetElevationMin(float elevationMin){ ElevationMin = elevationMin; }

    private FireZonesManager[] NoFireZones; public void SetNoFireZones(FireZonesManager[] noFireZones){ NoFireZones = noFireZones; }
    private ElevationZonesManager[] ElevationZones; public void SetElevationZones(ElevationZonesManager[] elevationZones){ ElevationZones = elevationZones; }


    private AudioSource _AudioSource;
    private AudioClip TurretRotationAudio; public void SetTurretRotationAudio(AudioClip turretRotationAudio){ TurretRotationAudio = turretRotationAudio; }
    public AudioClip GetTurretRotationAudio(){ return TurretRotationAudio; }
    private float PitchRange = 0.2f, OriginalPitch;
    private bool PlayRotationAudio = false;
    public void SetPlayRotationAudio(bool a){
        // Debug.Log("SetPlayRotationAudio"+ a);
        PlayRotationAudio = a; }
    private bool RotationAudioIsPlaying = false;
    public void SetRotationAudioIsPlaying(bool a){
        // Debug.Log("SetRotationAudioIsPlaying"+ a);
        RotationAudioIsPlaying = a; }

    private Vector3 ParentEulerAngles;     // Position/rotation of the direct parent
    private float TurretEulerAngle;                // Initial rotation of the turret
    private float TurretEulerElevAngle;           // initial elevation of the turret
    private float TargetAng;
    private float CurrentAng, CurrentAngSave;
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

    private Transform ParentTransform;      // Position/rotation of the parent (usually the hardpoint)
    private Transform Transform;            // Position/rotation of the turret itself, (the horizontal rotation axis)
    private Vector3 PositionSafeguard;
    private Transform ElevationTransform;   // Position/rotation of the guns, (Vertical elevation) IS ALWAYS AN OBJECT NAMED "VerticalAxis"

    public void BeginOperations(Transform parentTransform, HardPointComponent hardPointComponent, WorldSingleUnit.UnitHardPoint hardPointElement, GameObject turretSoundInstance){
        // Rotation transforms
            ParentTransform = parentTransform.parent;
            Transform = parentTransform;
            PositionSafeguard = parentTransform.position;
            ElevationTransform = this.transform.Find("VerticalAxis").transform;

        // Sound
            _AudioSource = turretSoundInstance.GetComponent<AudioSource>();
            OriginalPitch = _AudioSource.pitch;
            _AudioSource.pitch = Random.Range (OriginalPitch - PitchRange, OriginalPitch + PitchRange);
            

        ParentEulerAngles = parentTransform.rotation.eulerAngles;
        TurretEulerAngle = Transform.localRotation.eulerAngles.y;
        TurretEulerElevAngle = 90;
        LocalLeftTraverse = LeftTraverse + TurretEulerAngle;
        if (LocalLeftTraverse > 360)
            LocalLeftTraverse -= 360;
        LocalRightTraverse = 360 - RightTraverse + TurretEulerAngle;
        if (LocalRightTraverse > 360)
            LocalRightTraverse -= 360;


        CurrentAng = Transform.localRotation.eulerAngles.y;                  // Get initial angle horizontal
        CurrentAngElev = ElevationTransform.localRotation.eulerAngles.x;     // Get initial angle vertical

        StartCoroutine(PositionSafeguardLoop());
    }

    private void CheckAudio() {
        if (PlayRotationAudio && !RotationAudioIsPlaying && _AudioSource.clip != TurretRotationAudio) {
            // Debug.Log("_AudioSource.clip: " + _AudioSource.clip);
            PlayTurretAudio();
        } else if (!PlayRotationAudio && RotationAudioIsPlaying && _AudioSource.clip == TurretRotationAudio) {
            // Does not work as mentioned in documentation. .isPlaying returns true when audio source is stopped/ paused.
            // Debug.Log("_AudioSource.clip: " + _AudioSource.clip);
            StopTurretAudio();
        }
    }
    private void PlayTurretAudio() {
        // Debug.Log("PlayTurretAudio");
        _AudioSource.clip = TurretRotationAudio;
        _AudioSource.loop = true;
        _AudioSource.Play();
    }
    private void StopTurretAudio() {
        // Debug.Log("StopTurretAudio");
        _AudioSource.Stop();
        _AudioSource.clip = null;
        _AudioSource.loop = false;
    }
    public void SetPause(bool pause) {      // Pause music if game is paused
        if (pause && _AudioSource.clip == TurretRotationAudio) {
            _AudioSource.Pause();
        } else if (!pause && _AudioSource.clip == TurretRotationAudio) {
            _AudioSource.Play();
        }
    }

    private void FixedUpdate(){

        SetPlayRotationAudio(false);
        if (_AudioSource.clip == TurretRotationAudio) {
            // Debug.Log("_AudioSource.clip : " + _AudioSource.clip);
            SetRotationAudioIsPlaying(true);
        } else {
            SetRotationAudioIsPlaying(false);
        }


        CurrentAngSave = CurrentAng;

        if (!Dead) {
            // Debug.DrawRay(ElevationTransform.position, TargetPosition - ElevationTransform.position , Color.yellow);
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

        // Debug.Log("PlayRotationAudio : " + PlayRotationAudio+" RotationAudioIsPlaying : " + RotationAudioIsPlaying);
        CheckAudio();


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
        if (LimitTraverse) {
            CurrentAng = CheckLimitTraverse(CurrentAng);
        }

        // Debug.Log("CurrentAng = "+ CurrentAng+"TargetAng = "+ TargetAng);

        // if (TargetAng > CurrentAng && (TargetAng-3) > CurrentAng || TargetAng < CurrentAng && (TargetAng+3) < CurrentAng) {
        //     SetPlayRotationAudio(true);                   // For sounds, the turrets is currently rotating
        // }
        if (TargetAng > CurrentAng && (TargetAng-3) > CurrentAng || TargetAng < CurrentAng && (TargetAng+3) < CurrentAng) {
            if (CurrentAngSave > CurrentAng && (CurrentAngSave-(RotationSpeed * Time.fixedDeltaTime)) > CurrentAng || CurrentAngSave < CurrentAng && (CurrentAngSave+(RotationSpeed * Time.fixedDeltaTime)) < CurrentAng) {
                SetPlayRotationAudio(true);                   // For sounds, the turrets is currently rotating
            }
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
        if (TargetAng >= CurrentAng && TargetAng <= (CurrentAng+0.2)  || TargetAng <= CurrentAng && TargetAng >= (CurrentAng-0.2)) {
            // if (debug) { Debug.Log("Horizontal Close");}
            return;
        }

        // if (TargetAng > 359.5 && CurrentAng < 0.5 || TargetAng < 0.5 && CurrentAng > 359.5 || TargetAng >= CurrentAng && TargetAng <= (CurrentAng+0.2) || TargetAng >= (CurrentAng+0.5)) {
        //     PlayRotationAudio = true;                   // For sounds, the turrets is currently rotating
        // } else {
        //     PlayRotationAudio = false;
        // }

        // if (debug) { Debug.Log("CurrentAng = "+ CurrentAng+"TargetAng = "+ TargetAng);}

        // Debug.Log("CurrentAng = "+ CurrentAng+"TargetAng = "+ TargetAng);

        Transform.localRotation = Quaternion.Euler (new Vector3 (0.0f, CurrentAng, 0.0f));              // Update the turret angle

        // Debug.DrawLine(Transform.position, TargetPosition, Color.green);
    }

    private float BuildRotation(float currentAngle, float targetAngle){
        if (!LimitTraverse) {
            if (currentAngle < targetAngle && currentAngle+180 > TargetAng || currentAngle > targetAngle && currentAngle > targetAngle+180) {
                currentAngle += RotationSpeed * Time.fixedDeltaTime;
            } else {
                currentAngle -= RotationSpeed * Time.fixedDeltaTime;
            }
        } else {
            float Median;
            if (LocalLeftTraverse > LocalRightTraverse) {
                Median = ((LocalRightTraverse + LocalLeftTraverse)/2) - 180;
                if (currentAngle < targetAngle && targetAngle > Median || currentAngle > targetAngle && targetAngle < Median) {
                    currentAngle += RotationSpeed * Time.fixedDeltaTime;
                } else {
                    currentAngle -= RotationSpeed * Time.fixedDeltaTime;
                }
            } else {
                Median = (LocalRightTraverse + LocalLeftTraverse)/2;
                if (currentAngle < targetAngle && targetAngle < Median || currentAngle < targetAngle && currentAngle > Median || currentAngle > targetAngle && currentAngle >= Median && targetAngle < Median) {
                    currentAngle += RotationSpeed * Time.fixedDeltaTime;
                } else {
                    currentAngle -= RotationSpeed * Time.fixedDeltaTime;
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
        if (NoFireZones.Length > 0) {
            foreach (FireZonesManager fireZone in NoFireZones) {
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
        CurrentAngElev = ElevationTransform.localRotation.eulerAngles.x;

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

        TargetAngElev = (ElevationRatio * (ElevationMax - ElevationMin) / 100) + ElevationMin;      // Build traverse with the range required

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

        float speed = ElevationSpeed * Time.fixedDeltaTime;
        if (CurrentAngElev < TargetAngElev && CurrentAngElev + speed < TargetAngElev) {
            CurrentAngElev += speed;
        } else if (CurrentAngElev > TargetAngElev && CurrentAngElev - speed > TargetAngElev) {
            CurrentAngElev -= speed;
        } else {
            CurrentAngElev = TargetAngElev;
        }

        PreventFireVert = CheckNoFireVertical(CurrentAngElev,TargetAngElev);

        

        // Create a vector between the current position and the target
        //     Vector3 targetDir = TargetPosition - ElevationTransform.position;
        // // Get the angle between the facing of the current position and the new vector
        //     float signedAngle = Vector3.SignedAngle(targetDir, ElevationTransform.forward, Vector3.forward);
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

        ElevationTransform.localRotation = Quaternion.Euler (new Vector3 (CurrentAngElev, 0.0f, 0.0f));   // Set correct current angle to the cannons axis

        // Debug.DrawRay(ElevationTransform.position, ElevationTransform.TransformDirection(Vector3.forward) * 1000, Color.red);
    }
    private float CheckLimitElevation(float currentElevation){
        float localUpTraverse = ElevationMax;
        float localDownTraverse = ElevationMin;

        // Transform the elevation variable into the same axis than the limitations
        if (currentElevation > 180)
            currentElevation -= 360;
        currentElevation += 180;
        currentElevation = 360 - currentElevation;
        currentElevation -= 90;

        // Check the elevations zones, and update the limit values if needed
        if (ElevationZones.Length > 0){
            foreach (ElevationZonesManager elevationZone in ElevationZones) {
                if (elevationZone.ZoneBegin > elevationZone.ZoneEnd) {
                    if (CurrentAng > elevationZone.ZoneBegin || CurrentAng < elevationZone.ZoneEnd) {
                        if (elevationZone.OverrideMaxElev){
                            localUpTraverse = elevationZone.UpTraverse;
                        }
                        if (elevationZone.OverrideMinElev) {
                            localDownTraverse = elevationZone.DownTraverse;
                        }
                    }
                } else if (CurrentAng > elevationZone.ZoneBegin && CurrentAng < elevationZone.ZoneEnd ) {
                    if (elevationZone.OverrideMaxElev){
                        localUpTraverse = elevationZone.UpTraverse;
                    }
                    if (elevationZone.OverrideMinElev) {
                        localDownTraverse = elevationZone.DownTraverse;
                    }
                }
            }
        }

        // Implement limitations, clamp if the limitation is close
        if (currentElevation > (localUpTraverse+1)){
            currentElevation -= 2 * ElevationSpeed * Time.fixedDeltaTime;
        }
        else if (currentElevation >= localUpTraverse){
            currentElevation = localUpTraverse;
        }
        else if (currentElevation < (localDownTraverse-1)){
            currentElevation += 2 * ElevationSpeed * Time.fixedDeltaTime;
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