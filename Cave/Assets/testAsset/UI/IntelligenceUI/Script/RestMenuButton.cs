using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestMenuButton : MonoBehaviour
{

    /// <summary>
    /// 進む先だけでいいな
    /// これはいろんなボタンに使えるかもね
    /// </summary>

    [SerializeField]
    RestPoint res;

    [SerializeField]
    bool master;
    //次開くやつ
    public GameObject window;

    private void OnEnable()
    {
        if (master)
        {
            if (!res.isFirst)
            {
              //   Debug.Log($"aas");

                res.isFirst = true;
            }
        }
    }

    public void WindowSet()
    {
       // Debug.Log("sssssssss");
        window.SetActive(true);
        this.transform.root.gameObject.SetActive(false);
        res.isFirst = false;
    }

    public void SisterOff()
    {
        if (SManager.instance.Sister.activeSelf)
        {
            SManager.instance.Sister.SetActive(false);
        }
        else
        {
            SManager.instance.Sister.SetActive(true);
        }
    }


}
