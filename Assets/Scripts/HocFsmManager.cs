using System;
using System.Collections.Generic;
using UnityEngine;
using HocInternal;


namespace HocInternal
{

    public abstract class HocFsmState<T>
    {
        public abstract Type type
        {
            get;
        }
        public virtual void awake(IHocFsm<T> fsm)
        {

        }

        public virtual void start(IHocFsm<T> fsm)
        {

        }

        public virtual void update(IHocFsm<T> fsm, float elapsedTime)
        {

        }

        public virtual void stop(IHocFsm<T> fsm)
        {

        }

        public virtual void destroy(IHocFsm<T> fsm)
        {

        }

        public virtual void stateTransition<TargetState>(IHocFsm<T> fsm)
        {

        }
    }

    public interface IHocFsm<T>
    {
        string name();
        T owner();
        int countState();
        bool isRunning();
        bool isDestroyed();
        HocFsmState<T> currentState();
        float currentStateTime();
        void start<StateType>();
        bool containState<StateType>();
        HocFsmState<T>[] getAllStates();
        bool containData(string dataName);
        HocVariable getData(string dataName);
        U getData<U>(string dataName);
        void setData(string dataName, HocVariable data);
        void removeData(string dataName);
    };


    public abstract class HocFsmBase
    {
        protected string _name;

        public abstract string name();
        public abstract bool isRunning();
        public abstract bool isDestroyed();
        public abstract string currentStateName();
        public abstract float currentStateTime();
        public abstract void update(float elapsedTime);
        public abstract void stop();
        public abstract void destroy();
    }

    public class HocFsm<T> : HocFsmBase, IHocFsm<T>
    {
        private Dictionary<string, HocVariable> _data;
        private Dictionary<Type, HocFsmState<T>> _states;
        private T _owner;
        private bool _isRunning = false;
        private bool _isDestroyed = false;
        private string _currentStateName;
        private HocFsmState<T> _currentState;
        private float _currentStateTime;

        static public HocFsm<T> createFsm(string name, T owner, List<HocFsmState<T>> states)
        {
            HocFsm<T> _fsm = new HocFsm<T>();
            _fsm._states = new Dictionary<Type, HocFsmState<T>>();
            _fsm._data = new Dictionary<string, HocVariable>();
            foreach (var item in states)
            {
                _fsm._states.Add(item.GetType(), item);
                item.awake(_fsm);
            }

            _fsm._name = name;
            _fsm._owner = owner;
            return _fsm;
        }

        public override string name() { return _name; }
        public T owner() { return _owner; }
        public int countState() { return _states.Count; }
        public override bool isRunning() { return _isRunning && (!_isDestroyed); }
        public override bool isDestroyed() { return _isDestroyed; }
        public override string currentStateName() { return _currentStateName; }
        public HocFsmState<T> currentState() { return _currentState; }
        public override float currentStateTime() { return _currentStateTime; }

        /// <summary>
        /// This function will always start the state regardless of the transition graph
        /// </summary>
        /// <typeparam name="StateType"></typeparam>
        /// <exception cref="Exception"></exception>
        public void start<StateType>()
        {
            if (!containState<StateType>())
            {
                throw new Exception("Failed to start state: " + typeof(StateType) + " because it is not registered as valid states"
                    + " for the state machine with name: " + _name);
            }


            if (_currentState != null)
            {
                if (_currentState.type == typeof(StateType)) return;
                _currentState.stop(this);
                _currentState.stateTransition<StateType>(this);
                _currentState = _states[typeof(StateType)];
                _currentState.start(this);
            }
            else
            {
                _currentState = _states[typeof(StateType)];
                _currentState.start(this);
            }
            _currentStateName = typeof(StateType).Name;
            _currentStateTime = 0;
            _isRunning = true;
        }

        public bool containState<StateType>()
        {
            return _states.ContainsKey(typeof(StateType));
        }

        public HocFsmState<T>[] getAllStates()
        {
            HocFsmState<T>[] _allStates = new HocFsmState<T>[_states.Count];
            int idx = 0;

            foreach (KeyValuePair<Type, HocFsmState<T>> p in _states)
            {
                _allStates[idx++] = p.Value;
            }

            return _allStates;
        }

        public bool containData(string dataName)
        {
            return _data.ContainsKey(dataName);
        }

        public HocVariable getData(string dataName)
        {
#if DEBUG
            if (!containData(dataName))
            {
                throw new Exception("State machine with name: " + _name + " does not contain data with name: " + dataName);
            }
#endif
            return _data[dataName];
        }


        public U getData<U>(string dataName)
        {
#if DEBUG
            if (!containData(dataName))
            {
                throw new Exception("State machine with name: " + _name + " does not contain data with name: " + dataName);
            }
#endif
            return (U)_data[dataName].value;

        }


