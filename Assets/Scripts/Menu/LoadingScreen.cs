using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour {


    void Start() {
        SceneManager.LoadSceneAsync(LoadingData.sceneToLoad);
    }

    protected void Update() {
    }
}