using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.UI;

public class DropDownWindow : ValueChangeBase
{


    Dropdown d;


    private void Start()
    {
        d = this.gameObject.MMGetComponentNoAlloc<Dropdown>();
    }
    // Start is called before the first frame update


    public void ApplyDrop()
    {


            
            numberSave = d.value;
            ApplyValue();

    }

}
