using System.Linq;
using EnhancedUI.EnhancedScroller;
using UnityEngine;
using UnityEngine.UI;

public class FooCellView : EnhancedScrollerCellView
{
    [SerializeField] private Text[] contentTexts;

    public Button[] ContentButtons { get; private set; }

    public void SetData(ScrollerData data)
    {
        for (var i = 0; i < this.contentTexts.Length; i++)
        {
            this.contentTexts[i].text = data.Contents[i];
        }
    }

    private void Awake()
    {
        this.ContentButtons = this.contentTexts.Select(text => text.GetComponentInParent<Button>()).ToArray();
    }
}