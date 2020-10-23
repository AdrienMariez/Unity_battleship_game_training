using UnityEngine;
using UnityEngine.UI;

public class ShipDamageControl : UnitParameter {
    private bool Active = false;
    private bool Pause = false;
    private bool ShipDead = false;
    private bool MapActive = false;
    private GameObject DamageControlInstance;

    private GameObject DamageControlAlertInstance;
    public float RepairRate = 3;
    public int RepairCrew = 3;

    private ShipController ShipController;

    private int EngineRepairCrew = 0;
    private int FireRepairCrew = 0;
    private int WaterRepairCrew = 0;
    private int TurretsRepairCrew = 0;
    private int UnsetCrew = 0;
    private Text EngineCrewText;
    private Text FireCrewText;
    private Text WaterCrewText;
    private Text TurretsCrewText;
    private Text UnsetCrewText;
    private string UnitName;

    private Text DamagingHitTaken;
    private int DamagingHitTakenCount = 0;
    private Text BouncedHitTaken;
    private int BouncedHitTakenCount = 0;
    private Text DamagingHitDealt;
    private int DamagingHitDealtCount = 0;
    private Text BouncedHitDealt;
    private int BouncedHitDealtCount = 0;

    private int EngineStatus = 0;
    private bool SteeringStatus = false;
    private bool FireStatus = false;
    private bool WaterStatus = false;
    private int TotalTurrets = 0;
    private int DamagedTurrets = 0;

    private bool DmgCtrlOpen = false;


    public void BeginOperations(ShipController unitController) {
        UnsetCrew = RepairCrew;

        ShipController = unitController;
        // TurretsRepairCrew += RepairCrew;

        ShipController.SetDamageControlEngine(EngineRepairCrew);
        ShipController.SetDamageControlFire(FireRepairCrew);
        ShipController.SetDamageControlWater(WaterRepairCrew);
        ShipController.SetDamageControlTurrets(TurretsRepairCrew);
        ShipController.SetDamageControlUnset(UnsetCrew);
    }

    protected void Update() {
        if (Active && !MapActive && !ShipDead && !Pause) {
            if (Input.GetButtonDown ("OpenDamageControl")) 
                SetOpenDmgCtrl(!DmgCtrlOpen);
        }
    }
    protected void OpenDmgCtrl() {
        // Cursor.lockState = CursorLockMode.None;
        // Cursor.visible = true;

        DamageControlInstance = Instantiate(WorldUIVariables.GetShipDamageControlUI());
        // DamageControlInstance.transform.Find("UnitName").GetComponent<Text>().text = UnitName;

        EngineCrewText = DamageControlInstance.transform.Find("EngineCrewCount").GetComponent<Text>();
        FireCrewText = DamageControlInstance.transform.Find("FireCrewCount").GetComponent<Text>();
        WaterCrewText = DamageControlInstance.transform.Find("WaterCrewCount").GetComponent<Text>();
        TurretsCrewText = DamageControlInstance.transform.Find("TurretsCrewCount").GetComponent<Text>();
        UnsetCrewText = DamageControlInstance.transform.Find("UnsetCrewCount").GetComponent<Text>();

        Button buttonEnginePos = DamageControlInstance.transform.Find("buttonEnginePos").GetComponent<Button>();
		buttonEnginePos.onClick.AddListener(ButtonEnginePosOnClick);
        Button buttonEngineNeg = DamageControlInstance.transform.Find("buttonEngineNeg").GetComponent<Button>();
		buttonEngineNeg.onClick.AddListener(ButtonEngineNegOnClick);

        Button buttonFirePos = DamageControlInstance.transform.Find("buttonFirePos").GetComponent<Button>();
		buttonFirePos.onClick.AddListener(ButtonFirePosOnClick);
        Button buttonFireNeg = DamageControlInstance.transform.Find("buttonFireNeg").GetComponent<Button>();
		buttonFireNeg.onClick.AddListener(ButtonFireNegOnClick);

        Button buttonWaterPos = DamageControlInstance.transform.Find("buttonWaterPos").GetComponent<Button>();
		buttonWaterPos.onClick.AddListener(ButtonWaterPosOnClick);
        Button buttonWaterNeg = DamageControlInstance.transform.Find("buttonWaterNeg").GetComponent<Button>();
		buttonWaterNeg.onClick.AddListener(ButtonWaterNegOnClick);

        Button buttonTurretsPos = DamageControlInstance.transform.Find("buttonTurretsPos").GetComponent<Button>();
		buttonTurretsPos.onClick.AddListener(ButtonTurretsPosOnClick);
        Button buttonTurretsNeg = DamageControlInstance.transform.Find("buttonTurretsNeg").GetComponent<Button>();
		buttonTurretsNeg.onClick.AddListener(ButtonTurretsNegOnClick);

        DamagingHitTaken = DamageControlInstance.transform.Find("ShellsPenetratedTaken").GetComponent<Text>();
        BouncedHitTaken = DamageControlInstance.transform.Find("ShellsBouncedTaken").GetComponent<Text>();
        DamagingHitDealt = DamageControlInstance.transform.Find("ShellsPenetratedDealt").GetComponent<Text>();
        BouncedHitDealt = DamageControlInstance.transform.Find("ShellsBouncedDealt").GetComponent<Text>();

        SetCrewDisplay();
        DisplayDamagedEngine();
        DisplayDamagedSteering();
        DisplayFireBurning();
        DisplayBuoyancyCompromised();
        DisplayDamagedTurrets();

        CheckActiveButtons();

        SetShellDisplay();
    }

