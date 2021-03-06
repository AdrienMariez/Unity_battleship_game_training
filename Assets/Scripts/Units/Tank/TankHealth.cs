﻿using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    public float m_StartingHealth = 100f;               // The amount of health each tank starts with.
    public Slider m_Slider;                             // The slider to represent how much health the tank currently has.
    public Image m_FillImage;                           // The image component of the slider.
    public Color m_FullHealthColor = Color.green;       // The color the health bar will be when on full health.
    public Color m_ZeroHealthColor = Color.red;         // The color the health bar will be when on no health.
    public GameObject m_ExplosionPrefab;                // A prefab that will be instantiated in Awake, then used whenever the tank dies.


    private AudioSource ExplosionAudio;               // The audio source to play when the tank explodes.
    private ParticleSystem ExplosionParticles;        // The particle system the will play when the tank is destroyed.
    private float CurrentHealth;                      // How much health the tank currently has.
    private bool m_Dead;                                // Has the tank been reduced beyond zero health yet?


    private void Awake () {
        // Instantiate the explosion prefab and get a reference to the particle system on it.
        ExplosionParticles = Instantiate (m_ExplosionPrefab).GetComponent<ParticleSystem> ();

        // Get a reference to the audio source on the instantiated prefab.
        ExplosionAudio = ExplosionParticles.GetComponent<AudioSource> ();

        // Disable the prefab so it can be activated when it's required.
        ExplosionParticles.gameObject.SetActive (false);

        // Search "object pooling" to see how to set complex particles.
    }


    private void OnEnable()
    {
        // When the tank is enabled, reset the tank's health and whether or not it's dead.
        CurrentHealth = m_StartingHealth;
        m_Dead = false;

        // Update the health slider's value and color.
        SetHealthUI();
    }


    public void TakeDamage (float amount)
    {
        // Reduce current health by the amount of damage done.
        CurrentHealth -= amount;

        // Change the UI elements appropriately.
        SetHealthUI ();

        CheckDeath ();
    }

    private void Update()
    {
        // Check if the tank isn't underwater
        // if (transform.localPosition.y < 0)
        // {
        //     CurrentHealth -= 1;

        //     SetHealthUI ();
        //     CheckDeath ();
        // }
    }

    private void CheckDeath ()
    {
        // If the current health is at or below zero and it has not yet been registered, call OnDeath.
        if (CurrentHealth <= 0f && !m_Dead)
        {
            OnDeath ();
        }
    }


    private void SetHealthUI ()
    {
        // Set the slider's value appropriately.
        m_Slider.value = CurrentHealth;

        // Interpolate the color of the bar between the choosen colours based on the current percentage of the starting health.
        m_FillImage.color = Color.Lerp (m_ZeroHealthColor, m_FullHealthColor, CurrentHealth / m_StartingHealth);

        // "Lerp" : Linear interpolation
    }


    private void OnDeath ()
    {
        // Set the flag so that this function is only called once.
        m_Dead = true;

        // Move the instantiated explosion prefab to the tank's position and turn it on.
        ExplosionParticles.transform.position = transform.position;
        ExplosionParticles.gameObject.SetActive (true);

        // Play the particle system of the tank exploding.
        ExplosionParticles.Play ();

        // Play the tank explosion sound effect.
        ExplosionAudio.Play();

        // Turn the tank off.
        gameObject.SetActive (false);
    }

    public float GetCurrentHealth(){
        return CurrentHealth;
    }
    public float GetStartingHealth(){
        return m_StartingHealth;
    }
}