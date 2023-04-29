using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.UI;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using Rewired.Integration.CorgiEngine;
using DarkTonic.MasterAudio;


/// 攻撃中は場所変えないようにする。中断でエフェクト残る
/// あと角度これでいいか確認
/// z軸位置もな
/// 作戦に追加する要素
/// boolリストでMP節約とか先制攻撃とか個別につけさせるか
/// あるいはMP使用度みたいなのをintで作りチェックついてる箱で１、２とか数値変えるか
/// それか複数作戦持たせる？
/// 火炎放射は動かずホーミングでClosestEnemyをターゲットに
/// 敵側でトリガーが発生しない
/// それぞれの条件に魔法を入れる
/// なかったら判断して探す
/// 止まり木に触れてAIいじる画面に行くと一旦条件魔法を白紙にする

namespace MoreMountains.CorgiEngine // you might want to use your own namespace here
{
	/// <summary>
	/// 普通に状況判断して動くためのスクリプト
	/// ワープ、攻撃、コンビネーション（各動作は分ける？）は別スクリプトに
	/// あるいはコンビネーションは主人公に持たせる？
	/// 流れとしては判断、詠唱、攻撃状態に遷移しモーション変化、アニメーションイベント、魔法使用、アニメーションイベントで終了
	/// </summary>
	[AddComponentMenu("Corgi Engine/Character/Abilities/FireAbility")]
	public class FireAbility : MyAbillityBase
	{

		public override string HelpBoxText() { return "シスターさんの攻撃"; }

		//   [Header("武器データ")]
		/// declare your parameters here
		///WeaponHandle参考にして 


		// Animation parameters
		//0がなんもなし、１が魔法、２がコンビネーション、３が詠唱
		protected const string _actParameterName = "actType";
		protected int _actAnimationParameter;

		/// <summary>
		/// モーションを選ぶ
		/// </summary>
		protected const string _motionParameterName = "motionNum";
		protected int _motionAnimationParameter;

		public SisterParameter sister;

		//public GameObject firePosition;
		//	SisterFireBullet sisF;
		[HideInInspector]
		public BrainAbility sb;
		/// <summary>
		/// クールタイム待機のため専用のフラグ
		/// </summary>
		bool disEnable;
		//List<GameObject> targetPlan;
		//GameObject target;
		List<SisMagic> useSupport;//未使用の支援
		List<float> effectiveTime;//支援魔法、リジェネ、攻撃の時間をはかる
		/// <summary>
		/// 詠唱時間待ち
		/// </summary>
		 [HideInInspector]
		public float waitCast;

		float coolTime;
		/// <summary>
		/// 攻撃、回復、などの優先行動を入れ替える
		/// </summary>
		float stateJudge = 30;
		//float targetJudge = 30;

		/// <summary>
		/// 何個目の条件でターゲット見つけたかを確認する
		/// </summary>
		 [HideInInspector]
		public int judgeSequence;

		/// <summary>
		/// 詠唱中つかう音
		/// </summary>
		string castSound = "NormalCastLoop1";

		/// <summary>
		/// 詠唱の音鳴らすサイン
		/// 1はまだ再生してない。2は再生中。3は終わり
		/// </summary>
		bool soundStart;
		/// <summary>
		/// 撃つ弾丸の数
		/// </summary>
		int bCount;
		float recoverTime;

		List<GameObject> targetCanList;
		List<EnemyAIBase> targetCanStatus;
		List<SisMagic> magicCanList;



		/// <summary>
		/// 何番目の詠唱なのか
		/// </summary>
		int actionNum;

		[SerializeField] CombinationAbility ca;






		bool fireStart;

		/// <summary>
		/// 戦闘開始時の初期化のためのフラグ
		/// </summary>
		[HideInInspector]
		public bool isReset;
		//回復時間
		float healJudge;


		/// <summary>
		/// 弾丸生成の最中かどうか
		/// </summary>
		bool delayNow;
		//ビット演算でクールタイムを破棄する
		int _skipCondition;



		//protected RewiredCorgiEngineInputManager _inputManager;


		//-------------------------------------------バフの数値

		/// <summary>
		/// 攻撃倍率
		/// </summary>
		[HideInInspector]
		public float attackFactor = 1;
		[HideInInspector]
		public float fireATFactor = 1;
		[HideInInspector]
		public float thunderATFactor = 1;
		[HideInInspector]
		public float darkATFactor = 1;
		[HideInInspector]
		public float holyATFactor = 1;


		GameObject castEffect;




		/// <summary>
		///　ここで、パラメータを初期化する必要があります。
		/// </summary>
		protected override void Initialization()
		{
			base.Initialization();
			sister.nowMove = sister.priority;

			//Brainじゃないんですわ
			sb = GetComponent<BrainAbility>();
		}


		/// <summary>
		/// 1フレームごとに、しゃがんでいるかどうか、まだしゃがんでいるべきかをチェックします
		/// </summary>
		public override void ProcessAbility()
		{
			base.ProcessAbility();


			if (isReset)
			{
				//これは道中回復開始とそのサウンドを終わらせてる

				if (soundStart)
				{
					soundStart = false;
					GManager.instance.StopSound(castSound, 0.5f);
					if (castEffect != null)
					{
						Addressables.ReleaseInstance(castEffect);
						castEffect = null;
					}
					FireSoundJudge();
				}
				
				stateJudge = 0;
				coolTime = 0;
				disEnable = false;
				_skipCondition = 0;
				if (_condition.CurrentState == CharacterStates.CharacterConditions.Moving)
				{
					_movement.ChangeState(CharacterStates.MovementStates.Idle);
					_condition.ChangeState(CharacterStates.CharacterConditions.Normal);
				}

				isReset = false;
			}

			FireAct();
		}

		/// <summary>
		/// これコンビネーションに移植する？
		/// </summary>
		protected override void HandleInput()
		{

		}

