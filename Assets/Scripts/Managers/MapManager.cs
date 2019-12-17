using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    public Camera m_MapCamera;
    private bool MapStatus;
    private bool _MapStatus;
    private PlayerManager PlayerManager;

    private void Start() {
        MapStatus = false;
        _MapStatus = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        PlayerManager = GameObject.Find("GameManager").GetComponent<PlayerManager>();
    }

    protected void Update() {
        // Cursor.visible = true;
        if (Input.GetButtonDown ("OpenMap")) {
            MapStatus = !MapStatus;
            PlayerManager.SetMap(MapStatus);
        }
        m_MapCamera.enabled = MapStatus;
        if (MapStatus != _MapStatus) {
            if (MapStatus) {
                Cursor.lockState = CursorLockMode.None;
            }
            else {
                Cursor.lockState = CursorLockMode.Locked;
            }
            Cursor.visible = MapStatus;
            _MapStatus = !_MapStatus;
        }
    }
}