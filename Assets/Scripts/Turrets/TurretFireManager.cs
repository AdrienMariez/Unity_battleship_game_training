using UnityEngine;
// using UnityEngine.UI;
using System.Collections;
// using FreeLookCamera;

public class TurretFireManager : MonoBehaviour
{
    private bool Active = false;
    private bool PlayerControl = false;
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
    [Tooltip("Reload time, (seconds)")]
    public float m_ReloadTime = 5f;
    [Tooltip("Dispersion of shells for this turret. 0.01 : the most precise / 2 : lots of dispersion")] [Range(0.01f, 2f)]
    public float m_Precision = 0.1f; 
    [Tooltip("Check this if the turret is a main turret (rangefinding is done with main turrets). You need to check only one turret par unit, but you can chack as many as you need as long as all Director turrets are of the same type.")]
    public bool m_DirectorTurret = false;

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
        Torpedo
    }

    private bool AIControl = false;
    private bool AIPauseFire = false;

    private void Start (){
        TurretRotation = GetComponent<TurretRotation>();
        // FreeLookCam = GameObject.Find("FreeLookCameraRig").GetComponent<FreeLookCam>();
    }


    private void Update () {
        // if (debug) { Debug.Log("PreventFire = "+ PreventFire); Debug.Log("ReloadingTimer = "+ ReloadingTimer); }
        if (TargetRange > m_MaxRange) {
            OutOfRange = true;
            CheckTurretStatus(TurretManager.TurretStatusType.PreventFire);
        }else{
            OutOfRange = false;
        }
        if (PlayerControl && !Reloading && !PreventFire && !OutOfRange && !Dead) {
            CheckTurretStatus(TurretManager.TurretStatusType.Ready);
            if (Input.GetButtonDown ("FireMainWeapon")) {
                //start the reloading process immediately
                Reloading = true;
                ReloadingTimer = m_ReloadTime;
                CheckTurretStatus(TurretManager.TurretStatusType.Reloading);
                // ... launch the shell.
                Fire ();
            }
        } else if (AIControl && !Reloading && !PreventFire && !OutOfRange && !Dead && !AIPauseFire) {
            CheckTurretStatus(TurretManager.TurretStatusType.Ready);
            //start the reloading process immediately
            Reloading = true;
            ReloadingTimer = m_ReloadTime;
            // ... launch the shell.
            Fire ();
            CheckTurretStatus(TurretManager.TurretStatusType.Reloading);
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

            Rigidbody rigid = shellInstance.GetComponent<Rigidbody> ();
            // Add velocity in the forward direction
            // DISABLED - shell ballistics moved in the ShellStat class.
            // rigid.velocity = m_MuzzleVelocity * m_FireMuzzles[i].forward;


            shellInstance.GetComponent<ShellStat> ().SetTargetRange(TargetRange);
            shellInstance.GetComponent<ShellStat> ().SetMuzzleVelocity(m_MuzzleVelocity * 0.58f);
            shellInstance.GetComponent<ShellStat> ().SetPrecision(m_Precision);

            // Change the clip to the firing clip and play it.
            m_ShootingAudio.clip = m_FireClip;
            m_ShootingAudio.Play ();      
        }
    }

    public void SetPlayerControl(bool playerControl) { PlayerControl = playerControl; }
    public void SetActive(bool activate) {
        Active = activate;
    }
    public void SetAIControl(bool aiControl) { AIControl = aiControl; StartCoroutine(PauseFire()); }
    public void SetPreventFire(bool status){
        PreventFire = status;
        if (PreventFire)
            CheckTurretStatus(TurretManager.TurretStatusType.PreventFire);
    }
    public float GetMaxRange(){ return m_MaxRange; }
    public float GetMinRange(){ return m_MinRange; }
    public void SetTargetRange(float range){ TargetRange = range; }
    public void SetTurretDeath(bool IsShipDead) {
        Dead = IsShipDead;
        if (Dead)
            CheckTurretStatus(TurretManager.TurretStatusType.Dead);
    }

    private void CheckTurretStatus(TurretManager.TurretStatusType statusType) {
        if (Active) {
            if (Dead){
                statusType = TurretManager.TurretStatusType.Dead;
            }else if (Reloading){
                statusType = TurretManager.TurretStatusType.Reloading;
            }else if  (PreventFire || OutOfRange){
                statusType = TurretManager.TurretStatusType.PreventFire;
            }else{
                statusType = TurretManager.TurretStatusType.Ready;
            }
            if (statusType != TurretStatus) {
                TurretManager.SetSingleTurretStatus(statusType, TurretNumber);      
            }
        }
        TurretStatus = statusType;
    }
    public TurretManager.TurretStatusType GetTurretStatus() {
        TurretManager.TurretStatusType status;
        if (Dead){
            status = TurretManager.TurretStatusType.Dead;
        }else if  (PreventFire || OutOfRange){
            status = TurretManager.TurretStatusType.PreventFire;
        }else if (Reloading){
            status = TurretManager.TurretStatusType.Reloading;
        }else{
            status = TurretManager.TurretStatusType.Ready;
        }
        return status;
    }
    public void SetTurretNumber(int turretNumber){ TurretNumber = turretNumber; }
    public void SetTurretManager(TurretManager turretManager){ TurretManager = turretManager; }
}