using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour {
    private bool MapActive = false;
    private Camera MapCamera;
    private PlayerManager PlayerManager;
    // private GameObject SeaMap;
    private Canvas PlayerCanvas;
    private Canvas PlayerMapCanvas;

    private int InitialSize = 2000;
    private int MaxSize = 3000;
    private int MinSize = 300;

    private void Start() {
        // SeaMap = GameObject.Find("MapSea");
        PlayerCanvas = GameObject.Find("UICanvas").GetComponent<Canvas>();
        PlayerMapCanvas = GameObject.Find("UIMapCanvas").GetComponent<Canvas>();
        PlayerMapCanvas.enabled = false;
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
            if (Input.GetAxis("Mouse ScrollWheel") != 0f ) {
                MapCamera.orthographicSize = Mathf.Clamp(MapCamera.orthographicSize - Input.GetAxis("Mouse ScrollWheel"), MinSize, MaxSize);
            }

            // targetPosition.y = 0;
            // SeaMap.transform.position = targetPosition;
        }
    }

    public void SetPlayedUnit(GameObject ActiveTarget){
        Vector3 targetPosition = MapCamera.transform.position;
        targetPosition.x = ActiveTarget.transform.position.x;
        targetPosition.z = ActiveTarget.transform.position.z;
        MapCamera.transform.position = targetPosition;
    }
    public void SetMapCamera(Camera camera){ MapCamera = camera; }
    public void SetInitialPosition(GameObject target) {
        Vector3 targetPosition = target.transform.position;
        MapCamera.orthographicSize = InitialSize;
        targetPosition.y = 2000;
        MapCamera.transform.position = targetPosition;
    }
    public void SetMap(bool active) {
        MapActive = active;
        if (MapActive) {
            InitMap();
        }
        PlayerCanvas.enabled = !MapActive;
        PlayerMapCanvas.enabled = MapActive;
    }
}