using System.Linq;
using EnhancedUI.EnhancedScroller;
using UnityEngine;
using UnityEngine.UI;

public class MagicCellView : EnhancedScrollerCellView
{
    //[SerializeField] private Text[] contentTexts;
    [SerializeField] private Image[] contentIcon;

    public Button[] ContentButtons { get; private set; }

    public void SetData(MagicData data)
    {
        for (var i = 0; i < this.contentIcon.Length; i++)
        {
            //this.contentTexts[i].text = data.Contents[i].itemName;
            //For文でデータの数だけテキストにデータをぶち込む。そしてテキストの数＝ボタンの数になってる。
            this.contentIcon[i].sprite = data.Contents[i].icon;
        }
    }

    private void Awake()
    {
        this.ContentButtons = this.contentIcon.Select(text => text.GetComponentInParent<Button>()).ToArray();
    }
}