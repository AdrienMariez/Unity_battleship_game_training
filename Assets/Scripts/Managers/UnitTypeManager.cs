using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
public class UnitTypeManager : MonoBehaviour {
    public GameObject m_MapBattleship;
    public GameObject m_MapCruiser;
    public GameObject m_MapDestroyer;
    public GameObject m_MapCarrier;

    private GameObject TempModel;

    public enum UnitType {
        battleship,
        cruiser,
        destroyer,
        carrier
    }
    public void CreateElement(GameObject unitGameObject, UnitType unitType, GameManager.Teams team) {
        if (unitType == UnitType.battleship) {
            TempModel = Instantiate(m_MapBattleship, unitGameObject.transform);
        } else if (unitType == UnitType.cruiser) {
            TempModel = Instantiate(m_MapCruiser, unitGameObject.transform);
        } else if (unitType == UnitType.destroyer) {
            TempModel = Instantiate(m_MapDestroyer, unitGameObject.transform);
        } else if (unitType == UnitType.carrier) {
            TempModel = Instantiate(m_MapCarrier, unitGameObject.transform);
        }

        var Renderer = TempModel.GetComponent<Renderer>();

        Renderer.material.SetColor("_Color", SetColor(team));
    }
    public static Color SetColor(GameManager.Teams team) {
        Color color = Color.yellow;;
        if (team == GameManager.Teams.Allies) {
            color = new Color(0f, 0.47f, 1f, 1f);
        } else if (team == GameManager.Teams.AlliesAI) {
            color = new Color(0f, 0.1f, 1f, 1f);
        } else if (team == GameManager.Teams.Axis) {
            color = new Color(1f, 0.22f, 0.29f, 1f);
        } else if (team == GameManager.Teams.AxisAI) {
            color = new Color(1f, 0.0f, 0.0f, 0.49f);
        } else if (team == GameManager.Teams.NeutralAI) {
            color = Color.yellow;
        }
        return color;
    }
}