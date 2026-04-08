using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController
{
    public long ticks=954864000;
    public float timer;//表示当前过了多久  如果过了一个daytime就重置为0 重新计时  
    public float daytime = 0.5f;//表示一秒更新一天  
    

/// <summary>
/// 更新时间
/// </summary>
    public void UpdateTime()
    {
        timer += Time.deltaTime;    
        if (timer>=daytime)//说明过了一秒
        {
            ticks += 86400;
            timer = 0;
            //触发时间变化的事件 
            EventCenter.Instance.EventTrigger(GameEvent.日期每日更新变化);
        }
    }
}
