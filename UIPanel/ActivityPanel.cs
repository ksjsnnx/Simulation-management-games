using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivityPanel : BasePanel
{
    public override void Awake()
    {
        base.Awake(); // 初始化按钮绑定
        SetFullScreen(); // 强制全屏适配
    }
}
