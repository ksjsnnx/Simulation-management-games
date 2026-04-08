using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyItem : MonoBehaviour
{
    public Image icon;
    public Text name;
    public Text num;
    public Button buyBtn;
    public GameObject Des;
    public Text DesText;
    private MerchantingData currentMerchantingData;
    void Start()
    {
        buyBtn.onClick.AddListener(openBuyDetail);
        EventCenter.Instance.AddEventListener(GameEvent.玩家等级变化,updateDes);
    }

    private void OnDestroy()
    {
        buyBtn.onClick.RemoveAllListeners();
        EventCenter.Instance.RemoveEventListener(GameEvent.玩家等级变化,updateDes);
    }

    public void updateBuyItem(MerchantingData data)
    {
        currentMerchantingData = data;
        ProductItemData productItemData = GameManager.Instance.productItemDict[data.productid];
        icon.sprite = ResMgr.Instance.load<Sprite>("Icon/"+productItemData.sprite);
        name.text = productItemData.name;
        num.text = data.maxCount.ToString();
        updateDes();
    }
/// <summary>
/// 更新等级信息
/// </summary>
/// <param name="gameLevel"></param>
    private void updateDes()
    {
       int gameLevel =currentMerchantingData.gameLevel;
        //判断游戏等级是否大于等于商品中的等级  
        if (GameManager.Instance.playerData.GameLevel>=gameLevel)
        {
            Des.gameObject.SetActive(false);
        }
        else
        {
            Des.gameObject.SetActive(true);
            DesText.text = $"达到{gameLevel}等级解锁该商品,当前无法购买";
        }
    }

    /// <summary>
/// 打开购买的详情窗口
/// </summary>
    private void openBuyDetail()
    {
       
        UIManager.Instance.openPanel<BuyItemCheckPanel>().updateData(currentMerchantingData);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