		/// <summary>
		/// 攻撃の実行
		/// </summary>
		void FireAct()
		{
			//停止命令出てるかどうか

			//Debug.Log($"あいｄｊｄ{MathF.Floor(waitCast)}");
			//行動してないとき魔力回復




			if (_condition.CurrentState != CharacterStates.CharacterConditions.Moving)
			{
				recoverTime += _controller.DeltaTime;
				if (recoverTime >= 2.5 && sb.mp < sb.maxMp)
				{
					//クールタイム中は魔力回復1.2倍
					float recoverAmo = disEnable ? SManager.instance.sisStatus.mpRecover * 1.2f : SManager.instance.sisStatus.mpRecover;

					sb.mp += (recoverAmo + SManager.instance.sisStatus.additionalRecover);
					recoverTime = 0;
				}
			}
			if (sb.mp > sb.maxMp)
			{
				sb.mp = sb.maxMp;

			}
			if (_movement.CurrentState == CharacterStates.MovementStates.Attack)
			{
			//	
				MagicUse(SManager.instance.useMagic.HRandom, SManager.instance.useMagic.VRandom);
				return;
			}

			//戦闘中、かつ位置についているなら
			else if (sb.nowState == BrainAbility.SisterState.戦い)
			{
				//戦闘開始時にフラグをきれいにしておきます

				//戦闘開始時の初期化終わったなら

				//何も魔法ないなら戻る
				if (sb.status.equipMagic == null)
				{
					return;
				}
				//Disenableならクールタイムを待つ

				CoolTimeWait();

				//ターゲットが消えたら詠唱やめる
				CastStop().Forget();


				//	targetJudge += _controller.DeltaTime;
				stateJudge += _controller.DeltaTime;

				///ターゲットと使用魔法設定
				//クールタイム中でもスキップ条件があるなら動く
				#region
				
				if (_condition.CurrentState != CharacterStates.CharacterConditions.Moving && (!disEnable || _skipCondition != 0) &&  sb.nowPosition)
				{
					Debug.Log($"ss{sister.nowMove}");

					//一定時間経過で戦闘思考を、なしでない限りは優先する状態に戻す
					if (stateJudge >= sister.stateResetRes && sister.priority != SisterParameter.MoveType.なし)
					{
						//ステートリセット
						sister.nowMove = sister.priority;
						stateJudge = 0.0f;
					}



					//攻撃ステートで
					if (sister.nowMove == SisterParameter.MoveType.攻撃)
					{

						Debug.Log($"s1{sister.targetCondition.Length}");
						//ターゲットがいないならターゲットを探します。
						if (SManager.instance.target == null)
						{

							judgeSequence = 0;

							//五番目までやる
							//五番目までだから長さから1引いてる
                            for (int i = 0;i < sister.targetCondition.Length;i++)
                            {
							//	int skiCheck =  (int)Mathf.Pow(2, i)
								//0乗は１
								//クールタイム中で、なおかつスキップコンディションに当てはまらないなら処理を飛ばす。
								//シフト演算？
								if (disEnable && (_skipCondition & (int)Mathf.Pow(2, i)) != (int)Mathf.Pow(2, i))
                                {
									continue;
                                }

								SManager.instance.target = TargetSelect(sister.targetCondition[i], sister.AttackCondition[i]);
								
								if (SManager.instance.target != null)
                                {
									Debug.Log($"{SManager.instance.target.name}");
									judgeSequence = i;
									break;
                                }


							}
							Debug.Log($"s2");

							//それでもターゲットがいなかったら補足行動
							if (SManager.instance.target == null && !disEnable)
							{
								if (sister.AttackCondition[5].condition == FireCondition.ActJudge.回復行動に移行 || sister.AttackCondition[5].condition == FireCondition.ActJudge.支援行動に移行)
								{
									AttackStateChange(sister.AttackCondition[5]);
									return;
								}
								else if (sister.AttackCondition[5].condition != FireCondition.ActJudge.なにもしない)
								{
									SManager.instance.target = SManager.instance.targetList[RandomValue(0, SManager.instance.targetList.Count - 1)];
									judgeSequence = 5;
								}
							}

							//EnemyRecordとtargetConditionは一致してる。
							//敵情報更新
						}

                        else
                        {
							SManager.instance.target = null;
							return;
						}
						Debug.Log($"s3");
						if (SManager.instance.target != null)
						{
							//Debug.Log("使用魔法設定ができてないのが問題");

							//Disenaでスキップコンディションに含まれていないなら戻る
							if (sister.nowMove != SisterParameter.MoveType.攻撃 || (disEnable && ((_skipCondition & (int)Mathf.Pow(2, judgeSequence)) != (int)Mathf.Pow(2, judgeSequence))))
							{
								Debug.Log("ｄｄｄｄｆｓｆｓ");
								judgeSequence = 0;
								SManager.instance.target = null;
								return;
							}
							Debug.Log($"s4{judgeSequence}");

							AttackAct(sister.AttackCondition[judgeSequence]);

						}


					}

					//支援の時は対象は決まってるので条件に当てはまる状況か
					//そして当てはまる支援があるかを調べる
					else if (sister.nowMove == SisterParameter.MoveType.支援)
					{
						SManager.instance.target = GManager.instance.Player;

						for (int i = 0; i < sister.supportPlan.Length; i++)
						{
							if (disEnable && (_skipCondition & (int)Mathf.Pow(2, i)) != (int)Mathf.Pow(2, i))
							{
								continue;
							}
							if (i == sister.supportPlan.Length - 1)
                            {
								SupportAct(sister.supportPlan[i]);
								Debug.Log($"df{i}");
								judgeSequence = i;
							}
							else if(SupportJudge(sister.supportPlan[i]))
							{

								SupportAct(sister.supportPlan[i]);
								judgeSequence = i;
								Debug.Log($"ff{i}");
								break;
							}
						}

                        if (disEnable && (_skipCondition & (int)Mathf.Pow(2, judgeSequence)) != (int)Mathf.Pow(2, judgeSequence))
						{
							judgeSequence = 0;
							SManager.instance.target = null;
							return;
                        }
					}
					//支援と同じ
					else if (sister.nowMove == SisterParameter.MoveType.回復)
					{
						SManager.instance.target = GManager.instance.Player;
						for (int i = 0; i < sister.recoverCondition.Length; i++)
						{
							if (disEnable && (_skipCondition & (int)Mathf.Pow(2, i)) != (int)Mathf.Pow(2, i))
							{
								continue;
							}
							if (i == sister.recoverCondition.Length - 1)
							{
								RecoverAct(sister.recoverCondition[i]);
								judgeSequence = i;
								Debug.Log($"de{i}");
							}
							else if (HealJudge(sister.recoverCondition[i]))
							{
								RecoverAct(sister.recoverCondition[i]);
								judgeSequence = i;
							
								break;
							}
						}
						if (disEnable && (_skipCondition & (int)Mathf.Pow(2, judgeSequence)) != (int)Mathf.Pow(2, judgeSequence))
						{
							judgeSequence = 0;
							SManager.instance.target = null;
							return;
						}
					}

				}
				#endregion

				//ちゃんとターゲットと使用魔法が設定されているかのチェック
				//それが終われば発射

				if (SManager.instance.target != null && SManager.instance.useMagic != null)
				{
					//MPないなら逃げて
					if (sb.mp < SManager.instance.useMagic.useMP)
					{
						sb.PositionJudge();
						return;
					}

					//	bool isWrong = false;
					//使用魔法が攻撃で、かつターゲットが敵である。
					if (SManager.instance.useMagic.mType == SisMagic.MagicType.Attack)
					{
						if (sister.nowMove != SisterParameter.MoveType.攻撃)
						{
							return;
						}

						coolTime = sister.attackCT[judgeSequence];
						_skipCondition = sister.atSkipList[judgeSequence];
					}
					else if (SManager.instance.useMagic.mType == SisMagic.MagicType.Recover)
					{
						if (sister.nowMove != SisterParameter.MoveType.回復)
						{
							return;
							//isWrong = true;
						}
						coolTime = sister.attackCT[judgeSequence];
						_skipCondition = sister.atSkipList[judgeSequence];
					}
					else
					{
						if (sister.nowMove != SisterParameter.MoveType.支援)
						{
							return;
						}
						coolTime = sister.healCT[judgeSequence];
						_skipCondition = sister.hSkipList[judgeSequence];
					}

					if (_condition.CurrentState == CharacterStates.CharacterConditions.Normal)
					{
					//	Debug.Log("ssd");
						actionNum = (int)SManager.instance.useMagic.castType;
						_movement.ChangeState(CharacterStates.MovementStates.Cast);
						_condition.ChangeState(CharacterStates.CharacterConditions.Moving);
						_controller.SetHorizontalForce(0);
					}


					//ターゲットがいて使用する魔法もあって使用魔法とターゲットもかみ合っているなら

					ActionFire();

				}


			}

			//現在戦い中じゃないなら
			else if (sb.nowState != BrainAbility.SisterState.戦い )
			{
				//一応リセットと戦闘開始時にリセットしてもらえるよう仕込み


				if (sister.autoHeal && !disEnable)
				{
					//bool healAct = false;


					if (_condition.CurrentState != CharacterStates.CharacterConditions.Moving)
					{
						healJudge += _controller.DeltaTime;
						if (healJudge >= 3f)
						{

						for (int i = 0; i < sister.recoverCondition.Length; i++)
						{
								if (disEnable && (_skipCondition & (int)Mathf.Pow(2, i)) != (int)Mathf.Pow(2, i))
								{
									continue;
								}
								if (HealJudge(sister.nRecoverCondition[i]))
								{
									RecoverAct(sister.nRecoverCondition[i]);
									judgeSequence = i;
									
									break;
								}
						}


							healJudge = 0;
						}
						else if (healJudge < 3f)
						{

							SManager.instance.target = null;
						}
					}
					if (SManager.instance.target != null && SManager.instance.useMagic != null)
					{

						if (_condition.CurrentState == CharacterStates.CharacterConditions.Normal)
						{
							actionNum = (int)SManager.instance.useMagic.castType;
							_movement.ChangeState(CharacterStates.MovementStates.Cast);
							_condition.ChangeState(CharacterStates.CharacterConditions.Moving);
						}
						coolTime = sister.autHealCT[judgeSequence];
						_skipCondition = sister.ahSkipList[judgeSequence];

						ActionFire();
					}
				}

			}



		}

		/// <summary>
		/// 詠唱管理メソッド
		/// 詠唱完了したら攻撃状態に入ってモーション遷移
		/// </summary>
		/// <param name="random"></param>
		public void ActionFire(float random = 0.0f)
		{
			if (disEnable)
			{
				waitCast = 0;
				disEnable = false;
			}

			//ランダムに入れてもいいけど普通に入れてもいい

			//使用する魔法のMPがあって、かつ標的がいるなら
			if (sb.mp >= SManager.instance.useMagic.useMP && SManager.instance.target != null)
			{
				//活動開始
				//
				waitCast += _controller.DeltaTime;
				_controller.SetHorizontalForce(0);
				float dir = Mathf.Sign(SManager.instance.target.transform.position.x - transform.position.x);
				sb.SisFlip(dir);

				

				//詠唱終わったら
				if (waitCast >= SManager.instance.useMagic.castTime)
				{



					soundStart = false;
					disEnable = true;
					//GManager.instance.StopSound(castSound, 0.5f);
					_movement.ChangeState(CharacterStates.MovementStates.Attack);
					waitCast = 0;
					actionNum = (int)SManager.instance.useMagic.FireType;
					Addressables.InstantiateAsync(SManager.instance.useMagic.castBreak, sb.firePosition.position, sb.firePosition.rotation);
					if (castEffect != null)
					{
						Addressables.ReleaseInstance(castEffect);
						castEffect = null;
					}
					//ここからの処理ではアニメーションイベントを使う
				}
				//詠唱中なら
				else if(waitCast > 0.3)
				{

					//sb.//(SManager.instance.useMagic.castAnime);
					if (!soundStart)
					{
						 CastCircle().Forget();

						GManager.instance.PlaySound(castSound, transform.position);
						
						soundStart = true;
					}
				}
			}
			//Mp足りないかターゲット消えたなら
			else
			{
				//	詠唱中止。敵が死んで消えたりとか
				GManager.instance.StopSound(castSound, 0.5f);
		//		if (castEffect != null)
			//	{
					Addressables.ReleaseInstance(castEffect);
					castEffect = null;
			//	}
				_movement.ChangeState(CharacterStates.MovementStates.Idle);
				_condition.ChangeState(CharacterStates.CharacterConditions.Normal);
				actionNum = 0;
				waitCast = 0;

				return;
			}
		}

		/// <summary>
		/// XとYの間で乱数を出す
		/// </summary>
		/// <param name="X"></param>
		/// <param name="Y"></param>
		/// <returns></returns>
		public int RandomValue(int X, int Y, bool setSeed = false, int seed = 0)
		{
			//Random rnd = new UnityEngine.Random();

			/*	if (setSeed)
				{
					Random.InitState(seed);
				}*/
			//MyRandom random = new MyRandom();
			return UnityEngine.Random.Range(X, Y + 1);

		}




		/// <summary>
		///魔力回復などのために行動ごとにクールタイム設定可能 
		/// </summary>
		void CoolTimeWait()
		{
			if (disEnable && _condition.CurrentState != CharacterStates.CharacterConditions.Moving)
			{
			//	Debug.Log($"ddddd{coolTime}ｄｄ{waitCast}d{SManager.instance.useMagic}");
				waitCast += _controller.DeltaTime;
				//sb.//("Stand");
				//逃げる
				sb.PositionJudge();
				if (waitCast >= coolTime + 0.5f)
				{
					disEnable = false;
					_skipCondition = 0;
					waitCast = 0;
					//SManager.instance.target.MMGetComponentNoAlloc<EnemyAIBase>().TargetEffectCon(3);
					SManager.instance.target = null;
				}
			}

		}

		/// <summary>
		/// 攻撃時アニメイベント
		/// 詠唱完了時、射撃モーションにて一度呼ぶ
		/// 魔法レベルと属性で音判断
		/// </summary>
		public void EventFire()
		{
		//	Debug.Log("きtyaaaa");
			fireStart = true;
			//待てこれキャストサウンド介する必要ある？
			//普通に判断して普通に入れよ
		//	FireSoundJudge();
		}

		/// <summary>
		/// 詠唱モーションのアニメイベント
		/// 詠唱中のために音とエフェクトをセット
		/// 使用する魔法を引数にして魔法レベルと属性で見る
		/// 詠唱の音やエフェクト変えたいならここでいじる
		public async UniTaskVoid CastCircle()
		{

			Vector3 posi = new Vector3(transform.position.x, transform.position.y - 11, 40);
			//発生位置をPlayer
			castEffect = await Addressables.InstantiateAsync(SManager.instance.useMagic.castEffect, posi, Quaternion.Euler(-98,0,0));
			//	}

			float dir = Mathf.Sign(SManager.instance.target.transform.position.x - transform.position.x);
			sb.SisFlip(dir);


		}


