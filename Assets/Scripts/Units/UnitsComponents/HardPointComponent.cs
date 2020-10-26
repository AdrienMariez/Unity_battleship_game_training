using UnityEngine;
public class HardPointComponent : MonoBehaviour {

    [Header("Turrets parameters")]
    [Header("Turrets horizontal rotation limitations")]

        [Tooltip("When true, turret rotates according to left/right traverse limits. When false, turret can rotate freely.")]
            public bool m_LimitTraverse = false;
        [Tooltip("When traverse is limited, how many degrees to the left the turret can turn.")]
            [Range(0.0f, 180.0f)]
            public float m_LeftTraverse = 60.0f;
            private float LocalLeftTraverse;
        [Tooltip("When traverse is limited, how many degrees to the right the turret can turn.")]
            [Range(0.0f, 180.0f)]
            public float m_RightTraverse = 60.0f; 
            private float LocalRightTraverse;
        public FireZonesManager[] m_NoFireZones;

    [Header("Turrets vertical rotation limitations")]
        public ElevationZonesManager[] m_ElevationZones;

    [Header("Other Parameters")]

    [Tooltip("Prefab used")]
        public GameObject m_Prefab;


    // private void Start () { }
    // private void FixedUpdate(){ }

    public static void SetUpHardPointComponent(WorldSingleUnit.UnitHardPoint hardPointElement,HardPointComponent hardPointComponent, Transform hardPointTransform){
        if (hardPointElement.GetHardpointType() == CompiledTypes.HardPoints.RowValues.ShipFunnel) {
            SetUpShipFunnel(hardPointElement,hardPointComponent, hardPointTransform);
        }
        // Debug.Log ("Hardpoint set.  hardpoint id :"+ hardPointElement.GetHardPointID());
    }
    public static void SetUpWeaponHardPoint(WorldSingleUnit.UnitHardPoint hardPointElement,HardPointComponent hardPointComponent, Transform hardPointTransform, TurretManager turretManager){
        // Find & set variant master
            CompiledTypes.Weapons weaponReference = hardPointElement.GetWeaponType();
            CompiledTypes.Weapons masterWeaponReference = weaponReference;
            if (weaponReference.Isavariant && weaponReference.WeaponVariantReferenceList.Count > 0) {
                masterWeaponReference = weaponReference.WeaponVariantReferenceList[0].WeaponVariantRef;
                // Debug.Log (weaponReference.id+" / "+masterWeaponReference.id);
            }
            // else { Debug.Log (weaponReference.id); }

            // Debug.Log (weaponReference.WeaponModelPathList[0].TurretPath+"/"+weaponReference.WeaponModelPathList[0].TurretModel);


        // Set Prefab
            GameObject weaponPrefab = null;
            if (weaponReference.Isavariant && weaponReference.WeaponModelPathList.Count == 0) {
                if (masterWeaponReference.WeaponModelPathList.Count > 0) {
                    weaponPrefab = (Resources.Load("Prefabs/Weapons/"+masterWeaponReference.WeaponModelPathList[0].TurretPath+"/"+masterWeaponReference.WeaponModelPathList[0].TurretModel, typeof(GameObject))) as GameObject;
                }
            } else if (weaponReference.WeaponModelPathList.Count > 0) {
                weaponPrefab = (Resources.Load("Prefabs/Weapons/"+weaponReference.WeaponModelPathList[0].TurretPath+"/"+weaponReference.WeaponModelPathList[0].TurretModel, typeof(GameObject))) as GameObject;
            }
            if (weaponPrefab == null) {
                Debug.Log (" A turret was implemented without a model ! turret id :"+ weaponReference.id);
                weaponPrefab = WorldUIVariables.GetErrorModel();
            }

        // MODEL
            GameObject turretInstance =
                Instantiate (weaponPrefab, hardPointTransform);

            turretManager.AddNewWeapon(turretInstance);

        // Turret Rotation
            turretInstance.GetComponent<TurretRotation>().BeginOperations(hardPointTransform);
    }

    public static void SetUpShipFunnel(WorldSingleUnit.UnitHardPoint hardPointElement,HardPointComponent hardPointComponent, Transform hardPointTransform){
        
    }

}