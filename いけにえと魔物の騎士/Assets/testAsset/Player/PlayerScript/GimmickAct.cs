using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimmickAct : MonoBehaviour
{
    //プレイヤーオブジェクトの子オブジェクトの判定用のオブジェクトにアタッチする
    //ギミック利用可能か、またギミックを利用中かを判定するスクリプト

    [HideInInspector] public bool isGimmickOn;
    //ギミック利用中のフラグ。これが立っているとプレイヤーコントローラーのあらゆるアクションが不可能になる
    //またisDashやisJumpなどのあらゆるフラグがfalseされてアクションの計測されていた持続時間等も白紙にしたうえでreturnするように
    //プレイヤーコントローラーを停止してギミック側の処理でプレイヤーを動かすことでいろんなギミックに対応できるようにしたかった

    bool isGimmick;
    //接地判定と似たようなギミック利用可能かどうかのフラグ

    bool isGimmickEnter;
    bool isGimmickStay;
    bool isGimmickExit;
    string gimmickTag = "Gimmick";
    //Gimmickタグのオブジェクトの接触を検知する
    bool isPush;


    private void Update()
    {
        //ボタンが押されているかどうかの確認

        if (Input.GetButtonDown("Submit"))
        {

            isPush = true;
        }
        else
        {
            isPush = false;

        }

    }


    void FixedUpdate()
    {
        if (isGimmickEnter || isGimmickStay)
        {
            isGimmick = true;
            //ギミック利用可能
        }
        else if (isGimmickExit)
        {
            isGimmick = false;
            //ギミック利用不可能
        }
        isGimmickEnter = false;
        isGimmickStay = false;
        isGimmickExit = false;
        //次の判定まで初期化

        if (isGimmick && isPush && !isGimmickOn)
        {
            //ギミック利用可能でボタンが押されていて、ギミック利用中でなければ以下の処理。ギミック利用開始
            isGimmickOn = true;
            isPush = false;
            //ボタンを押されてないことにすることで下で誤反応を起こすのを防いだ
        }

        else if (isGimmickOn && isPush)
        {
            //ギミック利用中にボタンが押されたら以下の処理。ギミック利用停止
            isGimmickOn = false;
            isPush = false;
        }


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //ギミックを利用可能か判定
        if (collision.tag == gimmickTag)
        {
            isGimmickEnter = true;

        }


    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == gimmickTag)
        {
            isGimmickStay = true;

        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == gimmickTag)
        {
            isGimmickExit = true;
        }

    }


}
