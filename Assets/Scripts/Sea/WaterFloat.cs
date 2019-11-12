using Waterphysics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WaterFloat : MonoBehaviour
{
    //public properties
    public float AirDrag = 1;
    public float WaterDrag = 10;
    public bool AffectDirection = true;
    public bool AttachToSurface = false;
    // public Transform[] FloatPoints;

    [HideInInspector] public List<GameObject> m_BuoyancyPoints;		// List of points used for buoyancy. A list allows to add/remove elements later on.
	private GameObject m_Floats;
    // public string PointName = "BuoyancyFR";

    //used components
    protected Rigidbody Rigidbody;
    protected Waves Waves;

    //water line
    protected float WaterLine;
    protected Vector3[] WaterLinePoints;

    //help Vectors
    protected Vector3 smoothVectorRotation;
    protected Vector3 TargetUp;
    protected Vector3 centerOffset;

    public Vector3 Center { get { return transform.position + centerOffset; } }

    // Start is called before the first frame update
    void Awake()
    {
        //get components
        Waves = FindObjectOfType<Waves>();
        Rigidbody = GetComponent<Rigidbody>();
        m_Floats = transform.Find("Floats").gameObject;
		GetAllPossiblePoints();
        Rigidbody.useGravity = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //default water surface
        var newWaterLine = 0f;
        var pointUnderWater = false;

        //set WaterLinePoints and WaterLine
        for (int i = 0; i < m_BuoyancyPoints.Count; i++)
        {
            //height
            WaterLinePoints[i] = m_BuoyancyPoints[i].transform.position;
            WaterLinePoints[i].y = Waves.GetHeight(m_BuoyancyPoints[i].transform.position);
            newWaterLine += WaterLinePoints[i].y / m_BuoyancyPoints.Count;
            if (WaterLinePoints[i].y > m_BuoyancyPoints[i].transform.position.y)
                pointUnderWater = true;
        }

        var waterLineDelta = newWaterLine - WaterLine;
        WaterLine = newWaterLine;

        //compute up vector
        TargetUp = PhysicsHelper.GetNormal(WaterLinePoints);

        //gravity
        var gravity = Physics.gravity;
        Rigidbody.drag = AirDrag;
        if (WaterLine > Center.y)
        {
            Rigidbody.drag = WaterDrag;
            //under water
            if (AttachToSurface)
            {
                //attach to water surface
                Rigidbody.position = new Vector3(Rigidbody.position.x, WaterLine - centerOffset.y, Rigidbody.position.z);
            }
            else
            {
                //go up
                gravity = AffectDirection ? TargetUp * -Physics.gravity.y : -Physics.gravity;
                transform.Translate(Vector3.up * waterLineDelta * 0.9f);
            }
        }
        Rigidbody.AddForce(gravity * Mathf.Clamp(Mathf.Abs(WaterLine - Center.y),0,1));

        //rotation
        if (pointUnderWater)
        {
            //attach to water surface
            TargetUp = Vector3.SmoothDamp(transform.up, TargetUp, ref smoothVectorRotation, 0.2f);
            Rigidbody.rotation = Quaternion.FromToRotation(transform.up, TargetUp) * Rigidbody.rotation;
        }

    }

	private void GetAllPossiblePoints() {
		// Debug.Log ("GetAllPossiblePoints :"+ m_Floats.transform.childCount);

		for (int i = 0; i < m_Floats.transform.childCount; i++) {
			
			if(m_Floats.transform.GetChild(i).gameObject.name.Contains("Buoyancy")){
				m_BuoyancyPoints.Add( m_Floats.transform.GetChild(i).gameObject );
			}
		}
        // Debug.Log ("buoyancyPoints :"+ m_BuoyancyPoints.Count);
                //compute center
        WaterLinePoints = new Vector3[m_BuoyancyPoints.Count];
        for (int i = 0; i < m_BuoyancyPoints.Count; i++)
            WaterLinePoints[i] = m_BuoyancyPoints[i].transform.position;
        centerOffset = PhysicsHelper.GetCenter(WaterLinePoints) - transform.position;

	}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (m_BuoyancyPoints.Count == 0)
            return;

        for (int i = 0; i < m_BuoyancyPoints.Count; i++)
        {
            // Debug.Log ("buoyancyPoints :"+ m_BuoyancyPoints[i]);
            if (m_BuoyancyPoints[i] == null)
                continue;

            if (Waves != null)
            {

                //draw cube
                Gizmos.color = Color.red;
                Gizmos.DrawCube(WaterLinePoints[i], Vector3.one * 0.3f);
            }

            //draw sphere
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(m_BuoyancyPoints[i].transform.position, 0.1f);

        }

        //draw center
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(new Vector3(Center.x, WaterLine, Center.z), Vector3.one * 1f);
            Gizmos.DrawRay(new Vector3(Center.x, WaterLine, Center.z), TargetUp * 1f);
        }
    }


    // Next functions can be used to remove a single point from the buoyancy
    /*
	protected void Update() {
		if (Input.GetButtonDown ("SetNextUnit")){
                // RemoveOnePoint(PointName);
                SinkABit();
        }
	}
    /*
    private void RemoveOnePoint(string PointName) 
		Debug.Log ("RemoveOnePoint");
		for (int i = 0; i < m_BuoyancyPoints.Count; i++) {
			if(m_Floats.transform.GetChild(i).gameObject.name.Contains(PointName)){
				m_BuoyancyPoints.Remove( m_Floats.transform.GetChild(i).gameObject );
			}
		}
	}
    //changes the positions correctly, but doesn't update the model position !
    private void SinkABit() {
		Debug.Log ("SinkABit");
        float x = -40;
        float z = 40;
        Debug.Log (m_Floats.transform.localRotation);
        m_Floats.transform.localRotation = Quaternion.Euler(x, 0, z);
	}
    */
}