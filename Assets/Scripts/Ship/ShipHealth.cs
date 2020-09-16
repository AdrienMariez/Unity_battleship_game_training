using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShipHealth : MonoBehaviour {
    private bool Dead = false;
    public float m_StartingHealth = 100f;               // The amount of health each tank starts with.
    private float CurrentHealth;


    public GameObject m_ExplosionPrefab;                // A prefab that will be instantiated in Awake, then used whenever the tank dies.

    private AudioSource m_ExplosionAudio;               // The audio source to play when the tank explodes.
    private ParticleSystem m_ExplosionParticles;        // The particle system the will play when the tank is destroyed.

    private ShipController ShipController;

    private int Fires = 0;
    private float FireDamage;
    private int UnsetCrew;
    private bool AutorepairPaused = false;


    private void Awake () {
        CurrentHealth = m_StartingHealth;
        ShipController = GetComponent<ShipController>();
        // Instantiate the explosion prefab and get a reference to the particle system on it.
        m_ExplosionParticles = Instantiate (m_ExplosionPrefab).GetComponent<ParticleSystem> ();

        // Get a reference to the audio source on the instantiated prefab.
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource> ();

        // Disable the prefab so it can be activated when it's required.
        m_ExplosionParticles.gameObject.SetActive (false);

        // Search "object pooling" to see how to set complex particles.
    }

    private void FixedUpdate(){
        if (Fires > 0) {
            Burning();
        } else if (!AutorepairPaused && CurrentHealth < m_StartingHealth && !Dead) {
            AutoRepair();
        }
    }

    IEnumerator PauseAutorepair(){
        // Coroutine created to prevent too much calculus for ship behaviour
        AutorepairPaused = true;
        yield return new WaitForSeconds(3);
        AutorepairPaused = false;
    }

    public void ApplyDamage (float damage) {
        CurrentHealth -= damage;
        CheckDeath ();
        ShipController.SetCurrentHealth(CurrentHealth);
        StartCoroutine(PauseAutorepair());
    }

    private void AutoRepair () {
        CurrentHealth += (UnsetCrew + 1 )* Time.deltaTime;
        // if (CurrentHealth > 0){
            // Debug.Log("damage = "+ damage);
            // Debug.Log("CurrentHealth = "+ CurrentHealth);
        // }
        ShipController.SetCurrentHealth(CurrentHealth);
    }

    private void CheckDeath () {
        // If the current health is at or below zero and it has not yet been registered, call OnDeath.
        if (CurrentHealth <= 0f && !Dead) {
            OnDeath ();
        }
    }

    private void OnDeath () {
        // Set the flag so that this function is only called once.
        Dead = true;

        // Move the instantiated explosion prefab to the tank's position and turn it on.
        m_ExplosionParticles.transform.position = transform.position;
        m_ExplosionParticles.gameObject.SetActive (true);

        // Play the particle system of the tank exploding.
        m_ExplosionParticles.Play ();

        // Play the tank explosion sound effect.
        m_ExplosionAudio.Play();

        // Turn the tank off.
        // gameObject.SetActive (false);

        // Debug.Log("A ship was destroyed due to health getting to 0");

        ShipController.CallDeath();
    }

    public void AmmoExplosion(){
        // Ammo explosion deals 15% damage flat for the time being
        ApplyDamage (m_StartingHealth * 0.15f);
    }

    public void StartFire() {
        Fires++;
        FireDamage = Fires * (m_StartingHealth * 0.01f) * Time.deltaTime;
    }
    public void EndFire() {
        Fires--;
        FireDamage = Fires * (m_StartingHealth * 0.01f) * Time.deltaTime;
        StartCoroutine(PauseAutorepair());
    }
    private void Burning(){ ApplyDamage (FireDamage); }
    
    public float GetCurrentHealth(){ return CurrentHealth; }
    public float GetStartingHealth(){ return m_StartingHealth; }
    public void SetDamageControlUnset(int setCrew){ UnsetCrew = setCrew; }
}