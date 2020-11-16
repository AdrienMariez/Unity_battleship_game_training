using UnityEngine;
using Crest;
using System.Collections;
using System.Collections.Generic;

public class ShipController : UnitMasterController {
    [Header("Ship units elements : ")]
    private ShipBuoyancy Buoyancy;
    private ShipMovement Movement;
    private ShipDamageControl DamageControl;

    // Water damage parameters
        private Transform ShipModel;                // instead of moving the whole when it's taking water (but not sinking), only the visible model is tilted.
        protected List<Transform> TransformsToSink = new List<Transform>();
        private float CurrentRotationX  = 0.0f;
        private float CurrentRotationZ = 0.0f;
        private float CurrentpositionY = 0.0f;
        private float TargetRotationX  = 0.0f;
        private float TargetRotationZ = 0.0f;
        private float TargetpositionY = 0.0f;
        private float LeakRatio = 0.0f;
        private bool TakingWater = false;

    // Damage control crew management
        private float EngineCount = -1;                  // If there is an engine dm component, how many are there ? (If there are more than one, the engine disabling will work differently)
        private float EngineCountTotal = -1;

        private float EngineRepairCrew;
        private float FireRepairCrew;
        private float WaterRepairCrew;
        private float TurretsRepairCrew;

    protected void Awake() {
        // base.SpawnUnit();
    }

    public override void SetUnitFromWorldUnitsManager(WorldSingleUnit unit, bool aiMove, bool aiShoot, bool aiSpawn) {
        // Set Movement
            if (GetComponent<ShipMovement>()) {
                Movement = GetComponent<ShipMovement>();
                Movement.BeginOperations(this);
            }

        base.SetUnitFromWorldUnitsManager(unit, aiMove, aiShoot, aiSpawn);
        
        // Set Buoyancy
            Buoyancy = this.gameObject.AddComponent<ShipBuoyancy>() as Crest.ShipBuoyancy;
            Buoyancy = GetComponent<ShipBuoyancy>();
            Buoyancy.BeginOperations(unit);


        // Set DamageControl
            if (unit.GetDamageControlExists()) {
                DamageControl = this.gameObject.AddComponent<ShipDamageControl>() as ShipDamageControl;
                DamageControl.SetRepairRate(RepairRate);
                DamageControl.SetRepairCrew(unit.GetDamageControlRepairCrew());
                DamageControl.BeginOperations(this);
                RepairRate = DamageControl.GetRepairRate();
            }

        if (GetComponent<TurretManager>())
            Turrets.SetRepairRate(RepairRate);

        if (this.gameObject.transform.childCount > 0){
            ShipModel = this.gameObject.transform.GetChild(0);
        }
        foreach (Transform child in this.gameObject.transform) {
            TransformsToSink.Add(child);
        }


        foreach (HitboxComponent component in UnitComponents) {
            component.SetDamageControlEngine(EngineRepairCrew);
            component.SetDamageControlFire(FireRepairCrew);
        }
    }

