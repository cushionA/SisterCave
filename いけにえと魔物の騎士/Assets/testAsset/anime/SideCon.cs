using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideCon : MonoBehaviour
{
    public GameObject[] objs;
    SpriteRenderer rd0;
    SpriteRenderer rd1;
    SpriteRenderer rd2;
    SpriteRenderer rd3;
    SpriteRenderer rd4;
    SpriteRenderer rd5;
    SpriteRenderer rd6;
    SpriteRenderer rd7;
    SpriteRenderer rd8;
    SpriteRenderer rd9;
    SpriteRenderer rd10;
    SpriteRenderer rd11;
    SpriteRenderer rd12;
    SpriteRenderer rd13;
    SpriteRenderer rd14;
    SpriteRenderer rd15;
    SpriteRenderer rd16;
    SpriteRenderer rd17;
    SpriteRenderer rd18;
    SpriteRenderer rd19;
    SpriteRenderer rd20;
    SpriteRenderer rd21;


    // Start is called before the first frame update
    void Start()
    {

        //見付けたオブジェクトに付いているSpriteRendererを取得
        rd3 = objs[0].GetComponent<SpriteRenderer>();
        rd0 = objs[1].GetComponent<SpriteRenderer>();
        rd1 = objs[2].GetComponent<SpriteRenderer>();
        rd2 = objs[3].GetComponent<SpriteRenderer>();
        rd4 = objs[4].GetComponent<SpriteRenderer>();
        rd5 = objs[5].GetComponent<SpriteRenderer>();
        rd6 = objs[6].GetComponent<SpriteRenderer>();
        rd7 = objs[7].GetComponent<SpriteRenderer>();
        rd8 = objs[8].GetComponent<SpriteRenderer>();
        rd9 = objs[9].GetComponent<SpriteRenderer>();
        rd10 = objs[10].GetComponent<SpriteRenderer>();
        rd11 = objs[11].GetComponent<SpriteRenderer>();
        rd12 = objs[12].GetComponent<SpriteRenderer>();
        rd13 = objs[13].GetComponent<SpriteRenderer>();
        rd14 = objs[14].GetComponent<SpriteRenderer>();
        /*
        rd15 = objs[15].GetComponent<SpriteRenderer>();
        rd16 = objs[16].GetComponent<SpriteRenderer>();
        rd17 = objs[17].GetComponent<SpriteRenderer>();
        rd18 = objs[18].GetComponent<SpriteRenderer>();
        rd19 = objs[19].GetComponent<SpriteRenderer>();
        rd20 = objs[20].GetComponent<SpriteRenderer>();
        rd21 = objs[21].GetComponent<SpriteRenderer>();*/


    }

    void SideOff()
    {

        rd0.enabled = false;
        rd1.enabled = false;
        rd2.enabled = false;
        rd3.enabled = false;
        rd4.enabled = false;
        rd5.enabled = false;
        rd6.enabled = false;
        rd7.enabled = false;
        rd8.enabled = false;
        rd9.enabled = false;
        rd10.enabled = false;
        rd11.enabled = false;
        rd12.enabled = false;
        rd13.enabled = false;
        rd14.enabled = false;
        /*
        rd15.enabled = false;
        rd16.enabled = false;
        rd17.enabled = false;
        rd18.enabled = false;
        rd19.enabled = false;
        rd20.enabled = false;
        rd21.enabled = false;*/

    }
    void SideOn()
    {

        rd0.enabled = true;
        rd1.enabled = true;
        rd2.enabled = true;
        rd3.enabled = true;
        rd4.enabled = true;
        rd5.enabled = true;
        rd6.enabled = true;
        rd7.enabled = true;
        rd8.enabled = true;
        rd9.enabled = true;
        rd10.enabled = true;
        rd11.enabled = true;
        rd12.enabled = true;
        rd13.enabled = true;
        rd14.enabled = true;

        /*rd15.enabled = true;
        rd16.enabled = true;
        rd17.enabled = true;
        rd18.enabled = true;
        rd19.enabled = true;
        rd20.enabled = true;
        rd21.enabled = true;*/

    }
}
