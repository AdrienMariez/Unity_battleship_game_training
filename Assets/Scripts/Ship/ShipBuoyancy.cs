using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipBuoyancy : MonoBehaviour {
	/* Source :
	 https://forum.unity.com/threads/floating-a-object-on-water.31671
	Basics for RigidBody :
	Mass : 7
	Drag : 5
	Angular Drag : 0.4
	*/
	[HideInInspector] private float waterLevel = 0f;				// This sets the level of water, for the game, it will always be 0 !
	[SerializeField] private float floatHeight = 0.7f;				// Compensates the Mass of the RigidBody.
	public float bounceDamp = 5f;										//The higher, the less the ship wobbles left and right. See RigidBody AngularDrag.
	[HideInInspector] public List<GameObject> buoyancyPoints;		// List of points used for buoyancy. A list allows to add/remove elements later on.
	private GameObject m_Floats;

	// [HideInInspector] public string PointName;						//string to remove points in the game.
	// public string PointName = "BuoyancyFR";


         
	private void Start() {
		m_Floats = transform.Find("Floats").gameObject;
		GetAllPossiblePoints();
	}

	private void GetAllPossiblePoints() {
		// Debug.Log ("GetAllPossiblePoints :"+ m_Floats.transform.childCount);

		for (int i = 0; i < m_Floats.transform.childCount; i++) {
			
			if(m_Floats.transform.GetChild(i).gameObject.name.Contains("Buoyancy")){
				buoyancyPoints.Add( m_Floats.transform.GetChild(i).gameObject );
			}
		}
	}

	protected void Update()
	{
		// Testing stuff
		// if (Input.GetButtonDown ("SetNextUnit")){
        //         RemoveOnePoint(PointName);
        // }
	}

	void FixedUpdate () {
		// Single point system (useless)
		// Vector3 actionPoint = transform.position + transform.TransformDirection(buoyancyCentreOffset);
		// float forceFactor = 1f - ((actionPoint.y - waterLevel) / floatHeight);
		
		// if (forceFactor > 0f) {
		// 	Vector3 uplift = -Physics.gravity * (forceFactor - GetComponent<Rigidbody>().velocity.y * bounceDamp);
		// 	GetComponent<Rigidbody>().AddForceAtPosition(uplift, actionPoint);
		// }

		// Multi point system
		/*
		for (var i = 0; i < buoyancyPoints.Count; i++) {
			Vector3 actionPoint = buoyancyPoints[i].transform.position;
			float forceFactor = (1f - ((actionPoint.y - waterLevel) / floatHeight) / buoyancyPoints.Count);
		
			if (forceFactor > 0f) {
				Vector3 uplift = -Physics.gravity * (forceFactor -  GetComponent<Rigidbody>().velocity.y * ((bounceDamp / buoyancyPoints.Count) * Time.deltaTime));
				GetComponent<Rigidbody>().AddForceAtPosition(uplift, actionPoint);
			}
		}
		*/

		for (var i = 0; i < buoyancyPoints.Count; i++) {
			Vector3 actionPoint = buoyancyPoints[i].transform.position;
			float forceFactor = (1f - (actionPoint.y - waterLevel) / floatHeight);
		
			if (forceFactor > 0f) {
				Vector3 uplift = -Physics.gravity * (forceFactor -  GetComponent<Rigidbody>().velocity.y * (bounceDamp * Time.deltaTime));
				GetComponent<Rigidbody>().AddForceAtPosition(uplift, actionPoint);
			}
		}
	}

	// private void RemoveOnePoint(string PointName)
	// {
	// 	Debug.Log ("RemoveOnePoint");
	// 	for (int i = 0; i < buoyancyPoints.Count; i++) {
	// 		if(m_Floats.transform.GetChild(i).gameObject.name.Contains(PointName)){
	// 			buoyancyPoints.Remove( m_Floats.transform.GetChild(i).gameObject );
	// 		}
	// 	}
	// }
}

/*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
	
public class ShipBuoyancy : MonoBehaviour {
	//[SerializeField] private Transform[] m_BuoyancyPoints;
	[SerializeField] private float m_BuoyancyStrength = 1f;
	[SerializeField] private float m_BobbingDamp = 0.5f;

	[HideInInspector] public List<GameObject> buoyancyPoints;

	[HideInInspector] public string PointName;
	
	private Rigidbody m_ThisRigidbody;
	private bool m_InWater = false;
	private float m_WaterLevel;
	
	
	private void Start() {
		m_ThisRigidbody = GetComponent<Rigidbody>();
		GetAllPossiblePoints();
	}

	private void GetAllPossiblePoints()
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			if(transform.GetChild(i).gameObject.name.Contains("Buoyancy")){
				buoyancyPoints.Add( transform.GetChild(i).gameObject );
			}
		}
	}
	
	private void FixedUpdate () {
		if (m_InWater) {
			for (int i = 0; i < buoyancyPoints.Count; i++) {
				Vector3 actionPoint = buoyancyPoints[i].transform.position;
				float forceFactor = 1f - (actionPoint.y - m_WaterLevel) * m_BuoyancyStrength;
	
				if (forceFactor > 0f) {
					Vector3 uplift = -Physics.gravity * (forceFactor - m_ThisRigidbody.velocity.y) / buoyancyPoints.Count;
					m_ThisRigidbody.AddForceAtPosition(uplift, actionPoint);
				}
			}
		}
	}
	
	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Water")) {
			Vector3 waterPosition = other.transform.position;
			float waterHeightExtent = other.GetComponent<MeshRenderer>().bounds.extents.y;
			m_WaterLevel = waterPosition.y + waterHeightExtent;
	
			m_ThisRigidbody.angularDrag = m_BobbingDamp;
	
			m_InWater = true;
		}
	}
	
	private void OnTriggerExit(Collider other) {
		if (other.CompareTag("Water")) {
			m_ThisRigidbody.angularDrag = 0.05f;
	
			m_InWater = false;
		}
	}

	private void RemoveOnePoint(string PointName)
	{
		Debug.Log ("RemoveOnePoint");
		for (int i = 0; i < buoyancyPoints.Count; i++) {
			if(transform.GetChild(i).gameObject.name.Contains(PointName)){
				buoyancyPoints.Remove( transform.GetChild(i).gameObject );
			}
		}
	}
}
*/