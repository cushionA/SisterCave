using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownWindow : ValueChangeBase
{

    bool reset;



    // Start is called before the first frame update

    private void OnDisable()
    {
        reset = false;
    }

    public void ApplyDrop()
    {
       // Debug.Log("sssdfggh");
        if (!reset)
        {
            reset = true;
        }
        else
        {
        //    Debug.Log("dkfjg");
            Dropdown d = this.gameObject.GetComponent<Dropdown>();
            numberSave = d.value;
            ApplyValue();
        }
    }

}
