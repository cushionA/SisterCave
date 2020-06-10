using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
public class SisterAttack : MonoBehaviour
{

    public GameObject ef;
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

        if (Input.GetButtonDown("SFire"))
        {
            if(eqMagi.GetItemName() == "聖火")
            {

                Debug.Log("魔法");
                Addressables.LoadAssetAsync<GameObject>("Fire");
                Addressables.InstantiateAsync("Fire");

            }



        } 
        
    }
}
