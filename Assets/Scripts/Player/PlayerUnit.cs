using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUnit : Unit
{
    public int maxMoney;
    public TextMeshProUGUI coinText;
    [HideInInspector] public int currentMoney;
    public override void Awake()
    {
        base.Awake();
        currentMoney = maxMoney;
    }

    private void LateUpdate() {
        coinText.text = currentMoney.ToString();
    }
}
