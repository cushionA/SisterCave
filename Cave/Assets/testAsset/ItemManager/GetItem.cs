﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetItemScript : MonoBehaviour
{

    string playerTag = "Player";


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == playerTag && GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction18))
        {
           

        }

    }


}