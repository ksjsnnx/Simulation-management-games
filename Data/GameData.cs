using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// 服务器数据对象 
/// </summary>
public class ServerData  
{
    public int ServerId;
    public string ServerIp;
    public string State;
    public string ServerName;
}
/// <summary>
/// 土地属性信息  
/// </summary>
public class GroundPropertyData
{
    //0未购买  1购买 2 已建造 
    public int State = 0;
    public int Price = 200;
    public string GroundName;
    public bool isShowInitPrefab;
    public string buildId;//建造物id  
}
/// <summary>
/// 建造物的信息 
/// </summary>
public class BuildItemData
{
    public string id;
    public string type;
    public string name;
    public string prefab;
    public int price;
    public int keepCost;
    public int ripeningTime;
    [JsonConverter(typeof(ArrayToDictionaryConverter<string, int>))]
    public Dictionary<string, int> product;
    public int firstGrowTime;
    public string sprite;
    public string decription;
    public int jieduan2;//到达阶段2时间
    public int jieduan3;//到达阶段3时间    
}

/// <summary>
/// 玩家的数据 
/// </summary>
public class PlayerData
{
    private int gameLevel;

    public int GameLevel
    {
        get{return gameLevel;}
        set
        {
            gameLevel = value;
            EventCenter.Instance.EventTrigger(GameEvent.玩家等级变化);
        }
    }
    private float coin;
    //玩家的金币 
    public float Coin
    {
        get { return coin; }
        set //值发生改变的时候触发金币改变的事件 
        {
            //触发金币变化的事件
            coin = value;
            EventCenter.Instance.EventTrigger(GameEvent.金币变化);
        }
    }

}
/// <summary>
/// 产出物品的数据对象 
/// </summary>
public class ProductItemData
{
    public string id;//物品的id
    public string name;//物品的名字
    public string unit;//物品的单位 
    public string description;//物品的描述
    public string sprite;//物品的图片
    public int price;//物品的价值
}
//背包的数据对象  
public class KnapsackData
{
    //所有物品id 和数量 一开始默认都为0  
    public Dictionary<string, int> productDict=new Dictionary<string, int>();
    //装备 
}

/// <summary>
/// 消耗物品的数据对象
/// </summary>
public class XiaohaoItemData
{
    public string id;//主键id  
    public string buildid;//建造物id
    public string productid;//产出物品id
    [JsonConverter(typeof(ArrayToDictionaryConverter<string, int>))]
    public Dictionary<string, int> xiaohaoDict;//产出物品每天消耗的物品的字典  
}
/// <summary>
/// 售卖商品信息数据对象
/// </summary>
public class MerchantingData
{
    public string productid;//产品的id  产品数据表中的id  
    public int maxCount;//购买的最大数量
    public int gameLevel;//当前售卖的物品在哪个等级可以进行售卖  游戏的等级 
    
}
/// <summary>
/// 对话数据信息对象 
/// </summary>
public class DialogueItemData
{
    public string id;//对话id
    public string targetName;//对话目标的名字
    public string targetIcon;//对话目标的图片
    public string dialogueContent; //对话内容
    public string nextId;//下一个对话的id 
    public List<string> optionList;//选项列表 
    public string taskId;//任务id  
}
/// <summary>
/// 任务基本数据信息
/// </summary>
public class TaskItemData
{
    public string id;//任务id
    public bool isStarted;//任务是否开始中
    public bool isEnded;//任务是否结束，完成任务触发奖励机制
    public bool isFinished;//触发奖励后显示任务已完成完成
    [JsonConverter(typeof(ArrayToDictionaryConverter<string, string>))]
    public Dictionary<string, string> demandDict;//需求字典，键为需求类型，值为需求id  
    public string type;//任务类型  主线任务和支线任务
    public int level;//该任务所处的游戏等级
}
/// <summary>
/// 通过判断数量的需求数据信息 
/// </summary>
public class NumDemandData
{
    public string id;//需求id  
    public string itemId;//物品id  
    public int currentNum;//当前完成数量
    public int itemNum;//需要完成物品的数量   
    public string itemType;//物品类型  建造物，金币，稀有物品  
    public string descripe;//需求描述   

}
/// <summary>
/// 普通建造物存档信息  
/// </summary>
public class SampleBuildingData
{
    public int dangqianjieduan;//当前阶段
    public int currentTime;//当前时间 
    public bool isOverShengzhangqi;//是否过了生长周期 
    public int shouhuoTime;//收获时间 
    public bool IsMaterialEnough;//材料是否充足 
    public bool IsMoneyEnough;//金币是否充足  
    public bool canProduct;//能否建造 
    public string currentProductItemId;//当前产出物品id  
    public bool isAdult;//是否成年  
    public string groundName;//所在土地块的名字 
    
}
/// <summary>
/// 商店建造物存档信息  
/// </summary>
public class ShopBuldingData
{
    public float priceRate;//当前价格比例
    public int level;//当前商店等级 
    public float Incom;//总收入 
    public bool IsMoneyEnough;//金币是否充足  
    public string groundName;//对应所在土地块的名字

}
/// <summary>
/// 主场景游戏数据信息 
/// </summary>
public class MainSceneData
{
    public string name;//存档名字
    public string sceneName;//游戏场景名
    public PlayerData playerData;//玩家信息  
    public KnapsackData knapsackData;//背包数据 
    public long ticks;//时间戳 
    public Dictionary<string, GroundPropertyData> currentGroundDict;//键为土地块的名字 值为土地的信息  状态不为0的土地 
    //普通建造物信息  
    public Dictionary<string,SampleBuildingData> sampleBuildingDict;//键为土地块名字，值为所建造的建造物信息   
    //商店建造物信息 
    public Dictionary<string,ShopBuldingData> shopBuldingDict;//键为土地块名字，值为所建造的建造物信息  
    // //任务数据 
    public Dictionary<string,TaskItemData> taskItemDataDict;//键值为人物id，值为任务数据  
    //需求数据 列表 
    public List<NumDemandData> numDemandList;  
    

}