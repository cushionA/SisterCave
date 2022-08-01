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
         //   //this.contentIcon.sprite = GManager.instance.setWeapon[setNumber].icon;
        }
    }


    public void WeaponEquip()
    {
        // MainUICon.instance.isReBuild = true;
   //     MainUICon.instance.wec.isEver = false;
      //  MainUICon.instance.wec.isIniti = false;
     //   MainUICon.instance.weaponWindow.SetActive(true);
        EquipmentManager.instance.InitialButton.Select();
        EquipManager.instance.setNumber = setNumber;
        EquipManager.instance.isWeaponM = true;
    //    MainUICon.instance.eqWindow.SetActive(false);


    }
}
