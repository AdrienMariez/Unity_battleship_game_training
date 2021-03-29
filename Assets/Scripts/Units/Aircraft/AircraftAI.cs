using UnityEngine;
using System.Collections;

public class AircraftAI : UnitAIController {
        [SerializeField] private float m_RollSensitivity = .2f;         // How sensitively the AI applies the roll controls
        [SerializeField] private float m_PitchSensitivity = .5f;        // How sensitively the AI applies the pitch controls
        [SerializeField] private float m_LateralWanderDistance = 5;     // The amount that the plane can wander by when heading for a target
        [SerializeField] private float m_LateralWanderSpeed = 0.11f;    // The speed at which the plane will wander laterally
        [SerializeField] private float m_MaxClimbAngle = 45;            // The maximum angle that the AI will attempt to make plane can climb at
        [SerializeField] private float m_MaxRollAngle = 45;             // The maximum angle that the AI will attempt to u
        [SerializeField] private float m_SpeedEffect = 0.01f;           // This increases the effect of the controls based on the plane's speed.
        [SerializeField] private float m_TakeoffHeight = 20;            // the AI will fly straight and only pitch upwards until reaching this height
        private Transform MovePosition;                                 // The position the AI move to or circle
        [SerializeField] private float m_FlyAltitude = 200;              // the AI will fly at this altitude by default

        // private AeroplaneController m_AeroplaneController;  // The aeroplane controller that is used to move the plane
        private float m_RandomPerlin;                       // Used for generating random point on perlin noise so that the plane will wander off path slightly
        private bool m_TakenOff;                            // Has the plane taken off yet


