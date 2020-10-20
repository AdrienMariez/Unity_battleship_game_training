using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BuildingUI : MonoBehaviour {
    private bool Dead = false;
    private string Name;
    private CompiledTypes.Teams.RowValues Team;
    private float MaximumHealth;
    private float CurrentHealth;
    private List <GameObject> UIElement = new List<GameObject>();
    private List <GameObject> UIMapElement = new List<GameObject>();

    public void SetName(string name) { Name = name; }
    public void SetUnitTeam(CompiledTypes.Teams.RowValues team){ Team = team; }
    public void SetStartingHealth(float FullHP) {
        MaximumHealth = FullHP;
        CurrentHealth = FullHP;
    }

    public void SetUIElement(GameObject uiElement) {
        uiElement.gameObject.name = Name;
        uiElement.transform.Find("Name").GetComponent<Text>().text = Name;
        uiElement.transform.Find("Name").GetComponent<Text>().color = WorldUnitsManager.SetColor(Team);
        uiElement.transform.Find("Distance").GetComponent<Text>().color = WorldUnitsManager.SetColor(Team);
        uiElement.transform.Find("Pointer").GetComponent<Image>().color = WorldUnitsManager.SetColor(Team);
        uiElement.transform.Find("BoundingBox").transform.Find("BoundingBoxTopLeft").GetComponent<Image>().color = WorldUnitsManager.SetColor(Team);
        uiElement.transform.Find("BoundingBox").transform.Find("BoundingBoxTopRight").GetComponent<Image>().color = WorldUnitsManager.SetColor(Team);
        uiElement.transform.Find("BoundingBox").transform.Find("BoundingBoxBottomLeft").GetComponent<Image>().color = WorldUnitsManager.SetColor(Team);
        uiElement.transform.Find("BoundingBox").transform.Find("BoundingBoxBottomRight").GetComponent<Image>().color = WorldUnitsManager.SetColor(Team);
        uiElement.transform.Find("Health").GetComponent<Slider>().maxValue = MaximumHealth;
        uiElement.transform.Find("Health").GetComponent<Slider>().value = CurrentHealth;
        UIElement.Add(uiElement);
    }
    public void SetUIMapElement(GameObject uiElement) {
        // UIElement = uiElement;
        uiElement.gameObject.name = Name;
        uiElement.transform.Find("Name").GetComponent<Text>().text = Name;
        uiElement.transform.Find("Name").GetComponent<Text>().color = WorldUnitsManager.SetColor(Team);
        uiElement.transform.Find("Distance").GetComponent<Text>().color = WorldUnitsManager.SetColor(Team);
        uiElement.transform.Find("Health").GetComponent<Slider>().maxValue = MaximumHealth;
        uiElement.transform.Find("Health").GetComponent<Slider>().value = CurrentHealth;
        UIMapElement.Add(uiElement);
    }

    public void SetCurrentHealth(float HP) {
        CurrentHealth = HP;
        if (!Dead) {  
            Color barColor = CheckHealthColor();
            foreach (var element in UIElement) {
                element.GetComponent<UnitUIManager>().SetCurrentHealth(HP, barColor);
            }
            foreach (var element in UIMapElement) {
                element.GetComponent<UnitMapUIManager>().SetCurrentHealth(HP, barColor);
            }
        }
    }
    private Color CheckHealthColor() {
        if (CurrentHealth <= (0.2f * MaximumHealth)) {
            return Color.red;
        } else if (CurrentHealth <= (0.6f * MaximumHealth)) {
            return Color.yellow;
        } else {
            return new Color(0.0f, 0.75f, 0.14f);
            // return Color.green;
        }
    }

    public void SetDead() {
        Dead = true;
        foreach (var element in UIElement) {
            element.GetComponent<UnitUIManager>().SetDead();
        }
        UIElement.Clear();
        foreach (var element in UIMapElement) {
            element.GetComponent<UnitMapUIManager>().SetDead();
        }
        UIMapElement.Clear();
    }
    public void KillAllUIInstances() {
        foreach (var element in UIElement) {
            element.GetComponent<UnitUIManager>().Destroy();
        }
        foreach (var element in UIMapElement) {
            element.GetComponent<UnitMapUIManager>().Destroy();
        }
        UIElement.Clear();
        UIMapElement.Clear();
    }
}