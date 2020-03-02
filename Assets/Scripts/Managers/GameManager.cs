using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Monobehaviour marks that this script extends an existing class
public class GameManager : MonoBehaviour {
    public enum Teams {
        Allies,
        AlliesAI,
        Axis,
        AxisAI,
        NeutralAI
    }
    public enum Nations {
        US,
        Japan,
        GB,
        Germany,
        USSR,
        China,
        France
    }

    public Teams m_PlayerTeam;
    public UnitManager[] m_Units; 

    public int m_NumRoundsToWin = 5;            // The number of rounds a single player has to win to win the game.
    public float m_StartDelay = 3f;             // The delay between the start of RoundStarting and RoundPlaying phases.
    public float m_EndDelay = 3f;               // The delay between the end of RoundPlaying and RoundEnding phases.
    private int m_RoundNumber;                  // Which round the game is currently on.
    private WaitForSeconds m_StartWait;         // Used to have a delay whilst the round starts.
    private WaitForSeconds m_EndWait;           // Used to have a delay whilst the round or game ends.

    private int PlayableUnits;
    private int EnemiesUnits;
    private int WinsAllies;                     // How many Allies round victories this far ?
    private int WinsAxis;                       // How many Axis round victories this far ?
    private Teams RoundWinner;                  // Who won this particular round ?
    private Teams GameWinner;                   // Who won the whole game ?

    private PlayerManager PlayerManager;
    private GameObject PlayerCanvas;
    private GameObject PlayerMapCanvas;

    private void Start() {
        // Create the delays so they only have to be made once.
        m_StartWait = new WaitForSeconds (m_StartDelay);
        m_EndWait = new WaitForSeconds (m_EndDelay);
        PlayerManager = GetComponent<PlayerManager>();
        PlayerCanvas = GameObject.Find("UICanvas");
        PlayerMapCanvas = GameObject.Find("UIMapCanvas");

        PlayerManager.SetPlayerCanvas(PlayerCanvas, PlayerMapCanvas);

        // Once the units have been created and the camera is using them as targets, start the game.
        StartCoroutine (GameLoop ());
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
        if (GameWinner != Teams.NeutralAI) {
            // If there is a game winner, restart the level.
            // SceneManager.LoadScene (0);
            EndGame();
        } else {
            // If there isn't a winner yet, restart this coroutine so the loop continues.
            // Note that this coroutine doesn't yield.  This means that the current version of the GameLoop will end.
            StartCoroutine (GameLoop ());
        }
    }

    private IEnumerator RoundStarting () {
        // Reset counters
        PlayableUnits = 0;
        EnemiesUnits = 0;

        // Setup each unit
        for (int i = 0; i < m_Units.Length; i++) {
            m_Units[i].Destroy();

            if (m_Units[i].m_UseSpawnpoint) {
                m_Units[i].SetInstance(Instantiate(m_Units[i].m_UnitPrefab, m_Units[i].m_SpawnPoint.position, m_Units[i].m_SpawnPoint.rotation) as GameObject);
            }
            // TODO if not using a spawn point...

            m_Units[i].SetGameManager(this);
            m_Units[i].SetPlayerManager(PlayerManager);
            m_Units[i].Setup();

            // Set the needed units to attain win conditions
            if (m_PlayerTeam == Teams.Allies) {
                if (m_Units[i].m_Team == Teams.Axis || m_Units[i].m_Team == Teams.AxisAI) {
                    EnemiesUnits ++;
                    m_Units[i].SetUnactive();
                }
            } else {
                if (m_Units[i].m_Team == Teams.Allies || m_Units[i].m_Team == Teams.AlliesAI) {
                    EnemiesUnits ++;
                    m_Units[i].SetUnactive();
                }
            }

            // Set the playable units the player must keep to attain win conditions
            if (m_PlayerTeam == Teams.Allies) {
                if (m_Units[i].m_Team == Teams.Allies) {
                    PlayableUnits ++;
                }
            } else {
                if (m_Units[i].m_Team == Teams.Axis) {
                    PlayableUnits ++;
                }
            }
        }
        for (int i = 0; i < m_Units.Length; i++) {
            m_Units[i].SetUnitName();
        }
        PlayerManager.InitUnitsUI();

        // As soon as the round starts reset the units and make sure they can't move.
        ResetAllUnits ();
        DisableUnitsControl ();

        // Reset the assets for the player
        PlayerManager.Reset();

        // Increment the round number and display text showing the players what round it is.
        m_RoundNumber++;
        // m_MessageText.text = "ROUND " + m_RoundNumber;
        PlayerManager.SetScoreMessage("ROUND " + m_RoundNumber);

        // Wait for the specified length of time until yielding control back to the game loop.
        yield return m_StartWait;
    }

    private IEnumerator RoundPlaying () {
        // As soon as the round begins playing let the players control the tanks.
        EnableUnitsControl ();

        // Show score on the screen.
        PlayerManager.SetScoreMessage(GameMessage());

        // While there is not one side reduced to 0...
        while (!NoUnitLeftOnOneSide())
        {
            // ... return on the next frame.
            yield return null;
        }
    }

