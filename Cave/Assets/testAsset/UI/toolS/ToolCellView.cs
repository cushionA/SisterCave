using System.Collections.Generic;
using System.Linq;
using EnhancedUI.EnhancedScroller;
using UnityEngine;
using UnityEngine.UI;

public class ToolCellView : EnhancedScrollerCellView
{
    //[SerializeField] private Text[] contentTexts;
    [SerializeField] private Image[] contentIcon;

    public Button[] ContentButtons { get; private set; }

    [HideInInspector] public UseButtom[] ub;


    public void SetData(UseItemData data)
    {
        for (var i = 0; i < this.contentIcon.Length; i++)
        {
            //this.contentTexts[i].text = data.Contents[i].itemName;
            //For文でデータの数だけテキストにデータをぶち込む。そしてテキストの数＝ボタンの数になってる。
            //this.contentIcon[i].sprite = data.Contents[i].icon;
            ub[i].item = data.Contents[i];//そのボタンのアイテムに設定
        }
    }

    private void Awake()
    {
        this.ContentButtons = this.contentIcon.Select(text => text.GetComponentInParent<Button>()).ToArray();
        ub = this.contentIcon.Select(text => text.GetComponentInParent<UseButtom>()).ToArray();

        /*
        for (var i = 0; i < this.contentIcon.Length; i++)
        {
            if (contentIcon[i].GetComponentInParent<UseButtom>() == null)
            {
                //Debug.log("お星さま");
            }
            else
            {
                //Debug.log("お月様");
            }
            ub[i] = contentIcon[i].GetComponentInParent<UseButtom>();
        }*/

    }
}