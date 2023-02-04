using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum SwordState {Normal,Attack,Block,Attacked,PowerAttack };

public class HocSword : MonoBehaviour
{
    [SerializeField] Collider2D _collider2D;
    [SerializeField] string enemySordTag;
    [SerializeField] string enemyBodyTag;
    public Player enemy;

    public SwordState state = SwordState.Normal;

    public HocStatus playerStatus;
    HocStatus enemyStatus;

    bool isEnemySwordIn = false;
    bool isEnemyBodyIn = false;
    bool swordColliderEnabled = false;
    bool isOutAttackPhase = false;

    public static bool isInExecuteMode = false;

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
        if (!swordColliderEnabled)
        {
            return;
        }
        else
        {
            if(state == SwordState.PowerAttack)
            {
                playerStatus.AddEnergy(-3);
            }
        }

        if (isInExecuteMode)
        {
            return;
        }

        bool enemySwordColliderEnabled = enemy.GetComponentInChildren<HocSword>().swordColliderEnabled;
        SwordState enemySwordState = enemy.GetComponentInChildren<HocSword>().state;

        if (isEnemySwordIn && swordColliderEnabled)
        {
            if(state == SwordState.PowerAttack)
            {
                if(enemySwordState == SwordState.Attack ||
                   enemySwordState == SwordState.PowerAttack)
                {
                    enemyStatus.AddPosture(-3);

                    state = SwordState.Attacked;
                }
            }
        }

        if (isEnemySwordIn && enemySwordColliderEnabled)
        {
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
                    if (enemySwordState == SwordState.PowerAttack)
                    {
                        playerStatus.AddPosture(-2);

                        enemy.GetComponentInChildren<HocSword>().state = SwordState.Attacked;
                    }
                    break;
                case SwordState.Normal:
                    if (enemySwordState == SwordState.Attack)
                    {
                        playerStatus.AddPosture(-2);

                        enemy.GetComponentInChildren<HocSword>().state = SwordState.Attacked;
                    }
                    break;
                case SwordState.Attack:
                    if (enemySwordState == SwordState.Attack)
                    {
                        playerStatus.AddPosture(-2);
                        enemyStatus.AddPosture(-2);

                        state = SwordState.Attacked;
                        enemy.GetComponentInChildren<HocSword>().state = SwordState.Attacked;
                    }
                    
                    if(enemySwordState == SwordState.Normal)
                    {
                        enemyStatus.AddPosture(-2);
                        state = SwordState.Attacked;
                    }
                    break;
                case SwordState.PowerAttack:
                    if (enemySwordState != SwordState.Attack &&
                       enemySwordState != SwordState.PowerAttack)
                    { 
                        enemyStatus.AddPosture(-2);

                        state = SwordState.Attacked;
                    }
                    break;
            }
        }
        else if (isEnemyBodyIn && !enemySwordColliderEnabled)
        {
            if (state == SwordState.Attack)
            {
                enemyStatus.AddPosture(-2);

                state = SwordState.Attacked;
            }
            if (state == SwordState.PowerAttack)
            {
                enemyStatus.AddPosture(-2);

                state = SwordState.Attacked;
            }
        }
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
            isEnemySwordIn = true;
        }
        if (other.CompareTag(enemyBodyTag))
        {
            isEnemyBodyIn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(enemySordTag))
        {
            isEnemySwordIn = false;
        }
        if (other.CompareTag(enemyBodyTag))
        {
            isEnemyBodyIn = false;
        }
    }

    void SwordVSSword()
    {
        isInExecuteMode = true;

        //Player.startVSTime = Time.time;
    }

    public void EnableSwordCollider()
    {
        swordColliderEnabled = true;
    }

    public void DisableSwordCollider()
    {
        swordColliderEnabled = false;
    }

    public void SwordAttackStart()
    {
        isOutAttackPhase = false;
    }

    public void SwordAttackOutPhase()
    {
        isOutAttackPhase = true;
    }
}
