using UnityEngine.UI;
using UnityEngine;

public class MaterialNumberChange : MonoBehaviour
{

    [HideInInspector] public MaterialWindow uw;
    [HideInInspector] public bool isDump;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isDump)
        {

            if (GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction17))
            {
                isDump = false;
                uw.isDump = false;
                uw.thirdButton.Select();
                this.gameObject.SetActive(false);
            }
        }


        if (GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction14))
        {
            ResetFlag();
            uw.ResetWindow();
        }
    }

    public void DumpAct()
    {
        isDump = false;
        uw.isDump = false;
        MaterialManager.instance.DumpMaterial();
        MaterialManager.instance.changeNum = 1;
        MaterialManager.instance.selectButton.GetComponent<Button>().Select();
        uw.ResetWindow();
        this.gameObject.SetActive(false);
        MainUI.instance.MenuCancel();
    }

    public void ResetFlag()
    {
        //       MaterialManager.instance.selectButton = null;
        //     MaterialManager.instance.selectItem = null;
        isDump = false;
        this.gameObject.SetActive(false);
    }


}
