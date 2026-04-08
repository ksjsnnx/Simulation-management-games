using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 工厂基类 
/// </summary>
public class Factory : BuildItemBase
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
