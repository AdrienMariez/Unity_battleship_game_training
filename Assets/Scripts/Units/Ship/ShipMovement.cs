using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Crest;

public class ShipMovement : MonoBehaviour
{
    private bool Active = false; 
    private bool Dead = false; 
    private bool Damaged = false;
    private bool MapActive = false;
    private float DamagedRatio;
    private bool AllowTurnInput = true;
    [HideInInspector] public float m_MaxSpeed = 1f;
    [HideInInspector] public float m_TurnSpeed = 1f;
    public AudioSource m_MovementAudio;
    public AudioClip m_EngineIdling;
    public AudioClip m_EngineDriving;
    [Tooltip("The amount by which the pitch of the engine noises")] public float m_PitchRange = 0.2f;

    private float TurnInputValue = 0f;    
    private float OriginalPitch;         // The pitch of the audio source at the start of the scene.

    [Tooltip("this mimics an Engine Order Telegraph")] [SerializeField] [Range(-4, 4)] public int m_CurrentSpeedStep;      // possible speeds below in comments
    /*
        -4  Full Ahead 100%
        -3  Half Ahead 50%
        -2  Slow Ahead 30%
        -1  Dead Slow Ahead 10%
        0   Stop 0%
        1   Dead Slow Astern -10%
        2   Slow Astern -20%
        3   Half Astern -40%
        4   Full Astern -60%
    */
    private int PreviousSpeedStep;
    private float LocalTargetSpeed = 0f;     //  The speed calculated by the Engine Order Telegraph. This is not the real speed but what the ship will try to set.
    [Tooltip("The rate at which the ship will gain or lose speed. 0.3f = good inertia. 1f = almost instant speed correction.")]public float m_SpeedInertia = 0.3f;
    private float LocalRealSpeed;            // The real final speed of the ship.
    private float LocalRealRotation;            // The real final rotation of the ship.
    

    public StackComponentSmoke[] m_StackComponents;

    private ShipController ShipController;

    public void BeginOperations(ShipController unitController) {
        ShipController = unitController;
        // Store the original pitch of the audio source.
        OriginalPitch = m_MovementAudio.pitch;

        foreach (StackComponentSmoke component in m_StackComponents) {
            // component.SetInstanceIdle(Instantiate(component.GetSmokePrefab(), component.GetStackPosition().position, component.GetStackPosition().rotation, this.transform) as GameObject);
            component.SetStackInstance(Instantiate(component.GetSmokePrefab(), component.GetStackPosition().position, component.GetStackPosition().rotation, this.transform) as GameObject);
        }
        SetTargetSpeed();
        PreviousSpeedStep = m_CurrentSpeedStep;
        EngineAudio();
        m_MovementAudio.pitch = Random.Range (OriginalPitch - m_PitchRange, OriginalPitch + m_PitchRange);
    }

    private void EngineAudio() {
        // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.

        //Abs (x) means that if x=-3, Abs(x)=3
        //0.1f means 0.1 as a FLOAT value
        //if not moving or rotating
        if (PreviousSpeedStep != m_CurrentSpeedStep) {
            if (m_CurrentSpeedStep != 0) {
                //if moving audio currently playing
                if (m_MovementAudio.clip == m_EngineDriving) {
                    //switch playing clip
                    m_MovementAudio.clip = m_EngineIdling;
                    //play the new audio
                    m_MovementAudio.Play ();
                }
                //randomize pitch
                // m_MovementAudio.pitch = Random.Range ((OriginalPitch - m_PitchRange)*Mathf.Abs(m_CurrentSpeedStep), (OriginalPitch + m_PitchRange)*Mathf.Abs(m_CurrentSpeedStep));
                // m_MovementAudio.volume = Random.Range ((OriginalPitch - m_PitchRange)*Mathf.Abs(m_CurrentSpeedStep), (OriginalPitch + m_PitchRange)*Mathf.Abs(m_CurrentSpeedStep));
                // m_MovementAudio.volume = Mathf.Abs(m_CurrentSpeedStep) / 4;
                if (Mathf.Abs(m_CurrentSpeedStep) == 4) {
                    m_MovementAudio.volume = 1;
                } else if(Mathf.Abs(m_CurrentSpeedStep) == 3) {
                    m_MovementAudio.volume = 0.6f;
                } else if(Mathf.Abs(m_CurrentSpeedStep) == 2) {
                    m_MovementAudio.volume = 0.4f;
                } else if(Mathf.Abs(m_CurrentSpeedStep) == 1) {
                    m_MovementAudio.volume = 0.3f;
                }
            } else {
                //if idling audio currently playing
                if (m_MovementAudio.clip == m_EngineIdling) {
                    //switch playing clip
                    m_MovementAudio.clip = m_EngineDriving;
                    //randomize pitch
                    // m_MovementAudio.pitch = Random.Range (OriginalPitch - m_PitchRange, OriginalPitch + m_PitchRange);
                    m_MovementAudio.volume = 0.25f;
                    //play the new audio
                    m_MovementAudio.Play ();
                }
            }
            PreviousSpeedStep = m_CurrentSpeedStep;
            // Debug.Log("m_CurrentSpeedStep : " + Mathf.Abs(m_CurrentSpeedStep) + " - m_MovementAudio.volume : " + m_MovementAudio.volume);
        }
    }

