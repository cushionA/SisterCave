using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GManager : MonoBehaviour
{
    public static GManager instance = null;

    public GameObject Player;
    //プレイヤーオブジェクト
    public PlayerStatus pStatus;
    //プレイヤーのステータスを取得
    public int stRecover = 3;
    //スタミナ回復量
    public Slider stSlider;
    //スタミナスライダー
    public Slider HpSlider;
    //HPスライダー
    [HideInInspector] public bool isEnable;
    //スタミナが回復するかどうか
    [HideInInspector] public bool isAttack;
    //攻撃中か否か

    float stTime;
    //スタミナが回復する間隔の時間が経過したかどうか
    float disEnaTime;
    //スタミナ回復不能時間
    AttackM at;
    PlayerMove pm;
    bool stBreake;
    //スタミナ回復不能状態終わりフラグ

    //プレイヤーがレベルアップしたらHPとスタミナのスライダーの長さをチェックして伸ばす。
    //あとステータス画面に格納する値のチェックも

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
        HpSlider.value = 1;
        //HPなどを最大と同じに
        pStatus.hp = pStatus.maxHp;
        pStatus.stamina = pStatus.maxStamina;
    }

    // Update is called once per frame
    void Update()
    {
        stTime += Time.deltaTime;



        if (stTime >= 0.1f && !pm.isStUse && isEnable)
        {
            //前回のスタミナ回復から0.1秒経っててスタミナ使ってなくてスタミナ回復できるフラグあるなら
            disEnaTime = 0.0f;

            pStatus.stamina += stRecover;
            stTime = 0.0f;
            stBreake = false;

        }




        else if (pStatus.stamina >= pStatus.maxStamina)
        {

            pStatus.stamina = pStatus.maxStamina;
            isEnable = true;
        }

        else if (pStatus.stamina <= 0 && !stBreake)
        {

            disEnaTime += Time.deltaTime;

            isEnable = false;

            if (disEnaTime < 1.5f || pm.isStUse)
            {
               
                pStatus.stamina = 0;
                //ここ
            }
            else
            {
                stBreake = true;

            }

        }
        else
        {


            isEnable = true;
        }

        stSlider.value = pStatus.stamina / pStatus.maxStamina;
        HpSlider.value = pStatus.hp / pStatus.maxHp;
    }

    public void StaminaUse(int useStamina)
    {
        pStatus.stamina -= useStamina;
    }

    public void HpReduce(float damage)
    {
        pStatus.hp -= damage;
    }
    public void HpRecover(float Recovery)
    {
        pStatus.hp += Recovery;
    }

}
