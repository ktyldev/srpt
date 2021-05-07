using System;
using System.Collections;
using System.Collections.Generic;
using Ktyl.Util;
using UnityEngine;

public class VisualizerInput : MonoBehaviour
{
    [Serializable]
    private struct ToggleOption
    {
        public KeyCode key;
        public SerialBool value;

        public void Update()
        {
            value.Value ^= Input.GetKeyDown(key);
        }
    }
    
    [SerializeField] private ToggleOption _doppler;
    [SerializeField] private ToggleOption _searchlight;
    [SerializeField] private ToggleOption _wavelengthsScaled;
    [SerializeField] private ToggleOption _show;
    
    private void Update()
    {
        _doppler.Update();
        _searchlight.Update();
        _wavelengthsScaled.Update();
        _show.Update();
    }
}
