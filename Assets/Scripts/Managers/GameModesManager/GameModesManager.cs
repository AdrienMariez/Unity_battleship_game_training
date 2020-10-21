using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModesManager : MonoBehaviour {
    // Global parameters
    protected UnitManager[] Units;
    protected GameManager GameManager;
    bool CustomScenario = false;
    CustomScenariosManager CustomScenariosManager;

    //Scenarios parameters
    protected int NumRoundsToWin = 1;            // Look in GameModeDuel on how to override me. Without any option, the game ends after a single round.
    protected WaitForSeconds StartWait;         // Used to have a delay whilst the round starts.
    protected WaitForSeconds EndWait;           // Used to have a delay whilst the round or game ends.
    protected int WinsAllies;                     // How many Allies round victories this far ?
    protected int WinsAxis;                       // How many Axis round victories this far ?
    protected CompiledTypes.Teams.RowValues RoundWinner;                  // Who won this particular round ?
    protected CompiledTypes.Teams.RowValues GameWinner;                   // Who won the whole game ?
    protected int RoundNumber;                  // Which round the game is currently on.

    [Header("General options : ")]
    public bool ScenarioUsesCommandPointsForSpawnSystem;

    
    public virtual void SetGameManager(GameManager gameManager) {
        GameManager = gameManager;
        GameManager.SetCommandPointSystem(ScenarioUsesCommandPointsForSpawnSystem);
        if (GetComponent<CustomScenariosManager>()) {
            CustomScenario = true;
            CustomScenariosManager = GetComponent<CustomScenariosManager>();
            CustomScenariosManager.SetGameManager(gameManager);
        } else {
            CustomScenario = false;
            // Debug.Log ("No custom scenario called");
        }
    }
    public virtual void Begin() {
        if (CustomScenario) { CustomScenariosManager.Begin(); }
    }

    // Game Loop
    // This is called from start and will run each phase of the game one after another.
    protected virtual IEnumerator GameLoop () {
        // Debug.Log ("GameLoop ");
        // Start off by running the 'RoundStarting' coroutine but don't return until it's finished.
        yield return StartCoroutine (RoundStarting ());

        // Once the 'RoundStarting' coroutine is finished, run the 'RoundPlaying' coroutine but don't return until it's finished.
        yield return StartCoroutine (RoundPlaying());

        // Once execution has returned here, run the 'RoundEnding' coroutine, again don't return until it's finished.
        yield return StartCoroutine (RoundEnding());

        // This code is not run until 'RoundEnding' has finished.  At which point, check if a game winner has been found.
        if (GameWinner != CompiledTypes.Teams.RowValues.Neutral) {
            // If there is a game winner, restart the level.
            // SceneManager.LoadScene (0);
            GameManager.EndGame();
        } else {
            // If there isn't a winner yet, restart this coroutine so the loop continues.

            foreach (UnitManager unit in Units) {
                unit.Destroy();
            }
            // PlayerManager.UnitsUIManagerKillAllInstances();
            
            // Reset the assets for the player
            GameManager.ResetPlayerManager();
            
            // Note that this coroutine doesn't yield.  This means that the current version of the GameLoop will end.
            StartCoroutine (GameLoop ());
        }
    }
    protected virtual IEnumerator RoundStarting () {
        // Debug.Log ("RoundStarting ");
        // Increment the round number
        RoundNumber++;

        GameManager.SetScoreMessage(StartMessage());

        // Reset all counters
        GameManager.ResetCounters();

        // Setup each unit
        foreach (var unit in Units) {
            // TODO if not using a spawn point...
            // if (unit.m_UseSpawnpoint) {
                foreach (List<WorldSingleUnit> subCategory in WorldUnitsManager.GetUnitsBySubcategory()) {
                    foreach (WorldSingleUnit worldUnit in subCategory) {
                        if (unit.GetUnit().ToString() == worldUnit.GetUnitReference_DB().id.ToString()) {
                            // Debug.Log (unit.GetUnit().ToString()+" / "+worldUnit.GetUnitReference_DB().id.ToString());
                            unit.SetInstance(WorldUnitsManager.BuildUnit(worldUnit, unit.m_SpawnPoint.position, unit.m_SpawnPoint.rotation));
                        }
                    }
                }
                // unit.SetInstance(Instantiate(unit.m_UnitPrefab, unit.m_SpawnPoint.position, unit.m_SpawnPoint.rotation) as GameObject);
                // Will need to replace previous line with next line to use DB.
                // unit.SetInstance(WorldUnitsManager.BuildUnit(unit.m_UnitPrefab, unit.m_SpawnPoint.position, unit.m_SpawnPoint.rotation));
            // }
            unit.SetGameManager(GameManager);

            // If there are no corresponding playermanager, none will be sent... May cause bugs in the future, to check.
            foreach (PlayerManagerList playerManagerSingle in GameManager.GetPlayerManagerList()) {
                if (unit.m_Team == playerManagerSingle.m_PlayerTeam) {
                    unit.SetPlayerManager(playerManagerSingle.m_Player.GetComponent<PlayerManager>());
                }
            }
            unit.Setup();
            // unit.SetUnactive();
        }

        // As soon as the round starts reset the units and make sure they can't move.
        ResetAllUnits ();
        DisableUnitsControl ();

        // Wait for the specified length of time until yielding control back to the game loop.
        yield return StartWait;
    }
    protected virtual IEnumerator RoundPlaying () {
        // Debug.Log ("RoundPlaying ");
        // As soon as the round begins playing let the players control the tanks.
        EnableUnitsControl ();

        // Show score on the screen.
        UpdateMessage();

        // While the objective isn't accomplished for either side...
        while (!ObectiveAccomplished()) {
            // ... return on the next frame.
            yield return null;
        }
    }
    protected virtual IEnumerator RoundEnding () {
        // When a single round ends...
        DisableUnitsControl ();

        // Clear the winner from the previous round.
        RoundWinner = CompiledTypes.Teams.RowValues.Neutral;

        // See if there is a winner now the round is over.
        RoundWinner = GetRoundWinner();

        // If there is a winner, increment their score.
        if (RoundWinner != CompiledTypes.Teams.RowValues.Neutral){
            if (RoundWinner == CompiledTypes.Teams.RowValues.Allies) {
                WinsAllies++;
            }
            if (RoundWinner == CompiledTypes.Teams.RowValues.Axis) {
                WinsAxis++;
            }
        }

        // Now the winner's score has been incremented, see if someone has one the game.
        GameWinner = GetGameWinner ();

        // Get a message based on the scores and whether or not there is a game winner and display it.
        GameManager.SetScoreMessage(EndMessage ());

        // Wait for the specified length of time until yielding control back to the game loop.
        yield return EndWait;
    }
    // Returns for game loop
    protected virtual bool ObectiveAccomplished() {
        // BASIC : If there are still playable units or enemy units...
        bool obectiveAccomplishedForOneSide = false;
        if (GameManager.GetTeamAlliesUnits() == 0 || GameManager.GetTeamOppositionUnits() == 0) {
            obectiveAccomplishedForOneSide = true;
        }
        return obectiveAccomplishedForOneSide;
    }
    protected virtual CompiledTypes.Teams.RowValues GetRoundWinner() {
        // BASIC : Returns the winner of a single round
        if (GameManager.GetTeamAlliesUnits() == 0 && GameManager.GetTeamOppositionUnits() > 0) {
            return CompiledTypes.Teams.RowValues.Axis;
        } else if (GameManager.GetTeamAlliesUnits() > 0 && GameManager.GetTeamOppositionUnits() == 0) {
            return CompiledTypes.Teams.RowValues.Allies;
        } else {
            // Any other case gives a draw
            return CompiledTypes.Teams.RowValues.Neutral;
        }
    }
    protected virtual CompiledTypes.Teams.RowValues GetGameWinner() {
        // Returns the winner of the game
        if (WinsAllies == NumRoundsToWin) {
            return CompiledTypes.Teams.RowValues.Allies;
        }
        if (WinsAxis == NumRoundsToWin) {
            return CompiledTypes.Teams.RowValues.Axis;
        }

        // If no side enough rounds to win, return null.
        return CompiledTypes.Teams.RowValues.Neutral;
    }

    // Message system (displays info to players)
    protected virtual string StartMessage() {
        // Displays the message shown on screen at the beginning of a round / of gameplay
        string message;

        message = "Round : " + RoundNumber;

        return message;
    }
    protected virtual string GameMessage() {
        // Displays basic message shown on screen during the duration of a round / of gameplay for all players.
        string message;
        if (GameManager.GetTeamAlliesUnits() >  1) {
            message = "Allied units : " + GameManager.GetTeamAlliesUnits() +"\n";
        } else {
            message = "Allied unit : " + GameManager.GetTeamAlliesUnits() +"\n";
        }
        message += "Wins : "+ WinsAllies +"/"+ NumRoundsToWin +"\n";

        if (GameManager.GetTeamOppositionUnits() >  1) {
            message += "Axis units : " + GameManager.GetTeamOppositionUnits() +"\n";
        } else {
            message += "Axis unit : " + GameManager.GetTeamOppositionUnits() +"\n";
        }
        message += "Wins : "+ WinsAxis +"/"+ NumRoundsToWin +"\n";

        return message;
    }
    public virtual string GameMessageTeam(CompiledTypes.Teams.RowValues team) {
        // Displays personnalized for each team message shown on screen during the duration of a round / of gameplay 
        string message;
        if (GameManager.GetTeamAlliesUnits() >  1) {
            message = "Allied units : " + GameManager.GetTeamAlliesUnits() +"\n";
        } else {
            message = "Allied unit : " + GameManager.GetTeamAlliesUnits() +"\n";
        }
        message += "Wins : "+ WinsAllies +"/"+ NumRoundsToWin +"\n";

        if (GameManager.GetTeamOppositionUnits() >  1) {
            message += "Axis units : " + GameManager.GetTeamOppositionUnits() +"\n";
        } else {
            message += "Axis unit : " + GameManager.GetTeamOppositionUnits() +"\n";
        }
        message += "Wins : "+ WinsAxis +"/"+ NumRoundsToWin +"\n";

        return message;
    }
    protected virtual string EndMessage() {
        // Displays the message shown on screen at the end of a round / of gameplay
        // By default when a round ends there are no winners so the default end message is a draw.
        string message = "DRAW!";

        // If there is a ROUND winner...
        if (RoundWinner != CompiledTypes.Teams.RowValues.Neutral) {
            if (RoundWinner == CompiledTypes.Teams.RowValues.Allies) {
                message = "Allies won the round.";
            } else {
                message = "Axis won the round.";
            }
        }
        // If there is a GAME winner... NOT THE SAME !
        if (GameWinner != CompiledTypes.Teams.RowValues.Neutral) {
            if (GameWinner == CompiledTypes.Teams.RowValues.Allies) {
                message = "Allies won the game !";
            } else {
                message = "Axis won the game !";
            }
        }

        // Add some line breaks after the initial message.
        message += "\n\n\n\n";

        // Display CompiledTypes.Teams.RowValues scores :
        // Does not display correct score for unsolved reasons (TODO)
        // message += "Allies victories : " + WinsAllies + " victories.\n";
        // message += "Axis victories : " + WinsAllies + " victories.\n";
    
        return message;
    }
    public virtual void UpdateMessage() {
        // Activated when a unit spawns or dies.
        if (CustomScenario)
            CustomScenariosManager.UpdateMessage();
    }
    public virtual void UpdateGameplay() {
        // Activated when a unit spawns or dies.
        if (CustomScenario)
            CustomScenariosManager.UpdateGameplay();
        // Generic interactive stuff
    }

    // Units
    protected void ResetAllUnits() {
        for (int i = 0; i < Units.Length; i++) {
            Units[i].Reset();
        }
    }
    protected void EnableUnitsControl() {
        for (int i = 0; i < Units.Length; i++) {
            Units[i].EnableControl();
        }
    }
    protected void DisableUnitsControl() {
        for (int i = 0; i < Units.Length; i++) {
            Units[i].DisableControl();
        }
    }
}