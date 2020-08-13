using System;
using UnityEngine;

public class WorldUIVariables : MonoBehaviour {
    [Header("Player UI")]
        public GameObject m_TankUI;
        public GameObject m_PlaneUI;
        public GameObject m_ShipUI;
        public GameObject m_PlayerMapUI;
        public GameObject m_TurretUI;
        public GameObject m_PauseMenu;
    [Header("Turrets status icons")]
        public GameObject TurretStatusSprites;
        public float IconsSpacing = 22;
    [Header("Spawner UI")]
        public GameObject m_SpawnerUI;
    [Header("Shell Camera")]
        public GameObject m_ShellCamera;
        public float m_TimeToDestroyCamera = 3;
    [Header("Units UI")]
        public GameObject m_UnitUI;
        public GameObject m_UnitMapUI;
}