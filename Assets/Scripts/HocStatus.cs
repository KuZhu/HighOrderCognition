using System.Collections;
using System.Collections.Generic;
using HocInternal;
using UnityEngine;

public class HocStatus : MonoBehaviour
{
    public int initialEnergy;
    public int initialPosture;

    private int energy;
    private int posture;

    public PolygonCollider2D swordCollider;

    public System.Action<int> OnPostureChange;
    public System.Action<int> OnEnergyChange;
    
    void Start()
    {
        energy = initialEnergy;
        posture = initialPosture;

        OnEnergyChange?.Invoke(energy);
        OnPostureChange?.Invoke(posture);
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

        OnPostureChange?.Invoke(posture);
    }
    
    void Update()
    {
        
    }
}
