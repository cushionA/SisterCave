using UnityEngine.UI;
using UnityEngine;

public class WeaponEquipButton : MonoBehaviour
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
        if (GManager.instance.setWeapon[setNumber] != null)
        {
            this.contentIcon.sprite = GManager.instance.setWeapon[setNumber].icon;
        }
    }


    public void WeaponEquip()
    {
        // MainUI.instance.isReBuild = true;
        MainUI.instance.wec.isEver = false;
        MainUI.instance.wec.isIniti = false;
        MainUI.instance.weaponWindow.SetActive(true);
        EquipmentManager.instance.InitialButton.Select();
        EquipManager.instance.setNumber = setNumber;
        EquipManager.instance.isWeaponM = true;
        MainUI.instance.eqWindow.SetActive(false);


    }
}
