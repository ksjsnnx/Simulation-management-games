using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildItem : MonoBehaviour
{
    public Image sprite;
    public Text name;
    public Text price;
    public BuildItemData currentData;
    private Button buildBtn;
    void Start()
    {
        buildBtn = gameObject.GetComponent<Button>();
        if (buildBtn != null)
        {
            buildBtn.onClick.AddListener(() =>
            {
                buyCheck();
               
            });
        }
    }

    private void buyCheck()
    {
        if (GameManager.Instance.playerData.Coin >= currentData.price)
        {
            GameObject obj = ResMgr.Instance.load<GameObject>($"Ground/{currentData.prefab}",BuildController.Instance.currentGround.transform);
            obj.name = "Building";
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition=Vector3.zero;
            BuildItemBase buildItem=obj.GetComponent<BuildItemBase>();
            buildItem.groundName = BuildController.Instance.currentGround.name;
            //添加新建造物到建造字典中  
            BuildController.Instance.addBuilding(currentData.id,buildItem);
            EventCenter.Instance.EventTrigger(GameEvent.建造物品成功);
            GameManager.Instance.playerData.Coin-=currentData.price;
            BuildController.Instance.currentGround.groundProperty.State=2;
            BuildController.Instance.currentGround.groundProperty.buildId=currentData.id;   
            EventCenter.Instance.EventTrigger(GameEvent.土地状态变化,2);
        }
        else
        {
            UIManager.Instance.openPanel<TipPanel>().UpdateTipText("金币不够");
        }
      
    }

    public void UpdateData(BuildItemData data)
    {
       
        currentData = data;
        name.text = data.name;
        price.text = data.price.ToString();
        sprite.sprite = ResMgr.Instance.load<Sprite>($"Icon/{data.sprite}");
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
