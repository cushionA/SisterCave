﻿using System;
using System.Collections.Generic;
using System.Linq;
using EnhancedUI.EnhancedScroller;
using UnityEngine;
using UnityEngine.UI;

public class MaterialController : MonoBehaviour, IEnhancedScrollerDelegate
{
    // ここに最上段セルからの遷移先をセットしておく（UseSetボタン?）
    [SerializeField] private Selectable startCellNavigationTarget;

    [SerializeField] private EnhancedScroller fooScroller;
    [SerializeField] private MaterialCellView EquipCellViewPrefab;
    private List<MaterialData> data;
    private RectTransform content;
    [SerializeField]
    private MaterialDataBase toolDataBase;

    [SerializeField] Scrollbar useBar;
    [SerializeField] ScrollRect useRect;

    public MaterialManager dic;
    public MainUI mi;
    public MaterialItem space1;
    public MaterialItem space2;
    public MaterialItem space3;
    //List<ToolItem> enableTool;



    List<MaterialItem> setList;
    [HideInInspector] public bool isEver;

    int lLimit;
    int uLimit;

    float limit;
    float pos;


    bool isFirstU;
    bool isSecondU;
    bool isThirdU;

    bool isFirstD;
    bool isSecondD;
    bool isThirdD;

    bool isReverse;

    int N = 3;
    //Nと同じ数だけ格納するminiListを作成、miniList.size()の最大値 = N


    [HideInInspector] public bool isIniti;

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
        var cellView = scroller.GetCellView(this.EquipCellViewPrefab) as MaterialCellView;
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
        #region
        /*
        foreach(KeyValuePair<ToolItem,int>ti in dic.GetItemDictionary())
        {
            ToolItem item = ti.Key;
            int num = ti.Value;

            if (num > 0)
            {
                enableTool.Add(ti.Key);
                ////Debug.log("EEEE");
            }

        }
        ////Debug.log(enableTool.Count);

        for (int i = 0; i < enableTool.Count; i += 3)
        {
            ////Debug.log("WWW");
            this.data = new List<UseItemData>{

            new UseItemData(enableTool[i],enableTool[i+1],enableTool[i+2])
        };
        }*/
        #endregion



        #region

        for (int i = 0; i < toolDataBase.GetItemLists().Count; i++)
        {
            //　アイテム数を適当に設定
            toolDataBase.GetItemLists()[i].inventoryNum = 0;

        }

        data = null;

        setList = new List<MaterialItem>(ToolList());

        /*(from item in toolDataBase.GetItemLists()
               where item.inventoryNum > 0
               select item).ToList();*/
        //最終的に作りたいリストの初期化
        data = new List<MaterialData>();
        //Nと同じ数だけ格納するminiListを作成、miniList.size()の最大値 = N
        List<MaterialItem> miniList = new List<MaterialItem>(N);

        //ToolItem[] miniList = new ToolItem[3];

        if (setList.Count % N == N - 1)
        {
            setList.Add(space1);
        }
        else if (setList.Count % N == N - 2)
        {
            setList.Add(space1);
            setList.Add(space2);
        }
        //ぴったりになるよう穴埋めした


        for (int i = 0; i < setList.Count(); i++)
        {
            //if (setList.Count - i >= N){
            //Nの倍数ならminiListを初期化（例:0, 3, 6 ...)
            if (i % N == 0)
            {
                miniList = null;
                miniList = new List<MaterialItem>(N);
            }
            //miniListに格納
            miniList.Add(setList[i]);
            //Nの倍数-1ならminiListを元にUseItemDataを作成して格納

            if (i % N == N - 1)
            {

                MaterialData mini = new MaterialData(miniList.ToArray());
                data.Add(mini);
                mini = null;
            }
        }
        #endregion
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
        miniList = null;
        setList = null;
        this.UpdateNavigationConnections();
    }

    private void Update()
    {

        if (mi.isReBuild && !isEver)
        {
            data = null;//インベントリを一回からにして入れなおす
            #region

            //  for (int i = 0; i < toolDataBase.GetItemLists().Count; i++)
            // {
            //　アイテム数を適当に設定
            // setList.Add(toolDataBase.GetItemLists()[i]);
            //setList = toolDataBase.GetItemLists();
            // }

            setList = new List<MaterialItem>(ToolList());

            /*(from item in toolDataBase.GetItemLists()
                   where item.inventoryNum > 0
                   select item).ToList();*/
            //最終的に作りたいリストの初期化
            data = new List<MaterialData>();
            //Nと同じ数だけ格納するminiListを作成、miniList.size()の最大値 = N
            List<MaterialItem> miniList = new List<MaterialItem>(N);

            //ToolItem[] miniList = new ToolItem[3];

            if (setList.Count % N == N - 1)
            {
                setList.Add(space1);
            }
            else if (setList.Count % N == N - 2)
            {
                setList.Add(space1);
                setList.Add(space2);
            }
            //ぴったりになるよう穴埋めした


            for (int i = 0; i < setList.Count(); i++)
            {
                //if (setList.Count - i >= N){
                //Nの倍数ならminiListを初期化（例:0, 3, 6 ...)
                if (i % N == 0)
                {
                    miniList = null;
                    miniList = new List<MaterialItem>(N);
                }
                //miniListに格納
                miniList.Add(setList[i]);
                //Nの倍数-1ならminiListを元にUseItemDataを作成して格納

                if (i % N == N - 1)
                {

                    MaterialData mini = new MaterialData(miniList.ToArray());
                    data.Add(mini);
                    mini = null;
                }
            }
            #endregion
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
            miniList = null;
            setList = null;
            this.UpdateNavigationConnections();
            isEver = true;
        }
        /*else if(!mi.isReBuild && isEver)
        {
            ////Debug.log("愛してない");
            //isEver = false;
        }*/
        JButton();
    }


    public void JButton()
    {

        pos = 1f / ((float)data.Count - 4.0f);
        //MyItem.rowLengthが20にあたります、intで宣言しているので、floatに置換しています。
        //公式は　ScrollBarのValueの上限、セルの数、表示可能なセルの数。

        if (GManager.instance.InputR.GetAxisPrev(MainUI.instance.rewiredAction15) - GManager.instance.InputR.GetAxisRaw(MainUI.instance.rewiredAction15) < 0)
        {
            if (isReverse == true)
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
             //切れちゃうもんね。動かさないと

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
        if (GManager.instance.InputR.GetAxisPrev(MainUI.instance.rewiredAction15) - GManager.instance.InputR.GetAxisRaw(MainUI.instance.rewiredAction15) > 0 && isIniti)
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
        else if (GManager.instance.InputR.GetAxisPrev(MainUI.instance.rewiredAction15) - GManager.instance.InputR.GetAxisRaw(MainUI.instance.rewiredAction15) > 0 && !isIniti)
        {

            isIniti = true;

        }

        limit = useBar.value;

        if (limit >= 1)
        {
            limit = 1;

        }



        else if (limit <= 0)
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
            .Select(t => t.GetComponent<EquipCellView>())
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
                           ////Debug.log("yasai");
                       }
                       else
                       {
                           isLast = false;
                           ////Debug.log("niku");
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

    IEnumerable<MaterialItem> ToolList()
    {
        foreach (var item in toolDataBase.GetItemLists())
        {
            if (item.inventoryNum > 0)
            {
                yield return item;
            }

        }
    }


}