using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class OptionDialogueItem : DialogueItem
{
    [Header("任务按钮")]
    public Button checkBtn;
    [Header("任务文本")]
    public Text text;
    private void Start()
    {
        checkBtn.onClick.AddListener(UpdateNext);
        
        
    }

    private void OnDestroy()
    {
        checkBtn.onClick.RemoveAllListeners();
    }

    private void UpdateNext()
    {
        EventCenter.Instance.EventTrigger<string,string>(GameEvent.切换下一条对话语句,dialogueItemData.nextId,dialogueItemData.taskId);
    }

    public void UpdateOptionData(string dialogueId)
    { 
        id = dialogueId;
        dialogueItemData = GameManager.Instance.dialogueItemDict[id];
        text.text = dialogueItemData.dialogueContent;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
