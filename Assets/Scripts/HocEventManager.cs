using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor;
using HocInternal;

namespace HocInternal 
{

    /// <summary>
    /// Interface of a event 
    /// </summary>
    public interface IHocEvent
    {
        public void triggerHocEvent();
        public void eventDebugLog(int dispatcherUuid);
        public Type getInputParameterType();
    }

    
    public class HocEventBase  
    {
        private string _name;
        private Type _type = typeof(void);

        public string Name
        {
            get { return _name; }
        }

        public HocEventBase(string name = "Undefined")
        {
            _name = name;
        }

        public HocEventBase(Type type, string name = "Undefined")
        {
            _name = name;
            _type = type;
        }
        
        public virtual void eventDebugLog(int dispathcerUuid)
        {
            if(HocConfig.Instance.enableDebugLog && HocConfig.Instance.enableEventSystemDebug)
            {
                Debug.Log("Dispatcher with Id: " + dispathcerUuid + " triggered HocEvent: " + _name);
            }
        }
    }

    public class HocEvent<InputParameter_Type> : HocEventBase, IHocEvent
    {
        public Action<InputParameter_Type> m_Function;
        public InputParameter_Type m_Parameter;


        public HocEvent(Action<InputParameter_Type> _func, InputParameter_Type _parameter, string name = "Undefined") 
            : base(typeof(InputParameter_Type), name)
        {
            m_Function = _func;
            m_Parameter = _parameter;
        }
        
        public void triggerHocEvent()
        {
            m_Function(m_Parameter);
        }

        public Type getInputParameterType()
        {
            return typeof(InputParameter_Type);
        }

        public void triggerHocEvent(InputParameter_Type _parameter)
        {
            m_Function(_parameter);
        }

    };

    public class HocEvent : HocEventBase, IHocEvent
    {
        public Action m_Function;

        public HocEvent(Action _func, string name = "Undefined") : base(name)
        {
            m_Function = _func;
        }

        public void triggerHocEvent()
        {
            m_Function();
        }

        public Type getInputParameterType()
        {
            return typeof(void);
        }
    }
    
};
 


public class HocEventManager : MonoBehaviour
{
    private Dictionary<string, IHocEvent> m_HocEventList;

    private static HocEventManager _instance;
    public static HocEventManager Instance {  get { return _instance; } }

    public List<string> _registeredEventNames;

    public List<GameObject> _storyEventData;

    private void Awake()
    {
        m_HocEventList = new Dictionary<string, IHocEvent> ();
        _registeredEventNames = new List<string> ();
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
    }


    private void debugAdd(string _eventName)
    {
        if(HocConfig.Instance.enableEventSystemDebug)
        {
            _registeredEventNames.Add(_eventName);
        }
    }

    public void addHocEventListener<InputParameter_Type>(string _eventName, Action<InputParameter_Type> _func, InputParameter_Type _parameter)
    {
        if(m_HocEventList.ContainsKey(_eventName))
        {
            removeHocEvent(_eventName);
        }
        HocEvent<InputParameter_Type> newHocEvent = new HocEvent<InputParameter_Type>(_func, _parameter, _eventName);
        m_HocEventList.Add(_eventName, newHocEvent);

#if DEBUG
        debugAdd(_eventName);
#endif
    }


    public bool containEvent(string _eventName)
    {
        return m_HocEventList.ContainsKey(_eventName);
    }

    public void addHocEventListener(string _eventName, Action _func)
    {
        if(m_HocEventList.ContainsKey(_eventName))
        {
            removeHocEvent(_eventName);
        }
        HocEvent newHocEvent = new HocEvent(_func, _eventName);
        m_HocEventList.Add(_eventName, newHocEvent);

#if DEBUG
        debugAdd(_eventName);
#endif
    }

    public void addHocEventListener(HocEvent _event)
    {
        if(m_HocEventList.ContainsKey(_event.Name))
        {
            removeHocEvent(_event.Name);
        }

        m_HocEventList.Add(_event.Name, _event);

#if DEBUG
        debugAdd(_event.Name);
#endif
    }

    public void addHocEventListener<InputParameter_Type>(HocEvent<InputParameter_Type> _event)
    {
        if(m_HocEventList.ContainsKey(_event.Name))
        {
            removeHocEvent(_event.Name);
        }
        m_HocEventList.Add(_event.Name, _event);

#if DEBUG
        debugAdd(_event.Name);
#endif
    }

    public void dispatchHocEvent(string _eventName, int dispatcherUuid)
    {
        if(!m_HocEventList.ContainsKey(_eventName))
        {
            throw new Exception("failed to dispatch event with name: " + _eventName + " because it does not exist!");
        }

        var target = m_HocEventList[_eventName];
        target.eventDebugLog(dispatcherUuid);
        target.triggerHocEvent();
    }

    public void dispatchHocEvent<InputParameter_Type>(string _eventName, int dispatcherUuid, InputParameter_Type _parameter)
    {
        if(!m_HocEventList.ContainsKey(_eventName))
        {
            throw new Exception("failed to dispatch event with name: " + _eventName + " because it does not exist!");
        }


        var target = m_HocEventList[_eventName];
        if(target.getInputParameterType() != typeof(InputParameter_Type))
        {
            throw new Exception("failed to dispatch event with name: " + _eventName + " because parameter type does not match!");
        }
        else
        {
            target.eventDebugLog(dispatcherUuid);
            ((HocEvent<InputParameter_Type>)target).triggerHocEvent(_parameter);
        }
    }

    public void removeHocEvent(string _eventName)
    {
        if(!m_HocEventList.ContainsKey(_eventName))
        {
            throw new Exception("failed to remove event with name: " + _eventName + " because it does not exist!");
        }

        m_HocEventList.Remove(_eventName);

#if DEBUG
        if(HocConfig.Instance.enableEventSystemDebug)
        {
            _registeredEventNames.Remove(_eventName);
        }
#endif
    }

};

