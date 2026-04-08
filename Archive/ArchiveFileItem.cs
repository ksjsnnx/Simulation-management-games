using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 存档项  
/// </summary>
public class ArchiveFileItem : MonoBehaviour
{
    private Button fileBtn;
    [Header("存档名字")]
    public Text archiveName;
    [Header("年月日")]
    public Text dateText;
    private MainSceneData mainSceneData;
    void Start()
    {
        fileBtn=GetComponent<Button>();         
        fileBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.closePanel<LoadFilePanel>();
            GameManager.Instance.mainSceneData = mainSceneData;
            //初始化主场景游戏数据  
            GameManager.Instance.MainSceneInit();
            //关闭开始面板 
            UIManager.Instance.closePanel<StartPanel>();
            //跳转到mainscene场景
            SceneMgr.Instance.LoadSceneAsync("MainScene", () =>
            {
                UIManager.Instance.closePanel<LoadPanel>();
                //玩家数据面板
                UIManager.Instance.openPanel<PlayerPropPanel>();
                //任务数据面板
                UIManager.Instance.openPanel<TaskPanel>();
                //初始化任务面板数据
                TaskManager.Instance.Init();    
                //初始化主场景中的游戏对象  
                //遍历所有土地块信息   更改土对应地块信息   
                foreach (var groundName in mainSceneData.currentGroundDict.Keys)
                {
                    //拿到对应土地块对象
                    GameObject obj = GameObject.Find(groundName);
                    print(obj);
                    if (obj != null)      
                    {
                        //更改对应土地块的信息
                        obj.GetComponent<GroundProperties>().groundProperty=mainSceneData.currentGroundDict[groundName];
                        BuildController.Instance.currentGround = obj.GetComponent<GroundProperties>();
                        if (mainSceneData.currentGroundDict[groundName].State == 2 &&
                            mainSceneData.currentGroundDict[groundName].buildId != "0")
                        {
                            string builId=mainSceneData.currentGroundDict[groundName].buildId;
                            //拿到对应建造物的信息  
                            BuildItemData buildItem = GameManager.Instance.buildItemDict[builId];
                            //克隆建造物体到对应的土地块上  
                            GameObject buildObj =
                                ResMgr.Instance.load<GameObject>($"Ground/{buildItem.prefab}", obj.transform);
                            buildObj.name = "Building";
                            buildObj.transform.localScale=Vector3.one;
                            buildObj.transform.localPosition=Vector3.zero;
                            BuildItemBase buildBase=buildObj.GetComponent<BuildItemBase>(); 
                            BuildController.Instance.addBuilding(builId,buildBase);
                            //根据土地块名字拿到对应的建筑信息  
                            if (buildItem.type=="other")//商店  
                            {
                                Shop shop = (buildBase as Shop);
                                ShopBuldingData shopBuldingData =mainSceneData.shopBuldingDict[groundName];
                                shop.priceRate=shopBuldingData.priceRate;   
                                shop.level=shopBuldingData.level;
                                shop.Incom = shopBuldingData.Incom;
                                shop.groundName=groundName;
                                shop.IsMoneyEnough=shopBuldingData.IsMoneyEnough;
                            }
                            else
                            {
                                SampleBuildingData sampleBuild = mainSceneData.sampleBuildingDict[groundName];
                                buildBase.dangqianjieduan=sampleBuild.dangqianjieduan;  
                                buildBase.currentTime=sampleBuild.currentTime;
                                buildBase.isOverShengzhangqi=sampleBuild.isOverShengzhangqi;    
                                buildBase.shouhuoTime=sampleBuild.shouhuoTime;
                                buildBase.IsMaterialEnough=sampleBuild.IsMaterialEnough;
                                buildBase.IsMoneyEnough=sampleBuild.IsMoneyEnough;  
                                buildBase.canProduct=sampleBuild.canProduct;    
                                buildBase.currentProductItemId=sampleBuild.currentProductItemId;
                                buildBase.isAdult = sampleBuild.isAdult;
                                buildBase.groundName = groundName;
                                buildBase.updateXiaoHaoDict(buildBase.buildId,buildBase.currentProductItemId);
                            }
                        }
                        //因为土地信息发生改变，所以这里要触发土地状态变化的方法 
                        EventCenter.Instance.EventTrigger(GameEvent.土地状态变化,mainSceneData.currentGroundDict[groundName].State);
                    } 
                }
                
            });
        });
    }
    
    private void OnEnable()
    {
        if (fileBtn != null) 
            fileBtn.onClick.RemoveAllListeners();
    }
/// <summary>
/// 更新文件信息  
/// </summary>
/// <param name="fileName"></param>
    public void UpdateData(MainSceneData data)  
    {
        mainSceneData = data;
        archiveName.text = mainSceneData.name;
        DateTimeOffset now=DateTimeOffset.FromUnixTimeSeconds(mainSceneData.ticks);
        dateText.text=string.Format("{0:yyyy/MM/dd}", now);
    }
    
}
