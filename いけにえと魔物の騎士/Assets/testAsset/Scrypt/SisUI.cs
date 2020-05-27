using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SisUI : MonoBehaviour
{
    public bool sisMenu;
    public GameObject sisPanel;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!sisMenu)
        {
            Time.timeScale = 1.0f;
            sisPanel.SetActive(false);

        }
        if (sisMenu)
        {
            Time.timeScale = 0;
            sisPanel.SetActive(true);

        }


        
    }
}
