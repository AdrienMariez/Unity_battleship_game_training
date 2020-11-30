using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class UnitMapUIManager : MonoBehaviour {

    private Camera MapCam;
    private UnitsUIManager UnitsUIManager;
    GameObject PlayerMapOrdersCanvas;
    private GameObject Unit;
    private bool UnitCurrentlyPlayed = false;
    private GameObject EnemyTargetUnit;
    // private bool UnitCurrentlyTargeted = false;
    // private bool ActionPaused = false;
    // private bool ShortActionPaused = false;
    private string DistanceString;

    private GameObject UIName;
        private Text UINameText;
    private Text UIDistance;
    private GameObject UIHealth;
        private Slider HealthBar;
        private Image HealthBarColor;

    private CompiledTypes.Teams Team; public void SetUnitTeam(CompiledTypes.Teams team){ Team = team; }
    private float MaximumHealth;
    private float CurrentHealth;
    const string RangeDisplayMeter = "{0} m";
    const string RangeDisplayKilometer = "{0} km";

    private PlayerSideVisibleAttackMapOrder UnitAttackOrder;
    public class PlayerSideVisibleAttackMapOrder {               // Attack order displayed on the map for each unit for the player
        private GameObject _orderModel; public GameObject GetOrderModel(){ return _orderModel; } public void SetOrderModel(GameObject _g){ _orderModel = _g; }
        private GameObject _targetObj; public GameObject GetTargetObj(){ return _targetObj; } public void SetTargetObj(GameObject _g){ _targetObj = _g; }
    }
    private List<PlayerSideVisibleMoveMapOrders> UnitMoveOrderList = new List<PlayerSideVisibleMoveMapOrders>();
    public class PlayerSideVisibleMoveMapOrders {               // Move/waypoint order displayed on the map for each unit for the player
        private int _moveOrderSortOrder; public int GetMoveOrderSortOrder(){ return _moveOrderSortOrder; } public void SetMoveOrderSortOrder(int _i){ _moveOrderSortOrder = _i; }
        private GameObject _orderModel; public GameObject GetOrderModel(){ return _orderModel; } public void SetOrderModel(GameObject _g){ _orderModel = _g; }
        private GameObject _startingPointObj; public GameObject GetStartingPointObj(){ return _startingPointObj; } public void SetStartingPointObj(GameObject _g){ _startingPointObj = _g; }
        private Vector3 _startingPoint; public Vector3 GetStartingPoint(){ return _startingPoint; } public void SetStartingPoint(Vector3 _v){ _startingPoint = _v; }
        private GameObject _endingPointObj; public GameObject GetEndingPointObj(){ return _endingPointObj; } public void SetEndingPointObj(GameObject _g){ _endingPointObj = _g; }
        private Vector3 _endingPoint; public Vector3 GetEndingPoint(){ return _endingPoint; } public void SetEndingPoint(Vector3 _v){ _endingPoint = _v; }
    }


    // IEnumerator PauseAction(){
    //     // Coroutine created to prevent too much calculus for ship behaviour
    //     ActionPaused = true;
    //     yield return new WaitForSeconds(0.5f);
    //     ActionPaused = false;
    // }
    // IEnumerator PauseActionShort(){
    //     // Coroutine created to prevent too much calculus for ship behaviour
    //     ShortActionPaused = true;
    //     yield return new WaitForSeconds(0.1f);
    //     ShortActionPaused = false;
    // }
    public void InitializeUIModule(Camera cam, GameObject unit, UnitsUIManager unitsUIManager, GameObject playerMapOrdersCanvas) {
        MapCam = cam;
        PlayerMapOrdersCanvas = playerMapOrdersCanvas;
        Unit = unit;
        
        UIName = this.transform.Find("Name").gameObject;
            UINameText = UIName.GetComponent<Text>();
            UIName.SetActive(false);
        UIDistance = this.transform.Find("Distance").gameObject.transform.GetComponent<Text>();

        UIHealth = this.transform.Find("Health").gameObject;
        HealthBar = UIHealth.transform.GetComponent<Slider>();
        HealthBarColor = UIHealth.transform.Find("FillArea").Find("Fill").GetComponent<Image>();
        
        if (unit.GetComponent<UnitHealth>()) {
            MaximumHealth = unit.GetComponent<UnitHealth>().GetStartingHealth();
            CurrentHealth = unit.GetComponent<UnitHealth>().GetCurrentHealth();
        }
        HealthBar.maxValue = MaximumHealth;
        HealthBar.value = CurrentHealth;
        UnitsUIManager = unitsUIManager;
        StartCoroutine(CheckDestructionCondition());
    }

    protected void FixedUpdate() {
        // Debug.Log (this.gameObject.name+" calculus : " + Vector3.Dot(MapCam.transform.forward, heading));
        // if (Unit.transform.position.y <= 0) {
            
        // }
        Vector3 _unitScreenPos = MapCam.WorldToScreenPoint(Unit.transform.position);
        Vector3 updatedPos = new Vector2(_unitScreenPos.x, (_unitScreenPos.y + 30f));
        this.transform.position  = updatedPos;

        // Update distance text
        if (!UnitCurrentlyPlayed) {
            float distance = (Unit.transform.position - MapCam.transform.position).magnitude;
            if (distance > 999) {
                distance = (Mathf.Round(distance / 100)) / 10f;
                DistanceString = string.Format(RangeDisplayKilometer, distance);
            } else {
                DistanceString = string.Format(RangeDisplayMeter, Mathf.Round(distance));
            }
            UIDistance.text = DistanceString;
        } else {
            UIDistance.text = "Played unit";
        }

        if (UnitAttackOrder != null) {
            // float _scaleX = Mathf.Abs(UnitAttackOrder.GetTargetObj().transform.position.x - Unit.transform.position.x);
            // float _scaleZ = Mathf.Abs(UnitAttackOrder.GetTargetObj().transform.position.z - Unit.transform.position.z);
            // UnitAttackOrder.GetOrderModel().transform.localScale = new Vector3(_scaleX, 1, _scaleZ);
            // UnitAttackOrder.GetOrderModel().transform.LookAt(UnitAttackOrder.GetTargetObj().transform);
            // if (UnitAttackOrder.GetOrderModel().transform.position.y < 50) {
            //     UnitAttackOrder.GetOrderModel().transform.position = new Vector3(UnitAttackOrder.GetOrderModel().transform.position.x, 50, UnitAttackOrder.GetOrderModel().transform.position.z);
            // }
            Vector3 _orderScreenPos = new Vector2(_unitScreenPos.x, _unitScreenPos.y);
            Vector3 _targetScreenPos = MapCam.WorldToScreenPoint(UnitAttackOrder.GetTargetObj().transform.position);
            Vector3 _ordertargetScreenPos = new Vector2(_targetScreenPos.x, _targetScreenPos.y);

            UnitAttackOrder.GetOrderModel().transform.position = _orderScreenPos;

            Vector3 diff = _orderScreenPos - _targetScreenPos;
                diff.Normalize();
            float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            UnitAttackOrder.GetOrderModel().transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);                              // Rotate towards target

            float distance = Vector3.Distance(_orderScreenPos, _ordertargetScreenPos);                                              // Get UI distance
            UnitAttackOrder.GetOrderModel().transform.localScale = new Vector3(1, distance*0.01f, 1);                               // Scale Y, divide by 100 (why 100 ?)

        }
    }

    IEnumerator CheckDestructionCondition(){
        while (true) {
            yield return new WaitForSeconds(2f);
            if (Unit == null) {
                Destroy();
            }
        }
    }
    public void SetPlayerUnit(bool isActive){
        UnitCurrentlyPlayed = isActive;

        if (UnitCurrentlyPlayed) {
            UIName.SetActive(true);
        } else {
            UIName.SetActive(false);
        }
        // Debug.Log (" UnitCurrentlyPlayed : " + UnitCurrentlyPlayed);
    }

    public void SetHighlightedUnit(bool isHighlighted){
        if (isHighlighted) {
            UINameText.color = new Color(1, 0.92f, 0.016f, 1);
            UIDistance.color = new Color(1, 0.92f, 0.016f, 1);
            UIName.SetActive(true);
        } else {
            UINameText.color = WorldUnitsManager.SetColor(Team);
            UIDistance.color = WorldUnitsManager.SetColor(Team);
            if (UnitCurrentlyPlayed) {
                UIName.SetActive(true);
            } else {
                UIName.SetActive(false);
            }
        }
        // Debug.Log (" UnitCurrentlyPlayed : " + UnitCurrentlyPlayed);
    }

    public void SetEnemyTargetUnit(GameObject targetUnit){
        EnemyTargetUnit = targetUnit;
        // if (EnemyTargetUnit == Unit) {
        //     UnitCurrentlyTargeted = true;
        // } else {
        //     UnitCurrentlyTargeted = false;
        // }
        // Debug.Log (" UnitCurrentlyPlayed : " + UnitCurrentlyPlayed);
    }
    public void SetEnemyTargetUnitOrder(GameObject targetUnit){
        if (UnitAttackOrder != null) {
            Destroy(UnitAttackOrder.GetOrderModel());
        }

        UnitAttackOrder = new PlayerSideVisibleAttackMapOrder{};
            UnitAttackOrder.SetOrderModel(BuildOrderModel(OrderType.Attack, Unit.transform));
            UnitAttackOrder.SetTargetObj(targetUnit);
    }
    public enum OrderType { Attack, Move, WayPoint } 
    public GameObject BuildOrderModel(OrderType orderType, Transform unitTransform) {
        // GameObject _tempOrderObject = Instantiate(WorldGlobals.GetMapOrderModel(), unitTransform);
        // _tempOrderObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = SetOrderColor(orderType);
        GameObject _tempOrderObject = Instantiate(WorldUIVariables.GetMapOrderModel(), this.transform);
        // _tempOrderObject.transform.GetComponent<SpriteRenderer>().color = SetOrderColor(orderType);
        _tempOrderObject.transform.GetChild(0).GetComponent<Image>().color = SetOrderColor(orderType);
        return _tempOrderObject;
    }
    public static Color SetOrderColor(OrderType orderType) {
        switch (orderType){
            case OrderType.Attack:
                return new Color(1, 0, 0, 1f);
            case OrderType.Move:
                return new Color(0, 0, 1, 1f);
            case OrderType.WayPoint:
                return new Color(1, 1, 0, 1f);
            default:
                return new Color(1, 1, 1, 1f);
        }
    }

    public void SetCurrentHealth(float HP, Color barColor) {
        HealthBar.value = HP;
        HealthBarColor.color = barColor;
    }

    public void ChangeName(string name) {
        UINameText.text = name;
    }

    public void SetDead() {
        UIName.transform.GetComponent<Text>().color = Color.grey;
        UIDistance.color = Color.grey;
        // UnitsUIManager.RemoveMapUIElement(this.gameObject);
        StartCoroutine(WaitForDestroy());
        // Destroy (this.gameObject);
    }
    IEnumerator WaitForDestroy(){
        yield return new WaitForSeconds(5f);
        Destroy();
    }
    public void Destroy() { Destroy (this.gameObject); }
}