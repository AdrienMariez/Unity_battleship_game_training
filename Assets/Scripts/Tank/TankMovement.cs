using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public int m_PlayerNumber = 1;         
    [HideInInspector] public bool m_Active; 
    public float m_Speed = 12f;            
    public float m_TurnSpeed = 180f;       
    public AudioSource m_MovementAudio;    
    public AudioClip m_EngineIdling;       
    public AudioClip m_EngineDriving;      
    public float m_PitchRange = 0.2f;       // The amount by which the pitch of the engine noises

    [Tooltip ("Maximum Rotation Speed. (Degree per Second)")] public float rotationSpeed = 15.0f;
	[Tooltip ("Time to reach the maximum speed. (Sec)")] public float acceleration_Time = 0.2f;
	[Tooltip ("Angle range for slowing down. (Degree)")] public float bufferAngle = 5.0f;
    float speedRate;
    float targetAng;
    float currentAng;

    
    private string m_MovementAxisName;     
    private string m_TurnAxisName;         
    private Rigidbody m_Rigidbody;         // Reference used to move the tank.
    private float m_MovementInputValue;    // The current value of the movement input.
    private float m_TurnInputValue;        
    private float m_FreeCamInputValue;      
    private float m_OriginalPitch;         // The pitch of the audio source at the start of the scene.

    TurretFireManager TurretsFire;
    TurretRotation TurretsRotation;

    private GameObject CameraPivot;


    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        CameraPivot = GameObject.Find("CameraPivot");
    }


    private void OnEnable ()
    {
        // When the tank is turned on, make sure it's not kinematic.
        m_Rigidbody.isKinematic = false;

        // Also reset the input values.
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }


    private void OnDisable ()
    {
        // When the tank is turned off, set it to kinematic so it stops moving.
        m_Rigidbody.isKinematic = true;
    }


    private void Start()
    {
        // Store the original pitch of the audio source.
        m_OriginalPitch = m_MovementAudio.pitch;
    }

    private void Update()
    {
        // This is run every frame
        // Store the player's input and make sure the audio for the engine is playing.
        if (m_Active){
            m_MovementInputValue = Input.GetAxis ("VerticalGround");
            m_TurnInputValue = Input.GetAxis ("HorizontalGround");
        }
        else{
            m_MovementInputValue = Input.GetAxis ("Empty");
            m_TurnInputValue = Input.GetAxis ("Empty");
        }
        EngineAudio ();
    }


    private void EngineAudio()
    {
        // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.

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

        if (m_Active)
        {
            Move ();
            Turn ();
        }
        
    }


    private void Move()
    {
        // Create a vector in the direction the tank is facing with a magnitude based on the input, speed and the time between frames.
        // vector in tank's forward direction * input received * m_Speed multiplier and proportionnate this by seconds instead of frame.
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;

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

    float AngleBetweenTwoPoints(Vector3 a, Vector3 b) {
         return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }
}