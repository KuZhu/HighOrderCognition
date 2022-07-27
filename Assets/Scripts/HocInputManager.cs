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
    private List<InputAction> _playerControlActions;
#if DEBUG && ENABLE_INPUTSYSTEM_DEBUG
    [HocInternal.ReadOnly]
    public List<InputAction> inputActions; 
#endif

    private void Awake()
    {

        _master = new InputMaster();
        _actions = new Dictionary<string, InputAction>();
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
            "move",
            "look",
            "fire"
        };

        var p = _master.Player;
        InputAction[] _actionList =
        {
            p.Move,
            p.Look,
            p.Fire
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
                p.Move
            };
            _playerControlActions = new List<InputAction>();
            _playerControlActions.AddRange(_playerControlActionList);
        }

        for(int i = 0; i < _actionNames.Length; i++)
        {
            _actions[_actionNames[i]] = _actionList[i];
            _actionList[i].Enable();
        }
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

    public bool isPressed(string actionName)
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
        return _actions[actionName].IsPressed();
    }

}
