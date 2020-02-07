using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using FreeLookCamera;
using Crest;

namespace UI {
    public class UIManager : MonoBehaviour {
        [Header("Units UI")]
            public GameObject m_TankUI;
            public GameObject m_PlaneUI;
            public GameObject m_ShipUI;
            private GameObject PlayerUIInstance;
            public GameObject m_TurretUI;
            private GameObject TurretUIInstance;
            public GameObject m_PlayerMapUI;
            private GameObject PlayerUI;
        private Text Score;
            private string CurrentScore;
        private Text UnitName;
        private Slider UnitHP;
            private float StartingHP;
            private float CurrentHP;
        private Slider ShipSpeedStep;
            private float SpeedStep;
        private Text ShipCurrentSpeed;
            const string ShipCurrentSpeedDisplay = "{0} km/h";
        private Slider ShipTurningSpeed;
            private float CurrentRotation;
        private Text DisplayTurretsStatus;
            const string TurretsStatusDisplay = "{0}";
            private string TurretStatus = "";
            private GameObject[] Turrets;
        private Text DisplayTurretsTargetRange;
            const string TurretsTargetRangeDisplayMeter = "Targeting range : {0} m";
            const string TurretsTargetRangeDisplayKilometer = "Targeting range : {0} km";
            private float TurretTargetRange;

        private GameObject ActiveTarget;
        private string TargetType; 
        private bool CurrentUnitDead;
        private bool DisplayGameUI = true;
        private bool DisplayMapUI = false;
        private bool DisplayUI = true;
        PlayerManager PlayerManager;
        private FreeLookCam FreeLookCam;

        public GameObject m_PauseUI;
        private GameObject PauseUIInstance;

        private void Start() {
            PlayerUI = GameObject.Find("UI");
        }
        private void Update() {
            if (DisplayGameUI && ActiveTarget != null && DisplayUI) {
                float CurrentSpeed = Mathf.Round(ActiveTarget.GetComponent<Rigidbody>().velocity.magnitude);
                ShipCurrentSpeed.text = string.Format(ShipCurrentSpeedDisplay, CurrentSpeed);

                if (TargetType == "Tank") {
                    CurrentHP = Mathf.Round(ActiveTarget.GetComponent<TankHealth>().GetCurrentHealth());
                    if (ActiveTarget.GetComponent<TurretManager>()) {
                        TurretStatus = ActiveTarget.GetComponent<TurretManager>().GetTurretStatus();
                        DisplayTurretsStatus.text = string.Format(TurretsStatusDisplay, TurretStatus);
                    }
                } else if (TargetType == "Aircraft") {
                    CurrentHP = Mathf.Round(ActiveTarget.GetComponent<AircraftHealth>().GetCurrentHealth());
                    UnitHP.value = CurrentHP;
                }

                if (TurretUIInstance) {
                    TurretTargetRange = ActiveTarget.GetComponent<TurretManager>().GetTargetRange();
                    if (TurretTargetRange > 999) {
                        TurretTargetRange = (Mathf.Round(TurretTargetRange / 100)) / 10f;
                        DisplayTurretsTargetRange.text = string.Format(TurretsTargetRangeDisplayKilometer, TurretTargetRange);
                    } else {
                        DisplayTurretsTargetRange.text = string.Format(TurretsTargetRangeDisplayMeter, Mathf.Round(TurretTargetRange));
                    }
                }
            }
        }

