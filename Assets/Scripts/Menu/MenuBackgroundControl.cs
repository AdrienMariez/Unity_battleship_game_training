using UnityEngine;
using System.Collections.Generic;

public class MenuBackgroundControl : MonoBehaviour {

    [Header("Units in the background")]
        public GameObject[] m_ShipSpawnPoints;
        private List<GameObject> MenuUnits = new List<GameObject>();
    [Header("Camera")]
        public GameObject m_CameraAxis;
        private Camera Camera;
        private float Speed;

    void Start() {
        LoadMenuUnits();
        Camera = m_CameraAxis.GetComponentInChildren<Camera>();
        Speed = 1 * Time.deltaTime;
    }

    protected void LoadMenuUnits() {    
        // Create ship list
        List<WorldSingleUnit> spawnableShipList = new List<WorldSingleUnit>();
        foreach (List<WorldSingleUnit> category in WorldUnitsManager.GetUnitsByCategory()) {
            foreach (WorldSingleUnit unit in category) {
                // Debug.Log (" unit "+unit.GetUnitName());
                if (unit.GetUnitCategory_DB().id == WorldUnitsManager.GetDB().Units_categories.ship.id) {
                    spawnableShipList.Add(unit);
                } else {
                    break;
                }
            }
        }

        // spawn random ships for respective spawnpoints
        foreach (GameObject spawnPoint in m_ShipSpawnPoints) {
            int randomUnitId = Random.Range(0, spawnableShipList.Count);
            // Debug.Log (randomUnitId+" / "+spawnableShipList.Count);
            // Debug.Log (" Spawning "+spawnableShipList[randomUnitId].GetUnitName());
            GameObject instance = WorldUnitsManager.BuildUnit(spawnableShipList[randomUnitId], spawnPoint.transform.position, spawnPoint.transform.rotation, false, false, false);
            MenuUnits.Add(instance);
        }

        // Remove all map elements (so it doesn't show on the mission editor since it's the same scene)

        foreach (GameObject unit in MenuUnits) {
            GameObject mapInstance = unit.GetComponent<UnitMasterController>().GetMapModel();
            Destroy (mapInstance);
        }
    }
    protected void Update() {
        m_CameraAxis.transform.Rotate(0.0f, Speed, 0.0f, Space.Self);
    }
}