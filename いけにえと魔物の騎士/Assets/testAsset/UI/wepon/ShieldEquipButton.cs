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
            this.contentIcon.sprite = GManager.instance.setShield[setNumber].icon;
        }
    }


    public void ShieldEquip()
    {
        // MainUI.instance.isReBuild = true;
        MainUI.instance.wec.isEver = false;
        MainUI.instance.weponWindow.SetActive(true);
        EquipmentManager.instance.InitialButton.Select();
        EquipManager.instance.setNumber = setNumber;
        EquipManager.instance.isShieldM = true;
        MainUI.instance.eqWindow.SetActive(false);


    }
}
