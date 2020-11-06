
using UnityEngine;
using CastleDBImporter;
using System.Collections.Generic;
using System;

namespace CompiledTypes
{
    public class CastleDB
    {
        static CastleDBParser parsedDB;
        public ScenariosType Scenarios;
public Global_UnitsType Global_Units;
public Units_sub_categoriesType Units_sub_categories;
public Units_categoriesType Units_categories;
public TeamsType Teams;
public CountriesType Countries;
public HardPointsType HardPoints;
public WeaponsType Weapons;
public Weapons_rolesType Weapons_roles;
public AmmosType Ammos;
public FX_StorageType FX_Storage;
public AudioFX_StorageType AudioFX_Storage;

        public CastleDB(TextAsset castleDBAsset)
        {
            parsedDB = new CastleDBParser(castleDBAsset);
            Scenarios = new ScenariosType();Global_Units = new Global_UnitsType();Units_sub_categories = new Units_sub_categoriesType();Units_categories = new Units_categoriesType();Teams = new TeamsType();Countries = new CountriesType();HardPoints = new HardPointsType();Weapons = new WeaponsType();Weapons_roles = new Weapons_rolesType();Ammos = new AmmosType();FX_Storage = new FX_StorageType();AudioFX_Storage = new AudioFX_StorageType();
        }
        public class ScenariosType 
 {public Scenarios A { get { return Get(CompiledTypes.Scenarios.RowValues.A); } } 
public Scenarios B { get { return Get(CompiledTypes.Scenarios.RowValues.B); } } 
public Scenarios C { get { return Get(CompiledTypes.Scenarios.RowValues.C); } } 
public Scenarios D { get { return Get(CompiledTypes.Scenarios.RowValues.D); } } 
public Scenarios E { get { return Get(CompiledTypes.Scenarios.RowValues.E); } } 
private Scenarios Get(CompiledTypes.Scenarios.RowValues line) { return new Scenarios(parsedDB.Root, line); }

                public Scenarios[] GetAll() 
                {
                    var values = (CompiledTypes.Scenarios.RowValues[])Enum.GetValues(typeof(CompiledTypes.Scenarios.RowValues));
                    Scenarios[] returnList = new Scenarios[values.Length];
                    for (int i = 0; i < values.Length; i++)
                    {
                        returnList[i] = Get(values[i]);
                    }
                    return returnList;
                }
 } //END OF Scenarios 
public class Global_UnitsType 
 {public Global_Units fuso { get { return Get(CompiledTypes.Global_Units.RowValues.fuso); } } 
public Global_Units takao { get { return Get(CompiledTypes.Global_Units.RowValues.takao); } } 
public Global_Units fusoUSA { get { return Get(CompiledTypes.Global_Units.RowValues.fusoUSA); } } 
public Global_Units takaoUSA { get { return Get(CompiledTypes.Global_Units.RowValues.takaoUSA); } } 
public Global_Units TrainingCarrier { get { return Get(CompiledTypes.Global_Units.RowValues.TrainingCarrier); } } 
public Global_Units TrainingCarrierUSA { get { return Get(CompiledTypes.Global_Units.RowValues.TrainingCarrierUSA); } } 
public Global_Units ShipyardJapan { get { return Get(CompiledTypes.Global_Units.RowValues.ShipyardJapan); } } 
public Global_Units ShipyardUSA { get { return Get(CompiledTypes.Global_Units.RowValues.ShipyardUSA); } } 
private Global_Units Get(CompiledTypes.Global_Units.RowValues line) { return new Global_Units(parsedDB.Root, line); }

