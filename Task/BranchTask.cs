using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchTask : Task
{
    private void OnEnable()
    {
        taskType = "支线任务";
        EventCenter.Instance.AddEventListener<string>(GameEvent.任务开始事件,updateTaskItem);
    }

    public override void Start()
    {
        base.Start();       
       
        
    }

    private void OnDisable()
    {
        EventCenter.Instance.RemoveEventListener<string>(GameEvent.任务开始事件,updateTaskItem);
    }
}
