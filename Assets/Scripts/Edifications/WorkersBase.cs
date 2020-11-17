using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkersBase : Singleton<WorkersBase>
{
    public int goldInVault = 0;
    UIManager uiManager;
    public Transform workersBaseLocation;
    public override void Awake()
    {
        base.Awake();
        uiManager = UIManager.Get();
        workersBaseLocation = transform;
    }
    public void DepositGold(int gold)
    {
        goldInVault += gold;
        uiManager.UpdateGoldUI();
        Debug.Log("A worker has deposit: " + gold + "Total Gold: " + goldInVault);
    }
    
    public void Buy(int gold)
    {
        if (goldInVault >= gold)
            goldInVault -= gold;
        else
            Debug.Log("Not Enough gold");

        if (goldInVault < 0)
            goldInVault = 0;

        uiManager.UpdateGoldUI();
    }

}
