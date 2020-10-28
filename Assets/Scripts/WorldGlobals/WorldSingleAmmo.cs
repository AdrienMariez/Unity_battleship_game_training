using System;
using UnityEngine;

[Serializable]
public class WorldSingleAmmo {
    // Each parameter is built at the first loading of the game from each prefab added in WorldUnitsManager.

    private CompiledTypes.Ammos AmmoReference_DB; public CompiledTypes.Ammos GetAmmoReference_DB(){ return AmmoReference_DB; }
    private GameObject AmmoPrefab; public GameObject GetAmmoPrefab(){ return AmmoPrefab; }
    private GameObject ExplosionFX; public GameObject GetExplosionFX(){ return ExplosionFX; }
    private GameObject WaterHitFX; public GameObject GetWaterHitFX(){ return WaterHitFX; }
    private GameObject DamageFX; public GameObject GetDamageFX(){ return DamageFX; }

    // private float AmmoWeight; public float GetAmmoWeight(){ return AmmoWeight; }
    // private float AmmoDamageMax; public float GetAmmoDamageMax(){ return AmmoDamageMax; }
    // private float AmmoDamageMin; public float GetAmmoDamageMin(){ return AmmoDamageMin; }
    // private float AmmoArmorPenetration; public float GetAmmoArmorPenetration(){ return AmmoArmorPenetration; }
    // private float AmmoExlosionRadius; public float GetAmmoExlosionRadius(){ return AmmoExlosionRadius; }
    // private float AmmoMaxLifeTime; public float Get(){ return AmmoMaxLifeTime; }
    public void SetAmmo(CompiledTypes.Ammos ammo) {
        AmmoReference_DB = ammo;

        // PREFAB
            AmmoPrefab = (Resources.Load("Prefabs/Ammo/"+ammo.AmmoModelPathList[0].AmmoPath+""+ammo.AmmoModelPathList[0].AmmoModel, typeof(GameObject))) as GameObject;
            if (AmmoPrefab == null) {
                Debug.Log (" Ammo was implemented without a model ! Unit id :"+ ammo.id);
                AmmoPrefab = WorldUIVariables.GetErrorModel();
            }

        // FX
            ExplosionFX = (Resources.Load("FX/"+ammo.AmmoFXList[0].FXExplosion.FXPath+""+ammo.AmmoFXList[0].FXExplosion.FXPrefab, typeof(GameObject))) as GameObject;
            if (ExplosionFX == null) {
                Debug.Log (" No ExplosionFX found for"+ ammo.id);
                ExplosionFX = WorldUIVariables.GetErrorModel();
            }
            WaterHitFX = (Resources.Load("FX/"+ammo.AmmoFXList[0].FXWaterHit.FXPath+""+ammo.AmmoFXList[0].FXWaterHit.FXPrefab, typeof(GameObject))) as GameObject;
            if (WaterHitFX == null) {
                Debug.Log (" No WaterHitFX found for"+ ammo.id);
                WaterHitFX = WorldUIVariables.GetErrorModel();
            }
            DamageFX = WorldUIVariables.GetShellDecal();

        // remainder of data is set directly in ShellStat
    }
}