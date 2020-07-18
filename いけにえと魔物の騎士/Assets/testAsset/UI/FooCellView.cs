using System.Linq;
using EnhancedUI.EnhancedScroller;
using UnityEngine;
using UnityEngine.UI;

public class FooCellView : EnhancedScrollerCellView
{
    [SerializeField] private Text[] contentTexts;
    [SerializeField] private Image[] contentIcon;

    public Button[] ContentButtons { get; private set; }

    public void SetData(UseItemData data)
    {
        for (var i = 0; i < this.contentTexts.Length; i++)
        {
            this.contentTexts[i].text = data.Contents[i].GetItemName();
            //For文でデータの数だけテキストにデータをぶち込む。そしてテキストの数＝ボタンの数になってる。
            this.contentIcon[i].sprite = data.Contents[i].GetIcon();
        }
    }

    private void Awake()
    {
        this.ContentButtons = this.contentTexts.Select(text => text.GetComponentInParent<Button>()).ToArray();
    }
}