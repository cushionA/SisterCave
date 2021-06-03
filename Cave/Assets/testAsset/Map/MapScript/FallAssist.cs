using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallAssist : MonoBehaviour
{

    [SerializeField] GameObject fObj;
    FallTrap ft;

    string groundTag = "Ground";
    string playerTag = "Player";

    // Start is called before the first frame update
    void Start()
    {
        ft = fObj.GetComponent<FallTrap>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == groundTag && !ft.isDown)
        {
            ft.isFirst = false;
            ft.waitTime = 0.0f;
            ft.isDown = true;
        }

        if (collision.tag == playerTag)
        {


        }

    }


}
