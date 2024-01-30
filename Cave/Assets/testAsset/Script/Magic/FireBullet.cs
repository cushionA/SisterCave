using UnityEngine;
using Cysharp.Threading.Tasks;

using System;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine.Jobs;
using Unity.Jobs;
using Unity.Collections;
using FunkyCode.Buffers;
using static CombatManager;

/// <summary>
/// 最初にDamageOnTouchと回復量を初期化する。
/// タグ関連の仕様変更
/// 
/// まず攻撃者にかかってるバフを取得
/// そして攻撃力などはステータスから獲得
/// これは通り飛んで当たるだけ
/// 最初のサイズ保存してタイムリセットしてリストもきれいに

/// 最初は追尾弱くして上向きに飛ばす弾丸を徐々に追尾強くしたりすれば曲射とか特殊な軌道の弾丸作れそう
/// </summary>

public class FireBullet : MonoBehaviour
{

    #region 定義


    /// <summary>
    /// 魔法の動作に必要なデータをまとめる
    /// </summary>
    public class MagicActionData
	{

		/// <summary>
		/// 魔法使用者に関する情報
		/// </summary>
		public CharacterIdentify ownerData;


		/// <summary>
		/// ターゲットに関する情報
		/// </summary>
		public CharacterIdentify targetData;


		/// <summary>
		/// ターゲットの位置
		/// </summary>
        public Vector2 targetPosition;

        /// <summary>
        /// キャラデータのナンバーが正しいものであるかを監視する
        /// きちんと参照できているかを調べる
        /// </summary>
        /// <param name="data"></param>
        /// <returns>新しいキャラのナンバー。これが-1だとキャラ消失</returns>
        public int CharacterReferenceCheck(CharacterIdentify data)
		{
            //相手がプレイヤー側なら敵のコンバットマネージャーにいるはず
            if (data.side == CharacterSide.Player)
            {
				//記録してるIDが違うなら
				//新しいナンバーに入れ替え
				if (EnemyManager.instance._targetList[data.nunber].targetID != data.ID)
				{
					return EnemyManager.instance.GetTargetNumberByID(data.ID);
                }
            }
            //敵なら
            else
            {
                //記録してるIDが違うなら
                //新しいナンバーに入れ替え
                if (SManager.instance._targetList[data.nunber].targetID != data.ID)
                {
                    return SManager.instance.GetTargetNumberByID(data.ID);
                }
            }

			return data.nunber;
        }


		/// <summary>
		/// データをもとにキャラクターの位置を取得する
		/// </summary>
		/// <param name="data">調査対象のキャラのデータ</param>
		/// <returns></returns>
		public Vector2 GetCharacterPosition(CharacterIdentify data)
		{
			//相手がプレイヤー側なら敵のコンバットマネージャーにいるはず
			if(data.side == CharacterSide.Player)
			{
				return EnemyManager.instance._targetList[data.nunber]._condition.targetPosition;
			}
			//敵なら
			else
            {
                return SManager.instance._targetList[data.nunber]._condition.targetPosition;
            }
		}

        /// <summary>
        /// データをもとにキャラクターのコントロールアビリティを取得
        /// </summary>
        /// <param name="data">調査対象のキャラのデータ</param>
        /// <returns></returns>
        public ControllAbillity GetCharacterController(CharacterIdentify data)
        {
            //相手がプレイヤー側ならSmanagerの友軍リストから呼び出す
            if (data.side == CharacterSide.Player)
            {
                return SManager.instance.AllyList[data.nunber];
            }
			//敵なら
            else
            {
                return EnemyManager.instance.AllyList[data.nunber];
            }

        }

    }


	/// <summary>
	/// キャラクターを特定するために必要な要素
	/// 配列で何番目かとIDと陣営
	/// </summary>
	public struct CharacterIdentify
	{

		/// <summary>
		/// キャラの所属
		/// </summary>
		public CharacterSide side;

