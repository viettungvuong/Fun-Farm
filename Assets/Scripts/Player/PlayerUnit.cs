using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : Unit
{
    public int maxMoney;
    [HideInInspector] public int currentMoney;
    public override void Awake()
    {
        base.Awake();
        currentMoney = maxMoney;
    }
}
