using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour {
    private bool MapActive = false;
    private Camera MapCamera; public void SetMapCamera(Camera camera){ MapCamera = camera; }
    private GameManager GameManager;
    private PlayerManager PlayerManager;
    // private GameObject SeaMap;
    private Canvas PlayerCanvas;
    private Canvas PlayerMapCanvas;

    // private int InitialSpeed = 20;
    private float CurrentSpeed;
    private float InitialSize = 2000;
    private float CurrentSize;
    private float MaxSize = 3000;
    private float MinSize = 300;

    public void InitMapFromPlayerManager(GameManager gameManager) {
        GameManager = gameManager;
        MaxSize = GameManager.GetMapCameraMaxSize();
        PlayerCanvas = GameObject.Find("UICanvas").GetComponent<Canvas>();
        PlayerMapCanvas = GameObject.Find("UIMapCanvas").GetComponent<Canvas>();
        PlayerMapCanvas.enabled = false;
        CurrentSize = InitialSize;
        MapCamera.orthographicSize = CurrentSize;
        CheckCameraSpeed();
    }

    protected void Update() {
        if (MapActive) {
            Vector3 cameraPosition = MapCamera.transform.position;
            bool _positionChanged = false;
            if (Input.GetAxis ("VerticalMap") == 1) {
                // (CurrentSize/2)
                // Debug.Log ("case 1");
                if (cameraPosition.z + CurrentSize < GameManager.GetMapCameraPosTop()) {
                    cameraPosition.z += CurrentSpeed;
                    _positionChanged = true;
                }
            } else if (Input.GetAxis ("VerticalMap") == -1) {
                // Debug.Log ("case 2");
                if (cameraPosition.z - CurrentSize > GameManager.GetMapCameraPosBottom()) {
                    cameraPosition.z -= CurrentSpeed;
                    _positionChanged = true;
                }
            }
            if (Input.GetAxis ("HorizontalMap") == 1) {
                // Debug.Log ("case 1");
                if (cameraPosition.x + CurrentSize < GameManager.GetCameraPosRight()) {
                    cameraPosition.x += CurrentSpeed;
                    _positionChanged = true;
                }
            } else if (Input.GetAxis ("HorizontalMap") == -1) {
                // Debug.Log ("case 2");
                if (cameraPosition.x - CurrentSize > GameManager.GetMapCameraPosLeft()) {
                    cameraPosition.x -= CurrentSpeed;
                    _positionChanged = true;
                }
            }
            if (_positionChanged == true) {
                MapCamera.transform.position = cameraPosition;
            }
            if (Input.GetAxis("Mouse ScrollWheel") != 0f ) {
                CurrentSize = Mathf.Clamp(MapCamera.orthographicSize - Input.GetAxis("Mouse ScrollWheel"), MinSize, MaxSize);
                CheckPositionLimits(cameraPosition);
                MapCamera.orthographicSize = CurrentSize;
                CheckCameraSpeed();
            }
        }
    }

    private void CheckPositionLimits(Vector3 cameraPosition ){
        bool _positionChanged = false;
        if (cameraPosition.z + CurrentSize > GameManager.GetMapCameraPosTop()) {
            // Debug.Log ("bump top");
            cameraPosition.z = GameManager.GetMapCameraPosTop()-CurrentSize;
            _positionChanged = true;
        }

        if (cameraPosition.z - CurrentSize < GameManager.GetMapCameraPosBottom()) {
            // Debug.Log ("bump bottom");
            cameraPosition.z = GameManager.GetMapCameraPosBottom()+CurrentSize;
            _positionChanged = true;
        }

        if (cameraPosition.x + CurrentSize > GameManager.GetCameraPosRight()) {
            // Debug.Log ("bump right");
            cameraPosition.x = GameManager.GetCameraPosRight()-CurrentSize;
            _positionChanged = true;
        }

        if (cameraPosition.x - CurrentSize < GameManager.GetMapCameraPosLeft()) {
            // Debug.Log ("bump left");
            cameraPosition.x = GameManager.GetMapCameraPosLeft()+CurrentSize;
            _positionChanged = true;
        }

        if (_positionChanged == true) {
            MapCamera.transform.position = cameraPosition;
        }
    }

    private void CheckCameraSpeed(){
        CurrentSpeed = CurrentSize / 100;
    }

    public void MoveCameraToUnit(Transform unitTarget){
        Vector3 cameraPosition = MapCamera.transform.position;
        cameraPosition.x = unitTarget.position.x;
        cameraPosition.z = unitTarget.position.z;
        MapCamera.transform.position = cameraPosition;
        CheckPositionLimits(cameraPosition);
    }
    public void ResetCameraPositionToUnit(Transform unitTarget) {
        CurrentSize = InitialSize;
        MapCamera.orthographicSize = CurrentSize;
        MoveCameraToUnit(unitTarget);
    }

    public void SetMap(bool active) {
        MapActive = active;
        PlayerCanvas.enabled = !MapActive;
        PlayerMapCanvas.enabled = MapActive;
    }
}