    protected void CloseDmgCtrl() {
        if (DamageControlInstance)
            Destroy (DamageControlInstance);
    } 

    protected void CheckActiveButtons() {
        SetCrewDisplay();

        if (EngineRepairCrew == RepairCrew) {
            DamageControlInstance.transform.Find("buttonEnginePos").gameObject.SetActive(false);
        } else {
            DamageControlInstance.transform.Find("buttonEnginePos").gameObject.SetActive(true);
        }
        if (EngineRepairCrew > 0) {
            DamageControlInstance.transform.Find("buttonEngineNeg").gameObject.SetActive(true);
        } else {
            DamageControlInstance.transform.Find("buttonEngineNeg").gameObject.SetActive(false);
        }

        if (FireRepairCrew == RepairCrew) {
            DamageControlInstance.transform.Find("buttonFirePos").gameObject.SetActive(false);
        } else {
            DamageControlInstance.transform.Find("buttonFirePos").gameObject.SetActive(true);
        }
        if (FireRepairCrew > 0) {
            DamageControlInstance.transform.Find("buttonFireNeg").gameObject.SetActive(true);
        } else {
            DamageControlInstance.transform.Find("buttonFireNeg").gameObject.SetActive(false);
        }

        if (WaterRepairCrew == RepairCrew) {
            DamageControlInstance.transform.Find("buttonWaterPos").gameObject.SetActive(false);
        } else {
            DamageControlInstance.transform.Find("buttonWaterPos").gameObject.SetActive(true);
        }
        if (WaterRepairCrew > 0) {
            DamageControlInstance.transform.Find("buttonWaterNeg").gameObject.SetActive(true);
        } else {
            DamageControlInstance.transform.Find("buttonWaterNeg").gameObject.SetActive(false);
        }

        if (TurretsRepairCrew == RepairCrew) {
            DamageControlInstance.transform.Find("buttonTurretsPos").gameObject.SetActive(false);
        } else {
            DamageControlInstance.transform.Find("buttonTurretsPos").gameObject.SetActive(true);
        }
        if (TurretsRepairCrew > 0) {
            DamageControlInstance.transform.Find("buttonTurretsNeg").gameObject.SetActive(true);
        } else {
            DamageControlInstance.transform.Find("buttonTurretsNeg").gameObject.SetActive(false);
        }

        ShipController.SetDamageControlUnset(UnsetCrew);

        // if (UnsetCrew > 0) {
            // DamageControlInstance.transform.Find("buttonEnginePos").gameObject.SetActive(true);
            // DamageControlInstance.transform.Find("buttonFirePos").gameObject.SetActive(true);
            // DamageControlInstance.transform.Find("buttonWaterPos").gameObject.SetActive(true);
            // DamageControlInstance.transform.Find("buttonTurretsPos").gameObject.SetActive(true);
        // } else {
        //     DamageControlInstance.transform.Find("buttonEnginePos").gameObject.SetActive(false);
        //     DamageControlInstance.transform.Find("buttonFirePos").gameObject.SetActive(false);
        //     DamageControlInstance.transform.Find("buttonWaterPos").gameObject.SetActive(false);
        //     DamageControlInstance.transform.Find("buttonTurretsPos").gameObject.SetActive(false);
        // }
    }

