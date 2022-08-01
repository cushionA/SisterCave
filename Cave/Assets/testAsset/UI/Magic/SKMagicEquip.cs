using UnityEngine.UI;
using UnityEngine;
/// <summary>
/// 『装備する』のボタンの親オブジェクトの画像に配置
/// 魔法を装備する。
/// フラグによりこれ一つでシスターさんとプレイヤーを識別できる
/// </summary>
public class SKMagicEquip : MonoBehaviour
{

    MagicButton ub;
    bool isFirst;
    public Button eqButton;

    // Start is called before the first frame update
    void Start()
    {
        ub = MainUICon.instance.selectButton.GetComponent<MagicButton>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFirst)
        {
            eqButton.Select();
            isFirst = true;
        }

        if (GManager.instance.InputR.GetButtonDown(MainUICon.instance.rewiredAction17))
        {

            MagicManager.instance.isUseMenu = false;
            ub.CancelWindow();//選択ウィンドウを消す
        }


        if (GManager.instance.InputR.GetButtonDown(MainUICon.instance.rewiredAction14))
        {
            ResetWindow();
        }

    }

    public void Equip()
    {
        MagicManager.instance.isUseMenu = false;
        this.gameObject.SetActive(false);
        MainUICon.instance.MenuCancel();
       // MainUICon.instance.weaponWindow.SetActive(false);
       // MainUICon.instance.eqWindow.SetActive(true);
        if (MagicManager.instance.isKnightM)
        {
            GManager.instance.equipMagic[MagicManager.instance.setNumber] = MagicManager.instance.selectItem as PlayerMagic;
            EquipmentManager.instance.EqWeapon[MagicManager.instance.setNumber].Select();
        }
        else
        {
            SManager.instance.sisStatus.equipMagic[MagicManager.instance.setNumber] = MagicManager.instance.selectItem as SisMagic;
            EquipmentManager.instance.EqShield[MagicManager.instance.setNumber].Select();
        }
        MagicManager.instance.isKnightM = false;
        MagicManager.instance.isSisterM = false;
        isFirst = false;
        MainUICon.instance.ButtonOn();
        //  MainUICon.instance.eqButton.SetActive(true);
    }
    public void ResetWindow()
    {
        isFirst = false;
        MagicManager.instance.isUseMenu = false;
        this.gameObject.SetActive(false);
        MainUICon.instance.MenuCancel();
        MainUICon.instance.ButtonOn();
        //  MainUICon.instance.selectButton = null;
        //    EquipManager.instance.selectItem = null;
    }
}
