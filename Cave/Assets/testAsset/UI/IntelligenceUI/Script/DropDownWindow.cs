using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownWindow : ValueChangeBase
{



    

    // Start is called before the first frame update


    public void ApplyDrop()
    {

        Dropdown d = this.gameObject.GetComponent<Dropdown>();
        numberSave = d.value;
        ApplyValue();
    }

}
