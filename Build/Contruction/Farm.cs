using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// 农场的基类 
/// </summary>
public class Farm : BuildItemBase
{
    //1,2,3阶段
    protected GameObject jieduan1Obj;
    protected GameObject jieduan2Obj;
    protected GameObject jieduan3Obj;
    void Awake()
    {
        jieduan1Obj = transform.Find("1").gameObject;
        jieduan2Obj = transform.Find("2").gameObject;
        jieduan3Obj = transform.Find("3").gameObject;
        Init();
        Qiehuanjieduan(1);
        EventCenter.Instance.AddEventListener(GameEvent.日期每日更新变化,TurnDay);
    }

    /// <summary>
    /// 重写每天更新的方法  
    /// </summary>
    public override void TurnDay()
    {
        //更新canproduct
        IsMaterialEnough = isMaterialEnough();
        IsMoneyEnough = isMoneyEnough();
        canProduct = IsMaterialEnough&&IsMoneyEnough?true:false;
        //先判断是否能正常产出 
        if (!canProduct) return;
        shouhuoTime--;
        //判断当前是否过了生长周期 默认没有过 就是false  过了就是true  
        if (!isOverShengzhangqi)
        {
            if (currentTime >= shengzhangzhouqi)
            {
                isOverShengzhangqi = true;
                currentTime = 0;
                return;
            }

            //在生长周期内不可能到成熟阶段 也就是第三阶段 所以最多只能到第二阶段 
            if (currentTime >= jieduan2) Qiehuanjieduan(2);
            currentTime++; //因为没有过生长周期 所以下面有关成熟操作不会执行  
            return;
        }

        //说明过了生长周期了 可以开始正常产出物品  
        //切换阶段模型   
        if (currentTime >= jieduan2 && currentTime < jieduan3)
        {
            Qiehuanjieduan(2);
        }
        else if (currentTime >= jieduan3)
        {
            Qiehuanjieduan(3);
        }

        //判断是否到了产出时间  就要开始计算下一轮周期
        Dictionary<string, int> productdict = GameManager.Instance.buildItemDict[buildId].product;
        if (currentTime >= ripeningTime)
        {
            shouhuoTime = ripeningTime;
            currentTime = 0;
            //增加产出物品  
            //增加产出  更新背包数据  
            GameManager.Instance.currentKnapsack.productDict[currentProductItemId] += productdict[currentProductItemId];
            //背包数据发生变化了 执行事件
            EventCenter.Instance.EventTrigger(GameEvent.背包数据变化);
            return;
        }
        currentTime++;
    }





    /// <summary>
/// 切换阶段的方法 
/// </summary>
/// <param name="jieduan"></param>
    private void Qiehuanjieduan(int jieduan)
    {
        dangqianjieduan = jieduan;
        switch (jieduan)
        {
            case 1:
                jieduan1Obj.SetActive(true);
                jieduan2Obj.SetActive(false);
                jieduan3Obj.SetActive(false);
                break;
            case 2:
                jieduan1Obj.SetActive(false);
                jieduan2Obj.SetActive(true);
                jieduan3Obj.SetActive(false);    
                break;
            case 3:
                jieduan1Obj.SetActive(false);
                jieduan2Obj.SetActive(false);
                jieduan3Obj.SetActive(true);
                break;
        }
    }

    private void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener(GameEvent.日期每日更新变化,TurnDay);
    }
}
