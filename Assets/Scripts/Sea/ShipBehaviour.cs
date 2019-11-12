using Waterphysics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WaterFloat))]
public class ShipBehaviour : MonoBehaviour
{
    //visible Properties
    public int m_PlayerNumber = 1;
    public Transform Motor;
    public float SteerPower = 500f;
    public float Power = 5f;
    public float m_MaxSpeed = 10f;
    public float Drag = 0.1f;

    //used Components
    protected Rigidbody Rigidbody;
    protected Quaternion StartRotation;
    protected ParticleSystem ParticleSystem;

    //internal Properties
    protected Vector3 CamVel;     
    [HideInInspector] public bool m_Active; 
    private float m_MovementInputValue;    // The current value of the movement input.
    private float m_TurnInputValue;
    [SerializeField] [Range(-4, 4)] public int m_CurrentSpeedStep;      // this mimics an Engine Order Telegraph, possible speeds below in comments
    [HideInInspector] private bool m_OrderSystem = true;                // This var is used to pause the system to allow the player to choose the speed order (instead of being pushed to the max values)
    /*
        -4  Full Ahead
        -3  Half Ahead
        -2  Slow Ahead
        -1  Dead Slow Ahead
        0   Stop
        1   Dead Slow Astern
        2   Slow Astern
        3   Half Astern
        4   Full Astern
    */
    [HideInInspector] public float m_LocalTargetSpeed = 0f;     //  The speed calculated by the Engine Order Telegraph. This is not the real speed but what the ship will try to set.
    public float m_SpeedInertia = 0.3f;                        // The rate at which the ship will gain or lose speed. 0.3f = good inertia. 1f = almost instant speed correction.
    [HideInInspector] private bool m_SpeedIncrementation = true;// Used to allow the m_SpeedInertia to take some time.
    [HideInInspector] public float m_LocalRealSpeed;            // The real final speed of the ship.

    public void Awake()
    {
        ParticleSystem = GetComponentInChildren<ParticleSystem>();
        Rigidbody = GetComponent<Rigidbody>();
        StartRotation = Motor.localRotation;
        SetTargetSpeed();
        m_LocalRealSpeed = m_LocalTargetSpeed;
    }

    public void FixedUpdate()
    {

        // Debug.Log ("m_MovementInputValue : "+ m_MovementInputValue);
        // Debug.Log ("m_TurnInputValue : "+ m_TurnInputValue);

        if (m_Active) {
            m_MovementInputValue = Input.GetAxis ("VerticalShip");
            m_TurnInputValue = Input.GetAxis ("HorizontalShip");

            if (m_MovementInputValue == 1){
                ChangeSpeedStep(1);
            }
            else if(m_MovementInputValue == -1){
                ChangeSpeedStep(-1);
            }
            if (!Input.GetButton ("FreeCamera")) {
                //TurretRotate();
            }
            else {
                //Debug.Log ("Free Camera Activated");
            }
        }
        else
        {
            m_MovementInputValue = Input.GetAxis ("Empty");
            m_TurnInputValue = Input.GetAxis ("Empty");
        }

        Movement();
    }

    private void ChangeSpeedStep(int Step) {
        if (Step > 0 && m_CurrentSpeedStep < 4 && m_OrderSystem) {
            m_CurrentSpeedStep = m_CurrentSpeedStep +1;
            // Debug.Log (m_CurrentSpeedStep+" - m_CurrentSpeedStep -");
            SetTargetSpeed();
            StartCoroutine(PauseOrderSystem());
        }
        else if(Step < 0 && m_CurrentSpeedStep > -4 && m_OrderSystem) {
            m_CurrentSpeedStep = m_CurrentSpeedStep -1;
            // Debug.Log (m_CurrentSpeedStep+" - m_CurrentSpeedStep -");
            SetTargetSpeed();
            StartCoroutine(PauseOrderSystem());
        }
    }

