using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 牧场基类 
/// </summary>
public class Pasture : BuildItemBase
{
    public virtual void Awake()
    {
        Init();
        EventCenter.Instance.AddEventListener(GameEvent.日期每日更新变化,TurnDay);
    }

    private void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener(GameEvent.日期每日更新变化,TurnDay);
        
    }
}
