using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SisterAttack : MonoBehaviour
{

    public SisterStatus sst;
    float sisterFireKey;
    Item eqMagi;


    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        eqMagi = sst.GetEquipMagic();

        sisterFireKey = Input.GetAxisRaw("SFire");

        if (sisterFireKey > 0.0f)
        {
            if(eqMagi.GetItemName() == "聖火")
            {
                Debug.Log("大成功");

            }



        } 
        
    }
}
