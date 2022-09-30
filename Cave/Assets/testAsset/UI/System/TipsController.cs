using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Tools;
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
        mine = this.gameObject.MMGetComponentNoAlloc<RectTransform>();
    }

    private void OnDisable()
    {
        MainUICon.instance.isTips = false;
        isFirst = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (beforeObj != MainUICon.instance.eventSystem.currentSelectedGameObject || beforeObj == null)
        {
            MainUICon.instance.isTips = false;
            beforeObj = MainUICon.instance.eventSystem.currentSelectedGameObject;
         //   Debug.Log($"ｄ{MainUICon.instance.eventSystem.currentSelectedGameObject.name}");
        //    Debug.Log($"s{beforeObj}");
            return;
        }
        if (MainUICon.instance.isTips && !isFirst)
        {
            tx.text = MainUICon.instance.eventSystem.currentSelectedGameObject.MMGetComponentNoAlloc<TipsWindow>().description;

            //      Vector2 posi = MainUICon.instance.eventSystem.currentSelectedGameObject.MMGetComponentNoAlloc<RectTransform>().anchoredPosition;

            //サイズ変更
            //  mine.anchoredPosition = new Vector2(posi.x + 120,posi.y);
            Vector2 posi = MainUICon.instance.eventSystem.currentSelectedGameObject.MMGetComponentNoAlloc<RectTransform>().position;

            //サイズ変更
            mine.position = new Vector2(posi.x + 30, posi.y);
            isFirst = true;
            

        }
        else if (!MainUICon.instance.isTips)
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
