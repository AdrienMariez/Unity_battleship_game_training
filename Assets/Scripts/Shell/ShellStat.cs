using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShellStat : MonoBehaviour
{
    [Tooltip("What the shell can hit")]
    public LayerMask m_HitMask;                        // Used to filter what the explosion affects, this should be set to "Players".

    [Header("Shells Stats")]
    [Tooltip("Weight of the shell (kg)")]
    public float m_Weight = 100f;
    [Tooltip("Each time a shell explode, the maximum possible damage done will be between the max and the Min damage. Damage models far from the shell explosion will only receive a fraction of the maximum damage dealt.")]
    public float m_MaxDamage = 100f;                    // The maximum amount of damage done if the explosion is centred on a damage model.
    public float m_MinDamage = 10f;                    // The minimum amount of damage done if the explosion is centred on a damage model.
    [Tooltip("Armor the shell can bypass (equivalent in rolled steel mm) If the shell's armor pen is less than the armor of the element hit, no damage will be applied.")]
    public float m_ArmorPenetration = 100f;
    public float m_ExplosionRadius = 5f;                // The maximum distance away from the explosion models can be and are still affected.
    public float m_MaxLifeTime = 2f;                    // The time in seconds before the shell is removed.
    [Header("FX")]
    public GameObject m_Explosion;
    private GameObject ExplosionInstance;
    public GameObject m_ExplosionWater;
    private GameObject ExplosionWaterInstance;
    
    public GameObject m_DamageEffect;
    // [SerializeField] private GameObject m_DamageEffect;
    private GameObject DamageEffectInstance;


    private Rigidbody rb;
    private float MuzzleVelocity;
    private Vector3 StartPosition;

    private float currentRange;                     // Distance between the shell and the starting point
    private float TargetRange;                      // Distance between the target point and the starting point. The target point is always at the same
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
        if (ShellType == TurretFireManager.TurretType.Artillery) {
            // ExplosionWaterInstance = Instantiate(m_ExplosionWater, this.gameObject.transform);
            rb = GetComponent<Rigidbody>();
            // currentAltitudeGain = rb.velocity.y;
            StartPosition = transform.position;

            // Calculate target range by getting the percentage of vertical fire rotation of the turret
            // TargetRange = ((MaxRange - m_MinRange) / 100 * AngleLaunchPercentage) + m_MinRange;

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
            TargetPosition = transform.position + V * TargetRange;

            // get initial parameters of the shell, as using Vector3.SignedAngle is better than an Angle but can produce negative values. So we check first if the SignedAngle is + or -.

            // Create a vector between the current position and the target
            Vector3 targetDir = TargetPosition - transform.position;
            // Get the angle between the facing of the current position and the new vector
            float signedAngle = Vector3.SignedAngle(targetDir, transform.forward, Vector3.forward);

            if (signedAngle < 0)
                SignedAnglePositive = false;

            // Debug.Log("TargetRange = "+ TargetRange);
            // Debug.Log("TargetPosition = "+ TargetPosition);

            //Prebuild shell dispersion here
            ShellPrecision = Random.Range(-ShellPrecision, ShellPrecision);

        } else if (ShellType == TurretFireManager.TurretType.Torpedo) {
            // Torpedoes auto die after their lifetime is expended
            Destroy (gameObject, m_MaxLifeTime);
            // Prevent torpedoes from exploding in their tubes at creation
            StartCoroutine(PreventPrematureExplosion());
        }
    }
    private bool PreventExplosion = true;
    IEnumerator PreventPrematureExplosion(){
        yield return new WaitForSeconds(2);
        PreventExplosion = false;
    }

    private void FixedUpdate () {
        if (ShellType == TurretFireManager.TurretType.Artillery) {
            CalculateTrajectoryArtillery ();
            CheckIfShellNeedsToDieArtillery();
        } else if (ShellType == TurretFireManager.TurretType.Torpedo) {
            CalculateTrajectoryTorpedo ();
        }
    }

    private void CalculateTrajectoryArtillery () {
        // Create a vector between the current position and the target
        Vector3 targetDir = TargetPosition - transform.position;

        // Get the angle between the facing of the current position and the new vector
        float signedAngle = Vector3.SignedAngle(targetDir, transform.forward, Vector3.forward);
        // Debug.Log("signedAngle = "+ signedAngle);

        // Lots of shiny drawray !
            //Blue : from firing spawn to target position
            Debug.DrawRay(StartPosition, TargetPosition - StartPosition, Color.blue);
            // Red : facing of shell
            Debug.DrawRay(transform.position, transform.forward * TargetRange, Color.red);
            // Green : The vector between the shell and the target
            // Debug.DrawRay(transform.position, targetDir * TargetRange , Color.green);


        currentRange = Vector3.Distance(StartPosition, transform.position);

        float distanceToTarget = TargetRange - currentRange;
        float distanceToTargetRatio = (distanceToTarget * 100) / TargetRange;

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
    private void CheckIfShellNeedsToDieArtillery() {
        if (ArmorPenetrated) {
            CheckForExplosion();
        }
        if (transform.position.y <= 0f && !ArmorPenetrated) {
            //If there wasn't any penetration before, destroy the shell with a nice splash effect when the water is hit
            // Only if there was no penetration before or else there could be splashes inside ships and it would be silly
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

    private void CalculateTrajectoryTorpedo () {
        transform.Translate(0, 0, MuzzleVelocity * Time.deltaTime, Space.Self);
    }

    private bool ArmorPenetrated = false;
    private float CollisionArmor;
    private float PenetrationRatio;
    private void OnTriggerEnter (Collider colliderHit) {
        if (ShellType == TurretFireManager.TurretType.Artillery) {
            OnTriggerEnterArtillery(colliderHit);
        } else if (ShellType == TurretFireManager.TurretType.Torpedo && !PreventExplosion) {
            OnTriggerEnterTorpedo(colliderHit);
        }
    }
    private void OnTriggerEnterArtillery(Collider colliderHit) {
        HitboxComponent targetHitboxComponent = colliderHit.GetComponent<HitboxComponent> ();
        TurretHealth targetTurretHealth = colliderHit.GetComponent<TurretHealth> ();
        bool suitableTarget = false;
        if (targetHitboxComponent != null) {
            if (!targetHitboxComponent.GetBuoyancyComponent())
                CollisionArmor = targetHitboxComponent.GetElementArmor();
                suitableTarget = true;
        } else if (targetTurretHealth != null) {
            CollisionArmor = targetTurretHealth.GetElementArmor();
            suitableTarget = true;
        }
        if (suitableTarget) {
            if (m_ArmorPenetration < CollisionArmor && !ArmorPenetrated) {
                ShellExplosionFX();
                if (targetHitboxComponent != null) {
                    targetHitboxComponent.SendHitInfoToDamageControl(ArmorPenetrated);
                }
                TurretManager.FeedbackShellHit(ArmorPenetrated);
                return;
            } else {
                // Calculate the ratio of penetration for use in CheckForExplosion
                PenetrationRatio = 100 - ( (CollisionArmor * 100) / m_ArmorPenetration);
                // Minimum penetration ratio is 20 %
                PenetrationRatio = Mathf.Max(20f, PenetrationRatio);
                ApplyDecal(colliderHit);
                if (!ArmorPenetrated) {   
                    CheckForExplosion();
                    ArmorPenetrated = true;
                    if (targetHitboxComponent != null) {
                        targetHitboxComponent.SendHitInfoToDamageControl(ArmorPenetrated);
                    }
                    TurretManager.FeedbackShellHit(ArmorPenetrated);
                }
            }
        }
    }
    private void OnTriggerEnterTorpedo(Collider colliderHit) {
        HitboxComponent targetHitboxComponent = colliderHit.GetComponent<HitboxComponent> ();
        TurretHealth targetTurretHealth = colliderHit.GetComponent<TurretHealth> ();
        bool suitableTarget = false;
        if (targetHitboxComponent != null) {
            if (!targetHitboxComponent.GetBuoyancyComponent())
                CollisionArmor = targetHitboxComponent.GetElementArmor();
                suitableTarget = true;
        } else if (targetTurretHealth != null) {
            CollisionArmor = targetTurretHealth.GetElementArmor();
            suitableTarget = true;
        }
        if (suitableTarget) {
            if (m_ArmorPenetration < CollisionArmor && !ArmorPenetrated) {
                ShellExplosionFX();
                if (targetHitboxComponent != null) {
                    targetHitboxComponent.SendHitInfoToDamageControl(ArmorPenetrated);
                }
                TurretManager.FeedbackShellHit(ArmorPenetrated);
                return;
            } else {
                // Calculate the ratio of penetration for use in CheckForExplosion
                PenetrationRatio = 100 - ( (CollisionArmor * 100) / m_ArmorPenetration);
                // Minimum penetration ratio is 20 %
                PenetrationRatio = Mathf.Max(20f, PenetrationRatio);
                ApplyDecal(colliderHit);
                if (!ArmorPenetrated) {   
                    CheckForExplosion();
                    ArmorPenetrated = true;
                    if (targetHitboxComponent != null) {
                        targetHitboxComponent.SendHitInfoToDamageControl(ArmorPenetrated);
                    }
                    TurretManager.FeedbackShellHit(ArmorPenetrated);
                }
            }
        }
    }
    private bool DecalApplied = false;
    private void ApplyDecal(Collider colliderHit) {
        if (!DecalApplied) {
            DamageEffectInstance = Instantiate(m_DamageEffect, this.gameObject.transform);
            DamageEffectInstance.transform.parent = colliderHit.transform;
            DecalApplied = true;
        }
    }
    private void CheckForExplosion() {
        /*
            Each frame after penetration of an armor, we make a random check with the basis of the penetration proportion.
            If a shell penetration is only 20% over the armor, it has 80% chance to explode at each frame.
            But if a shell overpenetrates an armor, it will have a lot of chance to overpenetrate
        */
        float ChanceToExplodeAfterPenetration = Random.Range(0, 100);
        if (ChanceToExplodeAfterPenetration < PenetrationRatio) {
            ShellExplosion();
        } else {
            //reduce shell speed when it has entered a body
                SetMuzzleVelocity(0.5f*MuzzleVelocity);
        }
    }
    private void ShellExplosion() {
        // Collect all the colliders in a sphere from the shell's current position to a radius of the explosion radius.
        Collider[] colliders = Physics.OverlapSphere (transform.position, m_ExplosionRadius, m_HitMask);

        bool hullDamaged = false;

        // Go through all the colliders...
        foreach (var collider in colliders) {
            HitboxComponent targetHitboxComponent = collider.GetComponent<HitboxComponent> ();
            TurretHealth targetTurretHealth = collider.GetComponent<TurretHealth> ();
            float damage;

            if (targetHitboxComponent != null) {
                // This small check prevents any multiple damages on hull damage models with one shell
                if (targetHitboxComponent.GetElementType() == ShipController.ElementType.hull) {
                    if (hullDamaged) {
                        continue;
                    } else {
                       hullDamaged = true; 
                    }
                }
                // Calculate the amount of damage the target should take based on its distance from the shell.
                damage = CalculateDamage (collider);
                // damage = CalculateDamage (collider.transform.position);
                // Deal this damage to the component.
                targetHitboxComponent.TakeDamage (damage);
                // Debug.Log("damage = "+ damage);
            }

            if (targetTurretHealth != null) {
                // Calculate the amount of damage the target should take based on it's distance from the shell.
                damage = CalculateDamage (collider);

                // Deal this damage to the component.
                targetTurretHealth.TakeDamage (damage);
            }


        }
        ShellExplosionFX();
    }
    private void ShellExplosionFX(){
        if (ShellType == TurretFireManager.TurretType.Artillery) {
            ShellExplosionFXArtillery();
        } else if (ShellType == TurretFireManager.TurretType.Torpedo && !PreventExplosion) {
            ShellExplosionFXTorpedo();
        }
    }
    private void ShellExplosionFXArtillery(){
        ExplosionInstance = Instantiate(m_Explosion, this.gameObject.transform);
        ExplosionInstance.transform.parent = null;
        ExplosionInstance.GetComponent<ParticleSystem>().Play();
        ExplosionInstance.GetComponent<AudioSource>().Play();
        Destroy (ExplosionInstance.gameObject, ExplosionInstance.GetComponent<ParticleSystem>().main.duration);
        Destroy (gameObject);
    }
    private void ShellExplosionFXTorpedo(){
        Vector3 position = this.gameObject.transform.position;
        Quaternion rotation = this.gameObject.transform.rotation;
        Quaternion updatedRotation = new Quaternion(90,-90,0,1);


        ExplosionInstance = Instantiate(m_Explosion, position, updatedRotation);
        // ExplosionInstance.transform.parent = null;
        ExplosionInstance.GetComponent<ParticleSystem>().Play();
        ExplosionInstance.GetComponent<AudioSource>().Play();
        Destroy (ExplosionInstance.gameObject, ExplosionInstance.GetComponent<ParticleSystem>().main.duration);
        Destroy (gameObject);
    }
    
    private float CalculateDamage (Collider colliderDamaged) {
        // Calculate the distance from the shell to the target collider bounds.
        Vector3 closestPoint = colliderDamaged.ClosestPointOnBounds(transform.position);
        float distance = Vector3.Distance(closestPoint, transform.position);
        // Debug.Log("distance = "+ distance);

        // Calculate the proportion of the maximum distance (m_ExplosionRadius) the target is away and calc damages.
        float relativeDistance = (m_ExplosionRadius - distance) / m_ExplosionRadius;
        
        float damage = Random.Range(m_MinDamage, m_MaxDamage);
        damage = relativeDistance * damage;

        // Make sure that the minimum damage is always 0. (prevent negative)
        damage = Mathf.Max (0f, damage);
        return damage;
    }

    public void SetTargetRange(float range) { TargetRange = range; }
    public void SetMuzzleVelocity(float velocity) { MuzzleVelocity = velocity; }
    public void SetPrecision(float precision) { ShellPrecision = precision; }
    private TurretFireManager.TurretType ShellType;
    public void SetFiringMode(TurretFireManager.TurretType shellType) { ShellType = shellType; }
    private TurretManager TurretManager;
    public void SetParentTurretManager(TurretManager turretManager) {
        TurretManager = turretManager;
    }
}