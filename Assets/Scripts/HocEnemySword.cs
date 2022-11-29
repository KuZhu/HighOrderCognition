using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HocEnemySword : MonoBehaviour
{
    [SerializeField] Collider2D _collider2D;

    private void Start()
    {
        _collider2D.enabled = false;
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
