// This file is subject to the MIT License as seen in the root of this folder structure (LICENSE)

// Shout out to @holdingjason who posted a first version of this script here: https://github.com/huwb/crest-oceanrender/pull/100

using System;
using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;
using System.Collections.Generic;

namespace Crest
{
    /// <summary>
    /// Boat physics by sampling at multiple probe points.
    /// </summary>
    public class ShipBuoyancy : FloatingObjectBase {

        private bool Dead;

        [Header("Forces")]
        [Tooltip("Override RB center of mass, in local space."), SerializeField]
        public Vector3 _centerOfMass = Vector3.zero;
        [SerializeField, FormerlySerializedAs("ForcePoints")]
        public FloaterForcePoints[] _forcePoints = new FloaterForcePoints[] { };
        [Tooltip("Scale of the buoyancy of the unit. The larger this value, the harder will buoyancy limits be. The lowest this value is, the more difficult it will be to keep the unit afloat.")]
        private float _buoyancyMultiplier;      // Now guessed from the rigidbody mass
        [Tooltip("Width dimension of the unit. The larger this value, the more filtered/smooth the wave response will be."), SerializeField]
        public float _objectWidth = 12f;

        [Header("Drag")]
        [Tooltip("Drag of the ship. The higher the value, the lest the ship will up and down~~~~"), SerializeField, Range(0, 4)]
        public float _dragVertical = 1f;

        [Header("Control")]
        [Tooltip("Max speed of the ship in nautical knots."), SerializeField, FormerlySerializedAs("EnginePower")]
        public float _enginePower = 20;
        [Tooltip("How long does a ship takes to make a full 180° rotation ? The lowest the value, the faster the ship will turn."), SerializeField, FormerlySerializedAs("TurnPower")]
        public float _rotationTime = 12.5f;
        private float _rotationTimeConverted = 0.5f;     // Converted _rotationTime for game mechanic
        /*
            0.5 = 12.5 s
            0.05 = 130 s
        */

        // [Tooltip("How does the ship behaves when turning ? The higher the value, the more the ship will tip when turning (values too high can induce weird behaviour)"), SerializeField, Range(0, 1)]
        // public float _centrifugalForce = 0.35f;

        // [Header("Debug Controls")]
        // [SerializeField]
        // public bool _playerControlled = true;
        // [Tooltip("Used to automatically add throttle input"), SerializeField]
        // public float _engineBias = 0f;
        // [Tooltip("Used to automatically add turning input"), SerializeField]
        // public float _turnBias = 0f;


        private const float WATER_DENSITY = 1000;

        public override Vector3 Velocity => _rb.velocity;

        Rigidbody _rb;

        Vector3 _displacementToObject = Vector3.zero;
        public override Vector3 CalculateDisplacementToObject() { return _displacementToObject; }

        public override float ObjectWidth { get { return _objectWidth; } }
        public override bool InWater { get { return true; } }

        SamplingData _samplingData = new SamplingData();
        SamplingData _samplingDataFlow = new SamplingData();

        Rect _localSamplingAABB;
        float _totalWeight;

        Vector3[] _queryPoints;
        Vector3[] _queryResultDisps;
        Vector3[] _queryResultVels;

        SampleFlowHelper _sampleFlowHelper = new SampleFlowHelper();
        private float SpeedInput;
        private float RotationInput;

        private float SinkingFactor;
        private float RandomSinkingRatio;
        private float RandomSinkingRatioPoints;
        private float SinkingX;
        private float SinkingZ;


