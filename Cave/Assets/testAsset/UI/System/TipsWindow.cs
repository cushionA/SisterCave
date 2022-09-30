using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipsWindow : MonoBehaviour
{

    bool isFirst;

    [TextArea]
    public string description;





    // Update is called once per frame
    void Update()
    {
        if(MainUICon.instance.eventSystem.currentSelectedGameObject == this.gameObject)
        {
         //   Debug.Log($"fdfdfd");
            isFirst = true;
            MainUICon.instance.isTips = true;
        }
        else if (isFirst)
        {
            MainUICon.instance.isTips = false;
            isFirst = false;
        }
        
    }
}
