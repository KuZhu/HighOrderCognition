using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float searchPlayerRadius;
    [SerializeField] Transform searchPlayerCenterTf;

    [SerializeField] Animator animator;

    [SerializeField] AudioSource hitAudio;

    [SerializeField] HocEnemySword sword;

    GameObject target = null;

    bool isAttacking = false;

    public static System.Action OnEnterCanPerfectBlockAniArea;
    public static System.Action OnExitCanPerfectBlockAniArea;

    private void Update()
    {
        SearchPlayer();

        if(target != null)
        {
            Attack();
        }
    }

    void SearchPlayer()
    {
        Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(searchPlayerCenterTf.position, searchPlayerRadius);
        foreach (Collider2D collider2D in collider2Ds)
        { 
            if (collider2D.CompareTag("Player"))
            {
                target = collider2D.gameObject;

                return;
            }
        }

        target = null;
    }

    void Attack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            animator.SetTrigger("toAttackNormal");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(searchPlayerCenterTf.position, searchPlayerRadius);
    }

    public void OnAttackNormalHit()
    {
        hitAudio.Play();

        sword.AttackNormal_EnterAttack();
    }

    public void OnAttackNormalEnd()
    {
        isAttacking = false;

        sword.AttackNormal_ExitAttack();
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
