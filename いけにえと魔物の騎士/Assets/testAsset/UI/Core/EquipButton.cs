using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipButton : MonoBehaviour
{
    [HideInInspector]
    public Equip item;

    Vector2 position = new Vector2(70, -50);
    bool isFirst;


    EventSystem eventSystem;
    StandaloneInputModule stIn;


    // Start is called before the first frame update

    private void Start()
    {
        GameObject ev = GameObject.Find("EventSystem");
        eventSystem = ev.GetComponent<EventSystem>();
        stIn = ev.GetComponent<StandaloneInputModule>();
    }

    // Update is called once per frame
    void Update()
    {
        //  if(item.inventoryNum <= 0)
        //  {
        //      MainUI.instance.UseReBuild();
        //   }

        if (!EquipManager.instance.isUseMenu)//選択ウィンドウ出てないなら
        {
            EquipManager.instance.selectButton = eventSystem.currentSelectedGameObject;

          //  Debug.Log($"青い稲妻{EquipManager.instance.selectButton == this.gameObject}");
            if (EquipManager.instance.selectButton == this.gameObject && !isFirst)
            {
                //Debug.Log("冥王星");
                EquipManager.instance.selectItem = item;
                //position = this.GetComponent<RectTransform>().anchoredPosition;
                isFirst = true;
            }
            else
            {
                isFirst = false;
            }
        }


    }

    public void CallWindow()
    {
      //  Debug.Log($"お名前教えて{EquipManager.instance.selectButton.name}");
        if (EquipManager.instance.selectItem.inventoryNum > 0 && item != null && EquipManager.instance.selectButton == this.gameObject)
        {
            EquipManager.instance.selectItem = item;
            // position = this.GetComponent<RectTransform>().anchoredPosition;
            EquipManager.instance.selectWindow.SetActive(true);
            //EquipManager.instance.selectWindow.GetComponent<RectTransform>().anchoredPosition = position;
            //EquipManager.instance.selectWindow.GetComponent<RectTransform>().parent = EquipManager.instance.selectButton.GetComponent<RectTransform>();
            EquipManager.instance.selectWindow.transform.parent = EquipManager.instance.selectButton.transform;
            EquipManager.instance.selectWindow.GetComponent<RectTransform>().anchoredPosition = position;

            EquipManager.instance.isUseMenu = true;
        }
        else
        {

            //スカの音
        }
    }

    public void CancelWindow()
    {
        EquipManager.instance.selectWindow.SetActive(false);
        isFirst = false;
        EquipManager.instance.selectButton.GetComponent<Button>().Select();
        EquipManager.instance.isUseMenu = false;
    }


}