        /*private void Awake() {
            _rb = GetComponent<Rigidbody>();
            _rb.centerOfMass = _centerOfMass;

            _buoyancyMultiplier = _rb.mass / 10000;

            if (OceanRenderer.Instance == null) {
                enabled = false;
                return;
            }

            _localSamplingAABB = ComputeLocalSamplingAABB();

            CalcTotalWeight();

            _queryPoints = new Vector3[_forcePoints.Length + 1];
            _queryResultDisps = new Vector3[_forcePoints.Length + 1];
            _queryResultVels = new Vector3[_forcePoints.Length + 1];

            SinkingFactor = 0f;
            RandomSinkingRatio = UnityEngine.Random.Range(0.1f, 0.001f);
            RandomSinkingRatioPoints = UnityEngine.Random.Range(0.001f, 0.00001f);

            _rotationTimeConverted = (1/_rotationTime)*6.25f;
        }*/

        void CalcTotalWeight() {
            _totalWeight = 0f;
            foreach (var pt in _forcePoints) {
                _totalWeight += pt._weight;
            }
        }

        public void BeginOperations(WorldSingleUnit unit) {
            _rb = GetComponent<Rigidbody>();
            _buoyancyMultiplier = _rb.mass / 10000;

            _centerOfMass = unit.GetBuoyancyUnitCenterOfMass();
            _rb.centerOfMass = _centerOfMass;

            List<FloaterForcePoints> floaterForcePointsList = new List<FloaterForcePoints>();
            // Debug.Log (unit.GetBuoyancyForcePointsList().Count+" / "+_forcePoints.Length);
            foreach (Vector3 point in unit.GetBuoyancyForcePointsList()) {
                FloaterForcePoints _floaterForcePoints = new FloaterForcePoints { };
                _floaterForcePoints._offsetPosition = point;
                floaterForcePointsList.Add(_floaterForcePoints);
            }
            _forcePoints = floaterForcePointsList.ToArray();

            _objectWidth = unit.GetBuoyancyModelWidth();
            _dragVertical = unit.GetBuoyancyVerticalDrag();
            _enginePower = unit.GetBuoyancyEnginePower();
            _rotationTime = unit.GetBuoyancyRotationTime();








            if (OceanRenderer.Instance == null) {
                enabled = false;
                return;
            }

            _localSamplingAABB = ComputeLocalSamplingAABB();

            CalcTotalWeight();

            _queryPoints = new Vector3[_forcePoints.Length + 1];
            _queryResultDisps = new Vector3[_forcePoints.Length + 1];
            _queryResultVels = new Vector3[_forcePoints.Length + 1];

            SinkingFactor = 0f;
            RandomSinkingRatio = UnityEngine.Random.Range(0.1f, 0.001f);
            RandomSinkingRatioPoints = UnityEngine.Random.Range(0.001f, 0.00001f);

            _rotationTimeConverted = (1/_rotationTime)*6.25f;
        }

        private void FixedUpdate() {
            #if UNITY_EDITOR
                // Sum weights every frame when running in editor in case weights are edited in the inspector.
                CalcTotalWeight();
            #endif

            // If the ship is sinking, sink it by a factor determined by _buoyancyMultiplier
            if (Dead && SinkingFactor > 0 && _buoyancyMultiplier > 0) {
                Sink(SinkingFactor, SinkingX, SinkingZ);
            }
            // Also, stop engines if the ship is dead.
            if (Dead) {
                SpeedInput = 0f;
            }

            // Trigger processing of displacement textures that have come back this frame. This will be processed
            // anyway in Update(), but FixedUpdate() is earlier so make sure it's up to date now.
            if (OceanRenderer.Instance._simSettingsAnimatedWaves.CollisionSource == SimSettingsAnimatedWaves.CollisionSources.OceanDisplacementTexturesGPU && GPUReadbackDisps.Instance) {
                GPUReadbackDisps.Instance.ProcessRequests();
            }

            var collProvider = OceanRenderer.Instance.CollisionProvider;
            var thisRect = GetWorldAABB();
            if (!collProvider.GetSamplingData(ref thisRect, _objectWidth, _samplingData)) {
                // No collision coverage for the sample area, in this case use the null provider.
                collProvider = CollProviderNull.Instance;
            }

            // Do queries
            UpdateWaterQueries(collProvider);

            _displacementToObject = _queryResultDisps[_forcePoints.Length];
            var undispPos = transform.position - _queryResultDisps[_forcePoints.Length];
            undispPos.y = OceanRenderer.Instance.SeaLevel;

            var waterSurfaceVel = _queryResultVels[_forcePoints.Length];

            if(QueryFlow.Instance) {
                _sampleFlowHelper.Init(transform.position, _objectWidth);
                Vector2 surfaceFlow = Vector2.zero;
                _sampleFlowHelper.Sample(ref surfaceFlow);
                waterSurfaceVel += new Vector3(surfaceFlow.x, 0, surfaceFlow.y);
            }

            // Buoyancy
            FixedUpdateBuoyancy(collProvider);
            FixedUpdateDrag(collProvider, waterSurfaceVel);
            FixedUpdateEngine();

            collProvider.ReturnSamplingData(_samplingData);
        }

