using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    // This whole file should be moved into PlayerManager, as it should be instancied for each player in the future
    public Camera m_MapCamera;
    private bool MapStatus;
    private PlayerManager PlayerManager;

    private void Start() {
        PlayerManager = GameObject.Find("GameManager").GetComponent<PlayerManager>();
        MapStatus = false;
    }

    protected void Update() {
        if (Input.GetButtonDown ("OpenMap")) {
            MapStatus = !MapStatus;
            SetMap(MapStatus);
        }
    }

    public void SetMap(bool Active) {
        m_MapCamera.enabled = Active;
        PlayerManager.SetMap(Active);
    }
}