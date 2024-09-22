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
        Vertical
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
    public float sensorsAngle = 90;
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
        private ScanMethod scanMethod;
        private SensorsOrientation sensorsOrientation;
        
        private float3 startPosition;
        private float3 forwardDirection;
        private float3 upDirection;
        private float3 rightDirection;
        
        private float sensorsAngle;
        private float sensorsRange;
        
        private float scanAngle;
        
        private RandomScanType randomScanType;
        
        private float verticalRandomization;
        
        private NativeArray<JobSensorData> sensorDatas;
        private NativeArray<RaycastCommand> raycastCommands;

        private uint seed;
        
        public void Execute(int index)
        {
            Random random = Random.CreateFromIndex(seed + (uint)index);
            
            float startAngle = sensorsAngle * 0.5f;
            float stepAngle = sensorsAngle / (sensorDatas.Length - 1);

            float3 direction;

            if (sensorsOrientation == SensorsOrientation.Horizontal)
                direction = Quaternion.AngleAxis(startAngle - stepAngle * index, upDirection) * forwardDirection;
            else
                direction = Quaternion.AngleAxis(startAngle - stepAngle * index, rightDirection) * forwardDirection;

            switch (scanMethod)
            {
                case ScanMethod.Static:
                    // the angle is already calculated
                    break;
                case ScanMethod.Vertical:
                    direction = Quaternion.AngleAxis(scanAngle, rightDirection) * direction;
                    break;
                case ScanMethod.Horizontal:
                    direction = Quaternion.AngleAxis(scanAngle, upDirection) * direction;
                    break;
                case ScanMethod.Random:
                {
                    if (randomScanType is RandomScanType.Horizontal or RandomScanType.Both)
                    {
                        float angle = random.NextFloat() * sensorsAngle - sensorsAngle * 0.5f;
                        direction = Quaternion.AngleAxis(angle, upDirection) * forwardDirection;
                    }

                    if (randomScanType is RandomScanType.Vertical or RandomScanType.Both)
                    {
                        float verticalAngle = random.NextFloat() * verticalRandomization - verticalRandomization * 0.5f;
                        direction = Quaternion.AngleAxis(verticalAngle, rightDirection) * direction;
                    }
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            sensorDatas[index] = new JobSensorData
            {
                startPosition = startPosition,
                direction = direction,
            };

            raycastCommands[index] = new RaycastCommand(startPosition, direction, QueryParameters.Default, sensorsRange);
        }
        
        public CalculateDirectionsJob(ScanMethod scanMethod, SensorsOrientation sensorsOrientation, Vector3 startPosition, Vector3 forwardDirection, Vector3 upDirection, Vector3 rightDirection, float sensorsAngle, float sensorsRange, float scanAngle, RandomScanType randomScanType, float verticalRandomization, NativeArray<JobSensorData> sensorDatas, NativeArray<RaycastCommand> raycastCommands, uint seed)
        {
            this.scanMethod = scanMethod;
            this.sensorsOrientation = sensorsOrientation;
            this.startPosition = startPosition;
            this.forwardDirection = forwardDirection;
            this.upDirection = upDirection;
            this.rightDirection = rightDirection;
            this.sensorsAngle = sensorsAngle;
            this.sensorsRange = sensorsRange;
            this.scanAngle = scanAngle;
            this.randomScanType = randomScanType;
            this.verticalRandomization = verticalRandomization;
            this.sensorDatas = sensorDatas;
            this.raycastCommands = raycastCommands;
            this.seed = seed;
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
        CalculateDirectionsJob calculateDirectionsJob = new CalculateDirectionsJob(scanMethod, sensorsOrientation, transform.position, transform.forward, transform.up, transform.right, sensorsAngle, sensorsRange, _scanAngle, randomScanType, verticalRandomization, jobSensorDatas, raycastCommands, (uint)random.Next(0, int.MaxValue));
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