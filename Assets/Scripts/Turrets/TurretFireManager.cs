using UnityEngine;
using UnityEngine.UI;

public class TurretFireManager : MonoBehaviour
{
    // [Tooltip("Ammo used")] public Rigidbody m_Shell;
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
    public float m_MuzzleVelocity = 30f;
    [Tooltip("Reload time, (seconds)")]
    public float m_ReloadTime = 5f;

    [Header("Debug")]
        public bool debug = false;

    private string m_FireButton;
    [HideInInspector] public bool m_Reloading;
    private float m_ReloadingTimer;
    private TurretRotation TurretRotation;
    [HideInInspector] public bool PreventFire;
    [HideInInspector] public float CurrentAngleElevRatio;
    [HideInInspector] public bool m_Active;
    // private float ShellWeight;



    private void OnEnable()
    {
        // When the tank is turned on, reset the launch force and the UI
        // m_CurrentLaunchForce = m_MinLaunchForce;
        // m_AimSlider.value = m_MinLaunchForce;
    }


    private void Start ()
    {
        // ShellWeight = m_Shell.GetComponent<ShellStat>().m_Weight;
        TurretRotation = GetComponent<TurretRotation>();
    }


    private void Update () {
        // if (debug) { Debug.Log("PreventFire = "+ PreventFire); Debug.Log("m_ReloadingTimer = "+ m_ReloadingTimer); }
        if (PreventFire){
            GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
        }else if (m_Reloading){
            GetComponent<MeshRenderer>().material.SetColor("_Color", Color.yellow);
        }else{
            GetComponent<MeshRenderer>().material.SetColor("_Color", Color.green);
        }
        if (m_Active) {
            // Debug.Log("m_ReloadingTimer :"+ m_ReloadingTimer);
            PreviewFire ();

            if (Input.GetButtonDown ("FireMainWeapon") && !m_Reloading && !PreventFire) {
                //start the reloading process immediately
                m_Reloading = true;
                m_ReloadingTimer = m_ReloadTime;
                // ... launch the shell.
                Fire ();
            }
            else if (m_Reloading && m_ReloadingTimer > 0) {
                m_ReloadingTimer-= Time.deltaTime;
                if (m_ReloadingTimer <= 0) {
                    m_ReloadingTimer = 0;
                    m_Reloading = !m_Reloading;
                }
            }
        }
    }

    private void PreviewFire () {
        
    }

    private void Fire () {

        for (int i = 0; i < m_FireMuzzles.Length; i++) {
            // Create an instance of the shell and store a reference to it's rigidbody.
            // Rigidbody shellInstance =
            //     Instantiate (m_Shell, m_FireMuzzles[i].position, m_FireMuzzles[i].rotation) as Rigidbody;
            GameObject shellInstance =
                Instantiate (m_Shell, m_FireMuzzles[i].position, m_FireMuzzles[i].rotation);

            Rigidbody rigid = shellInstance.GetComponent<Rigidbody> ();
            rigid.velocity = m_MuzzleVelocity * m_FireMuzzles[i].forward;

            // ShellStat shellStats = shellInstance.GetComponent<ShellStat> ();
            // shellStats.m_MaxRange = m_MaxRange;
            shellInstance.GetComponent<ShellStat> ().m_MaxRange = m_MaxRange;
            shellInstance.GetComponent<ShellStat> ().m_MinRange = m_MinRange;
            shellInstance.GetComponent<ShellStat> ().m_MuzzleVelocity = m_MuzzleVelocity;
            shellInstance.GetComponent<ShellStat> ().AngleLaunchPercentage = TurretRotation.CurrentAnglePercentage;

            // Set the shell's velocity to the launch force in the fire position's forward direction.
            // shellInstance.velocity = m_MuzzleVelocity * m_FireMuzzles[i].forward;
            // shellInstance.velocity = ( (m_MuzzleVelocity * 100) / ShellWeight ) * m_FireMuzzles[i].forward;


            // Change the clip to the firing clip and play it.
            m_ShootingAudio.clip = m_FireClip;
            m_ShootingAudio.Play ();      
        }
    }
}