    private void FixedUpdate() {
        if (Active && !MapActive && !Dead) {
            // Debug.Log("Active");
            if (Input.GetAxis ("VerticalShip") == 1){
                ChangeSpeedStep(1);
            } else if(Input.GetAxis ("VerticalShip") == -1){
                ChangeSpeedStep(-1);
            }
            // Debug.Log("TurnInputValue : "+ TurnInputValue +" - RotationIncrementation = "+ RotationIncrementation +" - AllowTurnInput = "+ AllowTurnInput);
            // Determine the number of degrees to be turned based on the input, speed and time between frames.
            if (Input.GetAxis ("HorizontalShip") == 1 && TurnInputValue < 1 && RotationIncrementation && AllowTurnInput){
                // Debug.Log("HorizontalShip+");
                TurnInputValue += 0.5f;
                StartCoroutine(PauseTurnIncrementation());
            } else if(Input.GetAxis ("HorizontalShip") == -1 && TurnInputValue > -1 && RotationIncrementation && AllowTurnInput){
                // Debug.Log("HorizontalShip-");
                TurnInputValue -= 0.5f;
                StartCoroutine(PauseTurnIncrementation());
            }   
        }
        Move ();
        Turn ();
    }

    private void ChangeSpeedStep(int Step) {
        if (Step > 0 && m_CurrentSpeedStep < 4 && OrderSystem) {
            m_CurrentSpeedStep = m_CurrentSpeedStep +1;
            // Debug.Log (m_CurrentSpeedStep+" - m_CurrentSpeedStep -");
            // Those methods are not out of the if/else statement to prevent useless method calls if conditions not set
            SetTargetSpeed();
            StartCoroutine(PauseOrderSystem());
            ShipController.ChangeSpeedStep(m_CurrentSpeedStep);
            EngineAudio();
        }
        else if (Step < 0 && m_CurrentSpeedStep > -4 && OrderSystem) {
            m_CurrentSpeedStep = m_CurrentSpeedStep -1;
            // Debug.Log (m_CurrentSpeedStep+" - m_CurrentSpeedStep -");
            SetTargetSpeed();
            StartCoroutine(PauseOrderSystem());
            ShipController.ChangeSpeedStep(m_CurrentSpeedStep);
            EngineAudio();
        }
    }
    
    private bool OrderSystem = true;
    IEnumerator PauseOrderSystem(){
        OrderSystem = false;
        yield return new WaitForSeconds(1f);
        OrderSystem = true;
    }
    private bool RotationIncrementation = true;
    IEnumerator PauseTurnIncrementation(){
        RotationIncrementation = false;
        yield return new WaitForSeconds(0.5f);
        RotationIncrementation = true;
    }
    private bool RotationIncrementationAI = true;
    IEnumerator PauseAITurnIncrementation(){
        RotationIncrementationAI = false;
        yield return new WaitForSeconds(0.5f);
        RotationIncrementationAI = true;
    }

