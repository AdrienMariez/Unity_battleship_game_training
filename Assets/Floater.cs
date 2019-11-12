using UnityEngine;
using System.Collections;
 using System.Collections.Generic;

public class Floater : MonoBehaviour {
	public float waterLevel, floatHeight;
	public Vector3 buoyancyCentreOffset;
	public float bounceDamp;

	public List<GameObject> buoyancyPoints;

	public string PointName;
	// public string PointName = "BuoyancyPoint1";


         
	private void Start()
	{
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
		for (var i = 0; i < buoyancyPoints.Count; i++) {
			Vector3 actionPoint = buoyancyPoints[i].transform.position;
			float forceFactor = (1f - ((actionPoint.y - waterLevel) / floatHeight) / buoyancyPoints.Count);
		
			if (forceFactor > 0f) {
				Vector3 uplift = -Physics.gravity * (forceFactor -  GetComponent<Rigidbody>().velocity.y * ((bounceDamp / buoyancyPoints.Count) * Time.deltaTime));
				GetComponent<Rigidbody>().AddForceAtPosition(uplift, actionPoint);
			}
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
