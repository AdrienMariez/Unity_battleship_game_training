using UnityEngine;

public class ShellStat : MonoBehaviour
{
    [Tooltip("What the shell can hit")]
    public LayerMask m_HitMask;                        // Used to filter what the explosion affects, this should be set to "Players".

    [Tooltip("Weight of the shell (kg)")]
    public float m_Weight = 100f;
    // public ParticleSystem m_ExplosionParticles;         // Reference to the particles that will play on explosion.
    // public AudioSource m_ExplosionAudio;                // Reference to the audio that will play on explosion.

    public GameObject m_Explosion;
    private GameObject ExplosionInstance;
    public GameObject m_ExplosionWater;
    private GameObject ExplosionWaterInstance;
    
    [SerializeField] private GameObject m_DamageEffect;
    private GameObject DamageEffectInstance;

    public float m_MaxDamage = 100f;                    // The amount of damage done if the explosion is centred on a tank.
    [Tooltip("Armor the shell can bypass (equivalent in rolled steel mm) If the shell's armor pen is less than the armor of the element hit, no damage will be applied.")]
    public float m_ArmorPenetration = 100f;
    public float m_MaxLifeTime = 2f;                    // The time in seconds before the shell is removed.
    public float m_ExplosionRadius = 5f;                // The maximum distance away from the explosion tanks can be and are still affected.

    private Rigidbody rb;
    private float MuzzleVelocity;
    private Vector3 StartPosition;

    private float currentRange;                     // Distance between the shell and the starting point
    private float targetRange;                      // Distance between the target point and the starting point. The target point is always at the same
    private float currentAltitudeGain;
    private float xBasis;
    private float x;
    private Vector3 V;
    private Vector3 TargetPosition;
    private bool SignedAnglePositive = true;
    private float ShellPrecision;
    private bool RangePassed = false;
    private bool SelfDestruct = false;

    private void Start () {
        // ExplosionWaterInstance = Instantiate(m_ExplosionWater, this.gameObject.transform);
        rb = GetComponent<Rigidbody>();
        // currentAltitudeGain = rb.velocity.y;
        StartPosition = transform.position;

        // Calculate target range by getting the percentage of vertical fire rotation of the turret
        // targetRange = ((MaxRange - m_MinRange) / 100 * AngleLaunchPercentage) + m_MinRange;

        xBasis = transform.eulerAngles.x;
        if (xBasis > 180)
            xBasis -= 360;
        xBasis += 180;
        xBasis = 360 - xBasis;
        xBasis -= 90;

        // Make a vector in the direction of the facing of the shell, flattened on the Y axis, if V.y was switched to 0.1f, it will target a high place target, so it could be used to fire on hih-placed land targets with some tweaks
        V = transform.TransformDirection(Vector3.forward);
        V.y = 0;
        V.Normalize();
        //Then create a point that is the target point of the cannon.
        TargetPosition = transform.position + V * targetRange;

        // get initial parameters of the shell, as using Vector3.SignedAngle is better than an Angle but can produce negative values. So we check first if the SignedAngle is + or -.

        // Create a vector between the current position and the target
        Vector3 targetDir = TargetPosition - transform.position;
        // Get the angle between the facing of the current position and the new vector
        float signedAngle = Vector3.SignedAngle(targetDir, transform.forward, Vector3.forward);

        if (signedAngle < 0)
            SignedAnglePositive = false;

        // Debug.Log("targetRange = "+ targetRange);
        // Debug.Log("TargetPosition = "+ TargetPosition);

        //Prebuild shell dispersion here
        ShellPrecision = Random.Range(-ShellPrecision, ShellPrecision);
    }

    private void FixedUpdate () {
        CalculateTrajectoryWithRange ();
        if (transform.position.y <= 0f) {
            ExplosionWaterInstance = Instantiate(m_ExplosionWater, this.gameObject.transform);
            // Unparent the particles from the shell.
            ExplosionWaterInstance.transform.parent = null;
            // Play the particle system.
            ExplosionWaterInstance.GetComponent<ParticleSystem>().Play();
            // Play the explosion sound effect.
            ExplosionWaterInstance.GetComponent<AudioSource>().Play();
            Destroy (ExplosionWaterInstance.gameObject, ExplosionWaterInstance.GetComponent<ParticleSystem>().main.startLifetime.constant);
            // Destroy the shell.
            Destroy (gameObject);
        }
    }

