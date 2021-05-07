using System.Collections;
using System.Collections.Generic;
using Ktyl.Util;
using UnityEngine;

public class ShowControls : MonoBehaviour
{
    [SerializeField] private SerialBool _cursorLocked;
    [SerializeField] private GameObject _controlsPanel;
    
    void Update()
    {
        _controlsPanel.SetActive(!_cursorLocked);
    }
}
