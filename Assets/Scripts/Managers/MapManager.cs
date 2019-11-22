using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    public Camera m_MapCamera;
    private bool m_MapStatus;
    private bool _m_MapStatus;
    private GameObject m_GameManager;

    private void Start() {
        m_MapStatus = false;
        _m_MapStatus = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        m_GameManager = GameObject.Find("GameManager");
    }

    protected void Update() {
        // Cursor.visible = true;
        if (Input.GetButtonDown ("OpenMap")) {
            m_MapStatus = !m_MapStatus;
            m_GameManager.GetComponent<PlayerManager>().m_MapActive = m_MapStatus;
        }
        m_MapCamera.enabled = m_MapStatus;
        if (m_MapStatus != _m_MapStatus) {
            if (m_MapStatus) {
                Cursor.lockState = CursorLockMode.None;
            }
            else {
                Cursor.lockState = CursorLockMode.Locked;
            }
            Cursor.visible = m_MapStatus;
            _m_MapStatus = !_m_MapStatus;
        }
    }
}