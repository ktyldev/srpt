using UnityEngine;

namespace Ktyl.Util
{
    #region Editor

#if UNITY_EDITOR
    using UnityEditor;

    public abstract class SerialVarEditor<T> : Editor
    {
        private SerialVar<T> _t;

        protected virtual void OnEnable()
        {
            _t = target as SerialVar<T>;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.LabelField("Current Value", _t.Value.ToString());
            EditorGUI.EndDisabledGroup();
        }
    }
#endif

    public partial class SerialVar<T>
    {
        public T EDITOR_InitialValue
        {
            get => _initialValue;
            set => _initialValue = value;
        }
    }
    
    #endregion

    public abstract partial class SerialVar<T> : ScriptableObject
    {
        public T Value
        {
            get => Application.isPlaying ? _value : _initialValue;
            set
            {
                if (_readOnly)
                {
                    Debug.LogError("tried to write to read only variable", this);
                    return;
                }
                
                _value = value;   
            }
        }
        private T _value;

        [SerializeField] private T _initialValue;
        [SerializeField] private bool _readOnly;
        
        public static implicit operator T(SerialVar<T> t) => t.Value;

        private void OnValidate()
        {
            _value = _initialValue;
        }

        private void OnEnable()
        {
            _value = _initialValue;
        }

        public override string ToString() => Value.ToString();
    }
}