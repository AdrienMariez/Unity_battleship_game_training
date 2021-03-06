﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FreeLookCamera;
using Crest;

namespace UI {
    public class UIManager : MonoBehaviour {
        // Each instance of this class is linked to a single PlayerManager. Each player has his UI set independantly this way.
        [Header("Units UI")]
            private GameObject PlayerUIInstance;
            private GameObject TurretUIInstance;
            private GameObject PlayerUI;        // Refers to the gameObject all UI elements will be put in
            private GameObject PauseUIInstance;
        [Header("Turrets status icons")]
            private float IconsSpacing = 22;
        [Header("Shell Camera")]
            private float TimeToDestroyCamera = 3;
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
            const string ShipCurrentSpeedDisplay = "{0} m/s";
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
        private CompiledTypes.Units_categories.RowValues CurrentPlayedUnitCategory; public void SetCurrentPlayedUnitCategory(CompiledTypes.Units_categories.RowValues _E) { CurrentPlayedUnitCategory = _E; } 
        private bool CurrentUnitDead;
        private bool FreeCamera = false;
        private bool OverlayUI = false;
        private bool DisplayGameUI = true;
        private bool DisplayMapUI = false;
        private bool DisplayUI = true;
        private GameObject PlayerCanvas;
        private PlayerManager PlayerManager;
        private FreeLookCam FreeLookCam;
        private Vector3 AimerPosition;                              // Position of the target reticule
        CompiledTypes.Weapons_roles.RowValues CurrentControlledWeapon;
        private WorldUIVariables WorldUIVariables;

        private void Awake() {
            PlayerUI = GameObject.Find("UI");
            IconsSpacing = WorldUIVariables.GetIconsSpacing();
            TimeToDestroyCamera = WorldGlobals.GetTimeToDestroyCamera();
        }
        private void LateUpdate() {
            if (DisplayGameUI && ActiveTarget != null && DisplayUI) {
                float CurrentSpeed = Mathf.Round(ActiveTarget.GetComponent<Rigidbody>().velocity.magnitude);
                if (ShipCurrentSpeed != null)
                    ShipCurrentSpeed.text = string.Format(ShipCurrentSpeedDisplay, CurrentSpeed);

                if (CurrentPlayedUnitCategory == CompiledTypes.Units_categories.RowValues.ground) {
                    CurrentHP = Mathf.Round(ActiveTarget.GetComponent<TankHealth>().GetCurrentHealth());
                    if (ActiveTarget.GetComponent<TurretManager>()) {
                        // Heavy load ! to change if tanks are worked on !
                        CreateTurretsStatusDisplay();
                    }
                }

                if (TurretUIInstance) {
                    TurretTargetRange = ActiveTarget.GetComponent<TurretManager>().GetTargetRange();
                    if (TurretTargetRange > 999) {
                        TurretTargetRange = (Mathf.Round(TurretTargetRange / 100)) / 10f;
                        DisplayTurretsTargetRange.text = string.Format(TurretsTargetRangeDisplayKilometer, TurretTargetRange);
                    } else {
                        DisplayTurretsTargetRange.text = string.Format(TurretsTargetRangeDisplayMeter, Mathf.Round(TurretTargetRange));
                    }

                    if (CurrentControlledWeapon == CompiledTypes.Weapons_roles.RowValues.NavalArtillery) {
                        DisplayTurretsArtilleryAimer.transform.position = new Vector2(AimerPosition.x, AimerPosition.y);
                    } else if (CurrentControlledWeapon == CompiledTypes.Weapons_roles.RowValues.Artillery) {
                        DisplayTurretsAAAimer.transform.position = new Vector2(AimerPosition.x, AimerPosition.y);
                    } else if (CurrentControlledWeapon == CompiledTypes.Weapons_roles.RowValues.AntiAir) {
                        DisplayTurretsAAAimer.transform.position = new Vector2(AimerPosition.x, AimerPosition.y);
                    } else if (CurrentControlledWeapon == CompiledTypes.Weapons_roles.RowValues.Torpedo) {
                        DisplayTurretsArtilleryAimer.transform.position = new Vector2(AimerPosition.x, AimerPosition.y);
                    } else {
                        DisplayTurretsAAAimer.transform.position = new Vector2(AimerPosition.x, AimerPosition.y);
                    }
                }
                // else if (true) {
                //     CloseTurretUI();
                // }
            }
        }

