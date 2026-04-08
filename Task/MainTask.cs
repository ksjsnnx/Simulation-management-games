using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainTask : Task
{
    [Header("要达到的等级图标")]
    public Image levelIcon;
    private void OnEnable()
    {
        taskType = "主线任务";
        UpdateGradeData();
        EventCenter.Instance.AddEventListener<string>(GameEvent.任务开始事件,updateTaskItem);
        EventCenter.Instance.AddEventListener(GameEvent.玩家等级变化,UpdateGradeData);
    }

    public override void Start()
    {
        base.Start();       
        
        
    }
//更新等级信息  
    public void UpdateGradeData()
    {
        int level = GameManager.Instance.playerData.GameLevel + 1;
        switch (level)
        {
            case 1:
                levelIcon.sprite = ResMgr.Instance.load<Sprite>("Icon/等级一");
                taskBtnText.text = "达到等级一目标";
                break;
            case 2:
                levelIcon.sprite = ResMgr.Instance.load<Sprite>("Icon/等级二");
                taskBtnText.text = "达到等级二目标";
                break;
            case 3:
                levelIcon.sprite = ResMgr.Instance.load<Sprite>("Icon/等级三");
                taskBtnText.text = "达到等级三目标";
                break;
            default:
                taskBtnText.text = "当前无任务";
                break;
        }
    }
    private void OnDisable()
    {
        EventCenter.Instance.RemoveEventListener<string>(GameEvent.任务开始事件,updateTaskItem);
        EventCenter.Instance.RemoveEventListener(GameEvent.玩家等级变化,UpdateGradeData);
    }
}