		/// <summary>
		/// 何番目にいるかというナンバー
		/// これでIDチェック失敗したら次はIDで探す
		/// そしてナンバーを更新する
		/// IDでも見つからなければもう終わり
		/// </summary>
		public int nunber;

		/// <summary>
		/// キャラのID
		/// -1なら参照切れ
		/// </summary>
		public int ID;

	}




#endregion



    /// <summary>
    /// 魔法の使用者が誰か
    /// ダメージ処理とかで使うかな
    /// 初期化処理でも使うか、使用者の攻撃倍率とか聞かないといけないし
    /// </summary>
    [Header("魔法の使用者が誰か")]
	[SerializeField]
	MasicUser _user;

	[SerializeField]
	MyDamageOntouch _damage;

	public enum MasicUser
    {
		Player,
		Sister,
		Others,
		Child//子どもの弾丸
    }

	// === 外部パラメータ（インスペクタ表示） =====================

	public Magic em;




	// === 外部パラメータ ======================================
	[System.NonSerialized] GameObject owner;



	// === 内部パラメータ ======================================


	[SerializeField]
	Rigidbody2D rb;


	//リセット対象
    #region
    /// <summary>
    /// 弾丸がどれくらいの時間存在してるか計測する。
    /// </summary>
    float fireTime;


	MagicActionData useCharaData;

	/// <summary>
	/// 効果あるかどうか。
	/// 連続して効果発揮しないように
	/// </summary>
	bool isAct;	


	float effectWait;//サポートや回復が再度効果を現すまで
    

	
	/// <summary>
	/// すでに衝突したもの
	/// </summary>
	List<Transform> collisionList = new List<Transform>();
	#endregion




	TransformAccessArray myTransform;

	NativeArray<Vector3> result;


	/// <summary>
	/// 存在音ならしたか
	/// 音を鳴らすのに使う
	/// </summary>
	bool loud;//常に音が鳴るかどうか



	/// <summary>
	/// 存在音ならしたか
	/// 音を消すのに使う
	/// </summary>
	bool isExPlay;


	/// <summary>
	/// キャラが右と左どちらから弾を撃ったか
	/// </summary>
	[HideInInspector]int direction;




	Collider2D col;

	/// <summary>
	/// もし弾丸が動かない待ち時間があるなら止めるフラグ
	/// </summary>
	bool movable;





	FireJob job;

	JobHandle _handler;


	/// <summary>
	/// 魔法オブジェクトを管理しているアビリティ
	/// </summary>
	AtEffectCon atEf;

	string _healTag;




	/// 
	/// 新弾丸処理
	/// ・敵情報参照確認→情報更新（味方情報参照とその確認は必要な時だけ）
	/// ・弾丸移動
	/// ・衝突時処理
	/// ・その他常時処理
	/// ・初期化終了処理
	/// 
	/// に処理を分ける
	/// 
	/// ///



    private void OnEnable()
    {

	　　InitializeBullet();

		//起動時間初期化
		fireTime = GManager.instance.nowTime;


		myTransform = new TransformAccessArray(0);
		myTransform.Add(transform);
		result = new NativeArray<Vector3>(1, Allocator.Persistent);
		job.result = result;
	}


    // === コード（Monobehaviour基本機能の実装） ================
    void Start()
	{

		//最初だけやる処理--------------------------------------------------------------

		//ヒット可能回数をセット
		_damage._attackData.actionData._hitLimit = em._hitLimit;

		if (this.gameObject != null)
		{
			col = this.gameObject.MMGetComponentNoAlloc<Collider2D>();
		}
		//ジョブの初期化
		//一度だけでいい
		job = new FireJob
		{ _status = em._moveSt };
		//-------------------------------------------------------------------------



	}



	void OnTriggerEnter2D(Collider2D other)
	{
	//Debug.Log($"sdddssdsdsd{other.gameObject.name}");
	
		BulletHit(other);
	}
	void OnTriggerStay2D(Collider2D other)
	{
		//Debug.Log($"sdddssdsdsd{other.gameObject.name}");
		BulletHit(other);
	}


