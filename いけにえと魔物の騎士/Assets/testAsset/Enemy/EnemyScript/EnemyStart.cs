using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// スプライトに貼る。レンダラー使うため
/// </summary>
public class EnemyStart : MonoBehaviour
{
    EnemyBase eb;
    [System.NonSerialized] public bool cameraEnabled = false;
    [System.NonSerialized] public bool inActiveZone = false;
    protected Rigidbody2D rb;
    string avtiveTag = "ActiveZone";

    bool activeEnter;
    bool activeStay;
    bool isSleep;
    Animator ani;

    void Start()
    {
        eb = GetComponent<EnemyBase>();
        rb = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
    }

    // Update is called once per frame
    void OnWillRenderObject()
    {
        if (Camera.current.tag == "MainCamera")
        {
            rb.WakeUp();
            eb.cameraRendered = true;
            cameraEnabled = true;
        }
    }

    private void FixedUpdate()
    {

        Debug.Log($"カメラないでーす{eb.cameraRendered}");
        Debug.Log($"カメラな{activeStay}");
        if (!cameraEnabled)
        {
            rb.Sleep();
            isSleep = true;
            ani.enabled = false;
        }
        if (eb.cameraRendered && !isSleep)
        {
           rb.WakeUp();
            isSleep = false;
        }

        if (cameraEnabled)
        {
            if (activeEnter || activeStay)
            {
                eb.cameraRendered = true;

            }
            else
            {
                eb.cameraRendered = false;
                rb.Sleep();
                isSleep = true;
                ani.enabled = false;
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
