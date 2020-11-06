using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
public class SisterAttack : MonoBehaviour
{

    public AssetReference fi_ref;
    public SisterStatus sst;
    float sisterFireKey;
    public SisMagic[] eqMagi;
    Item useMagi;

    [SerializeField]
    GenericPool generator;

    [SerializeField] GameObject _eff;

    // Start is called before the first frame update
    void Start()
    {
        sst.useMagic =eqMagi[0];
    }

    // Update is called once per frame
    void Update()
    {
        useMagi = sst.useMagic;

        sisterFireKey = Input.GetAxisRaw("SFire");

        if (GManager.instance.InputR.GetButtonDown("SFire"))
        {
            if(useMagi.itemName == "聖火")
            {

                Addressables.InstantiateAsync(fi_ref);
                //generator.Place(_eff.transform.position);

            }



        } 
        
    }
}
