using System.Collections.Generic;
using System.Linq;
using EnhancedUI.EnhancedScroller;
using UnityEngine;
using UnityEngine.UI;

public class ScrollController : MonoBehaviour, IEnhancedScrollerDelegate
{
    // ここに最上段セルからの遷移先をセットしておく（UseSetボタン?）
    [SerializeField] private Selectable startCellNavigationTarget;

    [SerializeField] private EnhancedScroller fooScroller;
    [SerializeField] private FooCellView fooCellViewPrefab;
    private List<ScrollerData> data;
    private RectTransform content;



    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return this.data.Count;
        //List<ScrollerData> dataに格納された要素の数を数える。
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return 100.0f;
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        var cellView = scroller.GetCellView(this.fooCellViewPrefab) as FooCellView;
        cellView.SetData(this.data[dataIndex]);
        return cellView;
    }

    private void OnCellViewVisibilityChanged(EnhancedScrollerCellView cellView)
    {
        // スクローラーのセル表示・非表示更新タイミングでナビゲーションを繋ぎ直す
        this.UpdateNavigationConnections();
    }

    private void Start()
    {


        this.data = new List<ScrollerData>
        {
            new ScrollerData("Alfa", "Bravo", "Charlie"),
            new ScrollerData("Delta", "Echo", "Foxtrot"),
            new ScrollerData("Golf", "Hotel", "India"),
            new ScrollerData("Juliett", "Kilo", "Lima"),
            new ScrollerData("Mike", "November", "Oscar"),
            new ScrollerData("Papa", "Quebec", "Romeo"),
            new ScrollerData("Sierra", "Tango", "Uniform"),
            new ScrollerData("Victor", "Whiskey", "X-ray"),
            new ScrollerData("Yankee", "Zulu", "☺")
            
        };
        this.content = this.fooScroller.GetComponent<ScrollRect>().content;
        //スクロール可能なコンテンツ。ScrollRect コンポーネントの
        //アタッチされているGameObject の子でなければなりません
        this.fooScroller.Delegate = this;
        //EnhansedScrollerのデリゲートの集合体のインターフェイスのインスタンス
        this.fooScroller.cellViewVisibilityChanged = this.OnCellViewVisibilityChanged;
        //Cellの表示と非表示が切り替わった際に動くデリゲート
        //そしてそのデリゲートにthis.OnCellViewVisibilityChangedを格納してる
        //すなわち表示非表示切り替わるとthis.OnCellViewVisibilityChanged
        //というかUpdateNavigationConnections()する

        this.fooScroller.ReloadData();
        this.UpdateNavigationConnections();
    }

    private void UpdateNavigationConnections()
    {
        if (this.content == null)
        {
            return;
        }

        // EnhancedScrollerに現在表示されているセルのインデックスを得る機能が用意されているようだが、
        // まだ使い慣れておらずいまいち意図通りの値が得られなかったので、自前でセルビューを列挙することにした
        var cells = this.content.Cast<RectTransform>()
            .Select(t => t.GetComponent<FooCellView>())
            .Where(c => c != null).ToArray();
        for (var i = 0; i < cells.Length; i++)
        {
            var cell = cells[i];
            var previousCell = i == 0 ? null : cells[i - 1];
            var nextCell = i == (cells.Length - 1) ? null : cells[i + 1];
            var buttonCount = cell.ContentButtons.Length;
            for (var j = 0; j < buttonCount; j++)
            {
                // 上下は上下のセルの中の同じ列にあるボタンを、左右は自分のセルの中の左右のボタンを繋げる
                // ただし、自分のセルが画面上に表示されているセルのうち最上段ならば、上方向の遷移先として
                // startCellNavigationTargetをセットすることにする
                // また、左右の端も反対側の端と繋げてループするようにしてみた
                var button = cell.ContentButtons[j];
                var previousButton = previousCell == null ? null : previousCell.ContentButtons[j];
                var nextButton = nextCell == null ? null : nextCell.ContentButtons[j];
                var navigation = button.navigation;
                navigation.mode = Navigation.Mode.Explicit;
                navigation.selectOnUp = previousCell == null ? this.startCellNavigationTarget : previousButton;
                navigation.selectOnDown = nextButton;
                navigation.selectOnLeft = cell.ContentButtons[((j - 1) + buttonCount) % buttonCount];
                navigation.selectOnRight = cell.ContentButtons[(j + 1) % buttonCount];
                button.navigation = navigation;
            }
        }
    }


}