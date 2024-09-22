using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class EssentialsSensors : MonoBehaviour
{
    public enum ScanMethod
    {
        Static,
        Vertical,
        Horizontal,
        Random
    }

    public enum SensorsOrientation
    {
        Horizontal,
        Vertical,
        Both
    }

    public enum RandomScanType
    {
        Horizontal,
        Vertical,
        Both
    }

    public ScanMethod scanMethod = ScanMethod.Static;
    public SensorsOrientation sensorsOrientation = SensorsOrientation.Horizontal;

    [SerializeField] private int _sensorsCount = 10;
    [SerializeField] private int _sensorsRows = 3;
    public float sensorsAngle = 90;
    public float sensorsHorizontalAngle = 90;
    public float sensorsVerticalAngle = 90;
    public float sensorsRange = 10;

    public float scanAngleAmplitude = 90;
    public float scanAngleFrequency = 10;

    public RandomScanType randomScanType = RandomScanType.Horizontal;
    public float verticalRandomization = 0.1f;

    [Tooltip("An ID that is used to identify which objects are detected by this sensor. Use this if you want to have multiple sensors that detect different objects. If you want to have multiple sensors that detect the same object, leave this at 0.")]
    public int sensorsId;

    public bool showSensors;
    public bool showSensorHits;

    private float _scanAngle;

    private struct JobSensorData
    {
        public Vector3 startPosition;
        public Vector3 direction;
    }

    private struct SensorData
    {
        public Vector3 startPosition;
        public Vector3 direction;
        public bool hit;
        public Vector3 hitPosition;
        public EssentialsSensorsReciever reciever;
    }

    [BurstCompile]
    private struct CalculateDirectionsJob : IJobParallelFor
    {
        private ScanMethod _scanMethod;
        private SensorsOrientation _sensorsOrientation;

        private float3 _startPosition;
        private float3 _forwardDirection;
        private float3 _upDirection;
        private float3 _rightDirection;

        private int _sensorsRows;

        private float _sensorsAngle;
        private float _sensorsHorizontalAngle;
        private float _sensorsVerticalAngle;

        private float _sensorsRange;

        private float _scanAngle;

        private RandomScanType _randomScanType;

        private float _verticalRandomization;

        private NativeArray<JobSensorData> _sensorDatas;
        private NativeArray<RaycastCommand> _raycastCommands;

        private uint _seed;

        public void Execute(int index)
        {
            Random random = Random.CreateFromIndex(_seed + (uint)index);

            float startAngle = _sensorsAngle * 0.5f;
            float stepAngle = _sensorsAngle / (_sensorDatas.Length - 1);

            int row = index / _sensorsRows;
            int rowIndex = index % _sensorsRows;

            float3 direction;

            switch (_sensorsOrientation)
            {
                case SensorsOrientation.Horizontal:
                    direction = Quaternion.AngleAxis(startAngle - stepAngle * index, _upDirection) * _forwardDirection;
                    break;
                case SensorsOrientation.Vertical:
                    direction = Quaternion.AngleAxis(startAngle - stepAngle * index, _rightDirection) * _forwardDirection;
                    break;
                case SensorsOrientation.Both:
                    float startVerticalAngle = _sensorsVerticalAngle * 0.5f;
                    float stepVerticalAngle = _sensorsVerticalAngle / (_sensorDatas.Length / _sensorsRows - 1);

                    float startHorizontalAngle = _sensorsHorizontalAngle * 0.5f;
                    float stepHorizontalAngle = _sensorsHorizontalAngle / (_sensorsRows - 1);

                    Quaternion horizontalRotation = Quaternion.AngleAxis(startHorizontalAngle - stepHorizontalAngle * rowIndex, _rightDirection);
                    Quaternion verticalRotation = Quaternion.AngleAxis(startVerticalAngle - stepVerticalAngle * row, _upDirection);

                    direction = verticalRotation * horizontalRotation * _forwardDirection;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (_scanMethod)
            {
                case ScanMethod.Static:
                    // the angle is already calculated
                    break;
                case ScanMethod.Vertical:
                    direction = Quaternion.AngleAxis(_scanAngle, _rightDirection) * direction;
                    break;
                case ScanMethod.Horizontal:
                    direction = Quaternion.AngleAxis(_scanAngle, _upDirection) * direction;
                    break;
                case ScanMethod.Random:
                    {
                        if (_randomScanType is RandomScanType.Horizontal or RandomScanType.Both)
                        {
                            float angle = random.NextFloat() * _sensorsAngle - _sensorsAngle * 0.5f;
                            direction = Quaternion.AngleAxis(angle, _upDirection) * _forwardDirection;
                        }

                        if (_randomScanType is RandomScanType.Vertical or RandomScanType.Both)
                        {
                            float verticalAngle = random.NextFloat() * _verticalRandomization - _verticalRandomization * 0.5f;
                            direction = Quaternion.AngleAxis(verticalAngle, _rightDirection) * direction;
                        }
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _sensorDatas[index] = new JobSensorData
            {
                startPosition = _startPosition,
                direction = direction,
            };

            _raycastCommands[index] = new RaycastCommand(_startPosition, direction, QueryParameters.Default, _sensorsRange);
        }

        public CalculateDirectionsJob(ScanMethod scanMethod, SensorsOrientation sensorsOrientation, Vector3 startPosition, Vector3 forwardDirection, Vector3 upDirection, Vector3 rightDirection, int sensorsRows, float sensorsAngle, float sensorsHorizontalAngle, float sensorsVerticalAngle, float sensorsRange, float scanAngle, RandomScanType randomScanType, float verticalRandomization, NativeArray<JobSensorData> sensorDatas, NativeArray<RaycastCommand> raycastCommands, uint seed)
        {
            _scanMethod = scanMethod;
            _sensorsOrientation = sensorsOrientation;
            _startPosition = startPosition;
            _forwardDirection = forwardDirection;
            _upDirection = upDirection;
            _rightDirection = rightDirection;
            _sensorsRows = sensorsRows;
            _sensorsAngle = sensorsAngle;

            // needs to be swapped for some reason
            _sensorsHorizontalAngle = sensorsVerticalAngle;
            _sensorsVerticalAngle = sensorsHorizontalAngle;

            _sensorsRange = sensorsRange;
            _scanAngle = scanAngle;
            _randomScanType = randomScanType;
            _verticalRandomization = verticalRandomization;
            _sensorDatas = sensorDatas;
            _raycastCommands = raycastCommands;
            _seed = seed;
        }
    }

    private JobHandle calculateDirectionsJobHandle;
    private JobHandle calculateRaycastsJobHandle;

    private NativeArray<JobSensorData> jobSensorDatas;
    private NativeArray<RaycastHit> raycastHits;
    private NativeArray<RaycastCommand> raycastCommands;

    private SensorData[] sensorDatas;
    private System.Random random = new System.Random();

    private void Awake()
    {
        jobSensorDatas = new NativeArray<JobSensorData>(_sensorsCount, Allocator.Persistent);
        raycastHits = new NativeArray<RaycastHit>(_sensorsCount, Allocator.Persistent);
        raycastCommands = new NativeArray<RaycastCommand>(_sensorsCount, Allocator.Persistent);

        sensorDatas = new SensorData[_sensorsCount];
    }

    private void Update()
    {
        UpdateAngles();
        UpdateSensors();
    }

    private void LateUpdate()
    {
        UpdateLateSensors();
        SendCallbackToReciever();
    }

    private void UpdateSensors()
    {
        CalculateDirectionsJob calculateDirectionsJob = new CalculateDirectionsJob(scanMethod, sensorsOrientation, transform.position, transform.forward, transform.up, transform.right, _sensorsRows, sensorsAngle, sensorsHorizontalAngle, sensorsVerticalAngle, sensorsRange, _scanAngle, randomScanType, verticalRandomization, jobSensorDatas, raycastCommands, (uint)random.Next(0, int.MaxValue));
        calculateDirectionsJobHandle = calculateDirectionsJob.Schedule(_sensorsCount, 5);

        calculateRaycastsJobHandle = RaycastCommand.ScheduleBatch(raycastCommands, raycastHits, 5, calculateDirectionsJobHandle);
    }

    private void UpdateLateSensors()
    {
        calculateRaycastsJobHandle.Complete();

        for (int i = 0; i < jobSensorDatas.Length; i++)
        {
            sensorDatas[i] = new SensorData
            {
                startPosition = jobSensorDatas[i].startPosition,
                direction = jobSensorDatas[i].direction,
                hit = raycastHits[i].collider != null,
                hitPosition = raycastHits[i].point
            };
        }
    }

    private void UpdateAngles()
    {
        if (scanMethod != ScanMethod.Vertical && scanMethod != ScanMethod.Horizontal) return;
        _scanAngle = Mathf.Sin(Time.time * scanAngleFrequency) * scanAngleAmplitude;
    }

    private void SendCallbackToReciever()
    {
        foreach (RaycastHit hit in raycastHits)
        {
            if (hit.collider == null) continue;

            EssentialsSensorsReciever reciever = hit.collider.GetComponent<EssentialsSensorsReciever>();

            if (reciever == null) continue;
            if (reciever.sensorsId != sensorsId) continue;

            reciever.SendCallback();
        }
    }

    private void OnDestroy()
    {
        jobSensorDatas.Dispose();
        raycastHits.Dispose();
        raycastCommands.Dispose();
    }

    private void OnDrawGizmos()
    {
        if (!showSensors || sensorDatas == null) return;

        foreach (SensorData sensorData in sensorDatas)
        {
            if (sensorData.hit)
            {
                Gizmos.color = showSensorHits ? Color.red : Color.white;
                Gizmos.DrawLine(sensorData.startPosition, sensorData.hitPosition);
            }
            else
            {
                Gizmos.color = showSensorHits ? Color.green : Color.white;
                Gizmos.DrawLine(sensorData.startPosition, sensorData.startPosition + sensorData.direction * sensorsRange);
            }
        }
    }
}