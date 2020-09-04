using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeJump : MonoBehaviour
{
    //ロープにアタッチするジャンプ用のスクリプト


    public GameObject Player;
    //プレイヤーオブジェクトを取得

    PlayerMove pm;//プレイヤーコントローラーのスクリプト
    Rigidbody2D rb;//プレイヤーオブジェクトのRigidBody２D
    Rigidbody2D myRb;//自分のRigidBody２D
    Vector2 box;//Velocity変更のための構造体の入れ物
    Transform tf;//プレイヤーオブジェクトのトランスフォーム


    string playerTag = "Player";

    bool isFly;//プレイヤーオブジェクトがギミック利用可能範囲内に来たかどうかを判断する。trueならFlyできるという意味
    bool isGrip;//ロープにしがみついているかどうか
    bool isPush;//ボタンを押しているかどうか


    // Start is called before the first frame update
    void Start()
    {
        pm = Player.GetComponent<PlayerMove>();
        rb = Player.GetComponent<Rigidbody2D>();
        tf = Player.GetComponent<Transform>();

        myRb = GetComponent<Rigidbody2D>();
    //変数に上記の通りのものを格納
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Submit"))
        {

            isPush = true;
            //前と同じボタン処理
        }
        else
        {
            isPush = false;

        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        //プレイヤーオブジェクトがトリガーの範囲内に来たかどうか判定
        //一度範囲内にきて起動したら動作終了までtrueにするので再判定については考えていない
        if (collision.tag == playerTag)
        {
            isFly = true;
        }
    }


    private void FixedUpdate()
    {

        if (isFly)
        {
            //ギミック起動
            if (!isGrip && isPush)
            {
                //ロープにしがみついていなくて、ボタンが押されたら以下の処理

                //ロープ下端にジョイントされたオブジェクトの子オブジェクトに
                Player.transform.parent = this.gameObject.transform;
                //ローカルポジションはゼロにして
                Player.transform.localPosition = new Vector3(0, 0, 0);
                //親オブジェクトに連動するようにRigidBody2Dのタイプを変える
                rb.bodyType = RigidbodyType2D.Kinematic;
                //そしてしがみつきのフラグをtrueにして
                isGrip = true;
            //誤作動防止のためにボタンは押してないことに
                isPush = false;
            }

            //握っていてボタンが押されたら飛ぶ
            if (isGrip && isPush)
            {
                //握っていない
                isGrip = false;
                //isFlyをfalseに
                //そして再度Enterしない限りこのスクリプトは再びisFlyはtrueになれない。
                //よってジャンプしてすぐ別のギミックのためにボタンを押しても誤作動しない
                isFly = false;

                //親子関係を解消
                Player.transform.parent = null;
                //プレイヤーオブジェクトを直立させる
                Player.transform.rotation = new Quaternion(0, 0, 0,0);
                //プレイヤーオブジェクトをプレイヤーコントローラーを通じてジャンプさせる
                pm.isJump = true;
                //RigidBody2Dのタイプを元に戻す
                rb.bodyType = RigidbodyType2D.Dynamic;
                //ロープのVelocityを格納
                box = myRb.velocity;
                //プレイヤーオブジェクトにVelocityを与える。
                //これによりプレイヤーオブジェクトがロープの勢いをそのままに跳躍することを企図
                rb.velocity = box;
                //一応誤作動阻止
                isPush = false;
            }
        }
    }


        }
