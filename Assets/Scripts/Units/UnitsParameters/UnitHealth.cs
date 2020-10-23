using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnitHealth : UnitParameter {
    protected float StartingHealth = 100f;                  // The amount of health each tank starts with.
    protected GameObject ExplosionPrefab;                    // Search "object pooling" to see how to set complex particles.

    protected bool Dead = false;
    protected float CurrentHealth;


    protected AudioSource ExplosionAudio;                 // The audio source to play when the unit dies.
    protected ParticleSystem ExplosionParticles;          // The particle system the will play when the tank is destroyed.

    protected UnitMasterController UnitController;

    protected int Fires = 0;
    protected float FireDamage;
    protected int UnsetCrew;
    protected bool AutorepairPaused = false;


    public void BeginOperations(UnitMasterController unitController) {
        CurrentHealth = StartingHealth;
        UnitController = unitController;
    }

    private void FixedUpdate(){
        if (Fires > 0) {
            Burning();
        } else if (!AutorepairPaused && CurrentHealth < StartingHealth && !Dead) {
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
        UnitController.SetCurrentHealth(CurrentHealth);
        StartCoroutine(PauseAutorepair());
    }

    private void AutoRepair () {
        CurrentHealth += (UnsetCrew + 1 )* Time.deltaTime;
        // if (CurrentHealth > 0){
            // Debug.Log("damage = "+ damage);
            // Debug.Log("CurrentHealth = "+ CurrentHealth);
        // }
        UnitController.SetCurrentHealth(CurrentHealth);
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

        // Instantiate the explosion prefab and get a reference to the particle system on it.
        ExplosionParticles = Instantiate (ExplosionPrefab).GetComponent<ParticleSystem> ();

        // Get a reference to the audio source on the instantiated prefab.
        ExplosionAudio = ExplosionParticles.GetComponent<AudioSource> ();

        // Disable the prefab so it can be activated when it's required.
        ExplosionParticles.gameObject.SetActive (false);

        // Move the instantiated explosion prefab to the unit position and turn it on.
        ExplosionParticles.transform.position = transform.position;
        ExplosionParticles.gameObject.SetActive (true);

        // Play the particle system of the tank exploding.
        ExplosionParticles.Play ();

        // Play the tank explosion sound effect.
        ExplosionAudio.Play();

        // Debug.Log("A ship was destroyed due to health getting to 0");

        UnitController.CallDeath();
    }

    public void AmmoExplosion(){
        // Ammo explosion deals 15% damage flat for the time being
        ApplyDamage (StartingHealth * 0.15f);
    }

    public void StartFire() {
        Fires++;
        FireDamage = Fires * (StartingHealth * 0.01f) * Time.deltaTime;
    }
    public void EndFire() {
        Fires--;
        FireDamage = Fires * (StartingHealth * 0.01f) * Time.deltaTime;
        StartCoroutine(PauseAutorepair());
    }
    private void Burning(){ ApplyDamage (FireDamage); }
    
    public float GetCurrentHealth(){ return CurrentHealth; }
    public float GetStartingHealth(){ return StartingHealth; }
    public void SetCurrentHealth(float currentHealth){ CurrentHealth = currentHealth; }
    public void SetStartingHealth(float startingHealth){ StartingHealth = startingHealth; }
    public void SetDeathFX(GameObject deathFX){ ExplosionPrefab = deathFX; }
    public void SetDamageControlUnset(int setCrew){ UnsetCrew = setCrew; }
}