using System;
using System.Collections;
using System.Collections.Generic;
using Ktyl.Util;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public partial class RaytracingSamples : MonoBehaviour
{
    [SerializeField] private SerialFloat _lastInteractionTime;
    [SerializeField] private SerialFloat _sampleWeight;
    [SerializeField] private SerialFloat _maximumSampleWeight;

    public void Interact()
    {
        _lastInteractionTime.Value = Time.time;
    }

    private void OnEnable()
    {
        Interact();
    }

    private void LateUpdate()
    {
        var timeSinceInteract = Time.time - _lastInteractionTime;
        _sampleWeight.Value = _maximumSampleWeight / (1.0f + timeSinceInteract * 2.0f);
    }

    private void OnDisable()
    {
        Interact();
    }
}
