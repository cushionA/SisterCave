using UnityEngine.AddressableAssets;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class CastEffectController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] AssetReference endEffect;

    // Update is called once per frame
    void Update()
    {
        if (!SManager.instance.castNow)

        {    

            Addressables.InstantiateAsync(endEffect,this.transform.position,transform.rotation);

            Addressables.ReleaseInstance(this.gameObject);
        }

    }


}
