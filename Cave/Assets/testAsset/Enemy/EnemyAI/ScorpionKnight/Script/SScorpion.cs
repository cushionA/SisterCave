using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SScorpion : EnemyBase
{
    //ジャンプしてるとき回避させるな
    //フラグの重複に気を付けて
    //当たり判定はある程度太くないといけない

    //攻撃番号説明
    //0、通常攻撃,１、横縦連撃,３、高威力のしっぽ,４、距離詰める突き

    float direX;//モーションのX方向を決める
    float direY;//モーションのｙ方向を決める
    float attackChanceTime;
    bool rareAttack;
    [SerializeField]bool test;
    // Start is called before the first frame update
    [SerializeField] int tesnum;
    protected override void Start()
    {
      //  rb = GetComponent<Rigidbody2D>();
      //  Debug.Log("mennti");
        base.Start();

    }

    // Update is called once per frame
    protected override void Update()
    {

    }

  
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }
    protected override void OnTriggerStay2D(Collider2D collision)
    {
        base.OnTriggerStay2D(collision);
    }
    void RandomDirection(int upperRes, int separate)
    {
        if (separate >= upperRes)
        {
            separate = upperRes;
        }
        if (RandomValue(0, upperRes) <= separate)
        {
            direX = 1;
        }
        else
        {
            direX = -1;
        }
    }
}
