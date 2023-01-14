using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum SwordState {Normal,Attack,Block,Attacked };

public class HocSword : MonoBehaviour
{
    [SerializeField] Collider2D _collider2D;
    [SerializeField] string enemySordTag;
    [SerializeField] string enemyBodyTag;
    [SerializeField] Player enemy;

    public SwordState state = SwordState.Normal;

    public HocStatus playerStatus;
    HocStatus enemyStatus;

    // Start is called before the first frame update
    void Start()
    {
        enemy.OnEnterCanPerfectBlockAniArea += PerfectBlockOn;
        enemy.OnExitCanPerfectBlockAniArea += PerfectBlockOff;

        enemyStatus = enemy.GetComponent<HocStatus>();

        state = SwordState.Normal;
    }

    // Update is called once per frame
    void Update()
    {

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
        if (other.CompareTag(enemySordTag))
        {
            //if (enemy.isLeft)
            //{
            //    return;
            //}

            SwordState enemySwordState = enemy.GetComponentInChildren<HocSword>().state;

            Debug.Log(state);
            Debug.Log(enemySwordState);
            switch (state)
            {
                case SwordState.Block:
                    if (enemySwordState == SwordState.Attack)
                    {
                        Debug.Log(name + " Block, when " + enemy.name + " attack");
                        if (canPerfectBlock && enterPerfectBlockAni)
                        {
                            playerStatus.AddEnergy(1);
                        }
                        else
                        {
                            playerStatus.AddPosture(-1);
                        }

                        enemy.GetComponentInChildren<HocSword>().state = SwordState.Attacked;
                    }
                break;
                case SwordState.Normal:
                    if(enemySwordState == SwordState.Attack)
                    {
                        playerStatus.AddPosture(-2);

                        enemy.GetComponentInChildren<HocSword>().state = SwordState.Attacked;
                    }
                break;
                case SwordState.Attack:
                    if(enemySwordState == SwordState.Attack)
                    {
                        playerStatus.AddPosture(-2);
                        enemyStatus.AddPosture(-2);

                        state = SwordState.Attacked;
                    }
                break;
            }
        }
        else if (other.CompareTag(enemyBodyTag))
        {
            bool enemySwordColliderEnabled = enemy.GetComponentInChildren<HocSword>()._collider2D.enabled;
            if (state == SwordState.Attack && !enemySwordColliderEnabled)
            {
                enemyStatus.AddPosture(-2);

                state = SwordState.Attacked;
            }
        }
    }

    public void EnableSwordCollider()
    {
        _collider2D.enabled = true;
    }

    public void DisableSwordCollider()
    {
        _collider2D.enabled = false;
    }
}
