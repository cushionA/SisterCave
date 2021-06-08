using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// スプライトに貼る。レンダラー使うため
/// </summary>
public class EnemyStart : MonoBehaviour
{
    EnemyBase eb;
    [System.NonSerialized] public bool cameraEnabled = false;//一回カメラに映った
    [System.NonSerialized] public bool inActiveZone = false;
   // protected Rigidbody2D eb.rb;
    string avtiveTag = "ActiveZone";

    bool activeEnter;
    bool activeStay;
    //bool activeOut;
    bool isSleep;//休眠中フラグ
   // Animator ani;

    void Start()
    {
        eb = GetComponent<EnemyBase>();
        eb.rb = GetComponent<Rigidbody2D>();
        //ani = GetComponent<Animator>();
    }

    // Update is called once per frame
    void OnWillRenderObject()
    {
      //  if (Camera.current.tag == "MainCamera")レンダリングしてるカメラがメインカメラならってやつ。しかしおそらくURPのせいで動かない
       // {
            
            eb.cameraRendered = true;
            cameraEnabled = true;

        //  }
    }

    private void FixedUpdate()
    {

        ////Debug.log($"カメラないでーす{eb.cameraRendered}");
        ////Debug.log($"カメラな{activeStay}");
        if (!cameraEnabled && !isSleep)
        {
            //カメラにうつってない
            eb.rb.Sleep();
            isSleep = true;
            //Debug.Log("あああｓで");
            eb.sAni.enabled = false;
            eb.cameraRendered = false;
            return;
        }
        if (eb.cameraRendered && isSleep)
        { 

           eb.rb.WakeUp();
            //isSleep = false;
            isSleep = false;
            eb.sAni.enabled = true;
        }

        if (cameraEnabled)
        {
            if (activeEnter || activeStay)
            {
                //アクティブゾーンにいるなら
                eb.cameraRendered = true;
               //activeOut = false;
            }
            else
            {
              //  eb.cameraRendered = false;
              //  eb.rb.Sleep();
              //  isSleep = true;
              //  eb.sAni.enabled = false;
                cameraEnabled = false;
                
            }
            activeStay = false;
            activeEnter = false;

        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == avtiveTag)
        {
            activeEnter = true;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == avtiveTag)
        {
            activeStay = true;
        }
    }


}