    void ButtonEnginePosOnClick(){
        // UnsetCrew -= 1;
        if (UnsetCrew > 0) {
            UnsetCrew -= 1;
        } else {
            if (TurretsRepairCrew > 0) {
                TurretsRepairCrew -= 1;
            } else if (WaterRepairCrew > 0) {
                WaterRepairCrew -= 1;
            } else {
                FireRepairCrew -= 1;
            }
        }
        EngineRepairCrew += 1;
        CheckActiveButtons();
        ShipController.SetDamageControlEngine(EngineRepairCrew);
    }
    void ButtonEngineNegOnClick(){
        UnsetCrew += 1;
        EngineRepairCrew -= 1;
        CheckActiveButtons();
        ShipController.SetDamageControlEngine(EngineRepairCrew);
    }
    void ButtonFirePosOnClick(){
        if (UnsetCrew > 0) {
            UnsetCrew -= 1;
        } else {
            if (TurretsRepairCrew > 0) {
                TurretsRepairCrew -= 1;
            } else if (EngineRepairCrew > 0) {
                EngineRepairCrew -= 1;
            } else {
                WaterRepairCrew -= 1;
            }
        }
        FireRepairCrew += 1;
        CheckActiveButtons();
        ShipController.SetDamageControlFire(FireRepairCrew);
    }
    void ButtonFireNegOnClick(){
        UnsetCrew += 1;
        FireRepairCrew -= 1;
        CheckActiveButtons();
        ShipController.SetDamageControlFire(FireRepairCrew);
    }
    void ButtonWaterPosOnClick(){
        if (UnsetCrew > 0) {
            UnsetCrew -= 1;
        } else {
            if (TurretsRepairCrew > 0) {
                TurretsRepairCrew -= 1;
            } else if (EngineRepairCrew > 0) {
                EngineRepairCrew -= 1;
            } else {
                FireRepairCrew -= 1;
            }
        }
        WaterRepairCrew += 1;
        CheckActiveButtons();
        ShipController.SetDamageControlWater(WaterRepairCrew);
    }
    void ButtonWaterNegOnClick(){
        UnsetCrew += 1;
        WaterRepairCrew -= 1;
        CheckActiveButtons();
        ShipController.SetDamageControlWater(WaterRepairCrew);
    }
    void ButtonTurretsPosOnClick(){
        if (UnsetCrew > 0) {
            UnsetCrew -= 1;
        } else {
            if (EngineRepairCrew > 0) {
                EngineRepairCrew -= 1;
            } else if (WaterRepairCrew > 0) {
                WaterRepairCrew -= 1;
            } else {
                FireRepairCrew -= 1;
            }
        }
        TurretsRepairCrew += 1;
        CheckActiveButtons();
        ShipController.SetDamageControlTurrets(TurretsRepairCrew);
    }
    void ButtonTurretsNegOnClick(){
        UnsetCrew += 1;
        TurretsRepairCrew -= 1;
        CheckActiveButtons();
        ShipController.SetDamageControlTurrets(TurretsRepairCrew);
    }

