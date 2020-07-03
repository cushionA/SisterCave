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


    [SerializeField] Scrollbar useBar;
    [SerializeField] ScrollRect useRect;


    float verticalKey;


    int lLimit;
    int uLimit;

    float limit;
    float pos;

    bool isTop;
    bool isLast;

    bool isFirstU;
    bool isSecondU;
    bool isThirdU;

    bool isFirstD;
    bool isSecondD;
    bool isThirdD;

    bool isReverse;

    int jumpDataIndex;

    [HideInInspector]public bool isIniti;

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
            new ScrollerData("Yankee", "Zulu", "☺"),


             new ScrollerData("Gof", "Hotl", "Inda"),
            new ScrollerData("Julitt", "Klo", "Lia"),
            new ScrollerData("Mie", "Novmber", "Osar"),
            new ScrollerData("Paa", "Quebc", "Rmeo"),
            new ScrollerData("Siera", "Tago", "Unform"),
            new ScrollerData("Vicor", "Whikey", "Xray"),
            new ScrollerData("Yanee", "Zuu", "☺kk")
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

    private void Update()
    {
        JButton();
    }


    public void JButton()
    {

         pos = 1f / ((float)data.Count - 4.0f);
        //MyItem.rowLengthが20にあたります、intで宣言しているので、floatに置換しています。
        //公式は　ScrollBarのValueの上限、セルの数、表示可能なセルの数。

        if (Input.GetKeyDown(KeyCode.W))
        {
            if(isReverse == true)
            {
             
                isReverse = false;
            }

            if (!isFirstU)
            {
                isFirstU = true;
                isThirdD = false;
            }
            else if (isFirstU && !isSecondU && !isThirdU)
            {
                isSecondU = true;
                isSecondD = false;

            }
            else if (isFirstD && isSecondU)
            {//足りない助走を補う

                    isThirdU = true;
                    isFirstD = false;
                    useBar.value += pos;
            }
            else if (isSecondU && !isThirdU)
            {
                isThirdU = true;
                isFirstD = false;
            }
            else if (isThirdU)
            {
                useBar.value += pos;
            }
        }
      if(Input.GetKeyDown(KeyCode.S) && isIniti)
        {
            if (!isFirstD)
            {
                isFirstD = true;
                isThirdU = false;
            }
            else if (isFirstD && !isSecondD && !isThirdD)
            {
                isSecondD = true;
                isSecondU = false;

            }

            else if (isSecondD && !isThirdD)
            {
                isThirdD = true;
                isFirstU = false;
               
            }
 
            else if (isThirdD)
            {
                isReverse = true;
                useBar.value -= pos;
            }
        }
      else if(Input.GetKeyDown(KeyCode.S) && !isIniti)
        {

            isIniti = true;

        }

       limit = useBar.value;

      if(limit >= 1)
        {
            limit = 1;

        }



      else if(limit <= 0)
        {
            limit = 0.01f;
        }
        useBar.value = limit;

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

            uLimit = 0;
        lLimit = this.data.Count;

 /* if (i == (cells.Length - 1) )
            {
                isLast = true;
                Debug.Log("yasai");
            }
            else
            {
                isLast = false;
                Debug.Log("niku");
            }
            if(i != 0)
            {

                isTop = true;

            }
            else
            {

                isTop = false;

            }
            */
       
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