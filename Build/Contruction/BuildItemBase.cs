using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// 建造物的基类  
/// </summary>
public class BuildItemBase : MonoBehaviour
{
    [HideInInspector]
    public string groundName;//土地块名字 
    [HideInInspector]
    public bool isAdult;//是否成年 
    //是否能建造  需要有一个布尔值记录是否能正常产出 
    [HideInInspector]
    public bool canProduct;
    [HideInInspector]
    public bool IsMaterialEnough;//材料是否充足 
    [HideInInspector]
    public bool IsMoneyEnough;//金币是否充足 
    //建造的类型  
    [HideInInspector]
    public string BuildType;
    //当前产出物品的id  
    [HideInInspector]
    public string currentProductItemId;
    
    //生产周期 
    [HideInInspector]
    public int ripeningTime;

    //当前生产的时间  
    [HideInInspector]
    public int currentTime;

    //每日维护的价格
    [HideInInspector]
    public int weihuPrice;
    [HideInInspector]
    public int shengzhangzhouqi;//生长周期  
    [HideInInspector]
    public bool isOverShengzhangqi;//是否过了生长期
    [Header("建造物的id参考builditem数据表")] 
    public string buildId;
    [HideInInspector]
    public int dangqianjieduan = 1;
    protected int jieduan2;//到达阶段2的时间 
    protected int jieduan3;//到达阶段3的时间 
    [HideInInspector]
    public int shouhuoTime;//收获时间 
    //消耗物品的字典  可能这个建造物是不需要消耗任何东西的  那么这个字典有可能是空 
    public Dictionary<string,int> allXiaohaoDict=new Dictionary<string, int>();    
    //产出物品的字典  
    public Dictionary<string,int> produtctDict=new Dictionary<string, int>();
    public virtual void Init() //初始化 
    {
        //根据buildid拿到对应的建造物的信息 
        BuildItemData buildItemData = GameManager.Instance.buildItemDict[buildId];
        produtctDict = buildItemData.product;//拿到产出物品的字典 
        //初始化产出物品id   拿到当前要产出的物品的id  
        currentProductItemId = buildItemData.product.Keys.First();
        BuildType=buildItemData.type;
        ripeningTime = buildItemData.ripeningTime;
        currentTime = 0;
        weihuPrice = buildItemData.keepCost;
        jieduan2 = buildItemData.jieduan2;
        jieduan3 = buildItemData.jieduan3;
        isOverShengzhangqi = false;
        shengzhangzhouqi = buildItemData.firstGrowTime;//初次的生长周期时间  
        shouhuoTime = shengzhangzhouqi + ripeningTime;
        updateXiaoHaoDict(buildId, currentProductItemId);//初始化当前消耗的字典  
        IsMaterialEnough = isMaterialEnough();
        IsMoneyEnough=isMoneyEnough();
        //更新canproduct
        canProduct = IsMaterialEnough?true:false;
        
    }
    
    public void updateXiaoHaoDict(string buildId, string currentProductItemId)
    {
        allXiaohaoDict=GameManager.Instance.getxiaohaoItemDict(buildId, currentProductItemId);//初始化当前消耗的字典  
    }
    //每天要做的事情  每天过完之后会执行 6 7 
    public virtual void TurnDay()
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
                isAdult=true;
                currentTime = 0;
                return;
            }
            currentTime++; //因为没有过生长周期 所以下面有关成熟操作不会执行  
            return;
        }
        Dictionary<string,int> productdict=GameManager.Instance.buildItemDict[buildId].product;
        if (currentTime >= ripeningTime)
        {
            shouhuoTime = ripeningTime;
            currentTime = 0;
            //增加产出  更新背包数据  
                GameManager.Instance.currentKnapsack.productDict[currentProductItemId] += productdict[currentProductItemId];
            //背包数据发生变化了 执行事件
            EventCenter.Instance.EventTrigger(GameEvent.背包数据变化);
            return;
        }
            currentTime++;
  
    }
/// <summary>
/// 金币是否充足 
/// </summary>
    public virtual bool isMoneyEnough()
    {
        if (GameManager.Instance.playerData.Coin >= weihuPrice)
        {
            //消耗金币  
            GameManager.Instance.playerData.Coin -= weihuPrice;
            EventCenter.Instance.EventTrigger(GameEvent.金币变化);
            return true;
        }
        else
        {
            return false;
        }
    }
    
/// <summary>
/// 判断材料是否足够 
/// </summary>
/// <returns></returns>
/// <exception cref="NotImplementedException"></exception>
    public virtual bool isMaterialEnough()
    {
        if (allXiaohaoDict == null)
        {
            return true;
        }

        foreach (var id in allXiaohaoDict.Keys)
        {
            if (allXiaohaoDict[id]>GameManager.Instance.currentKnapsack.productDict[id])
            {
                return false;
            }
            GameManager.Instance.currentKnapsack.productDict[id] -= allXiaohaoDict[id];
        }
        return true;
    }
}