                public Global_Units[] GetAll() 
                {
                    var values = (CompiledTypes.Global_Units.RowValues[])Enum.GetValues(typeof(CompiledTypes.Global_Units.RowValues));
                    Global_Units[] returnList = new Global_Units[values.Length];
                    for (int i = 0; i < values.Length; i++)
                    {
                        returnList[i] = Get(values[i]);
                    }
                    return returnList;
                }
 } //END OF Global_Units 
public class Units_sub_categoriesType 
 {public Units_sub_categories Empty { get { return Get(CompiledTypes.Units_sub_categories.RowValues.Empty); } } 
public Units_sub_categories Battleship { get { return Get(CompiledTypes.Units_sub_categories.RowValues.Battleship); } } 
public Units_sub_categories Carrier { get { return Get(CompiledTypes.Units_sub_categories.RowValues.Carrier); } } 
public Units_sub_categories Heavy_Cruiser { get { return Get(CompiledTypes.Units_sub_categories.RowValues.Heavy_Cruiser); } } 
public Units_sub_categories Light_Cruiser { get { return Get(CompiledTypes.Units_sub_categories.RowValues.Light_Cruiser); } } 
public Units_sub_categories Destroyer { get { return Get(CompiledTypes.Units_sub_categories.RowValues.Destroyer); } } 
public Units_sub_categories Attack_Submarine { get { return Get(CompiledTypes.Units_sub_categories.RowValues.Attack_Submarine); } } 
public Units_sub_categories Fighter { get { return Get(CompiledTypes.Units_sub_categories.RowValues.Fighter); } } 
public Units_sub_categories Light_Bomber { get { return Get(CompiledTypes.Units_sub_categories.RowValues.Light_Bomber); } } 
public Units_sub_categories Heavy_Bomber { get { return Get(CompiledTypes.Units_sub_categories.RowValues.Heavy_Bomber); } } 
public Units_sub_categories Tank { get { return Get(CompiledTypes.Units_sub_categories.RowValues.Tank); } } 
public Units_sub_categories Airfield { get { return Get(CompiledTypes.Units_sub_categories.RowValues.Airfield); } } 
public Units_sub_categories Shipyard { get { return Get(CompiledTypes.Units_sub_categories.RowValues.Shipyard); } } 
public Units_sub_categories LandBase { get { return Get(CompiledTypes.Units_sub_categories.RowValues.LandBase); } } 
private Units_sub_categories Get(CompiledTypes.Units_sub_categories.RowValues line) { return new Units_sub_categories(parsedDB.Root, line); }

                public Units_sub_categories[] GetAll() 
                {
                    var values = (CompiledTypes.Units_sub_categories.RowValues[])Enum.GetValues(typeof(CompiledTypes.Units_sub_categories.RowValues));
                    Units_sub_categories[] returnList = new Units_sub_categories[values.Length];
                    for (int i = 0; i < values.Length; i++)
                    {
                        returnList[i] = Get(values[i]);
                    }
                    return returnList;
                }
 } //END OF Units_sub_categories 
public class Units_categoriesType 
 {public Units_categories ship { get { return Get(CompiledTypes.Units_categories.RowValues.ship); } } 
public Units_categories submarine { get { return Get(CompiledTypes.Units_categories.RowValues.submarine); } } 
public Units_categories aircraft { get { return Get(CompiledTypes.Units_categories.RowValues.aircraft); } } 
public Units_categories ground { get { return Get(CompiledTypes.Units_categories.RowValues.ground); } } 
public Units_categories building { get { return Get(CompiledTypes.Units_categories.RowValues.building); } } 
private Units_categories Get(CompiledTypes.Units_categories.RowValues line) { return new Units_categories(parsedDB.Root, line); }

                public Units_categories[] GetAll() 
                {
                    var values = (CompiledTypes.Units_categories.RowValues[])Enum.GetValues(typeof(CompiledTypes.Units_categories.RowValues));
                    Units_categories[] returnList = new Units_categories[values.Length];
                    for (int i = 0; i < values.Length; i++)
                    {
                        returnList[i] = Get(values[i]);
                    }
                    return returnList;
                }
 } //END OF Units_categories 
public class TeamsType 
 {public Teams Allies { get { return Get(CompiledTypes.Teams.RowValues.Allies); } } 
public Teams Axis { get { return Get(CompiledTypes.Teams.RowValues.Axis); } } 
public Teams Neutral { get { return Get(CompiledTypes.Teams.RowValues.Neutral); } } 
private Teams Get(CompiledTypes.Teams.RowValues line) { return new Teams(parsedDB.Root, line); }

