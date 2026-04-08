using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 商店建造物  比较特殊的  初始化的内容和别人不一样  每天要更新也和别人不一样  所以要对这两个方法进行重写  
/// </summary>
public class Shop : BuildItemBase
{
    //价格比例  
    [HideInInspector]
    public float priceRate;
    [HideInInspector]
    public int maxLevel;
    //商店的等级 
    [HideInInspector]
    public int level;
    //升级的价格 
    [HideInInspector]
    public int upgradePrice;
    //总收入 
    [HideInInspector]
    public float Incom;
    //每日产出的金币 
    [HideInInspector]
    public int chanchuCoin;
    //收益
    [HideInInspector]
    public float earnings;
    //商品的字典  
    public Dictionary<string, int> merchantingDict;
    //商店的名字 
    [HideInInspector]
    public string name;
    public virtual void Awake()
    {
        Init();
        EventCenter.Instance.AddEventListener(GameEvent.日期每日更新变化,TurnDay);
    }

    private void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener(GameEvent.日期每日更新变化,TurnDay);
        
    }

    public override void Init()
    {
        BuildItemData buildItemData = GameManager.Instance.buildItemDict[buildId];
        level = 1;
        priceRate = 1;
        upgradePrice = 10000;
        maxLevel = 3;
        weihuPrice=buildItemData.keepCost;
        chanchuCoin = 900;
        earnings=(chanchuCoin-weihuPrice)*priceRate;
        Incom += earnings;//总收入  
        BuildType = buildItemData.type;
        name=buildItemData.name;
        IsMoneyEnough = isMoneyEnough();
        canProduct=IsMoneyEnough?true:false;    
    }

    public override void TurnDay()
    {
        IsMoneyEnough = isMoneyEnough();
        canProduct = IsMoneyEnough?true:false;
        if (!canProduct) return;
        earnings=(chanchuCoin-weihuPrice)*priceRate;
        Incom += earnings;//总收入  
        GameManager.Instance.playerData.Coin += earnings;
        
    }
}
