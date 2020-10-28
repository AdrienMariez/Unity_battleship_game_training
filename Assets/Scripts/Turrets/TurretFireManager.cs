using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurretFireManager : MonoBehaviour
{
    private bool Active = false;
    private bool PlayerControl = false;
    private bool UIActive = false;
    private bool Dead;
    // [Tooltip("Type of turret")] public TurretRole m_TurretRole;
    [Tooltip("Availables roles for the turret")] public TurretRole[] m_TurretRoles;
    private TurretRole TurretCurrentRole;
    [Tooltip("Ammo used")] public GameObject m_Shell;
    [Tooltip("Points where the shells will be spawned, make as many points as there is barrels")] 
    public Transform[] m_FireMuzzles;
    
    // Weapon datas
    private float MaxRange = 10000f; public void SetMaxRange(float range){ MaxRange = range; } public float GetMaxRange(){ return MaxRange; }   // Ranges are in meters.
    private float MinRange = 1000f; public void SetMinRange(float range){ MinRange = range; } public float GetMinRange(){ return MinRange; }
    // It appears the muzzle velocity as implemented ingame is too fast, real time based on Iowa 16"/406mm gives a ratio of *0.58
    private float MuzzleVelocity = 30f; public void SetMuzzleVelocity(float _v){ MuzzleVelocity = _v; } // Velocity in m/s
    private float ReloadTime = 5f;  public void SetReloadTime(float _t){ ReloadTime = _t; } // Time in seconds 
    private bool Reloading;
    private float ReloadingTimer;
    // [Tooltip("Dispersion of shells for this turret. In DB, 100 is the best precision and 1 is extremely poor precision.")]
    private float Precision = 0.01f;  public void SetPrecision(float _p){ Precision = Precision = 2 - ( _p / 50); } // Transform the DB precision percentage data to game use.

    // Audio
    private AudioSource _AudioSource;
    private AudioClip FireAudio; public void SetFireAudio(AudioClip fireAudio){ FireAudio = fireAudio; } public AudioClip GetFireAudio(){ return FireAudio; }

    // FX
    private GameObject FireFx; public void SetFireFx(GameObject _g){ FireFx = _g; }
        
    private TurretManager TurretManager;
    private int TurretNumber;
    private TurretRotation TurretRotation;
    private bool PreventFire;
    private bool OutOfRange;
    private float TargetRange;
    private Vector3 TargetPosition;
    private TurretManager.TurretStatusType TurretStatus;
    public enum TurretRole {
        NavalArtillery,
        Artillery,
        AA,
        Torpedo,
        DepthCharge,
        None
    }

    private bool AIControl = false;
    private bool AIPauseFire = false;

    // private void Start (){
    //     TurretRotation = GetComponent<TurretRotation>();
    //     // FreeLookCam = GameObject.Find("FreeLookCameraRig").GetComponent<FreeLookCam>();
    //     Precision = 2 - ( m_Precision / 50);        // Transform the public precision percentage data to game use.
    //     // Debug.Log("Precision = "+ Precision);
    // }

    public void BeginOperations(TurretRotation turretRotation, GameObject turretSoundInstance){
        TurretRotation = GetComponent<TurretRotation>();

        _AudioSource = turretSoundInstance.GetComponent<AudioSource>();
    }

    private void Update () {
        // if (debug) { Debug.Log("PreventFire = "+ PreventFire); Debug.Log("ReloadingTimer = "+ ReloadingTimer); }
        if (TargetRange > MaxRange && TurretCurrentRole == TurretRole.NavalArtillery || TargetRange > MaxRange && TurretCurrentRole == TurretRole.Artillery) {
            OutOfRange = true;
            CheckTurretStatus();
        }else{
            OutOfRange = false;
            CheckTurretStatus();
        }
        if (PlayerControl && !Reloading && !PreventFire && !OutOfRange && !Dead) {
            CheckTurretStatus();
            if (Input.GetButtonDown ("FireMainWeapon")) {
                //start the reloading process immediately
                Reloading = true;                                               // Start the reloading process immediately
                ReloadingTimer = ReloadTime;
                Fire ();
                CheckTurretStatus();
            }
        } else if (AIControl && !Reloading && !PreventFire && !OutOfRange && !Dead) {
            CheckTurretStatus();
            if (!AIPauseFire) {
                Reloading = true;                                               // Start the reloading process immediately
                ReloadingTimer = ReloadTime;
                Fire ();
                CheckTurretStatus();    
            }
        }

        if (Reloading && ReloadingTimer > 0) {
            ReloadingTimer-= Time.deltaTime;
            if (ReloadingTimer <= 0) {
                ReloadingTimer = ReloadTime;
                Reloading = !Reloading;
            }
            // Debug.Log("ReloadingTimer :"+ ReloadingTimer);
            // Debug.Log("Calculated fire range : "+ TargetRange);
        }

        // if (Active) {
        //     if (Dead){
        //         TurretStatus = TurretManager.TurretStatusType.Dead;
        //     }else if  (PreventFire || OutOfRange){
        //         TurretStatus = TurretManager.TurretStatusType.PreventFire;
        //     }else if (Reloading){
        //         TurretStatus = TurretManager.TurretStatusType.Reloading;
        //     }else{
        //         TurretStatus = TurretManager.TurretStatusType.Ready;
        //     }
        // }
    }

    IEnumerator PauseFire(){
        AIPauseFire = true;
        yield return new WaitForSeconds(2);
        AIPauseFire = false;
    }

    private void Fire () {
        foreach (Transform fireMuzzle in m_FireMuzzles) {
            //Build a lateral random factor for the shell launch
            float shellPrecisionZ = Random.Range(-Precision, Precision);
            Quaternion firingDirection = fireMuzzle.rotation;
            firingDirection.z += shellPrecisionZ * 0.02f;

            // if (debug) { Debug.Log("ShellPrecisionZ : "+ shellPrecisionZ); }
            // if (debug) { Debug.Log("m_FireMuzzles[i].rotation : "+ firingDirection); }

            GameObject shellInstance =
                Instantiate (m_Shell, fireMuzzle.position, firingDirection);

            SendNeededInfoToShell(shellInstance);

            TurretManager.SendPlayerShellToUI(shellInstance);

            FireFX(fireMuzzle);

            // AUDIO FX
            // Change the clip to the firing clip and play it.
            _AudioSource.clip = FireAudio;
            _AudioSource.Play ();  
        }
    }
    private void SendNeededInfoToShell(GameObject shellInstance) {
        if (TurretCurrentRole == TurretRole.Artillery || TurretCurrentRole == TurretRole.NavalArtillery) {
            shellInstance.GetComponent<ShellStat>().SetTargetRange(TargetRange);
            if (AIControl) {
                shellInstance.GetComponent<ShellStat>().SetFiringMode(TurretRole.NavalArtillery);       // Don't let the AI shoot Artillery
            }
        } else {
            shellInstance.GetComponent<ShellStat>().SetTargetRange(MaxRange);       // Sends max range instead of target range to the unit. This may change in the future
            shellInstance.GetComponent<ShellStat>().SetFiringMode(TurretCurrentRole);
        }
        shellInstance.GetComponent<ShellStat>().SetTargetPosition(TargetPosition);
        shellInstance.GetComponent<ShellStat>().SetWasAILaunched(AIControl);

        shellInstance.GetComponent<ShellStat>().SetMuzzleVelocity(MuzzleVelocity * 0.58f);
        shellInstance.GetComponent<ShellStat>().SetPrecision(Precision);
        shellInstance.GetComponent<ShellStat>().SetParentTurretManager(TurretManager);
    }

    private void FireFX(Transform fireMuzzle) {
        // VISUAL FX
        if (FireFx == null) { return; }

        GameObject fireFxInstance = Instantiate(FireFx, fireMuzzle);

        if (fireFxInstance == null) { return; }
        // Play the particle system.
        fireFxInstance.GetComponent<ParticleSystem>().Play();
        // Play the explosion sound effect.
        // fireFxInstance.GetComponent<AudioSource>().Play();

        Destroy (fireFxInstance.gameObject, fireFxInstance.GetComponent<ParticleSystem>().main.startLifetime.constant);
    }

    public void SetPlayerControl(bool playerControl) { PlayerControl = playerControl; }
    public void SetActive(bool activate) {
        Active = activate;
    }
    public void SetAIControl(bool aiControl) { AIControl = aiControl; StartCoroutine(PauseFire()); }
    public void SetPreventFire(bool status){
        PreventFire = status;
        if (PreventFire)
            CheckTurretStatus();
    }
    public void SetTargetRange(float range){ TargetRange = range; }
    public void SetTargetPosition(Vector3 position) { 
        TargetPosition = position;
    }
    public void SetTurretDeath(bool IsShipDead) {
        Dead = IsShipDead;
        if (Dead)
            CheckTurretStatus();
    }

    private void CheckTurretStatus() {
        TurretManager.TurretStatusType statusType = GetTurretStatus();
        if (UIActive && statusType != TurretStatus) {
            // Debug.Log ("TurretNumber : "+ TurretNumber);
            TurretManager.SetSingleTurretStatus(statusType, TurretNumber);
        }
        TurretStatus = statusType;
    }
    public TurretManager.TurretStatusType GetTurretStatus() {
        TurretManager.TurretStatusType status;
        if (Dead){
            status = TurretManager.TurretStatusType.Dead;
        } else if  (PreventFire){
            status = TurretManager.TurretStatusType.PreventFire;
        } else if  (OutOfRange){
            status = TurretManager.TurretStatusType.PreventFire;
        } else if (Reloading){
            status = TurretManager.TurretStatusType.Reloading;
        } else {
            status = TurretManager.TurretStatusType.Ready;
        }
        return status;
    }
    public void SetTurretNumber(int turretNumber) {
        TurretNumber = turretNumber;
        // Debug.Log ("TurretStatus : "+ TurretStatus);
        // If the turrets are switched, wait a bit for the turrets UI to set up and force feed it the current values
        if (PlayerControl) {
            StartCoroutine(PauseAction());
        }
    }
    IEnumerator PauseAction(){ yield return new WaitForSeconds(0.02f); ReturnActiveTurretsStatus(); }
    public void ReturnActiveTurretsStatus() { TurretManager.SetSingleTurretStatus(TurretStatus, TurretNumber); }
    public void SetTurretManager(TurretManager turretManager){ TurretManager = turretManager; }
    public void SetTurretUIActive(bool uiActive){ UIActive = uiActive; }
    public void SetCurrentControlledTurretRole(TurretRole currentControlledTurretRole) { TurretCurrentRole = currentControlledTurretRole; }
    public TurretRole[] GetTurretRoles() { return m_TurretRoles; }
}