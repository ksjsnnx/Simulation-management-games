using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 任务项
/// </summary>
public class TaskItem : MonoBehaviour
{
    [Header("任务需求描述")] public Text taskDes;
    [Header("需要完成物品的图标")] public Image ItemImg;
    [Header("当前完成数量")] public Text currentCountText;
    [Header("需要完成的数量")] public Text CountText;

    [Header("打勾图标")] public Image isFinishIcon;

    //需求信息  
    public NumDemandData currentDemandData;

    //任务数据  
    public TaskItemData taskItemData;

    void Awake()
    {
        EventCenter.Instance.AddEventListener(GameEvent.建造物数量变化,UpdateNumData);
        EventCenter.Instance.AddEventListener(GameEvent.金币变化,UpdateNumData);
       
    }
/// <summary>
/// 任务结束要执行的方法  领取奖励
/// </summary>
    public void taskOver()
    {
    
        taskItemData.isEnded = true;
        GameManager.Instance.taskItemDict[taskItemData.id].isEnded =true;
    }

    /// <summary>
/// 完成任务要执行的方法
/// </summary>
    public void FinishTask()
    {
        //获得奖励后更改任务完成状态    
        taskItemData.isFinished = true;
        GameManager.Instance.taskItemDict[taskItemData.id].isFinished=true;
        //更新✔图标 
        UpdateCheckIcon();
        //执行任务完成的事件 
        EventCenter.Instance.EventTrigger(GameEvent.任务完成事件);
   
    }

    private void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener(GameEvent.建造物数量变化,UpdateNumData);
        EventCenter.Instance.RemoveEventListener(GameEvent.金币变化,UpdateNumData);
    }

    /// <summary>
    /// 需求信息   
    /// </summary>
    /// <param name="id"></param>
    public void updateTaskItem(string id)
    {
        //拿到任务信息  
        taskItemData = GameManager.Instance.taskItemDict[id];
        //拿到需求信息  
        currentDemandData = GameManager.Instance.getNumDemandData(taskItemData.demandDict["numdemand"]);
        taskDes.text = currentDemandData.descripe;
        switch (currentDemandData.itemType)
        {
            case "building":
                BuildItemData build=GameManager.Instance.buildItemDict[currentDemandData.itemId];
                ItemImg.sprite = ResMgr.Instance.load<Sprite>("Icon/" + build.sprite);
                break;
            case "coin":
                ItemImg.sprite=ResMgr.Instance.load<Sprite>("Icon/Money");
                break;
        }
        CountText.text=currentDemandData.itemNum.ToString();
        //更新数量信息
        UpdateNumData();
        //更新任务完成状态信息   
        UpdateCheckIcon();
    }
/// <summary>
/// 更新数量信息  
/// </summary>
    public void UpdateNumData()
    {
        switch (currentDemandData.itemType)
        {
            case "building":
                //拿到对应建筑物的数量   
                BuildItemData build = GameManager.Instance.buildItemDict[currentDemandData.itemId];
                int num=BuildController.Instance.currentBuildingDict==null?0:BuildController.Instance.currentBuildingDict[build.id].Count;
                currentCountText.text=num.ToString();
                currentDemandData.currentNum = num;
                GameManager.Instance.getNumDemandData(currentDemandData.id).currentNum=currentDemandData.currentNum;

                if (num>=currentDemandData.itemNum&&!taskItemData.isEnded)
                {
                    //任务结束 
                    EventCenter.Instance.EventTrigger<TaskItem>(GameEvent.任务结束事件,this);
                }
           
                break;
            case "coin":
                currentCountText.text=GameManager.Instance.playerData.Coin.ToString();
                currentDemandData.currentNum=(int)GameManager.Instance.playerData.Coin ;  
                GameManager.Instance.getNumDemandData(currentDemandData.id).currentNum=currentDemandData.currentNum;
                if ((int)GameManager.Instance.playerData.Coin>currentDemandData.itemNum&&!taskItemData.isEnded)
                {
                    //任务结束
                    EventCenter.Instance.EventTrigger<TaskItem>(GameEvent.任务结束事件,this);
                }
                break;
            
        }
       
    }
/// <summary>
/// 更新完成的打勾图标 
/// </summary>
    private void UpdateCheckIcon()
    {
        if (taskItemData.isFinished)
        {
            isFinishIcon.enabled = true;
        }
        else
        {
            isFinishIcon.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}