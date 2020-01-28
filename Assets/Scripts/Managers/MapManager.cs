using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour {
    private bool MapActive = false;
    private Camera MapCamera;
    private PlayerManager PlayerManager;
    private GameObject SeaMap;

    private void Start() {
        SeaMap = GameObject.Find("MapSea");
    }

    private void InitMap() {

    }

    protected void Update() {
        if (MapActive) {
            Vector3 targetPosition = MapCamera.transform.position;
            if (Input.GetAxis ("HorizontalMap") == 1) {
                targetPosition.x += 20;
            } else if (Input.GetAxis ("HorizontalMap") == -1) {
                targetPosition.x += -20;
            }
            if (Input.GetAxis ("VerticalMap") == 1) {
                targetPosition.z += 20;
            } else if (Input.GetAxis ("VerticalMap") == -1) {
                targetPosition.z += -20;
            }
            MapCamera.transform.position = targetPosition;

            targetPosition.y = 0;
            SeaMap.transform.position = targetPosition;
        }
    }

    public void SetMapCamera(Camera camera){ MapCamera = camera; }
    public void SetInitialPosition(GameObject target) {
        Vector3 targetPosition = target.transform.position;
        targetPosition.y = 2000;
        MapCamera.transform.position = targetPosition;
    }
    public void SetMap(bool active) {
        MapActive = active;
        if (MapActive) {
            InitMap();
        }
    }
}