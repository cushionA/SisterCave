using UnityEngine.UI;
using UnityEngine;

public class EquipToolButton : MonoBehaviour
{

    public int setNumber;
    [SerializeField] private Image contentIcon;

    private void Start()
    {


        //設定されたsetList[setNumber]の数量が0になったらアイコンが暗くなる処理
    }

    private void Update()
    {
        if (GManager.instance.useList[setNumber] != null)
        {
            //this.contentIcon.sprite = GManager.instance.useList[setNumber].icon;
        }
    }


    public void ToolEquip()
    {
      //  MainUICon.instance.useWindow.SetActive(true);
        EquipmentManager.instance.InitialButton.Select();
        ToolManager.instance.setNumber = setNumber;
        ToolManager.instance.isEquipMenu = true;
      //  MainUICon.instance.eqWindow.SetActive(false);

    }

}
