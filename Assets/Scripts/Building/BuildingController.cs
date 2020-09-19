using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingController : UnitMasterController {
    [Tooltip("Components (game object with collider + Hitbox Component script)")]
    public GameObject[] m_BuildingComponents;
    public WorldUnitsManager.BuildingSubCategories m_BuildingCategory;

    private BuildingHealth Health;
    private BuildingAI BuildingAI;
    private BuildingUI UI;

    private float RepairRate = 1;

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

        for (int i = 0; i < m_BuildingComponents.Length; i++) {
            m_BuildingComponents[i].GetComponent<HitboxComponent>().SetUnitController(this);
        }
    }
    private void Start() {
        StartCoroutine(SpawnPauseLogic());
    }
    IEnumerator SpawnPauseLogic(){
        yield return new WaitForSeconds(0.3f);
        ResumeStart();
    }
    // private void ResumeStart() {
    //     if (GameManager != null) {
    //         GameManager.BuildingSpawned(this.gameObject, Team, m_BuildingCategory);
    //     } else if (GameObject.Find("GameManager") != null) {
    //         GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    //         GameManager.BuildingSpawned(this.gameObject, Team, m_BuildingCategory);
    //     }
    // }

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

    public void CallDeath() {
        // Debug.Log("DEATH"+Dead);
        Dead = true;

        if (GetComponent<TurretManager>())
            Turrets.SetDeath(true);
        
        UI.SetDead();
        if (GameManager)
            GameManager.UnitDead(this.gameObject, Team, Active);

        if (GetComponent<SpawnerScriptToAttach>())
            GetComponent<SpawnerScriptToAttach>().SetDeath(true);

        tag = "Untagged";
    }

    public void SetCurrentHealth(float health){ if (Active && !Dead) PlayerManager.SetCurrentUnitHealth(health); }
    
    public void SetCurrentTarget(GameObject targetUnit) {
        EnemyTargetUnit = targetUnit;
        if (Active) {
            PlayerManager.SendCurrentEnemyTarget(targetUnit);
        }
    }

    // ALL OVERRIDES METHODS
    public override void SetActive(bool activate) {
        Active = activate;
        if (GetComponent<TurretManager>())
                Turrets.SetActive(Active);
        // if (Active)
        //     Debug.Log("Unit : "+ gameObject.name  +" - Active = "+ Active);
        BuildingAI.SetAIActive(!Active);
        if (GetComponent<SpawnerScriptToAttach>())
            GetComponent<SpawnerScriptToAttach>().SetActive(Active);
    }
    public override void SetMap(bool map) {
        if (GetComponent<TurretManager>())
            Turrets.SetMap(map);
    }
    public override void SetPause(bool pause){
        if (GetComponent<TurretManager>())
            Turrets.SetPause();
    }
    public override void SetTag(WorldUnitsManager.Teams team){
        base.SetTag(team);
        Team = team;
        UI.SetUnitTeam(team);
        BuildingAI.SetUnitTeam(team.ToString("g"));

    }
    public override void SetName(string name){
        base.SetName(name);
        UI.SetName(name);
        BuildingAI.SetName(name);
    }
    public override float GetRepairRate(){ return RepairRate; }

    // Turrets
    public override void SetSingleTurretStatus(TurretManager.TurretStatusType status, int turretNumber){
        if (PlayerManager != null) { PlayerManager.SetSingleTurretStatus(status, turretNumber); }
    }
    public override void SendPlayerShellToUI(GameObject shellInstance){
        if (PlayerManager != null) { PlayerManager.SendPlayerShellToUI(shellInstance); }
    }

    // UI
    public override void SetUIElement(GameObject uiElement) { UI.SetUIElement(uiElement); }
    public override void SetUIMapElement(GameObject uiElement) { UI.SetUIMapElement(uiElement); }
    public override void KillAllUIInstances() { UI.KillAllUIInstances(); }
    public override float GetStartingHealth() { return(Health.GetStartingHealth()); }
    public override float GetCurrentHealth() { return(Health.GetCurrentHealth()); }
    public override bool GetDeath(){ return Dead; }

    // Damage control
    public override void ApplyDamage(float damage) {
        Health.ApplyDamage(damage);
        float currentHealth = Health.GetCurrentHealth();
        UI.SetCurrentHealth(currentHealth);
    }
    public override void ModuleDestroyed(ElementType elementType) {
        // Debug.Log("ElementType :"+ ElementType);
        // Status : 0 : fixed and running / 1 : damaged / 2 : dead
        if (elementType == ElementType.ammo) {
            Health.AmmoExplosion();
        } else if (elementType == ElementType.fuel) {
            Health.StartFire();
        }
    }

    public override void ModuleRepaired(ElementType elementType) {
        if (elementType == ElementType.fuel) {
            Health.EndFire();
        }
    }
    public override void DestroyUnit(){
        // Debug.Log ("Destroy unit : "+gameObject.name);
        if (GetComponent<TurretManager>())
            Turrets.SetDeath(true);
        UI.KillAllUIInstances();
        if (GameManager) {
            GameManager.UnitDead(this.gameObject, Team, Active);
        }
        base.DestroyUnit();
    }
}