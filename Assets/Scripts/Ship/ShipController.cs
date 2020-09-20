using UnityEngine;
using Crest;
using System.Collections;
using System.Collections.Generic;

public class ShipController : UnitMasterController {
    [Header("Ship units elements : ")]

    [Tooltip("Components (game object with collider + Hitbox Component script)")]
    public GameObject[] m_ShipComponents;


    private ShipBuoyancy Buoyancy;
    private ShipMovement Movement;
    private ShipHealth Health;
    private ShipDamageControl DamageControl;
    private ShipAI ShipAI;
    private ShipUI UI;
    private Transform ShipModel;

    
    private float CurrentRotationX  = 0.0f;
    private float CurrentRotationZ = 0.0f;
    private float CurrentpositionY = 0.0f;
    private float TargetRotationX  = 0.0f;
    private float TargetRotationZ = 0.0f;
    private float TargetpositionY = 0.0f;
    private float LeakRatio = 0.0f;
    private bool TakingWater = false;

    private float EngineCount = -1;                  // If there is an engine dm component, how many are there ? (If there are more than one, the engine disabling will work differently)
    private float EngineCountTotal = -1;

    private float RepairRate;
    private float EngineRepairCrew;
    private float FireRepairCrew;
    private float WaterRepairCrew;
    private float TurretsRepairCrew;

    private GameObject EnemyTargetUnit;

    private void Awake() {
        base.SpawnUnit();
        Buoyancy = GetComponent<ShipBuoyancy>();
        Movement = GetComponent<ShipMovement>();
        Health = GetComponent<ShipHealth>();
        ShipAI = GetComponent<ShipAI>();
        float HP = Health.GetStartingHealth();

        UI = GetComponent<ShipUI>();
        UI.SetStartingHealth(HP);
        UI.SetCurrentHealth(HP);

        if (GetComponent<TurretManager>())
            ShipAI.SetTurretManager(Turrets);

        if (GetComponent<ShipDamageControl>()) {
            DamageControl = GetComponent<ShipDamageControl>();
            RepairRate = DamageControl.GetRepairRate();
        }

        if (GetComponent<TurretManager>())
            Turrets.SetRepairRate(RepairRate);

        ShipModel = this.gameObject.transform.GetChild(0);

        for (int i = 0; i < m_ShipComponents.Length; i++) {
            m_ShipComponents[i].GetComponent<HitboxComponent>().SetUnitController(this);
            m_ShipComponents[i].GetComponent<HitboxComponent>().SetDamageControlEngine(EngineRepairCrew);
            m_ShipComponents[i].GetComponent<HitboxComponent>().SetDamageControlFire(FireRepairCrew);
        }
    }
    private void Start() {
        StartCoroutine(SpawnPauseLogic());
    }
    IEnumerator SpawnPauseLogic(){
        yield return new WaitForSeconds(0.3f);
        ResumeStart();
    }

    // private bool ActionPaused = false;
    // private bool ActionPaused2 = false;
    private void FixedUpdate() {
		// Debug.Log("Active :"+ Active);
		// Debug.Log("m_Buoyancy :"+ m_Buoyancy);
        BuoyancyLoop();

        // The following is used to kill an active ship for debug purposes
        // if (Active && !Dead) {
        //     if (Input.GetAxis ("VerticalShip") == 1){
        //         CallDeath();
        //     }
        // }
        // kills all inactive ships for debug purposes
        // if (!Active && !ActionPaused) {
        //     ActionPaused = !ActionPaused;
        //     StartCoroutine(PauseAction());
        // }
        // if (!Active && ActionPaused2 && !Dead) {
        //     CallDeath();
        // }
    }
    
    // IEnumerator PauseAction(){
    //     yield return new WaitForSeconds(3f);
    //     ActionPaused2= true;
    // }

