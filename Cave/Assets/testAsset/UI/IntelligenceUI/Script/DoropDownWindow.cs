using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownWindow : ValueChangeBase
{


    

    // Start is called before the first frame update
    protected override void Start()
    {
        

    }

    void ApplyDrop()
    {

        Dropdown d = this.gameObject.GetComponent<Dropdown>();
        numberSave = d.value;
        ApplyValue();
    }

}
