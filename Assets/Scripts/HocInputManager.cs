using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HocInputManager : MonoBehaviour
{
    private InputMaster _master;

    private void Start()
    {
        _master = new InputMaster();
        _master.Enemy.Rush.Enable();
        _master.Enemy.Rush.performed += delegate (InputAction.CallbackContext context)
        {
            Debug.Log("Enemy: " + context.ReadValue<float>());
        };

        _master.Player.Rush.Enable();
        _master.Player.Rush.performed += delegate (InputAction.CallbackContext context)
        {
            Debug.Log("Player: " + context.ReadValue<float>());
        };
    }
}