    private void OnDisable()
    {




		isAct = false;
		effectWait = 0;//サポートや回復が再度効果を現すまで

		collisionList.Clear();
		
		//ここまで初期化


		myTransform.Dispose();
		result.Dispose();
	}

	
    void FixedUpdate()
	{

        Debug.Log($"名前{this.gameObject.name}標的{target == null}");

		float nowTime = GManager.instance.nowTime - fireTime;


		//衝突管理
		CollisionObjectController();


        //うるさい弾なら音を鳴らす
        if (loud)
		{
			GManager.instance.PlaySound(em.existSound, transform.position);
			loud = false;
		}




		BulletMoveController(nowTime);


	}


    #region キャラ情報関連処理



	/// <summary>
	/// 魔法が動作するためのデータを作って入れる
	/// </summary>
	/// <param name="owner"></param>
	/// <param name="ownerNum"></param>
	/// <param name="target"></param>
	/// <param name="targetNum"></param>
	public void ActionDataSet(TargetData owner,int ownerNum,TargetData target,int targetNum)
	{
		useCharaData = new MagicActionData();

		useCharaData.ownerData = GetCharacterData(owner, ownerNum);

        useCharaData.targetData = GetCharacterData(target, targetNum);
    }


	/// <summary>
	/// キャラのデータを作成して返す
	/// </summary>
	/// <param name="charaData"></param>
	/// <param name="number"></param>
	/// <returns></returns>
	CharacterIdentify GetCharacterData(TargetData charaData,int number)
	{
		CharacterIdentify data = new CharacterIdentify();

		data.ID = charaData.targetID;
		data.side = charaData._baseData.side;
		data.nunber = number;

		return data;

	}



	/// <summary>
	/// 毎フレーム最初にターゲットの位置を取得
	/// 参照できなければもうターゲットはいない
	/// </summary>
	void TargetPositionCheck()
	{
		//きちんと参照できてるなら
		if(useCharaData.CharacterReferenceCheck(useCharaData.targetData) != -1)
		{
			//位置情報更新
		useCharaData.targetPosition = useCharaData.GetCharacterPosition(useCharaData.targetData);
		}

	}



    #endregion


    #region 移動処理

	void BulletMoveController(float nowTime)
	{

        if (em._moveSt.fireType == Magic.FIREBULLET.STOP)
        {
            return;
        }


        // ターゲット設定
        //追尾時間以内、かつ敵への参照があるなら追いかける
        bool homing = ((nowTime < em._moveSt.homingTime) && useCharaData.targetData.nunber != -1);//((Time.fixedTime - fireTime) < em.homingTime);


        //弾丸が始動するまでの待ち時間がある場合、動かず当たり判定もなくその場にとどまる
		//Onenable時にem._moveSt.waitTime > 0 &&  を使ってmovable初期化と当たり判定消去
        if (!movable)
        {
            //if (col != null)
            //{
              //  col.enabled = false;
            //}

			//動き出す時間が来たなら
            if (nowTime >= em._moveSt.waitTime)
            {
                if (em.moveSound != null)
                {
					//始動音
                    GManager.instance.PlaySound(em.moveSound, transform.position);
                }
                movable = true;
                col.enabled = true;
            }

        }



        // ホーミング処理
        //ここでジョブ呼び出し
        //movableは渡す


		//ターゲット見失ってないなら
        if (useCharaData.targetData.nunber != -1)
        {
            job.targetLost = false;
            job.posTarget = useCharaData.targetPosition;
        }
        else
        {
            job.targetLost = true;
        }

		//加速処理
        if (em._moveSt.speedA != 0)
        {
            job.speed += (em._moveSt.speedA * Time.fixedDeltaTime);
        }


        job.movable = movable;
        job.homing = homing;
        job.time = Time.fixedDeltaTime;
        //	job.homingAngle = transform.rotation; 
        this._handler = this.job.Schedule(myTransform);
        _handler.Complete();


        if (movable)
        {
            rb.velocity = result[0];
        }
        else
        {
            rb.velocity = Vector2.zero;
        }


    }



