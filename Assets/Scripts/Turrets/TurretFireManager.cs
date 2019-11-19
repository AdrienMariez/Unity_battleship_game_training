using UnityEngine;
using UnityEngine.UI;

public class TurretFireManager : MonoBehaviour
{
    [Tooltip("Ammo used")]
    public Rigidbody m_Shell;                   // Prefab of the shell.
    [Tooltip("Points where the shells will be spawned, make as many points as there is barrels")] 
    public Transform[] m_FireMuzzles;      // Will be used for multiple cannons
    public AudioSource m_ShootingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
    [Tooltip("Audio for the shooting action")]
    public AudioClip m_FireClip;                // Audio that plays when each shot is fired.
    [Tooltip("Initial velocity for the shell")]
    public float m_LaunchVelocity = 30f;
    [Tooltip("The reload time for th gun, in seconds")]
    public float m_ReloadTime = 5f;


    private string m_FireButton;                // The input axis that is used for launching shells.
    private bool m_Reloading;
    private float m_ReloadingTimer;

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
        // Debug.Log("m_Active :"+ m_Active);
        if (m_Active) {
            // Debug.Log("m_ReloadingTimer :"+ m_ReloadingTimer);
            if (Input.GetButtonDown ("FireMainGround") && !m_Reloading)
            {
                //start the reloading process immediately
                m_Reloading = true;
                m_ReloadingTimer = m_ReloadTime;
                // ... launch the shell.
                Fire ();
            }
            else if (m_Reloading && m_ReloadingTimer > 0)
            {
                m_ReloadingTimer-= Time.deltaTime;
                if (m_ReloadingTimer <= 0)
                {
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