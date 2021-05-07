using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace Ktyl.Util
{
    public class SliderDouble : MonoBehaviour
    {
        [SerializeField] private SerialDouble _value;
        [SerializeField] private TMP_Text _valueText;
        [SerializeField] private Slider _slider;

        [SerializeField] private double _min;
        [SerializeField] private double _max;

        private void OnEnable()
        {
            _slider.maxValue = (float) _max;
            _slider.minValue = (float) _min;

            _slider.onValueChanged.AddListener(SetValue);
        }

        public void SetValue(float v)
        {
            _value.Value = v;
            _valueText.text = v.ToString();
        }

        private void LateUpdate()
        {
            _valueText.text = _value.ToString();
            _slider.value = (float)_value.Value;
        }
    }
}