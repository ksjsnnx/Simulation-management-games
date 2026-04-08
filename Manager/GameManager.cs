using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

public class GameManager : UnitySingleTonMono<GameManager>
{
    public PlayerData playerData; //玩家数据 

    //建造物品的信息的字典  
    public Dictionary<string, BuildItemData> buildItemDict = new Dictionary<string, BuildItemData>();

    //时间模块 
    public TimeController gameTimeController;

    //产品的字典对象  保存所有的产品的信息 
    public Dictionary<string, ProductItemData> productItemDict = new Dictionary<string, ProductItemData>();

    //当前背包中的数据 
    public KnapsackData currentKnapsack;

    //消耗物品的列表 
    private List<XiaohaoItemData> xiaohaoList = new List<XiaohaoItemData>();

    //商店售卖商品的列表
    public List<MerchantingData> merchantingList = new List<MerchantingData>();

    //对话数据的字典 
    public Dictionary<string, DialogueItemData> dialogueItemDict = new Dictionary<string, DialogueItemData>();

    //任务需求列表(判断数量) 
    public List<NumDemandData> numDemandList = new List<NumDemandData>();

    //任务字典  
    public Dictionary<string, TaskItemData> taskItemDict = new Dictionary<string, TaskItemData>();
    //主场景信息 
    public MainSceneData mainSceneData;
    void Start()
    {
        // MainSceneData mainSceneData=new MainSceneData();
        // mainSceneData.taskItemData = new TaskItemData();
        // mainSceneData.taskItemData.demandDict=new Dictionary<string, string>();
        // mainSceneData.taskItemData.demandDict.Add("1","需求一");
        // JsonMgr.Instance.SaveData(mainSceneData,"main");
        // MainSceneData newMain=JsonMgr.Instance.LoadData<MainSceneData>("main");
        //打开初始化面板
        UIManager.Instance.openPanel<StartPanel>();
    }

    /// <summary>
    /// 主场景游戏数据初始化 
    /// </summary>
    public void MainSceneInit()
    {
        //先初始化所有数据 
        gameDataInit();
        //初始化时间模块 
        gameTimeInit();

        //UI初始化 
        // UIManager.Instance.openPanel<LoginPanel>();
        //读档存
    }

    /// <summary>
    /// 初始化时间模块
    /// </summary>
    private void gameTimeInit()
    {
        
        gameTimeController = new TimeController();
        if(mainSceneData!=null) gameTimeController.ticks=mainSceneData.ticks;   
        //给时间模块中的updateTime 添加监听到公共mono模块中
        MonoMgr.Instance.addUpdateListener(gameTimeController.UpdateTime);
    }

    /// <summary>
    /// 游戏数据初始化的方法
    /// </summary>
    public void gameDataInit()
    {
       
        //初始化物品建造信息
        buildItemDataInit();
        //初始化产出物品信息 放到字典中方便我们后续拿到产品的信息 
        productItemDataInit();
        //初始化消耗物品的列表
        xiaohaoItemDataInit();
        //初始化对话数据字典信息
        dialogueItemDataInit();
        //初始商店商品的字典数据 
        merchantingDataInit();
        //受存档影响的数据   
        
        //初始化玩家信息
        playerDataInit();
        //初始化背包数据 这里先直接初始化，后面要判断当前是新开游戏还是读取某个存档信息来初始化对应的背包数据
        knapsackDataInit();
        //初始化任务数据 
        taskDataInit();
    }
    /// <summary>
    /// 初始化玩家数据
    /// </summary>
    private void playerDataInit()
    {
        if (mainSceneData != null)
        {
            playerData = mainSceneData.playerData;  
            return;
        }
        playerData = new PlayerData { Coin = 99999999, GameLevel = 0 };
    }