		/// <summary>
		/// 外部から中断させる
		/// </summary>
		public void MagicEnd()
		{
			SManager.instance.target = null;
			_skipCondition = 0;
			//disEnable = false;
			stateJudge = 0;
			waitCast = 0;
			coolTime = 0;
			//SManager.instance.target.MMGetComponentNoAlloc<EnemyAIBase>().TargetEffectCon(3);
			
			actionNum = 0;
			_condition.ChangeState(CharacterStates.CharacterConditions.Normal);
			_movement.ChangeState(CharacterStates.MovementStates.Idle);
		}

		/// <summary>
		/// MPが魔法使用できないほど減少して、なおかつMP支払い前であるかどうかを確認するメソッド
		/// </summary>
		public bool MPCheck()
        {

			return (SManager.instance.useMagic.useMP > sb.mp && bCount < 2);

        }



		/// <summary>
		/// for文ではないが	bcountを超えるまでuseMagicが真なので発動し続ける
		/// 弾丸を作るメソッド
		/// </summary>
		/// <param name="hRandom"></param>
		/// <param name="vRandom"></param>
		void MagicUse(int hRandom, int vRandom)
		{

			if (!fireStart || delayNow)
			{
				return;
			}
            //	Debug.Log("きてる");
            //	Debug.Log($"ハザード{SManager.instance.useMagic.name}標的{SManager.instance.target}動作{sister.nowMove}");
            //魔法使用中MagicUseでかつ弾丸生成中でなければ

            if (_movement.CurrentState == CharacterStates.MovementStates.Combination)
            {
				return;
            }



			bCount += 1;
			//弾の発射というか生成位置
			Vector3 goFire = sb.firePosition.position;
			//弾が一発目なら
			if (bCount == 1)
			{
				SManager.instance.restoreTarget = SManager.instance.target;
				//   MyInstantiate(SManager.instance.useMagic.fireEffect, goFire, Quaternion.identity).Forget();
				//Addressables.InstantiateAsync(SManager.instance.useMagic.fireEffect, goFire, Quaternion.identity);
				if (SManager.instance.useMagic.fireType == SisMagic.FIREBULLET.RAIN)
				{
					//山なりの弾道で打ちたいときとか射出角度決めれたらいいかも
					//位置をランダムにすれば角度はどうでもいい説もある
					SManager.instance.useAngle = GetAim(sb.firePosition.position, SManager.instance.restoreTarget.transform.position);

				}
				sb.mp -= SManager.instance.useMagic.useMP;
			}

			//敵の位置にサーチ攻撃するとき
			if (SManager.instance.useMagic.isChaice)
			{
				goFire.Set(SManager.instance.restoreTarget.transform.position.x, SManager.instance.restoreTarget.transform.position.y, SManager.instance.restoreTarget.transform.position.y);

			}
			//ランダムな位置に発生するとき
			else if (hRandom != 0 || vRandom != 0)
			{
				//Transform goFire = firePosition;


				float xRandom = 0;
				float yRandom = 0;
				if (hRandom != 0)
				{

					xRandom = RandomValue(-hRandom, hRandom);

				}
				if (vRandom != 0)
				{
					yRandom = RandomValue(-vRandom, vRandom);
				}
				//	xRandom = RandomValue(RandomValue(-random,0),RandomValue(0, random));
				//	yRandom = RandomValue(RandomValue(-random, 0), RandomValue(0, random));

				goFire = new Vector3(sb.firePosition.position.x + xRandom, sb.firePosition.position.y + yRandom, 0);//銃口から
				
			}

			//Debug.Log($"魔法の名前5{SManager.instance.useMagic.hiraganaName}");
			//    MyInstantiate(SManager.instance.useMagic.effects, goFire, Quaternion.identity).Forget();//.Result;//発生位置をPlayer
			//即座に発生する弾丸の一発目なら
			if (SManager.instance.useMagic.delayTime == 0 || bCount == 1)
			{
				Addressables.InstantiateAsync(SManager.instance.useMagic.effects, goFire, Quaternion.Euler(SManager.instance.useMagic.startRotation));
			}
			//2発目以降の弾で生成中じゃないなら
			else if (bCount > 1 && !delayNow)
			{
				DelayInstantiate(SManager.instance.useMagic.effects, goFire, Quaternion.Euler(SManager.instance.useMagic.startRotation)).Forget();
			}

                //	詠唱中止。敵が死んで消えたりとか
			if (SManager.instance.restoreTarget == null)
			{
				

				_movement.ChangeState(CharacterStates.MovementStates.Idle);
				_condition.ChangeState(CharacterStates.CharacterConditions.Normal);
				actionNum = 0;
			}

			//弾丸を生成し終わったら
			if (bCount >= SManager.instance.useMagic.bulletNumber)
			{
				//Debug.Log($"テンペスト{SManager.instance.useMagic.name}標的{SManager.instance.target}動作{sister.nowMove}");

				//disEnable = true;
				bCount = 0;
				_condition.ChangeState(CharacterStates.CharacterConditions.Normal);

				actionNum = 0;
				_movement.ChangeState(CharacterStates.MovementStates.Idle);
				SManager.instance.useMagic = null;
				fireStart = false;
				if (SManager.instance.restoreTarget != null && SManager.instance.target != GManager.instance.Player) 
				{
					SManager.instance.restoreTarget.MMGetComponentNoAlloc<EnemyAIBase>().TargetEffectCon(3);
				}

				sb.PositionJudge();
			}
			//	bCount += 1;
		}




