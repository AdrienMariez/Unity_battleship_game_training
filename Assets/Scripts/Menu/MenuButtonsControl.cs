using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;

public class MenuButtonsControl : MonoBehaviour {
    public GameObject m_MenuUI;
    private GameObject MenuUIInstance;
    public GameObject m_OptionsUI;
    private GameObject OptionsUIInstance;
    public GameObject m_CreditsUI;
    private GameObject CreditsUIInstance;
    private bool DisplayHelpImage = false;

    public GameObject[] m_ShipSpawnPoints;
    [HideInInspector] public List<WorldSingleUnit> SpawnableUnitsList;

    void Start() {
        OpenMainMenu();
        LoadMenuUnits();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1.0f;
    }

    protected void LoadMenuUnits() {
        
        foreach (List<WorldSingleUnit> subCategory in WorldUnitsManager.GetUnitsBySubcategory()) {
            for (int i=0; i < subCategory.Count; i++) {
                // Debug.Log(subCategory[0].GetUnitName());
                if (subCategory[0] != null) {                                                       // Check the first element of each category, if it is good !
                    if (subCategory[i].GetUnitCategory() == CompiledTypes.Units_categories.RowValues.ship) {
                        SpawnableUnitsList.Add(subCategory[i]);
                    }
                } else {
                    break;                                                                          // If nothing is in the list (as intended, stop checking)
                }
            }
        }

        // foreach (WorldSingleUnit unit in SpawnableUnitsList) {
        //     Debug.Log (unit.GetUnitSubCategory()+" / "+unit.GetUnitName()+" / "+unit.GetUnitTeam());
        // }

        foreach (GameObject spawnPoint in m_ShipSpawnPoints) {
            int randomUnitId = Random.Range(0, SpawnableUnitsList.Count);
            // Debug.Log (randomUnitId+" / "+SpawnableUnitsList.Count);
            // Debug.Log (" Spawning "+SpawnableUnitsList[randomUnitId].GetUnitName());
            GameObject Instance = WorldUnitsManager.BuildUnit(SpawnableUnitsList[randomUnitId], spawnPoint.transform.position, spawnPoint.transform.rotation);
            if (Instance.GetComponent<UnitAIController>()) {
                Instance.GetComponent<UnitAIController>().SetAIFromUnitManager(false, false, false);
            }
        }
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
        Button buttonCredits = MenuUIInstance.transform.Find("ButtonCredits").GetComponent<Button>();
		buttonCredits.onClick.AddListener(ButtonCreditsOnClick);
    }
    protected void CloseMainMenu() {
        if (MenuUIInstance)
            Destroy (MenuUIInstance);
    }
    void ButtonScenarioTrainingOnClick(){
        // Debug.Log ("Button Scenario Training Clicked !");
        // string sceneName = "ROTS_scenario_training";
        // SceneManager.LoadScene(sceneName, LoadSceneMode.Single);

        LoadingData.sceneToLoad = "ROTS_scenario_training";
        SceneManager.LoadScene("Loading");
    }
    void ButtonScenario1OnClick(){
        // Debug.Log ("Button Scenario 1 Clicked !");
        // string sceneName = "ROTS_scenario_1";
        // SceneManager.LoadScene(sceneName, LoadSceneMode.Single);

        LoadingData.sceneToLoad = "ROTS_scenario_1";
        SceneManager.LoadScene("Loading");
    }
    void ButtonScenario2OnClick(){
        // Debug.Log ("Button Scenario 2 Clicked !");
        // string sceneName = "ROTS_scenario_2";
        // SceneManager.LoadScene(sceneName, LoadSceneMode.Single);

        LoadingData.sceneToLoad = "ROTS_scenario_2";
        SceneManager.LoadScene("Loading");
    }
    void ButtonScenario3OnClick(){
        // Debug.Log ("Button Scenario 3 Clicked !");
        // string sceneName = "ROTS_scenario_3";
        // SceneManager.LoadScene(sceneName, LoadSceneMode.Single);

        LoadingData.sceneToLoad = "ROTS_scenario_3";
        SceneManager.LoadScene("Loading");
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
        DisplayHelpImage = false;
        Button ButtonDisplayHelp = OptionsUIInstance.transform.Find("ButtonDisplayHelp").GetComponent<Button>();
		ButtonDisplayHelp.onClick.AddListener(ButtonDisplayHelpOnClick);
        Button ButtonMainMenu = OptionsUIInstance.transform.Find("ButtonMainMenu").GetComponent<Button>();
		ButtonMainMenu.onClick.AddListener(ButtonMainMenuOnClick);
    }
    protected void CloseOptionsMenu() {
        Destroy (OptionsUIInstance);
    }
    
    void ButtonCreditsOnClick(){
        // Debug.Log ("Button Credits Clicked !");
        CloseMainMenu();
        OpenCreditsMenu();

    }
    protected void OpenCreditsMenu() {
        CreditsUIInstance = Instantiate(m_CreditsUI);
        DisplayHelpImage = false;
        Button ButtonMainMenu = CreditsUIInstance.transform.Find("ButtonMainMenu").GetComponent<Button>();
		ButtonMainMenu.onClick.AddListener(ButtonMainMenuOnClick);
    }
    protected void CloseCreditsMenu() {
        Destroy (CreditsUIInstance);
    }
    void ButtonDisplayHelpOnClick(){
        DisplayHelpImage = !DisplayHelpImage;
        OptionsUIInstance.transform.Find("ImageHelp").gameObject.SetActive(DisplayHelpImage);
    }
    void ButtonMainMenuOnClick(){
        // Debug.Log ("Back to main menu.");
        if (OptionsUIInstance)
            CloseOptionsMenu();
        if (CreditsUIInstance)
            CloseCreditsMenu();
        OpenMainMenu();
    }
}