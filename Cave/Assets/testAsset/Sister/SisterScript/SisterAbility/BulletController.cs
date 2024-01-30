using Cysharp.Threading.Tasks;
using MoreMountains.CorgiEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AttackData;

/// <summary>
/// 弾丸が一発以上の時に挙動をコントロールしてくれるやつ
/// あと情報を渡してくれる
/// 位置を整える役目も持つ（この場合最初、弾丸をアクティブ化する前にやる。非アクティブ状態だと座標操作に変な負荷がかからない）
/// 単発の場合これはいらない
/// あくまで複数の弾丸をまとめる処理
/// したがって単発かどうかで弾丸の消滅と呼び出しの処理が変わる？
/// 
/// 求められる機能
/// ・弾丸が必要とする情報を受け取って保持する機能（情報は参照たどって弾丸の方が取りに来る）　×　controllアビリティと繋がる
/// ・弾丸を順番にアクティブ化していく機能。（個別に秒数決めれる）　〇
/// ・アクティブ化と同時に親子関係を切り離す　〇
/// ・参照たどって弾丸が終了呼び出ししてきたら非アクティブ化させる。全部非アクティブ化したらもうプールに帰る　〇
/// ・ターゲットが健在であるかをこいつが検査する機能。検査系は一括で一度だけ　×
/// ・ターゲットの位置を更新し続ける機能　×
/// ・魔法がヒットした時、ヒット報告を受けてヒット回復などの効果を受ける
/// 
/// 問題
/// ・マズルフラッシュ系はどうするか（各魔法がアクティブ時に自分でこいつのatefを通じて呼び出す？）
/// ・単発は処理分けるか？（分けない。非アクティブの時に親子関係の操作をおこなえばいい）
/// ・別々の挙動の弾丸を入れるには？　→バレットコントローラーに魔法をつけておく。弾丸の挙動データと魔法データを分ける？
/// ・poolからの呼び出しがparticleじゃなくなる。着弾エフェクトと子魔法は分けるか？
/// ・プールへのエフェクト追加どうするか（弾丸情報みたいにして配列でまとめるか。魔法の弾丸情報のどれを参照するかみたいなのを番号で持たせとけ）
/// 
/// 
/// ・アイテムとしてのデータ
/// ・挙動データ（速度、追尾形式、弾丸の生存時間）
/// ・攻撃力データ（ヒット回数、モーション値、攻撃力）　弾丸所属（攻撃力だけ魔法から持ってくる）
/// ・エフェクトデータ（フラッシュエフェクト、着弾エフェクト、魔法エフェクト）　弾丸所属（フラッシュエフェクトと着弾エフェクトは）
/// ・詠唱データ（詠唱時間、モーション指定）　魔法所属
/// ・バレットコントローラープレハブへの参照　魔法所属
/// 
/// 
/// </summary>
public class BulletController : MonoBehaviour
{

    /// <summary>
    /// 弾丸をコントロールするのに必要な情報
    /// </summary>
    public struct BulletControllData
    {

        /// <summary>
        /// 生成するまでの時間
        /// </summary>
        [Header("生成されるまでの時間")]
        public float emitSecond;

        [Header("初期位置")]
        public Vector2 firstPosition;

        [Header("弾丸オブジェクト")]
        public FireBullet bullet;

    }


    /// <summary>
    /// 最初に初期化する
    /// 弾丸数
    /// </summary>
    int bulletCount;

    /// <summary>
    /// 非アクティブになったオブジェクトを数える
    /// 全部非アクティブになったらプールに帰る
    /// </summary>
    int disenableCount;


    /// <summary>
    /// 弾丸の配列
    /// これらを起動する
    /// </summary>
    [SerializeField]
    BulletControllData[] bullets;


    /// <summary>
    /// エフェクトコントローラー
    /// </summary>
    AtEffectCon atEf;



    /// <summary>
    /// 攻撃倍率
    /// </summary>
    public AttackMultipler multipler;

    #region　生成・消滅系のメソッド

    /// <summary>
    /// 最初に弾丸を生成していく処理
    /// </summary>
    void BulletGenerater()
    {
        //弾丸が沢山あるなら
        if (bulletCount > 1)
        {
            for (int i = 0; i < bulletCount; i++)
            {
                //弾丸生成処理を振り分けていく
                BulletGenerateExe(bullets[i]).Forget();
            }
        }
        //弾丸が一つなら
        else
        {
            //弾丸生成処理を振り分けていく
            BulletGenerateExe(bullets[i]).Forget();
        }
    }


    /// <summary>
    /// 弾丸生成実行
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    async UniTaskVoid BulletGenerateExe(BulletControllData data)
    {
        //位置を最初に戻す
        data.bullet.gameObject.transform.position = data.firstPosition;

        //親子関係を排除
        data.bullet.gameObject.transform.parent = null;

        //回転もいじるか？
        //0にしよう、いや弾丸が起動時に勝手に変えればいいか自分で決めさせろ

        if (data.emitSecond > 0)
        {
            //待ち時間あるなら待つ
            await UniTask.Delay(TimeSpan.FromSeconds(data.emitSecond));
        }

        //弾丸を有効化
        data.bullet.gameObject.SetActive(true);
    }




    /// <summary>
    /// 弾丸を消去するメソッド
    /// 非アクティブ化後親子関係を戻す
    /// 全て戻したらプールから消える
    /// 
    /// ちなみにマズルフラッシュは時間経過で勝手に消えるはずです
    /// </summary>
    /// <param name="bullet"></param>
    public void BulletClear(FireBullet bullet)
    {
        //すでに非アクティブなら
        //つまりもう消去されてるなら多重呼び出しは拒否
        if(bullet.gameObject.activeSelf == false)
        {
            return;
        }


        //無効化
        bullet.gameObject.SetActive(false);

        //親オブジェクトに戻る
        bullet.gameObject.transform.parent = this.transform;

        //カウントをプラスに
        disenableCount++;

        //弾丸の数を帰ってきた弾丸の数が超えた
        //つまり全ての弾丸が返ってきたと思われるときだけ
        //子オブジェクトの数確認をする
        //慎重にやる
        if(disenableCount >= bulletCount)
        {
            //子オブジェクトにみんな戻っているなら
            if(transform.childCount >= bulletCount)
            {

                //プールに帰る
                atEf.BulletClear(this.transform);
            }

        }


    }

    #endregion
}
