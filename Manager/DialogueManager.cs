using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.EventSystems;
/// <summary>
/// 对话的管理器    点击（碰到）npc触发对话内容  
/// </summary>
public class DialogueManager : UnitySingleTon<DialogueManager>
{
  [Header("当前选中的npc对象")]
  public DialogueItem currentNPC;

  [Header("与npc对话的虚拟摄像机")] public GameObject dialogueCamera;
  private void Start()
  {
    
  }

  private void Update()
  {
    //判断当前射线是否在UI层 如果UI上就直接return  
    if (EventSystem.current.IsPointerOverGameObject()) return;
    //从屏幕点发射射线 
    Ray ray=Camera.main.ScreenPointToRay(Input.mousePosition); 
    //碰撞信息
    RaycastHit hitInfo;
    //碰撞检测 判断是否碰到npc    
    if (Physics.Raycast(ray, out hitInfo, 1000))
    {
      if (hitInfo.collider.tag == "NPC")
      {
         //保存当前触发对话的NPC  
         currentNPC=hitInfo.collider.gameObject.GetComponent<DialogueItem>();
        
      }
      else
      {
        currentNPC = null;
      }
      
    }

    if (Input.GetMouseButtonDown(1)&&currentNPC!=null)
    {
        //弹出对话内容  执行对话的内容 打开对话的面板   //调用UpdateNextDialogue方法  传进去第一条语句的id 
        UIManager.Instance.openPanel<DialoguePanel>().UpdateNextDialogue(currentNPC.id);
        dialogueCamera.SetActive(true);
        EventCenter.Instance.EventTrigger(GameEvent.对话开始);
    }
    
    
  }
}