    /// <summary>
    /// 任务数据初始化
    /// </summary>
    private void taskDataInit()
    {
        if (mainSceneData != null)
        {
            taskItemDict = mainSceneData.taskItemDataDict;
            numDemandList= mainSceneData.numDemandList; 
            return;
        }
        taskItemDict = new Dictionary<string, TaskItemData>();
        //初始化需求列表 
        numDemandList = JsonMgr.Instance.LoadData<List<NumDemandData>>("tbnumdemand");
        List<TaskItemData> taskItemList = JsonMgr.Instance.LoadData<List<TaskItemData>>("tbtask");
        foreach (TaskItemData taskItemData in taskItemList)
        {
            taskItemDict.Add(taskItemData.id, taskItemData);
        }
    }

    /// <summary>
    /// 初始化背包的数据 
    /// </summary>
    private void knapsackDataInit(KnapsackData knapsackData = null)
    {
        if (mainSceneData != null)
        {
            currentKnapsack = mainSceneData.knapsackData;   
            return;
        }
        currentKnapsack = new KnapsackData();
        if (knapsackData != null)
        {
            //读档  
            currentKnapsack = knapsackData;
        }
        else //新开游戏 初始化背包中的数据  
        {
            //初始化字典 
            foreach (var key in productItemDict.Keys)
            {
                currentKnapsack.productDict.Add(key, 0); //默认都是0  
            }
        }
    }
    /// <summary>
    /// 获取指定需求id的数量类型需求数据信息  
    /// </summary>
    /// <returns></returns>
    public NumDemandData getNumDemandData(string id)
    {
        foreach (var data in numDemandList)
        {
            if (data.id == id)
            {
                return data;
            }
        }

        return null;
    }

    private void dialogueItemDataInit()
    {
        dialogueItemDict = new Dictionary<string, DialogueItemData>();
        //对话数据列表 
        List<DialogueItemData> dialogueItemDataList =
            JsonMgr.Instance.LoadData<List<DialogueItemData>>("tbdialogueitem");
        //保存到字典中 
        foreach (var data in dialogueItemDataList)
        {
            dialogueItemDict.Add(data.id, data);
        }
    }

    /// <summary>
    /// 商品信息的初始化 
    /// </summary>
    private void merchantingDataInit()
    {
        merchantingList = JsonMgr.Instance.LoadData<List<MerchantingData>>("tbmerchantingitem");
    }

    /// <summary>
    /// 消耗物品的信息初始化
    /// </summary>
    private void xiaohaoItemDataInit()
    {
        xiaohaoList = JsonMgr.Instance.LoadData<List<XiaohaoItemData>>("tbxiaohaoitem");
    }

    /// <summary>
    /// 找到对应的消耗物品的字典 
    /// </summary>
    /// <param name="buildId"></param>
    /// <param name="productId"></param>
    /// <returns></returns>
    public Dictionary<string, int> getxiaohaoItemDict(string buildId, string productId)
    {
        foreach (XiaohaoItemData itemData in xiaohaoList)
        {
            if (itemData.buildid == buildId && itemData.productid == productId)
            {
                return itemData.xiaohaoDict;
            }
        }

        return null;
    }


    /// <summary>
    /// 产出物品信息初始化的方法  productItemDict  
    /// </summary>
    private void productItemDataInit()
    {
        productItemDict = new Dictionary<string, ProductItemData>();
        //获取数据表的数据 序列化成数据对象  列表  
        List<ProductItemData> productItemList = JsonMgr.Instance.LoadData<List<ProductItemData>>("tbproductitem");
        //放到字典中  
        foreach (ProductItemData itemData in productItemList)
        {
            productItemDict.Add(itemData.id, itemData);
        }
    }


    /// <summary>
    /// 建造数据的初始化    读取我们的json数据 tbbuilditem
    /// </summary>
    private void buildItemDataInit()
    {
      
        buildItemDict = new Dictionary<string, BuildItemData>();
        List<BuildItemData> buildItemDatas = new List<BuildItemData>();
        buildItemDatas = JsonMgr.Instance.LoadData<List<BuildItemData>>("tbbuilditem");
        foreach (BuildItemData item in buildItemDatas)
        {
            buildItemDict.Add(item.id, item);
           
        }
    }