        public void setData(string dataName, HocVariable data)
        {
            if (containData(dataName))
            {
                _data[dataName] = data;
                return;
            }

            _data.Add(dataName, data);
        }

        public void removeData(string dataName)
        {
            _data.Remove(dataName);
        }

        public override void update(float elapsedTime)
        {
            if (!_isRunning) return;
            _currentStateTime += elapsedTime;
            _currentState.update(this, elapsedTime);
        }

        public override void stop()
        {
            _isRunning = false;
            _currentState.stop(this);
            _currentStateTime = 0;
            _currentStateName = "None";
        }

        public override void destroy()
        {
            _isRunning = false;
            _isDestroyed = true;
            _data.Clear();
            _states.Clear();
        }
    }

    public interface IHocFsmManager
    {
        public int countFsm();
        public void addFsm<T>(IHocFsm<T> fsm);
        public void removeFsm(string fsmName);
        public bool containFsm(string fsmName);
        public void stopFsm(string fsmName);
        public void destroyFsm(string fsmName);
        public void clear();
        public HocFsmBase[] getAllFsms();
    }

    [Serializable]
    public class HocFsmDebugDisplay
    {
        [ReadOnly]
        public string fsmName;
        [ReadOnly]
        public float currentStateTime;
        [ReadOnly]
        public string currentStateName;
        [ReadOnly]
        public bool isRunning;
        [ReadOnly]
        public bool isDestroyed;
    }

}

public class HocFsmManager : MonoBehaviour, IHocFsmManager
{
    private Dictionary<string, HocFsmBase> _fsms;
    private static HocFsmManager _instance;
    public static HocFsmManager Instance
    {
        get { return _instance; }
    }

#if DEBUG && ENABLE_FSMMANAGER_DEBUG
    public List<HocFsmDebugDisplay> _fsmDisplay;
#endif

    private void Awake()
    {

        _fsms = new Dictionary<string, HocFsmBase>();
#if DEBUG && ENABLE_FSMMANAGER_DEBUG
        _fsmDisplay = new List<HocFsmDebugDisplay>();
#endif
        if(_instance != null && _instance != this)
        {
            Destroy(this);
            return;
        }
        _instance = this;
    }


    public int countFsm() {  return _fsms.Count; }
    public void addFsm<T>(IHocFsm<T> fsm)
    {
        string fsmName = fsm.name();
#if DEBUG
        if(containFsm(fsmName))
        {
            throw new Exception("Failed to add to fsm with name: " + fsmName + " because it already exists!");
        }
#endif
        _fsms.Add(fsmName, (HocFsmBase)fsm);
    }
    
    public void removeFsm(string fsmName)
    {
        _fsms.Remove(fsmName);
    }

    public bool containFsm(string fsmName)
    {
        return _fsms.ContainsKey(fsmName);
    }

    public void stopFsm(string fsmName)
    {
#if DEBUG
        if(!containFsm(fsmName))
        {
            throw new Exception("Failed to stop fsm with name: " + fsmName + " because it doesn't exist in the target fsm manager");
        }
#endif
        _fsms[fsmName].stop();
    }

    public void destroyFsm(string fsmName)
    {
#if DEBUG
        if(!containFsm(fsmName))
        {
            throw new Exception("Failed to destroy fsm with name: " + fsmName + " because it doesn't exist in the target fsm manager");
        }
#endif
        _fsms[fsmName].destroy();
    }

    public HocFsmBase[] getAllFsms()
    {
        HocFsmBase[] _allFsms = new HocFsmBase[_fsms.Count];
        int idx = 0;
        foreach(KeyValuePair<string, HocFsmBase> p in _fsms)
        {
            _allFsms[idx++] = p.Value;
        }
        return _allFsms;
    }

    public void clear()
    {
        _fsms.Clear();
    }


    private HocFsmDebugDisplay getFsmDebugInfo(string fsmName, in HocFsmBase fsm)
    {
        HocFsmDebugDisplay _info = new HocFsmDebugDisplay();
        _info.fsmName = fsmName;
        _info.currentStateTime = fsm.currentStateTime();
        _info.currentStateName = fsm.currentStateName();
        _info.isRunning = fsm.isRunning();
        _info.isDestroyed = fsm.isDestroyed();
        return _info;
    }
    public void update(float elapsedTime)
    {
        
#if DEBUG
        _fsmDisplay.Clear();
#endif
        var temp = new Dictionary<string, HocFsmBase>(_fsms);
        foreach(KeyValuePair<string, HocFsmBase> p in temp)
        {
            if(!p.Value.isDestroyed())
            {
                p.Value.update(elapsedTime);
            }
#if DEBUG && ENABLE_FSMMANAGER_DEBUG
            var info = getFsmDebugInfo(p.Key, p.Value);
            _fsmDisplay.Add(info);
#endif
        }
    }

    public void Update()
    {
        update(Time.deltaTime);
    }
}

