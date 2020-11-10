using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour {

    void Start() {
        // Very crude check to see if a scenario is launched or a back to menu
        if (LoadingData.SelectedScenario != null && LoadingData.PlayerTeam != null && LoadingData.CurrentGameMode != null) {
            // Debug.Log ("case1");
            SceneManager.LoadSceneAsync(LoadingData.SelectedScenario.ScenarioScene);
        } else {
            // Debug.Log ("case2");
            SceneManager.LoadSceneAsync(LoadingData.MainMenu);
            LoadingData.CleanData();
        }
    }
}