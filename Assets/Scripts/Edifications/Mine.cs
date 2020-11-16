using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    [Header("Mine Settings")]
    public int minGold = 100;
    public int maxGold = 300;
    public int currentGoldCapacity;

    public GameObject flag;
    public bool isMarked = false;

    private void Start()
    {
        currentGoldCapacity = Random.Range(minGold, maxGold);
    }

    public void SetMark(bool state)
    {
        isMarked = state;
        flag.SetActive(state);
        UnitsManager.Get().AddMarkedMine(gameObject);
    }

    public bool GetMark() => isMarked ? true : false;

    public void StealGold(int gold)
    {
        if(currentGoldCapacity > 0)
        currentGoldCapacity -= gold;

        if (currentGoldCapacity < 0)
        {
            currentGoldCapacity = 0;
            Destroy(gameObject);
        }       
    }

}
