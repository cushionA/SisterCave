using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestMenuButton : MonoBehaviour
{

    /// <summary>
    /// �i�ސ悾���ł�����
    /// ����͂����ȃ{�^���Ɏg���邩����
    /// </summary>

    public GameObject window;
    public void WindowSet()
    {
        window.SetActive(true);
        this.transform.root.gameObject.SetActive(false);
    }

}
