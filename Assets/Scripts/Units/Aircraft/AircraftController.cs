using UnityEngine;
using System.Collections;

    //[RequireComponent(typeof (Rigidbody))]
    public class AircraftController : UnitMasterController {

        [Header("Aircraft units elements : ")]
        private AircraftMovement Movement;
        private AircraftLandingGear LandingGear;
        [SerializeField] private float m_MaxEnginePower = 15;        // The maximum output of the engine.
        [SerializeField] private float m_Lift = 0.015f;               // The amount of lift generated by the aeroplane moving forwards.
        [SerializeField] private float m_ZeroLiftSpeed = 70;         // The speed at which lift is no longer applied.
        [SerializeField] private float m_RollEffect = 1f;             // The strength of effect for roll input.
        [SerializeField] private float m_PitchEffect = 1f;            // The strength of effect for pitch input.
        [SerializeField] private float m_YawEffect = 0.5f;            // The strength of effect for yaw input.
        [SerializeField] private float m_BankedTurnEffect = 0.5f;     // The amount of turn from doing a banked turn.
        [SerializeField] private float m_AerodynamicEffect = 0.03f;   // How much aerodynamics affect the speed of the aeroplane.
        [SerializeField] private float m_AutoTurnPitch = 0.5f;        // How much the aeroplane automatically pitches when in a banked turn.
        [SerializeField] private float m_AutoRollLevel = 0.1f;        // How much the aeroplane tries to level when not rolling.
        [SerializeField] private float m_AutoPitchLevel = 0.1f;       // How much the aeroplane tries to level when not pitching.
        [SerializeField] private float m_AirBrakesEffect = 3f;        // How much the air brakes effect the drag.
        [SerializeField] private float m_ThrottleChangeSpeed = 0.3f;  // The speed with which the throttle changes.
        [SerializeField] private float m_DragIncreaseFactor = 0.001f; // how much drag should increase with speed.

        public float m_TimeBeforePlayerControl{ get; private set; }     // Time to wait after takeoff for the unit to be fully added to the player units.

        public float Altitude { get; private set; }                     // The aeroplane's height above the ground.
        public float Throttle { get; private set; }                     // The amount of throttle being used.
        public bool AirBrakes { get; private set; }                     // Whether or not the air brakes are being applied.
        public float ForwardSpeed { get; private set; }                 // How fast the aeroplane is traveling in it's forward direction.
        public float EnginePower { get; private set; }                  // How much power the engine is being given.
        public float MaxEnginePower{ get { return m_MaxEnginePower; }}    // The maximum output of the engine.
        public float RollAngle { get; private set; }
        public float RollAngle2 { get; set; }
        public float PitchAngle { get; private set; }
        public float RollInput { get; private set; }
        public float PitchInput { get; private set; }
        public float YawInput { get; private set; }
        public float ThrottleInput { get; private set; }

        private float m_OriginalDrag;         // The drag when the scene starts.
        private float m_OriginalAngularDrag;  // The angular drag when the scene starts.
        private float m_AeroFactor;
        private bool m_Immobilized = false;   // used for making the plane uncontrollable, i.e. if it has been hit or crashed.
        private float m_BankedTurnAmount;
        private Rigidbody m_Rigidbody;

        private bool InAirportZone = false;             // Is the plane in a airfield area ?

        public override void SetUnitFromWorldUnitsManager(WorldSingleUnit unit, bool aiMove, bool aiShoot, bool aiSpawn) {
            // Original Unity aircraft elements
                m_Rigidbody = GetComponent<Rigidbody>();
                // Store original drag settings, these are modified during flight.
                m_OriginalDrag = m_Rigidbody.drag;
                m_OriginalAngularDrag = m_Rigidbody.angularDrag;
                for (int i = 0; i < transform.childCount; i++ ) {
                    foreach (var componentsInChild in transform.GetChild(i).GetComponentsInChildren<WheelCollider>()) {
                        componentsInChild.motorTorque = 0.18f;
                    }
                }

                m_TimeBeforePlayerControl = 5f;

            // Build Max Speed for spawner (m/seconds)
                MaxSpeed = m_MaxEnginePower * 3.33f;

            // Set Sound (?)
                if (GetComponent<AircraftAudio>()) {
                    GetComponent<AircraftAudio>().BeginOperations(this, m_Rigidbody);
                }
            // Set AircraftControlSurfaceAnimator (?)
                if (GetComponent<AircraftControlSurfaceAnimator>()) {
                    GetComponent<AircraftControlSurfaceAnimator>().BeginOperations(this);
                }
            // Set AircraftLandingGear (?)
                if (GetComponent<AircraftLandingGear>()) {
                    LandingGear = GetComponent<AircraftLandingGear>();
                    LandingGear.BeginOperations(this, m_Rigidbody);
                }

            // Set Movement
                if (GetComponent<AircraftMovement>()) {
                    Movement = GetComponent<AircraftMovement>();
                    Movement.BeginOperations(this);
                }

            base.SetUnitFromWorldUnitsManager(unit, aiMove, aiShoot, aiSpawn);

            if (UnitWorldSingleUnit.GetPlaneWeaponExists()) {
                // Debug.Log("yes"+ PlaneWeapons);
                PlaneWeapons.BeginOperations(this);
                UnitAI.SetPlaneWeaponsManager(PlaneWeapons);
            }
        }

        public void Move(float rollInput, float pitchInput, float yawInput, float throttleInput, bool airBrakes) {
            // Transfer input parameters into properties
            if (rollInput == 100) {
                RollInput = 0;
                PitchInput = 0;
            } else {
                RollInput = rollInput;
                PitchInput = pitchInput;
            }
            YawInput = yawInput;
            ThrottleInput = throttleInput;
            AirBrakes = airBrakes;

            ClampInputs();

            CalculateRollAndPitchAngles();

            AutoLevel();

            CalculateForwardSpeed();

            ControlThrottle();

            CalculateDrag();

            CalculateAerodynamicEffect();

            CalculateLinearForces();

            CalculateTorque();

            CalculateAltitude();
        }


        private void ClampInputs() {
            // clamp the inputs to -1 to 1 range
            RollInput = Mathf.Clamp(RollInput, -1, 1);
            PitchInput = Mathf.Clamp(PitchInput, -1, 1);
            YawInput = Mathf.Clamp(YawInput, -1, 1);
            ThrottleInput = Mathf.Clamp(ThrottleInput, -1, 1);
        }

        private void CalculateRollAndPitchAngles() {
            // Calculate roll & pitch angles
            // Calculate the flat forward direction (with no y component).
            var flatForward = transform.forward;
            flatForward.y = 0;
            // If the flat forward vector is non-zero (which would only happen if the plane was pointing exactly straight upwards)
            if (flatForward.sqrMagnitude > 0)
            {
                flatForward.Normalize();
                // calculate current pitch angle
                var localFlatForward = transform.InverseTransformDirection(flatForward);
                PitchAngle = Mathf.Atan2(localFlatForward.y, localFlatForward.z);
                // calculate current roll angle
                var flatRight = Vector3.Cross(Vector3.up, flatForward);
                var localFlatRight = transform.InverseTransformDirection(flatRight);
                RollAngle = Mathf.Atan2(localFlatRight.y, localFlatRight.x);
            }
        }

        private void AutoLevel() {
            // The banked turn amount (between -1 and 1) is the sine of the roll angle.
            // this is an amount applied to elevator input if the user is only using the banking controls,
            // because that's what people expect to happen in games!
            m_BankedTurnAmount = Mathf.Sin(RollAngle);
            // auto level roll, if there's no roll input:
            if (RollInput == 0f)
            {
                RollInput = -RollAngle*m_AutoRollLevel;
            }
            // auto correct pitch, if no pitch input (but also apply the banked turn amount)
            if (PitchInput == 0f)
            {
                PitchInput = -PitchAngle*m_AutoPitchLevel;
                PitchInput -= Mathf.Abs(m_BankedTurnAmount*m_BankedTurnAmount*m_AutoTurnPitch);
            }
        }

        private void CalculateForwardSpeed() {
            // Forward speed is the speed in the planes's forward direction (not the same as its velocity, eg if falling in a stall)
            var localVelocity = transform.InverseTransformDirection(m_Rigidbody.velocity);
            ForwardSpeed = Mathf.Max(0, localVelocity.z);
        }

        private void ControlThrottle() {
            // override throttle if immobilized
            if (m_Immobilized) {
                ThrottleInput = -0.5f;
            }

            // Adjust throttle based on throttle input (or immobilized state)
            Throttle = Mathf.Clamp01(Throttle + ThrottleInput*Time.deltaTime*m_ThrottleChangeSpeed);

            foreach (AircraftPropellerAnimator animator in PropellerAnimators) {
                animator.Throttle = Throttle;
            }

            // current engine power is just:
            EnginePower = Throttle*m_MaxEnginePower;
        }

        private void CalculateDrag() {
            // increase the drag based on speed, since a constant drag doesn't seem "Real" (tm) enough
            float extraDrag = m_Rigidbody.velocity.magnitude*m_DragIncreaseFactor;
            // Air brakes work by directly modifying drag. This part is actually pretty realistic!
            m_Rigidbody.drag = (AirBrakes ? (m_OriginalDrag + extraDrag)*m_AirBrakesEffect : m_OriginalDrag + extraDrag);
            // Forward speed affects angular drag - at high forward speed, it's much harder for the plane to spin
            m_Rigidbody.angularDrag = m_OriginalAngularDrag*ForwardSpeed;
        }

        private void CalculateAerodynamicEffect() {
            // "Aerodynamic" calculations. This is a very simple approximation of the effect that a plane
            // will naturally try to align itself in the direction that it's facing when moving at speed.
            // Without this, the plane would behave a bit like the asteroids spaceship!
            if (m_Rigidbody.velocity.magnitude > 0)
            {
                // compare the direction we're pointing with the direction we're moving:
                m_AeroFactor = Vector3.Dot(transform.forward, m_Rigidbody.velocity.normalized);
                // multipled by itself results in a desirable rolloff curve of the effect
                m_AeroFactor *= m_AeroFactor;
                // Finally we calculate a new velocity by bending the current velocity direction towards
                // the the direction the plane is facing, by an amount based on this aeroFactor
                var newVelocity = Vector3.Lerp(m_Rigidbody.velocity, transform.forward*ForwardSpeed,
                                               m_AeroFactor*ForwardSpeed*m_AerodynamicEffect*Time.deltaTime);
                m_Rigidbody.velocity = newVelocity;

                // also rotate the plane towards the direction of movement - this should be a very small effect, but means the plane ends up
                // pointing downwards in a stall
                m_Rigidbody.rotation = Quaternion.Slerp(m_Rigidbody.rotation,
                                                      Quaternion.LookRotation(m_Rigidbody.velocity, transform.up),
                                                      m_AerodynamicEffect*Time.deltaTime);
            }
        }

        private void CalculateLinearForces() {
            // Now calculate forces acting on the aeroplane:
            // we accumulate forces into this variable:
            var forces = Vector3.zero;
            // Add the engine power in the forward direction
            forces += EnginePower*transform.forward;
            // The direction that the lift force is applied is at right angles to the plane's velocity (usually, this is 'up'!)
            var liftDirection = Vector3.Cross(m_Rigidbody.velocity, transform.right).normalized;
            // The amount of lift drops off as the plane increases speed - in reality this occurs as the pilot retracts the flaps
            // shortly after takeoff, giving the plane less drag, but less lift. Because we don't simulate flaps, this is
            // a simple way of doing it automatically:
            var zeroLiftFactor = Mathf.InverseLerp(m_ZeroLiftSpeed, 0, ForwardSpeed);
            // Calculate and add the lift power
            var liftPower = ForwardSpeed*ForwardSpeed*m_Lift*zeroLiftFactor*m_AeroFactor;
            forces += liftPower*liftDirection;
            // Apply the calculated forces to the the Rigidbody
            m_Rigidbody.AddForce(forces);
        }

        private void CalculateTorque() {
            // We accumulate torque forces into this variable:
            var torque = Vector3.zero;
            // Add torque for the pitch based on the pitch input.
            torque += PitchInput*m_PitchEffect*transform.right;
            // Add torque for the yaw based on the yaw input.
            torque += YawInput*m_YawEffect*transform.up;
            // Add torque for the roll based on the roll input.
            torque += -RollInput*m_RollEffect*transform.forward;
            // Add torque for banked turning.
            torque += m_BankedTurnAmount*m_BankedTurnEffect*transform.up;
            // The total torque is multiplied by the forward speed, so the controls have more effect at high speed,
            // and little effect at low speed, or when not moving in the direction of the nose of the plane
            // (i.e. falling while stalled)
            m_Rigidbody.AddTorque(torque*ForwardSpeed*m_AeroFactor);
        }

        private void CalculateAltitude() {
            // Altitude calculations - we raycast downwards from the aeroplane
            // starting a safe distance below the plane to avoid colliding with any of the plane's own colliders
            var ray = new Ray(transform.position - Vector3.up*10, -Vector3.up);
            RaycastHit hit;
            Altitude = Physics.Raycast(ray, out hit) ? hit.distance + 10 : transform.position.y;
        }

        // Immobilize can be called from other objects, for example if this plane is hit by a weapon and should become uncontrollable
        public void Immobilize() {
            m_Immobilized = true;
        }

        public void SetAISpeed(int speedProportion){ 
            Movement.SetAISpeed(speedProportion);
        }
        public void SetAIPitch(int pitchProportion){ 
            Movement.SetAIPitch(pitchProportion);
        }

        public void SetPhysicalAsNormal (){
            foreach (HitboxComponent hitbox in UnitComponents) {
                hitbox.SetHitBoxActive(true);
            }
            GetComponent<Rigidbody>().useGravity = true;
        }

        // ALL OVERRIDES METHODS
        public override void SetSquadLeader() {
            GameManager = WorldUnitsManager.GetGameManager();
            StartCoroutine(TakeoffActionPauseLogic());
        }
        IEnumerator TakeoffActionPauseLogic(){
            yield return new WaitForSeconds(m_TimeBeforePlayerControl);
            TakeoffActionEnd();
        }
        protected void TakeoffActionEnd(){
            UnitModel.transform.parent = null;
            GameManager.UnitSpawned(this, UnitTeam);
            if (GetComponent<SpawnerScriptToAttach>()){
                GetComponent<SpawnerScriptToAttach>().SetGameManager(GameManager);
            }
        }

        public override void SetStaging(bool activate, bool advancing) {
            // Debug.Log("SetStaging");
            foreach (AircraftPropellerAnimator animator in PropellerAnimators) {
                if (advancing && activate) {
                    animator.SetForceMaxThrottle(true);
                } else {
                    animator.SetForceMaxThrottle(false);
                } 
            }
            // base.SetStaging(activate, advancing);

            GetComponent<Rigidbody>().useGravity = false;

            // Set AI Inactive
            if (activate) {
                UnitAI.SetStaging(true);
                UnitAI.SetAIActive(false);
            } else {
                UnitAI.SetStaging(false);
                UnitAI.SetAIActive(!Active);
            }

            foreach (HitboxComponent hitbox in UnitComponents) {
                hitbox.SetHitBoxActive(false);
            }

            if (Turrets != null)
                Turrets.SetPause(activate);
        }
        public override void SetActive(bool activate) {
            base.SetActive(activate);
            if (Movement != null)
                Movement.SetActive(Active);

            if (PlaneWeapons != null)
                PlaneWeapons.SetActive(Active);
        }
        public override void SetMap(bool map) {
            if (Movement != null)
                Movement.SetMap(map);
            if (PlaneWeapons != null)
                PlaneWeapons.SetMap(map);
            base.SetMap(map);
        }
        public override void SetFreeCamera(bool freeCam) {
            base.SetFreeCamera(freeCam);
            Movement.SetFreeCamera(freeCam);
            if (PlaneWeapons != null) 
                PlaneWeapons.SetFreeCamera(freeCam);
        }
        public override void SetInAirfieldZone(bool action) {
            if (InAirportZone != action) {
                InAirportZone = action;
                if (InAirportZone) {
                    // Debug.Log(UnitName+" has has entered an airfield zone !");
                } else {
                    // Debug.Log(UnitName+" has exited an airfield zone !");
                }
                if (LandingGear != null) {
                    LandingGear.SetInAirfieldZone(action);
                }
            }
        }
        public override void CallDeath() {
            base.CallDeath();
            Movement.SetDead(true);
        }

    }