using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float searchPlayerRadius;
    [SerializeField] Transform searchPlayerCenterTf;

    [SerializeField] Animator animator;

    [SerializeField] AudioSource hitAudio;

    GameObject target = null;

    bool isAttacking = false;

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
        Collider2D collider2D = Physics2D.OverlapCircle(searchPlayerCenterTf.position, searchPlayerRadius);

        if (collider2D != null)
        {
            if (collider2D.CompareTag("Player"))
            {
                target = collider2D.gameObject;
            }
        }
        else
        {
            target = null;
        }
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

    public void OnAttackNormalEnd()
    {
        isAttacking = false;
    }

    public void OnAttackNormalHit()
    {
        hitAudio.Play();
    }
}