    #endregion

    #region 管理メソッド


	/// <summary>
	/// 毎フレーム実行
	/// 衝突したものを記録して管理するメソッド
	/// すでに当たったものには当たらないようにする
	/// </summary>
	void CollisionObjectController()
	{
        if (collisionList.Count > 0)
        {

            //攻撃魔法ならより短い時間で衝突をリセット
            if (em.mType == Magic.MagicType.Attack)
            {
                //貫通弾なら。速度0以上かつ弾丸も一秒以上生きるなら爆発とかではない
                if (em.penetration && em._moveSt.speedV > 0 && em._moveSt.lifeTime > 1)
                {
                    if (GManager.instance.nowTime - effectWait >= 0.5f)
                    {
                        collisionList = null;
                    }
                }

            }
			//そうでないなら三秒に一回効果する
            else if (GManager.instance.nowTime - effectWait >= 3)
            {
                collisionList = null;
            }
            if (collisionList == null)
            {
                collisionList = new List<Transform>();
            }

        }

    }


	/// <summary>
	/// 生存時間が経過したら消える
	/// そして音を消したりする
	/// </summary>
	/// <param name="nowTime"></param>
	void LifeTimeController(float nowTime)
	{
        //弾丸の生存時間終わりなら
        //あるいは追尾弾の標的消えたら
        //それか直進でいいか
        if (nowTime >= em._moveSt.lifeTime) // ((em._moveSt.fireType == Magic.FIREBULLET.HOMING || em._moveSt.fireType == Magic.FIREBULLET.HOMING) && target == null))
        {

            //   存在中の音声がなってるなら消す
            if (isExPlay)
            {
                GManager.instance.StopSound(em.existSound, 1f);
            }
            //子弾丸であるなら消える



            atEf.BulletClear(transform);
            Destroy(this.gameObject);

        }
    }


	/// <summary>
	/// 弾丸の初期化
	/// 毎回やる初期化
	/// ターゲット情報などがもうある前提で進める
	/// レイヤー設定もここでするか
	/// 所属情報と魔法の種類をセットして
	/// </summary>
    void InitializeBullet()
    {

		//倍率をセット
		_damage._attackData.multipler = useCharaData.GetCharacterController(useCharaData.ownerData).BulletBuffCalc();
			atEf = fa.atEf;

if (_user == MasicUser.Sister)
		{


			_healTag = "Player";
			em._moveSt.angle = SManager.instance.useAngle;
		}




		//弾の存在時間などのステータス初期化
		fireTime = GManager.instance.nowTime;
		//当たり判定初期化
		_damage.CollidRestoreResset();

		//Debug.Log($"標的{gameObject.name}");

		//発射音
		GManager.instance.PlaySound(em.fireSound, transform.position);

		// 存在してる間のサウンドがあるなら
		if (em.existSound != null)
		{
			loud = true;
			isExPlay = true;
		}

		///　
		/// 使用者（親）が子に渡す物
		/// 
		/// 敵と使用者の情報
		/// (バフもここで渡す？)
		/// 自分のオブジェクト
		/// 攻撃エフェクト管理機能
		/// 狙ってる角度（自分の向きとか角度も？）
		/// 
		/// ///

		//子の弾丸なら親の方向で向きを変える
		if (em.isChild)
		{
			Vector3 setScale = transform.localScale;
			if (direction < 0)
			{
				setScale.x = -setScale.x;
			}
			transform.localScale = setScale;
		}

		//この弾丸のダメージを算出
		if (this.gameObject != null)
			DamageCalc();



		//ジョブの初期化
		job.Initialize(target.transform.position, transform.position);


	}