    private IEnumerator RoundEnding () {
        // Disable units
        DisableUnitsControl ();

        // Clear the winner from the previous round.
        RoundWinner = Teams.NeutralAI;

        // See if there is a winner now the round is over.
        RoundWinner = GetRoundWinner ();

        // If there is a winner, increment their score.
        if (RoundWinner != Teams.NeutralAI){
            if (RoundWinner == Teams.Allies) {
                WinsAllies++;
            }
            if (RoundWinner == Teams.Axis) {
                WinsAxis++;
            }
        }

        // Now the winner's score has been incremented, see if someone has one the game.
        GameWinner = GetGameWinner ();

        // Get a message based on the scores and whether or not there is a game winner and display it.
        PlayerManager.SetScoreMessage(EndMessage ());

        // Wait for the specified length of time until yielding control back to the game loop.
        yield return m_EndWait;
    }

    private bool NoUnitLeftOnOneSide() {
        // If there are still playable units or enemy units...
        bool sideExterminated = false;
        if (PlayableUnits == 0 || EnemiesUnits == 0) {
            sideExterminated = true;
        }
        return sideExterminated;
    }

    private Teams GetRoundWinner() {
        // If the playable units are depleted, it is a player defeat
        if (PlayableUnits == 0 && EnemiesUnits > 0) {
            if (m_PlayerTeam == Teams.Allies) {
                return Teams.Axis;
            } else {
                return Teams.Allies;
            }
        }
        // If the enemy units are depleted, it is a player victory
        if (PlayableUnits > 0 && EnemiesUnits == 0) {
            if (m_PlayerTeam == Teams.Allies) {
                return Teams.Allies;
            } else {
                return Teams.Axis;
            }
        }
        // If both player units and enemy units are depleted, it is a draw
        return Teams.NeutralAI;
    }

    private Teams GetGameWinner() {
        if (WinsAllies == m_NumRoundsToWin) {
            return Teams.Allies;
        }
        if (WinsAxis == m_NumRoundsToWin) {
            return Teams.Axis;
        }

        // If no tanks have enough rounds to win, return null.
        return Teams.NeutralAI;
    }


    // Returns a string message to display at the end of each round.
    private string EndMessage() {
        // By default when a round ends there are no winners so the default end message is a draw.
        string message = "DRAW!";

        // If there is a round winner...
        if (RoundWinner != null) {
            if (RoundWinner == Teams.Allies) {
                message = "Allies won the round.";
            } else {
                message = "Axis won the round.";
            }
        }
        // If there is a game winner...
        if (GameWinner != Teams.NeutralAI) {
            if (GameWinner == Teams.Allies) {
                message = "Allies won the game !";
            } else {
                message = "Axis won the game !";
            }
        }

        // Add some line breaks after the initial message.
        message += "\n\n\n\n";

        // Display Teams scores :
        // Does not display correct score for unsolved reasons (TODO)
        // message += "Allies victories : " + WinsAllies + " victories.\n";
        // message += "Axis victories : " + WinsAllies + " victories.\n";
    

        return message;
    }

    private string GameMessage() {
        string message;

        if (PlayableUnits >  1) {
            message = "Player units : " + PlayableUnits +"\n";
        } else {
            message = "Player unit : " + PlayableUnits +"\n";
        }

        if (m_PlayerTeam == Teams.Allies) {
            message += "Wins : "+ WinsAllies +"/"+ m_NumRoundsToWin +"\n";
        } else {
            message += "Wins : "+ WinsAxis +"/"+ m_NumRoundsToWin +"\n";
        }

        if (PlayableUnits >  1) {
            message += "Enemy units : " + EnemiesUnits +"\n";
        } else {
            message += "Enemy unit : " + EnemiesUnits +"\n";
        }

        if (m_PlayerTeam == Teams.Allies) {
            message += "Wins : "+ WinsAxis +"/"+ m_NumRoundsToWin +"\n";
        } else {
            message += "Wins : "+ WinsAllies +"/"+ m_NumRoundsToWin +"\n";
        }

        return message;
    }

    // This function is used to turn all the units back on and reset their positions and properties.
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

    public Teams GetPlayer(){
        return m_PlayerTeam;
    }

    public void SetUnitDeath(int Unit, string Tag) {
        // Set the needed units to attain win conditions
        // Debug.Log("number :"+ Unit);
        // Debug.Log("Tag :"+ Tag);

        if (m_PlayerTeam == Teams.Allies) {
            if (Tag == Teams.Allies.ToString("g")) {
                PlayableUnits += Unit;
            } else if (Tag == Teams.Axis.ToString("g")) {
                EnemiesUnits += Unit;
            } else if (Tag == Teams.AxisAI.ToString("g")) {
                EnemiesUnits += Unit;
            }
        } else if (m_PlayerTeam == Teams.Axis) {
            if (Tag == Teams.Axis.ToString("g")) {
                PlayableUnits += Unit;
            } else if (Tag == Teams.Allies.ToString("g")) {
                EnemiesUnits += Unit;
            } else if (Tag == Teams.AlliesAI.ToString("g")) {
                EnemiesUnits += Unit;
            }
        }
        PlayerManager.SetScoreMessage(GameMessage());
    }

    public void EndGame() {
        string sceneName = "MainMenuScene3d";
        // Application.LoadLevel(sceneName);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}