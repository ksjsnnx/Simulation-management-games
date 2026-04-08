using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
public class BasePanel : MonoBehaviour
{
    protected RectTransform rootRect;
    //关闭按钮  并不是所有面板都有的 
    [SerializeField] protected Button closeBtn;    
    
    //控制淡入淡出的组件 
    private CanvasGroup canvasGroup;
    //淡入淡出速度 
    private float alphaSpeed=10;
    private bool isShow;
    private UnityAction hideAction;
    
    protected IEnumerator DelayedLayoutUpdate()
    {
        yield return null; // 等待一帧
        LayoutRebuilder.ForceRebuildLayoutImmediate(rootRect);
    }

    public virtual void Awake()
    {
        rootRect = this.transform as RectTransform;
     
        // 自动查找关闭按钮（如果 Inspector 没手动指定）
        if (closeBtn == null)
        {
            var btnTrans = transform.Find("CloseBtn");
            if (btnTrans != null)
            {
                closeBtn = btnTrans.GetComponent<Button>();
            }
        }

        if (closeBtn != null)
        {
            closeBtn.onClick.AddListener(() => { Hide(); });
        }
    }

    /// <summary>
    /// 动态添加一个全屏遮罩，用于拦截鼠标射线并提供背景感
    /// </summary>
    /// <param name="alpha">遮罩透明度 (0-1)</param>
    public void AddMask(float alpha = 0.5f)
    {
        // 1. 检查是否已经存在遮罩，避免重复添加
        if (transform.Find("DynamicMask") != null) return;

        // 2. 创建遮罩物体并设置为第一个子物体（确保在最底层）
        GameObject maskObj = new GameObject("DynamicMask");
        maskObj.transform.SetParent(this.transform);
        maskObj.transform.SetAsFirstSibling(); 

        // 3. 配置 RectTransform 实现全屏铺满
        RectTransform rt = maskObj.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.localScale = Vector3.one;

        // 4. 添加 Image 组件并配置
        Image img = maskObj.AddComponent<Image>();
        img.color = new Color(0, 0, 0, alpha); // 黑色半透明
        img.raycastTarget = true; // 关键：开启射线拦截
    }

 
    public virtual void OnDestroy()
    {
        if (closeBtn == null) return;
        closeBtn.onClick.RemoveAllListeners();
    }

    /// <summary>
/// 显示动画
/// </summary>
/// <param name="ev">ui动画类型</param>
    public void ShowAnim(float time=0.5f,UIAnimaEvent ev=UIAnimaEvent.渐变, TweenCallback action=null)
    {
        if (ev == UIAnimaEvent.渐变)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup= this.gameObject.AddComponent<CanvasGroup>();//添加canvasGroup组件来控制渐变效果  
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1, time).OnComplete(action);
        }
    }
/// <summary>
/// 隐藏动画
/// </summary>
/// <param name="ev">ui动画类型</param>
    public void HideAnim(float time=0.5f, UIAnimaEvent ev=UIAnimaEvent.渐变, TweenCallback action=null)
    {
        if (ev == UIAnimaEvent.渐变)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup= this.gameObject.AddComponent<CanvasGroup>();//添加canvasGroup组件来控制渐变效果  
            canvasGroup.alpha = 1;
            canvasGroup.DOFade(0, time).OnComplete(action);
        }
        else
        {
            action();
        }
    }
    public virtual void Show(float time = 0.3f, UIAnimaEvent ev = UIAnimaEvent.默认, TweenCallback action = null)
    {
        this.gameObject.SetActive(true);
        
        // 停止之前的动画，防止冲突
        rootRect.DOKill(); 
        
        // 初始缩放为0，执行动画
        rootRect.localScale = Vector3.zero;
        rootRect.DOScale(Vector3.one, time).SetEase(Ease.OutBack).OnComplete(action);

        if (ev == UIAnimaEvent.渐变)
        {
            ShowAnim(time);
        }
    }


    public virtual void Hide()
    {
        rootRect.DOKill();
        rootRect.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            HideAnim(0.5f, UIAnimaEvent.默认, () =>
            {
                this.gameObject.SetActive(false);
                // 隐藏后放回主 Canvas 保持整洁
                this.transform.SetParent(UIManager.Instance.Canvas);
            });
        });
    }

    private void ExpandPanelFrom0To1()
    {
        rootRect.DOScale(Vector3.one, 0.3f).SetEase(Ease. OutBack);
    }
    /// <summary>
    /// 强制将面板设置为全屏铺满
    /// </summary>
    public void SetFullScreen()
    {
        if (rootRect == null) rootRect = GetComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;
        rootRect.localScale = Vector3.one;
    }
    
    /// <summary>
    /// 将面板锚定在左上角
    /// </summary>
    /// <param name="width">宽度</param>
    /// <param name="height">高度</param>
    /// <param name="offset">相对左上角的偏移量 (x 为向右，y 为向下)</param>
    public void SetAnchorTopLeft(float width, float height, Vector2 offset = default)
    {
        if (rootRect == null) rootRect = GetComponent<RectTransform>();
        
        // 锚点和中心点都设为左上角 (0, 1)
        rootRect.anchorMin = new Vector2(0, 1);
        rootRect.anchorMax = new Vector2(0, 1);
        rootRect.pivot = new Vector2(0, 1);
        
        // 设置宽高
        rootRect.sizeDelta = new Vector2(width, height);
        
        // 设置坐标偏移 (Unity 中 y 轴向下通常是负值)
        rootRect.anchoredPosition = new Vector2(offset.x, -offset.y);
        
        rootRect.localScale = Vector3.one;
    }

    /// <summary>
    /// 将锚点和中心点都设为左上角，并设置坐标
    /// </summary>
    /// <param name="pos">Unity UI 标准坐标：x 正值向右，y 负值向下</param>
    public void SetAnchorTopLeftOnly(Vector2 pos = default)
    {
        if (rootRect == null) rootRect = GetComponent<RectTransform>();
        
        // 锚点 (0, 1) 表示左上角
        rootRect.anchorMin = new Vector2(0, 1);
        rootRect.anchorMax = new Vector2(0, 1);
        
        // 中心点 (0, 1) 表示以 UI 的左上角作为对齐基准
        rootRect.pivot = new Vector2(0, 1);
        
        // 直接设置坐标，不做负号转换，保持标准
        rootRect.anchoredPosition = pos;
        rootRect.localScale = Vector3.one;
    }

    private void Update()
    {
       
    }
}
