using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;

public class BattleTest : MonoBehaviour
{
    public GameObject Enemy;

    [SerializeField] bool isBattle;

    [SerializeField]
    AssetReference EnemyData;

    // Start is called before the first frame update
    void Start()
    {
        if ( Enemy != null)
        {
            Enemy.transform.position = this.gameObject.transform.position;
         //   GManager.instance.Player.transform.position = this.gameObject.transform.position;
        }
    }

    // Update is called once per frame


    public void Reset()
    {
        //   GameObject dd;

            if(Enemy == null)
            {
            return;
            }
        //GameObject del = Enemy.gameObject;
        else
        {
            Destroy(Enemy);
        }

    }

    public async  UniTaskVoid ReSporn()
    {
        
        Enemy = await Addressables.InstantiateAsync(EnemyData);
        //  Enemy = dd;
        Enemy.transform.position = this.gameObject.transform.position;
        Debug.Log($"{Enemy.name}");
    }

}
