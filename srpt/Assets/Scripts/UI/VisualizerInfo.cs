using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Ktyl.Rendering;
using Ktyl.Util;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(TMP_Text))]
public class VisualizerInfo : MonoBehaviour
{
    [FormerlySerializedAs("_settings")] [SerializeField] private SpecialRelativitySettings _srSettings;
    [SerializeField] private SerialBool _show;

    private TMP_Text _text;

    private readonly StringBuilder _sb = new StringBuilder();

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }

    private void LateUpdate()
    {
        /*
        v/c=0.67
        lorentz=1.54

        [1] doppler=on
        [2] searchlight=on
        [3] wavelengths=visible/scaled
        */
        
        _sb.Clear();

        if (_show.Value)
        {
            _sb.AppendLine($"v/c={_srSettings.B:0.0000}");
            _sb.AppendLine($"lorentz={_srSettings.Lorentz:0.0000}");
            _sb.AppendLine();

            _sb.AppendLine($"[1] doppler={(_srSettings.DopplerWeight > 0.5f ? "on" : "off")}");
            _sb.AppendLine($"[2] searchlight={(_srSettings.SearchlightWeight > 0.5f ? "on" : "off")}");
            _sb.AppendLine($"[3] wavelength scaling={(_srSettings.WavelengthScaling > 0.5f ? "on" : "off")}");
        }

        _text.text = _sb.ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
