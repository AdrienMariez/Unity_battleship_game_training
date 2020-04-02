using UnityEngine;
// using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
// using FreeLookCamera;

public class TurretFireManager : MonoBehaviour
{
    private bool Active = false;
    private bool PlayerControl = false;
    private bool UIActive = false;
    private bool Dead;
    [Tooltip("Type of turret")] public TurretType m_TurretType;
    [Tooltip("Ammo used")] public GameObject m_Shell;
    [Tooltip("Points where the shells will be spawned, make as many points as there is barrels")] 
    public Transform[] m_FireMuzzles;
    public AudioSource m_ShootingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
    [Tooltip("Audio for the shooting action")] public AudioClip m_FireClip;
    [Tooltip("Maximum Range (m)")]
    public float m_MaxRange = 10000f;
    [Tooltip("Minimum Range (m)")]
    public float m_MinRange = 1000f;
    [Tooltip("Muzzle velocity for the shell (m/s)")]
    public float m_MuzzleVelocity = 30f;        // It appears the muzzle velocity as implemented ingame is too fast, real time based on Iowa 16"/406mm gives a ratio of *0.58
    [Tooltip("Reload time (seconds)")]
    public float m_ReloadTime = 5f;
    [Tooltip("Dispersion of shells for this turret. 0.01 : the most precise / 2 : lots of dispersion")] [Range(0.01f, 2f)]
    public float m_Precision = 0.1f; 
    [Tooltip("Check this if the turret is a main turret (rangefinding is done with main turrets). You need to check only one turret par unit, but you can chack as many as you need as long as all Director turrets are of the same type.")]
    
    [Header("FX")]
    public GameObject m_FireFx;
    // private GameObject FireFxInstance;
    [Header("Debug")]
        public bool debug = false;
        
    private bool Reloading;
    private float ReloadingTimer;
    private TurretManager TurretManager;
    private int TurretNumber;
    private TurretRotation TurretRotation;
    private bool PreventFire;
    private bool OutOfRange;
    private float TargetRange;
    // private FreeLookCam FreeLookCam;
    private TurretManager.TurretStatusType TurretStatus;
    public enum TurretType {
        Artillery,
        ArtilleryAA,
        AA,
        Torpedo,
        DepthCharge
    }

    private bool AIControl = false;
    private bool AIPauseFire = false;

    private void Start (){
        TurretRotation = GetComponent<TurretRotation>();
        // FreeLookCam = GameObject.Find("FreeLookCameraRig").GetComponent<FreeLookCam>();
    }

    private void Update () {
        // if (debug) { Debug.Log("PreventFire = "+ PreventFire); Debug.Log("ReloadingTimer = "+ ReloadingTimer); }
        if (TargetRange > m_MaxRange && m_TurretType == TurretType.Artillery || TargetRange > m_MaxRange && m_TurretType == TurretType.ArtilleryAA && CurrentControlledTurretType == TurretType.Artillery) {
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
                Reloading = true;
                ReloadingTimer = m_ReloadTime;
                CheckTurretStatus();
                // ... launch the shell.
                Fire ();
            }
        } else if (AIControl && !Reloading && !PreventFire && !OutOfRange && !Dead) {
            CheckTurretStatus();
            if (!AIPauseFire) {
                //start the reloading process immediately
                Reloading = true;
                ReloadingTimer = m_ReloadTime;
                // ... launch the shell.
                Fire ();
                CheckTurretStatus();    
            }
        }

