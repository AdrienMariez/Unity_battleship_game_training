using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UnitUI : UnitParameter {
    private bool Dead = false;
    private string Name; public void SetName(string name) { Name = name; }
    private CompiledTypes.Teams Team; public void SetUnitTeam(CompiledTypes.Teams team){ Team = team; }
    private float MaximumHealth; public void SetStartingHealth(float FullHP) { MaximumHealth = FullHP; CurrentHealth = FullHP; }
    private float CurrentHealth;
    private List <GameObject> UIElement = new List<GameObject>();
    private List <GameObject> UIMapElement = new List<GameObject>();
    private List <UnitUIManager> UIElementManager = new List<UnitUIManager>();
    private List <UnitMapUIManager> UIMapElementManager = new List<UnitMapUIManager>();

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
        UIElementManager.Add(uiElement.GetComponent<UnitUIManager>());
    }
    public void SetUIMapElement(GameObject uiElement) {
        // UIElement = uiElement;
        uiElement.gameObject.name = Name;
        uiElement.transform.Find("Name").GetComponent<Text>().text = Name;
        uiElement.transform.Find("Name").GetComponent<Text>().color = WorldUnitsManager.SetColor(Team);
        uiElement.transform.Find("Distance").GetComponent<Text>().color = WorldUnitsManager.SetColor(Team);
        uiElement.transform.Find("Health").GetComponent<Slider>().maxValue = MaximumHealth;
        uiElement.transform.Find("Health").GetComponent<Slider>().value = CurrentHealth;
        uiElement.GetComponent<UnitMapUIManager>().SetUnitTeam(Team);
        UIMapElementManager.Add(uiElement.GetComponent<UnitMapUIManager>());
    }

    public void SetCurrentHealth(float HP) {
        CurrentHealth = HP;
        if (!Dead) {  
            Color barColor = CheckHealthColor();
            foreach (UnitUIManager element in UIElementManager) {
                element.SetCurrentHealth(HP, barColor);
            }
            foreach (UnitMapUIManager element in UIMapElementManager) {
                element.SetCurrentHealth(HP, barColor);
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

    public void ChangeName(string name) {
        foreach (UnitUIManager element in UIElementManager) {
            element.ChangeName(name);
        }
        foreach (UnitMapUIManager element in UIMapElementManager) {
            element.ChangeName(name);
        }
    }

    public void SetDead() {
        Dead = true;
        foreach (UnitUIManager element in UIElementManager) {
            element.SetDead();
        }
        UIElementManager.Clear();
        foreach (UnitMapUIManager element in UIMapElementManager) {
            element.SetDead();
        }
        UIMapElementManager.Clear();
    }
    public void KillAllUIInstances() {
        foreach (UnitUIManager element in UIElementManager) {
            element.Destroy();
        }
        foreach (UnitMapUIManager element in UIMapElementManager) {
            element.Destroy();
        }
        UIElementManager.Clear();
        UIMapElementManager.Clear();
    }
}