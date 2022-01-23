using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipsWindow : MonoBehaviour
{

    bool isFirst;

    [TextArea]
    public string description;

    // Start is called before the first frame update
    void Start()
    {
        
    }



    // Update is called once per frame
    void Update()
    {
        if(MainUI.instance.eventSystem.currentSelectedGameObject == this.gameObject)
        {
            isFirst = true;
            MainUI.instance.isTips = true;
        }
        else if (isFirst)
        {
            MainUI.instance.isTips = false;
            isFirst = false;
        }
        
    }
}
