using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject posturePrefab;
    [SerializeField] GameObject energyPrefab;

    [SerializeField] Transform postureUITf;
    [SerializeField] Transform energyUITf;

    [SerializeField] HocStatus playerStatus;

    private void Awake()
    {
        playerStatus.OnPostureChange += UpdatePosture;
        playerStatus.OnEnergyChange += UpdateEnergy;
    }

    void UpdatePosture(int amount,int delta)
    {
        ClearChildObj(postureUITf);

        for(int i = 0; i < amount; i++)
        {
            Instantiate(posturePrefab, postureUITf);
        }
    }

    void UpdateEnergy(int amount)
    {
        ClearChildObj(energyUITf);

        for (int i = 0; i < amount; i++)
        {
            Instantiate(energyPrefab, energyUITf);
        }

    }

    void ClearChildObj(Transform parentTf)
    {
        int num = parentTf.childCount;
        for(int i =0; i< num; i++)
        {
            Destroy(parentTf.GetChild(i).gameObject);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
