using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipsController : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    Text tx;


    RectTransform mine;

    [SerializeField]
    RectTransform textBox;

    bool isFirst;

    float before;

   GameObject beforeObj;

    void Start()
    {
        mine = GetComponent<RectTransform>();
    }

    private void OnDisable()
    {
        MainUI.instance.isTips = false;
        isFirst = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (beforeObj != MainUI.instance.eventSystem.currentSelectedGameObject || beforeObj == null)
        {
            MainUI.instance.isTips = false;
            beforeObj = MainUI.instance.eventSystem.currentSelectedGameObject;
            Debug.Log($"ｄ{MainUI.instance.eventSystem.currentSelectedGameObject.name}");
            Debug.Log($"s{beforeObj}");
            return;
        }
        if (MainUI.instance.isTips && !isFirst)
        {
            tx.text = MainUI.instance.eventSystem.currentSelectedGameObject.GetComponent<TipsWindow>().description;

            //      Vector2 posi = MainUI.instance.eventSystem.currentSelectedGameObject.GetComponent<RectTransform>().anchoredPosition;

            //サイズ変更
            //  mine.anchoredPosition = new Vector2(posi.x + 120,posi.y);
            Vector2 posi = MainUI.instance.eventSystem.currentSelectedGameObject.GetComponent<RectTransform>().position;

            //サイズ変更
            mine.position = new Vector2(posi.x + 30, posi.y);
            isFirst = true;
            

        }
        else if (!MainUI.instance.isTips)
        {
            isFirst = false;
            before = -1000;
            beforeObj = null;
        }

        if(textBox.sizeDelta.y != before)
        {
            before = textBox.sizeDelta.y;
            mine.sizeDelta = new Vector2(mine.sizeDelta.x, textBox.sizeDelta.y + 50);
        }

    }
}
