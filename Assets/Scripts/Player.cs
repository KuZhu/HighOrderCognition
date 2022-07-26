using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CanCacheInputType { A, D, MouseRight, None };

public class Player : MonoBehaviour
{
    [SerializeField] Animator animator;

    [Header("Attack")]
    [SerializeField] float attackMoveDistance;
    [SerializeField] float attackDuration;

    [Header("Rush")]
    [SerializeField] float rushMoveDistance;
    [SerializeField] float rushDuration;
    //[SerializeField] LayerMask colliderLayer;
    Coroutine attackMoveCoroutine = null;

    CanCacheInputType currentCache = CanCacheInputType.None;

    bool startCache = false;
    bool startDealAnimationEarlyEnd = false;
    bool firstInCache = false;
    bool firstDeal = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_ANI"))
            {
                animator.SetBool("toAttackNormal", true);

                Vector2 targetPosition = (Vector2)transform.position + new Vector2(attackMoveDistance, 0);
                attackMoveCoroutine = StartCoroutine(move(transform.position, targetPosition, attackDuration));
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            animator.SetBool("toAttackNormal", false);

            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("AttackNormal_ANI"))
            {
                if (attackMoveCoroutine != null)
                {
                    StopCoroutine(attackMoveCoroutine);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_ANI"))
            {
                ToLeftDash("toRushLeft");
            }
        }
        
        if (Input.GetMouseButtonDown(1))
        { 
            ToDefend("toBlock"); 
        }

        if (Input.GetMouseButtonUp(1))
            {
                animator.SetBool("toBlock", false);
            }


        if (Input.GetKeyDown(KeyCode.D))
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_ANI"))
            {
                ToRightDash("toRushRight");
            }
        }

        if (startCache)
        {
            if (!firstInCache)
            {
                firstInCache = true;
                Debug.Log("start cache");
            }
            if(Input.GetKeyDown(KeyCode.A))
            {
                currentCache = CanCacheInputType.A;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                currentCache = CanCacheInputType.D;
            }
            if (Input.GetMouseButtonDown(1))
            {
                currentCache = CanCacheInputType.MouseRight;
            }
        }

        if (startDealAnimationEarlyEnd)
        {
            if (!firstDeal)
            {
                firstDeal = true;
                Debug.Log("Deal early animation end");
            }

            switch (currentCache)
            {
                case CanCacheInputType.D:
                    ToRightDash("forceRightDash");
                    startDealAnimationEarlyEnd = false;
                    break;
                case CanCacheInputType.MouseRight:
                    ToDefend("forceBlock");
                    startDealAnimationEarlyEnd = false;
                    break;
                case CanCacheInputType.A:
                    ToLeftDash("forceLeftDash");
                    startDealAnimationEarlyEnd = false;
                    break;
                case CanCacheInputType.None:
                    break;
            }

            currentCache = CanCacheInputType.None;
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
  

        
    void ToLeftDash(string triggerName)
    {
        animator.SetTrigger(triggerName);
      
        Vector2 targetPosition = (Vector2)transform.position + new Vector2(-rushMoveDistance, 0);
        //targetPosition = GetActureTargetposition((Vector2)transform.position, targetPosition);
        StartCoroutine(move(transform.position, targetPosition, rushDuration));
    }

    void ToRightDash(string triggerName)
    {
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
            //Debug.Log(t);
            t += Time.deltaTime / duration;
            yield return new WaitForEndOfFrame();
        }
    }

//    Vector2 GetActureTargetposition(Vector2 startPosition, Vector2 targetPosition)
//    {
//        Vector2 direction = targetPosition - startPosition;
//        RaycastHit2D? hit = Physics2D.Raycast(transform.position, direction, rushMoveDistance, colliderLayer);
//        float distance = Vector2.Distance(targetPosition, startPosition);


//        if (hit != null)
//            {
//                distance = Vector2.Distance(hit.Value.transform.position, startPosition);
//            }
//       if(distance < rushMoveDistance)
//       {
//            return startPosition + new Vector2(distance, 0);
//        }
//        else
//       {
//           return targetPosition;
//       }


//    }
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
}
