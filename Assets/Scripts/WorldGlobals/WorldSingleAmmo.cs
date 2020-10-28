using System;
using UnityEngine;

[Serializable]
public class WorldSingleAmmo {
    // Each parameter is built at the first loading of the game from each prefab added in WorldUnitsManager.

    private CompiledTypes.Ammos AmmoReference_DB; public CompiledTypes.Ammos GetAmmoReference_DB(){ return AmmoReference_DB; }
    private GameObject AmmoPrefab; public GameObject GetAmmoPrefab(){ return AmmoPrefab; }

    // private float AmmoWeight; public float GetAmmoWeight(){ return AmmoWeight; }
    // private float AmmoDamageMax; public float GetAmmoDamageMax(){ return AmmoDamageMax; }
    // private float AmmoDamageMin; public float GetAmmoDamageMin(){ return AmmoDamageMin; }
    // private float AmmoArmorPenetration; public float GetAmmoArmorPenetration(){ return AmmoArmorPenetration; }
    // private float AmmoExlosionRadius; public float GetAmmoExlosionRadius(){ return AmmoExlosionRadius; }
    // private float AmmoMaxLifeTime; public float Get(){ return AmmoMaxLifeTime; }
    public void SetAmmo(CompiledTypes.Ammos ammo) {
        AmmoReference_DB = ammo;

        // PREFAB
            string path = "Prefabs/Ammo/"+ammo.AmmoModelPathList[0].AmmoPath+""+ammo.AmmoModelPathList[0].AmmoModel;
        // Debug.Log (path);
            AmmoPrefab = (Resources.Load(path, typeof(GameObject))) as GameObject;
            if (AmmoPrefab == null) {
                Debug.Log (" Ammo was implemented without a model ! Unit id :"+ ammo.id);
                AmmoPrefab = WorldUIVariables.GetErrorModel();
            }

        // AmmoWeight = ammo.Weight;
        // AmmoDamageMax = ammo.DamageMax;
        // AmmoDamageMin = ammo.DamageMin;
        // AmmoArmorPenetration = ammo.ArmorPenetration;
        // AmmoExlosionRadius = ammo.ExplosionRadius;
        // AmmoMaxLifeTime = ammo.MaxLifeTime;

        // Debug.Log (ammo.id +" is set");
    }
}