    private void BuoyancyLoop() {
        if (LeakRatio > 0) {
            // If the ship is taking water...
            // Transform the model to show the ship embedding into water
            if (CurrentpositionY != TargetpositionY) {
                BuoyancyCorrectY(LeakRatio);
            }
            // Transform the model to show the ship angling in the direction of the compartments
            if (CurrentRotationX != TargetRotationX || CurrentRotationZ != TargetRotationZ) {
                BuoyancyCorrectXZ(LeakRatio);
            }

            // Also, repair the leak while it is still opened
            LeakRatio -= 0.1f * RepairRate * WaterRepairCrew * Time.deltaTime;
            // Debug.Log("LeakRatio :"+ LeakRatio);

        } else {
            //If the leak is corrected...
            if (TakingWater){
                // Reset targets if the ship was still taking water
                if (CurrentpositionY != TargetpositionY)
                    TargetpositionY = CurrentpositionY;
                if (CurrentRotationX != TargetRotationX)
                    TargetRotationX = CurrentRotationX;
                if (CurrentRotationZ != TargetRotationZ)
                    TargetRotationX = CurrentRotationZ;
                TakingWater = false;
                if (DamageControl)
                    DamageControl.SetBuoyancyCompromised(TakingWater);
            }
            if (!Dead)
                BuoyancyRepair();
        }
    }
    private void BuoyancyRepair() {
        if (CurrentpositionY < 0) {
            TargetpositionY += RepairRate * (WaterRepairCrew + 1) * Time.deltaTime;
            BuoyancyCorrectY((WaterRepairCrew + 1));
        }

        if (TargetRotationX > 0) {
            TargetRotationX -= RepairRate * (WaterRepairCrew + 1) * Time.deltaTime;
        } else if (TargetRotationX < 0) {
            TargetRotationX += RepairRate * (WaterRepairCrew + 1) * Time.deltaTime;
        }
        if (TargetRotationZ > 0) {
            TargetRotationZ -= RepairRate * (WaterRepairCrew + 1) * Time.deltaTime;
        } else if (TargetRotationZ < 0) {
            TargetRotationZ += RepairRate * (WaterRepairCrew + 1) * Time.deltaTime;
        }
        if (TargetRotationX != 0 || TargetRotationZ != 0 ) {
            BuoyancyCorrectXZ((WaterRepairCrew + 1));
        }
    }
    private void BuoyancyCorrectY(float ratio) {
        // Debug.Log("CurrentpositionY :"+ CurrentpositionY);
        // Debug.Log("TargetpositionY :"+ TargetpositionY);
        if (CurrentpositionY > TargetpositionY) {
            // If sinking...
            CurrentpositionY -= 0.1f * ratio * Time.deltaTime;
        } else {
            // If raising...
            CurrentpositionY += 0.1f * ratio * Time.deltaTime;
        }
        ShipModel.transform.localPosition = new Vector3(0.0f, CurrentpositionY, 0.0f);

        // Check death by taking in too much water
        if (CurrentpositionY < -2 && !Dead) {
            // Debug.Log("A ship was destroyed due to rotation Y being : " + CurrentpositionY);
            CallDeath();
        }
    }
    private void BuoyancyCorrectXZ(float ratio) {
        if (CurrentRotationX > TargetRotationX) {
            // Apply force back
            CurrentRotationX -= 0.1f * ratio * Time.deltaTime;
        } else {
            // Apply force front
            CurrentRotationX += 0.1f * ratio * Time.deltaTime;
        }

        if (CurrentRotationZ > TargetRotationZ) {
            // Apply force right
            CurrentRotationZ -= 0.1f * ratio * Time.deltaTime;
        } else {
            // Apply force left
            CurrentRotationZ += 0.1f * ratio * Time.deltaTime;
        }

        ShipModel.transform.localRotation = Quaternion.Euler (new Vector3 (CurrentRotationX, 0.0f, CurrentRotationZ));

        // Check death by taking in too much water
        if (CurrentRotationX < -3  && !Dead|| CurrentRotationX > 3  && !Dead|| CurrentRotationZ < -15  && !Dead|| CurrentRotationZ > 15 && !Dead) {
            // Debug.Log("A ship was destroyed due to rotation X being : " + CurrentRotationX + " - or Z being : " + CurrentRotationZ);
            CallDeath();
        }
    }

    public void SetDamageControlEngine(int setCrew){
        EngineRepairCrew = setCrew;
        for (int i = 0; i < m_ShipComponents.Length; i++) {
            m_ShipComponents[i].GetComponent<HitboxComponent>().SetDamageControlEngine(EngineRepairCrew);
        }
    }
    public void SetDamageControlFire(int setCrew){
        FireRepairCrew = setCrew;
        for (int i = 0; i < m_ShipComponents.Length; i++) {
            m_ShipComponents[i].GetComponent<HitboxComponent>().SetDamageControlFire(FireRepairCrew);
        }
    }
    public void SetDamageControlWater(int setCrew){ WaterRepairCrew = setCrew; }
    public void SetDamageControlTurrets(int setCrew){
        TurretsRepairCrew = setCrew;
        if (GetComponent<TurretManager>() && Turrets != null) {
            Turrets.SetTurretRepairRate(TurretsRepairCrew);
        }
    }
    public void SetDamageControlUnset(int setCrew){ if (Health != null) { Health.SetDamageControlUnset(setCrew); } }
    public void SetSpeedInput(float Speed){ Buoyancy.SetSpeedInput(Speed); }
    public void SetRotationInput(float rotation){
        Buoyancy.SetRotationInput(rotation);
        if (Active && !Dead)
            PlayerManager.SetRotationInput(rotation);
    }
    public void ChangeSpeedStep(int currentSpeedStep){
        if (Active && !Dead)
            PlayerManager.ChangeSpeedStep(currentSpeedStep);
    }
    public void SetCurrentHealth(float health){ if (Active && !Dead) PlayerManager.SetCurrentUnitHealth(health); }
    public void SetAISpeed(int speedStep){ Movement.SetAISpeed(speedStep); }
    public void SetAIturn(float turn){ Movement.SetAIturn(turn); }
    public void SetAITurnInputValue(float turnInputValue){ ShipAI.SetAITurnInputValue(turnInputValue); }
    public void SetCurrentTarget(GameObject targetUnit) {
        EnemyTargetUnit = targetUnit;
        if (Active) {
            PlayerManager.SendCurrentEnemyTarget(targetUnit);
        }
    }


