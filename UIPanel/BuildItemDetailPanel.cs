using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BuildItemDetailPanel : BasePanel
{
    public GameObject ProductContent;//产出物品的容器 
    public GameObject productItemPanel;
    public Text Title;
    public Text productName;
    public Image productIcon;
    [Header("更换的按钮")]
    public Button changeProductBtn;
    public Text productDescription;
    public Text productTimeDescription;
    public Text changben;
    public Button chaichuBtn;
    public GameObject xiaohao;
    public GameObject state;
    public Text canProductText;
    public Text isAdultText;
    public Text moneyStateText;
    public BuildItemBase currentBuildItem; //当前点到的建造物  
    private bool isOpen = false;

    private void OnEnable()
    {
        StartCoroutine(DelayedLayoutUpdate());
        Init();
        UpdateData();//更新收获时间的信息 
    }
    public void Init()//初始化详情面板 就是每次打开详情面板的时候要初始化 
    {
        productItemPanel.gameObject.SetActive(false);
        isOpen = true;
        //当前的建造物  
        currentBuildItem = BuildController.Instance.currentGround.transform.Find("Building")
            .GetComponent<BuildItemBase>();
        //更换按钮逻辑
        changeBtnInit();
        //拿到当前的建造信息 
        BuildItemData buildItemData = GameManager.Instance.buildItemDict[currentBuildItem.buildId];
        //产出物品的字典
        Dictionary<string, ProductItemData> productDict = GameManager.Instance.productItemDict;
        //拿到产出物品
        ProductItemData currentProdcut = productDict[currentBuildItem.currentProductItemId];
        //产品信息更新    
        UpdateProductData(currentProdcut);
        //产出效率信息更新  
        UpdateProductEfficient(buildItemData,currentProdcut);         
        Title.text = buildItemData.name;
       
     
        productTimeDescription.text = $"距离收获还有{currentBuildItem.shouhuoTime}天";
        changben.text = buildItemData.keepCost.ToString();
        //初始化消耗物品的信息  
        updateXiaohaoItem(currentBuildItem.buildId,currentBuildItem.currentProductItemId);
        //更新状态栏内容
        UpdateState();
    }
/// <summary>
/// 更新产出效率的信息
/// </summary>
    private void UpdateProductEfficient(BuildItemData buildItemData,ProductItemData currentProdcut)
    {
        productDescription.text =
            $"每{currentBuildItem.ripeningTime}天产出{buildItemData.product[currentBuildItem.currentProductItemId]}{currentProdcut.unit}{currentProdcut.name}";
    }
/// <summary>
/// 更新产出物品的信息 
/// </summary>
    private void UpdateProductData(ProductItemData currentProdcut)
    {
        productName.text = currentProdcut.name;
        productIcon.sprite = ResMgr.Instance.load<Sprite>("Icon/" + currentProdcut.sprite);
    }

    /// <summary>
/// 更换按钮初始化  
/// </summary>
    private void changeBtnInit()
    {
        if (currentBuildItem.produtctDict.Count>1)
        {
            changeProductBtn.gameObject.SetActive(true);     
            changeProductBtn.onClick.AddListener(openProductItemPanel);
        }
        else
        {
            changeProductBtn.gameObject.SetActive(false);
        }
    }
/// <summary>
/// 打开产出物品的列表选项面板
/// </summary>
    private void openProductItemPanel()
    {
        Dictionary<string, int> productDict = currentBuildItem.produtctDict;
        if (productDict == null) return;
        productItemPanel.gameObject.SetActive(true);
        //更新内容  
        //先清除原来的物品项  
        for (int i = 0; i < ProductContent.transform.childCount; i++)
        {
            Destroy(ProductContent.transform.GetChild(i).gameObject);
        }
        //克隆新的物品项 放到productcontent中 
        foreach (string id in productDict.Keys)
        {
            GameObject obj=ResMgr.Instance.load<GameObject>("UI/ProductItemDetail",ProductContent.transform);    
            obj.GetComponent<ProductItemDetail>().UpdateData(GameManager.Instance.productItemDict[id],productDict[id],currentBuildItem);
        }
    }


    /// <summary>
    /// 更新消耗物品的信息
    /// </summary>
    /// <param name="buildId"></param>
    /// <param name="currentProductItemId"></param>
    private void updateXiaohaoItem(string buildId, string currentProductItemId)
    {
        //先清除原来的消耗的物品信息 
        for (int i = 0; i < xiaohao.transform.childCount; i++)
        {
            Destroy(xiaohao.transform.GetChild(i).gameObject);
        }

        //显示消耗物品  
        //拿到消耗物品的字典 
        Dictionary<string, int> xiaohaoDict = GameManager.Instance.getxiaohaoItemDict(buildId, currentProductItemId);
        if (xiaohaoDict != null)
        {
            foreach (var id in xiaohaoDict.Keys)
            {
                //克隆消耗物品到xiaohao 下面 
                GameObject xiaohaoItem = ResMgr.Instance.load<GameObject>("UI/XiaoHaoItem", xiaohao.transform);
                xiaohaoItem.GetComponent<XiaoHaoItem>()
                    .updateData(GameManager.Instance.productItemDict[id], xiaohaoDict[id]);
            }
        }
    }

    /// <summary>
    /// 更新状态栏内容    农场 显示金币是否足够的状态  牧场 显示三个信息 1.耗材 2.金币 3.是否成年   工厂 显示两个信息 1.耗材  2.金币   
    /// </summary>
    private void UpdateState()
    {
        state.SetActive(true);
        //金币的状态 
        if (currentBuildItem.IsMoneyEnough)
        {
            moneyStateText.text = "金币充足";
            moneyStateText.color = Color.black;
        }
        else
        {
            moneyStateText.text = "金币不足！！！";
            moneyStateText.color = Color.red;
        }

        isAdultText.gameObject.SetActive(true);
        canProductText.gameObject.SetActive(true); 
        switch (currentBuildItem.BuildType)
        {
           case "pasture":
               UpdateHaocaiDetail();
               isAdultText.text = currentBuildItem.isAdult ? "已成年" : "未成年";
               break;
           case "factory":
               UpdateHaocaiDetail();
               isAdultText.gameObject.SetActive(false);
               break;
           default:
               isAdultText.gameObject.SetActive(false);
               canProductText.gameObject.SetActive(false);  
               break;
        }
    }
    /// <summary>
    /// 更新耗材信息的方法 
    /// </summary>
    public void UpdateHaocaiDetail()
    {
        canProductText.color = Color.black;
        if (currentBuildItem.allXiaohaoDict==null)
        {
            canProductText.text = "无需耗材";
            return;
        }

        if (currentBuildItem.IsMaterialEnough)
        {
            canProductText.text = "材料充足";
        }
        else
        {
            canProductText.text = "耗材不足";
            canProductText.color = Color.red;
        }
    }
    private void OnDisable()
    {
        StopCoroutine(DelayedLayoutUpdate());
        isOpen = false;
        changeProductBtn.onClick.RemoveAllListeners();
    }

    void Start()
    {
        chaichuBtn.onClick.AddListener(()=>{BuildController.Instance.removeBuilding(currentBuildItem.buildId,currentBuildItem);});
        EventCenter.Instance.AddEventListener(GameEvent.拆除建造物,chaichu);
        EventCenter.Instance.AddEventListener(GameEvent.日期每日更新变化, UpdateDayData);
        EventCenter.Instance.AddEventListener(GameEvent.产出物品id变化,UpdateData);
        
    }
/// <summary>
/// 拆除建造物  
/// </summary>
    private void chaichu()
    {
        UIManager.Instance.closePanel<BuildItemDetailPanel>();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();   
        chaichuBtn.onClick.RemoveAllListeners();    
        EventCenter.Instance.RemoveEventListener(GameEvent.日期每日更新变化, UpdateDayData);
        EventCenter.Instance.RemoveEventListener(GameEvent.产出物品id变化,UpdateData);
    }
    // Update is called once per frame

/// <summary>
/// 每天更新的方法 
/// </summary>
    private void UpdateDayData()
    {
        if (!isOpen) return;//只有面板打开的时候才需要去刷新信息 
        productTimeDescription.text = $"距离收获还有{currentBuildItem.shouhuoTime}天";
        UpdateState();
    }
/// <summary>
/// 产品更换后要更新的方法  
/// </summary>
    private void UpdateData()
    {
        productItemPanel.gameObject.SetActive(false);
        //拿到当前的建造信息 
        BuildItemData buildItemData = GameManager.Instance.buildItemDict[currentBuildItem.buildId];
        //产出物品的字典
        Dictionary<string, ProductItemData> productDict = GameManager.Instance.productItemDict;
        //拿到产出物品
        ProductItemData currentProdcut = productDict[currentBuildItem.currentProductItemId];
        //产品信息更新 
        UpdateProductData(currentProdcut);
        //产出效率更新 
        UpdateProductEfficient(buildItemData,currentProdcut);
        //消耗物品更新 
        updateXiaohaoItem(currentBuildItem.buildId,currentBuildItem.currentProductItemId);
        
    }
}