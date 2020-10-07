
using UnityEngine;
using CastleDBImporter;
using System.Collections.Generic;
using System;

namespace CompiledTypes
{
    public class CastleDB
    {
        static CastleDBParser parsedDB;
        public UnitsType Units;
public Units_categoriesType Units_categories;
public Units_sub_categoriesType Units_sub_categories;
public TeamsType Teams;
public CountriesType Countries;
public WeaponsType Weapons;
public Weapons_rolesType Weapons_roles;
public AmmosType Ammos;

        public CastleDB(TextAsset castleDBAsset)
        {
            parsedDB = new CastleDBParser(castleDBAsset);
            Units = new UnitsType();Units_categories = new Units_categoriesType();Units_sub_categories = new Units_sub_categoriesType();Teams = new TeamsType();Countries = new CountriesType();Weapons = new WeaponsType();Weapons_roles = new Weapons_rolesType();Ammos = new AmmosType();
        }
        public class UnitsType 
 {public Units fuso { get { return Get(CompiledTypes.Units.RowValues.fuso); } } 
public Units takao { get { return Get(CompiledTypes.Units.RowValues.takao); } } 
private Units Get(CompiledTypes.Units.RowValues line) { return new Units(parsedDB.Root, line); }

                public Units[] GetAll() 
                {
                    var values = (CompiledTypes.Units.RowValues[])Enum.GetValues(typeof(CompiledTypes.Units.RowValues));
                    Units[] returnList = new Units[values.Length];
                    for (int i = 0; i < values.Length; i++)
                    {
                        returnList[i] = Get(values[i]);
                    }
                    return returnList;
                }
 } //END OF Units 
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
public class Units_sub_categoriesType 
 {public Units_sub_categories Battleship { get { return Get(CompiledTypes.Units_sub_categories.RowValues.Battleship); } } 
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
 {public Weapons_roles artillery { get { return Get(CompiledTypes.Weapons_roles.RowValues.artillery); } } 
public Weapons_roles navalartillery { get { return Get(CompiledTypes.Weapons_roles.RowValues.navalartillery); } } 
public Weapons_roles antiair { get { return Get(CompiledTypes.Weapons_roles.RowValues.antiair); } } 
public Weapons_roles torpedoes { get { return Get(CompiledTypes.Weapons_roles.RowValues.torpedoes); } } 
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

    }
}