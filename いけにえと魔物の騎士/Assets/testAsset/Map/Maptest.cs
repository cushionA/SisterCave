using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using PathologicalGames;

public class Maptest : MonoBehaviour
{
    Transform myInstance;
    public GameObject myPrefab;

    bool isbreak;
        float tikutaku;

    // Start is called before the first frame update
    void Start()
    {
      //  myInstance = Addressables.InstantiateAsync("TestMap77").Result;
    }

    // Update is called once per frame
    void Update()
    {
        tikutaku += Time.deltaTime;


        if (tikutaku >= 30)
        {
            if (!isbreak)
            {
                //  PathologicalGames.SpawnPool.Despawn(myInstance);

                myInstance = PoolManager.Pools["testmap"].Spawn(myPrefab.transform);
              //  PoolManager.Pools["testmap"].Spawn(myPrefab.transform);
                tikutaku = 0.0f;
                isbreak = true;
            }
            else
            {
                // Despawn an instance (makes an instance available for reuse)
                PoolManager.Pools["Shapes"].Despawn(myInstance);
                tikutaku = 0.0f;
                isbreak = false;
            }

        }
    }
}
