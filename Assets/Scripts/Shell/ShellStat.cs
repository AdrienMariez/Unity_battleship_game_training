using UnityEngine;

public class ShellStat : MonoBehaviour
{
    [Tooltip("What the shell can hit")]
    public LayerMask m_HitMask;                        // Used to filter what the explosion affects, this should be set to "Players".

    [Tooltip("Weight of the shell (kg)")]
    public float m_Weight = 100f;
    public ParticleSystem m_ExplosionParticles;         // Reference to the particles that will play on explosion.
    public AudioSource m_ExplosionAudio;                // Reference to the audio that will play on explosion.
    public float m_MaxDamage = 100f;                    // The amount of damage done if the explosion is centred on a tank.
    public float m_ExplosionForce = 1000f;              // The amount of force added to a tank at the centre of the explosion.
    public float m_MaxLifeTime = 2f;                    // The time in seconds before the shell is removed.
    public float m_ExplosionRadius = 5f;                // The maximum distance away from the explosion tanks can be and are still affected.

    private Rigidbody rb;
    [HideInInspector] public float m_MaxRange;
    [HideInInspector] public float m_MinRange;
    [HideInInspector] public float m_MuzzleVelocity;
    [HideInInspector] public float AngleLaunchPercentage;
    [HideInInspector] public Vector3 startPosition;

    private float currentRange;
    private float targetRange;
    private float currentAltitudeGain;
    private float xBasis;



    private void Start () {
        // If it isn't destroyed by then, destroy the shell after it's lifetime.
        Destroy (gameObject, m_MaxLifeTime);

        rb = GetComponent<Rigidbody>();
        currentAltitudeGain = rb.velocity.y;
        startPosition = transform.position;

        targetRange = ((m_MaxRange - m_MinRange) / 100 * AngleLaunchPercentage) + m_MinRange;

        xBasis = transform.eulerAngles.x;
        if (xBasis > 180)
            xBasis -= 360;
        xBasis += 180;
        xBasis = 360 - xBasis;
        xBasis -= 90;

        // Debug.Log("targetRange = "+ targetRange);
    }

    private void FixedUpdate () {
        currentRange = Vector3.Distance(startPosition, transform.position);

        float distanceToTarget = targetRange - currentRange;
        float distanceToTargetRatio = (distanceToTarget*100) / targetRange;

        // Debug.Log("distanceToTarget = "+ distanceToTarget);
        Debug.Log("distanceToTargetRatio = "+ distanceToTargetRatio);


        float x = transform.eulerAngles.x;
        float y = transform.eulerAngles.y;
        float z = transform.eulerAngles.z;

        if (distanceToTargetRatio>25) {

            x = (distanceToTargetRatio * xBasis) / 100; 

            if (x<0)
                x = 0;

            Debug.Log("x = "+ x);

            x += 90;
            x = 360 - x;
            x -= 180;
            if (x < 0)
                x += 360;
        }

            transform.localRotation = Quaternion.Euler (new Vector3 (x, y, z));
            transform.Translate(0, 0, m_MuzzleVelocity * Time.deltaTime, Space.Self); 
    }


    private void OnTriggerEnter (Collider other)
    {
        // Collect all the colliders in a sphere from the shell's current position to a radius of the explosion radius.
        Collider[] colliders = Physics.OverlapSphere (transform.position, m_ExplosionRadius, m_HitMask);

        // Go through all the colliders...
        for (int i = 0; i < colliders.Length; i++)
        {
            // ... and find their rigidbody.
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody> ();

            // If they don't have a rigidbody, go on to the next collider.
            if (!targetRigidbody)
                continue;

            // Add an explosion force.
            // This is used to move the target back away from the explosion
            //targetRigidbody.AddExplosionForce (m_ExplosionForce, transform.position, m_ExplosionRadius);

            // Find the TankHealth script associated with the rigidbody.
            TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth> ();

            // If there is no TankHealth script attached to the gameobject, go on to the next collider.
            if (!targetHealth)
                continue;

            // Calculate the amount of damage the target should take based on it's distance from the shell.
            float damage = CalculateDamage (targetRigidbody.position);

            // Deal this damage to the tank.
            targetHealth.TakeDamage (damage);
        }

        // Unparent the particles from the shell.
        m_ExplosionParticles.transform.parent = null;

        // Play the particle system.
        m_ExplosionParticles.Play();

        // Play the explosion sound effect.
        m_ExplosionAudio.Play();

        // Once the particles have finished, destroy the gameobject they are on.
        Destroy (m_ExplosionParticles.gameObject, m_ExplosionParticles.main.duration);
        
        //obsolete
        //Destroy (m_ExplosionParticles.gameObject, m_ExplosionParticles.duration);

        // Destroy the shell.
        Destroy (gameObject);
    }


    private float CalculateDamage (Vector3 targetPosition)
    {
        // Create a vector from the shell to the target.
        Vector3 explosionToTarget = targetPosition - transform.position;

        // Calculate the distance from the shell to the target.
        float explosionDistance = explosionToTarget.magnitude;

        // Calculate the proportion of the maximum distance (the explosionRadius) the target is away.
        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;

        // Calculate damage as this proportion of the maximum possible damage.
        float damage = relativeDistance * m_MaxDamage;

        // Make sure that the minimum damage is always 0. (prevent negative)
        damage = Mathf.Max (0f, damage);

        return damage;
    }
}