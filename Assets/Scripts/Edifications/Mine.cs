using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Mine : MonoBehaviour
{
    [Header("Mine Settings")]
    public int minGold = 100;
    public int maxGold = 300;
    public int currentGoldCapacity;
    public TextMeshProUGUI goldAmount;

    public GameObject flag;
    public bool isMarked = false;
    public bool hasGold = true;

    private void Start()
    {
        currentGoldCapacity = Random.Range(minGold, maxGold);
        UpdateUI();
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
        {
            currentGoldCapacity -= gold;
            UpdateUI();
        }
        if (currentGoldCapacity < 0)
        {
            hasGold = false;
            currentGoldCapacity = 0;
            UpdateUI();
        }       
    }

    void UpdateUI()
    {
        goldAmount.text = currentGoldCapacity.ToString();
    }

}
