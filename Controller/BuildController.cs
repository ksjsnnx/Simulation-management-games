using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildController : UnitySingleTon<BuildController>
{
    //当前选中的土地  
    public GroundProperties currentGround;

    //选中的提示对象 
    private GameObject selectGroundTip;

    //建造字典  
    public Dictionary<string, List<BuildItemBase>> currentBuildingDict; //当前所有建造物的字典 
    //当前所有状态不为0的土地信息  
    public Dictionary<string, GroundPropertyData> currentGroundDict;

    public void Start()
    {
        currentGroundDict=new Dictionary<string, GroundPropertyData>(); 
        //初始化建造字典信息  
        buildingDictDataInit();
        selectGroundTip = GameObject.Find("CurrentSelectTip");
        EventCenter.Instance.AddEventListener<int>(GameEvent.土地状态变化, groundStateChanged);
        EventCenter.Instance.AddEventListener(GameEvent.对话开始, () => { selectGroundTip?.SetActive(false); });
        EventCenter.Instance.AddEventListener(GameEvent.对话结束, () => { selectGroundTip?.SetActive(true); });
    }


    private void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener<int>(GameEvent.土地状态变化, groundStateChanged);
        EventCenter.Instance.RemoveEventListener(GameEvent.对话开始, () => { selectGroundTip?.SetActive(false); });
        EventCenter.Instance.RemoveEventListener(GameEvent.对话结束, () => { selectGroundTip?.SetActive(true); });
    }
    
    
/// <summary>
/// 初始化建造字典信息 
/// </summary>
    private void buildingDictDataInit()
    {
        currentBuildingDict=new Dictionary<string, List<BuildItemBase>>();
        foreach (var data in GameManager.Instance.buildItemDict.Values)
        {
            currentBuildingDict.Add(data.id,new List<BuildItemBase>());
        }
       
    }
/// <summary>
/// 添加建筑物
/// </summary>
/// <param name="id">建筑物id</param>
/// <param name="buildItem">建筑物对象</param>
    public void addBuilding(string id, BuildItemBase buildItem)
    {
        if (currentBuildingDict.ContainsKey(id))
        {
            //判断建筑物中没有该建筑对象，没有的话就添加
            if (!currentBuildingDict[id].Contains(buildItem))
            {
                currentBuildingDict[id].Add(buildItem);
                EventCenter.Instance.EventTrigger(GameEvent.建造物数量变化);
            }
            
        }
    }
/// <summary>
/// 移除建筑物  
/// </summary>
/// <param name="id"></param>
/// <param name="buildItem"></param>
    public void removeBuilding(string id,BuildItemBase buildItem)
    {
        if (currentBuildingDict.ContainsKey(id))
        {
            //判断建筑物中是否有该建筑对象，有的话就移除
            if (currentBuildingDict[id].Contains(buildItem))
            {
                currentBuildingDict[id].Remove(buildItem);
                currentGround = buildItem.transform.parent.GetComponent<GroundProperties>();
                currentGround.groundProperty.State=1;
                Destroy(buildItem.gameObject);//销毁建造对象
                EventCenter.Instance.EventTrigger<int>(GameEvent.土地状态变化,1);
                EventCenter.Instance.EventTrigger(GameEvent.拆除建造物);
                EventCenter.Instance.EventTrigger(GameEvent.建造物数量变化);
            }
        }
    }
    private void groundStateChanged(int state)
    {
        switch (state)
        {
            case 1:
                //显示已购买的物体  
                GameObject hasBuyObj = ResMgr.Instance.load<GameObject>("HasBuyGroundObj", currentGround.transform);
                hasBuyObj.transform.localPosition = new Vector3(0, 0.5f, 0);
                //隐藏初始化的预制体 比如土地上原有的草木  
                currentGround.transform.Find("InitPrefab").gameObject.SetActive(false);
                currentGround.groundProperty.isShowInitPrefab = false;
                currentGround.groundProperty.buildId = "0";
                //添加已购买土地的信息 
                if (currentGroundDict.ContainsKey(currentGround.name))
                {
                    currentGroundDict[currentGround.name] = currentGround.groundProperty;
                }
                else
                {
                    currentGroundDict.Add(currentGround.name,currentGround.groundProperty);
                }
                break;
            case 2:
                //隐藏已购买显示的预制体克隆的对象   
                Transform Obj = currentGround.transform.Find("HasBuyGroundObj");
                if (Obj != null) Destroy(Obj.gameObject);
                //添加已购买的土地
                if (currentGroundDict.ContainsKey(currentGround.name))
                {
                    currentGroundDict[currentGround.name] = currentGround.groundProperty;
                }
                else
                {
                    currentGroundDict.Add(currentGround.name,currentGround.groundProperty);
                }
                break;
            case 0:
                //显示初始化的预制体 比如土地上原有的草木  
                currentGround.transform.Find("InitPrefab").gameObject.SetActive(true);
                currentGround.groundProperty.isShowInitPrefab = true;
                //移除土地信息    
                if(currentGroundDict.ContainsKey(currentGround.name)) currentGroundDict.Remove(currentGround.name);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //判断当前射线是否在UI层 如果UI上就直接return  
        if (EventSystem.current.IsPointerOverGameObject()) return;
        //编写购买的功能  
        //从屏幕点发射射线 
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //碰撞信息
        RaycastHit hitInfo;
        //射线和土地的碰撞检测  
        if (Physics.Raycast(ray, out hitInfo, 1000))
        {
            //判断碰撞的物体是不是土地
            if (hitInfo.collider.tag == "Ground")
            {
                //保存当前选中的土地 
                currentGround = hitInfo.collider.gameObject.GetComponent<GroundProperties>();
                //让提示框跟随到鼠标的位置 
                //让提示框跟随到鼠标的位置 
                selectGroundTip.transform.localPosition = hitInfo.collider.gameObject.transform.localPosition;
                selectGroundTip.transform.localScale = hitInfo.collider.gameObject.transform.localScale;
            }
        }

        //鼠标左键点击 要显示当前点到的土地  
        if (Input.GetMouseButtonDown(0) && currentGround != null)
        {
            switch (currentGround.groundProperty.State) 
            {
                case 0:
                    //显示购买的窗口  
                    UIManager.Instance.openPanel<BuyGroundPanel>();
                    break;
                case 1:
                    //已购买 显示建造的窗口   建造对应的工厂 农场 各种铺子     
                    UIManager.Instance.openPanel<BuildPanel>();
                    break;
                case 2:
                    //已经建造  显示详情,拆除面板     
                    //拿到建造物类型显示对应的详情面板  
                    string type = currentGround.transform.Find("Building").GetComponent<BuildItemBase>().BuildType;
                    if (type == "other")
                    {
                      
                        UIManager.Instance.closePanel<BuildItemDetailPanel>();
                        var shopDetailPanel=UIManager.Instance.openPanel<ShopDetailPanel>();//打开商品界面
                        shopDetailPanel?.Init();//刷新下该面板的信息
                       
                    }
                    else
                    {
                        UIManager.Instance.closePanel<ShopDetailPanel>();
                        var buildItemDetailPanel=UIManager.Instance.openPanel<BuildItemDetailPanel>();//打开建造详情面板
                        buildItemDetailPanel?.Init();//刷新该面板信息
                       
                    }

                    break;
            }
        }
    }
}