using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 输入管理器 基本的按键控制  
/// </summary>
public class InputManager : UnitySingleTon<InputManager>
{
    bool isOpenKnapsackPanel = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.Instance.openPanel<GamePausePanel>();
        }

        // 右键关闭当前打开的面板
        if (Input.GetMouseButtonDown(1))
        {
            UIManager.Instance.CloseCurrentPanel();
        }


        if (Input.GetKeyDown(KeyCode.M)&&!isOpenKnapsackPanel)
        {
            UIManager.Instance.openPanel<KnapsackPanel>();
            isOpenKnapsackPanel = !isOpenKnapsackPanel;

        }
        else  if(Input.GetKeyDown(KeyCode.M) && isOpenKnapsackPanel)
        {
            UIManager.Instance.closePanel<KnapsackPanel>();
            isOpenKnapsackPanel=!isOpenKnapsackPanel;
        }
    }
}
