using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Micosmo.SensorToolkit;

public class AggresiveSerch : MonoBehaviour
{

    [SerializeField] SisterBrain sister;
    [SerializeField] float pulseWait;
    float pulseTime;

    //  public float SerchRadius;
  //  [SerializeField]
   // private LayerMask layerMask;

    LOSSensor2D se;

    // private int layerMask = 1 << 11;//1 << 8 | 1 << 10 | 1<< 11 | 1 << 16 | 1 <<　9;

    void Start()
    {
     //   sister = GetComponentInParent<SisterBrain>();
        se = GetComponent<LOSSensor2D>();
    }

    public void SerchEnemy()
    {
        
      //  Debug.Log("調査");


       // for (int i = 0; i < se.Detections.Count; i++)
        //{
        //    SManager.instance.TargetAdd(se.GetDetectionsByDistance() [i]);
          //  SManager.instance.targetCondition.Add(SManager.instance.targetList[i].GetComponent<EnemyBase>());
        //}

    }

    private void FixedUpdate()
    {
        pulseTime += Time.fixedDeltaTime;
        if (pulseTime >= pulseWait)
        {
            //Debug.Log("機能してますよー");
            se.Pulse();
            SManager.instance.isSerch = true;
            pulseTime = 0;
        }

    }


}
