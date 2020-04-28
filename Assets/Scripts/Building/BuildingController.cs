using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingController : MonoBehaviour {
    [Tooltip("Components (game object with collider + Hitbox Component script)")]
    private bool Active = false;
    private bool Dead = false;
    public WorldUnitsManager.BuildingSubCategories m_BuildingCategory;
    private WorldUnitsManager.Teams Team;

    private GameManager GameManager;
    private PlayerManager PlayerManager;

    private BuildingHealth Health;
    private BuildingAI BuildingAI;
    private BuildingUI UI;
    private TurretManager Turrets;

    private float RepairRate;

    private GameObject EnemyTargetUnit;

    private void Awake() {
        Health = GetComponent<BuildingHealth>();
        BuildingAI = GetComponent<BuildingAI>();
        float HP = Health.GetStartingHealth();

        UI = GetComponent<BuildingUI>();
        UI.SetStartingHealth(HP);
        UI.SetCurrentHealth(HP);

        if (GetComponent<TurretManager>())
            Turrets = GetComponent<TurretManager>();
            BuildingAI.SetTurretManager(Turrets);

        if (GetComponent<TurretManager>())
            Turrets.SetRepairRate(RepairRate);

    }
    private void Start() {
        StartCoroutine(SpawnPauseLogic());
    }
    IEnumerator SpawnPauseLogic(){
        yield return new WaitForSeconds(0.3f);
        ResumeStart();
    }
    private void ResumeStart() {
        if (GameManager != null) {
            GameManager.BuildingSpawned(this.gameObject, Team, m_BuildingCategory);
        } else if (GameObject.Find("GameManager") != null) {
            GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            GameManager.BuildingSpawned(this.gameObject, Team, m_BuildingCategory);
        }
    }

    // private bool ActionPaused = false;
    // private bool ActionPaused2 = false;
    private void FixedUpdate() {
		// Debug.Log("Active :"+ Active);

        // The following is used to kill an active ship for debug purposes
        // if (Active && !Dead) {
        //     if (Input.GetAxis ("VerticalShip") == 1){
        //         CallDeath();
        //     }
        // }
        // kills all inactive ships for debug purposes
        // if (!Active && !ActionPaused) {
        //     ActionPaused = !ActionPaused;
        //     StartCoroutine(PauseAction());
        // }
        // if (!Active && ActionPaused2 && !Dead) {
        //     CallDeath();
        // }
    }
    
    // IEnumerator PauseAction(){
    //     yield return new WaitForSeconds(3f);
    //     ActionPaused2= true;
    // }

    public void ApplyDamage(float damage) {
        Health.ApplyDamage(damage);
        float currentHealth = Health.GetCurrentHealth();
        UI.SetCurrentHealth(currentHealth);
    }

    public void CallDeath() {
        // Debug.Log("DEATH"+Dead);
        Dead = true;

        if (GetComponent<TurretManager>())
            Turrets.SetDeath(true);
        
        UI.SetDead();
        if (GameManager)
            GameManager.UnitDead(this.gameObject, Team, Active);
        // if (PlayerManager) {
        //     if (Active)
        //         PlayerManager.SetCurrentUnitDead(true);
            // PlayerManager.UnitDead(this.gameObject, Team);
        // }

        tag = "Untagged";
    }

    public void SetMap(bool map) {
        if (GetComponent<TurretManager>())
            Turrets.SetMap(map);
    }
    public void SetActive(bool activate) {
        Active = activate;
        if (GetComponent<TurretManager>())
                Turrets.SetActive(Active);
        // if (Active)
        //     Debug.Log("Unit : "+ gameObject.name  +" - Active = "+ Active);
        BuildingAI.SetAIActive(!Active);
    }
    
    public void SetTag(WorldUnitsManager.Teams team){
        Team = team;
        gameObject.tag = team.ToString("g");
        UI.SetUnitTeam(team);
        BuildingAI.SetUnitTeam(team.ToString("g"));

    }
    public void SetName(string name){
        gameObject.name = name;
        UI.SetName(name);
        BuildingAI.SetName(name);
    }

    public void SetGameManager(GameManager gameManager){ GameManager = gameManager; }
    public void SetPlayerManager(PlayerManager playerManager){
        PlayerManager = playerManager;
        // PlayerManager.UnitSpawned(this.gameObject, Team, m_UnitType);
        if (GetComponent<TurretManager>())
            Turrets.SetPlayerManager(PlayerManager);
    }
    public void SetSingleTurretStatus(TurretManager.TurretStatusType status, int turretNumber){
        if (PlayerManager != null) { PlayerManager.SetSingleTurretStatus(status, turretNumber); }
    }
    public void SendPlayerShellToUI(GameObject shellInstance){
        if (PlayerManager != null) { PlayerManager.SendPlayerShellToUI(shellInstance); }
    }

    public void SetCurrentHealth(float health){ if (Active && !Dead) PlayerManager.SetCurrentUnitHealth(health); }
    public void SetPause(bool pause){
        if (GetComponent<TurretManager>())
            Turrets.SetPause();
    }
    public void SetCurrentTarget(GameObject targetUnit) {
        EnemyTargetUnit = targetUnit;
        if (Active) {
            PlayerManager.SendCurrentEnemyTarget(targetUnit);
        }
    }
    public bool GetDeath(){ return Dead; }
    public float GetRepairRate(){ return RepairRate; }
    public void DestroyUnit(){
        // Debug.Log ("Destroy unit : "+gameObject.name);
        if (GetComponent<TurretManager>())
            Turrets.SetDeath(true);
        UI.KillAllUIInstances();
        if (GameManager) {
            // if (Active)
            //     PlayerManager.SetCurrentUnitDead(true);
            // PlayerManager.UnitDead(this.gameObject, Team, Active);
            GameManager.UnitDead(this.gameObject, Team, Active);
        }
        Destroy (gameObject);
    }
}