    IEnumerator PauseOrderSystem(){
        m_OrderSystem = false;
        yield return new WaitForSeconds(1f);
        m_OrderSystem = true;
    }
    private void SetTargetSpeed() {
        if (m_CurrentSpeedStep == 4) {
            m_LocalTargetSpeed = m_MaxSpeed;
        }
        else if(m_CurrentSpeedStep == 3) {
            m_LocalTargetSpeed = m_MaxSpeed*0.6f;
        }
        else if(m_CurrentSpeedStep == 2) {
            m_LocalTargetSpeed = m_MaxSpeed*0.3f;
        }
        else if(m_CurrentSpeedStep == 1) {
            m_LocalTargetSpeed = m_MaxSpeed*0.1f;
        }
        else if(m_CurrentSpeedStep == 0) {
            m_LocalTargetSpeed = 0;
        }
        else if(m_CurrentSpeedStep == -1) {
            m_LocalTargetSpeed = -m_MaxSpeed*0.1f;
        }
        else if(m_CurrentSpeedStep == -2) {
            m_LocalTargetSpeed = -m_MaxSpeed*0.2f;
        }
        else if(m_CurrentSpeedStep == -3) {
            m_LocalTargetSpeed = -m_MaxSpeed*0.4f;
        }
        else if(m_CurrentSpeedStep == -4) {
            m_LocalTargetSpeed = -m_MaxSpeed*0.6f;
        }
        // Debug.Log ("- m_LocalTargetSpeed - :"+ m_LocalTargetSpeed);
    }
    private void SetRealSpeed() {
        if (m_LocalRealSpeed < m_LocalTargetSpeed && m_SpeedIncrementation) {
            if ((m_LocalRealSpeed+m_SpeedInertia) > m_LocalTargetSpeed) {
                m_LocalRealSpeed = m_LocalTargetSpeed;
            }
            else {
                m_LocalRealSpeed += m_SpeedInertia;
            }
            
            if (m_LocalRealSpeed < 0 && m_LocalTargetSpeed > 0)
            {
                m_LocalRealSpeed += m_SpeedInertia;
                // Debug.Log ("- DOUBLE SPEED FRONT - :"+ m_LocalRealSpeed);
            }

            StartCoroutine(PauseSpeedIncrementation());

            Debug.Log ("- FORTH - :"+ m_LocalRealSpeed);

        }
        else if (m_LocalRealSpeed > m_LocalTargetSpeed && m_SpeedIncrementation) {
            if ((m_LocalRealSpeed-m_SpeedInertia) < m_LocalTargetSpeed) {
                m_LocalRealSpeed = m_LocalTargetSpeed;
            }
            else {
                m_LocalRealSpeed += -m_SpeedInertia;
            }
            if (m_LocalRealSpeed > 0 && m_LocalTargetSpeed < 0)
            {
                m_LocalRealSpeed += -m_SpeedInertia;
                // Debug.Log ("- DOUBLE SPEED BACK - :"+ m_LocalRealSpeed);
            }
            StartCoroutine(PauseSpeedIncrementation());
            Debug.Log ("- BACK - :"+ m_LocalRealSpeed);
        }
    }
    IEnumerator PauseSpeedIncrementation(){
        m_SpeedIncrementation = false;
        yield return new WaitForSeconds(0.5f);
        m_SpeedIncrementation = true;
    }

    private void Movement() {
        SetRealSpeed();
        //default direction
        var forceDirection = transform.forward;
        Debug.Log ("- forceDirection - :"+ forceDirection);
        var steer = 0;

        if (m_TurnInputValue > 0)
            steer = -1;
        if (m_TurnInputValue < 0)
            steer = 1;
        
        //Rotational Force
        Rigidbody.AddForceAtPosition(steer * transform.right * SteerPower / 100f, Motor.position);

        //compute vectors
        var forward = Vector3.Scale(new Vector3(1,0,1), transform.forward);
        // Debug.Log ("- forward - :"+ forward);
        var targetVel = Vector3.zero;

        //forward/backward power
        // if (m_MovementInputValue > 0)
        //     PhysicsHelper.ApplyForceToReachVelocity(Rigidbody, forward * m_MaxSpeed, Power);
        // if (m_MovementInputValue < 0)
        //     PhysicsHelper.ApplyForceToReachVelocity(Rigidbody, forward * -m_MaxSpeed, Power);
        
        PhysicsHelper.ApplyForceToReachVelocity(Rigidbody, forward * m_LocalRealSpeed, Power);

        //Motor Animation // Particle system
        Motor.SetPositionAndRotation(Motor.position, transform.rotation * StartRotation * Quaternion.Euler(0, 30f * steer, 0));
        if (ParticleSystem != null)
        {
            if (m_MovementInputValue != 0)
                ParticleSystem.Play();
            else
                ParticleSystem.Pause();
        }

        //moving forward
        var movingForward = Vector3.Cross(transform.forward, Rigidbody.velocity).y < 0;
        Debug.Log ("- transform.forward - :"+ transform.forward);

        //move in direction
        Rigidbody.velocity = Quaternion.AngleAxis(Vector3.SignedAngle(Rigidbody.velocity, (movingForward ? 1f : 0f) * transform.forward, Vector3.up) * Drag, Vector3.up) * Rigidbody.velocity;
        // Debug.Log ("- Rigidbody.velocity - :"+ Rigidbody.velocity);
    }

}