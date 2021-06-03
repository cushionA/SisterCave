using UnityEngine.UI;
using UnityEngine;

public class WeponEquipButton : MonoBehaviour
{

    public int setNumber;
    [SerializeField] private Image contentIcon;

    private void Start()
    {


        //設定されたsetList[setNumber]の数量が0になったらアイコンが暗くなる処理
        //武器はいらねーだろこの処理
    }

    private void Update()
    {
        if (GManager.instance.setWepon[setNumber] != null)
        {
            this.contentIcon.sprite = GManager.instance.setWepon[setNumber].icon;
        }
    }


    public void WeponEquip()
    {
        // MainUI.instance.isReBuild = true;
        MainUI.instance.wec.isEver = false;
        MainUI.instance.wec.isIniti = false;
        MainUI.instance.weponWindow.SetActive(true);
        EquipmentManager.instance.InitialButton.Select();
        EquipManager.instance.setNumber = setNumber;
        EquipManager.instance.isWeponM = true;
        MainUI.instance.eqWindow.SetActive(false);


    }
}
