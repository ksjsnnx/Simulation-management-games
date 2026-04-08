using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 任务管理器  主线任务 支线任务 领取任务  
/// </summary>
public class TaskManager : UnitySingleTon<TaskManager>
{
    public Dictionary<string, TaskItem> mainTaskDict=new Dictionary<string, TaskItem>();//主线任务字典 
    public Dictionary<string, TaskItem> branchDict=new Dictionary<string, TaskItem>();//支线任务字典  
    public override void Awake()
    {
        base.Awake();

    }

    void Start()
    {
       
    }
/// <summary>
/// 清空所有任务 
/// </summary>
    public void clearAllTask()
    {
        clearMainTask();
        clearBranchTask();  
    }
/// <summary>
/// 清空主线任务   
/// </summary>
    public void clearMainTask()
    {
        foreach (TaskItem taskItem in mainTaskDict.Values)
        {
            Destroy(taskItem.gameObject);
        }
        mainTaskDict.Clear();   
    }
/// <summary>
/// 清空支线任务 
/// </summary>
    public void clearBranchTask()
    {
        foreach (TaskItem taskItem in branchDict.Values)
        {
            Destroy(taskItem.gameObject);
        }        
        branchDict.Clear(); 
    }

    
/// <summary>
/// 初始化任务数据 
/// </summary>
    public void Init()
    {
        EventCenter.Instance.AddEventListener(GameEvent.玩家等级变化,UpdateMainTask);
        EventCenter.Instance.AddEventListener<TaskItem>(GameEvent.任务结束事件,jiangli);
        EventCenter.Instance.AddEventListener(GameEvent.任务完成事件,checkUpdateGrade);
        UpdateTask();

    }

    private void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener(GameEvent.玩家等级变化,UpdateMainTask);
        EventCenter.Instance.RemoveEventListener<TaskItem>(GameEvent.任务结束事件,jiangli);
        EventCenter.Instance.RemoveEventListener(GameEvent.任务完成事件,checkUpdateGrade);
            
    }
/// <summary>
/// 更新任务内容 
/// </summary>
    public void UpdateTask()
    {
        UpdateMainTask();
        UpdateBranchTask();
    }
    /// <summary>
/// 更新主线任务内容 
/// </summary>
    public void UpdateMainTask()
    {
        //玩家等级变化
        int level = GameManager.Instance.playerData.GameLevel+1;
        //显示等级图标 
        UIManager.Instance.openPanel<GradePanel>();
        foreach (var taskItem in mainTaskDict.Values)
        {
            Destroy(taskItem.gameObject);
        }
        mainTaskDict.Clear();
        foreach (var taskItem in GameManager.Instance.taskItemDict.Values)
        {
            if (taskItem.level == level && taskItem.type == "主线任务")
            {
                creatNewTask(taskItem.id);
            }
        }
        
    }
/// <summary>
/// 更新支线任务信内容
/// </summary>
    public void UpdateBranchTask()
    {
        foreach (var taskItem in branchDict.Values)
        {
            Destroy(taskItem.gameObject);
        }
        branchDict.Clear();
        foreach (var taskItem in GameManager.Instance.taskItemDict.Values)
        {
            if (taskItem.level<=GameManager.Instance.playerData.GameLevel && taskItem.type == "支线任务")
            {
                creatNewTask(taskItem.id);
            }
        }
    }
    
    
    /// <summary>
/// 创建新任务  更改任务的开始状态为真 触发任务开始状态事件  
/// </summary>
    public void creatNewTask(string id)
    {
        TaskItemData taskItem = GameManager.Instance.taskItemDict[id];
        if(taskItem==null)return;
        if (taskItem.type == "主线任务")
        {
        //更改游戏管理器中任务字典中任务开始状态
        GameManager.Instance.taskItemDict[id].isStarted=true;
        //触发任务开始事件 
        EventCenter.Instance.EventTrigger<string>(GameEvent.任务开始事件,id);
        }else if (taskItem.type == "支线任务"&&taskItem.isStarted)
        {
            //触发任务开始事件 
            EventCenter.Instance.EventTrigger<string>(GameEvent.任务开始事件,id);
        }
    }
/// <summary>
/// 领取支线任务 
/// </summary>
/// <param name="id"></param>
    public void creatNewBranchTask(string id)
    {
        UIManager.Instance.openPanel<TipPanel>().UpdateTipText("领取任务成功");
        GameManager.Instance.taskItemDict[id].isStarted=true;
        creatNewTask(id);
    } 
    
/// <summary>
/// 奖励方法  任务完成后会触发该方法  
/// </summary>
/// <param name="id">任务id</param>
    public void jiangli(TaskItem taskItem)
    {
        if(taskItem==null)return;
        taskItem.taskOver();//任务结束
        UIManager.Instance.openPanel<TipPanel>().UpdateTipText("完成任务获得任务奖励");
        //任务完成触发任务完成 这里任务结束了要领取奖励之后才算任务完成 
        taskItem.FinishTask();
    }

/// <summary>
/// 核对更新等级的方法 判断当前等级所有任务是否都完成  如果完成则升级游戏等级    
/// </summary>
    public void checkUpdateGrade()
    {
        foreach (var taskItem in mainTaskDict.Values)
        {
            if(!taskItem.taskItemData.isFinished)return;
        }
            
        GameManager.Instance.playerData.GameLevel++;
    }
    void Update()
    {
      
    }
}