        private void SetOpenUI() {
            CloseGameUI();
            CloseTurretUI();
            if (DisplayGameUI && !DisplayMapUI && DisplayUI) {
                // Debug.Log("SetOpenUI :  - CurrentPlayedUnitCategory = "+ CurrentPlayedUnitCategory);
                if (CurrentPlayedUnitCategory == CompiledTypes.Units_categories.RowValues.ground) {
                    OpenTankUI();
                } else if (CurrentPlayedUnitCategory == CompiledTypes.Units_categories.RowValues.building) {
                    OpenBuildingUI();
                }else if (CurrentPlayedUnitCategory == CompiledTypes.Units_categories.RowValues.aircraft) {
                    OpenPlaneUI();
                } else if (CurrentPlayedUnitCategory == CompiledTypes.Units_categories.RowValues.ship) {
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
            PlayerUIInstance = Instantiate(WorldUIVariables.GetTankUI(), PlayerUI.transform);

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
            PlayerUIInstance = Instantiate(WorldUIVariables.GetBuildingUI(), PlayerUI.transform);

            Score = PlayerUIInstance.transform.Find("Score").GetComponent<Text>();
            UnitName = PlayerUIInstance.transform.Find("UnitName").GetComponent<Text>();
            UnitHP = PlayerUIInstance.transform.Find("UnitHealthSlider").GetComponent<Slider>();
            UnitHPColor = PlayerUIInstance.transform.Find("UnitHealthSlider").Find("Fill Area").Find("Fill").GetComponent<Image>();

            UnitName.text = ActiveTarget.name;
            UnitHP.maxValue = StartingHP;
            UnitHP.value = CurrentHP;
            CheckHealthColor();
            Score.text = CurrentScore;
        }
        private void OpenPlaneUI() {
            PlayerUIInstance = Instantiate(WorldUIVariables.GetPlaneUI(), PlayerUI.transform);

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
            PlayerUIInstance = Instantiate(WorldUIVariables.GetShipUI(), PlayerUI.transform);

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
            TurretUIInstance = Instantiate(WorldUIVariables.GetTurretUI(), PlayerUI.transform);

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
            SetPlayerUITurretRole(ActiveTarget.GetComponent<TurretManager>().GetCurrentTurretRole());
            DisplayAITurretOverlay();
        }
        private void CloseTurretUI() {
           if (TurretUIInstance)
                Destroy (TurretUIInstance); 
        }
        private void OpenMapUI() {
            PlayerUIInstance = Instantiate(WorldUIVariables.GetPlayerMapUI(), PlayerUI.transform);

            Score = PlayerUIInstance.transform.Find("Score").GetComponent<Text>();
            UnitName = PlayerUIInstance.transform.Find("UnitName").GetComponent<Text>();
            UnitHP = PlayerUIInstance.transform.Find("UnitHealthSlider").GetComponent<Slider>();
            if (ActiveTarget != null)
                UnitName.text = ActiveTarget.name;
                UnitHP.maxValue = StartingHP;
                UnitHP.value = CurrentHP;
            Score.text = CurrentScore;
        }


        public void SetActiveTarget(GameObject targetModel, UnitMasterController unitController) {
            ActiveTarget = targetModel;

            SetOpenUI();
            StartingHP = unitController.GetStartingHealth();
            SetCurrentUnitHealth(unitController.GetCurrentHealth());
            CurrentUnitDead = unitController.GetDeath();
            ChangeSpeedStep(unitController.GetCurrentSpeedStep());
        }

        public void SetPlayerUITurretRole(CompiledTypes.Weapons_roles.RowValues currentControlledTurret) {
            if (TurretUIInstance) {
                CurrentControlledWeapon = currentControlledTurret;
                // Debug.Log ("currentControlledTurret : "+ currentControlledTurret);
                DisplayTurretsArtilleryAimer.SetActive(false);
                DisplayTurretsAAAimer.SetActive(false);
                DisplayTurretsCurrentArtillery.SetActive(false);
                DisplayTurretsCurrentAA.SetActive(false);
                DisplayTurretsCurrentTorpedoes.SetActive(false);
                DisplayTurretsCurrentDepthCharges.SetActive(false);
                if (CurrentControlledWeapon == CompiledTypes.Weapons_roles.RowValues.NavalArtillery) {
                    DisplayTurretsArtilleryAimer.SetActive(true);
                    DisplayTurretsCurrentArtillery.SetActive(true);
                } else if (CurrentControlledWeapon == CompiledTypes.Weapons_roles.RowValues.Artillery) {
                    DisplayTurretsAAAimer.SetActive(true);
                    DisplayTurretsCurrentArtillery.SetActive(true);
                } else if (CurrentControlledWeapon == CompiledTypes.Weapons_roles.RowValues.AntiAir) {
                    DisplayTurretsAAAimer.SetActive(true);
                    DisplayTurretsCurrentAA.SetActive(true);
                } else if (CurrentControlledWeapon == CompiledTypes.Weapons_roles.RowValues.Torpedo) {
                    DisplayTurretsArtilleryAimer.SetActive(true);
                    DisplayTurretsCurrentTorpedoes.SetActive(true);
                } else if (CurrentControlledWeapon == CompiledTypes.Weapons_roles.RowValues.DepthCharge) {
                    DisplayTurretsCurrentDepthCharges.SetActive(true);
                } else {
                    DisplayTurretsAAAimer.SetActive(true);
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
                // for (int i = 0; i < TurretStatus.Count; i++) {
                //     // Debug.Log ("position : "+ position);
                //     GameObject turret = Instantiate(WorldUIVariables.GetTurretStatusSprites(), DisplayTurretsStatus.transform);
                // }
                foreach (TurretManager.TurretStatusType turret in TurretStatus) {
                    Instantiate(WorldUIVariables.GetTurretStatusSprites(), DisplayTurretsStatus.transform);
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
                    // Debug.Log ("case 1 AI : "+FreeCamera+" / "+ OverlayUI);
                    DisplayTurretsAAAimer.GetComponent<Image>().enabled = false;
                    DisplayTurretsArtilleryAimer.GetComponent<Image>().enabled = false;
                    DisplayTurretsAIControl.GetComponent<Image>().enabled = true;
                } else {
                    // Debug.Log ("case 2 Normal : "+FreeCamera+" / "+ OverlayUI);
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
            PauseUIInstance = Instantiate(WorldUIVariables.GetPauseMenu(), PlayerUI.transform);

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
            GameObject shellCamera = Instantiate (WorldGlobals.GetShellCamera(), shellInstance.transform);
            shellInstance.GetComponent<ShellStat>().SetIsFollowedByCamera(PlayerManager, shellCamera, TimeToDestroyCamera);
        }
        public void ShellFollowedByCameraDestroyed() { StartCoroutine(WaitForDestroy()); }
        IEnumerator WaitForDestroy(){
            yield return new WaitForSeconds(TimeToDestroyCamera);
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
            if (PlayerUIInstance) {
                if (CurrentPlayedUnitCategory == CompiledTypes.Units_categories.RowValues.ship) {
                    ShipSpeedStep.value = SpeedStep;
                }
            }
        }
        public void SetRotationInput(float rotation){
            CurrentRotation = rotation;
            if (PlayerUIInstance) {
                if (CurrentPlayedUnitCategory == CompiledTypes.Units_categories.RowValues.ship) {
                    ShipTurningSpeed.value = CurrentRotation;
                }
            }
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
        public void SetAimerPosition(Vector3 aimerPosition) {
            AimerPosition = aimerPosition;
        }
        public void Reset() {
            CurrentUnitDead = false;
        }
    }
}