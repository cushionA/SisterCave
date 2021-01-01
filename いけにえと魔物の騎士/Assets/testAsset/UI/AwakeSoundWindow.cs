using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AwakeSoundWindow : MonoBehaviour
{
    // Start is called before the first frame update


    [SerializeField] GameObject soundWindow;
    [SerializeField] Button bButton;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenSound()
    {
        soundWindow.SetActive(true);
        bButton.Select();
        MainUI.instance.masterUI.SetActive(false);
    }


}
