using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipMovement : MonoBehaviour
{
    public int m_PlayerNumber = 1;         
    [HideInInspector] public bool m_Active; 
    public float m_MaxSpeed = 12f;            
    public float m_TurnSpeed = 180f;       
    public AudioSource m_MovementAudio;    
    public AudioClip m_EngineIdling;       
    public AudioClip m_EngineDriving;      
    public float m_PitchRange = 0.2f;       // The amount by which the pitch of the engine noises

    
    private string m_MovementAxisName;     
    private string m_TurnAxisName;         
    private Rigidbody m_Rigidbody;         // Reference used to move the tank.
    private float m_MovementInputValue;    // The current value of the movement input.
    private float m_TurnInputValue;        
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
    [HideInInspector] private bool m_SpeedIncrementation = true;// Used to allow the m_SpeedInertia to take some time.
    [HideInInspector] public float m_LocalRealSpeed;            // The real final speed of the ship.


    // private Rigidbody TankTurret;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();

        //TankTurret = this.transform.GetChild(0).GetChild(3).GetComponent<Rigidbody>();
    }


    private void OnEnable ()
    {
        // When the tank is turned on, make sure it's not kinematic.
        m_Rigidbody.isKinematic = false;

        // Also reset the input values.
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
        SetTargetSpeed();
        m_LocalRealSpeed = m_LocalTargetSpeed;
    }


    private void OnDisable ()
    {
        // When the tank is turned off, set it to kinematic so it stops moving.
        m_Rigidbody.isKinematic = true;
    }


    private void Start()
    {
        // The axes names are based on player number.

        //m_MovementAxisName = "Vertical" + m_PlayerNumber;
        //m_TurnAxisName = "Horizontal" + m_PlayerNumber;

        // Store the original pitch of the audio source.
        m_OriginalPitch = m_MovementAudio.pitch;
    }

    private void Update()
    {
        // This is run every frame
        // Store the player's input and make sure the audio for the engine is playing.

        if (m_Active)
        {
            m_MovementInputValue = Input.GetAxis ("VerticalShip");
            m_TurnInputValue = Input.GetAxis ("HorizontalShip");
        }
        else
        {
            m_MovementInputValue = Input.GetAxis ("Empty");
            m_TurnInputValue = Input.GetAxis ("Empty");
        }
        EngineAudio ();

        if (transform.localPosition.y < 1){
            m_Rigidbody.drag = 5;
        }
        else{
            m_Rigidbody.drag = 0;
        }
    }


    private void EngineAudio()
    {
        // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.

        //Abs (x) means that if x=-3, Abs(x)=3
        //0.1f means 0.1 as a FLOAT value
        //if not moving or rotating
        if (Mathf.Abs (m_MovementInputValue) < 0.1f && Mathf.Abs (m_TurnInputValue) < 0.1f)
        {
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
        }
        else
        {
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


    private void FixedUpdate()
    {
        // This is run every physics step instead of every frame
        // Move and turn the tank.
        Move ();
        Turn ();
        if (m_Active)
        {
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

            // Debug.Log ("- FORTH - :"+ m_LocalRealSpeed);

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
            // Debug.Log ("- BACK - :"+ m_LocalRealSpeed);
        }
    }
    IEnumerator PauseSpeedIncrementation(){
        m_SpeedIncrementation = false;
        yield return new WaitForSeconds(0.5f);
        m_SpeedIncrementation = true;
    }

    private void Move()
    {
        SetRealSpeed();
        // Create a vector in the direction the tank is facing with a magnitude based on the input, speed and the time between frames.
        // vector in tank's forward direction * input received * m_Speed multiplier and proportionate this by seconds instead of frame.
        //Vector3 movement = transform.forward * m_MovementInputValue * m_LocalSpeed * Time.deltaTime;

        Vector3 movement = transform.forward * m_LocalRealSpeed * Time.deltaTime;

        //update current rigidbody position with new values
        m_Rigidbody.MovePosition (m_Rigidbody.position + movement);
    }

    private void Turn()
    {
        // Determine the number of degrees to be turned based on the input, speed and time between frames.
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

        //Quaternion : way for Unity to stock rotation
        //(0f, turn, 0f) : X,Y,Z
        Quaternion turnRotation = Quaternion.Euler (0f, turn, 0f);
        m_Rigidbody.MoveRotation (m_Rigidbody.rotation * turnRotation);
    }

    //     private void TurretRotate()
    // {
    //     // Get vehicle rotation
    //     Vector3 tankEulerAngles = this.transform.rotation.eulerAngles;
    //     //Debug.Log("transform.rotation angles x: " + eulerAngles.x + " y: " + eulerAngles.y + " z: " + eulerAngles.z);

    //     // Get Camera rotation
    //     Vector3 cameraEulerAngles = GameObject.Find("Pivot").transform.rotation.eulerAngles;

    //     // Turn turret towards camera facing
    //     TankTurret.transform.rotation = Quaternion.Euler(tankEulerAngles.x, cameraEulerAngles.y, tankEulerAngles.z);    
    // }

    float AngleBetweenTwoPoints(Vector3 a, Vector3 b) {
         return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }
}