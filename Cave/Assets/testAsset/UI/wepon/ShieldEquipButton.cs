using UnityEngine.UI;
using UnityEngine;

public class ShieldEquipButton : MonoBehaviour
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
        if (GManager.instance.setShield[setNumber] != null)
        {
            //this.contentIcon.sprite = GManager.instance.setShield[setNumber].icon;
        }
    }


    public void ShieldEquip()
    {
        // MainUICon.instance.isReBuild = true;
     //   MainUICon.instance.wec.isEver = false;
  //      MainUICon.instance.weaponWindow.SetActive(true);
        EquipmentManager.instance.InitialButton.Select();
        EquipManager.instance.setNumber = setNumber;
        EquipManager.instance.isShieldM = true;
       // MainUICon.instance.eqWindow.SetActive(false);


    }
}