        void UpdateWaterQueries(ICollProvider collProvider){
            // Update query points
            for (int i = 0; i < _forcePoints.Length; i++) {
                _queryPoints[i] = transform.TransformPoint(_forcePoints[i]._offsetPosition + new Vector3(0, _centerOfMass.y, 0));
            }
            _queryPoints[_forcePoints.Length] = transform.position;

            collProvider.Query(GetHashCode(), _samplingData, _queryPoints, _queryResultDisps, null, _queryResultVels);
        }

        void FixedUpdateEngine() {
            var forcePosition = _rb.position;

            // if (!_playerControlled) {
            //     var forward = _engineBias;
            //     var sideways = _turnBias;
            //     _rb.AddForceAtPosition(transform.forward * _enginePower * forward, forcePosition, ForceMode.Acceleration);
            //     var rotVec = transform.up + _rotationTimeConverted * transform.forward;
            //     _rb.AddTorque(rotVec * _rotationTimeConverted * RotationInput, ForceMode.Acceleration);
            // }else{
                // Convert _enginePower to game speed (m/s)
                // m/s = * 0.2f
                // Km/h = *0.055f
                // Nautical knots = *0.103f
                _rb.AddForceAtPosition(transform.forward * (_enginePower *0.103f) * SpeedInput, forcePosition, ForceMode.Acceleration);
                // var rotVec = transform.up + _centrifugalForce * transform.forward;
                var rotVec = transform.up + 1 * transform.forward;
                _rb.AddTorque(rotVec * _rotationTimeConverted * RotationInput, ForceMode.Acceleration);
            // }
        }

        void FixedUpdateBuoyancy(ICollProvider collProvider) {
            var archimedesForceMagnitude = WATER_DENSITY * Mathf.Abs(Physics.gravity.y);

            for (int i = 0; i < _forcePoints.Length; i++) {
                var waterHeight = OceanRenderer.Instance.SeaLevel + _queryResultDisps[i].y;
                var heightDiff = waterHeight - _queryPoints[i].y;
                if (heightDiff > 0) {
                    // _rb.AddForceAtPosition(archimedesForceMagnitude * heightDiff * Vector3.up * _forcePoints[i]._weight * _buoyancyMultiplier / _totalWeight, _queryPoints[i]);
                    _rb.AddForceAtPosition(archimedesForceMagnitude * heightDiff * Vector3.up * _forcePoints[i]._weight * _buoyancyMultiplier, _queryPoints[i]);
                }
            }
        }