    protected AircraftController AircraftController;
    public UnitsAIStates AircraftAISpawnState = UnitsAIStates.Patrol;
    protected float RotationSafeDistance;  // this var is used to determine if a waypoint is inside a turning arc of a full speed ship
    public override void BeginOperations (bool aiMove, bool aiShoot, bool aiSpawn) {
        // pick a random perlin starting point for lateral wandering
            m_RandomPerlin = Random.Range(0f, 100f);
            m_TakenOff = false;

        UnitsAICurrentState = AircraftAISpawnState;
        // Still need the specific unit Controller for specific methods
        AircraftController = GetComponent<AircraftController>();
        // RotationSafeDistance = ShipController.GetUnitWorldSingleUnit().GetBuoyancyRotationTime() * 6;
        // Debug.Log("Unit : "+ Name +"RotationSafeDistance: " + RotationSafeDistance);
        base.BeginOperations(aiMove, aiShoot, aiSpawn);
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();

        if (AIActive) {
            if (Waypoints.Count > 0 && UsesWaypoints && UnitCanMove) {
                FollowWayPointsAction();
            }
            if (UnitsAICurrentState == UnitsAIStates.NoAI) {
                NoAIInput();
            } else if (UnitsAICurrentState == UnitsAIStates.Takeoff) {
                FlyForward();
            } else if (Stressed && TargetUnit != null) {
                FlyTowardsTarget();
            }
        }
    }
    protected void FlyTowardsTarget() {
        if (AIActive) {
            Debug.Log("FlyTowardsTarget - "+ UnitsAICurrentState);
        }
        // Debug.Log("Unit : "+ Name +" - TargetUnit = "+ TargetUnit);
        // make the plane wander from the path, useful for making the AI seem more human, less robotic.
        Vector3 targetPos = TargetUnit.transform.position +
                            transform.right*
                            (Mathf.PerlinNoise(Time.time*m_LateralWanderSpeed, m_RandomPerlin)*2 - 1)*
                            m_LateralWanderDistance;

        // adjust the yaw and pitch towards the target
        Vector3 localTarget = transform.InverseTransformPoint(targetPos);
        float targetAngleYaw = Mathf.Atan2(localTarget.x, localTarget.z);
        float targetAnglePitch = -Mathf.Atan2(localTarget.y, localTarget.z);


        // Set the target for the planes pitch, we check later that this has not passed the maximum threshold
        targetAnglePitch = Mathf.Clamp(targetAnglePitch, -m_MaxClimbAngle*Mathf.Deg2Rad,
                                        m_MaxClimbAngle*Mathf.Deg2Rad);

        // calculate the difference between current pitch and desired pitch
        float changePitch = targetAnglePitch - AircraftController.PitchAngle;

        // AI always applies gentle forward throttle
        const float throttleInput = 0.5f;

        // AI applies elevator control (pitch, rotation around x) to reach the target angle
        float pitchInput = changePitch*m_PitchSensitivity;

        // clamp the planes roll
        float desiredRoll = Mathf.Clamp(targetAngleYaw, -m_MaxRollAngle*Mathf.Deg2Rad, m_MaxRollAngle*Mathf.Deg2Rad);
        float yawInput = 0;
        float rollInput = 0;
        if (!m_TakenOff) {
            // If the planes altitude is above m_TakeoffHeight we class this as taken off
            if (AircraftController.Altitude > m_TakeoffHeight) {
                m_TakenOff = true;
            }
        } else {
            // now we have taken off to a safe height, we can use the rudder and ailerons to yaw and roll
            yawInput = targetAngleYaw;
            rollInput = -(AircraftController.RollAngle - desiredRoll)*m_RollSensitivity;
        }

        // adjust how fast the AI is changing the controls based on the speed. Faster speed = faster on the controls.
        float currentSpeedEffect = 1 + (AircraftController.ForwardSpeed*m_SpeedEffect);
        rollInput *= currentSpeedEffect;
        pitchInput *= currentSpeedEffect;
        yawInput *= currentSpeedEffect;

        // pass the current input to the plane (false = because AI never uses air brakes!)
        AircraftController.Move(rollInput, pitchInput, yawInput, throttleInput, false);
    }
    protected void FlyTowardsPosition(Vector3 positionV3) {
        if (AIActive) {
            Debug.Log("FlyTowardsPosition - "+ UnitsAICurrentState);
        }
        
        // make the plane wander from the path, useful for making the AI seem more human, less robotic.
        Vector3 targetPos = positionV3 +
                            transform.right*
                            (Mathf.PerlinNoise(Time.time*m_LateralWanderSpeed, m_RandomPerlin)*2 - 1)*
                            m_LateralWanderDistance;

        // adjust the yaw and pitch towards the target
        Vector3 localTarget = transform.InverseTransformPoint(targetPos);
        float targetAngleYaw = Mathf.Atan2(localTarget.x, localTarget.z);
        float targetAnglePitch = -Mathf.Atan2(localTarget.y, localTarget.z);


        // Set the target for the planes pitch, we check later that this has not passed the maximum threshold
        targetAnglePitch = Mathf.Clamp(targetAnglePitch, -m_MaxClimbAngle*Mathf.Deg2Rad,
                                        m_MaxClimbAngle*Mathf.Deg2Rad);

        // calculate the difference between current pitch and desired pitch
        float changePitch = targetAnglePitch - AircraftController.PitchAngle;

        // AI always applies gentle forward throttle
        const float throttleInput = 0.5f;

        // AI applies elevator control (pitch, rotation around x) to reach the target angle
        float pitchInput = changePitch*m_PitchSensitivity;

        // clamp the planes roll
        float desiredRoll = Mathf.Clamp(targetAngleYaw, -m_MaxRollAngle*Mathf.Deg2Rad, m_MaxRollAngle*Mathf.Deg2Rad);
        float yawInput = 0;
        float rollInput = 0;
        if (!m_TakenOff) {
            // If the planes altitude is above m_TakeoffHeight we class this as taken off
            if (AircraftController.Altitude > m_TakeoffHeight) {
                m_TakenOff = true;
            }
        } else {
            // now we have taken off to a safe height, we can use the rudder and ailerons to yaw and roll
            yawInput = targetAngleYaw;
            rollInput = -(AircraftController.RollAngle - desiredRoll)*m_RollSensitivity;
        }

        // adjust how fast the AI is changing the controls based on the speed. Faster speed = faster on the controls.
        float currentSpeedEffect = 1 + (AircraftController.ForwardSpeed*m_SpeedEffect);
        rollInput *= currentSpeedEffect;
        pitchInput *= currentSpeedEffect;
        yawInput *= currentSpeedEffect;

        // pass the current input to the plane (false = because AI never uses air brakes!)
        AircraftController.Move(rollInput, pitchInput, yawInput, throttleInput, false);
    }
    protected void FlyForward() {
        if (AIActive) {
            Debug.Log("FlyForward - "+ UnitsAICurrentState);
        }
        AircraftController.Move(0, 0, 0, 1, false);
    }
    protected  void NoAIInput() {
        AircraftController.Move(0, 0, 0, 0, false);
    }
    protected override IEnumerator AIOrdersLoop(){
        while (true) {
            yield return new WaitForSeconds(1f);
            if (AIActive) {
                // Debug.Log("PauseAIOrders");
                // If AI controlled
                if (TargetUnit == null &&  UnitCanShoot) {
                    SetNewTarget();
                }
                CheckState();
            }
        }
    }
    protected override void SetNewTarget() {
        // SetNewTarget is completely overwritten as the target position needs to be reused for TargetPosition used by planes to know where they need to go.
        Debug.Log("Unit : "+ Name +" - Team.id = "+ Team.id);
        TargetUnit = null;
        float range = 0f;
        if (EnemyUnitsList.Count > 0) {
            foreach (var enemyUnit in EnemyUnitsList) {
                // Debug.Log("enemyUnit : "+ enemyUnit);
                float distance = (gameObject.transform.position - enemyUnit.transform.position).magnitude;
                if (range == 0) {
                    range = distance;
                    TargetUnit = enemyUnit;
                } else if (distance < range) {
                    TargetUnit = enemyUnit;
                }
            }
        }
        // UnitMasterController.SetCurrentTarget(TargetUnit);
        // Debug.Log("EnemyUnitsList : "+ EnemyUnitsList.Count);
        // Debug.Log("TargetUnit : "+ TargetUnit);    

        CheckState();
    }