    #endregion



    public int RandomValue(int X, int Y)
	{
		return UnityEngine.Random.Range(X, Y + 1);

	}

	/// <summary>
	/// 使用者ごとに異なる弾丸の初期化
	/// 使用者が呼び出す
	/// 子弾のためにこのスクリプト内で呼び出す
	/// </summary>
	public void InitializeNextBullet(GameObject _owner, GameObject _target, int _direction,AtEffectCon con)
	{

		owner = _owner;
		target = _target;
		//使用者に従って
		if (_user == MasicUser.Child)
        {
			//武器の威力修正でやる？
			_owner.MMGetComponentNoAlloc<FireBullet>().BuffCalc(this);
			direction = _direction;
		}
        else
        {
			//敵、その他の場合EnemyMagicのATKをそのままぶち込む
			_owner.MMGetComponentNoAlloc<EnemyAIBase>().BuffCalc(this);
			_healTag = "Enemy";

		}

		atEf = con;


	}





	void HealMagic(GameObject target)
	{
		target.MMGetComponentNoAlloc<MyHealth>().Heal(em.recoverAmount);
		effectWait = GManager.instance.nowTime;

	}

	/// <summary>
	/// 弾丸当たった時の処理
	/// </summary>
	void BulletHit(Collider2D other)
	{


		// オーナーチェック。あるかどうか、Nullなら戻る
		if (owner == other.gameObject || isAct || other == null)
		{
			//Debug.Log($"衝突{owner == other.gameObject.transform}{isAct}{other != null}");// {gameObject.name}");
			return;
		}
		// 自分自身にヒットしないようにチェック


		/// <summary>
		/// すでに効果しているので無効
		/// </summary>
		bool alreadyEffect = false;//サポートや回復が効果を及ぼす。原則一回だけ
						   // 壁アタリをチェック

		isAct = true;


		if (em.hitEffect != null)
		{

			//子弾の魔法あるなら
			if (em.childM != null)
			{

				//next.transform.localScale.Set(0, 0,0);

				Vector3 goFire = transform.position;
				Vector2 random = Vector2.zero;

				FireBullet nb = atEf.BulletCall(em.hitEffect, this.gameObject.transform.position, transform.rotation,em.childM.flashEffect).gameObject.MMGetComponentNoAlloc<FireBullet>();

				nb.InitializeNextBullet(this.gameObject, target, direction, atEf);
				transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, em.childM._moveSt.angle);
				nb.transform.rotation = transform.rotation;

				float num = em.childM.bulletNumber;
				if (num > 1)
				{

					random.Set(em.childM.HRandom, em.childM.VRandom);



					for (int x = 1; x < num; x++)
					{
						if (x == 0)
						{
						}

						else
						{
							if (random.x != 0 || random.y != 0)
							{
								goFire.Set(goFire.x + RandomValue((int)-random.x, (int)random.x), goFire.y + RandomValue((int)-random.y, (int)random.y), this.gameObject.transform.position.z);
							}

							atEf.BulletCall(em.hitEffect, goFire, transform.rotation, em.childM.flashEffect).gameObject.MMGetComponentNoAlloc<FireBullet>().InitializeNextBullet(this.gameObject, target, direction, atEf);
						}
					}
				}
			}
			else
				{
					atEf.BulletCall(em.hitEffect, this.gameObject.transform.position, transform.rotation);
				}



		}

