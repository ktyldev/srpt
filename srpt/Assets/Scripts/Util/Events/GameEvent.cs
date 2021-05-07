using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Ktyl.Util
{
    [CreateAssetMenu(menuName = "ktyl/Events/Game Event")]
    public class GameEvent : ScriptableObject
    {
        [SerializeField] private bool _logRaised;

        protected readonly List<GameEventListener> _listeners = new List<GameEventListener>();

        public virtual void Raise()
        {
            if (_logRaised)
            {
                Debug.Log($"raised {this}", this);
            }

            for (int i = 0; i < _listeners.Count; i++)
            {
                _listeners[i].OnEventRaised();
            }
        }

        public void Register(GameEventListener listener)
        {
            if (_listeners.Contains(listener))
            {
                Debug.LogError($"{listener} already registered", this);
                return;
            }
            
            _listeners.Add(listener);
        }

        public void Unregister(GameEventListener listener)
        {
            if (!_listeners.Contains(listener))
            {
                Debug.LogError($"{listener} not already registered");
                return;
            }

            _listeners.Remove(listener);
        }
    }
    
    #region Editor
    #if UNITY_EDITOR

    [CustomEditor(typeof(GameEvent), true)]
    public class GameEventEditor : Editor
    {
        private GameEvent _event;

        private void OnEnable()
        {
            _event = target as GameEvent;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Raise"))
            {
                _event.Raise();
            }
        }
    }
    
    #endif  
    #endregion
}