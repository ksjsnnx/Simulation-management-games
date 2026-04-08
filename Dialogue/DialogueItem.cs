using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueItem : MonoBehaviour
{
   [Header("对话的id")]
   public string id;
   //对话内容
   [HideInInspector] public DialogueItemData dialogueItemData;

   private void Start()
   {
      Init();
   }

   private void Init()
   {
      dialogueItemData = GameManager.Instance.dialogueItemDict[id];
   }
}
