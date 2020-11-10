using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;

public static class LoadingData {

    public static string MainMenu = "MainMenuScene3d";
    public static CompiledTypes.Scenarios SelectedScenario = null;

    public static string SceneToLoad;
        // public static CompiledTypes.Scenarios _tempSelectedScenario = null;

    // Gameplay Options :
        public static CompiledTypes.Teams PlayerTeam = null;
            // public static CompiledTypes.Teams _tempPlayerTeam = null;
        public static CompiledTypes.GameModes CurrentGameMode = null;
            // public static CompiledTypes.GameModes _tempCurrentGameMode = null;
    // Duel Options :
        public static  List<MenuButtonsControl.SpawnPointDuel> SpawnPointsDuel = null;
            // public static  List<MenuButtonsControl.SpawnPointDuel> _tempSpawnPointsDuel = null;

    public static void CleanData() {
        LoadingData.SelectedScenario = null;
        LoadingData.PlayerTeam = null;
        LoadingData.CurrentGameMode = null;
        LoadingData.SpawnPointsDuel = null;
    }
}