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

    // Duel Options
        public GameObject m_MenuUIDuelOptions;
        public GameObject m_DuelSpawnDropdown;
        private GameObject MenuUIDuelOptionsInstance;
        private GameObject MenuUIDuelSpawnPointsContainerInstance;
        private List<SpawnPointDuel> SpawnPointsDuel = new List<SpawnPointDuel>();

    [Header("Units in the background")]
        public GameObject[] m_ShipSpawnPoints;

    // // Units lists
    //     [HideInInspector] public List<WorldSingleUnit> SpawnableUnitsList;

    // Scenario type selection
        Button ButtonDuelCategory;
        Button ButtonPointsCategory;
        Button ButtonCustomCategory;

    [Header("Scenario List")]
        public GameObject m_ScenarioSelect;
        public float m_ScenariosSpacing = 220;
        private GameObject ScenariosListContainerInstance;
        [HideInInspector] public List<CompiledTypes.Scenarios> ScenariosDBList = new List<CompiledTypes.Scenarios>();
        [HideInInspector] public List<CompiledTypes.Scenarios> ScenariosDBListTrimed = new List<CompiledTypes.Scenarios>();
        private string CurrentGameModeId;
    // Scenarios Options
        private GameObject ScenariosOptionsContainerInstance;
        private string  CurrentScenarioId;
        

    void Start() {
        // foreach (CompiledTypes.Scenarios scenario2 in WorldUnitsManager.GetDB().Scenarios.GetAll()) {
            // Debug.Log (" scenario : "+scenario.ScenarioScene);
            foreach (CompiledTypes.Scenarios scenario in WorldUnitsManager.GetDB().Scenarios.GetAll()) {
                ScenariosDBList.Add(scenario);
            }
        // }

        OpenMainMenu();
        LoadMenuUnits();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1.0f;
    }

    protected void LoadMenuUnits() {    
        List<WorldSingleUnit> spawnableShipList = new List<WorldSingleUnit>();
        foreach (List<WorldSingleUnit> category in WorldUnitsManager.GetUnitsByCategory()) {
            foreach (WorldSingleUnit unit in category) {
                // Debug.Log (" unit "+unit.GetUnitName());
                if (unit.GetUnitCategory_DB().id == WorldUnitsManager.GetDB().Units_categories.ship.id) {
                    spawnableShipList.Add(unit);
                } else {
                    break;
                }
            }
        }

        foreach (GameObject spawnPoint in m_ShipSpawnPoints) {
            int randomUnitId = Random.Range(0, spawnableShipList.Count);
            // Debug.Log (randomUnitId+" / "+spawnableShipList.Count);
            // Debug.Log (" Spawning "+spawnableShipList[randomUnitId].GetUnitName());
            GameObject Instance = WorldUnitsManager.BuildUnit(spawnableShipList[randomUnitId], spawnPoint.transform.position, spawnPoint.transform.rotation);
            if (Instance.GetComponent<UnitAIController>()) {
                Instance.GetComponent<UnitAIController>().SetAIFromUnitManager(false, false, false);
            }
        }
    }

    protected void OpenMainMenu() {
        MenuUIInstance = Instantiate(m_MenuUI);

        Button buttonExit = MenuUIInstance.transform.Find("ButtonExit").GetComponent<Button>();
		buttonExit.onClick.AddListener(ButtonExitOnClick);
        Button buttonOptions = MenuUIInstance.transform.Find("ButtonOptions").GetComponent<Button>();
		buttonOptions.onClick.AddListener(ButtonOptionsOnClick);
        Button buttonCredits = MenuUIInstance.transform.Find("ButtonCredits").GetComponent<Button>();
		buttonCredits.onClick.AddListener(ButtonCreditsOnClick);

        ButtonDuelCategory = MenuUIInstance.transform.Find("ScenariosChoice").transform.Find("ButtonDuel").GetComponent<Button>();
        ButtonDuelCategory.onClick.AddListener(() => { ButtonCategoryOnClick(WorldUnitsManager.GetDB().GameModes.duel); });

        ButtonPointsCategory = MenuUIInstance.transform.Find("ScenariosChoice").transform.Find("ButtonPoints").GetComponent<Button>();
        ButtonPointsCategory.onClick.AddListener(() => { ButtonCategoryOnClick(WorldUnitsManager.GetDB().GameModes.points); });

        ButtonCustomCategory = MenuUIInstance.transform.Find("ScenariosChoice").transform.Find("ButtonCustom").GetComponent<Button>();
        ButtonCustomCategory.onClick.AddListener(() => { ButtonCategoryOnClick(WorldUnitsManager.GetDB().GameModes.custom); });
        
        ScenariosListContainerInstance = MenuUIInstance.transform.Find("ScenarioList").transform.Find("Scroll View").transform.Find("Viewport").transform.Find("Content").gameObject;
        ScenariosOptionsContainerInstance = MenuUIInstance.transform.Find("ScenariosParameters").gameObject;
    }

    void ButtonCategoryOnClick(CompiledTypes.GameModes buttonGameMode){

        if (buttonGameMode.id == CurrentGameModeId) {                               // If the current selected category is already set, don't reset it 
            return;
        }
        CurrentGameModeId = buttonGameMode.id;
        // Debug.Log (buttonGameMode);

        foreach (Transform child in ScenariosListContainerInstance.transform) {     // Clear the current scenario list to make place for the new ones
            GameObject.Destroy(child.gameObject);
        }
        ScenariosDBListTrimed = new List<CompiledTypes.Scenarios>();
        if (MenuUIDuelOptionsInstance) {                                            // Destroy the parameters instance here
            Destroy (MenuUIDuelOptionsInstance);
        }

        foreach (CompiledTypes.Scenarios scenario in ScenariosDBList) {             // Find all suitable scenarios
            foreach (CompiledTypes.ScenarioGameModes gameMode in scenario.ScenarioGameModesList) {
                if (gameMode.ScenarioGameMode.id == buttonGameMode.id) {
                    ScenariosDBListTrimed.Add(scenario);
                    break;
                }
            }
        }
        // Debug.Log (ScenariosDBListTrimed.Count);

        foreach (CompiledTypes.Scenarios scenario in ScenariosDBListTrimed) {       // Create individual buttons
            GameObject listElement = Instantiate(m_ScenarioSelect, ScenariosListContainerInstance.transform);

            string text = scenario.ScenarioScene + " "+ scenario.id + " "+ buttonGameMode.id;
            listElement.transform.GetChild(0).GetComponentInChildren<Text>().text = text;

            Button spawnerButton = listElement.transform.GetChild(0).GetComponent<Button>();

            spawnerButton.onClick.AddListener(() => { OpenScenarioOptions(scenario, buttonGameMode); });        // Clicking on this button will open the options for the chosen scenario
        }
    }

    protected void OpenScenarioOptions (CompiledTypes.Scenarios scenario, CompiledTypes.GameModes gamemode) {
        // Debug.Log (" scenario : "+scenario.id +", gameMode : " + gamemode.id);
        if (scenario.id == CurrentScenarioId) {
            return;
        }
        if (gamemode.id == WorldUnitsManager.GetDB().GameModes.duel.id) {
            if (!MenuUIDuelOptionsInstance) {
                CreateDuelOptions();
            }
            SetDuelOptions(scenario);
        }
    }
    protected void CreateDuelOptions () {
        // Debug.Log (" OpenDuelOptions ");
        MenuUIDuelOptionsInstance = Instantiate(m_MenuUIDuelOptions, ScenariosOptionsContainerInstance.transform);
        MenuUIDuelSpawnPointsContainerInstance = MenuUIDuelOptionsInstance.transform.Find("SpawnPoints").transform.Find("Viewport").transform.Find("Content").gameObject;
    }
    protected void SetDuelOptions (CompiledTypes.Scenarios scenario) {
        foreach (Transform child in MenuUIDuelSpawnPointsContainerInstance.transform) {     // Clear the current spawnpoints list to make place for the new ones
            GameObject.Destroy(child.gameObject);
            SpawnPointsDuel = new List<SpawnPointDuel>();
        }

        // Debug.Log (" SetDuelOptions ");
        CurrentScenarioId = scenario.id;
        foreach (CompiledTypes.DuelSpawnPoints spawnPoint in scenario.DuelSpawnPointsList) {
            GameObject listElement = Instantiate(m_DuelSpawnDropdown, MenuUIDuelSpawnPointsContainerInstance.transform);
            // listElement.transform.Find("Dropdown").transform.Find("Label").GetComponent<Text>().text = spawnPoint.DuelSpawnPointCategory.Name;
            listElement.transform.Find("CategoryHeader").GetComponent<Text>().text = spawnPoint.DuelSpawnPointCategory.Name;

            Dropdown _dropdown = listElement.transform.Find("Dropdown").GetComponent<Dropdown>();
            List<string> _dropdownOptions = new List<string>();
            List<WorldSingleUnit> optionUnits = new List<WorldSingleUnit>();
            _dropdownOptions.Add("Empty");                  // Add an empty string at the beginning of each select, regardless of any option.

            foreach (List<WorldSingleUnit> category in WorldUnitsManager.GetUnitsByCategory()) {
                foreach (WorldSingleUnit unit in category) {
                    // Debug.Log (" unit "+unit.GetUnitName());
                    
                    if (unit.GetUnitCategory_DB().id == spawnPoint.DuelSpawnPointCategory.id) {
                        _dropdownOptions.Add(unit.GetUnitName());
                        optionUnits.Add(unit);
                    } else {
                        break;
                    }
                }
            }
            _dropdown.AddOptions(_dropdownOptions);

            _dropdown.onValueChanged.AddListener(delegate {
                DropdownValueChanged(_dropdown, spawnPoint, optionUnits);
            });

            Toggle _toggleAIMove = listElement.transform.Find("AICanMove").GetComponent<Toggle>();
            _toggleAIMove.onValueChanged.AddListener(delegate {
                ToggleAIChanged(_dropdown, _toggleAIMove, "move");
            });

            Toggle _toggleAIShoot = listElement.transform.Find("AICanShoot").GetComponent<Toggle>();
            _toggleAIShoot.onValueChanged.AddListener(delegate {
                ToggleAIChanged(_dropdown, _toggleAIShoot, "shoot");
            });

            Toggle _toggleAISpawn = listElement.transform.Find("AICanSpawn").GetComponent<Toggle>();
            _toggleAISpawn.onValueChanged.AddListener(delegate {
                ToggleAIChanged(_dropdown, _toggleAISpawn, "spawn");
            });

            SpawnPointDuel _spawnPointDuel = new SpawnPointDuel{};
                _spawnPointDuel.SetDropdown(_dropdown);
                _spawnPointDuel.SetSpawnPointDB(spawnPoint);
                _toggleAIMove.isOn = _spawnPointDuel.GetCanMove();
                _toggleAIShoot.isOn = _spawnPointDuel.GetCanShoot();
                _toggleAISpawn.isOn = _spawnPointDuel.GetCanSpawn();
            SpawnPointsDuel.Add(_spawnPointDuel);
        }
    }
    public class SpawnPointDuel {
        private CompiledTypes.DuelSpawnPoints _spawnPointDB; public CompiledTypes.DuelSpawnPoints GetSpawnPointDB(){ return _spawnPointDB; } public void SetSpawnPointDB(CompiledTypes.DuelSpawnPoints _sp){ _spawnPointDB = _sp; }
        // private string _spawnPointIdentifier;  public string GetSpawnPointID(){ return _spawnPointIdentifier; } public void SetSpawnPointID(string _spID){ _spawnPointIdentifier = _spID; }
        private Dropdown _dropdown; public Dropdown GetDropdown(){ return _dropdown; } public void SetDropdown(Dropdown _d){ _dropdown = _d; }
        private WorldSingleUnit _unit = null;  public WorldSingleUnit GetUnit(){ return _unit; } public void SetUnit(WorldSingleUnit _SWU ){ _unit = _SWU; }
        private bool _unitCanMove = true;  public bool GetCanMove(){ return _unitCanMove; } public void SetCanMove(bool _b){ _unitCanMove = _b; }
        private bool _unitCanShoot = true;  public bool GetCanShoot(){ return _unitCanShoot; } public void SetCanShoot(bool _b){ _unitCanShoot = _b; }
        private bool _unitCanSpawn = true;  public bool GetCanSpawn(){ return _unitCanSpawn; } public void SetCanSpawn(bool _b){ _unitCanSpawn = _b; }
    }

    protected void DropdownValueChanged(Dropdown dropDown, CompiledTypes.DuelSpawnPoints spawnPoint, List<WorldSingleUnit> optionUnits) {
        if (dropDown.value == 0) {
            // Debug.Log (spawnPoint.DuelSpawnPointName + " is now empty ! ");
            // Do stuff to empty !
            foreach (SpawnPointDuel sp in SpawnPointsDuel) {
                if (dropDown == sp.GetDropdown()) {
                    Debug.Log (sp.GetSpawnPointDB().DuelSpawnPointName + " is now empty ! ");
                    sp.SetUnit(null);
                }
            }
        } else {
            // Debug.Log (" Spawn point : " + spawnPoint.DuelSpawnPointName + " - Value : " + dropDown.options[dropDown.value].text);
            // Debug.Log (" Spawn point : " + spawnPoint.DuelSpawnPointName + " - Unit : " + optionUnits[dropDown.value-1].GetUnitName());
            // dropDown.value
            foreach (SpawnPointDuel sp in SpawnPointsDuel) {
                if (dropDown == sp.GetDropdown()) {
                    sp.SetUnit(optionUnits[dropDown.value-1]);
                    Debug.Log (" Spawn point : " + sp.GetSpawnPointDB().DuelSpawnPointName + " - Unit : " + optionUnits[dropDown.value-1].GetUnitName());
                }
            }
        }
    }

    protected void ToggleAIChanged(Dropdown dropDown, Toggle _toggleAI, string _toogleType) {
        foreach (SpawnPointDuel sp in SpawnPointsDuel) {
            if (dropDown == sp.GetDropdown()) {
                if (_toogleType == "move") {
                    sp.SetCanMove(_toggleAI.isOn);
                    Debug.Log (" Spawn point : " + sp.GetSpawnPointDB().DuelSpawnPointName + " - AIMove : " + _toggleAI.isOn +" / "+ sp.GetCanMove());
                } else if (_toogleType == "shoot") {
                    sp.SetCanShoot(_toggleAI.isOn);
                    Debug.Log (" Spawn point : " + sp.GetSpawnPointDB().DuelSpawnPointName + " - AIshoot : " + _toggleAI.isOn +" / "+ sp.GetCanShoot());
                } else if (_toogleType == "spawn") {
                    sp.SetCanSpawn(_toggleAI.isOn);
                    Debug.Log (" Spawn point : " + sp.GetSpawnPointDB().DuelSpawnPointName + " - AIspawn : " + _toggleAI.isOn +" / "+ sp.GetCanSpawn());
                }
                break;
            }
        }
    }


    /*protected void OpenMainMenu() {
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
    }*/
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