using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FreeLookCamera;
using Crest;

namespace UI {
    public class UIManager : MonoBehaviour {
        [Header("Units UI")]
            private GameObject m_TankUI;
            private GameObject m_PlaneUI;
            private GameObject m_ShipUI;
            private GameObject PlayerUIInstance;
            private GameObject m_PlayerMapUI;
            private GameObject m_TurretUI;
            private GameObject TurretUIInstance;
            private GameObject PlayerUI;
            private GameObject m_PauseUI;
            private GameObject PauseUIInstance;
            // private GameObject PlayerCanvas;
            // private GameObject PlayerMapCanvas;
        [Header("Turrets status icons")]
            private GameObject TurretStatusSprites;
            private float IconsSpacing = 22;
        [Header("Shell Camera")]
            private GameObject m_ShellCamera;
            private float m_TimeToDestroyCamera = 3;
        private Text Score;
            private string CurrentScore;
        private Text UnitName;
        private Slider UnitHP;
        private Image UnitHPColor;
            private float StartingHP;
            private float CurrentHP;
        private Slider ShipSpeedStep;
            private float SpeedStep;
        private Text ShipCurrentSpeed;
            const string ShipCurrentSpeedDisplay = "{0} km/h";
        private Slider ShipTurningSpeed;
            private float CurrentRotation;
        private GameObject DisplayTurretsArtilleryAimer;
        private GameObject DisplayTurretsAAAimer;
        private GameObject DisplayTurretsStatus;
            private List <TurretManager.TurretStatusType> TurretStatus;
        private Text DisplayTurretsTargetRange;
            const string TurretsTargetRangeDisplayMeter = "{0} m";
            const string TurretsTargetRangeDisplayKilometer = "{0} km";
            private float TurretTargetRange;
        private GameObject DisplayTurretsAIControl;
        private GameObject DisplayTurretsCurrentArtillery;
        private GameObject DisplayTurretsCurrentAA;
        private GameObject DisplayTurretsCurrentTorpedoes;
        private GameObject DisplayTurretsCurrentDepthCharges;
        private GameObject ActiveTarget;
        private string TargetType; 
        private bool CurrentUnitDead;
        private bool FreeCamera = false;
        private bool OverlayUI = false;
        private bool DisplayGameUI = true;
        private bool DisplayMapUI = false;
        private bool DisplayUI = true;
        private GameObject PlayerCanvas;
        private PlayerManager PlayerManager;
        private FreeLookCam FreeLookCam;
        private WorldUIVariables WorldUIVariables;

