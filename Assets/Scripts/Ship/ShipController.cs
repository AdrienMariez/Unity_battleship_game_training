using UnityEngine;
using Crest;

public class ShipController : MonoBehaviour {
    [Tooltip("Components (game object with collider + Hitbox Component script)")]
    public GameObject[] m_ShipComponents;
    private bool Active = false;
    private bool Dead;

    private GameManager GameManager;
    private ShipBuoyancy Buoyancy;
    private ShipMovement Movement;
    private ShipHealth Health;
    private TurretManager Turrets;
    private ShipDamageControl DamageControl;
    private ShipUI UI;
    private Transform ShipModel;

    
    private float CurrentRotationX  = 0.0f;
    private float CurrentRotationZ = 0.0f;
    private float CurrentpositionY = 0.0f;
    private float TargetRotationX  = 0.0f;
    private float TargetRotationZ = 0.0f;
    private float TargetpositionY = 0.0f;
    private float LeakRatio = 0.0f;

    private bool engine = false;                    // Is there an engine dm component ? (aka can the engine be disabled ?)
    private float engineCount = 0;                  // If there is an engine dm component, how many are there ? (If there are more than one, the engine disabling will work differently)

    private float RepairRate;
    private float EngineRepairCrew;
    private float FireRepairCrew;
    private float WaterRepairCrew;
    private float TurretsRepairCrew;
    private string unitTag;

    [HideInInspector] public float CurrentHealth;

    public enum ElementType {
        hull,
        engine,
        steering,
        ammo,
        fuel,
        turret,
        underwaterFrontLeft,
        underwaterFrontRight,
        underwaterBackLeft,
        underwaterBackRight
    }

    private void Awake() {
        Dead = false;
        Buoyancy = GetComponent<ShipBuoyancy>();
        Movement = GetComponent<ShipMovement>();
        Health = GetComponent<ShipHealth>();
        float HP = Health.GetStartingHealth();

        UI = GetComponent<ShipUI>();
        UI.Init();
        UI.SetStartingHealth(HP);
        UI.SetCurrentHealth(HP);


        if (GetComponent<ShipDamageControl>()) {
            DamageControl = GetComponent<ShipDamageControl>();
            RepairRate = DamageControl.GetRepairRate();
        }
        if (GetComponent<TurretManager>()) {
            Turrets = GetComponent<TurretManager>();
            Turrets.SetRepairRate(RepairRate);
            Turrets.SetTurretRepairRate(TurretsRepairCrew);
        }
        ShipModel = this.gameObject.transform.GetChild(0);
    }

    private void FixedUpdate() {
		// Debug.Log("Active :"+ Active);
		// Debug.Log("m_Buoyancy :"+ m_Buoyancy);
        BuoyancyLoop();

        // The following is used to kill an active ship for debug purposes
        if (Active && !Dead) {
            if (Input.GetAxis ("VerticalShip") == 1){
                CallDeath();
            }
        }
    }

    public void ApplyDamage(float damage) {
        Health.ApplyDamage(damage);
        CurrentHealth = Health.GetCurrentHealth();
        UI.SetCurrentHealth(CurrentHealth);
        // if (CurrentHealth <= 0)
        //     CallDeath();
        // Debug.Log("CurrentHealth = "+ CurrentHealth);
    }

    public void BuoyancyCompromised(ElementType ElementType, float damage) {
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

        ShipModel.transform.localRotation = Quaternion.Euler (new Vector3 (CurrentRotationX, 0.0f, CurrentRotationZ));

        ShipModel.transform.localPosition = new Vector3(0.0f, CurrentpositionY, 0.0f);

        // Debug.Log("ShipModel :"+ ShipModel);
        // Debug.Log("ShipModel :"+ ShipModel.transform.localRotation);
        // Debug.Log("ShipModel :"+ ShipModel.transform.localPosition);  
    }

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
            // Reset targets if the ship was still taking water
            if (CurrentpositionY != TargetpositionY)
                TargetpositionY = CurrentpositionY;
            if (CurrentRotationX != TargetRotationX)
                TargetRotationX = CurrentRotationX;
            if (CurrentRotationZ != TargetRotationZ)
                TargetRotationX = CurrentRotationZ;
            if (!Dead)
                BuoyancyRepair();
        }
    }

    private void BuoyancyRepair() {
        if (CurrentpositionY < 0) {
            TargetpositionY += RepairRate * WaterRepairCrew * Time.deltaTime;
            BuoyancyCorrectY(WaterRepairCrew);
        }

        if (TargetRotationX > 0) {
            TargetRotationX -= RepairRate * WaterRepairCrew * Time.deltaTime;
        } else if (TargetRotationX < 0) {
            TargetRotationX += RepairRate * WaterRepairCrew * Time.deltaTime;
        }
        if (TargetRotationZ > 0) {
            TargetRotationZ -= RepairRate * WaterRepairCrew * Time.deltaTime;
        } else if (TargetRotationZ < 0) {
            TargetRotationZ += RepairRate * WaterRepairCrew * Time.deltaTime;
        }
        if (TargetRotationX != 0 || TargetRotationZ != 0 )
        {
            BuoyancyCorrectXZ(WaterRepairCrew);
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
        if (CurrentpositionY < -2 && !Dead)
            CallDeath();
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
        if (CurrentRotationX < -3  && !Dead|| CurrentRotationX > 3  && !Dead|| CurrentRotationZ < -15  && !Dead|| CurrentRotationZ > 15 && !Dead)
            CallDeath();
    }

    public void ModuleDestroyed(ElementType ElementType) {
        // Debug.Log("ElementType :"+ ElementType);
    }

    public void CallDeath() {
        if (GameManager)
            GameManager.SetUnitDeath(-1, unitTag);

        // Debug.Log("DEATH"+Dead);
        Dead = true;
        tag = "Untagged";

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
        if (GetComponent<TurretManager>())
            Turrets.SetDeath(true);
        Movement.SetDead(true);
        Buoyancy.SetDead(true);
        UI.SetActive(false);
    }

    public void SetMap(bool map) {
        if (GetComponent<TurretManager>())
            Turrets.SetMap(map);
        if (GetComponent<ShipUI>())
            UI.SetMap(map);
    }
    public void SetActive(bool activate) {
        Active = activate;
        if (GetComponent<TurretManager>())
                Turrets.SetActive(Active);
        Movement.SetActive(Active);
        // UI is activated if the unit is NOT active.
        UI.SetActive(!Active);
    }
    
    public void SetTag(string team){
        unitTag = team;
        gameObject.tag = unitTag;
    }
    public void SetGameManager(GameManager gameManager){ GameManager = gameManager; }
    public void SetDamageControlEngineComponent(bool setEngine){ engine = setEngine; }
    public void SetDamageControlEngineCount(float setEngineCount){ engineCount += setEngineCount; }
    public void SetDamageControlEngine(float setCrew){ EngineRepairCrew = setCrew; }
    public void SetDamageControlFire(float setCrew){ FireRepairCrew = setCrew; }
    public void SetDamageControlWater(float setCrew){ WaterRepairCrew = setCrew; }
    public void SetDamageControlTurrets(float setCrew){ TurretsRepairCrew = setCrew; }
    public bool GetDeath(){ return Dead; }
    public float GetRepairRate(){ return RepairRate; }
    public float GetEngineRepairCrew(){ return EngineRepairCrew; }
    public float GetTurretsRepairCrew(){ return TurretsRepairCrew; }
    public void DestroyUnit(){ Destroy (gameObject); }
}