                public Teams[] GetAll() 
                {
                    var values = (CompiledTypes.Teams.RowValues[])Enum.GetValues(typeof(CompiledTypes.Teams.RowValues));
                    Teams[] returnList = new Teams[values.Length];
                    for (int i = 0; i < values.Length; i++)
                    {
                        returnList[i] = Get(values[i]);
                    }
                    return returnList;
                }
 } //END OF Teams 
public class CountriesType 
 {public Countries australia { get { return Get(CompiledTypes.Countries.RowValues.australia); } } 
public Countries china { get { return Get(CompiledTypes.Countries.RowValues.china); } } 
public Countries france { get { return Get(CompiledTypes.Countries.RowValues.france); } } 
public Countries greatbritain { get { return Get(CompiledTypes.Countries.RowValues.greatbritain); } } 
public Countries germany { get { return Get(CompiledTypes.Countries.RowValues.germany); } } 
public Countries italy { get { return Get(CompiledTypes.Countries.RowValues.italy); } } 
public Countries japan { get { return Get(CompiledTypes.Countries.RowValues.japan); } } 
public Countries newzealand { get { return Get(CompiledTypes.Countries.RowValues.newzealand); } } 
public Countries usa { get { return Get(CompiledTypes.Countries.RowValues.usa); } } 
public Countries ussr { get { return Get(CompiledTypes.Countries.RowValues.ussr); } } 
private Countries Get(CompiledTypes.Countries.RowValues line) { return new Countries(parsedDB.Root, line); }

                public Countries[] GetAll() 
                {
                    var values = (CompiledTypes.Countries.RowValues[])Enum.GetValues(typeof(CompiledTypes.Countries.RowValues));
                    Countries[] returnList = new Countries[values.Length];
                    for (int i = 0; i < values.Length; i++)
                    {
                        returnList[i] = Get(values[i]);
                    }
                    return returnList;
                }
 } //END OF Countries 
public class HardPointsType 
 {public HardPoints Weapon { get { return Get(CompiledTypes.HardPoints.RowValues.Weapon); } } 
public HardPoints ShipFunnel { get { return Get(CompiledTypes.HardPoints.RowValues.ShipFunnel); } } 
public HardPoints ShipPropeller { get { return Get(CompiledTypes.HardPoints.RowValues.ShipPropeller); } } 
public HardPoints PlanePropeller { get { return Get(CompiledTypes.HardPoints.RowValues.PlanePropeller); } } 
private HardPoints Get(CompiledTypes.HardPoints.RowValues line) { return new HardPoints(parsedDB.Root, line); }

                public HardPoints[] GetAll() 
                {
                    var values = (CompiledTypes.HardPoints.RowValues[])Enum.GetValues(typeof(CompiledTypes.HardPoints.RowValues));
                    HardPoints[] returnList = new HardPoints[values.Length];
                    for (int i = 0; i < values.Length; i++)
                    {
                        returnList[i] = Get(values[i]);
                    }
                    return returnList;
                }
 } //END OF HardPoints 
public class WeaponsType 
 {public Weapons JapanBB356mmA { get { return Get(CompiledTypes.Weapons.RowValues.JapanBB356mmA); } } 
public Weapons JapanBB356mmB { get { return Get(CompiledTypes.Weapons.RowValues.JapanBB356mmB); } } 
public Weapons JapanCA200mmA { get { return Get(CompiledTypes.Weapons.RowValues.JapanCA200mmA); } } 
public Weapons JapanCA200mmB { get { return Get(CompiledTypes.Weapons.RowValues.JapanCA200mmB); } } 
public Weapons JapanCA200mmY { get { return Get(CompiledTypes.Weapons.RowValues.JapanCA200mmY); } } 
public Weapons Japan120mmType10AATwin { get { return Get(CompiledTypes.Weapons.RowValues.Japan120mmType10AATwin); } } 
public Weapons JapanBBCasemate120mm { get { return Get(CompiledTypes.Weapons.RowValues.JapanBBCasemate120mm); } } 
public Weapons JapanTorpedo4CA { get { return Get(CompiledTypes.Weapons.RowValues.JapanTorpedo4CA); } } 
private Weapons Get(CompiledTypes.Weapons.RowValues line) { return new Weapons(parsedDB.Root, line); }

