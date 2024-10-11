using System;
using UnityEngine;
using UnityEngine.Events;

public class EssentialsSensorsReceiver : MonoBehaviour
{
    public enum CallbackType
    {
        DisableRenderer,
        Custom
    }

    [Tooltip("Callback type that will be used when sensors detect this object. 'Disable Renderer' will disable the renderer, and 'Custom' will call the custom callback.")]
    public CallbackType callbackType;

    public UnityEvent onSensorsRecieved;
    public UnityEvent onSensorsLost;

    public float checkInterval = 1f;

    [Tooltip("An ID that is used to identify which sensors are detecting this object. Use this if you want to have multiple sensors that detect different objects. If you want to have multiple sensors that detect the same object, leave this at 0.")]
    public int sensorsId;
    [Tooltip("Is the object currently detected by sensors?")]
    public bool isDetected;

    private float timer;

    private void Update()
    {
        if (isDetected) UpdateInterval();
    }

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
                onSensorsRecieved.Invoke();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void UpdateInterval()
    {
        timer += Time.deltaTime;
        if (timer < checkInterval) return;

        timer = 0;
        OnSensorsLost();
    }

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