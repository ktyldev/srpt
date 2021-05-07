using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Ktyl.Util;
using TMPro;
using UnityEngine;

namespace Ktyl.T2.Rendering
{
    // https://catlikecoding.com/unity/tutorials/frames-per-second/
    [RequireComponent(typeof(TMP_Text))]
    public class FPSCounter : MonoBehaviour
    {
        private const int BUFFER_SIZE = 60;
        private const int MAX_FPS = 99;

        [Header("Data")] [SerializeField] private SerialInt _averageFps;
        [SerializeField] private SerialInt _rawFps;

        private TMP_Text _text;

        private struct FrameData
        {
            public int fps;
            public float dt;
        }

        private int _raw;
        private FrameData[] _buffer;
        private int _bufferIdx;

        private string[] _numberStrings;

        private readonly StringBuilder _sb = new StringBuilder();

        private void OnEnable()
        {
            _text = GetComponent<TMP_Text>();

            _numberStrings = new string[MAX_FPS];
            for (int i = 0; i < MAX_FPS; i++)
            {
                _numberStrings[i] = i.ToString().PadLeft(3, '0');
            }
        }

        private void Update()
        {
            if (_buffer == null || _buffer.Length != BUFFER_SIZE)
            {
                InitializeBuffer();
            }

            UpdateBuffer();
            CalculateFPS();

            var fps = Mathf.Clamp(_averageFps.Value, 0, MAX_FPS - 1);

            _sb.Clear();
            _sb.AppendLine(_numberStrings[fps]);

            // average frame time
            var dt = 0f;
            for (int i = 0; i < BUFFER_SIZE; i++)
            {
                dt += _buffer[i].dt;
            }
            dt /= BUFFER_SIZE;
            
            _sb.AppendLine($"{dt * 1000f:0.00}ms");

            _text.text = _sb.ToString();
        }

        private void UpdateBuffer()
        {
            var dt = Time.unscaledDeltaTime;
            _rawFps.Value = (int) (1f / dt);

            _buffer[_bufferIdx++] = new FrameData
            {
                fps = _rawFps, 
                dt = dt
            };
            
            if (_bufferIdx >= BUFFER_SIZE)
            {
                _bufferIdx = 0;
            }
        }

        private void CalculateFPS()
        {
            var sum = 0;
            
            for (int i = 0; i < BUFFER_SIZE; i++)
            {
                sum += _buffer[i].fps;
            }

            _averageFps.Value = sum / BUFFER_SIZE;
        }

        private void InitializeBuffer()
        {
            _buffer = new FrameData[BUFFER_SIZE];
            _bufferIdx = 0;
        }
    }
}