    private void SetTargetSpeed() {
        float multiplier = 0.25f * m_CurrentSpeedStep;;
        if (m_CurrentSpeedStep == 4) {
            multiplier = 1;
        } else if(m_CurrentSpeedStep == 3) {
            multiplier = 0.5f;
        } else if(m_CurrentSpeedStep == 2) {
            multiplier = 0.3f;
        } else if(m_CurrentSpeedStep == 1) {
            multiplier = 0.1f;
        } else if(m_CurrentSpeedStep == 0) {
            multiplier = 0f;
        } else if(m_CurrentSpeedStep == -1) {
            multiplier = -0.1f;
        } else if(m_CurrentSpeedStep == -2) {
            multiplier = -0.2f;
        } else if(m_CurrentSpeedStep == -3) {
            multiplier = -0.4f;
        } else if(m_CurrentSpeedStep == -4) {
            multiplier = -0.6f;
        }
        if (Dead) {
            LocalTargetSpeed = 0;
        } else if (Damaged) {
            LocalTargetSpeed = m_MaxSpeed * multiplier * DamagedRatio;
        } else {
            LocalTargetSpeed = m_MaxSpeed * multiplier;
        }
        //Debug.Log ("- m_LocalSpeed - :"+ LocalTargetSpeed);
        // if (Active) {
        //     Debug.Log ("- LocalTargetSpeed - :"+ LocalTargetSpeed);
        // }

        foreach (StackComponentSmoke component in m_StackComponents) {
            component.InstantiateMoveSet(Mathf.Abs(multiplier));
        }
    }
    public void SetRealSpeed() {
        if (LocalRealSpeed < LocalTargetSpeed && SpeedIncrementation) {
            if ((LocalRealSpeed+m_SpeedInertia) > LocalTargetSpeed) {
                LocalRealSpeed = LocalTargetSpeed;
            } else {
                LocalRealSpeed += m_SpeedInertia;
            }
            
            if (LocalRealSpeed < 0 && LocalTargetSpeed > 0) {
                LocalRealSpeed += m_SpeedInertia;
                // Debug.Log ("- DOUBLE SPEED FRONT - :"+ LocalRealSpeed);
            }

            StartCoroutine(PauseSpeedIncrementation());
            // Debug.Log ("- FORTH - :"+ LocalRealSpeed);
        }
        else if (LocalRealSpeed > LocalTargetSpeed && SpeedIncrementation) {
            if ((LocalRealSpeed-m_SpeedInertia) < LocalTargetSpeed) {
                LocalRealSpeed = LocalTargetSpeed;
            } else {
                LocalRealSpeed += -m_SpeedInertia;
            }
            if (LocalRealSpeed > 0 && LocalTargetSpeed < 0) {
                LocalRealSpeed += -m_SpeedInertia;
                // Debug.Log ("- DOUBLE SPEED BACK - :"+ LocalRealSpeed);
            }
            StartCoroutine(PauseSpeedIncrementation());
            // Debug.Log ("- BACK - :"+ LocalRealSpeed);
        }
    }
    private bool SpeedIncrementation = true;
    IEnumerator PauseSpeedIncrementation(){
        SpeedIncrementation = false;
        yield return new WaitForSeconds(0.5f);
        SpeedIncrementation = true;
    }

    private void Move() {
        SetRealSpeed();
        ShipController.SetSpeedInput(LocalRealSpeed);
    }

    private void Turn() {
        // Multiply the targeted rotation by the speed : reverts input when in reverse and prevents spinning stopped ships 
        LocalRealRotation = TurnInputValue * LocalRealSpeed;
        // Debug.Log ("- TurnInputValue - :"+ TurnInputValue);
        ShipController.SetRotationInput(LocalRealRotation);
    }

    public void SetDamaged(float proportion){
        if (proportion == 1){
            Damaged = false;
        } else{
            Damaged = true;
            DamagedRatio = proportion;
        }
        SetTargetSpeed();
        // if (Active) {
            // Debug.Log ("- Engine Damaged - :"+ Damaged);
        // }
    }
    public void SetActive(bool activate) { Active = activate; }
    public void SetDead(bool death) {
        Dead = death;
        SetTargetSpeed();
        if (Dead) {
            m_MovementAudio.Stop ();
            foreach (StackComponentSmoke component in m_StackComponents) {
                // component.InstantiateIdleStop();
                component.StackInstanceStop();
            }
        } else {
            m_MovementAudio.Play ();
            foreach (StackComponentSmoke component in m_StackComponents) {
                // component.InstantiateIdleStart();
                component.StackInstanceStart();
            }
        }
        // if (Active) {
        //     Debug.Log ("- Engine Dead - :"+ Dead);
        // }
    }
    public void SetAllowTurnInputChange(bool allow) { AllowTurnInput = allow; }

    public void SetMap(bool map) {
        MapActive = map;
    }
    public int GetCurrentSpeedStep(){ return m_CurrentSpeedStep; }
    public float GetLocalRealSpeed(){ return LocalRealSpeed; }
    public void SetAISpeed(int speedStep) {
        m_CurrentSpeedStep = speedStep;
        // Debug.Log("m_CurrentSpeedStep : "+ m_CurrentSpeedStep);
        SetTargetSpeed();
        StartCoroutine(PauseOrderSystem());
        ShipController.ChangeSpeedStep(m_CurrentSpeedStep);
        EngineAudio();
    }
    public void SetAIturn(float turn) {
        if (turn == 0.5f && TurnInputValue < 1 && RotationIncrementationAI && AllowTurnInput && !Dead){
            TurnInputValue += 0.5f;
            StartCoroutine(PauseAITurnIncrementation());
        } else if(turn  == -0.5f && TurnInputValue > -1 && RotationIncrementationAI && AllowTurnInput && !Dead){
            TurnInputValue -= 0.5f;
            StartCoroutine(PauseAITurnIncrementation());
        } else if (turn == 0 && RotationIncrementationAI && AllowTurnInput && !Dead){
            TurnInputValue = 0;
            StartCoroutine(PauseAITurnIncrementation());
        }
        ShipController.SetAITurnInputValue(TurnInputValue);
    }
}