        private void SetOpenUI() {
            CloseGameUI();
            CloseTurretUI();
            if (DisplayGameUI && !DisplayMapUI && DisplayUI) {
                // CloseMapUI();
                if (TargetType == "Tank") {
                    OpenTankUI();
                } else if (TargetType == "Aircraft") {
                    OpenPlaneUI();
                } else if (TargetType == "Ship") {
                    OpenShipUI();
                }
                if (ActiveTarget.GetComponent<TurretManager>()) {
                    OpenTurretUI();
                }
            } else if (!DisplayGameUI && DisplayMapUI && DisplayUI) {
                OpenMapUI();
            }
        }
        private void OpenTankUI() {
            PlayerUIInstance = Instantiate(m_TankUI, PlayerUI.transform);

            Score = PlayerUIInstance.transform.Find("Score").GetComponent<Text>();
            UnitName = PlayerUIInstance.transform.Find("UnitName").GetComponent<Text>();
            UnitHP = PlayerUIInstance.transform.Find("UnitHealthSlider").GetComponent<Slider>();
            ShipSpeedStep = PlayerUIInstance.transform.Find("ShipSpeedStep").GetComponent<Slider>();
            ShipCurrentSpeed = PlayerUIInstance.transform.Find("ShipCurrentSpeed").GetComponent<Text>();
            ShipTurningSpeed = PlayerUIInstance.transform.Find("ShipTurningSpeed").GetComponent<Slider>();

            UnitName.text = ActiveTarget.name;
            UnitHP.maxValue = StartingHP;
            UnitHP.value = CurrentHP;
            ShipSpeedStep.value = SpeedStep;
            Score.text = CurrentScore;
        }
        private void OpenPlaneUI() {
            PlayerUIInstance = Instantiate(m_PlaneUI, PlayerUI.transform);

            Score = PlayerUIInstance.transform.Find("Score").GetComponent<Text>();
            UnitName = PlayerUIInstance.transform.Find("UnitName").GetComponent<Text>();
            UnitHP = PlayerUIInstance.transform.Find("UnitHealthSlider").GetComponent<Slider>();
            ShipSpeedStep = PlayerUIInstance.transform.Find("ShipSpeedStep").GetComponent<Slider>();
            ShipCurrentSpeed = PlayerUIInstance.transform.Find("ShipCurrentSpeed").GetComponent<Text>();
            ShipTurningSpeed = PlayerUIInstance.transform.Find("ShipTurningSpeed").GetComponent<Slider>();

            UnitName.text = ActiveTarget.name;
            UnitHP.maxValue = StartingHP;
            UnitHP.value = CurrentHP;
            ShipSpeedStep.value = SpeedStep;
            Score.text = CurrentScore;
        }
        private void OpenShipUI() {
            PlayerUIInstance = Instantiate(m_ShipUI, PlayerUI.transform);

            Score = PlayerUIInstance.transform.Find("Score").GetComponent<Text>();
            UnitName = PlayerUIInstance.transform.Find("UnitName").GetComponent<Text>();
            UnitHP = PlayerUIInstance.transform.Find("UnitHealthSlider").GetComponent<Slider>();
            ShipSpeedStep = PlayerUIInstance.transform.Find("ShipSpeedStep").GetComponent<Slider>();
            ShipCurrentSpeed = PlayerUIInstance.transform.Find("ShipCurrentSpeed").GetComponent<Text>();
            ShipTurningSpeed = PlayerUIInstance.transform.Find("ShipTurningSpeed").GetComponent<Slider>();

            UnitName.text = ActiveTarget.name;
            UnitHP.maxValue = StartingHP;
            UnitHP.value = CurrentHP;
            ShipSpeedStep.value = SpeedStep;
            Score.text = CurrentScore;
        }
        private void CloseGameUI() {
           if (PlayerUIInstance)
                Destroy (PlayerUIInstance); 
        }
        private void OpenTurretUI() {
            TurretUIInstance = Instantiate(m_TurretUI, PlayerUI.transform);

            DisplayTurretsStatus = TurretUIInstance.transform.Find("TurretsStatus").GetComponent<Text>();
            DisplayTurretsTargetRange = TurretUIInstance.transform.Find("TurretsTargetRange").GetComponent<Text>();
        }
        private void CloseTurretUI() {
           if (TurretUIInstance)
                Destroy (TurretUIInstance); 
        }
        private void OpenMapUI() {
            PlayerUIInstance = Instantiate(m_PlayerMapUI, PlayerUI.transform);

            Score = PlayerUIInstance.transform.Find("Score").GetComponent<Text>();
            UnitName = PlayerUIInstance.transform.Find("UnitName").GetComponent<Text>();
            UnitHP = PlayerUIInstance.transform.Find("UnitHealthSlider").GetComponent<Slider>();

            UnitName.text = ActiveTarget.name;
            UnitHP.maxValue = StartingHP;
            UnitHP.value = CurrentHP;
            Score.text = CurrentScore;
        }

        public void SetTargetType(string Type) {
            TargetType = Type;
        }

