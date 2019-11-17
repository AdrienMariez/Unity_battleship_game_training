using UnityEngine;

public class TurretRotation : MonoBehaviour
{    
    [HideInInspector] public bool m_Active;
    public AudioClip m_TurretRotationAudio;
    public Rigidbody TankTurret;
    public Rigidbody TankCannon;

    [Tooltip ("Maximum Rotation Speed. (Degree per Second)")] public float rotationSpeed = 15.0f;
	[Tooltip ("Time to reach the maximum speed. (Sec)")] public float acceleration_Time = 0.2f;
	[Tooltip ("Angle range for slowing down. (Degree)")] public float bufferAngle = 5.0f;
    private float speedRate;
    [HideInInspector] public Vector3 unitEulerAngles;
    private float targetAng;
    private float currentAng;
    private GameObject CameraPivot;


    private void Awake(){
        CameraPivot = GameObject.Find("CameraPivot");
    }

    /*Those method are not used but could be to allow to disable all turrets of a unit if needed
    private void OnEnable () { 
    }
    private void OnDisable () {
    }*/


    private void TurretAudio()
    {
        // TODO play turret rotation audio if the turret axis is moving
        /*

        //Abs (x) means that if x=-3, Abs(x)=3
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
            if (m_MovementAudio.clip == m_EngineIdling){
                //switch playing clip
                m_MovementAudio.clip = m_EngineDriving;
                //randomize pitch
                m_MovementAudio.pitch = Random.Range (m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                //play the new audio
                m_MovementAudio.Play ();
            }
        }
        */
    }


    private void FixedUpdate()
    {
        // This is run every physics step instead of every frame
        // Move and turn the tank.

        if (m_Active){
            if (!Input.GetButton ("FreeCamera"))
            {
                TurretRotate();
                CannonElevation();
            }
        }
        
    }

    private void TurretRotate()
    {

        // Get Camera rotation
        Vector3 cameraEulerAngles = CameraPivot.transform.rotation.eulerAngles;
        // Debug.Log("cameraEulerAngles: " + cameraEulerAngles);

        targetAng = cameraEulerAngles.y-unitEulerAngles.y;
        if (targetAng<0)
            targetAng += 360;
        currentAng = TankTurret.transform.localRotation.eulerAngles.y;
        // Debug.Log("targetAng = "+targetAng+" /:/ currentAng = "+currentAng);
        
         // Calculate Turn Rate.
        float targetSpeedRate = Mathf.Lerp (0.0f, 1.0f, Mathf.Abs (targetAng) / (rotationSpeed * Time.fixedDeltaTime + bufferAngle)) * Mathf.Sign (targetAng);
        // Calculate Rate
        speedRate = Mathf.MoveTowardsAngle (speedRate, targetSpeedRate, Time.fixedDeltaTime / acceleration_Time);
        // Rotate
        
        // if (Mathf.Abs (targetAng) > 0.01f) {
        if (Mathf.Abs (currentAng) < Mathf.Abs (targetAng) && Mathf.Abs (currentAng)+180 > Mathf.Abs (targetAng) || Mathf.Abs (currentAng) > Mathf.Abs (targetAng) && Mathf.Abs (currentAng) > Mathf.Abs (targetAng)+180) {
            // Debug.Log("IF IF IF IF IF");
            currentAng += rotationSpeed * speedRate * Time.fixedDeltaTime;
        }
        else{
            // Debug.Log("ELSELSELSE");
            currentAng -= rotationSpeed * speedRate * Time.fixedDeltaTime;
        }
        TankTurret.transform.localRotation = Quaternion.Euler (new Vector3 (0.0f, currentAng, 0.0f));
    }

    private void CannonElevation()
    {
        // Get Camera rotation
        Vector3 cameraEulerAngles = CameraPivot.transform.rotation.eulerAngles;
        // Debug.Log("cameraEulerAngles: " + cameraEulerAngles);

        targetAng = cameraEulerAngles.x-unitEulerAngles.x;
        if (targetAng<0)
            targetAng += 360;

        // Turn turret towards camera facing
        TankCannon.transform.localRotation = Quaternion.Euler(targetAng, 0.0f, 0.0f);
        // Debug.Log(targetAng);
    }

    float AngleBetweenTwoPoints(Vector3 a, Vector3 b) {
         return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }
}