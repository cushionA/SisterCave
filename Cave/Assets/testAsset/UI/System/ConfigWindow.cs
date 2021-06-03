using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ConfigWindow : MonoBehaviour
{
    public GameObject mUI;
    public Button configButton;
    public Button doneButton;

    public void CloseEvent()
    {
        MainUI.instance.selectWindow = false;
        mUI.SetActive(true);
        configButton.Select();
    }

    public void OpenEvent()
    {
        MainUI.instance.selectWindow = true;
        mUI.SetActive(false);
        doneButton.Select();
    }

}
