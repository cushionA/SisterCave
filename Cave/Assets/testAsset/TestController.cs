using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class TestController : MonoBehaviour
{
   // List<GameObject> EnemyPosition;
    [SerializeField] List<Transform> SetPosition;

    [SerializeField] Transform startposi;

    List<GameObject> EnemyList;

    int unk;
    float waitTime = 10;
    float sisterWait = 3;

    SisterBrain sb;
    SisterFire sf;
    bool seren;

    // Start is called before the first frame update
    void Start()
    {
        GManager.instance.Player.transform.position = startposi.position;
        SManager.instance.Sister.transform.position = startposi.position;
        sb = SManager.instance.Sister.GetComponent<SisterBrain>();
        sf = SManager.instance.Sister.GetComponent<SisterFire>();
    }

    // Update is called once per frame
    void Update()
    {
        waitTime += Time.deltaTime;
        sisterWait += Time.deltaTime;
        if(waitTime >= 10 && Input.GetKeyDown("z"))
        {
            Controll();
            waitTime = 0;
        }
        if(sisterWait >= 3 && Input.GetKeyDown("x"))
        {
            if(seren == false)
            {
                sb.enabled = false;
                sf.enabled = false;
                seren = true;
            }
            else
            {
                sb.enabled = true;
                sf.enabled = true;
                seren = false;
            }
            sisterWait = 0;
        }

    }
    public void Controll()
    {

     //   Debug.Log("‚·‚µ");
       EnemyList = GameObject.FindGameObjectsWithTag("Enemy").ToList<GameObject>();

        foreach (GameObject enemy in EnemyList)
        {
            GameObject.Destroy(enemy);
        }
        EnemyList = null;
        foreach(Transform position in SetPosition)
        {
            if (unk % 2 == 0)
            {
                Addressables.InstantiateAsync("SaberScorpionKight", position);
            }
            else
            {
                Addressables.InstantiateAsync("NIdiot", position);
            }
            unk++;
    }
        unk = 0;
        GManager.instance.initialSetting();
       // SManager.instance.sisStatus.mp = SManager.instance.sisStatus.maxMp;
        GManager.instance.Player.transform.position = startposi.position;
        SManager.instance.Sister.transform.position = startposi.position;

    }

}
