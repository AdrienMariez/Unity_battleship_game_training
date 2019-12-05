using UnityEngine;
using UnityEngine.UI;

public class ShipHealth : MonoBehaviour
{
    public float m_StartingHealth = 100f;               // The amount of health each tank starts with.
    public GameObject m_ExplosionPrefab;                // A prefab that will be instantiated in Awake, then used whenever the tank dies.


    private AudioSource m_ExplosionAudio;               // The audio source to play when the tank explodes.
    private ParticleSystem m_ExplosionParticles;        // The particle system the will play when the tank is destroyed.
    private float m_CurrentHealth;                      // How much health the tank currently has.
    private bool m_Dead;                                // Has the tank been reduced beyond zero health yet?


    private void Awake ()
    {
        // Instantiate the explosion prefab and get a reference to the particle system on it.
        m_ExplosionParticles = Instantiate (m_ExplosionPrefab).GetComponent<ParticleSystem> ();

        // Get a reference to the audio source on the instantiated prefab.
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource> ();

        // Disable the prefab so it can be activated when it's required.
        m_ExplosionParticles.gameObject.SetActive (false);

        // Search "object pooling" to see how to set complex particles.
    }


    private void OnEnable()
    {
        // When the tank is enabled, reset the tank's health and whether or not it's dead.
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;
    }


    public void TakeDamage (float amount)
    {
        // Reduce current health by the amount of damage done.
        m_CurrentHealth -= amount;

        CheckDeath ();
    }

    private void Update() {
        // Check if the ship isn't underwater
        // if (transform.localPosition.y < 0)
        // {
        //     m_CurrentHealth -= 1;

        //     SetHealthUI ();
        //     CheckDeath ();
        // }
        // Debug.Log("m_CurrentHealth = "+ m_CurrentHealth);
    }

    private void CheckDeath () {
        // If the current health is at or below zero and it has not yet been registered, call OnDeath.
        if (m_CurrentHealth <= 0f && !m_Dead)
        {
            OnDeath ();
        }
    }


    private void OnDeath () {
        // Set the flag so that this function is only called once.
        m_Dead = true;

        // Move the instantiated explosion prefab to the tank's position and turn it on.
        m_ExplosionParticles.transform.position = transform.position;
        m_ExplosionParticles.gameObject.SetActive (true);

        // Play the particle system of the tank exploding.
        m_ExplosionParticles.Play ();

        // Play the tank explosion sound effect.
        m_ExplosionAudio.Play();

        // Turn the tank off.
        gameObject.SetActive (false);
    }
}