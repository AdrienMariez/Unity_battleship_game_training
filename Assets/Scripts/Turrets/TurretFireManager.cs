using UnityEngine;
using UnityEngine.UI;

public class TurretFireManager : MonoBehaviour
{
    [HideInInspector] public bool m_Active;
    [HideInInspector] public bool m_Dead;
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
    [Tooltip("Check this if the turret is a main turret (rangefinding is done with main turrets). You need to check only one turret par unit, but you can chack as many as you need as long as all Director turrets are of the same type.")]
    public bool m_DirectorTurret = false;

    [Header("Debug")]
        public bool debug = false;
        
    [HideInInspector] public bool m_Reloading;
    private float m_ReloadingTimer;
    private TurretRotation TurretRotation;
    [HideInInspector] public bool PreventFire;
    private bool OutOfRange;
    [HideInInspector] public float CurrentAngleElevRatio;
    [HideInInspector] public float targetRange;
    // private float ShellWeight;



    private void OnEnable()
    {
        // When the tank is turned on, reset the launch force and the UI
        // m_CurrentLaunchForce = m_MinLaunchForce;
        // m_AimSlider.value = m_MinLaunchForce;
    }


    private void Start (){
        TurretRotation = GetComponent<TurretRotation>();
    }


    private void Update () {
        // if (debug) { Debug.Log("PreventFire = "+ PreventFire); Debug.Log("m_ReloadingTimer = "+ m_ReloadingTimer); }
        if (targetRange > m_MaxRange) {
            OutOfRange = true;
        }else{
            OutOfRange = false;
        }
        if (m_Dead){
            GetComponent<MeshRenderer>().material.SetColor("_Color", Color.black);
        }else if  (PreventFire || OutOfRange){
            GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
        }else if (m_Reloading){
            GetComponent<MeshRenderer>().material.SetColor("_Color", Color.yellow);
        }else{
            GetComponent<MeshRenderer>().material.SetColor("_Color", Color.green);
        }
        if (m_Active && !m_Dead) {
            // Debug.Log("m_Reloading :"+ m_Reloading);
            // Debug.Log("m_ReloadingTimer :"+ m_ReloadingTimer);
            if (m_DirectorTurret)
                PreviewFire ();

            // Debug.Log("Calculated fire range : "+ targetRange);

            if (Input.GetButtonDown ("FireMainWeapon") && !m_Reloading && !PreventFire && !OutOfRange) {
                //start the reloading process immediately
                m_Reloading = true;
                m_ReloadingTimer = m_ReloadTime;
                // ... launch the shell.
                Fire ();
            }
            if (m_Reloading && m_ReloadingTimer > 0) {
                m_ReloadingTimer-= Time.deltaTime;
                if (m_ReloadingTimer <= 0) {
                    m_ReloadingTimer = 0;
                    m_Reloading = !m_Reloading;
                }
                // Debug.Log("m_ReloadingTimer :"+ m_ReloadingTimer);
            }
        }
    }

    private void PreviewFire () {
        // For gameplay reasons, we cheat the physics here. The director(s) turrets will send their telemetric data to all other turrets
        targetRange = ((m_MaxRange - m_MinRange) / 100 * TurretRotation.CurrentAnglePercentage) + m_MinRange;
        // Debug.Log("Calculated fire range : "+ targetRange);
    }

    private void Fire () {
        for (int i = 0; i < m_FireMuzzles.Length; i++) {
            // Create an instance of the shell and store a reference to it's rigidbody.
            // Rigidbody shellInstance =
            //     Instantiate (m_Shell, m_FireMuzzles[i].position, m_FireMuzzles[i].rotation) as Rigidbody;
            GameObject shellInstance =
                Instantiate (m_Shell, m_FireMuzzles[i].position, m_FireMuzzles[i].rotation);

            Rigidbody rigid = shellInstance.GetComponent<Rigidbody> ();
            // Add velocity in the forward direction
            // DISABLED - shell ballistics moved in the ShellStat class.
            // rigid.velocity = m_MuzzleVelocity * m_FireMuzzles[i].forward;

            // ShellStat shellStats = shellInstance.GetComponent<ShellStat> ();
            // shellStats.m_MaxRange = m_MaxRange;
            shellInstance.GetComponent<ShellStat> ().m_MaxRange = m_MaxRange;
            shellInstance.GetComponent<ShellStat> ().m_MinRange = m_MinRange;
            shellInstance.GetComponent<ShellStat> ().targetRange = targetRange;
            shellInstance.GetComponent<ShellStat> ().m_MuzzleVelocity = m_MuzzleVelocity * 0.58f;
            shellInstance.GetComponent<ShellStat> ().AngleLaunchPercentage = TurretRotation.CurrentAnglePercentage;

            // Change the clip to the firing clip and play it.
            m_ShootingAudio.clip = m_FireClip;
            m_ShootingAudio.Play ();      
        }
    }
}