using System;
using System.Collections;
using System.Collections.Generic;
using HocInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HocStatus : MonoBehaviour
{
    [SerializeField] Animator postureTransition_UI;

    public int initialEnergy;
    public int initialPosture;

    private int energy;
    private int posture;

    public PolygonCollider2D swordCollider;

    public System.Action<int,int> OnPostureChange;
    public System.Action<int> OnEnergyChange;
    public static Action OnGameOver;
    
    void Start()
    {
        energy = initialEnergy;
        posture = initialPosture;

        OnEnergyChange?.Invoke(energy);
        OnPostureChange?.Invoke(posture,0);
    }
    
    public void SetColliderStatus(bool isEnabled)
    {
        swordCollider.enabled = isEnabled;
    }

    public int GetEnergy()
    {
        return energy;
    }
 
    public void AddEnergy(int delta)
    {
        energy += delta;
        energy = Mathf.Clamp(energy, 0, 3);

        OnEnergyChange?.Invoke(energy);
    }

    public void AddPosture(int delta)
    {
        posture += delta;
        posture = Mathf.Clamp(posture, 0, 3);

        OnPostureChange?.Invoke(posture,delta);
    }

    public int GetPosture()
    {
        return posture;
    }

    public void Update()
    {
        postureTransition_UI.SetInteger("posture", posture);
        if (posture <= 0)
        {
            OnGameOver?.Invoke();

            SceneManager.LoadScene("Start");
        }
    }

}
