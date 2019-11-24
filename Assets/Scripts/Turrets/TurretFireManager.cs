using UnityEngine;
using UnityEngine.UI;

public class TurretFireManager : MonoBehaviour
{
    [Tooltip("Ammo used")] public Rigidbody m_Shell;
    [Tooltip("Points where the shells will be spawned, make as many points as there is barrels")] 
    public Transform[] m_FireMuzzles;
    public AudioSource m_ShootingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
    [Tooltip("Audio for the shooting action")] public AudioClip m_FireClip;
    [Tooltip("Initial velocity for the shell")]
    public float m_LaunchVelocity = 30f;
    [Tooltip("The reload time for th gun, in seconds")]
    public float m_ReloadTime = 5f;

    [Header("Debug")]
        public bool debug = false;

    private string m_FireButton;
    [HideInInspector] public bool m_Reloading;
    private float m_ReloadingTimer;
    [HideInInspector] public bool PreventFire;
    [HideInInspector] public bool m_Active;


    private void OnEnable()
    {
        // When the tank is turned on, reset the launch force and the UI
        // m_CurrentLaunchForce = m_MinLaunchForce;
        // m_AimSlider.value = m_MinLaunchForce;
    }


    private void Start ()
    {
        // The fire axis is based on the player number.
        //m_FireButton = "Fire" + m_PlayerNumber;

        // The rate that the launch force charges up is the range of possible forces by the max charge time.
        // m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
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


    private void Fire () {
        for (int i = 0; i < m_FireMuzzles.Length; i++) {
            // Create an instance of the shell and store a reference to it's rigidbody.
            Rigidbody shellInstance =
                Instantiate (m_Shell, m_FireMuzzles[i].position, m_FireMuzzles[i].rotation) as Rigidbody;

            // Set the shell's velocity to the launch force in the fire position's forward direction.
            shellInstance.velocity = m_LaunchVelocity * m_FireMuzzles[i].forward; ;

            // Change the clip to the firing clip and play it.
            m_ShootingAudio.clip = m_FireClip;
            m_ShootingAudio.Play ();      
        }
    }
}