                public Weapons[] GetAll() 
                {
                    var values = (CompiledTypes.Weapons.RowValues[])Enum.GetValues(typeof(CompiledTypes.Weapons.RowValues));
                    Weapons[] returnList = new Weapons[values.Length];
                    for (int i = 0; i < values.Length; i++)
                    {
                        returnList[i] = Get(values[i]);
                    }
                    return returnList;
                }
 } //END OF Weapons 
public class Weapons_rolesType 
 {public Weapons_roles Artillery { get { return Get(CompiledTypes.Weapons_roles.RowValues.Artillery); } } 
public Weapons_roles NavalArtillery { get { return Get(CompiledTypes.Weapons_roles.RowValues.NavalArtillery); } } 
public Weapons_roles AntiAir { get { return Get(CompiledTypes.Weapons_roles.RowValues.AntiAir); } } 
public Weapons_roles Torpedo { get { return Get(CompiledTypes.Weapons_roles.RowValues.Torpedo); } } 
public Weapons_roles DepthCharge { get { return Get(CompiledTypes.Weapons_roles.RowValues.DepthCharge); } } 
private Weapons_roles Get(CompiledTypes.Weapons_roles.RowValues line) { return new Weapons_roles(parsedDB.Root, line); }

                public Weapons_roles[] GetAll() 
                {
                    var values = (CompiledTypes.Weapons_roles.RowValues[])Enum.GetValues(typeof(CompiledTypes.Weapons_roles.RowValues));
                    Weapons_roles[] returnList = new Weapons_roles[values.Length];
                    for (int i = 0; i < values.Length; i++)
                    {
                        returnList[i] = Get(values[i]);
                    }
                    return returnList;
                }
 } //END OF Weapons_roles 
public class AmmosType 
 {public Ammos ShellJapan356mm { get { return Get(CompiledTypes.Ammos.RowValues.ShellJapan356mm); } } 
public Ammos ShellJapan200mm { get { return Get(CompiledTypes.Ammos.RowValues.ShellJapan200mm); } } 
public Ammos ShellJapan152mm { get { return Get(CompiledTypes.Ammos.RowValues.ShellJapan152mm); } } 
public Ammos ShellJapan120mm { get { return Get(CompiledTypes.Ammos.RowValues.ShellJapan120mm); } } 
public Ammos TorpedoJapan610mm { get { return Get(CompiledTypes.Ammos.RowValues.TorpedoJapan610mm); } } 
private Ammos Get(CompiledTypes.Ammos.RowValues line) { return new Ammos(parsedDB.Root, line); }

