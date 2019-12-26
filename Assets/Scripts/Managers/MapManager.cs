using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    public Camera m_MapCamera;
    private bool MapStatus;
    private PlayerManager PlayerManager;

    private void Start() {
        PlayerManager = GameObject.Find("GameManager").GetComponent<PlayerManager>();
        MapStatus = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    protected void Update() {
        if (Input.GetButtonDown ("OpenMap")) {
            MapStatus = !MapStatus;
            SetMap(MapStatus);
        }
    }

    public void SetMap(bool Active) {
        m_MapCamera.enabled = Active;
        if (Active) {
            Cursor.lockState = CursorLockMode.None;
        } else {
            Cursor.lockState = CursorLockMode.Locked;
        }
        Cursor.visible = Active;
        PlayerManager.SetMap(Active);
    }
}