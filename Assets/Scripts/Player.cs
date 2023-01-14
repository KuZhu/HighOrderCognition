using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public enum CanCacheInputType { A, D, MouseRight, None };

public class Player : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] HocStatus status;

    [Header("Input")]
    InputMaster inputMaster;
    public bool isLeft;

    [Header("Attack")]
    [SerializeField] float attackMoveDistance;
    [SerializeField] float attackDuration;

    [Header("Rush")]
    [SerializeField] float rushMoveDistance = 2;
    [SerializeField] float rushDuration;
    [SerializeField] LayerMask colliderLayer;
    Coroutine attackMoveCoroutine = null;

    [Header("Sword")]
    [SerializeField] HocSword sword;

    public System.Action OnEnterCanPerfectBlockAniArea;
    public System.Action OnExitCanPerfectBlockAniArea;

    CanCacheInputType currentCache = CanCacheInputType.None;

    bool startCache = false;
    bool startDealAnimationEarlyEnd = false;
    bool firstInCache = false;
    bool firstDeal = false;
    private bool _dashing = false;
    private Coroutine _dashRoutine;

    bool attackValid = false;

    private void Awake()
    {

    }

    private void Start()
    {
        inputMaster = new InputMaster();

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

    void Attack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (status.GetEnergy() < 3)
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_ANI"))
                {
                    animator.SetBool("toAttackNormal", true);

                    Vector2 targetPosition = (Vector2)transform.position + new Vector2(attackMoveDistance, 0) * transform.localScale.x;
                    attackMoveCoroutine = StartCoroutine(move(transform.position, targetPosition, attackDuration));
                }
            }
            else
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_ANI"))
                {
                    animator.SetBool("toAttackPower", true);

                    Vector2 targetPosition = (Vector2)transform.position + new Vector2(attackMoveDistance, 0) * transform.localScale.x;
                    attackMoveCoroutine = StartCoroutine(move(transform.position, targetPosition, attackDuration));
                }
            }

            sword.state = SwordState.Attack;
        }
    }

    void AttackCancle(InputAction.CallbackContext context)
    {
        animator.SetBool("toAttackNormal", false);
        animator.SetBool("toAttackPower", false);

        if (attackMoveCoroutine != null)
        {
            StopCoroutine(attackMoveCoroutine);
        }

        sword.state = SwordState.Normal;
    }

    void Block(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ToDefend("toBlock");

            sword.state = SwordState.Block;
        }
    }

    void BlockCancle(InputAction.CallbackContext context)
    {
        animator.SetBool("toBlock", false);

        sword.state = SwordState.Normal;
    }

    void DashInput(InputAction.CallbackContext context)
    {
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_ANI"))
        { 
            if (context.ReadValue<float>() < 0.0)
            {
                if (transform.localScale.x < 0)
                {
                    dash("toRushRight", -1);
                }
                else
                {
                    dash("toRushLeft", -1);
                }
            }
            else if(context.ReadValue<float>() > 0.0)
            {
                if (transform.localScale.x < 0)
                {
                    dash("toRushLeft", 1);
                }
                else
                {
                    dash("toRushRight", 1);
                }
            }
        }
    }

    void Update()
    {
        // Handles Attack
        {
            
        }

        // Handles Rush / Dash
        {
            //Dash();
        }

        // Handles Block

        {
            // if (startDealAnimationEarlyEnd)
            // {
            //     if (!firstDeal)
            //     {
            //         firstDeal = true;
            //         Debug.Log("Deal early animation end");
            //     }

            //     switch (currentCache)
            //     {
            //         case CanCacheInputType.D:
            //             ToRightDash("forceRightDash");
            //             startDealAnimationEarlyEnd = false;
            //             break;
            //         case CanCacheInputType.MouseRight:
            //             ToDefend("forceBlock");
            //             startDealAnimationEarlyEnd = false;
            //             break;
            //         case CanCacheInputType.A:
            //             ToLeftDash("forceLeftDash");
            //             startDealAnimationEarlyEnd = false;
            //             break;
            //         case CanCacheInputType.None:
            //             break;
            //     }

            //     currentCache = CanCacheInputType.None;
            // }
        }

        if (attackValid)
        {

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

    void enableCachedInput()
    {
        //Debug.Log("Released Cached Input: " + HocInputManager.Instance.hasCachedInput);
        //HocInputManager.Instance.releaseCachedInput();
        //HocInputManager.Instance.enableCachedInput();
    }

    void processEarlyEndCache()
    {
        //if (HocInputManager.Instance.hasCachedInput)
        //{
        //    string cachedName = HocInputManager.Instance.cachedInput.name;
        //    switch (cachedName)
        //    {
        //        case "Block":
        //            ToDefend("forceBlock");
        //            break;
        //        case "Dash":
        //            Debug.Log("Early processing Dash with _dashing = " + _dashing);
        //            if(HocInputManager.Instance.cachedValue < 0.0f) dash("toRushLeft", -1); //ToLeftDash("forceLeftDash");
        //            else dash("toRushRight", 1); //ToRightDash("forceRightDash");
        //            break;
        //        default:
        //            break;
        //    }
        //}
    }

    void dash(string triggerName, float direction)
    {
        if (_dashRoutine != null) StopCoroutine(_dashRoutine);
        animator.SetTrigger(triggerName);
        Vector2 targetPosition = (Vector2)transform.position + direction * new Vector2(rushMoveDistance, 0);
        _dashRoutine = StartCoroutine(move(transform.position, targetPosition, rushDuration));
    }
        
    void ToLeftDash(string triggerName)
    {
        if (_dashing) return;
        animator.SetTrigger(triggerName);
        Vector2 targetPosition = (Vector2)transform.position + new Vector2(-rushMoveDistance, 0) * transform.localScale.x;
        //targetPosition = GetActureTargetposition((Vector2)transform.position, targetPosition);
        StartCoroutine(move(transform.position, targetPosition, rushDuration));
    }

    void ToRightDash(string triggerName)
    {
        if (_dashing) return;
        animator.SetTrigger(triggerName);
        Vector2 targetPosition = (Vector2)transform.position + new Vector2(rushMoveDistance, 0);
        //targetPosition = GetActureTargetposition((Vector2)transform.position, targetPosition);
        StartCoroutine(move(transform.position, targetPosition, rushDuration));
    }

    IEnumerator move(Vector2 startPosition, Vector2 targetPosition,float duration)
    {
        float t = 0;
        while (t <= 1)
        {
            transform.position = Vector2.Lerp(startPosition, targetPosition,t);
            t += Time.deltaTime / duration;
            yield return null;
        }
        _dashRoutine = null;
    }

    Vector2 GetActureTargetposition(Vector2 startPosition, Vector2 targetPosition)
    {
        Vector2 direction = targetPosition - startPosition;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, rushMoveDistance, colliderLayer);
        float distance = Vector2.Distance(targetPosition, startPosition);


        if (hit.collider != null)
            {
                distance = Vector2.Distance(hit.transform.position, startPosition);
           }
      if(distance < rushMoveDistance)
       {
            return startPosition + new Vector2(distance, 0);
        }
        else
       {
           return startPosition - new Vector2(distance,0);
       }


    }

    public void AttackAniEnd()
    {
        animator.SetBool("toAttackNormal", false);

        startDealAnimationEarlyEnd = false;
        startCache = false;
        firstInCache = false;
        firstDeal = false;
    }

    public void StartCache()
    {
        startCache = true;
    }

    public void DealEarlyEnd()
    {
        startDealAnimationEarlyEnd = true;
    }

    public void OnAttackNormalHit()
    {
        sword.EnableSwordCollider();

        attackValid = true;
    }

    public void OnAttackNormalEnd()
    {
        sword.DisableSwordCollider();

        attackValid = false;
    }

    public void OnAttackPowerHit()
    {
        status.AddEnergy(-3);

        attackValid = true;
    }

    public void OnAttackPowerEnd()
    {
        attackValid = false;
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
}
