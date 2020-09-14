using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Monobehaviour marks that this script extends an existing class
public class GameModeDuel : MonoBehaviour {
    private GameManager GameManager;
    public UnitManager[] m_Units;

    [Header("For single Player : ")]
    [Tooltip("Check this box if it is a single player game mode.")]
    public bool m_SinglePlayerMode;
    public WorldUnitsManager.Teams m_SinglePlayerTeam;

    /*TODO here :
        Send all singleplayer data from GameManager here, change GameManager so that teams are Team1, Team2...
    */

    [Header("Gameplay options : ")]

    public int m_NumRoundsToWin = 5;            // The number of rounds a single player has to win to win the game.
    public float m_StartDelay = 3f;             // The delay between the start of RoundStarting and RoundPlaying phases.
    public float m_EndDelay = 3f;               // The delay between the end of RoundPlaying and RoundEnding phases.
    private WaitForSeconds m_StartWait;         // Used to have a delay whilst the round starts.
    private WaitForSeconds m_EndWait;           // Used to have a delay whilst the round or game ends.
    private int m_RoundNumber;                  // Which round the game is currently on.

    private int WinsAllies;                     // How many Allies round victories this far ?
    private int WinsAxis;                       // How many Axis round victories this far ?
    private WorldUnitsManager.Teams RoundWinner;                  // Who won this particular round ?
    private WorldUnitsManager.Teams GameWinner;                   // Who won the whole game ?

    private WorldUnitsManager WorldUnitsManager;

    public void BeginDuel() {
        // Create the delays so they only have to be made once.
        m_StartWait = new WaitForSeconds (m_StartDelay);
        m_EndWait = new WaitForSeconds (m_EndDelay);

        WorldUnitsManager = GameObject.Find("GlobalSharedVariables").GetComponent<WorldUnitsManager>();

        // Once the units have been created and the camera is using them as targets, start the game.
        StartCoroutine (GameLoop ());
    }

    public void SetGameManager(GameManager gameManager) {
        GameManager = gameManager;
    }

    // This is called from start and will run each phase of the game one after another.
    private IEnumerator GameLoop () {
        // Start off by running the 'RoundStarting' coroutine but don't return until it's finished.
        yield return StartCoroutine (RoundStarting ());

        // Once the 'RoundStarting' coroutine is finished, run the 'RoundPlaying' coroutine but don't return until it's finished.
        yield return StartCoroutine (RoundPlaying());

        // Once execution has returned here, run the 'RoundEnding' coroutine, again don't return until it's finished.
        yield return StartCoroutine (RoundEnding());

        // This code is not run until 'RoundEnding' has finished.  At which point, check if a game winner has been found.
        if (GameWinner != WorldUnitsManager.Teams.NeutralAI) {
            // If there is a game winner, restart the level.
            // SceneManager.LoadScene (0);
            GameManager.EndGame();
        } else {
            // If there isn't a winner yet, restart this coroutine so the loop continues.

            foreach (UnitManager unit in m_Units) {
                unit.Destroy();
            }
            // PlayerManager.UnitsUIManagerKillAllInstances();
            
            // Reset the assets for the player
            GameManager.ResetPlayerManager();
            
            // Note that this coroutine doesn't yield.  This means that the current version of the GameLoop will end.
            StartCoroutine (GameLoop ());
        }
    }

    private IEnumerator RoundStarting () {
        // Reset counters
        GameManager.SetPlayableUnits(0);
        GameManager.SetEnemiesUnits(0);

        // Setup each unit
        for (int i = 0; i < m_Units.Length; i++) {
            if (m_Units[i].m_UseSpawnpoint) {
                m_Units[i].SetInstance(Instantiate(m_Units[i].m_UnitPrefab, m_Units[i].m_SpawnPoint.position, m_Units[i].m_SpawnPoint.rotation) as GameObject);
            }
            // TODO if not using a spawn point...

            m_Units[i].SetGameManager(GameManager);
            m_Units[i].SetPlayerManager(GameManager.GetPlayerManager());
            m_Units[i].Setup();
            m_Units[i].SetUnactive();
        }

        // As soon as the round starts reset the units and make sure they can't move.
        ResetAllUnits ();
        DisableUnitsControl ();

        // Increment the round number and display text showing the players what round it is.
        m_RoundNumber++;


        GameManager.SetScoreMessage("ROUND " + m_RoundNumber);

        // Wait for the specified length of time until yielding control back to the game loop.
        yield return m_StartWait;
    }

    private IEnumerator RoundPlaying () {
        // As soon as the round begins playing let the players control the tanks.
        EnableUnitsControl ();

        // Show score on the screen.
        GameManager.SetScoreMessage(GameMessage());

        // While there is not one side reduced to 0...
        while (!NoUnitLeftOnOneSide()) {
            // ... return on the next frame.
            yield return null;
        }
    }

