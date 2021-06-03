using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimmickAct : MonoBehaviour
{
    //プレイヤーオブジェクトの子オブジェクトの判定用のオブジェクトにアタッチする
    //ギミック利用可能か、またギミックを利用中かを判定するスクリプト

    //ギミック利用中のフラグ。これが立っているとプレイヤーコントローラーのあらゆるアクションが不可能になる
    //またisDashやisJumpなどのあらゆるフラグがfalseされてアクションの計測されていた持続時間等も白紙にしたうえでreturnするように
    //プレイヤーコントローラーを停止してギミック側の処理でプレイヤーを動かすことでいろんなギミックに対応できるようにしたかった

     [HideInInspector]public bool isGimmick;
    //接地判定と似たようなギミック利用可能かどうかのフラグ

    bool isGimmickEnter;
    bool isGimmickStay;
    bool isGimmickExit;
    string playerTag = "Player";
    //Gimmickタグのオブジェクトの接触を検知する



    void FixedUpdate()
    {
        if (isGimmickEnter || isGimmickStay)
        {
            isGimmick = true;
            //ギミック利用可能
        }
        else// if (isGimmickExit)
        {
            isGimmick = false;
            //ギミック利用不可能
        }
        isGimmickEnter = false;
        isGimmickStay = false;
        isGimmickExit = false;
        //次の判定まで初期化


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //ギミックを利用可能か判定
        if (collision.tag == playerTag)
        {
            isGimmickEnter = true;

        }


    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == playerTag)
        {
            isGimmickStay = true;

        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == playerTag)
        {
            isGimmickExit = true;
        }

    }


}
