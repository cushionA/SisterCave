using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    List<GameObject> EnemyList;

    [SerializeField]
   // [AssetReferenceUILabelRestriction]
    List<AssetReference> EnemyData;

    [SerializeField]
    List<Transform> PointList;

    [SerializeField]
    List<BattleTest> bt;

    // Start is called before the first frame update

    int isEnd;

    public void ResetEnemy()
    {
        GManager.instance.hp = GManager.instance.maxHp;
        GManager.instance.mp = GManager.instance.maxMp;
        SManager.instance.sisStatus.mp = SManager.instance.sisStatus.maxMp;

        //bool isEnd = false;
        if (isEnd == 0)
        {
            for (int i = 0; i < EnemyList.Count; i++)
            {
                Destroy(EnemyList[i]);
                isEnd = i == 4 ? 1 : isEnd;
            }
            
        }
        if (isEnd == 1)
        {
            for (int i = 0; i < PointList.Count; i++)
            {

                if (i == 0)
                {
                    // EnemyList.Add(.Result);
                    Addressables.InstantiateAsync(EnemyData[i], PointList[0]);
                    // EnemyList.Add(PointList[i].transform.GetChild(0).gameObject);
                }
                else if (i > 0 && i < 4)
                {
                    //EnemyList.Add(.Result);
                    Addressables.InstantiateAsync(EnemyData[1], PointList[i]);
                    // EnemyList.Add(PointList[i].transform.GetChild(0).gameObject);
                }
                else
                {
                    //EnemyList.Add(.Result);
                    Addressables.InstantiateAsync(EnemyData[2], PointList[i]);
                    //EnemyList.Add(PointList[i].transform.GetChild(0).gameObject);
                }
                // EnemyList.Add(PointList[i].transform.GetChild(0).gameObject);
                isEnd = i == 4 ? 2 : isEnd;
            }
        }
        if (isEnd == 2)
        {
            EnemyList[0] = (PointList[0].transform.GetChild(0).gameObject);
            EnemyList[1] =(PointList[1].transform.GetChild(0).gameObject);
            EnemyList[2] = (PointList[2].transform.GetChild(0).gameObject);
            EnemyList[3] = (PointList[3].transform.GetChild(0).gameObject);
            EnemyList[4] = (PointList[4].transform.GetChild(0).gameObject);
            isEnd = 0;
        }

    }


    public void AnotherReset()
    {
        GManager.instance.hp = GManager.instance.maxHp;
        GManager.instance.mp = GManager.instance.maxMp;
        SManager.instance.sisStatus.mp = SManager.instance.sisStatus.maxMp;
        for (int i = 0;i < bt.Count;i++)
        {
            bt[i].Reset();
            _ = bt[i].ReSporn();
        }
    }
}
