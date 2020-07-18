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
    public Item[] eqMagi;
    Item useMagi;


    // Start is called before the first frame update
    void Start()
    {
        sst.SetUseMagic(eqMagi[0]);
    }

    // Update is called once per frame
    void Update()
    {
        useMagi = sst.GetUseMagic();

        sisterFireKey = Input.GetAxisRaw("SFire");

        if (Input.GetButtonDown("SFire"))
        {
            if(useMagi.GetItemName() == "聖火")
            {

                Debug.Log("魔法");
                Addressables.LoadAssetAsync<GameObject>("Fire");
                Addressables.InstantiateAsync("Fire");

            }



        } 
        
    }
}
