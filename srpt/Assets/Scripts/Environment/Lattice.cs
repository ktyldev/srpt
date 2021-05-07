using System;
using System.Collections;
using System.Collections.Generic;
using Ktyl.Util;
using UnityEngine;

public class Lattice : MonoBehaviour
{
    [SerializeField] private SerialFloat4x4 _lattice2World;

    private Camera _cam;

    private void Start()
    {
        _cam = Camera.main;
    }

    private void LateUpdate()
    {
        _lattice2World.Value = transform.localToWorldMatrix;
    }
}
