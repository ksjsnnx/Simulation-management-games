using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using DG.Tweening;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.UI;
public class Task : MonoBehaviour
{
    [Header("任务下拉按钮")]
    public Button TaskBtn;
    [Header("任务按钮描述")]
    public Text taskBtnText;
    [Header("任务下拉列表")]
    public GameObject taskScrollView;
    [Header("任务容器")]
    public GameObject taskContent;
    [Header("任务列表是否打开")]
    private bool isOpen;
    //任务列表Y的规模长度
    private int contentLength;
    [Header("任务列表对象")]
    public GameObject taskView;
    //任务类型
    [HideInInspector]
    public string taskType;
    public virtual void Start()
    {
        
        openTaskContent();
        TaskBtn.onClick.AddListener(openTaskContent);
    }
/// <summary>
/// 打开任务详情的方法         
/// </summary>
    public virtual void openTaskContent()
    {
        isOpen = !isOpen;
        taskView.transform.localScale = isOpen?Vector3.zero:Vector3.one;
        contentLength = isOpen ? 1 : 0;
        taskScrollView.transform.localScale = new Vector3(1,1-contentLength,1);
        taskScrollView.transform.DOScaleY(contentLength, 0.5f).OnComplete(() =>
        {
            EventCenter.Instance.EventTrigger(GameEvent.任务UI切换);
            taskView.transform.DOScale(isOpen ? Vector3.one : Vector3.zero, 0.2f);
        });
    }
/// <summary>
/// 更新任务项的方法  
/// </summary>
/// <param name="id">任务id</param>
    public void updateTaskItem(string id)
    {
        //拿到任务数据 
        TaskItemData taskItemData = GameManager.Instance.taskItemDict[id];
        if(taskItemData==null)return;
        //判断任务类型 
        if (taskItemData.type==taskType)
        {
            //克隆taskitem到taskContent下面  
            GameObject obj=ResMgr.Instance.load<GameObject>("UI/taskItem",taskContent.transform);
            obj.transform.localScale=Vector3.one;
            obj.GetComponent<TaskItem>().updateTaskItem(taskItemData.id);
            if (taskItemData.type == "主线任务")
            {
                TaskManager.Instance.mainTaskDict.Add(taskItemData.id,obj.GetComponent<TaskItem>());
            }
            else
            {
                TaskManager.Instance.branchDict.Add(taskItemData.id,obj.GetComponent<TaskItem>());
            }
        }        
    }


}
