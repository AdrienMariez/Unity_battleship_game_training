using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using UnityEngine.SceneManagement;

public class MenuButtonsControl : MonoBehaviour {
    public GameObject m_MenuUI;
    private GameObject MenuUIInstance;
    public GameObject m_OptionsUI;
    private GameObject OptionsUIInstance;

    void Start() {
        OpenMainMenu();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1.0f;
    }

    protected void Update() {
        
    }

    protected void OpenMainMenu() {
        MenuUIInstance = Instantiate(m_MenuUI);
        Button buttonScenarioTraining = MenuUIInstance.transform.Find("ButtonScenarioTraining").GetComponent<Button>();
		buttonScenarioTraining.onClick.AddListener(ButtonScenarioTrainingOnClick);
        Button buttonScenario1 = MenuUIInstance.transform.Find("ButtonScenario1").GetComponent<Button>();
		buttonScenario1.onClick.AddListener(ButtonScenario1OnClick);
        Button buttonScenario2 = MenuUIInstance.transform.Find("ButtonScenario2").GetComponent<Button>();
		buttonScenario2.onClick.AddListener(ButtonScenario2OnClick);
        Button buttonScenario3 = MenuUIInstance.transform.Find("ButtonScenario3").GetComponent<Button>();
		buttonScenario3.onClick.AddListener(ButtonScenario3OnClick);
        Button buttonExit = MenuUIInstance.transform.Find("ButtonExit").GetComponent<Button>();
		buttonExit.onClick.AddListener(ButtonExitOnClick);
        Button buttonOptions = MenuUIInstance.transform.Find("ButtonOptions").GetComponent<Button>();
		buttonOptions.onClick.AddListener(ButtonOptionsOnClick);
    }
    protected void CloseMainMenu() {
        if (MenuUIInstance)
            Destroy (MenuUIInstance);
    }
    void ButtonScenarioTrainingOnClick(){
        // Debug.Log ("Button Scenario Training Clicked !");
        string sceneName = "ROTS_scenario_training";
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
    void ButtonScenario1OnClick(){
        // Debug.Log ("Button Scenario 1 Clicked !");
        string sceneName = "ROTS_scenario_1";
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
    void ButtonScenario2OnClick(){
        // Debug.Log ("Button Scenario 2 Clicked !");
        string sceneName = "ROTS_scenario_2";
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
    void ButtonScenario3OnClick(){
        // Debug.Log ("Button Scenario 3 Clicked !");
        string sceneName = "ROTS_scenario_3";
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
    void ButtonExitOnClick(){
        // Debug.Log ("Exit Options Clicked !");
        Application.Quit();
    }

    void ButtonOptionsOnClick(){
        // Debug.Log ("Button Options Clicked !");
        CloseMainMenu();
        OpenOptionsMenu();
    }

    protected void OpenOptionsMenu() {
        OptionsUIInstance = Instantiate(m_OptionsUI);
        Button ButtonMainMenu = OptionsUIInstance.transform.Find("ButtonMainMenu").GetComponent<Button>();
		ButtonMainMenu.onClick.AddListener(ButtonMainMenuOnClick);
    }
    protected void CloseOptionsMenu() {
        if (OptionsUIInstance)
            Destroy (OptionsUIInstance);
    }
    void ButtonMainMenuOnClick(){
        Debug.Log ("Back to main menu.");
        CloseOptionsMenu();
        OpenMainMenu();
    }
}