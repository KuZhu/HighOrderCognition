using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public enum CanCacheInputType { A, D, MouseRight, None };

public class Player : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] HocStatus status;

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

    CanCacheInputType currentCache = CanCacheInputType.None;

    bool startCache = false;
    bool startDealAnimationEarlyEnd = false;
    bool firstInCache = false;
    bool firstDeal = false;
    private bool _dashing = false;
    private Coroutine _dashRoutine;

    bool attackValid = false;

    void Update()
    {
        // Handles Attack
        {
            if (HocInputManager.Instance.isHold("Attack"))
            {
                if (status.GetEnergy() < 3)
                {
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_ANI"))
                    {
                        animator.SetBool("toAttackNormal", true);

                        Vector2 targetPosition = (Vector2)transform.position + new Vector2(attackMoveDistance, 0);
                        attackMoveCoroutine = StartCoroutine(move(transform.position, targetPosition, attackDuration));
                    }
                }
                else
                {
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_ANI"))
                    {
                        animator.SetBool("toAttackPower", true);

                        status.AddEnergy(-3);

                        Vector2 targetPosition = (Vector2)transform.position + new Vector2(attackMoveDistance, 0);
                        attackMoveCoroutine = StartCoroutine(move(transform.position, targetPosition, attackDuration));
                    }
                }
            }

            if (!HocInputManager.Instance.isHold("Attack"))
            {
                animator.SetBool("toAttackNormal", false);
                animator.SetBool("toAttackPower", false);

                if (attackMoveCoroutine != null)
                {
                    StopCoroutine(attackMoveCoroutine);
                }

                //if (!animator.GetCurrentAnimatorStateInfo(0).IsName("AttackNormal_ANI"))
                //{
                //    if (attackMoveCoroutine != null)
                //    {
                //        StopCoroutine(attackMoveCoroutine);
                //    }
                //}
            }
        }

        // Handles Rush / Dash
        {
            //Debug.Log("Has Cached Input: " + HocInputManager.Instance.hasCachedInput);
            if (HocInputManager.Instance.getValue<float>("Dash") < 0.0f &&
                animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_ANI"))
            {
                dash("toRushLeft", -1);
                // ToLeftDash("toRushLeft");
            }
            else if (HocInputManager.Instance.getValue<float>("Dash") > 0.0f &&
                     animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_ANI"))
            {
                dash("toRushRight", 1);
                //ToRightDash("toRushRight");
            }
        }

        // Handles Block
        if (HocInputManager.Instance.isHold("Block")) ToDefend("toBlock");
        else
        {
            animator.SetBool("toBlock", false);
            HocEventManager.Instance.dispatchHocEvent("exitDefendMode", 0);
        }

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
            HocEventManager.Instance.dispatchHocEvent("enterDefendMode", 0);
        }
        else
        {
            animator.SetTrigger(transitionName);
        }
    }

    void enableCachedInput()
    {
        Debug.Log("Released Cached Input: " + HocInputManager.Instance.hasCachedInput);
        HocInputManager.Instance.releaseCachedInput();
        HocInputManager.Instance.enableCachedInput();
    }

    void processEarlyEndCache()
    {
        if (HocInputManager.Instance.hasCachedInput)
        {
            string cachedName = HocInputManager.Instance.cachedInput.name;
            switch (cachedName)
            {
                case "Block":
                    ToDefend("forceBlock");
                    break;
                case "Dash":
                    Debug.Log("Early processing Dash with _dashing = " + _dashing);
                    if(HocInputManager.Instance.cachedValue < 0.0f) dash("toRushLeft", -1); //ToLeftDash("forceLeftDash");
                    else dash("toRushRight", 1); //ToRightDash("forceRightDash");
                    break;
                default:
                    break;
            }
        }
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
        Vector2 targetPosition = (Vector2)transform.position + new Vector2(-rushMoveDistance, 0);
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
        sword.AttackNormal_EnterAttack();

        attackValid = true;
    }

    public void OnAttackNormalEnd()
    {
        sword.AttackNormal_ExitAttack();

        attackValid = false;
    }

    public void OnEnterBlockPerfect()
    {
        Debug.Log("Enter Perfect Block Ani");
        sword.enterPerfectBlockAni = true;
    }

    public void OnExitBlockPerfect()
    {
        Debug.Log("Exit Perfect Block Ani");
        sword.enterPerfectBlockAni = false;
    }
}