    private void SetAlertsDamageControlDisplay() {
        // Debug.Log("SetAlertsDamageControlDisplay()");
        if (Active && !ShipDead && EngineStatus > 0 || Active && !ShipDead && SteeringStatus || Active && !ShipDead && FireStatus || Active && !ShipDead && WaterStatus || Active && !ShipDead && DamagedTurrets > 0) {
            if (!DamageControlAlertInstance) {
                DamageControlAlertInstance = Instantiate(WorldUIVariables.GetShipDamageControlAlertUI());
            }
            if (EngineStatus == 2) {
                DamageControlAlertInstance.transform.Find("EngineDestroyed").gameObject.SetActive(true);
            } else {
                DamageControlAlertInstance.transform.Find("EngineDestroyed").gameObject.SetActive(false);
            }
            if (EngineStatus == 1) {
                DamageControlAlertInstance.transform.Find("EngineDamaged").gameObject.SetActive(true);
            } else {
                DamageControlAlertInstance.transform.Find("EngineDamaged").gameObject.SetActive(false);
            }
            if (SteeringStatus) {
                DamageControlAlertInstance.transform.Find("SteeringDamaged").gameObject.SetActive(true);
            } else {
                DamageControlAlertInstance.transform.Find("SteeringDamaged").gameObject.SetActive(false);
            }
            if (FireStatus) {
                DamageControlAlertInstance.transform.Find("FireDamaged").gameObject.SetActive(true);
            } else {
                DamageControlAlertInstance.transform.Find("FireDamaged").gameObject.SetActive(false);
            }
            if (WaterStatus) {
                DamageControlAlertInstance.transform.Find("WaterDamaged").gameObject.SetActive(true);
            } else {
                DamageControlAlertInstance.transform.Find("WaterDamaged").gameObject.SetActive(false);
            }
            if (DamagedTurrets > 0) {
                DamageControlAlertInstance.transform.Find("TurretsDamaged").gameObject.SetActive(true);
            } else {
                DamageControlAlertInstance.transform.Find("TurretsDamaged").gameObject.SetActive(false);
            }
        } else {
            if (DamageControlAlertInstance)
                Destroy (DamageControlAlertInstance);
        }
    }

