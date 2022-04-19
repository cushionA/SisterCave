using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;

public class BattleTest : MonoBehaviour
{
    [SerializeField] GameObject Enemy;
    GameObject sis;
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
            if (sis != null)
            {
                Enemy = sis.gameObject;
            }
            }
        //GameObject del = Enemy.gameObject;
        if (Enemy != null)
        {
            Destroy(Enemy);
        }

    }

    public async  UniTaskVoid ReSporn()
    {
        
        sis = await Addressables.InstantiateAsync(EnemyData);
        //  Enemy = dd;
        sis.transform.position = this.gameObject.transform.position;
        Debug.Log($"{sis.name}");
    }

}
