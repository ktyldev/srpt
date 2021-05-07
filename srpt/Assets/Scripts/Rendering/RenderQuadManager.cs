using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class RenderQuadManager : MonoBehaviour
{
    [Serializable]
    public enum RenderMode
    {
        Raytracing = 0,
        Depth,
        REST_frame,
    }

    public RenderMode SelectedRenderMode
    {
        get => _renderMode;
        set
        {
            if (value == _renderMode) return;

            _renderMode = value;
            switch (_renderMode)
            {
                case RenderMode.Raytracing:
                    _raytracing.SetActive(true);
                    _depth.SetActive(false);
                    _restFrame.SetActive(false);
                    break;
                
                case RenderMode.Depth:
                    _depth.SetActive(true);
                    _raytracing.SetActive(false);
                    _restFrame.SetActive(false);
                    break;
                
                case RenderMode.REST_frame:
                    _restFrame.SetActive(true);
                    _raytracing.SetActive(false);
                    _depth.SetActive(false);
                    break;
            }
        }
    }
    private RenderMode _renderMode = default;

    [SerializeField] private GameObject _depth;
    [SerializeField] private GameObject _raytracing;
    [SerializeField] private GameObject _restFrame;
}

#region Editor
#if UNITY_EDITOR

[CustomEditor(typeof(RenderQuadManager))]
public class RenderQuadManagerEditor : Editor
{
    private RenderQuadManager _data;

    private void OnEnable()
    {
        _data = target as RenderQuadManager;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var oldMode = _data.SelectedRenderMode;
        var mode = (RenderQuadManager.RenderMode)EditorGUILayout.EnumPopup(
            "Render Mode", 
            oldMode);

        if (mode != oldMode)
        {
            _data.SelectedRenderMode = mode;
        }
    }
}

#endif
#endregion
