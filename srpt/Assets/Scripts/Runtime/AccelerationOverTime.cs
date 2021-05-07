using System;
using System.Collections;
using System.Collections.Generic;
using Ktyl.Util;
using Unity.Mathematics;
using UnityEngine;

public partial class AccelerationOverTime : MonoBehaviour
{
    [SerializeField] private SerialDouble _velocity;
    [SerializeField] private float _accelerationRate;

    private float _elapsed = 0;

    private void Awake()
    {
        _elapsed = 0;
    }

    private void OnEnable()
    {
        
        _elapsed = math.log((float)(1.0f-_velocity.Value)) / -_accelerationRate;
    }

    private void Update()
    {
        _elapsed += Time.deltaTime;
        
        _velocity.Value = 1.0f - math.pow(math.E, -_elapsed * _accelerationRate);
    }
}