		///判断に利用
		#region
		/// <summary>
		/// 攻撃ステートでターゲットを決定
		/// </summary>
		/// <param name="condition"></param>
		public GameObject TargetSelect(AttackJudge condition, FireCondition act)
		{
			//Debug.Log($"判断番号{judgeSequence}");
			//		bool testes = false;
			//	targetJudge = 0;
			//float value = 0.0f;//HPなどの比較のための数字を
			switch (condition.condition)
			{
				//-----------------------------------------------------------------------------------------------------
				case AttackJudge.TargetJudge.指定なし:
					//	ランダムバリュー使ってレコードから指定

					if (act.condition == FireCondition.ActJudge.回復行動に移行 || act.condition == FireCondition.ActJudge.支援行動に移行)
					{

						AttackStateChange(act);
						return null;
					}
					else if (act.condition == FireCondition.ActJudge.なにもしない)
					{
						return null;
					}
					targetCanList = new List<GameObject>(SManager.instance.targetList);
					targetCanStatus = new List<EnemyAIBase>(SManager.instance.targetCondition);
					return SecondTargetJudge(targetCanList, condition, targetCanStatus);

				//-----------------------------------------------------------------------------------------------------
				case AttackJudge.TargetJudge.プレイヤーのHPが規定値に達した際:
					//	ランダムバリュー使ってレコードから指定
					if (condition.highOrLow)
					{
						if (sb.pc.HPRatio() >= condition.percentage / 100f)
						{
							if (act.condition == FireCondition.ActJudge.回復行動に移行 || act.condition == FireCondition.ActJudge.支援行動に移行)
							{
								Debug.Log($"zz");
								AttackStateChange(act);
								return null;
							}
							else if (act.condition == FireCondition.ActJudge.なにもしない)
							{
								return null;
							}
							targetCanList = new List<GameObject>(SManager.instance.targetList);
							targetCanStatus = new List<EnemyAIBase>(SManager.instance.targetCondition);
							return SecondTargetJudge(targetCanList, condition, targetCanStatus);
						}
					}
					else
					{
						//		Debug.Log($"あああ{sb.pc.HPRatio()}と{condition.percentage / 100}");
						//Debug.Log($"あああ{condition.percentage / 100f}");
						if (sb.pc.HPRatio() <= condition.percentage / 100f)
						{

							if (act.condition == FireCondition.ActJudge.回復行動に移行 || act.condition == FireCondition.ActJudge.支援行動に移行)
							{
								//kuroko++;
								//Debug.Log($"朝顔{sister.nowMove}");

								AttackStateChange(act);
								return null;
							}
							else if (act.condition == FireCondition.ActJudge.なにもしない)
							{
								return null;
							}

							targetCanList = new List<GameObject>(SManager.instance.targetList);
							targetCanStatus = new List<EnemyAIBase>(SManager.instance.targetCondition);
							return SecondTargetJudge(targetCanList, condition, targetCanStatus);
						}
					}
					break;
				//-----------------------------------------------------------------------------------------------------
				case AttackJudge.TargetJudge.プレイヤーのMPが規定値に達した際:
					//	ランダムバリュー使ってレコードから指定
					if (condition.highOrLow)
					{
						if (GManager.instance.mp / GManager.instance.maxMp >= condition.percentage / 100f)
						{
							if (act.condition == FireCondition.ActJudge.回復行動に移行 || act.condition == FireCondition.ActJudge.支援行動に移行)
							{

								AttackStateChange(act);
								return null;
							}
							else if (act.condition == FireCondition.ActJudge.なにもしない)
							{
								return null;
							}
							targetCanList = new List<GameObject>(SManager.instance.targetList);
							targetCanStatus = new List<EnemyAIBase>(SManager.instance.targetCondition);
							return SecondTargetJudge(targetCanList, condition, targetCanStatus);
						}
					}
					else
					{
						if (GManager.instance.mp / GManager.instance.maxMp <= condition.percentage / 100f)
						{

							if (act.condition == FireCondition.ActJudge.回復行動に移行 || act.condition == FireCondition.ActJudge.支援行動に移行)
							{

								AttackStateChange(act);
								return null;
							}
							else if (act.condition == FireCondition.ActJudge.なにもしない)
							{
								return null;
							}

							targetCanList = new List<GameObject>(SManager.instance.targetList);
							targetCanStatus = new List<EnemyAIBase>(SManager.instance.targetCondition);
							return SecondTargetJudge(targetCanList, condition, targetCanStatus);
						}
					}
					break;
				//-----------------------------------------------------------------------------------------------------
				case AttackJudge.TargetJudge.プレイヤーが状態異常にかかった時://未実装
															  //	ランダムバリュー使ってレコードから指定
					targetCanList = new List<GameObject>(SManager.instance.targetList);
					targetCanStatus = new List<EnemyAIBase>(SManager.instance.targetCondition);
					return SecondTargetJudge(targetCanList, condition, targetCanStatus);
					
				//-----------------------------------------------------------------------------------------------------
				case AttackJudge.TargetJudge.状態異常にかかってる敵://未実装
														 //	ランダムバリュー使ってレコードから指定
					targetCanList = new List<GameObject>(SManager.instance.targetList);
					targetCanStatus = new List<EnemyAIBase>(SManager.instance.targetCondition);
					return SecondTargetJudge(targetCanList, condition, targetCanStatus);
					
				//-----------------------------------------------------------------------------------------------------
				case AttackJudge.TargetJudge.自分のMPが規定値に達した際:
					if (condition.highOrLow)
					{
						if (sb.mp / SManager.instance.sisStatus.maxMp >= condition.percentage / 100f)
						{
							if (act.condition == FireCondition.ActJudge.回復行動に移行 || act.condition == FireCondition.ActJudge.支援行動に移行)
							{

								AttackStateChange(act);
								return null;
							}
							else if (act.condition == FireCondition.ActJudge.なにもしない)
							{
								return null;
							}
							targetCanList = new List<GameObject>(SManager.instance.targetList);
							targetCanStatus = new List<EnemyAIBase>(SManager.instance.targetCondition);
							return SecondTargetJudge(targetCanList, condition, targetCanStatus);
						}
					}
					else
					{

						if (sb.mp / SManager.instance.sisStatus.maxMp <= condition.percentage / 100f)
						{
							if (act.condition == FireCondition.ActJudge.回復行動に移行 || act.condition == FireCondition.ActJudge.支援行動に移行)
							{

								AttackStateChange(act);
								return null;
							}
							else if (act.condition == FireCondition.ActJudge.なにもしない)
							{
								return null;
							}
							targetCanList = new List<GameObject>(SManager.instance.targetList);
							targetCanStatus = new List<EnemyAIBase>(SManager.instance.targetCondition);
							return SecondTargetJudge(targetCanList, condition, targetCanStatus);
						}
					}
					break;
				//-----------------------------------------------------------------------------------------------------
				case AttackJudge.TargetJudge.強敵の存在:
					//強敵を優先
					targetCanList = new List<GameObject>();
					targetCanStatus = new List<EnemyAIBase>();
					if (condition.highOrLow)
					{
						for (int i = 0; i < SManager.instance.targetList.Count; i++)
						{
							if (SManager.instance.targetCondition[i].status.strong)
							{
								if (act.condition == FireCondition.ActJudge.回復行動に移行 || act.condition == FireCondition.ActJudge.支援行動に移行)
								{

									AttackStateChange(act);
									return null;
								}
								else if (act.condition == FireCondition.ActJudge.なにもしない)
								{
									return null;
								}
								targetCanList.Add(SManager.instance.targetList[i]);
								//break;
								targetCanStatus.Add(SManager.instance.targetCondition[i]);
							}
						}
					}
					else
					{
						for (int i = 0; i < SManager.instance.targetList.Count; i++)
						{

							if (!SManager.instance.targetCondition[i].status.strong)
							{
								if (act.condition == FireCondition.ActJudge.回復行動に移行 || act.condition == FireCondition.ActJudge.支援行動に移行)
								{

									AttackStateChange(act);
									return null;
								}
								else if (act.condition == FireCondition.ActJudge.なにもしない)
								{
									return null;
								}
								targetCanList.Add(SManager.instance.targetList[i]);
								//break;
								targetCanStatus.Add(SManager.instance.targetCondition[i]);
							}
						}
					}
					return SecondTargetJudge(targetCanList, condition, targetCanStatus);
					//ここに二次処理三次処理をCanListを引数に開始

				
				//-----------------------------------------------------------------------------------------------------
				case AttackJudge.TargetJudge.敵タイプ:
					//   Soldier,//陸の雑兵

					targetCanList = new List<GameObject>();
					targetCanStatus = new List<EnemyAIBase>();

					//選ぶ敵タイプがすべてと選択されてるなら
					if (condition.percentage == 0b00011111)
					{

						if (act.condition == FireCondition.ActJudge.回復行動に移行 || act.condition == FireCondition.ActJudge.支援行動に移行)
						{

							AttackStateChange(act);
							return null;
						}
						else if (act.condition == FireCondition.ActJudge.なにもしない)
						{
							return null;
						}
						targetCanList = new List<GameObject>(SManager.instance.targetList);
						targetCanStatus = new List<EnemyAIBase>(SManager.instance.targetCondition);


						return SecondTargetJudge(targetCanList, condition, targetCanStatus);
						//break;
					}
					else
					{
						//int test;
						for (int i = 0; i < SManager.instance.targetList.Count; i++)
						{
							//test = ;
							//Debug.Log("大根");

							if ((condition.percentage & 0b00000001) == 0b00000001)
							{

								if (SManager.instance.targetCondition[i].status.kind == EnemyStatus.KindofEnemy.Soldier)
								{
									if (act.condition == FireCondition.ActJudge.回復行動に移行 || act.condition == FireCondition.ActJudge.支援行動に移行)
									{

										AttackStateChange(act);
										return null;
									}
									else if (act.condition == FireCondition.ActJudge.なにもしない)
									{
										return null;
									}
									targetCanList.Add(SManager.instance.targetList[i]);
									targetCanStatus.Add(SManager.instance.targetCondition[i]);
									continue;
								}
							}
							//test = 0b01000000;
							if ((condition.percentage & 0b00000010) == 0b00000010)
							{
								if (SManager.instance.targetCondition[i].status.kind == EnemyStatus.KindofEnemy.Fly)
								{
									if (act.condition == FireCondition.ActJudge.回復行動に移行 || act.condition == FireCondition.ActJudge.支援行動に移行)
									{

										AttackStateChange(act);
										return null;
									}
									else if (act.condition == FireCondition.ActJudge.なにもしない)
									{
										return null;
									}
									targetCanList.Add(SManager.instance.targetList[i]);
									targetCanStatus.Add(SManager.instance.targetCondition[i]);
									continue;
								}
							}
							if ((condition.percentage & 0b00000100) == 0b00000100)
							{
								if (SManager.instance.targetCondition[i].status.kind == EnemyStatus.KindofEnemy.Shooter)
								{
									if (act.condition == FireCondition.ActJudge.回復行動に移行 || act.condition == FireCondition.ActJudge.支援行動に移行)
									{

										AttackStateChange(act);
										return null;
									}
									else if (act.condition == FireCondition.ActJudge.なにもしない)
									{
										return null;
									}
									//	siroko++;
									//		Debug.Log($"今の数字{siroko}");
									targetCanList.Add(SManager.instance.targetList[i]);
									targetCanStatus.Add(SManager.instance.targetCondition[i]);
									continue;
								}
							}
							if ((condition.percentage & 0b00001000) == 0b00001000)
							{
								if (SManager.instance.targetCondition[i].status.kind == EnemyStatus.KindofEnemy.Knight)
								{
									if (act.condition == FireCondition.ActJudge.回復行動に移行 || act.condition == FireCondition.ActJudge.支援行動に移行)
									{

										AttackStateChange(act);
										return null;
									}
									else if (act.condition == FireCondition.ActJudge.なにもしない)
									{
										return null;
									}
									//	siroko++;
									//		Debug.Log($"今の数字{siroko}");
									targetCanList.Add(SManager.instance.targetList[i]);
									targetCanStatus.Add(SManager.instance.targetCondition[i]);
									continue;
								}
							}
							if ((condition.percentage & 0b00010000) == 0b00010000)
							{
								if (SManager.instance.targetCondition[i].status.kind == EnemyStatus.KindofEnemy.Trap)
								{
									if (act.condition == FireCondition.ActJudge.回復行動に移行 || act.condition == FireCondition.ActJudge.支援行動に移行)
									{

										AttackStateChange(act);
										return null;
									}
									else if (act.condition == FireCondition.ActJudge.なにもしない)
									{
										return null;
									}
									targetCanList.Add(SManager.instance.targetList[i]);
									targetCanStatus.Add(SManager.instance.targetCondition[i]);
									continue;
								}
							}
						}
						return SecondTargetJudge(targetCanList, condition, targetCanStatus);
					}

					
					//-----------------------------------------------------------------------------------------------------
			}
			return null;
		}

