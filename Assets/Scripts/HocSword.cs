using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HocSword : MonoBehaviour
{
    private bool _isDefenseMode = false;

    private int _maxPerfectBlockDetectionFrame = 10000;
    private int _defendModeFrameCounter = 0;
    // Start is called before the first frame update
    void Start()
    {
        System.Action enterDefendMode = () =>
        {
            _isDefenseMode = true;
            Debug.Log("Enter Defend Mode!");
        };

        System.Action exitDefendMode = () =>
        {
            _isDefenseMode = false;
            _defendModeFrameCounter = 0;
            Debug.Log("Exit Defend Mode!");
        };
        
        HocEventManager.Instance.addHocEventListener("enterDefendMode", enterDefendMode);
        HocEventManager.Instance.addHocEventListener("exitDefendMode", exitDefendMode);
    }

    // Update is called once per frame
    void Update()
    {
        if (_isDefenseMode && _defendModeFrameCounter < _maxPerfectBlockDetectionFrame) _defendModeFrameCounter++;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isDefenseMode)
        {
            // TODO: check frames counted by frame counter
        }
        else
        {
            // TODO: Reduce Health ... etc
        }
    }
}
