using System.Collections;
using System.Collections.Generic;
using HocInternal;
using UnityEngine;

public class HocStatus : MonoBehaviour
{
    public int energy;
    public int posture;
    private const int maxEnergy = 3;
    private const int maxPosture = 3;

    public PolygonCollider2D swordCollider;
    
    void Start()
    {
        energy = maxEnergy;
        posture = maxPosture;
    }

    public void setColliderStatus(bool isEnabled)
    {
        swordCollider.enabled = isEnabled;
    }

    public void addEnergy(int delta)
    {
        energy += delta;
    }

    public void addPosture(int delta)
    {
        posture += delta;
    }
    
    void Update()
    {
        
    }
}
