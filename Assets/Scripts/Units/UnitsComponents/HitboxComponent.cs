using UnityEngine;
public class HitboxComponent : MonoBehaviour {

    [Tooltip("Initial HP of the element")]
    public float m_ElementHealth = 100.0f;
    [Tooltip("Armor of the element (equivalent in rolled steel mm) Buoyancy elements do not use this (use conventionnal armors or hull parts to make belt armors)")]
    public float m_ElementArmor = 100.0f; public float GetElementArmor(){ return m_ElementArmor; }
    [Tooltip("Type of element")]
    public UnitMasterController.ElementType m_ElementType = UnitMasterController.ElementType.hull; public UnitMasterController.ElementType GetElementType(){ return m_ElementType; }

    [Header("FX")]
    [Tooltip("When the element is destroyed, will it emit smoke ?")]
    public bool Emitter = false;
    [Tooltip("If the element emits smoke when damaged, select one")]
    public GameObject m_Smoke;
        private GameObject SmokeInstance;

    // [Header("Debug")]
    //     public bool debug = false;
    
    private float CurrentHealth;
    private float RepairRate;
    private float EngineRepairRate; public void SetDamageControlEngine(float crew){ EngineRepairRate = crew; }
    private float FireRepairRate; public void SetDamageControlFire(float crew){ FireRepairRate = crew; }
    private bool ImmortalComponent = false;
    private bool ArmorComponent = false;
    private bool BuoyancyComponent = false; public bool GetBuoyancyComponent(){ return BuoyancyComponent; }
    private bool Dead = false;


    private UnitMasterController UnitMasterController;

    private void Start () {
        if (m_ElementType == UnitMasterController.ElementType.hull) {
            ImmortalComponent = true;
        }
        if (m_ElementType == UnitMasterController.ElementType.underwaterFrontLeft || m_ElementType == UnitMasterController.ElementType.underwaterFrontRight || m_ElementType == UnitMasterController.ElementType.underwaterBackLeft || m_ElementType == UnitMasterController.ElementType.underwaterBackRight) {
            BuoyancyComponent = true;
            ImmortalComponent = true;
        }
        if (m_ElementType == UnitMasterController.ElementType.armorPlate) {
            ArmorComponent = true;
        }
        CurrentHealth = m_ElementHealth;
        if (Emitter) {
            SmokeInstance = Instantiate(m_Smoke, this.transform);
            SmokeInstance.GetComponent<ParticleSystem>().Stop();
        }
    }

    public void InitializeModules () {
        RepairRate = UnitMasterController.GetRepairRate();

        // Depending of the ElementType, send it to the UnitController
        if (m_ElementType == UnitMasterController.ElementType.engine){
            // Will be used only for mobile units...
            UnitMasterController.SetDamageControlEngineCount();
        }
    }

    private void FixedUpdate(){
        if (Dead) {
            RepairModule();
        }
    }

    public void TakeDamage (float amount) {
        // If a underwater armor is damaged, apply water damage
        if (BuoyancyComponent) {
            UnitMasterController.ApplyDamage(amount);
            UnitMasterController.BuoyancyCompromised(m_ElementType, amount);
        } else if (!ArmorComponent) {
            // Debug.Log("amount = "+ amount);
            // Reduce current health by the amount of damage done.
            if (!ImmortalComponent)
                CurrentHealth -= amount;

            if (CurrentHealth < 0)
                CurrentHealth = 0;

            if (CurrentHealth == 0 && !Dead) {
                ModuleDestroyed();
            }
            // This directly transfers damage to modules to the unit itself
            else if (CurrentHealth > 0 && !Dead) {
                UnitMasterController.ApplyDamage(amount);
            }
            else if (Dead) {
                RepairModule();
            }
        }

        // if (debug){
            // Debug.Log("amount = "+ amount);
            // Debug.Log("m_ElementType = "+ m_ElementType);
            // Debug.Log("CurrentHealth = "+ CurrentHealth);
        // }
    }

    public void SendHitInfoToDamageControl (bool armorPenetrated) {
        UnitMasterController.SendHitInfoToDamageControl(armorPenetrated);
    }

    public void TakeDamageDecal() {

    }

    private void ModuleDestroyed () {
        Dead = true;
        UnitMasterController.ModuleDestroyed(m_ElementType); 
        
        if (Emitter) {
            SmokeInstance.GetComponent<ParticleSystem>().Play();
        }
    }

    private void RepairModule () {
        // If the module is destroyed, repair it to full health while keeping it disabled as long as it's not fully repaired
        float ModuleRepairRate;
        // If the module type is either engine, steering or a turret, accelerate the repair time of the module with damage control teams
        if (m_ElementType == UnitMasterController.ElementType.engine || m_ElementType == UnitMasterController.ElementType.steering) {
            ModuleRepairRate = RepairRate * (EngineRepairRate + 1) * Time.deltaTime;
        } else if (m_ElementType == UnitMasterController.ElementType.fuel) {
            ModuleRepairRate = RepairRate * (FireRepairRate + 1) * Time.deltaTime;
        }  else if (m_ElementType == UnitMasterController.ElementType.ammo) {
            // Make all ammo repair time a fixed time, otherwise ships with great damage control will end up with much more explosions than ships with none...
            ModuleRepairRate = 1 * Time.deltaTime;
        } else {
            ModuleRepairRate = RepairRate * Time.deltaTime;
        }
        CurrentHealth += ModuleRepairRate;

        // Stop repair and reactivate the module when full health is back
        if (CurrentHealth >= m_ElementHealth) {
            ModuleRepaired ();
        }
    }

    private void ModuleRepaired () {
        CurrentHealth = m_ElementHealth;
        Dead = false;
        UnitMasterController.ModuleRepaired(m_ElementType); 
        if (Emitter) {
            SmokeInstance.GetComponent<ParticleSystem>().Stop();
        }
    }

    public void InteractionWithGameBoundaries (bool action) {
        // True : enters game boundaries
        // False : exits game boundaries
        UnitMasterController.SetInGameBoundaries(action);
    }

    public void SetUnitController(UnitMasterController unitMasterComponent){
        UnitMasterController = unitMasterComponent;
        InitializeModules();
    }
}