    // ALL OVERRIDES METHODS
    public override void SetNewEnemyList(List <GameObject> enemiesUnitsObjectList) {
        ShipAI.SetNewEnemyList(enemiesUnitsObjectList);
    }
    public override void SetActive(bool activate) {
        base.SetActive(activate);
        Movement.SetActive(Active);
        ShipAI.SetAIActive(!Active);
        // Damage Control can be shown if active
        if (GetComponent<ShipDamageControl>())
            DamageControl.SetActive(Active);
    }
    public override void SetMap(bool map) {
        if (GetComponent<TurretManager>())
            Turrets.SetMap(map);
        if (GetComponent<ShipDamageControl>()) {
            DamageControl.SetMap(map);
        }
        Movement.SetMap(map);
    }
    public override void SetPause(bool pause){
        base.SetPause(pause);
        if (GetComponent<ShipDamageControl>())
            DamageControl.SetPause();
    }
    public override void SetTag(WorldUnitsManager.Teams team){
        base.SetTag(team);
        UI.SetUnitTeam(team);
        ShipAI.SetUnitTeam(team.ToString("g"));

    }
    public override void SetName(string name){
        base.SetName(name);
        UI.SetName(name);
        if (GetComponent<ShipDamageControl>()) {
            DamageControl.SetName(name);
        }
        ShipAI.SetName(name);
    }
    public override float GetRepairRate(){ return RepairRate; }

    // Turrets
    public override void SetTotalTurrets(int turrets){ if (GetComponent<ShipDamageControl>()) { DamageControl.SetTotalTurrets(turrets); } }
    public override void SetMaxTurretRange(float maxRange){ ShipAI.SetMaxTurretRange(maxRange); }
    public override void SetDamagedTurrets(int turrets){ if (GetComponent<ShipDamageControl>()) { DamageControl.SetDamagedTurrets(turrets); } }
    public override void SetSingleTurretStatus(TurretManager.TurretStatusType status, int turretNumber){
        if (PlayerManager != null) { PlayerManager.SetSingleTurretStatus(status, turretNumber); }
    }
    public override void SendPlayerShellToUI(GameObject shellInstance){
        if (PlayerManager != null) { PlayerManager.SendPlayerShellToUI(shellInstance); }
    }
    public override void FeedbackShellHit (bool armorPenetrated) {
        if (GetComponent<ShipDamageControl>())
            DamageControl.UpdateShellsSentCounter(armorPenetrated);
    }

    // UI
    public override void SetUIElement(GameObject uiElement) { UI.SetUIElement(uiElement); }
    public override void SetUIMapElement(GameObject uiElement) { UI.SetUIMapElement(uiElement); }
    public override void KillAllUIInstances() { UI.KillAllUIInstances(); }
    public override float GetStartingHealth() { return(Health.GetStartingHealth()); }
    public override float GetCurrentHealth() { return(Health.GetCurrentHealth()); }
    public override int GetCurrentSpeedStep() { return(Movement.GetCurrentSpeedStep()); }