        private void Start() {
            PlayerUI = GameObject.Find("UI");
            WorldUIVariables worldUIVariables = GameObject.Find("GlobalSharedVariables").GetComponent<WorldUIVariables>();
            m_TankUI = worldUIVariables.m_TankUI;
            m_ShipUI = worldUIVariables.m_ShipUI;
            m_PlayerMapUI = worldUIVariables.m_PlayerMapUI;
            m_TurretUI = worldUIVariables.m_TurretUI;
            m_PauseUI = worldUIVariables.m_PauseMenu;
            TurretStatusSprites = worldUIVariables.TurretStatusSprites;
            IconsSpacing = worldUIVariables.IconsSpacing;
            m_ShellCamera = worldUIVariables.m_ShellCamera;
            m_TimeToDestroyCamera = worldUIVariables.m_TimeToDestroyCamera;
        }
        private void Update() {
            if (DisplayGameUI && ActiveTarget != null && DisplayUI) {
                float CurrentSpeed = Mathf.Round(ActiveTarget.GetComponent<Rigidbody>().velocity.magnitude);
                if (ShipCurrentSpeed != null)
                    ShipCurrentSpeed.text = string.Format(ShipCurrentSpeedDisplay, CurrentSpeed);

                if (TargetType == "Tank") {
                    CurrentHP = Mathf.Round(ActiveTarget.GetComponent<TankHealth>().GetCurrentHealth());
                    if (ActiveTarget.GetComponent<TurretManager>()) {
                        // Heavy load ! to change if tanks are worked on !
                        CreateTurretsStatusDisplay();
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
                if (TargetType == "Tank") {
                    OpenTankUI();
                } else if (TargetType == "Building") {
                    OpenBuildingUI();
                }else if (TargetType == "Aircraft") {
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
            UnitHPColor = PlayerUIInstance.transform.Find("UnitHealthSlider").Find("Fill Area").Find("Fill").GetComponent<Image>();
            ShipSpeedStep = PlayerUIInstance.transform.Find("ShipSpeedStep").GetComponent<Slider>();
            ShipCurrentSpeed = PlayerUIInstance.transform.Find("ShipCurrentSpeed").GetComponent<Text>();
            ShipTurningSpeed = PlayerUIInstance.transform.Find("ShipTurningSpeed").GetComponent<Slider>();

            UnitName.text = ActiveTarget.name;
            UnitHP.maxValue = StartingHP;
            UnitHP.value = CurrentHP;
            CheckHealthColor();
            ShipSpeedStep.value = SpeedStep;
            Score.text = CurrentScore;
        }
        private void OpenBuildingUI() {
            PlayerUIInstance = Instantiate(m_TankUI, PlayerUI.transform);

            Score = PlayerUIInstance.transform.Find("Score").GetComponent<Text>();
            UnitName = PlayerUIInstance.transform.Find("UnitName").GetComponent<Text>();
            UnitHP = PlayerUIInstance.transform.Find("UnitHealthSlider").GetComponent<Slider>();
            UnitHPColor = PlayerUIInstance.transform.Find("UnitHealthSlider").Find("Fill Area").Find("Fill").GetComponent<Image>();
            ShipSpeedStep = PlayerUIInstance.transform.Find("ShipSpeedStep").GetComponent<Slider>();
            ShipCurrentSpeed = PlayerUIInstance.transform.Find("ShipCurrentSpeed").GetComponent<Text>();
            ShipTurningSpeed = PlayerUIInstance.transform.Find("ShipTurningSpeed").GetComponent<Slider>();

            UnitName.text = ActiveTarget.name;
            UnitHP.maxValue = StartingHP;
            UnitHP.value = CurrentHP;
            CheckHealthColor();
            ShipSpeedStep.value = SpeedStep;
            Score.text = CurrentScore;
        }
        private void OpenPlaneUI() {
            PlayerUIInstance = Instantiate(m_PlaneUI, PlayerUI.transform);

            Score = PlayerUIInstance.transform.Find("Score").GetComponent<Text>();
            UnitName = PlayerUIInstance.transform.Find("UnitName").GetComponent<Text>();
            UnitHP = PlayerUIInstance.transform.Find("UnitHealthSlider").GetComponent<Slider>();
            UnitHPColor = PlayerUIInstance.transform.Find("UnitHealthSlider").Find("Fill Area").Find("Fill").GetComponent<Image>();
            ShipSpeedStep = PlayerUIInstance.transform.Find("ShipSpeedStep").GetComponent<Slider>();
            ShipCurrentSpeed = PlayerUIInstance.transform.Find("ShipCurrentSpeed").GetComponent<Text>();
            ShipTurningSpeed = PlayerUIInstance.transform.Find("ShipTurningSpeed").GetComponent<Slider>();

            UnitName.text = ActiveTarget.name;
            UnitHP.maxValue = StartingHP;
            UnitHP.value = CurrentHP;
            CheckHealthColor();
            ShipSpeedStep.value = SpeedStep;
            Score.text = CurrentScore;
        }
        private void OpenShipUI() {
            PlayerUIInstance = Instantiate(m_ShipUI, PlayerUI.transform);

            Score = PlayerUIInstance.transform.Find("Score").GetComponent<Text>();
            UnitName = PlayerUIInstance.transform.Find("UnitName").GetComponent<Text>();
            UnitHP = PlayerUIInstance.transform.Find("UnitHealthSlider").GetComponent<Slider>();
            UnitHPColor = PlayerUIInstance.transform.Find("UnitHealthSlider").Find("Fill Area").Find("Fill").GetComponent<Image>();
            ShipSpeedStep = PlayerUIInstance.transform.Find("ShipSpeedStep").GetComponent<Slider>();
            ShipCurrentSpeed = PlayerUIInstance.transform.Find("ShipCurrentSpeed").GetComponent<Text>();
            ShipTurningSpeed = PlayerUIInstance.transform.Find("ShipTurningSpeed").GetComponent<Slider>();

            UnitName.text = ActiveTarget.name;
            UnitHP.maxValue = StartingHP;
            UnitHP.value = CurrentHP;
            CheckHealthColor();
            ShipSpeedStep.value = SpeedStep;
            Score.text = CurrentScore;
        }
        private void CloseGameUI() {
           if (PlayerUIInstance)
                Destroy (PlayerUIInstance); 
        }
        private void OpenTurretUI() {
            TurretUIInstance = Instantiate(m_TurretUI, PlayerUI.transform);

            DisplayTurretsArtilleryAimer = TurretUIInstance.transform.Find("AimerArtillery").gameObject;
            DisplayTurretsAAAimer = TurretUIInstance.transform.Find("AimerAA").gameObject;
            DisplayTurretsStatus = TurretUIInstance.transform.Find("TurretsStatus").gameObject;
            DisplayTurretsTargetRange = TurretUIInstance.transform.Find("TurretsTargetRange").GetComponent<Text>();
            DisplayTurretsAIControl = TurretUIInstance.transform.Find("TurretsAIControl").gameObject;
            DisplayTurretsCurrentArtillery = TurretUIInstance.transform.Find("TurretsCurrentShells").gameObject;
            DisplayTurretsCurrentAA = TurretUIInstance.transform.Find("TurretsCurrentAA").gameObject;
            DisplayTurretsCurrentTorpedoes = TurretUIInstance.transform.Find("TurretsCurrentTorpedoes").gameObject;
            DisplayTurretsCurrentDepthCharges = TurretUIInstance.transform.Find("TurretsCurrentDepthCharges").gameObject;
            CreateTurretsStatusDisplay();
            SetPlayerUITurretType(ActiveTarget.GetComponent<TurretManager>().GetCurrentTurretType());
            DisplayAITurretOverlay();
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
            // if (ActiveTarget.GetComponent<TurretManager>()) {
            //     CreateTurretsStatusDisplay();
            // }

            if (TargetType == "Tank") {
                if (ActiveTarget.GetComponent<TankHealth>())
                    StartingHP = ActiveTarget.GetComponent<TankHealth>().GetStartingHealth();
            } else if (TargetType == "Building") {
                if (ActiveTarget.GetComponent<BuildingHealth>())
                    StartingHP = ActiveTarget.GetComponent<BuildingHealth>().GetStartingHealth();
            } else if (TargetType == "Aircraft") {
                if (ActiveTarget.GetComponent<AircraftHealth>())
                    StartingHP = ActiveTarget.GetComponent<AircraftHealth>().GetStartingHealth();
            } else if (TargetType == "Ship") {
                StartingHP = ActiveTarget.GetComponent<ShipHealth>().GetStartingHealth();
                SetCurrentUnitHealth(ActiveTarget.GetComponent<ShipHealth>().GetCurrentHealth());
                ChangeSpeedStep(ActiveTarget.GetComponent<ShipMovement>().GetCurrentSpeedStep());
                CurrentUnitDead = ActiveTarget.GetComponent<ShipController>().GetDeath();
            }

            SetOpenUI();
        }

        public void SetPlayerUITurretType(TurretFireManager.TurretType currentControlledTurret) {
            if (TurretUIInstance) {
                // Debug.Log ("currentControlledTurret : "+ currentControlledTurret);
                DisplayTurretsArtilleryAimer.SetActive(false);
                DisplayTurretsAAAimer.SetActive(false);
                DisplayTurretsCurrentArtillery.SetActive(false);
                DisplayTurretsCurrentAA.SetActive(false);
                DisplayTurretsCurrentTorpedoes.SetActive(false);
                DisplayTurretsCurrentDepthCharges.SetActive(false);
                if (currentControlledTurret == TurretFireManager.TurretType.Artillery) {
                    DisplayTurretsArtilleryAimer.SetActive(true);
                    DisplayTurretsCurrentArtillery.SetActive(true);
                }
                if (currentControlledTurret == TurretFireManager.TurretType.AA) {
                    DisplayTurretsAAAimer.SetActive(true);
                    DisplayTurretsCurrentAA.SetActive(true);
                }
                if (currentControlledTurret == TurretFireManager.TurretType.Torpedo) {
                    DisplayTurretsArtilleryAimer.SetActive(true);
                    DisplayTurretsCurrentTorpedoes.SetActive(true);
                }
                if (currentControlledTurret == TurretFireManager.TurretType.DepthCharge) {
                    DisplayTurretsCurrentDepthCharges.SetActive(true);
                }
                CreateTurretsStatusDisplay();
            }
        }
        protected void CreateTurretsStatusDisplay() {
            // Remove previous iteration
            foreach (Transform child in DisplayTurretsStatus.transform) {
                GameObject.Destroy(child.gameObject);
            }
            if (ActiveTarget != null) {
                TurretStatus = ActiveTarget.GetComponent<TurretManager>().GetTurretsStatus();

                // Loop for each position
                for (int i = 0; i < TurretStatus.Count; i++) {
                    // Debug.Log ("position : "+ position);
                    GameObject turret = Instantiate(TurretStatusSprites, DisplayTurretsStatus.transform);
                }
                StartCoroutine(PauseAction());
            }
        }
        IEnumerator PauseAction(){
            yield return new WaitForSeconds(0.01f);
            ChangeIconsPosition();
        }
        protected void ChangeIconsPosition(){
            // Prepare the positions for the display, initial position will place the first icon, and the others will follow
            float position = 0;
            if (TurretStatus.Count % 2 == 0) {
                position = (TurretStatus.Count*IconsSpacing)/2 - (IconsSpacing/2);
            } else {
                position = (TurretStatus.Count*IconsSpacing)/2;
            }

            for (int i = 0; i < TurretStatus.Count; i++) {
                if (!DisplayTurretsStatus) {
                    continue;
                }
                if (DisplayTurretsStatus.transform.GetChild(i) == null)
                    continue;
                Vector3 positionning = DisplayTurretsStatus.transform.GetChild(i).transform.position;
                positionning.x = position;
                positionning.y = 0;
                DisplayTurretsStatus.transform.GetChild(i).transform.localPosition = positionning;
                position -= IconsSpacing;
                // Debug.Log ("TurretStatus[i] : "+ TurretStatus[i]);
                CreateSingleTurretStatusDisplay(TurretStatus[i], i);
            }
        }
        protected void CreateSingleTurretStatusDisplay(TurretManager.TurretStatusType status, int turretNumber) {
            // Debug.Log ("status : "+ status);
            if (DisplayTurretsStatus.transform.GetChild(turretNumber).transform.GetChild(0) != null) {
                DisplayTurretsStatus.transform.GetChild(turretNumber).transform.GetChild(0).GetComponent<Image>().enabled = false;         // Ready to fire
                DisplayTurretsStatus.transform.GetChild(turretNumber).transform.GetChild(1).GetComponent<Image>().enabled = false;         // Reloading
                DisplayTurretsStatus.transform.GetChild(turretNumber).transform.GetChild(2).GetComponent<Image>().enabled = false;         // Dead
                DisplayTurretsStatus.transform.GetChild(turretNumber).transform.GetChild(3).GetComponent<Image>().enabled = false;         // Not ok

                if (status == TurretManager.TurretStatusType.Ready) {
                    DisplayTurretsStatus.transform.GetChild(turretNumber).transform.GetChild(0).GetComponent<Image>().enabled = true;
                } else if (status == TurretManager.TurretStatusType.Reloading) {
                    DisplayTurretsStatus.transform.GetChild(turretNumber).transform.GetChild(1).GetComponent<Image>().enabled = true;
                } else if (status == TurretManager.TurretStatusType.Dead) {
                    DisplayTurretsStatus.transform.GetChild(turretNumber).transform.GetChild(2).GetComponent<Image>().enabled = true;
                } else {
                    DisplayTurretsStatus.transform.GetChild(turretNumber).transform.GetChild(3).GetComponent<Image>().enabled = true;
                }
            }
        }
        public void SetSingleTurretStatus(TurretManager.TurretStatusType status, int turretNumber) {
            if (TurretUIInstance) 
                CreateSingleTurretStatusDisplay(status, turretNumber);
        }

        public void SetPauseUI(bool pause) {
            if (pause){
                OpenPauseUI();
            } else {
                ClosePauseUI();
            }
        }

        public void SetFreeCamera(bool freeCamera) {
            FreeCamera = freeCamera;
            DisplayAITurretOverlay();
        }
        public void SetOverlayUI(bool overlayUI){
            OverlayUI = overlayUI;
            DisplayAITurretOverlay();
        }
        private void DisplayAITurretOverlay(){
            if (TurretUIInstance) {
                if (FreeCamera || OverlayUI) {
                    DisplayTurretsAAAimer.GetComponent<Image>().enabled = false;
                    DisplayTurretsArtilleryAimer.GetComponent<Image>().enabled = false;
                    DisplayTurretsAIControl.GetComponent<Image>().enabled = true;
                } else {
                    DisplayTurretsAAAimer.GetComponent<Image>().enabled = true;
                    DisplayTurretsArtilleryAimer.GetComponent<Image>().enabled = true;
                    DisplayTurretsAIControl.GetComponent<Image>().enabled = false;
                }
            }
        }

        private void CheckHealthColor() {
        // updates health bar color depending on the health of the current unit
        Color color;
        if (CurrentHP <= (0.2f * StartingHP)) {
            color = Color.red;
        } else if (CurrentHP <= (0.6f * StartingHP)) {
            color = Color.yellow;
        } else {
            color = new Color(0.0f, 0.75f, 0.14f);
        }
        if (PlayerUIInstance)
            UnitHPColor.color = color;
    }

        protected void OpenPauseUI(){
            PauseUIInstance = Instantiate(m_PauseUI, PlayerUI.transform);

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

        private bool ShellCameraUsed = false;
        public void SendPlayerShellToUI(GameObject shellInstance){ if (!ShellCameraUsed) { CreatePlayerShellUI(shellInstance); } }
        private void CreatePlayerShellUI(GameObject shellInstance) {
            ShellCameraUsed = true;
            GameObject shellCamera = Instantiate (m_ShellCamera, shellInstance.transform);
            shellInstance.GetComponent<ShellStat>().SetIsFollowedByCamera(PlayerManager, shellCamera, m_TimeToDestroyCamera);
        }
        public void ShellFollowedByCameraDestroyed() { StartCoroutine(WaitForDestroy()); }
        IEnumerator WaitForDestroy(){
            yield return new WaitForSeconds(m_TimeToDestroyCamera);
            ShellCameraUsed = false;
        }
        public void SetPlayerCanvas(GameObject playerCanvas) { PlayerCanvas = playerCanvas; }
        public void SetFreeLookCamera(FreeLookCam freeLookCam){ FreeLookCam = freeLookCam; }
        public void SetMap(bool map) {
            DisplayMapUI = map;
            if (!CurrentUnitDead)
                DisplayGameUI = !map;
            SetOpenUI();
        }
        public void SetScoreMessage(string message) {
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
        public void SetHideUI() {
            DisplayUI = !DisplayUI;
            SetOpenUI();
            PlayerCanvas.SetActive(DisplayUI);
        }
        public void SetCurrentUnitDead(bool isUnitDead) {
            CurrentUnitDead = isUnitDead;
            DisplayGameUI = !isUnitDead;
            SetOpenUI();
        }
        public void Reset() {
            CurrentUnitDead = false;
        }
    }
}