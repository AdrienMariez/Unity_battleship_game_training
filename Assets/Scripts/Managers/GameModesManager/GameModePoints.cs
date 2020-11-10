using System.Collections;
using UnityEngine;

public class GameModePoints : GameModesManager {
    public UnitManager[] m_Units;               // Public unit list. There could be multiple of those, in theory

    [Header("Gameplay options : ")]
    public int m_TeamCommandPoints;
    public int m_RequiredVictoryPointsToWin;
    public float m_StartDelay = 3f;             // The delay between the start of RoundStarting and RoundPlaying phases.
    public float m_EndDelay = 3f;               // The delay between the end of RoundPlaying and RoundEnding phases.


    public override void Begin() {
        base.Begin();
        
        // Create the delays so they only have to be made once.
        StartWait = new WaitForSeconds (m_StartDelay);
        EndWait = new WaitForSeconds (m_EndDelay);

        // Set the real values in the parent class

        GameManager.SetMaxTeamCommandPoints(m_TeamCommandPoints);
        GameManager.SetRequiredVictoryPointsToWin(m_RequiredVictoryPointsToWin);
        Units = m_Units;

        // Once the units have been created and the camera is using them as targets, start the game.
        StartCoroutine (GameLoop ());
    }

    protected override bool ObectiveAccomplished() {
        // If a team met the required victory points
        bool obectiveAccomplishedForOneSide = false;
        if (GameManager.GetAlliesTeamCurrentVictoryPoints() >= GameManager.GetRequiredVictoryPointsToWin() || GameManager.GetAxisTeamCurrentVictoryPoints() >= GameManager.GetRequiredVictoryPointsToWin()) {
            obectiveAccomplishedForOneSide = true;
        }
        return obectiveAccomplishedForOneSide;
    }
    protected override CompiledTypes.Teams.RowValues GetRoundWinner() {
        // Returns the winner of a single round
        if (GameManager.GetAlliesTeamCurrentVictoryPoints() >= GameManager.GetRequiredVictoryPointsToWin() && GameManager.GetAxisTeamCurrentVictoryPoints() < GameManager.GetRequiredVictoryPointsToWin()) {
            return CompiledTypes.Teams.RowValues.Allies;
        } else if (GameManager.GetAxisTeamCurrentVictoryPoints() >= GameManager.GetRequiredVictoryPointsToWin() && GameManager.GetAlliesTeamCurrentVictoryPoints() < GameManager.GetRequiredVictoryPointsToWin()) {
            return CompiledTypes.Teams.RowValues.Axis;
        } else {
            // Any other case gives a draw
            return CompiledTypes.Teams.RowValues.Neutral;
        }
    }

    // Message system
    protected override string StartMessage() {
        string message;
        // Override here the message shown on screen at the beginning of a round / of gameplay
        message = "Beat the enemy in a prolonged fight !";
        return message;
    }
    /*protected override string GameMessage() {
        string message;
        // Displays basic message shown on screen during the duration of a round / of gameplay for all players.
        return message;
    }*/
    public override string GameMessageTeam(CompiledTypes.Teams team) {
        string message = "";
        // Displays personnalized for each team message shown on screen during the duration of a round / of gameplay
        if (team.id == WorldUnitsManager.GetDB().Teams.Allies.id) {
            message = "Command Points : " + GameManager.GetAlliesTeamCurrentCommandPoints() +" / "+ GameManager.GetMaxTeamCommandPoints() +"\n";
            message += "Victory Points : " + GameManager.GetAlliesTeamCurrentVictoryPoints() +" / "+ GameManager.GetRequiredVictoryPointsToWin() +"\n";
            message += "Enemy victory Points : " + GameManager.GetAxisTeamCurrentVictoryPoints() +" / "+ GameManager.GetRequiredVictoryPointsToWin() +"\n";
        } else if (team.id == WorldUnitsManager.GetDB().Teams.Axis.id) {
            message = "Command Points : " + GameManager.GetAxisTeamCurrentCommandPoints() +" / "+ GameManager.GetMaxTeamCommandPoints() +"\n";
            message += "Victory Points : " + GameManager.GetAxisTeamCurrentVictoryPoints() +" / "+ GameManager.GetRequiredVictoryPointsToWin() +"\n";
            message += "Enemy Victory Points : " + GameManager.GetAlliesTeamCurrentVictoryPoints() +" / "+ GameManager.GetRequiredVictoryPointsToWin() +"\n";
        }

        return message;
    }
    /*protected override string EndMessage() {
        string message;
        // Override here the message shown on screen at the end of a round / of gameplay
        return message;
    }*/

    public override void UpdateMessage() { 
        //Hi I am needed by GameManager, please do not mess with me.
        base.UpdateMessage();
        GameManager.SetScoreMessageTeam();
    }

    public override void UpdateGameplay() {
        base.UpdateGameplay();
        //Hi I am needed by GameManager, please do not mess with me.
        //Add specifics for gameplay when a unit dies/spawns here
    }
}