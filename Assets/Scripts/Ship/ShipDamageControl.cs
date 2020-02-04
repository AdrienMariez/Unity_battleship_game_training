using UnityEngine;
using UnityEngine.UI;

public class ShipDamageControl : MonoBehaviour {
    private bool Active = false;
    private bool Pause = false;
    private bool ShipDead = false;
    private bool MapActive = false;
    public GameObject m_DamageControlUI;
    public float RepairRate = 3;
    public float RepairCrew = 3;

    private ShipController ShipController;

    private float EngineRepairCrew = 0;
    private float FireRepairCrew = 0;
    private float WaterRepairCrew = 0;
    private float TurretsRepairCrew = 0;
    private float UnsetCrew = 0;
    private string UnitName;

    private int EngineStatus = 0;
    private bool SteeringStatus = false;
    private bool FireStatus = false;
    private bool WaterStatus = false;
    private int TotalTurrets = 0;
    private int DamagedTurrets = 0;

    private bool DmgCtrlOpen = false;
    private GameObject DamageControlInstance;


    private void Awake() {
        UnsetCrew = RepairCrew;

        ShipController = GetComponent<ShipController>();
        // TurretsRepairCrew += RepairCrew;

        ShipController.SetDamageControlEngine(EngineRepairCrew);
        ShipController.SetDamageControlFire(FireRepairCrew);
        ShipController.SetDamageControlWater(WaterRepairCrew);
        ShipController.SetDamageControlTurrets(TurretsRepairCrew);
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

        DamageControlInstance = Instantiate(m_DamageControlUI);
        DamageControlInstance.transform.Find("UnitName").GetComponent<Text>().text = UnitName;
        DamageControlInstance.transform.Find("EngineCrewCount").GetComponent<Text>().text = EngineRepairCrew.ToString("g");
        DamageControlInstance.transform.Find("FireCrewCount").GetComponent<Text>().text = FireRepairCrew.ToString("g");
        DamageControlInstance.transform.Find("WaterCrewCount").GetComponent<Text>().text = WaterRepairCrew.ToString("g");
        DamageControlInstance.transform.Find("TurretsCrewCount").GetComponent<Text>().text = TurretsRepairCrew.ToString("g");
        DamageControlInstance.transform.Find("UnsetCrewCount").GetComponent<Text>().text = UnsetCrew.ToString("g");

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

        DisplayDamagedEngine();
        DisplayDamagedSteering();
        DisplayFireBurning();
        DisplayBuoyancyCompromised();
        DisplayDamagedTurrets();

        if (DamagedTurrets > 0) {
            // String : "X damaged turrets on Y total"
            DamageControlInstance.transform.Find("TurretsDamaged").GetComponent<Text>().text = DamagedTurrets.ToString("g") + "/" + TotalTurrets.ToString("g")+" turrets HS";
            DamageControlInstance.transform.Find("TurretsDamaged").gameObject.SetActive(true);
        } else {
            DamageControlInstance.transform.Find("TurretsDamaged").gameObject.SetActive(false);
        }

        CheckActiveButtons();
    }

    protected void CloseDmgCtrl() {
        if (DamageControlInstance)
            Destroy (DamageControlInstance);
    } 

    protected void CheckActiveButtons() {
        DamageControlInstance.transform.Find("UnsetCrewCount").GetComponent<Text>().text = UnsetCrew.ToString("g");

        if (EngineRepairCrew > 0) {
            DamageControlInstance.transform.Find("buttonEngineNeg").gameObject.SetActive(true);
        } else {
            DamageControlInstance.transform.Find("buttonEngineNeg").gameObject.SetActive(false);
        }

        if (FireRepairCrew > 0) {
            DamageControlInstance.transform.Find("buttonFireNeg").gameObject.SetActive(true);
        } else {
            DamageControlInstance.transform.Find("buttonFireNeg").gameObject.SetActive(false);
        }

        if (WaterRepairCrew > 0) {
            DamageControlInstance.transform.Find("buttonWaterNeg").gameObject.SetActive(true);
        } else {
            DamageControlInstance.transform.Find("buttonWaterNeg").gameObject.SetActive(false);
        }

        if (TurretsRepairCrew > 0) {
            DamageControlInstance.transform.Find("buttonTurretsNeg").gameObject.SetActive(true);
        } else {
            DamageControlInstance.transform.Find("buttonTurretsNeg").gameObject.SetActive(false);
        }

        if (UnsetCrew > 0) {
            DamageControlInstance.transform.Find("buttonEnginePos").gameObject.SetActive(true);
            DamageControlInstance.transform.Find("buttonFirePos").gameObject.SetActive(true);
            DamageControlInstance.transform.Find("buttonWaterPos").gameObject.SetActive(true);
            DamageControlInstance.transform.Find("buttonTurretsPos").gameObject.SetActive(true);
        } else {
            DamageControlInstance.transform.Find("buttonEnginePos").gameObject.SetActive(false);
            DamageControlInstance.transform.Find("buttonFirePos").gameObject.SetActive(false);
            DamageControlInstance.transform.Find("buttonWaterPos").gameObject.SetActive(false);
            DamageControlInstance.transform.Find("buttonTurretsPos").gameObject.SetActive(false);
        }
    }