        public void SetActiveTarget(GameObject Target) {
            ActiveTarget = Target;
            // m_UnitName.text = ActiveTarget.name;
            if (ActiveTarget.GetComponent<TurretManager>()) {
                Turrets = ActiveTarget.GetComponent<TurretManager>().GetTurrets();
            }

            if (TargetType == "Tank") {
                if (ActiveTarget.GetComponent<TankHealth>())
                    StartingHP = ActiveTarget.GetComponent<TankHealth>().GetStartingHealth();
            } else if (TargetType == "Aircraft") {
                if (ActiveTarget.GetComponent<AircraftHealth>())
                    StartingHP = ActiveTarget.GetComponent<AircraftHealth>().GetStartingHealth();
            } else if (TargetType == "Ship") {
                StartingHP = ActiveTarget.GetComponent<ShipHealth>().GetStartingHealth();
                SetCurrentUnitHealth(ActiveTarget.GetComponent<ShipHealth>().GetCurrentHealth());
                ChangeSpeedStep(ActiveTarget.GetComponent<ShipMovement>().GetCurrentSpeedStep());
                CurrentUnitDead = ActiveTarget.GetComponent<ShipController>().GetDeath();
            }
        }

        public void SetPauseUI(bool pause) {
            if (pause){
                OpenPauseUI();
            } else {
                ClosePauseUI();
            }
        }

        protected void OpenPauseUI(){
            PauseUIInstance = Instantiate(m_PauseUI);

            Button buttonResumeGame = PauseUIInstance.transform.Find("ButtonResumeGame").GetComponent<Button>();
            buttonResumeGame.onClick.AddListener(ButtonResumeGameOnClick);
            Button buttonBackToMenu = PauseUIInstance.transform.Find("ButtonBackToMenu").GetComponent<Button>();
            buttonBackToMenu.onClick.AddListener(ButtonBackToMenuOnClick);
            Button buttonBackToDesktop = PauseUIInstance.transform.Find("ButtonCloseGame").GetComponent<Button>();
            buttonBackToDesktop.onClick.AddListener(ButtonBackToDesktopOnClick);
        }
        protected void ButtonResumeGameOnClick(){
            PlayerManager.SetPause();
        }
        protected void ButtonBackToMenuOnClick(){
            PlayerManager.EndGame();
        }
        protected void ButtonBackToDesktopOnClick(){
            Application.Quit();
        }
        protected void ClosePauseUI(){
            if (PauseUIInstance)
                Destroy (PauseUIInstance);
        }

        public void SetPlayerManager(PlayerManager playerManager){ PlayerManager = playerManager; }
        // public void SetPlayerCanvas(GameObject playerCanvas, GameObject playerMapCanvas) {
        //     PlayerCanvas = playerCanvas;
        //     PlayerMapCanvas = playerMapCanvas;
        // }
        public void SetFreeLookCamera(FreeLookCam freeLookCam){ FreeLookCam = freeLookCam; }
        public void SetMap(bool map) {
            DisplayMapUI = map;
            if (!CurrentUnitDead)
                DisplayGameUI = !map;
            SetOpenUI();
        }
        public void SetScoreMessage(string message) {
            // This contradicts a bit the No UI toogle but better gameplay than a cosmetic error
            CurrentScore = message;
            if (PlayerUIInstance)
                Score.text = CurrentScore;
        }
        public void SetCurrentUnitHealth(float health){
            CurrentHP = health;
            if (PlayerUIInstance)
                UnitHP.value = CurrentHP;

        }
        public void ChangeSpeedStep(int currentSpeedStep){
            SpeedStep = currentSpeedStep;
            if (PlayerUIInstance)
                ShipSpeedStep.value = SpeedStep;
        }
        public void SetRotationInput(float rotation){
            CurrentRotation = rotation;
            if (PlayerUIInstance)
                ShipTurningSpeed.value = CurrentRotation;
        }
        public void SetTurretStatus(string status){
            TurretStatus = status;
            if (TurretUIInstance) 
                DisplayTurretsStatus.text = string.Format(TurretsStatusDisplay, TurretStatus);
        }
        public void SetHideUI() { DisplayUI = !DisplayUI;  SetOpenUI(); }
        public void SetCurrentUnitDead(bool isUnitDead) {
            CurrentUnitDead = isUnitDead;
            DisplayGameUI = !isUnitDead;
            SetOpenUI();
        }
    }
}