    private void SetOpenDmgCtrl(bool open) {
        DmgCtrlOpen = open;
        ShipController.SetDamageControl(DmgCtrlOpen);
        if (DmgCtrlOpen) {
            OpenDmgCtrl();
        } else {
            CloseDmgCtrl();
        }
    }
    protected void SetCrewDisplay() {
        EngineCrewText.text = EngineRepairCrew.ToString("g");
        FireCrewText.text = FireRepairCrew.ToString("g");
        WaterCrewText.text = WaterRepairCrew.ToString("g");
        TurretsCrewText.text = TurretsRepairCrew.ToString("g");
        UnsetCrewText.text = UnsetCrew.ToString("g");
    }
    protected void SetShellDisplay() {
        // string a = "Damaging hits taken : ";
        // a += DamagingHitTakenCount.ToString("g");
        // DamagingHitTaken.text = a;
        // string b = "Shell bounced : ";
        // b += BouncedHitTakenCount.ToString("g");
        // BouncedHitTaken.text = b;
        DamagingHitTaken.text = DamagingHitTakenCount.ToString("g");
        BouncedHitTaken.text = BouncedHitTakenCount.ToString("g");
        DamagingHitDealt.text = DamagingHitDealtCount.ToString("g");
        BouncedHitDealt.text = BouncedHitDealtCount.ToString("g");
    }
    public void SetActive(bool activate) {
        Active = activate;
        SetAlertsDamageControlDisplay();
        if (!Active)
            SetOpenDmgCtrl(false);
    }
    public void SetShipDeath(bool deathStatus) {
        ShipDead = deathStatus;
        if (ShipDead){
            SetOpenDmgCtrl(false);
            if (DamageControlAlertInstance)
                Destroy (DamageControlAlertInstance);
        }
    }
    public void SetMap(bool map) {
        MapActive = map;
        if (MapActive)
            SetOpenDmgCtrl(false);
    }
    public void SetPause(){
        Pause = !Pause;
    }
    public void SetName(string name) { UnitName = name; }
    public void SetDamagedEngine(int status){
        EngineStatus = status;
        SetAlertsDamageControlDisplay();
        if (DmgCtrlOpen)
            DisplayDamagedEngine();
    }
    private void DisplayDamagedEngine(){
        // Status : 0 : fixed and running / 1 : damaged / 2 : dead
        if (EngineStatus == 2) {
            DamageControlInstance.transform.Find("EngineOk").gameObject.SetActive(false);
            DamageControlInstance.transform.Find("EngineDamaged").gameObject.SetActive(false);
            DamageControlInstance.transform.Find("EngineDestroyed").gameObject.SetActive(true);
        } else if (EngineStatus == 1) {
            DamageControlInstance.transform.Find("EngineOk").gameObject.SetActive(false);
            DamageControlInstance.transform.Find("EngineDamaged").gameObject.SetActive(true);
            DamageControlInstance.transform.Find("EngineDestroyed").gameObject.SetActive(false);
        } else {
            DamageControlInstance.transform.Find("EngineOk").gameObject.SetActive(true);
            DamageControlInstance.transform.Find("EngineDamaged").gameObject.SetActive(false);
            DamageControlInstance.transform.Find("EngineDestroyed").gameObject.SetActive(false);
        }
    }
    public void SetDamagedSteering(bool status){
        SteeringStatus = status;
        SetAlertsDamageControlDisplay();
        if (DmgCtrlOpen)
            DisplayDamagedSteering();
    }
    private void DisplayDamagedSteering(){
        // Status : false : fixed and running / true : damaged
        if (SteeringStatus) {
            DamageControlInstance.transform.Find("SteeringDamaged").gameObject.SetActive(true);
            DamageControlInstance.transform.Find("SteeringOk").gameObject.SetActive(false);
        } else {
            DamageControlInstance.transform.Find("SteeringDamaged").gameObject.SetActive(false);
            DamageControlInstance.transform.Find("SteeringOk").gameObject.SetActive(true);
        }
    }
    public void SetFireBurning(bool status){
        FireStatus = status;
        SetAlertsDamageControlDisplay();
        if (DmgCtrlOpen)
            DisplayFireBurning();
    }
    private void DisplayFireBurning(){
        if (FireStatus) {
            DamageControlInstance.transform.Find("FireDamaged").gameObject.SetActive(true);
            DamageControlInstance.transform.Find("FireOk").gameObject.SetActive(false);
        } else {
            DamageControlInstance.transform.Find("FireDamaged").gameObject.SetActive(false);
            DamageControlInstance.transform.Find("FireOk").gameObject.SetActive(true);
        }
    }
    public void SetBuoyancyCompromised(bool status){
        WaterStatus = status;
        SetAlertsDamageControlDisplay();
        if (DmgCtrlOpen)
            DisplayBuoyancyCompromised();
    }
    private void DisplayBuoyancyCompromised(){
        if (WaterStatus) {
            DamageControlInstance.transform.Find("WaterDamaged").gameObject.SetActive(true);
            DamageControlInstance.transform.Find("WaterOk").gameObject.SetActive(false);
        } else {
            DamageControlInstance.transform.Find("WaterDamaged").gameObject.SetActive(false);
            DamageControlInstance.transform.Find("WaterOk").gameObject.SetActive(true);
        }
    }
    public void SetTotalTurrets(int turrets){
        TotalTurrets = turrets;
        DamagedTurrets = 0;
    }
    public void SetDamagedTurrets(int turrets){
        DamagedTurrets = turrets;
        SetAlertsDamageControlDisplay();
        if (DmgCtrlOpen)  
            DisplayDamagedTurrets();
    }
    private void DisplayDamagedTurrets(){
        if (DamagedTurrets > 0) {
            DamageControlInstance.transform.Find("TurretsDamaged").gameObject.SetActive(true);
            DamageControlInstance.transform.Find("TurretsOk").gameObject.SetActive(false);
        } else {
            DamageControlInstance.transform.Find("TurretsDamaged").gameObject.SetActive(false);
            DamageControlInstance.transform.Find("TurretsOk").gameObject.SetActive(true);
        }
    }
    public void UpdateShellsReceivedCounter (bool armorPenetrated) {
        if (armorPenetrated) {
            DamagingHitTakenCount++;
        } else {
            BouncedHitTakenCount++;
        }
        if (DmgCtrlOpen)
            SetShellDisplay();
    }
    public void UpdateShellsSentCounter (bool armorPenetrated) {
        if (armorPenetrated) {
            DamagingHitDealtCount++;
        } else {
            BouncedHitDealtCount++;
        }
        if (DmgCtrlOpen)
            SetShellDisplay();
    }

    public float GetRepairRate() { return RepairRate; }

    public void Destroy() {
        if (DamageControlAlertInstance)
                Destroy (DamageControlAlertInstance);
        SetOpenDmgCtrl(false);
    }
}