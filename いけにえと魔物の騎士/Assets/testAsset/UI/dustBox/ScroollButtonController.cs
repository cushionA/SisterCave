using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAutoScroll : MonoBehaviour
{
    GameObject nowActiveButton;

    [System.Serializable]
    private struct ScrollViewNumber
    {
        public float ScrollViewHeight;
        public float ViewportTop;
        public float ViewportButtom;
        public float ButtonHeight;
        public float VerticalLayoutGroupSpacing;
        public float VerticalLayoutGroupTopPadding;
        public float VerticalLayoutGroupButtomPadding;
        public float ButtonsNumber;

        public float ScrollValue;
    }

    void Start()
    {
        nowActiveButton = hoge;
        //最初にアクティブにしたいボタンを指定する
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            GameObject parentButton = nowActiveButton.transform.parent.gameObject;
            List<GameObject> childrenObj = FindChildren(parentButton.transform);
            int ActiveNumber = -1;
            for (int i = 0; i < childrenObj.Count; i++)
            {
                if (childrenObj[i] == nowActiveButton)
                {
                    ActiveNumber = i;
                }
            }
            if (ActiveNumber == 0 || ActiveNumber == -1) { return; }
            nowActiveButton = childrenObj[ActiveNumber - 1];

            if (nowActiveButton.transform.parent.tag == "ScrollContent") { ScrollViewMove(ActiveNumber, true); }
            //スクロールバーのボタン群の親（通常はScrollView/ViewPort/Content）のtagにScrollContetnを指定します
            //第二引数がtrueの場合は上移動、falseの場合は下移動です
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            GameObject parentButton = nowActiveButton.transform.parent.gameObject;
            List<GameObject> childrenObj = FindChildren(parentButton.transform);
            int ActiveNumber = -1;
            for (int i = 0; i < childrenObj.Count; i++)
            {
                if (childrenObj[i] == nowActiveButton)
                {
                    ActiveNumber = i;
                }
            }
            if (ActiveNumber == -1 || ActiveNumber == childrenObj.Count - 1) { return; }
            nowActiveButton = childrenObj[ActiveNumber + 1];

            if (nowActiveButton.transform.parent.tag == "ScrollContent") { ScrollViewMove(ActiveNumber, false); }

        }
    }

    List<GameObject> FindChildren(Transform parentObj)
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in parentObj)
        {
            children.Add(child.gameObject);
        }
        if (children == null) { Debug.Log("KeyMouseControll FindChildren() is null. there are no children."); }
        return children;
    }

    void ScrollViewMove(int ActiveNumber, bool updown)
    {
        ScrollViewNumber scrollViewNumber = new ScrollViewNumber();
        scrollViewNumber.ScrollViewHeight = nowActiveButton.transform.parent.transform.parent.transform.parent.GetComponent<RectTransform>().sizeDelta.y;
        RectTransform viewportRect = nowActiveButton.transform.parent.transform.parent.GetComponent<RectTransform>();
        scrollViewNumber.ViewportTop = viewportRect.offsetMax.y;
        scrollViewNumber.ViewportButtom = viewportRect.offsetMin.y;
        VerticalLayoutGroup verticalLayoutGroup = nowActiveButton.transform.parent.GetComponent<VerticalLayoutGroup>();
        scrollViewNumber.VerticalLayoutGroupSpacing = verticalLayoutGroup.spacing;
        scrollViewNumber.VerticalLayoutGroupTopPadding = verticalLayoutGroup.padding.top;
        scrollViewNumber.VerticalLayoutGroupButtomPadding = verticalLayoutGroup.padding.bottom;

        scrollViewNumber.ButtonHeight = nowActiveButton.GetComponent<RectTransform>().sizeDelta.y;
        scrollViewNumber.ButtonsNumber = nowActiveButton.transform.parent.transform.childCount;

        Scrollbar scrollBar = nowActiveButton.transform.parent.transform.parent.transform.parent.GetChild(1).GetComponent<Scrollbar>();
        scrollViewNumber.ScrollValue = scrollBar.value;

        float seeAbleRange = scrollViewNumber.ScrollViewHeight - scrollViewNumber.ViewportTop - scrollViewNumber.ViewportButtom - scrollViewNumber.VerticalLayoutGroupTopPadding - scrollViewNumber.VerticalLayoutGroupButtomPadding;

        int nextButtonNumber;
        if (updown) { nextButtonNumber = ActiveNumber + 1 - 1; } else { nextButtonNumber = ActiveNumber + 1 + 1; }
        float nowButtonPosition = (scrollViewNumber.ButtonHeight + scrollViewNumber.VerticalLayoutGroupSpacing) * (nextButtonNumber);

        float allButtonHeights = (scrollViewNumber.ButtonHeight + scrollViewNumber.VerticalLayoutGroupSpacing) * scrollViewNumber.ButtonsNumber;
        float valueRange = allButtonHeights - seeAbleRange;
        float overlap;
        float newValue = -1;

        switch (updown)
        {
            case false:  //下に行く場合
                if (seeAbleRange + (1 - scrollBar.value) * valueRange > nowButtonPosition) { return; }  //ボタンがScrollView内部で見えている場合はreturn

                overlap = valueRange - (nowButtonPosition - seeAbleRange);
                newValue = overlap / valueRange;
                break;
            default:  //上に行く場合
                float reserveSpace = allButtonHeights - seeAbleRange;
                float nextButtonHeight = nowButtonPosition - scrollViewNumber.ButtonHeight - scrollViewNumber.VerticalLayoutGroupSpacing;
                if (reserveSpace * (1 - scrollBar.value) < nextButtonHeight) { return; }

                overlap = reserveSpace - nextButtonHeight;
                newValue = overlap / reserveSpace;
                break;
        }

        if (newValue == -1) { Debug.Log("New Value is not changed by switch case. use return."); return; }
        if (newValue > 1) { newValue = 1; }
        if (newValue < 0) { newValue = 0; }
        scrollBar.value = newValue;
    }
}