                public Ammos[] GetAll() 
                {
                    var values = (CompiledTypes.Ammos.RowValues[])Enum.GetValues(typeof(CompiledTypes.Ammos.RowValues));
                    Ammos[] returnList = new Ammos[values.Length];
                    for (int i = 0; i < values.Length; i++)
                    {
                        returnList[i] = Get(values[i]);
                    }
                    return returnList;
                }
 } //END OF Ammos 
public class FX_StorageType 
 {public FX_Storage DamageExplosionLargePlaceholder { get { return Get(CompiledTypes.FX_Storage.RowValues.DamageExplosionLargePlaceholder); } } 
public FX_Storage DamageSmokeEmitterLarge { get { return Get(CompiledTypes.FX_Storage.RowValues.DamageSmokeEmitterLarge); } } 
public FX_Storage DamageSmokeEmitterMedium { get { return Get(CompiledTypes.FX_Storage.RowValues.DamageSmokeEmitterMedium); } } 
public FX_Storage DamageSteamEmitterLarge { get { return Get(CompiledTypes.FX_Storage.RowValues.DamageSteamEmitterLarge); } } 
public FX_Storage ShellExplosionLargePlaceholder { get { return Get(CompiledTypes.FX_Storage.RowValues.ShellExplosionLargePlaceholder); } } 
public FX_Storage ShellExplosionMediumPlaceholder { get { return Get(CompiledTypes.FX_Storage.RowValues.ShellExplosionMediumPlaceholder); } } 
public FX_Storage ShellExplosionSmallPlaceholder { get { return Get(CompiledTypes.FX_Storage.RowValues.ShellExplosionSmallPlaceholder); } } 
public FX_Storage ShellWaterHitLargePlaceholder { get { return Get(CompiledTypes.FX_Storage.RowValues.ShellWaterHitLargePlaceholder); } } 
public FX_Storage ShellWaterHitMediumPlaceholder { get { return Get(CompiledTypes.FX_Storage.RowValues.ShellWaterHitMediumPlaceholder); } } 
public FX_Storage ShellWaterHitSmallPlaceholder { get { return Get(CompiledTypes.FX_Storage.RowValues.ShellWaterHitSmallPlaceholder); } } 
public FX_Storage ShellWaterHitTorpedoPlaceholder { get { return Get(CompiledTypes.FX_Storage.RowValues.ShellWaterHitTorpedoPlaceholder); } } 
public FX_Storage StackSmokeEmitterLarge { get { return Get(CompiledTypes.FX_Storage.RowValues.StackSmokeEmitterLarge); } } 
public FX_Storage StackSmokeEmitterMedium { get { return Get(CompiledTypes.FX_Storage.RowValues.StackSmokeEmitterMedium); } } 
private FX_Storage Get(CompiledTypes.FX_Storage.RowValues line) { return new FX_Storage(parsedDB.Root, line); }

                public FX_Storage[] GetAll() 
                {
                    var values = (CompiledTypes.FX_Storage.RowValues[])Enum.GetValues(typeof(CompiledTypes.FX_Storage.RowValues));
                    FX_Storage[] returnList = new FX_Storage[values.Length];
                    for (int i = 0; i < values.Length; i++)
                    {
                        returnList[i] = Get(values[i]);
                    }
                    return returnList;
                }
 } //END OF FX_Storage 
public class AudioFX_StorageType 
 {public AudioFX_Storage musicA_calm { get { return Get(CompiledTypes.AudioFX_Storage.RowValues.musicA_calm); } } 
public AudioFX_Storage musicA_stress { get { return Get(CompiledTypes.AudioFX_Storage.RowValues.musicA_stress); } } 
public AudioFX_Storage ShellExplosion { get { return Get(CompiledTypes.AudioFX_Storage.RowValues.ShellExplosion); } } 
public AudioFX_Storage ShotFiring { get { return Get(CompiledTypes.AudioFX_Storage.RowValues.ShotFiring); } } 
public AudioFX_Storage TankExplosion { get { return Get(CompiledTypes.AudioFX_Storage.RowValues.TankExplosion); } } 
public AudioFX_Storage ShotCharging { get { return Get(CompiledTypes.AudioFX_Storage.RowValues.ShotCharging); } } 
public AudioFX_Storage turretRotation { get { return Get(CompiledTypes.AudioFX_Storage.RowValues.turretRotation); } } 
private AudioFX_Storage Get(CompiledTypes.AudioFX_Storage.RowValues line) { return new AudioFX_Storage(parsedDB.Root, line); }

                public AudioFX_Storage[] GetAll() 
                {
                    var values = (CompiledTypes.AudioFX_Storage.RowValues[])Enum.GetValues(typeof(CompiledTypes.AudioFX_Storage.RowValues));
                    AudioFX_Storage[] returnList = new AudioFX_Storage[values.Length];
                    for (int i = 0; i < values.Length; i++)
                    {
                        returnList[i] = Get(values[i]);
                    }
                    return returnList;
                }
 } //END OF AudioFX_Storage 

    }
}