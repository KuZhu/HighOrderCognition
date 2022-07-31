using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HocInputManager : MonoBehaviour
{
    // Singleton Pattern
    private static HocInputManager _instance;
    public static HocInputManager Instance
    {
        get { return _instance; }
    }

    // Input Data
    private InputMaster _master;
    private Dictionary<string, InputAction> _actions;
    private Dictionary<string, bool> _isPressed;
    private List<InputAction> _playerControlActions;
    
    // Data Used to Implement Cached Input
    private InputAction _cachedInput;
    private bool _hasCachedInput = false;
    private float _cachedValue = 0.0f;
    private bool _enableCachedInput = false;
    private string _prevInputName = "";
#if DEBUG && ENABLE_INPUTSYSTEM_DEBUG
    [HocInternal.ReadOnly]
    public List<InputAction> inputActions; 
#endif

    private void Awake()
    {

        _master = new InputMaster();
        _actions = new Dictionary<string, InputAction>();
        _isPressed = new Dictionary<string, bool>();
#if DEBUG && ENABLE_INPUTSYSTEM_DEBUG
        inputActions = new List<InputAction>();
#endif
        if(_instance != null && _instance != this)
        {
            Destroy(this);
            return;
        }

        _instance = this;
    }

    private void Start()
    {
        string[] _actionNames =
        {
            "Dash",
            "Attack",
            "Block"
        };

        var p = _master.Player;
        InputAction[] _actionList =
        {
            p.Dash,
            p.Attack,
            p.Block
        };
#if DEBUG 
        if(HocConfig.Instance != null && HocConfig.Instance.enableInputSystemDebug)
        {
            inputActions.AddRange(_actionList);
        }
#endif
        {
            InputAction[] _playerControlActionList =
            {
                p.Dash
            };
            _playerControlActions = new List<InputAction>();
            _playerControlActions.AddRange(_playerControlActionList);
        }

        for(int i = 0; i < _actionNames.Length; i++)
        {
            _actions[_actionNames[i]] = _actionList[i];
            _actionList[i].Enable();
            _isPressed[_actionNames[i]] = false;
        }

#if DEBUG
        _actions["Attack"].performed +=  delegate(InputAction.CallbackContext context) { Debug.Log("Attack is Pressed!"); };
        _actions["Dash"].performed += delegate(InputAction.CallbackContext context)
        {
            Debug.Log("Dash is Pressed with Value: " + getValue<float>("Dash"));
        };
#endif
        System.Action<InputAction.CallbackContext> cacheInputRegister = delegate(InputAction.CallbackContext context)
        {
            if (!_hasCachedInput && _enableCachedInput)
            {
                _hasCachedInput = true;
                _cachedInput = context.action;
#if DEBUG
                Debug.Log("Cached Input: " + _cachedInput.name );
#endif
                if (_cachedInput.name == "Dash")
                {
                    _cachedValue = getValue<float>("Dash");
#if DEBUG
                    Debug.Log("Cached Input Value: " + _cachedValue);
#endif
                }
            }
        };
        
        _actions["Attack"].performed += cacheInputRegister;
        _actions["Dash"].performed += cacheInputRegister;
        _actions["Block"].performed += cacheInputRegister;
        


    }

    public void setPlayerControlActive(bool isActive)
    {
        foreach(var action in _playerControlActions)
        {
            if (isActive) action.Enable();
            else action.Disable();
        }
    }

     public T getValue<T>(string actionName) where T: struct  
    {
#if DEBUG
        if(HocConfig.Instance != null && HocConfig.Instance.enableInputSystemDebug)
        {
            if(!_actions.ContainsKey(actionName))
            {
                throw new System.Exception("Cannot get value from action: " + actionName + " since it doesn't exist!");
            }
        }
#endif
        return _actions[actionName].ReadValue<T>();
    }

     public void enableCachedInput(string prevInputName)
     {
         _enableCachedInput = true;
         _prevInputName = prevInputName;
     }

     public void releaseCachedInput()
     {
         _enableCachedInput = false;
         _hasCachedInput = false;
     }

     public void disableCachedInput()
     {
         _enableCachedInput = false;
         _prevInputName = "";
     }

     public bool isPressed(string actionName)
     {
#if DEBUG
        if(HocConfig.Instance && HocConfig.Instance.enableInputSystemDebug)
        {
            if(!_actions.ContainsKey(actionName))
            {
                throw new System.Exception("Cannot get value from action: " + actionName + " since it doesn't exist!");
            }
        }
#endif
        return _isPressed[actionName];
         
     }
    public bool isHold(string actionName)
    {
#if DEBUG
        if(HocConfig.Instance && HocConfig.Instance.enableInputSystemDebug)
        {
            if(!_actions.ContainsKey(actionName))
            {
                throw new System.Exception("Cannot get value from action: " + actionName + " since it doesn't exist!");
            }
        }
#endif
        return _actions[actionName].IsPressed();
    }

}
