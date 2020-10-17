using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class MaterialButton : MonoBehaviour
{
    [HideInInspector]
    public MaterialItem item;

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

        if (!MaterialManager.instance.isUseMenu)//選択ウィンドウ出てないなら
        {
            MaterialManager.instance.selectButton = eventSystem.currentSelectedGameObject;

            //  Debug.Log($"青い稲妻{MaterialManager.instance.selectButton == this.gameObject}");
            if (MaterialManager.instance.selectButton == this.gameObject && !isFirst)
            {
                //Debug.Log("冥王星");
                MaterialManager.instance.selectItem = item;
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
        //  Debug.Log($"お名前教えて{MaterialManager.instance.selectButton.name}");
        if (MaterialManager.instance.selectItem.inventoryNum > 0 && item != null && MaterialManager.instance.selectButton == this.gameObject)
        {
            MaterialManager.instance.selectItem = item;
            // position = this.GetComponent<RectTransform>().anchoredPosition;
            MaterialManager.instance.selectWindow.SetActive(true);
            //MaterialManager.instance.selectWindow.GetComponent<RectTransform>().anchoredPosition = position;
            //MaterialManager.instance.selectWindow.GetComponent<RectTransform>().parent = MaterialManager.instance.selectButton.GetComponent<RectTransform>();
            MaterialManager.instance.selectWindow.transform.parent = MaterialManager.instance.selectButton.transform;
            MaterialManager.instance.selectWindow.GetComponent<RectTransform>().anchoredPosition = position;

            MaterialManager.instance.isUseMenu = true;
        }
        else
        {

            //スカの音
        }
    }

    public void CancelWindow()
    {
        MaterialManager.instance.selectWindow.SetActive(false);
        isFirst = false;
        MaterialManager.instance.selectButton.GetComponent<Button>().Select();
        MaterialManager.instance.isUseMenu = false;
    }


}