        void FixedUpdateDrag(ICollProvider collProvider, Vector3 waterSurfaceVel)
        {
            // Apply drag relative to water
            var _velocityRelativeToWater = _rb.velocity - waterSurfaceVel;

            var forcePosition = _rb.position + Vector3.up;
            _rb.AddForceAtPosition(Vector3.up * Vector3.Dot(Vector3.up, -_velocityRelativeToWater) * _dragVertical, forcePosition, ForceMode.Acceleration);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(transform.TransformPoint(_centerOfMass), Vector3.one * 0.25f);

            for (int i = 0; i < _forcePoints.Length; i++)
            {
                var point = _forcePoints[i];

                var transformedPoint = transform.TransformPoint(point._offsetPosition + new Vector3(0, _centerOfMass.y, 0));

                Gizmos.color = Color.red;
                Gizmos.DrawCube(transformedPoint, Vector3.one * 0.5f);
            }

            var worldAABB = GetWorldAABB();
            new Bounds(new Vector3(worldAABB.center.x, 0f, worldAABB.center.y), Vector3.right * worldAABB.width + Vector3.forward * worldAABB.height).DebugDraw();
        }

        Rect ComputeLocalSamplingAABB() {
            if (_forcePoints.Length == 0) return new Rect();

            float xmin = _forcePoints[0]._offsetPosition.x;
            float zmin = _forcePoints[0]._offsetPosition.z;
            float xmax = xmin, zmax = zmin;
            for (int i = 1; i < _forcePoints.Length; i++)
            {
                float x = _forcePoints[i]._offsetPosition.x, z = _forcePoints[i]._offsetPosition.z;
                xmin = Mathf.Min(xmin, x); xmax = Mathf.Max(xmax, x);
                zmin = Mathf.Min(zmin, z); zmax = Mathf.Max(zmax, z);
            }

            return Rect.MinMaxRect(xmin, zmin, xmax, zmax);
        }

        Rect GetWorldAABB() {
            Bounds b = new Bounds(transform.position, Vector3.one);
            b.Encapsulate(transform.TransformPoint(new Vector3(_localSamplingAABB.xMin, 0f, _localSamplingAABB.yMin)));
            b.Encapsulate(transform.TransformPoint(new Vector3(_localSamplingAABB.xMin, 0f, _localSamplingAABB.yMax)));
            b.Encapsulate(transform.TransformPoint(new Vector3(_localSamplingAABB.xMax, 0f, _localSamplingAABB.yMin)));
            b.Encapsulate(transform.TransformPoint(new Vector3(_localSamplingAABB.xMax, 0f, _localSamplingAABB.yMax)));
            return Rect.MinMaxRect(b.min.x, b.min.z, b.max.x, b.max.z);
        }

        private void OnDisable() {
            if (QueryDisplacements.Instance)
            {
                QueryDisplacements.Instance.RemoveQueryPoints(GetHashCode());
            }
        }

        public void Sink(float sinking, float x, float z) {
            // Debug.Log("sinkingfactor = "+ sinking +" - x = "+ x +" - z = "+ z);
            _buoyancyMultiplier -= RandomSinkingRatio * sinking;
            for (int i = 0; i < _forcePoints.Length; i++) {
                float localX = _forcePoints[i]._offsetPosition.x, localZ = _forcePoints[i]._offsetPosition.z;
                if (Math.Sign(x) == Math.Sign(localX) || Math.Sign(z) == Math.Sign(localZ)) {
                    if (_forcePoints[i]._weight > 0)
                        _forcePoints[i]._weight -= RandomSinkingRatioPoints * SinkingFactor;
                }
            }

            if (SinkingFactor == 0)
                SinkingFactor = sinking;
            if (SinkingX == 0)
                SinkingX = x;
            if (SinkingZ == 0)
                SinkingZ = z;
        }

        public void SetSpeedInput(float Speed){ SpeedInput = Speed; }
        public void SetRotationInput(float Rotation){ RotationInput = Rotation; }
        public void SetDead(bool death) { Dead = death; }
        // public float GetRealSpeed(){
        //     float Speed = _enginePower * SpeedInput;
        //     return Speed;
        // }
    }


    [Serializable]
    public class FloaterForcePoints {
        // [FormerlySerializedAs("_factor")]
        [HideInInspector]public float _weight = 1f;

        public Vector3 _offsetPosition;
    }
    
}
