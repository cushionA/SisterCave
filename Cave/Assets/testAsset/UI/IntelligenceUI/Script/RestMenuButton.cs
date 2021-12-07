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

    public GameObject window;
    public void WindowSet()
    {
        window.SetActive(true);
        this.transform.root.gameObject.SetActive(false);
    }

}
