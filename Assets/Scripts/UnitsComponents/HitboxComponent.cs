using UnityEngine;
public class HitboxComponent : MonoBehaviour {

    [Tooltip("Initial HP of the element")]
    public float m_ElementHealth = 100.0f;
    [Tooltip("Armor of the element (equivalent in rolled steel mm)")]
    public float m_ElementArmor = 100.0f;
    [Tooltip("Type of element")]
    public ShipController.ElementType m_ElementType = ShipController.ElementType.hull;

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
    private float EngineRepairRate;
    private float FireRepairRate;
    private bool ImmortalComponent = false;
    private bool BuoyancyComponent = false;
    private bool Dead = false;


    private ShipController ShipController;


    private void Start () {
        if (m_ElementType == ShipController.ElementType.hull) {
            ImmortalComponent = true;
        }
        if (m_ElementType == ShipController.ElementType.underwaterFrontLeft || m_ElementType == ShipController.ElementType.underwaterFrontRight || m_ElementType == ShipController.ElementType.underwaterBackLeft || m_ElementType == ShipController.ElementType.underwaterBackRight) {
            BuoyancyComponent = true;
            ImmortalComponent = true;
        }
        CurrentHealth = m_ElementHealth;
        if (Emitter) {
            SmokeInstance = Instantiate(m_Smoke, this.transform);
            SmokeInstance.GetComponent<ParticleSystem>().Stop();
        }
    }

    public void InitializeModules () {
        RepairRate = ShipController.GetRepairRate();

        // Depending of the ElementType, send it to the ShipController
        if (m_ElementType == ShipController.ElementType.engine){
            ShipController.SetDamageControlEngineCount();
        }
    }

    private void FixedUpdate(){
        if (Dead) {
            RepairModule();
        }
    }

    public void TakeDamage (float amount) {
        // If a underwater armor is damaged, apply water damage
        if (ImmortalComponent) {
            ShipController.ApplyDamage(amount);
            ShipController.BuoyancyCompromised(m_ElementType, amount);
        } else {
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
                ShipController.ApplyDamage(amount);
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

    private void ModuleDestroyed () {
        Dead = true;
        ShipController.ModuleDestroyed(m_ElementType);
        if (Emitter) {
            SmokeInstance.GetComponent<ParticleSystem>().Play();
        }
    }

    private void RepairModule () {
        // If the module is destroyed, repair it to full health while keeping it disabled as long as it's not fully repaired
        float ModuleRepairRate;
        // If the module type is either engine, steering or a turret, accelerate the repair time of the module with damage control teams
        if (m_ElementType == ShipController.ElementType.engine || m_ElementType == ShipController.ElementType.steering) {
            ModuleRepairRate = RepairRate * (EngineRepairRate + 1) * Time.deltaTime;
        } else if (m_ElementType == ShipController.ElementType.fuel) {
            ModuleRepairRate = RepairRate * (FireRepairRate + 1) * Time.deltaTime;
        }  else if (m_ElementType == ShipController.ElementType.ammo) {
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
        ShipController.ModuleRepaired(m_ElementType);
        if (Emitter) {
            SmokeInstance.GetComponent<ParticleSystem>().Stop();
        }
    }

    public void SetShipController(ShipController shipController){
        ShipController = shipController;
        InitializeModules();
    }
    public void SetDamageControlEngine(float crew){ EngineRepairRate = crew; }
    public void SetDamageControlFire(float crew){ FireRepairRate = crew; }
    public ShipController.ElementType GetElementType(){ return m_ElementType; }
}