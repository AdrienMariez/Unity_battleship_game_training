using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Crest;

public class ShipMovement : MonoBehaviour
{
    [HideInInspector] public bool m_Active; 
    [HideInInspector] public bool m_Dead; 
    [HideInInspector] public float m_MaxSpeed = 1f;
    [HideInInspector] public float m_TurnSpeed = 1f;
    public AudioSource m_MovementAudio;
    public AudioClip m_EngineIdling;
    public AudioClip m_EngineDriving;
    public float m_PitchRange = 0.2f;       // The amount by which the pitch of the engine noises

    
    private string m_MovementAxisName;
    private string m_TurnAxisName;
    private float m_MovementInputValue;    // The current value of the movement input.
    private float m_TurnInputValue = 0f;        
    private float m_FreeCamInputValue;      
    private float m_OriginalPitch;         // The pitch of the audio source at the start of the scene.

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
    private bool m_SpeedIncrementation = true;                  // Used to allow the m_SpeedInertia to take some time.
    [HideInInspector] public float m_LocalRealSpeed;            // The real final speed of the ship.
    [HideInInspector] public float m_LocalRealRotation;            // The real final rotation of the ship.
    private bool m_RotationIncrementation = true;// Used to allow the m_SpeedInertia to take some time.

    private ShipBuoyancy m_Buoyancy;

    private void Awake() {
    }

    private void Start()
    {
        m_Buoyancy = GetComponent<ShipBuoyancy>();
        // Store the original pitch of the audio source.
        m_OriginalPitch = m_MovementAudio.pitch;
    }

    private void Update()
    {
        // This is run every frame
        // Store the player's input and make sure the audio for the engine is playing.
        EngineAudio ();
    }


    private void EngineAudio()
    {
        // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.

        //Abs (x) means that if x=-3, Abs(x)=3
        //0.1f means 0.1 as a FLOAT value
        //if not moving or rotating
        if (m_CurrentSpeedStep != 0) {
            //if moving audio currently playing
             if (m_MovementAudio.clip == m_EngineDriving)
             {
                //switch playing clip
                m_MovementAudio.clip = m_EngineIdling;
                //randomize pitch
                m_MovementAudio.pitch = Random.Range (m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                //play the new audio
                m_MovementAudio.Play ();
             }
        } else {
            //if idling audio currently playing
             if (m_MovementAudio.clip == m_EngineIdling)
             {
                //switch playing clip
                m_MovementAudio.clip = m_EngineDriving;
                //randomize pitch
                m_MovementAudio.pitch = Random.Range (m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                //play the new audio
                m_MovementAudio.Play ();
             }
        }
    }


    private void FixedUpdate() {
        // This is run every physics step instead of every frame
        // Move and turn the tank.
        if (m_Active && !m_Dead) {
            if (Input.GetAxis ("VerticalShip") == 1){
                ChangeSpeedStep(1);
            } else if(Input.GetAxis ("VerticalShip") == -1){
                ChangeSpeedStep(-1);
            }
            Move ();
            Turn ();
        }
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
        //Debug.Log ("- m_LocalSpeed - :"+ m_LocalTargetSpeed);
    }
    public void SetRealSpeed() {
        if (m_LocalRealSpeed < m_LocalTargetSpeed && m_SpeedIncrementation) {
            if ((m_LocalRealSpeed+m_SpeedInertia) > m_LocalTargetSpeed) {
                m_LocalRealSpeed = m_LocalTargetSpeed;
            } else {
                m_LocalRealSpeed += m_SpeedInertia;
            }
            
            if (m_LocalRealSpeed < 0 && m_LocalTargetSpeed > 0) {
                m_LocalRealSpeed += m_SpeedInertia;
                // Debug.Log ("- DOUBLE SPEED FRONT - :"+ m_LocalRealSpeed);
            }

            StartCoroutine(PauseSpeedIncrementation());
            // Debug.Log ("- FORTH - :"+ m_LocalRealSpeed);
        }
        else if (m_LocalRealSpeed > m_LocalTargetSpeed && m_SpeedIncrementation) {
            if ((m_LocalRealSpeed-m_SpeedInertia) < m_LocalTargetSpeed) {
                m_LocalRealSpeed = m_LocalTargetSpeed;
            } else {
                m_LocalRealSpeed += -m_SpeedInertia;
            }
            if (m_LocalRealSpeed > 0 && m_LocalTargetSpeed < 0) {
                m_LocalRealSpeed += -m_SpeedInertia;
                // Debug.Log ("- DOUBLE SPEED BACK - :"+ m_LocalRealSpeed);
            }
            StartCoroutine(PauseSpeedIncrementation());
            // Debug.Log ("- BACK - :"+ m_LocalRealSpeed);
        }
    }
    IEnumerator PauseSpeedIncrementation(){
        m_SpeedIncrementation = false;
        yield return new WaitForSeconds(0.5f);
        m_SpeedIncrementation = true;
    }

    private void Move() {
        SetRealSpeed();
        m_Buoyancy.SpeedInput = m_LocalRealSpeed;
    }

    private void Turn()
    {
        // float tempTurn = m_TurnInputValue;
        // Determine the number of degrees to be turned based on the input, speed and time between frames.
        if (Input.GetAxis ("HorizontalShip") == 1 && m_TurnInputValue < 1 && m_RotationIncrementation){
            m_TurnInputValue += 0.5f;
            StartCoroutine(PauseTurnIncrementation());
        } else if(Input.GetAxis ("HorizontalShip") == -1 && m_TurnInputValue > -1 && m_RotationIncrementation){
            m_TurnInputValue -= 0.5f;
            StartCoroutine(PauseTurnIncrementation());
        }
        // Multiply the targeted rotation by the speed : reverts input when in reverse and prevents spinning stopped ships 
        m_LocalRealRotation = m_TurnInputValue * m_LocalRealSpeed;
        // Debug.Log ("- m_TurnInputValue - :"+ m_TurnInputValue);
        m_Buoyancy.RotationInput = m_LocalRealRotation;
    }

    IEnumerator PauseTurnIncrementation(){
        m_RotationIncrementation = false;
        yield return new WaitForSeconds(0.5f);
        m_RotationIncrementation = true;
    }

    public float GetCurrentSpeedStep(){
        return m_CurrentSpeedStep;
    }
    public float GetLocalRealSpeed(){
        return m_LocalRealSpeed;
    }
    public float GetLocalRealRotation(){
        return m_LocalRealRotation;
    }
}