    public override void SetPlayerSetTargetByController(UnitMasterController targetedUnitController) {
        base.SetPlayerSetTargetByController(targetedUnitController);
    }

    public override void SetNewMoveLocation(Vector3 waypointPosition, MapManager.RaycastHitType raycastHitType) {
        // Change altitude of point location
        Vector3 updatedwaypointPosition = new Vector3(waypointPosition.x, waypointPosition.y + m_FlyAltitude, waypointPosition.z);

        base.SetNewMoveLocation(updatedwaypointPosition, raycastHitType);
    }

    // UnitsAIStates
    // UnitsAICurrentState
    protected override void CheckState() {
        // if (UnitsAICurrentState == UnitsAIStates.NoAI) {
        //     return;
        // }
        // If there is a target
        if (Waypoints.Count > 0 && UsesWaypoints && UnitCanMove) {
            UnitsAICurrentState = UnitsAIStates.FollowWayPoints;
        } else if (UnitsAICurrentState == UnitsAIStates.Takeoff) {
            //Don't touch anything if the plane is taking off
        } else if (TargetUnit != null && UnitCanShoot) {
            // I'm not sure about this one... The logic is : if it's one of the correct states, check if the target is in range or not. Act accordingly
            if (UnitsAICurrentState == UnitsAIStates.ApproachTarget || UnitsAICurrentState == UnitsAIStates.CircleTarget || UnitsAICurrentState == UnitsAIStates.Patrol) {
                if ((gameObject.transform.position - TargetUnit.transform.position).magnitude > MaxTurretsRange) {
                    UnitsAICurrentState = UnitsAIStates.ApproachTarget;
                    // Debug.Log("Unit : "+ Name +" UnitsAICurrentState = "+ UnitsAICurrentState);
                } else {
                    UnitsAICurrentState = UnitsAIStates.CircleTarget;
                    // Debug.Log("Unit : "+ Name +" UnitsAICurrentState = "+ UnitsAICurrentState);
                }
            }
        } else {
            if (UnitsAICurrentState == UnitsAIStates.ApproachTarget || UnitsAICurrentState == UnitsAIStates.CircleTarget || UnitsAICurrentState == UnitsAIStates.Patrol) {
                UnitsAICurrentState = UnitsAIStates.Patrol;
                // Debug.Log("Unit : "+ Name +" UnitsAICurrentState = "+ UnitsAICurrentState);
            } else {
                UnitsAICurrentState = UnitsAIStates.Idle;
                // Debug.Log("Unit : "+ Name +" UnitsAICurrentState = "+ UnitsAICurrentState);
            }
        }

        base.CheckState();

        if (AIActive) {
            Debug.Log("Unit : "+ Name +" - TargetUnit = "+ TargetUnit +" - UnitsAICurrentState = "+ UnitsAICurrentState);
        }

        // Debug.Log("Unit : "+ Name +" - magnitude = "+ (gameObject.transform.position - TargetUnit.transform.position).magnitude +" - MaxTurretsRange = "+ MaxTurretsRange);
    }