    private void CalculateTrajectoryWithRange () {
        // Create a vector between the current position and the target
        Vector3 targetDir = TargetPosition - transform.position;

        // Get the angle between the facing of the current position and the new vector
        float signedAngle = Vector3.SignedAngle(targetDir, transform.forward, Vector3.forward);
        // Debug.Log("signedAngle = "+ signedAngle);

        // Lots of shiny drawray !
            //Blue : from firing spawn to target position
            Debug.DrawRay(StartPosition, TargetPosition - StartPosition, Color.blue);
            // Red : facing of shell
            Debug.DrawRay(transform.position, transform.forward * targetRange, Color.red);
            // Green : The vector between the shell and the target
            // Debug.DrawRay(transform.position, targetDir * targetRange , Color.green);


        currentRange = Vector3.Distance(StartPosition, transform.position);

        float distanceToTarget = targetRange - currentRange;
        float distanceToTargetRatio = (distanceToTarget * 100) / targetRange;

        // Debug.Log("distanceToTarget = "+ distanceToTarget);
        // Debug.Log("distanceToTargetRatio = "+ distanceToTargetRatio);
        // Debug.Log("signedAngle = "+ signedAngle);



        //If the angle is not yet met and wasn't met...
        if (!RangePassed) {
            // Original calculation, worked but the shells didn't fly high enough
            // x = (distanceToTargetRatio * xBasis) / 100;

            // This CorrectedRatio (which should always be limited to 100) lets the shell fly high enough for all distance shells. after multiple tests, the simple multiplier makes a strange physics trajectory but works good ingame.  
            float CorrectedRatio = 1.4f * distanceToTargetRatio;
            if (CorrectedRatio > 100)
                CorrectedRatio = 100;

            x = (CorrectedRatio * xBasis) / 100;
    
            // This is not tested !!
            // Theorically, if the shell overshots the target, we force it to peak his nose a bit downwards so he can "catch" the correct angle
            if (distanceToTargetRatio < 20)
                x -= (20 - distanceToTargetRatio);

            // This prevents the shell from moving backwards
            if (x<0)
                x = 0;

            // Debug.Log("x = "+ x);

            // Convert X back into an euler angle
            x += 90;
            x = 360 - x;
            x -= 180;
            if (x < 0)
                x += 360;
        }
        // When the angle passes the ShellPrecision RNG, stop the gavity simulation
        if (SignedAnglePositive & signedAngle < ShellPrecision)
            RangePassed = true;
        if (!SignedAnglePositive & signedAngle > ShellPrecision)
            RangePassed = true;

        if (distanceToTargetRatio < 0 && !SelfDestruct) {
            // Engage auto destruct if the range is passed
            // Debug.Log("engage self destruct !");
            Destroy (gameObject, m_MaxLifeTime);
            SelfDestruct = true;
        }

        transform.localRotation = Quaternion.Euler (new Vector3 (x, transform.eulerAngles.y, transform.eulerAngles.z));
        transform.Translate(0, 0, MuzzleVelocity * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter (Collider other) {
        // Collect all the colliders in a sphere from the shell's current position to a radius of the explosion radius.
        Collider[] colliders = Physics.OverlapSphere (transform.position, m_ExplosionRadius, m_HitMask);

        // Debug.Log( "collide (name) : " + collide.collider.gameObject.name );
        // Debug.Log("Collider = "+ other);

        // Go through all the colliders...
        for (int i = 0; i < colliders.Length; i++) {
            HitboxComponent targetHitboxComponent = colliders[i].GetComponent<HitboxComponent> ();
            TurretHealth targetTurretHealth = colliders[i].GetComponent<TurretHealth> ();
            float damage;
            bool decalApplied = false;

            if (targetHitboxComponent != null) {
                if (m_ArmorPenetration < targetHitboxComponent.m_ElementArmor)
                    continue;

                // Calculate the amount of damage the target should take based on it's distance from the shell.
                damage = CalculateDamage (colliders[i].transform.position);

                // Deal this damage to the component.
                targetHitboxComponent.TakeDamage (damage);
                // Debug.Log("damage = "+ damage);

                if (!decalApplied) {
                    DamageEffectInstance = Instantiate(m_DamageEffect, this.gameObject.transform);
                    DamageEffectInstance.transform.parent = colliders[i].transform;
                    decalApplied = true;
                }
                
            }

            if (targetTurretHealth != null) {
                if (m_ArmorPenetration < targetTurretHealth.m_ElementArmor)
                    continue;

                // Calculate the amount of damage the target should take based on it's distance from the shell.
                damage = CalculateDamage (colliders[i].transform.position);

                // Deal this damage to the component.
                targetTurretHealth.TakeDamage (damage);

                // DamageEffectInstance = Instantiate(m_DamageEffect, this.gameObject.transform);
                // DamageEffectInstance.transform.parent = colliders[i].transform;
            }


        }
        ExplosionInstance = Instantiate(m_Explosion, this.gameObject.transform);
        ExplosionInstance.transform.parent = null;
        ExplosionInstance.GetComponent<ParticleSystem>().Play();
        ExplosionInstance.GetComponent<AudioSource>().Play();
        Destroy (ExplosionInstance.gameObject, ExplosionInstance.GetComponent<ParticleSystem>().main.duration);
        Destroy (gameObject);


        // DamageEffectInstance = Instantiate(m_DamageEffect, this.gameObject.transform);
        // DamageEffectInstance.transform.parent = null;
    }

    private float CalculateDamage (Vector3 targetPosition) {
        // Create a vector from the shell to the target.
        Vector3 explosionToTarget = targetPosition - transform.position;

        // Calculate the distance from the shell to the target.
        float explosionDistance = explosionToTarget.magnitude;

        // Calculate the proportion of the maximum distance (the explosionRadius) the target is away.
        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;

        // Debug.Log("m_ExplosionRadius = "+ m_ExplosionRadius + " - explosionDistance = "+ explosionDistance);
        // Debug.Log("explosionDistance = "+ explosionDistance);

        // Calculate damage as this proportion of the maximum possible damage.
        float damage = relativeDistance * m_MaxDamage;

        // Make sure that the minimum damage is always 0. (prevent negative)
        damage = Mathf.Max (0f, damage);

        return damage;
    }

    public void SetTargetRange(float range) {
        targetRange = range;
    }

    public void SetMuzzleVelocity(float velocity) {
        MuzzleVelocity = velocity;
    }
    public void SetPrecision(float precision) {
        ShellPrecision = precision;
    }
}