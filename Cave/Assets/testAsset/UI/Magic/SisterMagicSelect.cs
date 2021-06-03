using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// シスターさんの魔法選択の起点になるボタンにそれぞれ配置する。
/// 魔法インベントリを呼び出す
/// </summary>
public class SisterMagicSelect : MonoBehaviour
{
    public int setNumber;
    [SerializeField] private Image contentIcon;

    private void Start()
    {

        //設定されたsetList[setNumber]の数量が0になったらアイコンが暗くなる処理
        //魔法はいらねーだろこの処理
    }

    private void Update()
    {
        if (SManager.instance.sisStatus.equipMagic[setNumber] != null)
        {
            this.contentIcon.sprite = SManager.instance.sisStatus.equipMagic[setNumber].icon;
        }
    }


    public void PMagicEquip()
    {
        // MainUI.instance.isReBuild = true;
        MainUI.instance.mac.isEver = false;
        MainUI.instance.mac.isIniti = false;
        MainUI.instance.magicWindow.SetActive(true);
        EquipmentManager.instance.InitialButton.Select();
        MagicManager.instance.setNumber = setNumber;
        MagicManager.instance.isSisterM = true;
        MainUI.instance.eqWindow.SetActive(false);
    }
}
