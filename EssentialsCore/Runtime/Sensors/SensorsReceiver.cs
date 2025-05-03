using System;
using UnityEngine;
using UnityEngine.Events;

namespace Essentials.Core.Sensors
{
    /// <summary>
    /// Component attached to objects that can be detected by Sensors.
    /// Handles callbacks when detected or when detection is lost.
    /// </summary>
    public class SensorsReceiver : MonoBehaviour
    {
        /// <summary>
        /// Defines the action to take when the sensor state changes.
        /// </summary>
        public enum CallbackType
        {
            /// <summary>
            /// Automatically disable the object's Renderer when not detected.
            /// </summary>
            DisableRenderer,
            /// <summary>
            /// Invoke custom UnityEvents for detection and loss.
            /// </summary>
            Custom
        }

        [Tooltip("Callback type that will be used when sensors detect this object. 'Disable Renderer' will disable the renderer when no sensors are detected, and 'Custom' will call a custom callback.")]
        public CallbackType callbackType;

        [Tooltip("Callback that will be called when sensors detect this object.")]
        public UnityEvent onSensorsReceived;

        [Tooltip("Callback that will be called when sensors stop detecting this object.")]
        public UnityEvent onSensorsLost;

        [Tooltip("How often (in seconds) should the receiver check if the object is still detected?")]
        public float checkInterval = 1f;

        [Tooltip("An ID that is used to identify which sensors are detecting this object. Use this if you want to have multiple sensors that detect different objects. If you want to have multiple sensors that detect the same object, leave this at 0.")]
        public int sensorsId;

        [Tooltip("Is the object currently being detected?")]
        [SerializeField] private bool _isDetected;

        /// <summary>
        /// Gets whether the object is currently being detected by a sensor with a matching ID.
        /// </summary>
        public bool isDetected { get => _isDetected; private set => _isDetected = value; }

        private float timer;

        /// <summary>
        /// Called each frame. Updates the detection interval timer if the object is currently detected.
        /// </summary>
        private void Update()
        {
            if (isDetected) UpdateInterval();
        }

        /// <summary>
        /// Called by a Sensor when it detects this object.
        /// Sets the object as detected, resets the timer, and triggers the appropriate callback based on callbackType.
        /// </summary>
        public void SendCallback()
        {
            isDetected = true;
            timer = 0;

            switch (callbackType)
            {
                case CallbackType.DisableRenderer:
                    GetComponent<Renderer>().enabled = true;
                    break;
                case CallbackType.Custom:
                    onSensorsReceived.Invoke();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Updates the timer that tracks how long it has been since the last detection signal.
        /// If the interval exceeds checkInterval, it calls OnSensorsLost.
        /// </summary>
        private void UpdateInterval()
        {
            timer += Time.deltaTime;
            if (timer < checkInterval) return;

            timer = 0;
            OnSensorsLost();
        }

        /// <summary>
        /// Called when the checkInterval expires without receiving a new detection signal.
        /// Sets the object as not detected and triggers the appropriate callback based on callbackType.
        /// </summary>
        private void OnSensorsLost()
        {
            isDetected = false;

            switch (callbackType)
            {
                case CallbackType.DisableRenderer:
                    GetComponent<Renderer>().enabled = false;
                    break;
                case CallbackType.Custom:
                    onSensorsLost.Invoke();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}