    void ButtonEnginePosOnClick(){
        UnsetCrew -= 1;
        EngineRepairCrew += 1;
        DamageControlInstance.transform.Find("EngineCrewCount").GetComponent<Text>().text = EngineRepairCrew.ToString("g");
        CheckActiveButtons();
        ShipController.SetDamageControlEngine(EngineRepairCrew);
    }
    void ButtonEngineNegOnClick(){
        UnsetCrew += 1;
        EngineRepairCrew -= 1;
        DamageControlInstance.transform.Find("EngineCrewCount").GetComponent<Text>().text = EngineRepairCrew.ToString("g");
        CheckActiveButtons();
        ShipController.SetDamageControlEngine(EngineRepairCrew);
    }
    void ButtonFirePosOnClick(){
        UnsetCrew -= 1;
        FireRepairCrew += 1;
        DamageControlInstance.transform.Find("FireCrewCount").GetComponent<Text>().text = FireRepairCrew.ToString("g");
        CheckActiveButtons();
        ShipController.SetDamageControlFire(FireRepairCrew);
    }
    void ButtonFireNegOnClick(){
        UnsetCrew += 1;
        FireRepairCrew -= 1;
        DamageControlInstance.transform.Find("FireCrewCount").GetComponent<Text>().text = FireRepairCrew.ToString("g");
        CheckActiveButtons();
        ShipController.SetDamageControlFire(FireRepairCrew);
    }
    void ButtonWaterPosOnClick(){
        UnsetCrew -= 1;
        WaterRepairCrew += 1;
        DamageControlInstance.transform.Find("WaterCrewCount").GetComponent<Text>().text = WaterRepairCrew.ToString("g");
        CheckActiveButtons();
        ShipController.SetDamageControlWater(WaterRepairCrew);
    }
    void ButtonWaterNegOnClick(){
        UnsetCrew += 1;
        WaterRepairCrew -= 1;
        DamageControlInstance.transform.Find("WaterCrewCount").GetComponent<Text>().text = WaterRepairCrew.ToString("g");
        CheckActiveButtons();
        ShipController.SetDamageControlWater(WaterRepairCrew);
    }
    void ButtonTurretsPosOnClick(){
        UnsetCrew -= 1;
        TurretsRepairCrew += 1;
        DamageControlInstance.transform.Find("TurretsCrewCount").GetComponent<Text>().text = TurretsRepairCrew.ToString("g");
        CheckActiveButtons();
        ShipController.SetDamageControlTurrets(TurretsRepairCrew);
    }
    void ButtonTurretsNegOnClick(){
        UnsetCrew += 1;
        TurretsRepairCrew -= 1;
        DamageControlInstance.transform.Find("TurretsCrewCount").GetComponent<Text>().text = TurretsRepairCrew.ToString("g");
        CheckActiveButtons();
        ShipController.SetDamageControlTurrets(TurretsRepairCrew);
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
    public void SetActive(bool activate) {
        Active = activate;
        if (!Active)
            SetOpenDmgCtrl(false);
    }
    public void SetShipDeath(bool deathStatus) {
        ShipDead = deathStatus;
        if (ShipDead)
            SetOpenDmgCtrl(false);
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
        if (DmgCtrlOpen)
            DisplayDamagedEngine();
    }
    private void DisplayDamagedEngine(){
        // Status : 0 : fixed and running / 1 : damaged / 2 : dead
        if (EngineStatus == 2){
            DamageControlInstance.transform.Find("EngineDamaged").GetComponent<Text>().text = "Engine\n";
            DamageControlInstance.transform.Find("EngineDamaged").GetComponent<Text>().text += "HS";
            DamageControlInstance.transform.Find("EngineDamaged").gameObject.SetActive(true);}
        else if (EngineStatus == 1){
            DamageControlInstance.transform.Find("EngineDamaged").GetComponent<Text>().text = "Engine\n";
            DamageControlInstance.transform.Find("EngineDamaged").GetComponent<Text>().text = "damaged";
            DamageControlInstance.transform.Find("EngineDamaged").gameObject.SetActive(true);}
        else{
            DamageControlInstance.transform.Find("EngineDamaged").gameObject.SetActive(false);}
    }
    public void SetDamagedSteering(bool status){
        SteeringStatus = status;
        if (DmgCtrlOpen)
            DisplayDamagedSteering();
    }
    private void DisplayDamagedSteering(){
        // Status : false : fixed and running / true : damaged
        if (SteeringStatus)
            DamageControlInstance.transform.Find("SteeringDamaged").gameObject.SetActive(true);
        else
            DamageControlInstance.transform.Find("SteeringDamaged").gameObject.SetActive(false);
    }
    public void SetFireBurning(bool status){
        FireStatus = status;
        if (DmgCtrlOpen){
            DisplayFireBurning();
        }
    }
    private void DisplayFireBurning(){
        if (FireStatus)
            DamageControlInstance.transform.Find("FireDamaged").gameObject.SetActive(true);
        else
            DamageControlInstance.transform.Find("FireDamaged").gameObject.SetActive(false);
    }
    public void SetBuoyancyCompromised(bool status){
        WaterStatus = status;
        if (DmgCtrlOpen)
            DisplayBuoyancyCompromised();
    }
    private void DisplayBuoyancyCompromised(){
        if (WaterStatus)
            DamageControlInstance.transform.Find("WaterDamaged").gameObject.SetActive(true);
        else
            DamageControlInstance.transform.Find("WaterDamaged").gameObject.SetActive(false);
    }
    public void SetTotalTurrets(int turrets){
        TotalTurrets = turrets;
    }
    public void SetDamagedTurrets(int turrets){
        DamagedTurrets = turrets;
        if (DmgCtrlOpen)  
            DisplayDamagedTurrets();
    }
    private void DisplayDamagedTurrets(){
        if (DamagedTurrets < TotalTurrets) {
            DamageControlInstance.transform.Find("TurretsDamaged").GetComponent<Text>().text = DamagedTurrets.ToString("g") + "/" + TotalTurrets.ToString("g");
            DamageControlInstance.transform.Find("TurretsDamaged").gameObject.SetActive(true);
        } else {
            DamageControlInstance.transform.Find("TurretsDamaged").gameObject.SetActive(false);
        }
    }

    public float GetRepairRate() { return RepairRate; }
}