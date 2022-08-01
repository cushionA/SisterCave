using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class MaterialButton : MonoBehaviour
{
    [HideInInspector]
    public MaterialItem item;

    Vector2 position = new Vector2(70, -50);
    bool isFirst;


 //   EventSystem eventSystem;
  //  StandaloneInputModule stIn;


    // Start is called before the first frame update

    private void Start()
    {
        //GameObject ev = GameObject.Find("EventSystem");
      //  eventSystem = ev.GetComponent<EventSystem>();
     //   stIn = ev.GetComponent<StandaloneInputModule>();
    }

    // Update is called once per frame
    void Update()
    {
        //  if(5 <= 0)
        //  {
        //      MainUICon.instance.UseReBuild();
        //   }

        if (!MaterialManager.instance.isUseMenu)//選択ウィンドウ出てないなら
        {
            MainUICon.instance.selectButton = MainUICon.instance.eventSystem.currentSelectedGameObject;

            //  ////Debug.log($"青い稲妻{MainUICon.instance.selectButton == this.gameObject}");
            if (MainUICon.instance.selectButton == this.gameObject && !isFirst)
            {
                //////Debug.log("冥王星");
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
        //  ////Debug.log($"お名前教えて{MainUICon.instance.selectButton.name}");
        if (5/*5*/ > 0 && item != null && MainUICon.instance.selectButton == this.gameObject)
        {
            MaterialManager.instance.selectItem = item;
            // position = this.GetComponent<RectTransform>().anchoredPosition;
            MaterialManager.instance.selectWindow.SetActive(true);
            //MaterialManager.instance.selectWindow.GetComponent<RectTransform>().anchoredPosition = position;
            //MaterialManager.instance.selectWindow.GetComponent<RectTransform>().parent = MainUICon.instance.selectButton.GetComponent<RectTransform>();
            MaterialManager.instance.selectWindow.transform.parent = MainUICon.instance.selectButton.transform;
            MaterialManager.instance.selectWindow.GetComponent<RectTransform>().anchoredPosition = position;

            MaterialManager.instance.isUseMenu = true;
           // MainUICon.instance.openWindow = true;
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
        MainUICon.instance.selectButton.GetComponent<Button>().Select();
        MaterialManager.instance.isUseMenu = false;
    }


}
