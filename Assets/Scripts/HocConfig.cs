using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using HocInternal;

namespace HocInternal
{
    public class ReadOnlyAttribute : PropertyAttribute
    {

    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            string _value;
            switch(property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    _value = property.intValue.ToString();
                    break;
                case SerializedPropertyType.Boolean:
                    _value = property.boolValue.ToString();
                    break;
                case SerializedPropertyType.Float:
                    _value = property.floatValue.ToString();
                    break;
                case SerializedPropertyType.String:
                    _value = property.stringValue;
                    break;
                default:
                    _value = "[Not Supported]";
                    break;
            }
            EditorGUI.LabelField(position, label.text, _value);
        }
    }
#endif
}

/// <summary>
/// HocConfig Class holds necessary compile-time information
/// for programs to run
/// </summary>
public class HocConfig : MonoBehaviour 
{
    // Implement a Singleton-Pattern
    private static HocConfig _instance;
    public static HocConfig Instance { get { return _instance; } }

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
    }


    
    // HocConfiguration for Event System Debug
    public bool enableDebugLog;
#if DEBUG && ENABLE_EVENTSYSTEM_DEBUG
    [ReadOnly]
    public bool enableEventSystemDebug = true;
#else
    [ReadOnly]
    public readonly bool enableEventSystemDebug = false;
#endif

#if DEBUG && ENABLE_FSMMANAGER_DEBUG
    [ReadOnly]
    public bool enableFsmManagerDebug = true;
#else
    [ReadOnly]
    public bool enableFsmManagerDebug = false;
#endif

#if DEBUG && ENABLE_INPUTSYSTEM_DEBUG
    [ReadOnly]
    public bool enableInputSystemDebug = true;
#else
    public bool enableInputSystemDebug = false;
#endif
}
