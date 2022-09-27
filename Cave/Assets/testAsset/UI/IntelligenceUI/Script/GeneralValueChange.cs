using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Tools;

public class GeneralValueChange : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    int Type;

    [SerializeField]
     GameObject nextWin;

    [SerializeField]
    Selectable next;

    [SerializeField]
    Selectable back;

    void OnEnable()
    {
        if (MainUI.instance != null)
        {
            ValueApply();
        }
        else
        {
            Debug.Log("ann");
        }
    }

    // Update is called once per frame
    void Update()
    {

        if(Type == 4)
        { 
        if (MainUICon.instance._reInput.CancelButton.State.CurrentState == MMInput.ButtonStates.ButtonDown && MainUICon.instance.settingNumber != 100 && !MainUICon.instance.isAH)
        {
                // back.Select();
                transform.root.gameObject.MMGetComponentNoAlloc<OperationGeneral>().SelectReset();

              //  Debug.Log("ssss2");
            }
            if (MainUICon.instance.eventSystem.currentSelectedGameObject == this.gameObject)
            {

                MainUICon.instance.settingNumber = 0;
            }
        }
    }

    public void ValueChange()
    {
        if(Type == 0)
        {
            int d = this.gameObject.MMGetComponentNoAlloc<Dropdown>().value;

            if (d == 0)
            {
                MainUICon.instance.editParameter.priority = SisterParameter.MoveType.çUåÇ;
            }
            else if (d == 1)
            {
                MainUICon.instance.editParameter.priority = SisterParameter.MoveType.éxâá;
            }
            else 
            {
                MainUICon.instance.editParameter.priority = SisterParameter.MoveType.âÒïú;
            }
        }
        else if(Type == 1)
        {
            MainUICon.instance.editParameter.stateResetRes = this.gameObject.MMGetComponentNoAlloc<Slider>().value;
        }
        else if (Type == 2)
        {
            MainUICon.instance.editParameter.targetResetRes = this.gameObject.MMGetComponentNoAlloc<Slider>().value;
        }
        else if(Type == 3)
        {
            int d = this.gameObject.MMGetComponentNoAlloc<Dropdown>().value;

            if (d == 0)
            {
                MainUICon.instance.editParameter.autoHeal = true;
            }
            else
            {
                MainUICon.instance.editParameter.autoHeal = false;
            }
        }
        else
        {
            MainUICon.instance.isAH = true;
            nextWin.SetActive(true);
            next.Select();
            MainUICon.instance.settingNumber = 5;
        }
        MainUICon.instance.isSave = false;
    }

    /// <summary>
    /// ílÇìKóp
    /// </summary>
    public void ValueApply()
    {
        if (Type == 0)
        {
            Dropdown d = this.gameObject.MMGetComponentNoAlloc<Dropdown>();

            //Debug.Log($"{MainUICon.instance.editParameter.priority}");
           if(MainUI.instance == null)
            {
                Debug.Log("Ç‘ÇÒ");
            }
            if (MainUICon.instance.editParameter.priority == SisterParameter.MoveType.çUåÇ)
            {
                d.value = 0;
            }
            else if (MainUICon.instance.editParameter.priority == SisterParameter.MoveType.éxâá)
            {
                d.value = 1;
            }
            else
            {
                d.value = 2;
            }
        }
        else if (Type == 1)
        {
            this.gameObject.MMGetComponentNoAlloc<Slider>().value = MainUICon.instance.editParameter.stateResetRes;
        }
        else if (Type == 2)
        {
            this.gameObject.MMGetComponentNoAlloc<Slider>().value = MainUICon.instance.editParameter.targetResetRes;
        }
        else if(Type == 3)
        {
            Dropdown d = this.gameObject.MMGetComponentNoAlloc<Dropdown>();

            if (MainUICon.instance.editParameter.autoHeal == true)
            {
                d.value = 0;
            }
            else
            {
                d.value = 1;
            }
        }
    }
}
