using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HocSword : MonoBehaviour
{
    [SerializeField] Collider2D _collider2D;

    private bool _isDefenseMode = false;

    private int _maxPerfectBlockDetectionFrame = 10000;
    //private int _defendModeFrameCounter = 0;
    public HocStatus playerStatus;
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
            //_defendModeFrameCounter = 0;
            //Debug.Log("Exit Defend Mode!");
        };
        
        HocEventManager.Instance.addHocEventListener("enterDefendMode", enterDefendMode);
        HocEventManager.Instance.addHocEventListener("exitDefendMode", exitDefendMode);

        Enemy.OnEnterCanPerfectBlockAniArea += PerfectBlockOn;
        Enemy.OnExitCanPerfectBlockAniArea += PerfectBlockOff;
    }

    // Update is called once per frame
    void Update()
    {
        //if (_isDefenseMode && _defendModeFrameCounter < _maxPerfectBlockDetectionFrame) _defendModeFrameCounter++;
        //Debug.Log("Defend Mode Frame: " + _defendModeFrameCounter);

    }

    public bool enterPerfectBlockAni = false;
    bool canPerfectBlock = false;
    void PerfectBlockOn()
    {
        canPerfectBlock = true;
    }
    void PerfectBlockOff()
    {
        canPerfectBlock = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemySword"))
        {
            if (_isDefenseMode)
            {
                if (canPerfectBlock && enterPerfectBlockAni)
                {
                    Debug.Log("Perfect Block");
                }
                else
                {
                    Debug.Log("Normal Block");
                }
                // TODO: check frames counted by frame counter
            }
            else
            {
                // TODO: Reduce Health ... etc
            }
        }
    }

    public void AttackNormal_EnterAttack()
    {
        _collider2D.enabled = true;
    }

    public void AttackNormal_ExitAttack()
    {
        _collider2D.enabled = false;
    }
}