    /// <summary>
    /// 保存存档信息
    /// 保存存档有两种 一种是每次游戏退出保存上一次游戏的存档信息，加载上一次游戏进度，每次保存固定加载    Archive/LastGame/Main.json  
    /// 第二种是保存对应游戏存档到archive文件夹下面   Archive/时间戳/Main.json  
    /// </summary>
    public void SaveArchive(ArchiveType type=ArchiveType.上一次游戏, string name = "上一次游戏存档")
    {
        //保存 主场景 MainSceneData的数据 
        mainSceneData = new MainSceneData();
        mainSceneData.name =name;
        mainSceneData.sceneName = "MainScene";
        mainSceneData.playerData = playerData;
        mainSceneData.knapsackData = currentKnapsack;
        mainSceneData.ticks=gameTimeController.ticks;
        mainSceneData.currentGroundDict = BuildController.Instance.currentGroundDict;
        mainSceneData.sampleBuildingDict = new Dictionary<string, SampleBuildingData>();
        mainSceneData.shopBuldingDict = new Dictionary<string, ShopBuldingData>();
        //保存普通建造物信息  //保存商店建造物信息  
        foreach (var value in BuildController.Instance.currentBuildingDict.Values)
        {
            foreach (BuildItemBase building in value)
            {
                if (building.BuildType.Trim() == "other")//商店 
                {
                    ShopBuldingData shopBuldingData=new ShopBuldingData();
                    Shop shop = (building as Shop);
                    shopBuldingData.priceRate = shop.priceRate;
                    shopBuldingData.level=shop.level;
                    shopBuldingData.Incom = shop.Incom;
                    shopBuldingData.IsMoneyEnough=shop.IsMoneyEnough;
                    shopBuldingData.groundName=shop.groundName;
                    if(!mainSceneData.shopBuldingDict.ContainsKey(shop.groundName))mainSceneData.shopBuldingDict.Add(shop.groundName,shopBuldingData);
                }
                else//普通建筑物 
                {
                    SampleBuildingData sampleBuildingData = new SampleBuildingData();
                    sampleBuildingData.dangqianjieduan=building.dangqianjieduan;
                    sampleBuildingData.currentTime = building.currentTime;
                    sampleBuildingData.isOverShengzhangqi=building.isOverShengzhangqi;  
                    sampleBuildingData.shouhuoTime=building.shouhuoTime;
                    sampleBuildingData.IsMaterialEnough=building.IsMaterialEnough;  
                    sampleBuildingData.IsMoneyEnough=building.IsMoneyEnough;
                    sampleBuildingData.canProduct=building.canProduct;  
                    sampleBuildingData.currentProductItemId=building.currentProductItemId;
                    sampleBuildingData.isAdult = building.isAdult;
                    sampleBuildingData.groundName=building.groundName;  
                    if(!mainSceneData.sampleBuildingDict.ContainsKey(sampleBuildingData.groundName))mainSceneData.sampleBuildingDict.Add(sampleBuildingData.groundName,sampleBuildingData); 
                }
            }
        }
        mainSceneData.taskItemDataDict = taskItemDict;//保存任务数据  
        mainSceneData.numDemandList=numDemandList; //保存需求数据信息  
        
        //根据不同类型 保存的地址会有不同 
        if (type == ArchiveType.上一次游戏) 
        {
            JsonMgr.Instance.SaveData(mainSceneData,"main","Archive/LastGame/");
        }
        else if (type == ArchiveType.新游戏存档)
        {
            JsonMgr.Instance.SaveData(mainSceneData,"main","Archive/EveryGame/"+mainSceneData.ticks+"/");
        }
    }

// Update is called once per frame
    void Update()
    {
    }
}