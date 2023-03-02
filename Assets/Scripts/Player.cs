using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;


public enum CanCacheInputType { DashRight, DashLeft, Attack, Block };

public class Player : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Animator bleed;
    [SerializeField] HocStatus status;

    [Header("Input")]
    InputMaster inputMaster;
    public bool isLeft;

    [Header("Attack")]
    [SerializeField] float attackMoveDistance;
    [SerializeField] float attackDuration;

    [Header("Take Damage")]
    [SerializeField] float backDistance;
    [SerializeField] float backDuration;

    [Header("Rush")]
    [SerializeField] float dashMoveDistance = 2;
    [SerializeField] float dashDuration;

    [Header("Move")]
    [SerializeField] string enemyTag;
    [SerializeField] float gapDistance;

    [Header("Sword")]
    [SerializeField] HocSword sword;

    public System.Action OnEnterCanPerfectBlockAniArea;
    public System.Action OnExitCanPerfectBlockAniArea;

    CanCacheInputType currentCache;

    bool startCache = false;
    bool cached = false;

    private void Start()
    {
        inputMaster = new InputMaster();

        status.OnPostureChange += OnTakeDamage;

        HocStatus.OnGameOver += OnGameOver;

        if (isLeft)
        {
            inputMaster.Player.Enable();

            inputMaster.Player.Attack.performed += Attack;
            inputMaster.Player.Attack.canceled += AttackCancle;

            inputMaster.Player.Block.performed += Block;
            inputMaster.Player.Block.canceled += BlockCancle;

            inputMaster.Player.Rush.Enable();
            inputMaster.Player.Rush.performed += DashInput;
        }
        else
        {
            inputMaster.Enemy.Enable();

            inputMaster.Enemy.Attack.performed += Attack;
            inputMaster.Enemy.Attack.canceled += AttackCancle;

            inputMaster.Enemy.Block.performed += Block;
            inputMaster.Enemy.Block.canceled += BlockCancle;

            inputMaster.Enemy.Rush.Enable();
            inputMaster.Enemy.Rush.performed += DashInput;
        }
    }

    void OnGameOver()
    {
        if (isLeft)
        {
            inputMaster.Player.Attack.performed -= Attack;
            inputMaster.Player.Attack.canceled -= AttackCancle;

            inputMaster.Player.Block.performed -= Block;
            inputMaster.Player.Block.canceled -= BlockCancle;

            inputMaster.Player.Rush.performed -= DashInput;
            inputMaster.Player.Rush.Disable();

            inputMaster.Player.Disable();
        }
        else
        {
            inputMaster.Enemy.Attack.performed -= Attack;
            inputMaster.Enemy.Attack.canceled -= AttackCancle;

            inputMaster.Enemy.Block.performed -= Block;
            inputMaster.Enemy.Block.canceled -= BlockCancle;

            inputMaster.Enemy.Rush.performed -= DashInput;
            inputMaster.Enemy.Rush.Disable();

            inputMaster.Enemy.Disable();
        }

        Destroy(gameObject);
    }

    void OnTakeDamage(int currentPosture,int delta)
    {
        if (Mathf.Abs(delta) > 1)
        {
            animator.SetTrigger("toTakeDamage");
            bleed.SetTrigger("bleed");
        }
        else
        {
            moving = true;
            moveT = 0;
            moveDuration = backDuration;
            moveStartPosition = (Vector2)transform.position;
            moveTargetPosition = (Vector2)transform.position + new Vector2(-backDistance, 0) * transform.localScale.x;
            moveDirection = moveTargetPosition - moveStartPosition;
        }
    }

    void Attack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (startCache)
            {
                if (!cached)
                {
                    cached = true;
                    currentCache = CanCacheInputType.Attack;
                }
            }

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_ANI"))
            {
                _Attack(false);
            }
        }
    }


    void _Attack(bool isForce)
    {
        if (status.GetEnergy() < 3)
        {
            if (isForce)
            {
                animator.SetTrigger("forceAttackNormal");  
            }
            animator.SetBool("toAttackNormal", true);          

            moving = true;
            moveT = 0;
            moveDuration = attackDuration;
            moveStartPosition = (Vector2)transform.position;
            moveTargetPosition = (Vector2)transform.position + new Vector2(attackMoveDistance, 0) * transform.localScale.x;
            moveDirection = moveTargetPosition - moveStartPosition;

            sword.state = SwordState.Attack;
        }
        else
        {
            if (isForce)
            {
                animator.SetTrigger("forceAttackPower");
            }
            animator.SetBool("toAttackPower", true);

            moving = true;
            moveT = 0;
            moveDuration = attackDuration;
            moveStartPosition = (Vector2)transform.position;
            moveTargetPosition = (Vector2)transform.position + new Vector2(attackMoveDistance, 0) * transform.localScale.x;
            moveDirection = moveTargetPosition - moveStartPosition;

            sword.state = SwordState.PowerAttack;
        }
    }

    void AttackCancle(InputAction.CallbackContext context)
    {
        if (startCache)
        {
            if (cached)
            {
                if (currentCache == CanCacheInputType.Attack)
                {
                    cached = false;
                }
            }
        }

        animator.SetBool("toAttackNormal", false);
        animator.SetBool("toAttackPower", false);

        moving = false;
    }

    void Block(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (startCache)
            {
                if (!cached)
                {
                    cached = true;
                    currentCache = CanCacheInputType.Block;
                }
            }

            _Block(false);
        }
    }

    void _Block(bool isForce)
    {
        if (isForce)
        {
            animator.SetTrigger("forceBlock");
        }

        ToDefend("toBlock");

        sword.state = SwordState.Block;
    }

    void BlockCancle(InputAction.CallbackContext context)
    {
        if (startCache)
        {
            if (cached)
            {
                if (currentCache == CanCacheInputType.Block)
                {
                    cached = false;
                }
            }
        }

        animator.SetBool("toBlock", false);

        sword.state = SwordState.Normal;
    }

    void DashInput(InputAction.CallbackContext context)
    {
        if (startCache)
        {
            if (!cached)
            {
                cached = true;

                if (context.ReadValue<float>() < 0.0)
                {
                    currentCache = CanCacheInputType.DashLeft;
                }
                else if(context.ReadValue<float>() > 0.0)
                {
                    currentCache = CanCacheInputType.DashRight;
                }
            }
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_ANI"))
        {
            _Dash(context.ReadValue<float>(), false);
        }
    }

    void _Dash(float direction, bool isForce)
    {
        if (direction < 0.0)
        {
            if (isLeft)
            {
                if (isForce)
                {
                    Dash("forceDashLeft",-1);
                }
                else
                {
                    Dash("toDashLeft",-1);
                }
            }
            else
            {
                if (isForce)
                {
                    Dash("forceDashRight",1);
                }
                else
                {
                    Dash("toDashRight",1);
                }
            }
        }
        else if (direction > 0.0)
        {
            if (isLeft)
            {
                if (isForce)
                {
                    Dash("forceDashRight",1);
                }
                else
                {
                    Dash("toDashRight",1);
                }
            }
            else
            {
                if (isForce)
                {
                    Dash("forceDashLeft",-1);
                }
                else
                {
                    Dash("toDashLeft",-1);
                }
            }
        }
    }

    bool moving = false;
    Vector2 moveStartPosition;
    Vector2 moveTargetPosition;
    float moveT = 0;
    float moveDuration;
    Vector2 moveDirection;

    Vector2 GetActureTargetposition(Vector2 currentPosition, Vector2 targetPosition)
    {
        float remainDistance = Vector2.Distance(transform.position, moveTargetPosition);

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, moveDirection, remainDistance);

        float distanceToTarget = remainDistance;
        foreach (var hit in hits)
        {
            if (hit.collider.gameObject.tag == enemyTag)
            {
                distanceToTarget = hit.distance - gapDistance;
            }
        }

        if (distanceToTarget < remainDistance)
        {
            return currentPosition + new Vector2(distanceToTarget, 0) * transform.localScale.x;
        }
        else
        {
            return targetPosition;
        }
    }

    void Update()
    {
        if (moving) 
        {
            moveTargetPosition = GetActureTargetposition(transform.position, moveTargetPosition);

            transform.position = Vector2.Lerp(moveStartPosition, moveTargetPosition, moveT);
            moveT += Time.deltaTime / moveDuration;

            if (moveT >= 1)
            {
                moveT = 0;
                moving = false; ;
            }
        }
    }

    void ToDefend(string transitionName)
    {
        if (transitionName == "toBlock")
        {
            animator.SetBool("toBlock", true);
        }
        else
        {
            animator.SetTrigger(transitionName);
        }
    }

    void Dash(string triggerName, int direction)
    {
        animator.SetTrigger(triggerName);

        moving = true;
        moveT = 0;
        moveDuration = dashDuration;
        moveStartPosition = (Vector2)transform.position;
        moveTargetPosition = (Vector2)transform.position + direction * transform.localScale.x * new Vector2(dashMoveDistance, 0);
        moveDirection = moveTargetPosition - moveStartPosition;
    }

    public void StartCache()
    {
        startCache = true;
    }

    public void DealEarlyEnd()
    {
        startCache = false;

        if (cached)
        {
            cached = false;

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("DashRight_ANI") ||
                animator.GetCurrentAnimatorStateInfo(0).IsName("DashLeft_ANI"))
            {
                if (currentCache == CanCacheInputType.Attack)
                {
                    _Attack(true);

                    return;
                }

                if(currentCache == CanCacheInputType.Block)
                {
                    _Block(true);

                    return;
                }

                if (currentCache == CanCacheInputType.DashRight)
                {
                    _Dash(1, true);

                    return;
                }

                if (currentCache == CanCacheInputType.DashLeft)
                {
                    _Dash(-1, true);

                    return;
                }
            }

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("AttackNormal_ANI") ||
                animator.GetCurrentAnimatorStateInfo(0).IsName("AttackPower_ANI"))
            {
                if (currentCache == CanCacheInputType.DashRight)
                {
                    OnAttackAniEnd();
                    _Dash(1, true);

                    return;
                }

                if (currentCache == CanCacheInputType.DashLeft)
                {
                    OnAttackAniEnd();
                    _Dash(-1, true);

                    return;
                }

                if (currentCache == CanCacheInputType.Block)
                {
                    OnAttackAniEnd();
                    _Block(true);

                    return;
                }
                if (currentCache == CanCacheInputType.Attack)
                {
                    OnAttackAniEnd();
                    _Attack(true);
                    return;
                }
            }
        }
    }

    public void OnAttackNormalHit()
    {
        sword.EnableSwordCollider();
    }

    public void OnAttackNormalEnd()
    {
        sword.DisableSwordCollider();

        sword.SwordAttackOutPhase();
    }

    public void OnAttackPowerHit()
    {
        sword.EnableSwordCollider();
    }

    public void OnAttackPowerEnd()
    {
        sword.DisableSwordCollider();

        sword.SwordAttackOutPhase();
    }

    public void OnEnterBlockPerfect()
    {
        //Debug.Log(name + " Enter Perfect Block Ani");

        sword.EnableSwordCollider();

        sword.enterPerfectBlockAni = true; 
    }

    public void OnExitBlockPerfect()
    {
        //Debug.Log(name + " Exit Perfect Block Ani");
        sword.enterPerfectBlockAni = false;
    }

    public void OnExitBlockAni()
    {
        sword.DisableSwordCollider();
    }

    public void _OnEnterCanPerfectBlockAniArea()
    {
        OnEnterCanPerfectBlockAniArea?.Invoke();
    }

    public void _OnExitCanPerfectBlockAniArea()
    {
        OnExitCanPerfectBlockAniArea?.Invoke();
    }

    public void OnAttackAniEnd()
    {
        sword.state = SwordState.Normal;

        animator.SetBool("toAttackNormal", false);
        animator.SetBool("toAttackPower", false);
    }

    public void OnAttackStart()
    {
        sword.SwordAttackStart();
    }
}