		/// <summary>
		/// 攻撃ステートで使用魔法を決定
		/// </summary>
		/// <param name="condition"></param>
		public void AttackAct(FireCondition condition)
		{


			if (condition.UseMagic == null)
			{
				Debug.Log("ghdks");
				switch (condition.condition)
				{
					case FireCondition.ActJudge.斬撃属性:

						magicCanList = new List<SisMagic>();
						for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
						{
							if (SManager.instance.attackMagi[i].phyBase > 0 && SManager.instance.attackMagi[i].atType == Magic.AttackType.Slash)
							{
								magicCanList.Add(SManager.instance.attackMagi[i]);
								break;
							}
						}
						secondATMagicJudge(magicCanList, condition);
						break;
					//-----------------------------------------------------------------------------------------------------
					case FireCondition.ActJudge.刺突属性:
						magicCanList = new List<SisMagic>();
						for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
						{
							if (SManager.instance.attackMagi[i].phyBase > 0 && SManager.instance.attackMagi[i].atType == Magic.AttackType.Stab)
							{
								magicCanList.Add(SManager.instance.attackMagi[i]);
								break;
							}
						}
						secondATMagicJudge(magicCanList, condition);
						break;
					//-----------------------------------------------------------------------------------------------------
					case FireCondition.ActJudge.打撃属性:
						magicCanList = new List<SisMagic>();
						for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
						{
							if (SManager.instance.attackMagi[i].phyBase > 0 && SManager.instance.attackMagi[i].atType == Magic.AttackType.Strike)
							{
								magicCanList.Add(SManager.instance.attackMagi[i]);
								break;
							}
						}
						secondATMagicJudge(magicCanList, condition);
						break;
					//-----------------------------------------------------------------------------------------------------
					case FireCondition.ActJudge.聖属性:
						magicCanList = new List<SisMagic>();
						for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
						{
							if (SManager.instance.attackMagi[i].holyBase > 0)
							{
								magicCanList.Add(SManager.instance.attackMagi[i]);
								break;
							}
						}
						secondATMagicJudge(magicCanList, condition);
						break;
					//-----------------------------------------------------------------------------------------------------
					case FireCondition.ActJudge.闇属性:
						magicCanList = new List<SisMagic>();
						for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
						{
							if (SManager.instance.attackMagi[i].darkBase > 0)
							{
								magicCanList.Add(SManager.instance.attackMagi[i]);
								break;
							}
						}
						secondATMagicJudge(magicCanList, condition);
						break;
					//-----------------------------------------------------------------------------------------------------
					case FireCondition.ActJudge.炎属性:
						Debug.Log($"rrrr");
						magicCanList = new List<SisMagic>();
						for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
						{
							if (SManager.instance.attackMagi[i].fireBase > 0)
							{
								magicCanList.Add(SManager.instance.attackMagi[i]);
								break;
							}
						}
						//Debug.Log($"asgd{magicCanList[0].name}");
						secondATMagicJudge(magicCanList, condition);
						break;
					//-----------------------------------------------------------------------------------------------------
					case FireCondition.ActJudge.雷属性:
						//Debug.Log($"ssssss");
						magicCanList = new List<SisMagic>();
						for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
						{
							if (SManager.instance.attackMagi[i].thunderBase > 0)
							{
								//		Debug.Log($"第二段階{SManager.instance.attackMagi[i].name}");
								magicCanList.Add(SManager.instance.attackMagi[i]);
								break;
							}
						}
						secondATMagicJudge(magicCanList, condition);
						break;
					//-----------------------------------------------------------------------------------------------------
					case FireCondition.ActJudge.毒属性:
						for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
						{
							if (SManager.instance.attackMagi[i].thunderBase >= 0)
							{
								magicCanList.Add(SManager.instance.attackMagi[i]);
								break;
							}
						}
						secondATMagicJudge(magicCanList, condition);
						break;
					//-----------------------------------------------------------------------------------------------------
					case FireCondition.ActJudge.属性指定なし:

						magicCanList = new List<SisMagic>(SManager.instance.attackMagi);

						secondATMagicJudge(magicCanList, condition);
						break;
					//-----------------------------------------------------------------------------------------------------
					case FireCondition.ActJudge.移動速度低下攻撃://未実装
						for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
						{
							if (SManager.instance.attackMagi[i].thunderBase >= 0)
							{
								magicCanList.Add(SManager.instance.attackMagi[i]);
								break;
							}
						}
						secondATMagicJudge(magicCanList, condition);
						break;
					//-----------------------------------------------------------------------------------------------------
					case FireCondition.ActJudge.攻撃力低下攻撃://未実装
						for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
						{
							if (SManager.instance.attackMagi[i].thunderBase >= 0)
							{
								magicCanList.Add(SManager.instance.attackMagi[i]);
								break;
							}
						}
						secondATMagicJudge(magicCanList, condition);
						break;
					//-----------------------------------------------------------------------------------------------------
					case FireCondition.ActJudge.防御力低下攻撃://未実装
						for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
						{
							if (SManager.instance.attackMagi[i].thunderBase >= 0)
							{
								magicCanList.Add(SManager.instance.attackMagi[i]);
								break;
							}
						}
						secondATMagicJudge(magicCanList, condition);
						break;
				}
			}
			else
			{
				if (judgeSequence != 2)
				{
					Debug.Log($"おかしなとき{condition.condition}{condition.UseMagic}{judgeSequence}");
				}
				if (condition.condition == FireCondition.ActJudge.回復行動に移行 || condition.condition == FireCondition.ActJudge.支援行動に移行)
				{

					AttackStateChange(condition);
					return;
				}
				else if (condition.condition == FireCondition.ActJudge.なにもしない)
				{
					return;
				}
				SManager.instance.useMagic = condition.UseMagic;
			}
		}

		public bool SupportJudge(SupportCondition condition)
		{

			switch (condition.sCondition)
			{
				/*	case SupportCondition.SupportStatus.かかっていない支援がある:
						//useSupport = null;
						for (int i = 0; i < SManager.instance.supportMagi.Count; i++)
						{
							if (!SManager.instance.supportMagi[i].effectNow)
							{
								magicCanList.Add(SManager.instance.supportMagi[i]);
							}
						}

						return useSupport != null ? true : false;
					//break;*/

				//-----------------------------------------------------------------------------------------------------
				case SupportCondition.SupportStatus.プレイヤーの体力が規定値の時:

					if (condition.highOrLow)
					{
						return sb.pc.HPRatio() >= condition.percentage / 100f ? true : false;
					}
					else
					{
						return sb.pc.HPRatio() <= condition.percentage / 100f ? true : false;
					}
				//return GManager.instance.hp == GManager.instance.maxHp ? true : false;
				//-----------------------------------------------------------------------------------------------------
				case SupportCondition.SupportStatus.プレイヤーのMPが規定値に達した際:

					if (condition.highOrLow)
					{
						return GManager.instance.mp / GManager.instance.maxMp >= condition.percentage / 100f ? true : false;
					}
					else
					{
						return GManager.instance.mp / GManager.instance.maxMp <= condition.percentage / 100f ? true : false;
					}
				//return GManager.instance.hp == GManager.instance.maxHp ? true : false;
				//-----------------------------------------------------------------------------------------------------
				case SupportCondition.SupportStatus.自分のMPが規定値に達した際:

					if (condition.highOrLow)
					{
						return sb.mp / SManager.instance.sisStatus.maxMp >= condition.percentage / 100f ? true : false;
					}
					else
					{
						return sb.mp / SManager.instance.sisStatus.maxMp <= condition.percentage / 100f ? true : false;
					}
				//return GManager.instance.hp == GManager.instance.maxHp ? true : false;
				//-----------------------------------------------------------------------------------------------------
				case SupportCondition.SupportStatus.プレイヤーが状態異常にかかった時:

					return GManager.instance.badCondition ? true : false;
				//-----------------------------------------------------------------------------------------------------
				case SupportCondition.SupportStatus.強敵がいるかどうか:
					//強敵を優先

					if (condition.highOrLow)
					{
						for (int i = 0; i < SManager.instance.targetList.Count; i++)
						{
							if (SManager.instance.targetCondition[i].status.strong)
							{
								return true;
							}
						}
					}
					else
					{
						for (int i = 0; i < SManager.instance.targetList.Count; i++)
						{
							if (SManager.instance.targetCondition[i].status.strong)
							{
								return false;
							}
						}
						return true;
					}
					return false;
				//	break;
				//-----------------------------------------------------------------------------------------------------
				case SupportCondition.SupportStatus.敵タイプ:
					//   Soldier,//陸の雑兵
					if ((0b00011111 & condition.percentage) == 0b00011111)
					{

						return true;
					}
					//int test;
					for (int i = 0; i < SManager.instance.targetList.Count; i++)
					{
						//test = ;


						if ((0b00000001 & condition.percentage) == 0b00000001)
						{
							if (SManager.instance.targetCondition[i].status.kind == EnemyStatus.KindofEnemy.Soldier)
							{
								return true;
							}
						}
						//test = 0b01000000;
						else if ((0b00000010 & condition.percentage) == 0b00000010)
						{
							if (SManager.instance.targetCondition[i].status.kind == EnemyStatus.KindofEnemy.Fly)
							{
								return true;
							}
						}
						else if ((0b00000100 & condition.percentage) == 0b00000100)
						{
							if (SManager.instance.targetCondition[i].status.kind == EnemyStatus.KindofEnemy.Shooter)
							{
								return true;
							}
						}
						else if ((0b00001000 & condition.percentage) == 0b00001000)
						{
							if (SManager.instance.targetCondition[i].status.kind == EnemyStatus.KindofEnemy.Knight)
							{
								return true;
							}
						}
						else if ((0b00010000 & condition.percentage) == 0b00010000)
						{
							if (SManager.instance.targetCondition[i].status.kind == EnemyStatus.KindofEnemy.Trap)
							{
								return true;
							}
						}
					}
					return false;
				//-----------------------------------------------------------------------------------------------------
				case SupportCondition.SupportStatus.任意の支援が切れているとき:

					for (int i = 0; i < SManager.instance.supportMagi.Count; i++)
					{
						if (!SManager.instance.supportMagi[i].effectNow && SManager.instance.supportMagi[i].sType == condition.needSupport)
						{
							magicCanList.Add(SManager.instance.supportMagi[i]);
						}
					}
					return magicCanList != null ? true : false;
				//-----------------------------------------------------------------------------------------------------
				case SupportCondition.SupportStatus.指定なし:
					//	ランダムバリュー使ってレコードから指定
					return true;
					//-----------------------------------------------------------------------------------------------------
			}
			return false;
		}