    // private bool ActionPaused = false;
    // private bool ActionPaused2 = false;
    public override void FixedUpdate() {
        base.FixedUpdate();
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
        if (LeakRatio > 0 && !Dead) {
            // If the ship is taking water...
            // Transform the model to show the ship embedding into water
            if (!IsApproximately(CurrentpositionY, TargetpositionY, 0.02f)) {
                BuoyancyCorrectY(LeakRatio);
            }
            // if (CurrentpositionY != TargetpositionY) {
            //     BuoyancyCorrectY(LeakRatio);
            // }
            // Transform the model to show the ship angling in the direction of the compartments
            if (!IsApproximately(CurrentRotationX, TargetRotationX, 0.02f) || !IsApproximately(CurrentRotationZ, TargetRotationZ, 0.02f)) {
                BuoyancyCorrectXZ(LeakRatio);
            }
            // if (CurrentRotationX != TargetRotationX || CurrentRotationZ != TargetRotationZ) {
            //     BuoyancyCorrectXZ(LeakRatio);
            // }

            // Also, repair the leak while it is still opened
            LeakRatio -= 0.1f * RepairRate * (WaterRepairCrew + 1) * Time.deltaTime;
            LeakRatio = (Mathf.Round(LeakRatio * 100)) / 100;
            // Debug.Log("LeakRatio :"+ LeakRatio);

        } else if (!Dead) {
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
            BuoyancyRepair();
        }
    }
    private void BuoyancyRepair() {
        if (TargetpositionY < 0) {
            TargetpositionY += RepairRate * (WaterRepairCrew + 1) * Time.deltaTime;
            TargetpositionY = (Mathf.Round(TargetpositionY * 100)) / 100;
            // if (IsApproximately(TargetpositionY, 0f, 0.02f)) {
            //     TargetpositionY = 0f;
            // }
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
        TargetRotationX = (Mathf.Round(TargetRotationX * 100)) / 100;
        TargetRotationZ = (Mathf.Round(TargetRotationZ * 100)) / 100;
        if (TargetRotationX != 0 || TargetRotationZ != 0 ) {
            BuoyancyCorrectXZ((WaterRepairCrew + 1));
        }
    }
    private void BuoyancyCorrectY(float ratio) {
        // CurrentpositionY represents how much a ship has sunk vertically. Only a negative value can kill it.
        // Debug.Log("TargetpositionY :"+ TargetpositionY);
        if (CurrentpositionY > TargetpositionY) {
            // If sinking...
            CurrentpositionY -= 0.1f * ratio * Time.deltaTime;
            // Debug.Log("CurrentY :"+ CurrentpositionY + "Target :"+ TargetpositionY);
        } else {
            // If raising...
            CurrentpositionY += 0.1f * ratio * Time.deltaTime;
            // Debug.Log("CurrentY :"+ CurrentpositionY + "Target :"+ TargetpositionY);
        }
        // CurrentpositionY = (Mathf.Round(CurrentpositionY * 100)) / 100;
        // ShipModel.transform.localPosition = new Vector3(0.0f, CurrentpositionY, 0.0f);
        foreach (Transform child in TransformsToSink) {
            child.localPosition = new Vector3(0.0f, CurrentpositionY, 0.0f);
        }

        // Check death by taking in too much water
        if (CurrentpositionY < -5 && !Dead) {
            Debug.Log("A ship was destroyed due to rotation Y being : " + CurrentpositionY + " / -5");
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
        // CurrentRotationX = (Mathf.Round(CurrentRotationX * 100)) / 100;

        if (CurrentRotationZ > TargetRotationZ) {
            // Apply force right
            CurrentRotationZ -= 0.1f * ratio * Time.deltaTime;
        } else {
            // Apply force left
            CurrentRotationZ += 0.1f * ratio * Time.deltaTime;
        }
        // CurrentRotationZ = (Mathf.Round(CurrentRotationZ * 100)) / 100;

        foreach (Transform child in TransformsToSink) {
            child.localRotation = Quaternion.Euler (new Vector3 (CurrentRotationX, 0.0f, CurrentRotationZ));
        }

        // ShipModel.transform.localRotation = Quaternion.Euler (new Vector3 (CurrentRotationX, 0.0f, CurrentRotationZ));

        // Check death by taking in too much water
        if (CurrentRotationX < -5  && !Dead|| CurrentRotationX > 5  && !Dead|| CurrentRotationZ < -20  && !Dead|| CurrentRotationZ > 20 && !Dead) {
            Debug.Log("A ship was destroyed due to rotation X being : " + CurrentRotationX + " / 5 - or Z being : " + CurrentRotationZ + " / 20");
            CallDeath();
        }
    }

    public override void BuoyancyCompromised(ElementType ElementType, float damage) {
        //If a water tight compartment is hit, apply effects here
        // Debug.Log("ElementType :"+ ElementType);
        // Debug.Log("damage :"+ damage);
        TargetpositionY -= damage * 0.001f;
        TargetpositionY = (Mathf.Round(TargetpositionY * 100)) / 100;
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
        TargetRotationX = (Mathf.Round(TargetRotationX * 100)) / 100;
        TargetRotationZ = (Mathf.Round(TargetRotationZ * 100)) / 100;

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

    private bool IsApproximately(float a, float b, float tolerance) {
        return Mathf.Abs(a - b) < tolerance;
    }

    public void SetDamageControlEngine(int setCrew){
        EngineRepairCrew = setCrew;
        foreach (HitboxComponent component in UnitComponents) {
            component.SetDamageControlEngine(EngineRepairCrew);
        }
    }
    public void SetDamageControlFire(int setCrew){
        FireRepairCrew = setCrew;
        foreach (HitboxComponent component in UnitComponents) {
            component.SetDamageControlFire(FireRepairCrew);
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
        if (GetComponent<ShipBuoyancy>())
            Buoyancy.SetRotationInput(rotation);
        if (Active && !Dead)
            PlayerManager.SetRotationInput(rotation);
    }
    public void ChangeSpeedStep(int currentSpeedStep){
        if (Active && !Dead)
            PlayerManager.ChangeSpeedStep(currentSpeedStep);
    }
    public void SetAISpeed(int speedStep){ 
        if (GetComponent<ShipMovement>())
            Movement.SetAISpeed(speedStep);
    }
    public void SetAIturn(float turn){
        if (GetComponent<ShipMovement>())
            Movement.SetAIturn(turn);
    }
    public void SetAITurnInputValue(float turnInputValue){ UnitAI.SetAITurnInputValue(turnInputValue); }
    


    // ALL OVERRIDES METHODS
    public override void SetActive(bool activate) {
        base.SetActive(activate);
        if (GetComponent<ShipMovement>())
            Movement.SetActive(Active);
        // Damage Control can be shown if active
        if (GetComponent<ShipDamageControl>())
            DamageControl.SetActive(Active);
    }
    public override void SetMap(bool map) {
        if (GetComponent<TurretManager>())
            Turrets.SetMap(map);
        if (GetComponent<ShipDamageControl>())
            DamageControl.SetMap(map);
        if (GetComponent<ShipMovement>())
            Movement.SetMap(map);
    }
    public override void SetPause(bool pause){
        base.SetPause(pause);
        if (GetComponent<ShipDamageControl>())
            DamageControl.SetPause();
    }

    // Turrets
    public override void SetTotalTurrets(int turrets){ if (GetComponent<ShipDamageControl>()) { DamageControl.SetTotalTurrets(turrets); } }
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
    public override float GetStartingHealth() { return(Health.GetStartingHealth()); }
    public override float GetCurrentHealth() { return(Health.GetCurrentHealth()); }
    public override int GetCurrentSpeedStep() {
        if (GetComponent<ShipMovement>()) {
            return(Movement.GetCurrentSpeedStep());
        } else {
            return 0;
        }
    }

    // Damage control
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
    }
    public override void DestroyUnit(){
        // This removes the unit from the scene
        // Debug.Log ("Destroy unit : "+gameObject.name);
        if (GetComponent<ShipDamageControl>())
            DamageControl.Destroy();
        
        base.DestroyUnit();
    }
}