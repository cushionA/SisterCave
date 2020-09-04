using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    [Header("移動経路")] public GameObject[] movePoint;
    [Header("速さ")] public float speed = 1.0f;

    private Rigidbody2D rb;
    private int nowPoint = 0;
    private bool returnPoint = false;
    private Vector2 oldPos = Vector2.zero;
    private Vector2 myVelocity = Vector2.zero;
    //Vector2.zeroはVector2(0, 0) と同じ意味

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (movePoint != null && movePoint.Length > 0 && rb != null)
        {
            //最初の位置
            rb.position = movePoint[0].transform.position;
            //フレーム開始の位置を入れて、それから移動処理とかする。そしてまた次のフレームの頭で位置代入。
            //つまり前フレームの処理の結果の位置。前フレームの位置。
            oldPos = rb.position;
        }
    }
    public Vector2 GetVelocity()
    {
        return myVelocity;
    }

    private void FixedUpdate()
    {
        if (movePoint != null && movePoint.Length > 1 && rb != null)
        {
            //通常進行
            if (!returnPoint)
            {
                int nextPoint = nowPoint + 1;

                //目標ポイントとの誤差がわずかになるまで移動
                //Distance (Vector2 a, Vector2 b);aからbへの距離を測る
                if (Vector2.Distance(transform.position, movePoint[nextPoint].transform.position) > 0.1f)
                {
                    //現在地から次のポイントへのベクトルを作成
                    //public static 『『Vector2（返り値）』』 MoveTowards (Vector2 current, Vector2 target, float maxDistanceDelta);
                    //現在地から目的地、スピード
                    //vector2型のベクトル、現在地と目的地の間の座標の計算のメソッド
                    Vector2 toVector = Vector2.MoveTowards(transform.position, movePoint[nextPoint].transform.position, speed * Time.deltaTime);

                    //次のポイントへ移動
                    //MovePosition (Vector3 position);指定した位置に動かす
                    rb.MovePosition(toVector);
                }
                //次のポイントを１つ進める
                else
                {
                    rb.MovePosition(movePoint[nextPoint].transform.position);
                    ++nowPoint;

                    //現在地が配列の最後だった場合
                    if (nowPoint + 1 >= movePoint.Length)
                    {
                        returnPoint = true;
                    }
                }
            }
            //折返し進行
            else
            {
                int nextPoint = nowPoint - 1;

                //目標ポイントとの誤差がわずかになるまで移動
                if (Vector2.Distance(transform.position, movePoint[nextPoint].transform.position) > 0.1f)
                {
                    //現在地から次のポイントへのベクトルを作成
                    Vector2 toVector = Vector2.MoveTowards(transform.position, movePoint[nextPoint].transform.position, speed * Time.deltaTime);

                    //次のポイントへ移動
                    rb.MovePosition(toVector);
                }
                //次のポイントを１つ戻す
                else
                {
                    rb.MovePosition(movePoint[nextPoint].transform.position);
                    --nowPoint;

                    //現在地が配列の最初だった場合
                    if (nowPoint <= 0)
                    {
                        returnPoint = false;
                    }
                }
            }
            myVelocity = (rb.position - oldPos) / Time.deltaTime;
            oldPos = rb.position;
            //velocity、速度型の変数に１フレームの移動距離を時間で割った速度を出して格納
            //そして最後に現在地を次のフレームのためのoldに
            //
        }
    }
}
