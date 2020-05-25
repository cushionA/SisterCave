using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GManager : MonoBehaviour
{
    public static GManager instance = null;

    public GameObject Player;


    public float stamina;
    public float stRecover;
    [HideInInspector] public float currentSt;
    public Slider stSlider;
    [HideInInspector] public bool isEnable;

    float stTime;

    AttackM at;
    PlayerMove pm;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

void Start()
    {
      at = Player.GetComponent<AttackM>();
        pm = Player.GetComponent<PlayerMove>();

        //スライダーを満タンに
        stSlider.value = 1;
        //現在のスタミナを最大と同じに
        currentSt = stamina;
    }

    // Update is called once per frame
    void Update()
    {
        stTime += Time.deltaTime;


      
        if(stTime >= 0.1f  && !at.isAttack && !pm.isDash && !pm.isJump)
        {
            currentSt += stRecover;
            stTime = 0.0f;
        }

     
        
        if(currentSt <= 0) {

            isEnable = false;
            currentSt = 0.0f;
        }
        else if(currentSt >= stamina)
        {

            currentSt = stamina;
            isEnable = true;
        }

        else {


            isEnable = true;
        }

        stSlider.value = currentSt / stamina;

    }
}