		//いろんなものと衝突してるなら
		if (collisionList.Count > 0)
        {//Debug.Log("ssssdf");
			for (int i = 0; i < collisionList.Count;i++)
            {
				//もしすでにぶつかったものと一致したら
				if(other.transform == collisionList[i])
                {
					alreadyEffect = true;
					break;
                }
            }

        }
		if (!alreadyEffect)
		{
			if (Magic.MagicType.Attack == em.mType)
			{

				direction = other.transform.position.x >= transform.position.x ? 1 : -1;

				if (em.hitSound != null)
				{
					GManager.instance.PlaySound(em.hitSound, transform.position);
				}
				if (!em.penetration)
				{
					if (isExPlay)
					{
						GManager.instance.StopSound(em.existSound, 1f);
					}
		

					if(other.gameObject.tag == SManager.instance.enemyTag || other.gameObject.tag == GManager.instance.playerTag)
                    {
                        _damage.isBreake = true;
                    }
                    else
                    {

						atEf.BulletClear(transform);
                       // Destroy(this.gameObject);
					}

				}
				else
				{
					collisionList.Add(other.transform);
					//何回貫通できるかみたいな数値入れてもいいかも

					//他のとこでは衝突で消えるでしょ
					isAct = false;
					//	Addressables.ReleaseInstance(this.gameObject);
				}
			}
			else if (other.gameObject.tag == _healTag)
			{
				collisionList.Add(other.transform);
				if (Magic.MagicType.Recover == em.mType)
				{

					HealMagic(other.gameObject);
				}
				else if (Magic.MagicType.Support == em.mType)
				{

				}
			}
			//Debug.Log($"{this.gameObject.name}が{other.transform.gameObject.name}");
		}


	}



	/// <summary>
	/// 二点間の角度を求める
	/// </summary>
	/// <param name="p1">自分の座標</param>
	/// <param name="p2">相手の座標</param>
	/// <returns></returns>
	float GetAim(Vector2 p1, Vector2 p2)
	{
		float dx = p2.x - p1.x;
		float dy = p2.y - p1.y;
		float rad = Mathf.Atan2(dy, dx);
		return rad * Mathf.Rad2Deg;
	}


	/// <summary>
	/// ダメージ計算
	/// ステータスとバフを親から取得してダメージ計算
	/// </summary>
	/// <param name="isFriend">真なら味方</param>
	public void DamageCalc()
	{
		//GManager.instance.isDamage = true;
		//useEquip.hitLimmit--;
		//mValueはモーション値

          _damage._attackData._attackType = em.magicElement;
		_damage._attackData.phyType = em.phyElement;

		if (em.phyAtk > 0)
		{
			_damage._attackData.phyAtk = em.phyAtk * attackFactor;

			//_damage._attackData._attackType = 4;

			//						Debug.Log("皿だ");
			if (em.shock >= 40)
			{
				_damage._attackData.isHeavy = true;
			}
			else
			{
				_damage._attackData.isHeavy = false;
			}

		}
		//神聖
		if (em.holyAtk > 0)
		{
			_damage._attackData.holyAtk = em.holyAtk * holyATFactor;

		}
		//闇
		if (em.darkAtk > 0)
		{
			_damage._attackData.darkAtk = em.darkAtk * darkATFactor;

		}
		//炎
		if (em.fireAtk > 0)
		{
			_damage._attackData.fireAtk = em.fireAtk * fireATFactor;

		}
		//雷
		if (em.thunderAtk > 0)
		{
			_damage._attackData.thunderAtk = em.thunderAtk * thunderATFactor;

		}
		_damage._attackData.shock = em.shock;

		_damage._attackData.mValue = em.mValue;
		_damage._attackData.attackBuff = attackBuff;
		//damage = Mathf.Floor(damage * attackBuff);

		_damage._attackData.isBlow = em.isBlow;

		_damage._attackData.blowPower.Set(em.blowPower.x, em.blowPower.y);

		

	}

	/// <summary>
	/// バフの数値を与える
	/// 弾丸から呼ぶ
	/// </summary>
	public void BuffCalc(FireBullet _fire)
	{
		_fire.attackFactor = attackFactor;
		_fire.fireATFactor = fireATFactor;
		_fire.thunderATFactor = thunderATFactor;
		_fire.darkATFactor = darkATFactor;
		_fire.holyATFactor = holyATFactor;
	}


}