		public void SupportAct(SupportCondition condition)
		{
			if (sister.nowMove != SisterParameter.MoveType.支援)
			{
				return;


			}
			List<SisMagic> candidate;
			List<int> removeNumber = new List<int>();
			if (magicCanList != null)
			{
				candidate = magicCanList;
			}
			else
			{
				candidate = new List<SisMagic>(SManager.instance.supportMagi);
			}


			if (condition.UseMagic != null)
			{

				if (condition.ActBase == SupportCondition.MagicJudge.なにもしない)
				{
					magicCanList = null;

				}
				//-----------------------------------------------------------------------------------------------------
				else if (condition.ActBase == SupportCondition.MagicJudge.回復ステートに)
				{
					magicCanList = null;
					SupportStateChange(condition.ActBase);
				}
				else if (condition.ActBase == SupportCondition.MagicJudge.攻撃ステートに)
				{
					magicCanList = null;
					SupportStateChange(condition.ActBase);

				}
				else if (condition.ActBase == SupportCondition.MagicJudge.各種支援魔法)
				{

					for (int i = 0; i < candidate.Count; i++)
					{
						if (candidate[i].sType != condition.useSupport)
						{
							removeNumber.Add(i);
						}
					}
				}

				if (removeNumber.Count != 0)
				{
					for (int i = 0; i < removeNumber.Count; i++)
					{
						candidate.Remove(candidate[removeNumber[i] - i]);
						//targetCanStatus.Remove(statusList[removeNumber[i] - i]);
					}
				}
				if (candidate.Count == 0)
				{
					return;
				}
				else
				{
					if (condition.nextCondition == SupportCondition.AdditionalJudge.指定なし)
					{
						SManager.instance.useMagic = candidate[0];
						condition.UseMagic = candidate[0];
						magicCanList = null;
					}
					else
					{
						int selectNumber = 150;
						if (condition.nextCondition == SupportCondition.AdditionalJudge.MP使用量)
						{
							if (condition.upDown)
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (selectNumber == 150 || candidate[i].useMP > candidate[selectNumber].useMP)
									{
										selectNumber = i;
									}

								}
							}
							else
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (selectNumber == 150 || candidate[i].useMP < candidate[selectNumber].useMP)
									{
										selectNumber = i;
									}
								}
							}
						}
						else if (condition.nextCondition == SupportCondition.AdditionalJudge.詠唱時間)
						{
							if (condition.upDown)
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (selectNumber == 150 || candidate[i].castTime > candidate[selectNumber].castTime)
									{
										selectNumber = i;
									}

								}
							}
							else
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (selectNumber == 150 || candidate[i].castTime < candidate[selectNumber].castTime)
									{
										selectNumber = i;
									}
								}
							}
						}
						else if (condition.nextCondition == SupportCondition.AdditionalJudge.持続効果時間)
						{
							if (condition.upDown)
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (selectNumber == 150 || candidate[i].effectTime > candidate[selectNumber].effectTime)
									{
										selectNumber = i;
									}

								}
							}
							else
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (selectNumber == 150 || candidate[i].effectTime < candidate[selectNumber].effectTime)
									{
										selectNumber = i;
									}
								}
							}
						}
						else if (condition.nextCondition == SupportCondition.AdditionalJudge.強化倍率)
						{
							if (condition.upDown)
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (selectNumber == 150 || candidate[i].mValue > candidate[selectNumber].mValue)
									{
										selectNumber = i;
									}

								}
							}
							else
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (selectNumber == 150 || candidate[i].mValue < candidate[selectNumber].mValue)
									{
										selectNumber = i;
									}
								}
							}
						}
						SManager.instance.useMagic = candidate[selectNumber];

					}
				}
			}
			else
			{
			
				SManager.instance.useMagic = condition.UseMagic;
			}
		}

		public bool HealJudge(RecoverCondition condition)
		{

			//Debug.Log($"回復判断{condition.condition}真なら上{condition.highOrLow}");

			switch (condition.condition)
			{

				/*	case RecoverCondition.RecoverStatus.かかっていない支援がある:
						//useSupport = null;
						for (int i = 0; i < SManager.instance.supportMagi.Count; i++)
						{
							if (!SManager.instance.supportMagi[i].effectNow)
							{
								magicCanList.Add(SManager.instance.supportMagi[i]);
							}
						}

						return useSupport != null ? true : false;
					//break;*/

				//-----------------------------------------------------------------------------------------------------
				case RecoverCondition.RecoverStatus.プレイヤーのHPが規定値の時:

					if (condition.highOrLow)
					{
						return sb.pc.HPRatio() >= condition.percentage / 100f ? true : false;
					}
					else
					{
						return sb.pc.HPRatio() <= condition.percentage / 100f ? true : false;
					}
				//return GManager.instance.hp == GManager.instance.maxHp ? true : false;
				//-----------------------------------------------------------------------------------------------------
				case RecoverCondition.RecoverStatus.プレイヤーのMPが規定値に達した際:

					if (condition.highOrLow)
					{
						return GManager.instance.mp / GManager.instance.maxMp >= condition.percentage / 100f ? true : false;
					}
					else
					{
						return GManager.instance.mp / GManager.instance.maxMp <= condition.percentage / 100f ? true : false;
					}
				//return GManager.instance.hp == GManager.instance.maxHp ? true : false;
				//-----------------------------------------------------------------------------------------------------
				case RecoverCondition.RecoverStatus.自分のMPが規定値に達した際:

					if (condition.highOrLow)
					{
						return sb.mp / SManager.instance.sisStatus.maxMp >= condition.percentage / 100f ? true : false;
					}
					else
					{
						return sb.mp / SManager.instance.sisStatus.maxMp <= condition.percentage / 100f ? true : false;
					}
				//return GManager.instance.hp == GManager.instance.maxHp ? true : false;
				//-----------------------------------------------------------------------------------------------------
				case RecoverCondition.RecoverStatus.プレイヤーが状態異常にかかった時:

					return GManager.instance.badCondition ? true : false;
				//-----------------------------------------------------------------------------------------------------
				case RecoverCondition.RecoverStatus.強敵がいるかどうか:
					//強敵を優先

					if (condition.highOrLow)
					{
						for (int i = 0; i < SManager.instance.targetList.Count; i++)
						{
							if (SManager.instance.targetCondition[i].status.strong)
							{
								return true;
							}
						}
					}
					else
					{
						for (int i = 0; i < SManager.instance.targetList.Count; i++)
						{
							if (SManager.instance.targetCondition[i].status.strong)
							{
								return false;
							}
						}
						return true;
					}
					return false;
				//	break;
				//-----------------------------------------------------------------------------------------------------
				case RecoverCondition.RecoverStatus.敵タイプ:
					//   Soldier,//陸の雑兵
					if ((0b00011111 & condition.percentage) == 0b00011111)
					{
						return true;
					}
					//int test;
					for (int i = 0; i < SManager.instance.targetList.Count; i++)
					{
						//test = ;


						if ((0b00000001 & condition.percentage) == 0b00000001)
						{
							if (SManager.instance.targetCondition[i].status.kind == EnemyStatus.KindofEnemy.Soldier)
							{
								return true;
							}
						}
						//test = 0b01000000;
						else if ((0b00000010 & condition.percentage) == 0b00000010)
						{
							if (SManager.instance.targetCondition[i].status.kind == EnemyStatus.KindofEnemy.Fly)
							{
								return true;
							}
						}
						else if ((0b00000100 & condition.percentage) == 0b00000100)
						{
							if (SManager.instance.targetCondition[i].status.kind == EnemyStatus.KindofEnemy.Shooter)
							{
								return true;
							}
						}
						else if ((0b00001000 & condition.percentage) == 0b00001000)
						{
							if (SManager.instance.targetCondition[i].status.kind == EnemyStatus.KindofEnemy.Knight)
							{
								return true;
							}
						}
						else if ((0b00010000 & condition.percentage) == 0b00010000)
						{
							if (SManager.instance.targetCondition[i].status.kind == EnemyStatus.KindofEnemy.Trap)
							{
								return true;
							}
						}
					}
					return false;
				//-----------------------------------------------------------------------------------------------------
				case RecoverCondition.RecoverStatus.任意の支援が切れているとき:

					for (int i = 0; i < SManager.instance.supportMagi.Count; i++)
					{
						if (!SManager.instance.supportMagi[i].effectNow && SManager.instance.supportMagi[i].sType == condition.needSupport)
						{
							return false;
						}
					}
					return true;
				//-----------------------------------------------------------------------------------------------------
				case RecoverCondition.RecoverStatus.指定なし:
					//	ランダムバリュー使ってレコードから指定
					return true;
					//-----------------------------------------------------------------------------------------------------
			}
			return false;
		}

		public void RecoverAct(RecoverCondition condition)
		{


			if (sister.nowMove != SisterParameter.MoveType.回復 && sb.nowState == BrainAbility.SisterState.戦い)
			{
				//	judgeSequence = 0;
				return;
			}
			if (condition.ActBase == RecoverCondition.MagicJudge.なにもしない)
			{
				magicCanList = null;
				return;
			}
			//-----------------------------------------------------------------------------------------------------
			else if (condition.ActBase == RecoverCondition.MagicJudge.支援ステートに)
			{
				magicCanList = null;
				RecoverStateChange(condition.ActBase);
				return;
			}
			else if (condition.ActBase == RecoverCondition.MagicJudge.攻撃ステートに)
			{
				magicCanList = null;
				RecoverStateChange(condition.ActBase);
				return;
			}

			List<SisMagic> candidate = new List<SisMagic>(SManager.instance.recoverMagi);
			List<int> removeNumber = new List<int>();


			if (condition.UseMagic == null)
			{

				/*		if (condition.ActBase == RecoverCondition.MagicJudge.なにもしない)
						{
							magicCanList = null;
							return;
						}
						//-----------------------------------------------------------------------------------------------------
						else if (condition.ActBase == RecoverCondition.MagicJudge.支援ステートに)
						{
							magicCanList = null;
							RecoverStateChange(condition.ActBase);
							return;
						}
						else if (condition.ActBase == RecoverCondition.MagicJudge.攻撃ステートに)
						{
							magicCanList = null;
							RecoverStateChange(condition.ActBase);
							return;
						}
						else if (condition.ActBase == RecoverCondition.MagicJudge.治癒魔法)
						{
							if (condition.useSupport != SisMagic.SupportType.なし)
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (candidate[i].sType != condition.useSupport)
									{
										removeNumber.Add(i);
										Debug.Log($"焼肉{candidate[i].name}");
									}
								}
							}
						}*/
				if (condition.useSupport != SisMagic.SupportType.なし)
				{
					for (int i = 0; i < candidate.Count; i++)
					{
						if (candidate[i].sType != condition.useSupport)
						{
							removeNumber.Add(i);
							Debug.Log($"焼肉{candidate[i].name}");
						}
					}
				}
				if (removeNumber.Count != 0)
				{
					for (int i = 0; i < removeNumber.Count; i++)
					{
						candidate.Remove(candidate[removeNumber[i] - i]);
						//targetCanStatus.Remove(statusList[removeNumber[i] - i]);
					}
				}
				if (candidate.Count == 0)
				{
					
					SManager.instance.useMagic = null;
					return;
				}
				else
				{
					if (condition.nextCondition == RecoverCondition.AdditionalJudge.指定なし)
					{
						SManager.instance.useMagic = candidate[0];

						condition.UseMagic = candidate[0];
						magicCanList = null;

					}
					else
					{
						int selectNumber = 150;
						if (condition.nextCondition == RecoverCondition.AdditionalJudge.MP使用量)
						{
							if (condition.upDown)
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (selectNumber == 150 || candidate[i].useMP > candidate[selectNumber].useMP)
									{
										selectNumber = i;
									}

								}
							}
							else
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (selectNumber == 150 || candidate[i].useMP < candidate[selectNumber].useMP)
									{
										selectNumber = i;
									}
								}
							}
						}
						else if (condition.nextCondition == RecoverCondition.AdditionalJudge.詠唱時間)
						{
							if (condition.upDown)
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (selectNumber == 150 || candidate[i].castTime > candidate[selectNumber].castTime)
									{
										selectNumber = i;
									}

								}
							}
							else
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (selectNumber == 150 || candidate[i].castTime < candidate[selectNumber].castTime)
									{
										selectNumber = i;
									}
								}
							}
						}
						else if (condition.nextCondition == RecoverCondition.AdditionalJudge.リジェネ回復量)
						{
							if (condition.upDown)
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (selectNumber == 150 || candidate[i].regeneAmount > candidate[selectNumber].regeneAmount)
									{
										selectNumber = i;
									}

								}
							}
							else
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (selectNumber == 150 || candidate[i].regeneAmount < candidate[selectNumber].regeneAmount)
									{
										selectNumber = i;
									}
								}
							}
						}
						else if (condition.nextCondition == RecoverCondition.AdditionalJudge.リジェネ総回復量)
						{
							if (condition.upDown)
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (selectNumber == 150 || candidate[i].regeneAmount * candidate[i].effectTime > candidate[selectNumber].regeneAmount * candidate[selectNumber].effectTime)
									{
										selectNumber = i;
									}

								}
							}
							else
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (selectNumber == 150 || candidate[i].regeneAmount * candidate[i].effectTime < candidate[selectNumber].regeneAmount * candidate[selectNumber].effectTime)
									{
										selectNumber = i;
									}
								}
							}
						}
						else if (condition.nextCondition == RecoverCondition.AdditionalJudge.持続効果時間)
						{
							if (condition.upDown)
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (selectNumber == 150 || candidate[i].effectTime > candidate[selectNumber].effectTime)
									{
										selectNumber = i;
									}

								}
							}
							else
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (selectNumber == 150 || candidate[i].effectTime < candidate[selectNumber].effectTime)
									{
										selectNumber = i;
									}
								}
							}
						}
						else if (condition.nextCondition == RecoverCondition.AdditionalJudge.回復量)
						{
							if (condition.upDown)
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (selectNumber == 150 || candidate[i].recoverBase > candidate[selectNumber].recoverBase)
									{
										selectNumber = i;
									}

								}
							}
							else
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (selectNumber == 150 || candidate[i].recoverBase < candidate[selectNumber].recoverBase)
									{
										selectNumber = i;
									}
								}
							}
						}
						else if (condition.nextCondition == RecoverCondition.AdditionalJudge.状態異常回復)
						{
							for (int i = 0; i < candidate.Count; i++)
							{
								if (selectNumber == 150 || candidate[i].cureCondition)
								{
									selectNumber = i;
									break;
								}

							}

						}
						SManager.instance.useMagic = candidate[selectNumber];
						condition.UseMagic = candidate[selectNumber];
					}
				}
			}
			else
			{
				SManager.instance.useMagic = condition.UseMagic;
			}

			SManager.instance.target = GManager.instance.Player;

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
		///　リストを削り、その中から合致する条件の敵を選び出す。
		/// </summary>
		/// <param name="targetList"></param>
		/// <param name="condition"></param>
		/// <param name="statusList"></param>
		GameObject SecondTargetJudge(List<GameObject> targetList, AttackJudge condition, List<EnemyAIBase> statusList)
		{

			//	Debug.Log($"sddf{targetCanList[0].name}");
			//	Debug.Log($"sdgs{targetList[0].name}");


			if (targetList.Count == 0 || targetList == null)
			{
				targetCanList = null;
				targetCanStatus = null;
				return null;
			}
			else if (targetList.Count >= 1)
			{
				List<int> removeNumber = new List<int>();

				if (condition.wp != AttackJudge.WeakPoint.指定なし)
				{
					///<Summary>
					///属性判断
					/// </Summary>
					#region
					if (condition.wp != AttackJudge.WeakPoint.斬撃属性)
					{
						for (int i = 0; i < targetList.Count; i++)
						{
							if (!statusList[i].status.wp.Contains(EnemyStatus.WeakPoint.Slash))
							{
								removeNumber.Add(i);
							}
						}

					}
					else if (condition.wp != AttackJudge.WeakPoint.刺突属性)
					{
						for (int i = 0; i < targetList.Count; i++)
						{
							if (!statusList[i].status.wp.Contains(EnemyStatus.WeakPoint.Stab))
							{
								removeNumber.Add(i);
							}
						}

					}
					else if (condition.wp != AttackJudge.WeakPoint.打撃属性)
					{
						for (int i = 0; i < targetList.Count; i++)
						{
							if (!statusList[i].status.wp.Contains(EnemyStatus.WeakPoint.Strike))
							{
								removeNumber.Add(i);
							}
						}
					}
					else if (condition.wp != AttackJudge.WeakPoint.打撃属性)
					{
						for (int i = 0; i < targetList.Count; i++)
						{
							if (!statusList[i].status.wp.Contains(EnemyStatus.WeakPoint.Strike))
							{
								removeNumber.Add(i);
							}
						}
					}
					else if (condition.wp != AttackJudge.WeakPoint.炎属性)
					{
						for (int i = 0; i < targetList.Count; i++)
						{
							if (!statusList[i].status.wp.Contains(EnemyStatus.WeakPoint.Fire))
							{
								removeNumber.Add(i);
							}
						}
					}
					else if (condition.wp != AttackJudge.WeakPoint.雷属性)
					{
						for (int i = 0; i < targetList.Count; i++)
						{
							if (!statusList[i].status.wp.Contains(EnemyStatus.WeakPoint.Thunder))
							{
								removeNumber.Add(i);
							}
						}
					}
					else if (condition.wp != AttackJudge.WeakPoint.聖属性)
					{
						for (int i = 0; i < targetList.Count; i++)
						{
							if (!statusList[i].status.wp.Contains(EnemyStatus.WeakPoint.Holy))
							{
								removeNumber.Add(i);
							}
						}
					}
					else if (condition.wp != AttackJudge.WeakPoint.闇属性)
					{
						for (int i = 0; i < targetList.Count; i++)
						{
							if (!statusList[i].status.wp.Contains(EnemyStatus.WeakPoint.Dark))
							{
								removeNumber.Add(i);
							}
						}
					}
					else if (condition.wp != AttackJudge.WeakPoint.毒属性)
					{
						for (int i = 0; i < targetList.Count; i++)
						{
							if (!statusList[i].status.wp.Contains(EnemyStatus.WeakPoint.Poison))
							{
								removeNumber.Add(i);
							}
						}
					}
					for (int i = 0; i < removeNumber.Count; i++)
					{
						//[removeNumber[i] - i]はこれまで配列の中から消した分を差し引いて指定してる
						//i=2なら少なくとも二個の要素が消えてるのでその分インデックスが変わる
						targetList.Remove(targetList[removeNumber[i] - i]);
						statusList.Remove(statusList[removeNumber[i] - i]);
					}
					removeNumber = null;
					#endregion

				}

			}

			if (targetList.Count == 0 || targetList == null)
			{
				
				return null;

			}

			///<summary>
			///追加条件判断
			/// </summary>
			else
			{
				int selectNumber = 150;
				if (condition.nextCondition != AttackJudge.AdditionalJudge.指定なし)
				{
					if (condition.nextCondition != AttackJudge.AdditionalJudge.敵のHP)
					{
						if (condition.upDown)
						{
							for (int i = 0; i < targetList.Count; i++)
							{
								if (selectNumber == 150 || statusList[i]._health.CurrentHealth > statusList[selectNumber]._health.CurrentHealth)
								{
									selectNumber = i;
								}
							}
						}
						else
						{
							for (int i = 0; i < targetList.Count; i++)
							{
								if (selectNumber == 150 || statusList[i]._health.CurrentHealth < statusList[selectNumber]._health.CurrentHealth)
								{
									selectNumber = i;
								}
							}
						}
					}
					else if (condition.nextCondition != AttackJudge.AdditionalJudge.敵の攻撃力)
					{
						if (condition.upDown)
						{
							for (int i = 0; i < targetList.Count; i++)
							{
								if (selectNumber == 150 || statusList[i].status.atkDisplay > statusList[selectNumber].status.atkDisplay)
								{
									selectNumber = i;
								}
							}
						}
						else
						{
							for (int i = 0; i < targetList.Count; i++)
							{
								if (selectNumber == 150 || statusList[i].status.atkDisplay < statusList[selectNumber].status.atkDisplay)
								{
									selectNumber = i;
								}
							}
						}
					}
					else if (condition.nextCondition != AttackJudge.AdditionalJudge.敵の防御力)
					{
						if (condition.upDown)
						{
							for (int i = 0; i < targetList.Count; i++)
							{
								if (selectNumber == 150 || statusList[i].status.defDisplay > statusList[selectNumber].status.defDisplay)
								{
									selectNumber = i;
								}
							}
						}
						else
						{
							for (int i = 0; i < targetList.Count; i++)
							{
								if (selectNumber == 150 || statusList[i].status.defDisplay < statusList[selectNumber].status.defDisplay)
								{
									selectNumber = i;
								}
							}
						}
					}
					else if (condition.nextCondition != AttackJudge.AdditionalJudge.敵の高度)//真なら高い
					{
						if (condition.upDown)
						{
							for (int i = 0; i < targetList.Count; i++)
							{
								if (selectNumber == 150 || targetList[i].transform.position.y > targetList[selectNumber].transform.position.y)
								{
									selectNumber = i;
								}
							}
						}
						else
						{
							for (int i = 0; i < targetList.Count; i++)
							{
								if (selectNumber == 150 || targetList[i].transform.position.y < targetList[selectNumber].transform.position.y)
								{
									selectNumber = i;
								}
							}
						}
					}
					else if (condition.nextCondition != AttackJudge.AdditionalJudge.敵の距離)
					{
						float distance = 0;
						if (condition.upDown)
						{
							//近い
							for (int i = 0; i < targetList.Count; i++)
							{
								if (selectNumber == 150 || Mathf.Abs(targetList[i].transform.position.x - this.gameObject.transform.position.x) < distance)
								{
									selectNumber = i;
									distance = Mathf.Abs(targetList[i].transform.position.x - this.gameObject.transform.position.x);
								}
							}
						}
						else
						{
							//遠い
							for (int i = 0; i < targetList.Count; i++)
							{
								if (selectNumber == 150 || Mathf.Abs(targetList[i].transform.position.x - this.gameObject.transform.position.x) > distance)
								{
									selectNumber = i;
									distance = Mathf.Abs(targetList[i].transform.position.x - this.gameObject.transform.position.x);
								}
							}
						}
					}
					targetCanList = null;
					targetCanStatus = null;

					if (SManager.instance.target != null)
					{
						targetList[selectNumber].MMGetComponentNoAlloc<EnemyAIBase>().TargetEffectCon(2);
					}
					return targetList[selectNumber];
				}
				else
				{
					if (SManager.instance.target != null)
					{
						targetList[0].MMGetComponentNoAlloc<EnemyAIBase>().TargetEffectCon(2);
					}
					targetCanList = null;
					targetCanStatus = null;
					return targetList[0];
				}

			}

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="magicList"></param>
		void secondATMagicJudge(List<SisMagic> magicList, FireCondition condition)
		{
			//	Debug.Log($"初期状態{magicList[0].name}");
			if (magicList.Count == 0)
			{
				return;
			}
			else
			{
				//	Debug.Log("確認");
				//第一条件
				if (condition.firstCondition != FireCondition.FirstCondition.指定なし)
				{
					List<int> removeNumber = new List<int>();
					if (condition.firstCondition == FireCondition.FirstCondition.敵を吹き飛ばす)
					{
						for (int i = 0; i < magicList.Count; i++)
						{
							if (!magicList[i].isBlow && !magicList[i].cBlow)
							{
								//Debug.Log("削除");
								removeNumber.Add(i);
							}
						}
					}
					//-----------------------------------------------------------------------------------------------------
					else if (condition.firstCondition == FireCondition.FirstCondition.範囲攻撃)
					{
						for (int i = 0; i < magicList.Count; i++)
						{
							if (!magicList[i].isExprode)
							{
								removeNumber.Add(i);
							}
						}
						//break;
					}
					else if (condition.firstCondition == FireCondition.FirstCondition.貫通する)
					{
						for (int i = 0; i < magicList.Count; i++)
						{
							if (!magicList[i].penetration)
							{
								removeNumber.Add(i);
							}
						}
						//break;
					}
					else if (condition.firstCondition == FireCondition.FirstCondition.追尾する)
					{
						for (int i = 0; i < magicList.Count; i++)
						{
							if (magicList[i].fireType == Magic.FIREBULLET.ANGLE || magicList[i].fireType == Magic.FIREBULLET.RAIN)
							{
								removeNumber.Add(i);
							}
						}
						//break;
					}
					else if (condition.firstCondition == FireCondition.FirstCondition.設置攻撃)
					{
						for (int i = 0; i < magicList.Count; i++)
						{
							if (magicList[i].speedV != 0)
							{
								removeNumber.Add(i);
							}
						}
						//break;
					}
					else if (condition.firstCondition == FireCondition.FirstCondition.追尾する)
					{
						for (int i = 0; i < magicList.Count; i++)
						{
							if (magicList[i].fireType == Magic.FIREBULLET.ANGLE || magicList[i].fireType == Magic.FIREBULLET.RAIN)
							{
								removeNumber.Add(i);
							}
						}
						//break;
					}
					else if (condition.firstCondition == FireCondition.FirstCondition.範囲攻撃)
					{
						for (int i = 0; i < magicList.Count; i++)
						{
							if (magicList[i].fireType == Magic.FIREBULLET.RAIN)
							{
								removeNumber.Add(i);
							}
						}
						//break;
					}
					else if (condition.firstCondition == FireCondition.FirstCondition.サーチ攻撃)
					{
						for (int i = 0; i < magicList.Count; i++)
						{
							if (!magicList[i].isChaice)
							{
								removeNumber.Add(i);
							}
						}
						//break;
					}
					for (int i = 0; i < removeNumber.Count; i++)
					{
						magicList.Remove(magicList[removeNumber[i] - i]);
						//targetCanStatus.Remove(statusList[removeNumber[i] - i]);
					}
				}
			}

			if (magicList.Count == 0)
			{
				Debug.Log("帰れ");
				return;
			}
			else
			{
				//	Debug.Log("第三段階");
				if (condition.nextCondition == FireCondition.AdditionalCondition.指定なし)
				{

					SManager.instance.useMagic = magicList[0];
					condition.UseMagic = magicList[0];
					magicCanList = null;
				}
				else
				{
					int selectNumber = 150;
					if (condition.nextCondition == FireCondition.AdditionalCondition.MP使用量)
					{
						if (condition.upDown)
						{
							for (int i = 0; i < magicList.Count; i++)
							{
								if (selectNumber == 150 || magicList[i].useMP > magicList[selectNumber].useMP)
								{
									selectNumber = i;
								}

							}
						}
						else
						{
							for (int i = 0; i < magicList.Count; i++)
							{
								if (selectNumber == 150 || magicList[i].useMP < magicList[selectNumber].useMP)
								{
									selectNumber = i;
								}
							}
						}
					}
					else if (condition.nextCondition == FireCondition.AdditionalCondition.攻撃力)
					{
						if (condition.upDown)
						{
							for (int i = 0; i < magicList.Count; i++)
							{
								if (selectNumber == 150 || magicList[i].displayAtk > magicList[selectNumber].displayAtk)
								{
									selectNumber = i;
								}

							}
						}
						else
						{
							for (int i = 0; i < magicList.Count; i++)
							{
								if (selectNumber == 150 || magicList[i].displayAtk < magicList[selectNumber].displayAtk)
								{
									selectNumber = i;
								}
							}
						}
					}
					else if (condition.nextCondition == FireCondition.AdditionalCondition.発射数)
					{
						if (condition.upDown)
						{
							for (int i = 0; i < magicList.Count; i++)
							{
								if (selectNumber == 150 || magicList[i].bulletNumber > magicList[selectNumber].bulletNumber)
								{
									selectNumber = i;
								}

							}

						}
						else
						{
							for (int i = 0; i < magicList.Count; i++)
							{
								if (selectNumber == 150 || magicList[i].bulletNumber < magicList[selectNumber].bulletNumber)
								{
									selectNumber = i;
								}
							}
						}
					}
					else if (condition.nextCondition == FireCondition.AdditionalCondition.削り値)
					{
						if (condition.upDown)
						{
							for (int i = 0; i < magicList.Count; i++)
							{
								if (selectNumber == 150 || magicList[i].shock > magicList[selectNumber].shock)
								{
									selectNumber = i;
								}

							}
						}
						else
						{
							for (int i = 0; i < magicList.Count; i++)
							{
								if (selectNumber == 150 || magicList[i].shock < magicList[selectNumber].shock)
								{
									selectNumber = i;
								}
							}
						}
					}
					else if (condition.nextCondition == FireCondition.AdditionalCondition.詠唱時間)
					{
						if (condition.upDown)
						{
							for (int i = 0; i < magicList.Count; i++)
							{
								if (selectNumber == 150 || magicList[i].castTime > magicList[selectNumber].castTime)
								{
									selectNumber = i;
								}

							}
						}
						else
						{
							for (int i = 0; i < magicList.Count; i++)
							{
								if (selectNumber == 150 || magicList[i].castTime < magicList[selectNumber].castTime)
								{
									selectNumber = i;
								}
							}
						}
					}

					SManager.instance.useMagic = magicList[selectNumber];
					condition.UseMagic = magicList[selectNumber];
					magicCanList = null;
				}
			}
		}
		/// <summary>
		/// 攻撃ステートからのステート変更
		/// </summary>
		void AttackStateChange(FireCondition condition)
		{
			if (condition.condition == FireCondition.ActJudge.回復行動に移行)
			{
				//Debug.Log("万死");
				sister.nowMove = SisterParameter.MoveType.回復;
			}
			else
			{
				sister.nowMove = SisterParameter.MoveType.支援;
			}
			stateJudge = 0.0f;
			judgeSequence = 0;
			SManager.instance.useMagic = null;
		}
		/// <summary>
		/// 支援ステートからのステート変更
		/// </summary>
		void SupportStateChange(SupportCondition.MagicJudge condition)
		{
			if (condition == SupportCondition.MagicJudge.攻撃ステートに)
			{
				//	targetJudge = sister.targetResetRes;

			//	SManager.instance.target = null;
				sister.nowMove = SisterParameter.MoveType.攻撃;
			}
			else
			{
				sister.nowMove = SisterParameter.MoveType.回復;
			}
			stateJudge = 0.0f;
			//	return;
			judgeSequence = 0;
			SManager.instance.useMagic = null;
		}
		/// <summary>
		/// 回復ステートからのステート変更
		/// </summary>
		void RecoverStateChange(RecoverCondition.MagicJudge condition)
		{
			if (condition == RecoverCondition.MagicJudge.攻撃ステートに)
			{
				//	targetJudge = sister.targetResetRes;
				if (SManager.instance.target.MMGetComponentNoAlloc<EnemyAIBase>() != null)
				{
					SManager.instance.target.MMGetComponentNoAlloc<EnemyAIBase>().TargetEffectCon(3);
				}
				//SManager.instance.target = null;
				sister.nowMove = SisterParameter.MoveType.攻撃;
			}
			else
			{
				sister.nowMove = SisterParameter.MoveType.支援;
			}
			SManager.instance.useMagic = null;
			judgeSequence = 0;
			stateJudge = 0.0f;
		}



		void FireSoundJudge()
		{
			GManager.instance.PlaySound("NormalCastEnd", transform.position);
		}

		#endregion

		async UniTaskVoid CastStop()
		{
			await UniTask.RunOnThreadPool(() => castCheck());
		}
		void castCheck()
		{

			if (_movement.CurrentState == CharacterStates.MovementStates.Cast && SManager.instance.target == null)
			{
				_movement.ChangeState(CharacterStates.MovementStates.Idle);
				_condition.ChangeState(CharacterStates.CharacterConditions.Normal);
				waitCast = 0;
				actionNum = (int)SManager.instance.useMagic.castType;
			}

		}




		async UniTaskVoid DelayInstantiate(object key, Vector3 position, Quaternion rotation, Transform parent = null, bool trackHandle = true)
		{
			delayNow = true;
			await UniTask.Delay(TimeSpan.FromSeconds(SManager.instance.useMagic.delayTime));
			await Addressables.InstantiateAsync(key, position, rotation, parent);
			delayNow = false;
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



		/// <summary>
		///  必要なアニメーターパラメーターがあれば、アニメーターパラメーターリストに追加します。
		/// </summary>
		protected override void InitializeAnimatorParameters()
		{
			RegisterAnimatorParameter(_actParameterName, AnimatorControllerParameterType.Int, out _actAnimationParameter);
			RegisterAnimatorParameter(_motionParameterName, AnimatorControllerParameterType.Int, out _motionAnimationParameter);
		}

		/// <summary>
		/// これをオーバーライドすると、キャラクターのアニメーターにパラメータを送信することができます。
		/// これは、Characterクラスによって、Early、normal、Late process()の後に、1サイクルごとに1回呼び出される。
		/// </summary>
		public override void UpdateAnimator()
		{
			//クラウチングに気をつけろよ
			//MasicUseとCastnowを組み合わせようか
			int state = 0;
			if (_movement.CurrentState == CharacterStates.MovementStates.Attack)
			{
				state = 2;
			}
			else if (_movement.CurrentState == CharacterStates.MovementStates.Cast)
			{
				state = 1;
			}

			MMAnimatorExtensions.UpdateAnimatorInteger(_animator, _actAnimationParameter, (state), _character._animatorParameters);
			MMAnimatorExtensions.UpdateAnimatorInteger(_animator, _motionAnimationParameter, (actionNum), _character._animatorParameters);
		}




	}
}