        if (Reloading && ReloadingTimer > 0) {
            ReloadingTimer-= Time.deltaTime;
            if (ReloadingTimer <= 0) {
                ReloadingTimer = 0;
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
        for (int i = 0; i < m_FireMuzzles.Length; i++) {
            //Build a lateral random factor for the shell launch
            float shellPrecisionZ = Random.Range(-m_Precision, m_Precision);
            Quaternion firingDirection = m_FireMuzzles[i].rotation;
            firingDirection.z += shellPrecisionZ * 0.02f;

            // if (debug) { Debug.Log("ShellPrecisionZ : "+ shellPrecisionZ); }
            // if (debug) { Debug.Log("m_FireMuzzles[i].rotation : "+ firingDirection); }

            // Create an instance of the shell and store a reference to it's rigidbody.
            // Rigidbody shellInstance =
            //     Instantiate (m_Shell, m_FireMuzzles[i].position, m_FireMuzzles[i].rotation) as Rigidbody;
            GameObject shellInstance =
                Instantiate (m_Shell, m_FireMuzzles[i].position, firingDirection);

            // Rigidbody rigid = shellInstance.GetComponent<Rigidbody> ();
            // Add velocity in the forward direction
            // DISABLED - shell ballistics moved in the ShellStat class.
            // rigid.velocity = m_MuzzleVelocity * m_FireMuzzles[i].forward;

            SendNeededInfoToShell(shellInstance);

            TurretManager.SendPlayerShellToUI(shellInstance);

            FireFX(m_FireMuzzles[i]);

            // AUDIO FX
            // Change the clip to the firing clip and play it.
            m_ShootingAudio.clip = m_FireClip;
            m_ShootingAudio.Play ();  
        }
    }
    private void SendNeededInfoToShell(GameObject shellInstance) {
        if (m_TurretType == TurretType.ArtilleryAA && CurrentControlledTurretType == TurretType.Artillery || m_TurretType == TurretType.Artillery) {
            shellInstance.GetComponent<ShellStat>().SetFiringMode(TurretType.Artillery);
            shellInstance.GetComponent<ShellStat> ().SetTargetRange(TargetRange);
        } else if (m_TurretType == TurretType.ArtilleryAA && CurrentControlledTurretType == TurretType.AA || m_TurretType == TurretType.AA) {
            shellInstance.GetComponent<ShellStat>().SetFiringMode(TurretType.AA);
            shellInstance.GetComponent<ShellStat> ().SetTargetRange(m_MaxRange);
        } else {
            shellInstance.GetComponent<ShellStat>().SetFiringMode(m_TurretType);
            shellInstance.GetComponent<ShellStat> ().SetTargetRange(m_MaxRange);
        }
        shellInstance.GetComponent<ShellStat> ().SetMuzzleVelocity(m_MuzzleVelocity * 0.58f);
        shellInstance.GetComponent<ShellStat> ().SetPrecision(m_Precision);
        shellInstance.GetComponent<ShellStat> ().SetParentTurretManager(TurretManager);
    }

    private void FireFX(Transform fireMuzzle) {
        // VISUAL FX
        if (m_FireFx == null) { return; }

        GameObject fireFxInstance = Instantiate(m_FireFx, fireMuzzle);

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
    public float GetMaxRange(){ return m_MaxRange; }
    public float GetMinRange(){ return m_MinRange; }
    public void SetTargetRange(float range){ TargetRange = range; }
    public void SetTurretDeath(bool IsShipDead) {
        Dead = IsShipDead;
        if (Dead)
            CheckTurretStatus();
    }

    private void CheckTurretStatus() {
        TurretManager.TurretStatusType statusType = TurretManager.TurretStatusType.Ready;
        if (UIActive) {
            if (Dead){
                statusType = TurretManager.TurretStatusType.Dead;
            } else if (Reloading){
                statusType = TurretManager.TurretStatusType.Reloading;
            } else if (PreventFire){
                statusType = TurretManager.TurretStatusType.PreventFire;
            } else if (OutOfRange && m_TurretType == TurretType.ArtilleryAA && CurrentControlledTurretType == TurretType.Artillery || OutOfRange && m_TurretType == TurretType.Artillery){
                statusType = TurretManager.TurretStatusType.PreventFire;
            } else {
                statusType = TurretManager.TurretStatusType.Ready;
            }

            if (statusType != TurretStatus) {
                // Debug.Log ("TurretNumber : "+ TurretNumber);
                TurretManager.SetSingleTurretStatus(statusType, TurretNumber);
            }
        }
        TurretStatus = statusType;
    }
    public TurretManager.TurretStatusType GetTurretStatus() {
        TurretManager.TurretStatusType status;
        if (Dead){
            status = TurretManager.TurretStatusType.Dead;
        } else if  (PreventFire){
            status = TurretManager.TurretStatusType.PreventFire;
        } else if  (OutOfRange && m_TurretType == TurretType.ArtilleryAA && CurrentControlledTurretType == TurretType.Artillery || OutOfRange && m_TurretType == TurretType.Artillery){
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
    private TurretType CurrentControlledTurretType;
    public void SetCurrentControlledTurretType(TurretType currentControlledTurretType) { CurrentControlledTurretType = currentControlledTurretType; }
    public TurretType GetTurretType() { return m_TurretType; }
}