    private IEnumerator RoundEnding () {
        // Disable units
        DisableUnitsControl ();

        // Clear the winner from the previous round.
        RoundWinner = WorldUnitsManager.Teams.NeutralAI;

        // See if there is a winner now the round is over.
        RoundWinner = GetRoundWinner ();

        // If there is a winner, increment their score.
        if (RoundWinner != WorldUnitsManager.Teams.NeutralAI){
            if (RoundWinner == WorldUnitsManager.Teams.Allies) {
                WinsAllies++;
            }
            if (RoundWinner == WorldUnitsManager.Teams.Axis) {
                WinsAxis++;
            }
        }

        // Now the winner's score has been incremented, see if someone has one the game.
        GameWinner = GetGameWinner ();

        // Get a message based on the scores and whether or not there is a game winner and display it.
        GameManager.SetScoreMessage(EndMessage ());

        // Wait for the specified length of time until yielding control back to the game loop.
        yield return m_EndWait;
    }

    private bool NoUnitLeftOnOneSide() {
        // If there are still playable units or enemy units...
        bool sideExterminated = false;
        if (GameManager.GetPlayableUnits() == 0 || GameManager.GetEnemiesUnits() == 0) {
            sideExterminated = true;
        }
        return sideExterminated;
    }

    private WorldUnitsManager.Teams GetRoundWinner() {
        // If the playable units are depleted, it is a player defeat
        if (GameManager.GetPlayableUnits() == 0 && GameManager.GetEnemiesUnits() > 0) {
            if (GameManager.GetSoloPlayerTeam() == WorldUnitsManager.Teams.Allies) {
                return WorldUnitsManager.Teams.Axis;
            } else {
                return WorldUnitsManager.Teams.Allies;
            }
        }
        // If the enemy units are depleted, it is a player victory
        if (GameManager.GetPlayableUnits() > 0 && GameManager.GetEnemiesUnits() == 0) {
            if (GameManager.GetSoloPlayerTeam() == WorldUnitsManager.Teams.Allies) {
                return WorldUnitsManager.Teams.Allies;
            } else {
                return WorldUnitsManager.Teams.Axis;
            }
        }
        // If both player units and enemy units are depleted, it is a draw
        return WorldUnitsManager.Teams.NeutralAI;
    }

    private WorldUnitsManager.Teams GetGameWinner() {
        if (WinsAllies == m_NumRoundsToWin) {
            return WorldUnitsManager.Teams.Allies;
        }
        if (WinsAxis == m_NumRoundsToWin) {
            return WorldUnitsManager.Teams.Axis;
        }

        // If no tanks have enough rounds to win, return null.
        return WorldUnitsManager.Teams.NeutralAI;
    }


    // Message system
    private string EndMessage() {
        // By default when a round ends there are no winners so the default end message is a draw.
        string message = "DRAW!";

        // If there is a round winner...
        if (RoundWinner != WorldUnitsManager.Teams.NeutralAI) {
            if (RoundWinner == WorldUnitsManager.Teams.Allies) {
                message = "Allies won the round.";
            } else {
                message = "Axis won the round.";
            }
        }
        // If there is a game winner...
        if (GameWinner != WorldUnitsManager.Teams.NeutralAI) {
            if (GameWinner == WorldUnitsManager.Teams.Allies) {
                message = "Allies won the game !";
            } else {
                message = "Axis won the game !";
            }
        }

        // Add some line breaks after the initial message.
        message += "\n\n\n\n";

        // Display WorldUnitsManager.Teams scores :
        // Does not display correct score for unsolved reasons (TODO)
        // message += "Allies victories : " + WinsAllies + " victories.\n";
        // message += "Axis victories : " + WinsAllies + " victories.\n";
    

        return message;
    }

    private string GameMessage() {
        string message;

        if (GameManager.GetPlayableUnits() >  1) {
            message = "Player units : " + GameManager.GetPlayableUnits() +"\n";
        } else {
            message = "Player unit : " + GameManager.GetPlayableUnits() +"\n";
        }

        if (GameManager.GetSoloPlayerTeam() == WorldUnitsManager.Teams.Allies) {
            message += "Wins : "+ WinsAllies +"/"+ m_NumRoundsToWin +"\n";
        } else {
            message += "Wins : "+ WinsAxis +"/"+ m_NumRoundsToWin +"\n";
        }

        if (GameManager.GetPlayableUnits() >  1) {
            message += "Enemy units : " + GameManager.GetEnemiesUnits() +"\n";
        } else {
            message += "Enemy unit : " + GameManager.GetEnemiesUnits() +"\n";
        }

        if (GameManager.GetSoloPlayerTeam() == WorldUnitsManager.Teams.Allies) {
            message += "Wins : "+ WinsAxis +"/"+ m_NumRoundsToWin +"\n";
        } else {
            message += "Wins : "+ WinsAllies +"/"+ m_NumRoundsToWin +"\n";
        }

        return message;
    }

    public void UpdateMessage() { 
        //Hi I am needed by GameManager, please do not mess with me.
        GameManager.SetScoreMessage(GameMessage());
    }

    public WorldUnitsManager.Teams GetPlayer(){
        return GameManager.GetSoloPlayerTeam();
    }

    public void UpdateGameplay() {
        //Add specifics for gameplay when a unit dies/spawns here
    }

    // Those functions will be common for many gamemodes
    private void ResetAllUnits() {
        for (int i = 0; i < m_Units.Length; i++) {
            m_Units[i].Reset();
        }
    }

    private void EnableUnitsControl() {
        for (int i = 0; i < m_Units.Length; i++) {
            m_Units[i].EnableControl();
        }
    }

    private void DisableUnitsControl() {
        for (int i = 0; i < m_Units.Length; i++) {
            m_Units[i].DisableControl();
        }
    }

}