    // Possible Actions
    protected override void PatrolAction(){
        // Not done at all
        // AircraftController.SetAISpeed(1);
        // ShipController.SetAIturn(0);
    }
    protected override void CircleTargetAction(){
        AircraftController.SetAISpeed(1);

        Vector3 targetDir = gameObject.transform.position - TargetUnit.transform.position;
        Vector3 forward = gameObject.transform.forward;
        float angle = Vector3.SignedAngle(targetDir, forward, Vector3.up);

        if (angle > 95 && angle > 0 && TurnInputLimit < 1 || angle > -85 && angle < 0 && TurnInputLimit < 1) {
            // ShipController.SetAIturn(-0.5f);
        } else if (angle < 85  && angle > 0 && TurnInputLimit > -1 || angle < -95 && angle < 0 && TurnInputLimit > -1) {
            // ShipController.SetAIturn(0.5f);
        } else {
            // ShipController.SetAIturn(0);
        }
    }
    protected override void ApproachTargetAction(){
                // Debug.Log("ApproachTarget");
        AircraftController.SetAISpeed(1);

        Vector3 targetDir = gameObject.transform.position - TargetUnit.transform.position;
        Vector3 forward = gameObject.transform.forward;
        float angle = Vector3.SignedAngle(targetDir, forward, Vector3.up);

        // Not tested !
        if (angle > 5 && angle < 180 && TurnInputLimit < 1) {
            // ShipController.SetAIturn(-0.5f);
        } else if (angle < -5 && angle > -180 && TurnInputLimit > -1) {
            // ShipController.SetAIturn(0.5f);
        } else {
            // ShipController.SetAIturn(0);
        }
        // Debug.Log("angle : "+ angle);
    }
    protected override void IdleAction(){
        AircraftController.SetAISpeed(0);
        // ShipController.SetAIturn(0);
    }
    protected override void FollowWayPointsAction(){
        // Debug.Log("Unit : "+ Name +" - UnitsAICurrentState = "+ UnitsAICurrentState);
        
        if (!UsesWaypoints) {
            FlyForward();
        }

        FlyTowardsPosition(Waypoints[0]);
    }
    protected override void FleeAction(){
        AircraftController.SetAISpeed(1);
    }
    protected override void BackToBaseAction(){
        AircraftController.SetAISpeed(1);
    }
    protected override void TakeoffAction(){
        // Debug.Log("Unit : "+ Name +" - UnitsAICurrentState = "+ UnitsAICurrentState);
        // AircraftController.SetAISpeed(1);
        // AircraftController.SetAIPitch(-1);
        // Take off; wait for X time then switch ai for what is specified as the basic AI.
        //It's logical that there can be no planes flying after a takeoff that won't try to fly so do not care about NoAI edgecase.
        StartCoroutine(TakeoffActionPauseLogic());
        // UnitsAICurrentState = AircraftAISpawnState;
        // AircraftController.SetPhysicalAsNormal();

        // FUCK YOU CHECK STATE
        // CheckState();
    }
    IEnumerator TakeoffActionPauseLogic(){
        yield return new WaitForSeconds(AircraftController.m_TimeBeforePlayerControl);
        TakeoffActionEnd();
    }
    protected void TakeoffActionEnd(){
        AircraftController.SetPhysicalAsNormal();
        // AircraftController.SetAIPitch(0);
        UnitsAICurrentState = AircraftAISpawnState;
        CheckState();
    }
    protected override void LandingAction(){
        AircraftController.SetAISpeed(0);
    }
    protected override void NoAIAction(){
        AircraftController.SetAISpeed(0);
    }

    // Unit Manager sent info
    public override void SetAITurnInputValue(float turnInputValue){ TurnInputLimit = turnInputValue; }

    public override void SetAIActive(bool activate) {
        base.SetAIActive(activate);
        CheckState();
    }
    public override void SetStaging(bool staging) {
        if (staging) {
            UnitsAICurrentState = UnitsAIStates.NoAI; 
        } else {
            UnitsAICurrentState = UnitsAIStates.Takeoff;
        }

        // Debug.Log("Unit : "+ Name +" - UnitsAICurrentState = "+ UnitsAICurrentState);
    }
}