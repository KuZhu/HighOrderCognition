using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostureTransitionUI : MonoBehaviour
{
    [SerializeField] HocStatus playerStats;


    public void OnTransitionAniEnd()
    {
        Animator postureUI_ani = transform.parent.GetComponent<Animator>();
        int i = playerStats.GetPosture();
        postureUI_ani.SetInteger("posture", i);
    }
}