    // Damage control
    public override void ApplyDamage(float damage) {
        Health.ApplyDamage(damage);
        float currentHealth = Health.GetCurrentHealth();
        UI.SetCurrentHealth(currentHealth);
    }
    public override void ModuleDestroyed(ElementType elementType) {
        // Debug.Log("ElementType :"+ elementType);
        // Status : 0 : fixed and running / 1 : damaged / 2 : dead
        if (elementType == ElementType.engine) {
            EngineCount--;
            if (EngineCount == 0){
                Movement.SetDead(true);
                if (GetComponent<ShipDamageControl>())
                    DamageControl.SetDamagedEngine(2);
            } else {
                Movement.SetDamaged(EngineCount/EngineCountTotal);
                if (GetComponent<ShipDamageControl>())
                    DamageControl.SetDamagedEngine(1);
            }
        } else if (elementType == ElementType.steering) {
            Movement.SetAllowTurnInputChange(false);
            if (GetComponent<ShipDamageControl>())
                DamageControl.SetDamagedSteering(true);
        } else if (elementType == ElementType.ammo) {
            Health.AmmoExplosion();
        } else if (elementType == ElementType.fuel) {
            Health.StartFire();
            if (GetComponent<ShipDamageControl>())
                DamageControl.SetFireBurning(true);
        }
    }
    public override void ModuleRepaired(ElementType elementType) {
        if (elementType == ElementType.engine) {
            EngineCount++;
            if (EngineCount > 0)
                Movement.SetDead(false);
                if (GetComponent<ShipDamageControl>() && EngineCount < EngineCountTotal)
                    DamageControl.SetDamagedEngine(1);
                else
                    DamageControl.SetDamagedEngine(0);
            Movement.SetDamaged(EngineCount/EngineCountTotal);
        } else if (elementType == ElementType.steering) {
            Movement.SetAllowTurnInputChange(true);
            if (GetComponent<ShipDamageControl>())
                DamageControl.SetDamagedSteering(false);
        } else if (elementType == ElementType.fuel) {
            Health.EndFire();
            if (GetComponent<ShipDamageControl>())
                DamageControl.SetFireBurning(false);
        }
    }
    public override void BuoyancyCompromised(ElementType ElementType, float damage) {
        //If a water tight compartment is hit, apply effects here
        // Debug.Log("ElementType :"+ ElementType);
        // Debug.Log("damage :"+ damage);
        TargetpositionY -= damage * 0.001f;
        if (ElementType == ElementType.underwaterFrontLeft) {
            TargetRotationX += damage * 0.01f;
            TargetRotationZ += damage * 0.1f;
        } else if (ElementType == ElementType.underwaterFrontRight) {
            TargetRotationX += damage * 0.01f;
            TargetRotationZ += damage * -0.1f;
        } else if (ElementType == ElementType.underwaterBackLeft) {
            TargetRotationX += damage * -0.01f;
            TargetRotationZ += damage * 0.1f;
        } else if (ElementType == ElementType.underwaterBackRight) {
            TargetRotationX += damage * -0.01f;
            TargetRotationZ += damage * -0.1f;
        }

        LeakRatio += damage * 0.003f;
        TakingWater = true;

        ShipModel.transform.localRotation = Quaternion.Euler (new Vector3 (CurrentRotationX, 0.0f, CurrentRotationZ));

        ShipModel.transform.localPosition = new Vector3(0.0f, CurrentpositionY, 0.0f);

        if (DamageControl)
            DamageControl.SetBuoyancyCompromised(TakingWater);

        // Debug.Log("ShipModel :"+ ShipModel);
        // Debug.Log("ShipModel :"+ ShipModel.transform.localRotation);
        // Debug.Log("ShipModel :"+ ShipModel.transform.localPosition);  
    }
    public override void SetDamageControlEngineCount(){ if (EngineCount < 0) { EngineCount = 1; EngineCountTotal = 1; } else { EngineCount ++; EngineCountTotal++; } }
    public override void SendHitInfoToDamageControl (bool armorPenetrated) {
        if (GetComponent<ShipDamageControl>())
            DamageControl.UpdateShellsReceivedCounter(armorPenetrated);
    }
    public override void CallDeath() {
        base.CallDeath();

        // Sink the ship
        if (TargetRotationX > 1 && TargetRotationZ > 5 || TargetRotationX < -1 && TargetRotationZ < -5) {
            Buoyancy.Sink(1f + LeakRatio, TargetRotationX, TargetRotationZ);
        } else if (TargetRotationX > 1 || TargetRotationX < -1) {
            Buoyancy.Sink(1f + LeakRatio, TargetRotationX, 0);
        } else if (TargetRotationZ > 5 || TargetRotationZ < -5) {
            Buoyancy.Sink(1f + LeakRatio, 0, TargetRotationZ);
        } else {
            Buoyancy.Sink(1f + LeakRatio, 0, 0);
        }

        if (GetComponent<ShipDamageControl>())
            DamageControl.SetShipDeath(true);
        Movement.SetDead(true);
        Buoyancy.SetDead(true);
        UI.SetDead();
    }
    public override void DestroyUnit(){
        // This removes the unit from the scene

        // Debug.Log ("Destroy unit : "+gameObject.name);
        if (GetComponent<TurretManager>())
            Turrets.SetDeath(true);
        if (GetComponent<ShipDamageControl>())
            DamageControl.Destroy();
        UI.KillAllUIInstances();
        if (GameManager) {
            GameManager.UnitDead(this.gameObject, Team, Active);
        }
        base.DestroyUnit();
    }
}