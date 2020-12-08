using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlaneShellStat : MonoBehaviour {
    private Rigidbody rb;
    private float MuzzleVelocity;
        public void SetMuzzleVelocity(float velocity) { MuzzleVelocity = velocity; }

    private Vector3 StartPosition;                  // Start position P1
    private Vector3 BezierCurvePeak;                // Curve peak P2
    private Vector3 TargetPosition;                 // Target position P3
        public void SetTargetPosition(Vector3 position) { TargetPosition = position; }
    private Vector3 CurrentPositionInCurve;         // Current position of the shell in the Bézier curve
    private Vector3 CurrentPositionFlat;            // A fake shell that goes directly from emitter to target and brings CurrentPositionInCurve with him.

    private float CurrentRangeRatio;                // Range established in proportion (0 = start 1 = target met) K

    private float CurrentRange;                     // Distance between the shell and the starting point
    private float TargetRange;                      // Distance between the target point and the starting point. The target point is always at the same
        public void SetTargetRange(float range) { TargetRange = range; }
    private Vector3 V;
    private float ShellPrecision;
        public void SetPrecision(float precision) { ShellPrecision = precision; }
    private bool WasAILaunched;                     // Was the ammo spent by an AI ?
        public void SetWasAILaunched(bool wasAILaunched) { WasAILaunched = wasAILaunched; }
    private bool SelfDestruct = false;

    // Data from DB
        private float Weight = 100f;
        private float MaxDamage = 100f;
        private float MinDamage = 10f;
        private float ArmorPenetration = 100f;
        private float ExplosionRadius = 5f;
        private float MaxLifeTime = 2f;
        private GameObject ExplosionFX; public void SetExplosionFX(GameObject _g){ ExplosionFX = _g; }
        private GameObject WaterHitFX; public void SetWaterHitFX(GameObject _g){ WaterHitFX = _g; }
        private GameObject DamageHoleFX; public void SetDamageFX(GameObject _g){ DamageHoleFX = _g; }
    // FX instances
    private GameObject ExplosionFXInstance;
    private GameObject WaterHitFXInstance;
    private GameObject DamageHoleFXInstance;

    private CompiledTypes.Weapons_roles.RowValues ShellType;
        public void SetFiringMode(CompiledTypes.Weapons_roles.RowValues shellType) { ShellType = shellType; }
    private PlaneWeaponsManager PlaneWeaponsManager;
        public void SetParentTurretManager(PlaneWeaponsManager _s) { PlaneWeaponsManager = _s; }

    public void BeginOperations(CompiledTypes.Ammos ammoDBRef) {
        Weight = ammoDBRef.Weight;
        MaxDamage = ammoDBRef.DamageMax;
        MinDamage = ammoDBRef.DamageMin;
        ArmorPenetration = ammoDBRef.ArmorPenetration;
        ExplosionRadius = ammoDBRef.ExplosionRadius;
        MaxLifeTime = ammoDBRef.MaxLifeTime;

        rb = GetComponent<Rigidbody>();

        float peakDistance = TargetRange/3;                                             // Used for Bézier
        BezierCurvePeak = transform.position + transform.TransformDirection(Vector3.forward) * peakDistance;             // Create Bezier curve peak

        ShellPrecision = Random.Range(-ShellPrecision, ShellPrecision);                 //Prebuild shell dispersion here

        StartPosition = transform.position;

        CurrentPositionFlat = transform.position;                                       // Initialize position

        if (ShellType == CompiledTypes.Weapons_roles.RowValues.NavalArtillery && !WasAILaunched) {

            V = transform.TransformDirection(Vector3.forward);                          // Make a vector in the direction of the facing of the shell
            V.y = 0;                                                                    // Flatten on the Y axis for naval artillery
            V.Normalize();
            TargetPosition = transform.position + V * TargetRange;                      // Then create a point that is the target point of the cannon.

            // Debug.Log("TargetRange = "+ TargetRange);
            // Debug.Log("ShellPrecision = "+ ShellPrecision);
        }
        // else if (ShellType == CompiledTypes.Weapons_roles.RowValues.NavalArtillery && WasAILaunched) {
        //     TargetPosition = TargetPosition;                      //
        // }
        if (ShellType == CompiledTypes.Weapons_roles.RowValues.AntiAir) {
            // TargetPosition = transform.position + V * TargetRange;                   // TargetRange for AA...This shouldn't be here.
        }
        if (ShellType == CompiledTypes.Weapons_roles.RowValues.Torpedo) {
            Destroy (gameObject, MaxLifeTime);                                        // Torpedoes auto die after their lifetime is expended
            StartCoroutine(PreventPrematureExplosion(1f));                               // Prevent torpedoes from exploding in their tubes at creation
        } else {
            StartCoroutine(PreventPrematureExplosion(0.1f));                             // Prevent any shell from exploding at creation (happens when firing at high speed)
        }

    }
    private bool PreventExplosion = true;
    IEnumerator PreventPrematureExplosion(float time){
        yield return new WaitForSeconds(time);
        PreventExplosion = false;
    }


    private void FixedUpdate () {
        if (ShellType == CompiledTypes.Weapons_roles.RowValues.Torpedo) {
            CalculateTrajectoryTorpedo();
        } else {
            CalculateTrajectoryArtillery();
            CheckIfShellNeedsToDieArtillery();
        }
    }

    public static Vector3 GetBezierPosition (Vector3 p0, Vector3 p1, Vector3 p2, float t) {
		return Vector3.Lerp(Vector3.Lerp(p0, p1, t), Vector3.Lerp(p1, p2, t), t);
	}

    private void CalculateTrajectoryArtillery () {
        float distanceToTarget = Vector3.Distance(TargetPosition, CurrentPositionFlat);                                         // Get remaining distance to travel

        CurrentRangeRatio = 1 - (((distanceToTarget * 100) / TargetRange) / 100);                                               // Used in Bezier to find position in the curve

        CurrentPositionInCurve = GetBezierPosition(StartPosition, BezierCurvePeak, TargetPosition, CurrentRangeRatio);          // Build Bézier curve current location

        // Lots of shiny drawray !
        // Yellow : from firing spawn to Bezier Curve Peak
            Debug.DrawRay(StartPosition, BezierCurvePeak - StartPosition, Color.yellow);
        // red : from start to Bezier Curve current position
            Debug.DrawRay(StartPosition, CurrentPositionInCurve - StartPosition , Color.red);
        // Green : The vector between the shell and the target
            Debug.DrawRay(transform.position, TargetPosition - transform.position , Color.green);
        // blue : The vector between the fake flat trajectory and the target
            Debug.DrawRay(CurrentPositionFlat, TargetPosition - CurrentPositionFlat , Color.blue);
        

        if (!SelfDestruct) {        // Normal behaviour until the target position is met
            transform.LookAt(CurrentPositionInCurve);
            
            transform.position = CurrentPositionInCurve;
            
            CurrentPositionFlat = Vector3.MoveTowards(CurrentPositionFlat, TargetPosition, MuzzleVelocity * Time.deltaTime);
        }

        if (CurrentRangeRatio >=1 && !SelfDestruct) {               // Engage auto destruct if the range is passed
            // Debug.Log("engage self destruct !");
            SelfDestruct = true;
            DestroyShell(MaxLifeTime);

            rb.velocity = transform.forward * MuzzleVelocity;       // Give a forward velocity to the shell for the last instants.
        }
    }
    private void CheckIfShellNeedsToDieArtillery() {
        if (ArmorPenetrated) {                                                                                  // if an armor was penetrates but the shell didn't explode yet
            CheckForExplosion();
        }
        if (transform.position.y <= 0f && !ArmorPenetrated) {                                                   // If water is hit
            //If there wasn't any penetration before, destroy the shell with a nice splash effect when the water is hit
            // Only if there was no penetration before or else there could be splashes inside ships and it would be silly
            WaterHitFX = Instantiate(WaterHitFX, this.gameObject.transform);
            WaterHitFX.transform.parent = null;                                                     // Unparent the particles from the shell.
            WaterHitFX.GetComponent<ParticleSystem>().Play();                                       // Play the particle system.
            WaterHitFX.GetComponent<AudioSource>().Play();                                          // Play the explosion sound effect.
            Destroy (WaterHitFX.gameObject, WaterHitFX.GetComponent<ParticleSystem>().main.startLifetime.constant);
            DestroyShell(0);                                                                                    // Destroy the shell.
        }
        if (Vector3.Distance(transform.position, CurrentPositionInCurve) > 0.01f ) {                            // If the shell is stationnary (This is here to resolve glitches)
            ShellExplosionFX();
            DestroyShell(0);
        }
    }

    private void CalculateTrajectoryAA () {
        //Blue : from firing spawn to target position
        // Debug.DrawRay(StartPosition, TargetPosition - StartPosition, Color.blue);
        // Red : facing of shell
        // Debug.DrawRay(transform.position, transform.forward * TargetRange, Color.red);

        transform.Translate(0, 0, MuzzleVelocity * Time.deltaTime, Space.Self);

        CurrentRange = Vector3.Distance(StartPosition, transform.position);
        float distanceToTarget = TargetRange - CurrentRange;
        float distanceToTargetRatio = (distanceToTarget * 100) / TargetRange;

        if (distanceToTargetRatio < 0 && !SelfDestruct) {
            // Engage auto destruct if the range is passed
            // Debug.Log("engage self destruct !");
            // Destroy (gameObject, MaxLifeTime);
            ShellExplosion();
            SelfDestruct = true;
        }
    }

    private void CalculateTrajectoryTorpedo () {
        transform.Translate(0, 0, MuzzleVelocity * Time.deltaTime, Space.Self);
    }

    private bool ArmorPenetrated = false;
    private float CollisionArmor;
    private float PenetrationRatio;
    private void OnTriggerEnter (Collider colliderHit) {
        if (!PreventExplosion) {
            OnTriggerEnterArtillery(colliderHit);
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
            if (ArmorPenetration < CollisionArmor && !ArmorPenetrated) {
                ShellExplosionFX();
                if (targetHitboxComponent != null) {
                    targetHitboxComponent.SendHitInfoToDamageControl(ArmorPenetrated);
                }
                if (PlaneWeaponsManager) // Prevents exception if the ship is removed from game before all shells hit
                    PlaneWeaponsManager.FeedbackShellHit(ArmorPenetrated);
                return;
            } else {
                // Calculate the ratio of penetration for use in CheckForExplosion
                // Minimum penetration ratio is 20 %
                PenetrationRatio = Mathf.Max( 20f, (100 - ( (CollisionArmor * 100) / ArmorPenetration)) );
                
                ApplyDecal(colliderHit);
                if (!ArmorPenetrated) {   
                    CheckForExplosion();
                    ArmorPenetrated = true;
                    if (targetHitboxComponent != null) {
                        targetHitboxComponent.SendHitInfoToDamageControl(ArmorPenetrated);
                    }
                    if (PlaneWeaponsManager)
                        PlaneWeaponsManager.FeedbackShellHit(ArmorPenetrated);
                }
            }
        }
    }
    private bool DecalApplied = false;
    private void ApplyDecal(Collider colliderHit) {
        if (!DecalApplied) {
            DamageHoleFXInstance = Instantiate(DamageHoleFX, this.gameObject.transform);
            DamageHoleFXInstance.transform.parent = colliderHit.transform;
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
        Collider[] colliders = Physics.OverlapSphere (transform.position, ExplosionRadius, WorldUnitsManager.GetHitMask());

        bool hullDamaged = false;

        // Go through all the colliders...
        foreach (var collider in colliders) {
            HitboxComponent targetHitboxComponent = collider.GetComponent<HitboxComponent> ();
            TurretHealth targetTurretHealth = collider.GetComponent<TurretHealth> ();
            float damage;

            if (targetHitboxComponent != null) {
                // This small check prevents any multiple damages on hull damage models with one shell
                if (targetHitboxComponent.GetElementType() == UnitMasterController.ElementType.hull) {
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
        if (ShellType == CompiledTypes.Weapons_roles.RowValues.Torpedo && !PreventExplosion) {
            ShellExplosionFXTorpedo();
        } else {
            ShellExplosionFXArtillery();
        }
    }
    private void ShellExplosionFXArtillery(){
        ExplosionFXInstance = Instantiate(ExplosionFX, this.gameObject.transform);
        ExplosionFXInstance.transform.parent = null;
        ExplosionFXInstance.GetComponent<ParticleSystem>().Play();
        ExplosionFXInstance.GetComponent<AudioSource>().Play();
        Destroy (ExplosionFXInstance.gameObject, ExplosionFXInstance.GetComponent<ParticleSystem>().main.duration);
        DestroyShell(0);
    }
    private void ShellExplosionFXTorpedo(){
        Vector3 position = this.gameObject.transform.position;
        Quaternion rotation = this.gameObject.transform.rotation;
        Quaternion updatedRotation = new Quaternion(90,-90,0,1);


        ExplosionFXInstance = Instantiate(ExplosionFX, position, updatedRotation);
        // ExplosionFXInstance.transform.parent = null;
        ExplosionFXInstance.GetComponent<ParticleSystem>().Play();
        ExplosionFXInstance.GetComponent<AudioSource>().Play();
        Destroy (ExplosionFXInstance.gameObject, ExplosionFXInstance.GetComponent<ParticleSystem>().main.duration);
        DestroyShell(0);
    }
    
    private float CalculateDamage (Collider colliderDamaged) {
        // Calculate the distance from the shell to the target collider bounds.
        Vector3 closestPoint = colliderDamaged.ClosestPointOnBounds(transform.position);
        float distance = Vector3.Distance(closestPoint, transform.position);
        // Debug.Log("distance = "+ distance);

        // Calculate the proportion of the maximum distance (ExplosionRadius) the target is away and calc damages.
        float relativeDistance = (ExplosionRadius - distance) / ExplosionRadius;
        
        float damage = Random.Range(MinDamage, MaxDamage);
        damage = relativeDistance * damage;

        // Make sure that the minimum damage is always 0. (prevent negative)
        damage = Mathf.Max (0f, damage);
        return damage;
    }

    private void DestroyShell(float time) {
        if (FollowedByCamera && PlayerManager != null && ShellCamera != null) {
            PlayerManager.ShellFollowedByCameraDestroyed();
            ShellCamera.transform.parent = null;
            Destroy (ShellCamera, DestroyTimer);
        }
        Destroy (gameObject, time);
    }

    // Camera following shell
    private bool FollowedByCamera = false;
    private PlayerManager PlayerManager;
    private GameObject ShellCamera;
    private float DestroyTimer;
    public void SetIsFollowedByCamera(PlayerManager playerManager, GameObject shellCamera, float destroyTimer) {
        FollowedByCamera = true;
        PlayerManager = playerManager;
        ShellCamera = shellCamera;
        DestroyTimer = destroyTimer;
    }
}