using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    public TextMeshProUGUI goldInVault;
    WorkersBase workersBase;
    void Start()
    {
        workersBase = WorkersBase.Get();
        goldInVault.text = "0";
    }

    public void UpdateGoldUI()
    {
        goldInVault.text = workersBase.goldInVault.ToString();
    }
}
