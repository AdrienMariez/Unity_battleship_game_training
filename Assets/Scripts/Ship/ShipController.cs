using UnityEngine;
using Crest;

public class ShipController : MonoBehaviour {
    [Tooltip("Components (game object with collider + Hitbox Component script)")]
    public GameObject[] m_ShipComponents;

    [HideInInspector] public bool m_Active;
    [HideInInspector] public bool m_Dead;

    private ShipBuoyancy Buoyancy;
    private ShipMovement Movement;
    private ShipHealth Health;
    private TurretManager Turrets;
    private ShipDamageControl DamageControl;
    private Transform ShipModel;
    private float CurrentRotationX  = 0.0f;
    private float CurrentRotationZ = 0.0f;
    private float CurrentpositionY = 0.0f;
    private float TargetRotationX  = 0.0f;
    private float TargetRotationZ = 0.0f;
    private float TargetpositionY = 0.0f;
    private float LeakRatio = 0.0f;

    [HideInInspector] public bool engine = false;
    [HideInInspector] public float engineCount = 0;

    [HideInInspector] public float RepairRate;
    [HideInInspector] public float EngineRepairCrew;
    [HideInInspector] public float FireRepairCrew;
    [HideInInspector] public float WaterRepairCrew;
    [HideInInspector] public float TurretsRepairCrew;

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

    private void Start() {
        m_Dead = false;
        Buoyancy = GetComponent<ShipBuoyancy>();
        Movement = GetComponent<ShipMovement>();
        Health = GetComponent<ShipHealth>();
        if (GetComponent<ShipDamageControl>()) {
            DamageControl = GetComponent<ShipDamageControl>();
            RepairRate = DamageControl.RepairRate;
        }
        if (GetComponent<TurretManager>()) {
            Turrets = GetComponent<TurretManager>();
            Turrets.RepairRate = RepairRate;
            Turrets.TurretsRepairCrew = TurretsRepairCrew;
        }
        ShipModel = this.gameObject.transform.GetChild(0);
        
    }

    private void FixedUpdate() {
		// Debug.Log("m_Active :"+ m_Active);
		// Debug.Log("m_Buoyancy :"+ m_Buoyancy);
        if (!m_Dead) {
            Movement.m_Active = m_Active;
            Turrets.m_Active = m_Active;
        } else {
            // Prevent any action from the ship once it is dead
            Buoyancy.m_Dead = true;
            Movement.m_Dead = true;
            Turrets.m_Dead = true;
        }
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
            BuoyancyRepair();
        }
    }

    public void ApplyDamage(float damage) {
        Health.ApplyDamage(damage);
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
        Debug.Log("CurrentpositionY :"+ CurrentpositionY);
        Debug.Log("TargetpositionY :"+ TargetpositionY);
        if (CurrentpositionY > TargetpositionY) {
            // If sinking...
            CurrentpositionY -= 0.1f * ratio * Time.deltaTime;
        } else {
            // If raising...
            CurrentpositionY += 0.1f * ratio * Time.deltaTime;
        }
        ShipModel.transform.localPosition = new Vector3(0.0f, CurrentpositionY, 0.0f);

        // Check death by taking in too much water
        if (CurrentpositionY < 2)
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
        if (CurrentRotationX < -2 || CurrentRotationX > 2 || CurrentRotationZ < -2 || CurrentRotationZ > 2)
            CallDeath();
    }

    public void ModuleDestroyed(ElementType ElementType) {
        // Debug.Log("ElementType :"+ ElementType);
    }

    public void CallDeath() {
        m_Dead = true;
        tag = "Untagged";

        // Sink the ship
        Buoyancy.Sink(1f + LeakRatio);
    }
}