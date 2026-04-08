using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 游戏事件 
/// </summary>
public enum GameEvent
{
    进度条加载,
    建造物品成功,
    土地状态变化,
    日期每日更新变化,
    金币变化,
    背包数据变化,
    产出物品id变化,
    玩家等级变化,
    切换下一条对话语句,
    任务UI切换,
    任务开始事件,
    任务结束事件,
    任务完成事件,
    建造物数量变化,
    拆除建造物,
    对话开始,
    对话结束
}
/// <summary>
/// UI动画切换类型 
/// </summary>
public enum UIAnimaEvent
{
    渐变,
    默认
}
/// <summary>
/// 存档类型 
/// </summary>
public enum ArchiveType
{
    上一次游戏=1,
    新游戏存档=2
}
