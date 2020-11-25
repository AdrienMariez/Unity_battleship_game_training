using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class UnitUIManager : MonoBehaviour {

    private Camera Cam;
    private UnitsUIManager UnitsUIManager;
    private UnitsUIManager.PlayerSideUnitsUI PlayerSideUnitUI;
    private GameObject UnitModel;
    private BoxCollider BoxCollider;
    private bool BoundsFound = false;
    private bool UnitCurrentlyPlayed = false;
    private GameObject EnemyTargetUnit;
    private bool UnitCurrentlyTargeted = false;
    // private bool ActionPaused = false;
    
    private string DistanceString;

    private GameObject UIName;
        private Text UINameText;
    private GameObject UIDistance;
    private GameObject UIHealth;
        private Slider HealthBar;
        private Image HealthBarColor;
    private GameObject UIPointer;
    private GameObject UIBoundingBox;
    private GameObject UIBoundingBoxTL;
    private GameObject UIBoundingBoxTR;
    private GameObject UIBoundingBoxBL;
    private GameObject UIBoundingBoxBR;

    private float MaximumHealth;
    private float CurrentHealth;
    const string RangeDisplayMeter = "{0} m";
    const string RangeDisplayKilometer = "{0} km";

    public void InitializeUIModule(Camera cam, GameObject unit, UnitsUIManager.PlayerSideUnitsUI playerSideUnitUI, UnitsUIManager unitsUIManager) {
        // Debug.Log ("InitializeUIModule");
        Cam = cam;
        PlayerSideUnitUI = playerSideUnitUI;
        UnitModel = PlayerSideUnitUI.GetUnitModel();
        if (PlayerSideUnitUI.GetUnitModel().transform.Find("Bounding").transform.Find("BoundingBox")) {
            BoxCollider =  PlayerSideUnitUI.GetUnitModel().transform.Find("Bounding").transform.Find("BoundingBox").GetComponent<BoxCollider>();
            BoundsFound = true;
        }

        UIName = this.transform.Find("Name").gameObject;
            UINameText = UIName.GetComponent<Text>();
        UIDistance = this.transform.Find("Distance").gameObject;
        UIPointer = this.transform.Find("Pointer").gameObject;

        UIBoundingBox = this.transform.Find("BoundingBox").gameObject;
        UIBoundingBoxTL = UIBoundingBox.transform.Find("BoundingBoxTopLeft").gameObject;
        UIBoundingBoxTR = UIBoundingBox.transform.Find("BoundingBoxTopRight").gameObject;
        UIBoundingBoxBL = UIBoundingBox.transform.Find("BoundingBoxBottomLeft").gameObject;
        UIBoundingBoxBR = UIBoundingBox.transform.Find("BoundingBoxBottomRight").gameObject;

        // Health
            UIHealth = this.transform.Find("Health").gameObject;
            HealthBar = UIHealth.transform.GetComponent<Slider>();
            HealthBarColor = UIHealth.transform.Find("FillArea").Find("Fill").GetComponent<Image>();
            MaximumHealth = PlayerSideUnitUI.GetUnitController().GetUnitHealth().GetStartingHealth();
            CurrentHealth = PlayerSideUnitUI.GetUnitController().GetUnitHealth().GetCurrentHealth();
                // Debug.Log ("InitializeUIModule"+MaximumHealth);
            HealthBar.maxValue = MaximumHealth;
            HealthBar.value = CurrentHealth;

        UnitsUIManager = unitsUIManager;
        StartCoroutine(PauseActionName());
        StartCoroutine(CheckDestructionCondition());
    }
    // private bool NameActionPaused = false;
    IEnumerator PauseActionName(){
        // Coroutine created to prevent too much calculus for ship behaviour
        // NameActionPaused = true;
        yield return new WaitForSeconds(0.1f);
        UINameText.text = PlayerSideUnitUI.GetUnitController().GetUnitName();
        // NameActionPaused = false;
    }

    protected void FixedUpdate() {
        // Debug.Log ("Unit : "+ Unit+" - Cam : "+ Cam);
        
        Vector3 heading = UnitModel.transform.position - Cam.transform.position;

        if (Vector3.Dot(Cam.transform.forward, heading) > 0 && !UnitCurrentlyPlayed) {
            

            float size = 1000*(1/Vector3.Distance(Cam.transform.position, UnitModel.transform.position));
            size = Mathf.Min(size, 1);
            size = Mathf.Max(size, 0.2f);
            // var position = Cam.WorldToScreenPoint (enemies[e].position);
            // GUI.DrawTexture(Rect((screenPos.x - (size/2)), (screenPos.y - (size/2)), size, targetSize), texture);

            // Update position of UI if it is supposed to be visible
            // Debug.Log (this.gameObject.name+" calculus : " + Vector3.Dot(Cam.transform.forward, heading));
            Vector3 screenPos = Cam.WorldToScreenPoint(UnitModel.transform.position);
            Vector3 updatedPos = new Vector2(screenPos.x, (screenPos.y + (30*size)));
            this.transform.position  = updatedPos;

            // Debug.Log (this.gameObject.name+"  - screenPos : " + screenPos);
            if (UnitCurrentlyTargeted) {
                UIName.SetActive(true);
                UIDistance.SetActive(true);
                UIHealth.SetActive(true);
                UIPointer.SetActive(false);
                UIBoundingBox.SetActive(true);
                UpdateDistanceText();
                CreateBoundBox();
            } else if (screenPos.x >= (0.45f*Screen.width) && screenPos.x <= (0.55f*Screen.width) && screenPos.y >= (0.35f*Screen.height) && screenPos.y <= (0.75f*Screen.height)){
                // Debug.Log (this.gameObject.name+"  - screenPos x : " + screenPos.x+"  - screenPos  y: " + screenPos.y);
                UIName.SetActive(false);
                UIDistance.SetActive(true);
                UIHealth.SetActive(true);
                UIPointer.SetActive(false);
                UIBoundingBox.SetActive(false);
                CreateBoundBox();

                // Update distance text
                if (!UnitCurrentlyPlayed) {
                    UpdateDistanceText();
                } else {
                    UINameText.text = "Played unit";
                }
            } else {
                UIName.SetActive(false);
                UIDistance.SetActive(false);
                UIHealth.SetActive(false);
                UIPointer.SetActive(true);
                UIPointer.transform.localScale = new Vector2(size, size);
                UIBoundingBox.SetActive(false);
            }
        } else {
            // put the UI away if it is not visible
            this.transform.position  = new Vector2(0.0f, 3000f);
        }
    }

    IEnumerator CheckDestructionCondition(){
        while (true) {
            yield return new WaitForSeconds(2f);
            if (UnitModel == null) {
                Destroy();
            }
        }
    }

    private void UpdateDistanceText(){
        if (!DistanceActionPaused) {
            float distance = (UnitModel.transform.position - Cam.transform.position).magnitude;
            if (distance > 999) {
                distance = (Mathf.Round(distance / 100)) / 10f;
                DistanceString = string.Format(RangeDisplayKilometer, distance);
            } else {
                DistanceString = string.Format(RangeDisplayMeter, Mathf.Round(distance));
            }
            UIDistance.GetComponent<Text>().text = DistanceString;
        }
    }
    private bool DistanceActionPaused = false;
    IEnumerator PauseActionDistance(){
        // Coroutine created to prevent too much calculus
        DistanceActionPaused = true;
        yield return new WaitForSeconds(0.1f);
        DistanceActionPaused = false;
    }
    private void CreateBoundBox(){
        if (BoundsFound) {
            // Inspired by : https://answers.unity.com/questions/292031/how-to-display-a-rectangle-around-a-player.html
            Bounds b = BoxCollider.bounds;
            Vector3[] pts = new Vector3[8];

            // All 8 vertices of the bounds
            pts[0] = Cam.WorldToScreenPoint (new Vector3 (b.center.x + b.extents.x, b.center.y + b.extents.y, b.center.z + b.extents.z));
            pts[1] = Cam.WorldToScreenPoint (new Vector3 (b.center.x + b.extents.x, b.center.y + b.extents.y, b.center.z - b.extents.z));
            pts[2] = Cam.WorldToScreenPoint (new Vector3 (b.center.x + b.extents.x, b.center.y - b.extents.y, b.center.z + b.extents.z));
            pts[3] = Cam.WorldToScreenPoint (new Vector3 (b.center.x + b.extents.x, b.center.y - b.extents.y, b.center.z - b.extents.z));
            pts[4] = Cam.WorldToScreenPoint (new Vector3 (b.center.x - b.extents.x, b.center.y + b.extents.y, b.center.z + b.extents.z));
            pts[5] = Cam.WorldToScreenPoint (new Vector3 (b.center.x - b.extents.x, b.center.y + b.extents.y, b.center.z - b.extents.z));
            pts[6] = Cam.WorldToScreenPoint (new Vector3 (b.center.x - b.extents.x, b.center.y - b.extents.y, b.center.z + b.extents.z));
            pts[7] = Cam.WorldToScreenPoint (new Vector3 (b.center.x - b.extents.x, b.center.y - b.extents.y, b.center.z - b.extents.z));
            
            //Calculate the min and max positions
            Vector3 min = pts[0];
            Vector3 max = pts[0];
            for (int i=1;i<pts.Length;i++) {
                min = Vector3.Min (min, pts[i]);
                max = Vector3.Max (max, pts[i]);
            }
            // Apply some margin
            float margin = 15;
            min.x -= margin;
            max.x += margin;
            min.y -= margin;
            max.y += margin;

            // Place the bounding elements according to positions
            UIBoundingBoxTL.transform.position = new Vector2(max.x, min.y);
            UIBoundingBoxTR.transform.position = new Vector2(max.x, max.y);
            UIBoundingBoxBL.transform.position = new Vector2(min.x, min.y);
            UIBoundingBoxBR.transform.position = new Vector2(min.x, max.y);
        }
    }

    public void SetPlayerUnit(bool isActive){
        if (isActive) {
            UnitCurrentlyPlayed = true;
        } else {
            UnitCurrentlyPlayed = false;
        }
        // Debug.Log (" UnitCurrentlyPlayed : " + UnitCurrentlyPlayed);
    }

    public void SetEnemyTargetUnit(GameObject targetUnit){
        EnemyTargetUnit = targetUnit;
        if (EnemyTargetUnit == UnitModel) {
            UnitCurrentlyTargeted = true;
            // UIName.GetComponent<Text>().text = Unit.name;
        } else {
            UnitCurrentlyTargeted = false;
        }
        // Debug.Log (" UnitCurrentlyPlayed : " + UnitCurrentlyPlayed);
    }

    public void SetCurrentHealth(float HP, Color barColor) {
        // Debug.Log (" HP : " + HP +" / "+ MaximumHealth);
        HealthBar.value = HP;
        HealthBarColor.color = barColor;
    }

    public void ChangeName(string name) {
        UINameText.text = name;
    }

    public void SetDead() {
        UIName.GetComponent<Text>().color = Color.grey;
        UIDistance.GetComponent<Text>().color = Color.grey;
        UIPointer.GetComponent<Image>().color = Color.grey;
        UIBoundingBoxTL.GetComponent<Image>().color = Color.grey;
        UIBoundingBoxTR.GetComponent<Image>().color = Color.grey;
        UIBoundingBoxBL.GetComponent<Image>().color = Color.grey;
        UIBoundingBoxBR.GetComponent<Image>().color = Color.grey;

        // UnitsUIManager.RemoveUIElement(this.gameObject);
        StartCoroutine(WaitForDestroy());
    }
    IEnumerator WaitForDestroy(){
        yield return new WaitForSeconds(5f);
        // Destroy();
        Destroy (this.gameObject);
    }
    public void Destroy() {
        // Debug.Log ("Destroy");
        UnitsUIManager.RemoveUIForSingleUnit(PlayerSideUnitUI);
        Destroy (this.gameObject);
    }
}