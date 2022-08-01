using UnityEngine.UI;
using UnityEngine;

public class CoreEquipButton : MonoBehaviour
{

    [SerializeField] private Image contentIcon;

    private void Start()
    {


        //設定されたsetList[setNumber]の数量が0になったらアイコンが暗くなる処理
    }

    private void Update()
    {
        if (GManager.instance.pStatus.equipCore != null)
        {
           // //this.contentIcon.sprite = GManager.instance.pStatus.equipCore.icon;
        }
    }


    public void EquipCore()
    {
     //   MainUICon.instance.coreWindow.SetActive(true);
        EquipmentManager.instance.InitialButton.Select();
        CoreManager.instance.isEquipMenu = true;
      //  MainUICon.instance.eqWindow.SetActive(false);
    }

}
