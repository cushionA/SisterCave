using Cysharp.Threading.Tasks;
using MoreMountains.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


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


		// Animation parameters
		//0がなんもなし、１が魔法、２がコンビネーション、３が詠唱
		protected const string _actParameterName = "actType";
		protected int _actAnimationParameter;

		/// <summary>
		/// モーションを選ぶ
		/// </summary>
		protected const string _motionParameterName = "motionNum";
		protected int _motionAnimationParameter;

		//条件判断や攻撃のパラメーター
		public SisterParameter sister;

		/// <summary>
		/// シスターさんの基礎AI
		/// </summary>
		[HideInInspector]
		public BrainAbility sb;

		/// <summary>
		/// クールタイム待機のため専用のフラグ
		/// </summary>
		bool disEnable;


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
		
		/// <summary>
		/// 敵検知、判断の時間計測
		/// </summary>
		float targetJudge = 30;

		/// <summary>
		/// 何個目の条件でターゲット見つけたかを確認する
		/// </summary>
		 [HideInInspector]
		public int judgeSequence;


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

		List<int> targetCanList;

		List<int> magicCanList;



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

		public AtEffectCon atEf;



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



		/// <summary>
		/// 弾丸生成中にターゲットが消えてもいいように位置を覚えておく
		/// </summary>
		Vector2 _tgPosition;

		


		/// <summary>
		///　ここで、パラメータを初期化する必要があります。
		/// </summary>
		protected override void Initialization()
		{
			base.Initialization();
			sister.nowMove = sister.priority;

			//Brainじゃないんですわ
			sb = _character.FindAbility<BrainAbility>();
			atEf = _character.FindAbility<AtEffectCon>();

			SManager.instance.MagicEffectSet(atEf);
		}


		/// <summary>
		/// 1フレームごとに、しゃがんでいるかどうか、まだしゃがんでいるべきかをチェックします
		/// </summary>
		public override void ProcessAbility()
		{
			base.ProcessAbility();


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
		async void  FireAct()
		{
			//停止命令出てるかどうか

			//Debug.Log($"チェック{MathF.Floor(waitCast)}");


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

			//限界量は超えないように
			if (sb.mp > sb.maxMp)
			{
				sb.mp = sb.maxMp;

			}

			//攻撃処理
			if (_movement.CurrentState == CharacterStates.MovementStates.Attack)
			{
			//	
				MagicUse(SManager.instance.useMagic.HRandom, SManager.instance.useMagic.VRandom);
				return;
			}

			//戦闘中、かつ位置についているなら
			else if (sb.nowState == BrainAbility.SisterState.戦い)
			{

				//何も魔法ないなら戻る
				if (sb.status.equipMagic == null)
				{
					return;
				}



				//ターゲットが消えたら詠唱やめる
				castCheck();


				
				stateJudge += _controller.DeltaTime;

				///ターゲットと使用魔法設定
				//クールタイム中でもスキップ条件があるなら動く
				#region
				
				if (_condition.CurrentState != CharacterStates.CharacterConditions.Moving)
				{
				
					//Disenableならクールタイムを待つ
					CoolTimeWait();

　　　　　　　　　　targetJudge += _controller.DeltaTime;

					//位置についていないか、クールタイム消化中でなおかつスキップコンディションもないなら
                    if ((disEnable && _skipCondition == 0) || !sb.nowPosition)
                    {
						return;
                    }

                    bool reset = false;
					//一定時間経過で戦闘思考を、行動がなしでない限りは優先する状態に戻す
					if (stateJudge >= sister.stateResetRes && sister.priority != SisterParameter.MoveType.なし)
					{
						//ステートリセット
						sister.nowMove = sister.priority;
						stateJudge = 0.0f;
						reset = true;
					}



					//時間経過かターゲット消滅かステートチェンジで再判断
					if (targetJudge >= sister.targetResetRes + 2 || SManager.instance.target == null || reset)
                    {
						targetJudge = 0;
						//一定時間ごとに検知をおこなう
						//ここで検知してから攻撃を行うまでは時間も計測しないし再び計測も行われない
						//しかし再検知するまでにクールタイムが終われば再び攻撃をする
						//その時標的が死んでいない限りはターゲットを変えない
						sb._sensor.AggresiveSerch();

						await sb._sensor.SerchAsync;

						//じゃあここでターゲット再設定やるのか
						//でも条件再判断の時間が長いとクールタイム破棄に気が付きにくくなる

						reset = true;
                    }
                       TargetReset(reset);
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
						coolTime = sister.healCT[judgeSequence];
						_skipCondition = sister.hSkipList[judgeSequence];
					}
					else
					{
						if (sister.nowMove != SisterParameter.MoveType.支援)
						{
							return;
						}
						coolTime = sister.supportCT[judgeSequence];
						_skipCondition = sister.sSkipList[judgeSequence];
					}

					if (_condition.CurrentState == CharacterStates.CharacterConditions.Normal)
					{
						//Debug.Log($"ssd{(int)SManager.instance.useMagic.castType}");
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
					//		Debug.Log("hhhhhhj");
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

					_movement.ChangeState(CharacterStates.MovementStates.Attack);
					waitCast = 0;

					actionNum = (int)SManager.instance.useMagic.FireType;
					atEf.CastEnd(SManager.instance.useMagic.magicLevel, SManager.instance.useMagic.magicElement);

					//ここからの処理ではアニメーションイベントを使う
				}
				//詠唱中なら
				else if(waitCast > 0.3 && !soundStart)
				{
				    CastCircle();
				}
			}
			//Mp足りないかターゲット消えたなら
			else
			{
				//とりあえずカメラから消す
				atEf.CastStop(SManager.instance.useMagic.magicLevel, SManager.instance.useMagic.magicElement);

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

				//逃げる
				sb.PositionJudge();

				if (waitCast >= coolTime + 0.5f)
				{
					disEnable = false;
					_skipCondition = 0;
					waitCast = 0;
					
				}
			}

		}



		/// <summary>
		/// 詠唱モーションのアニメイベント
		/// 詠唱中のために音とエフェクトをセット
		/// 使用する魔法を引数にして魔法レベルと属性で見る
		/// 詠唱の音やエフェクト変えたいならここでいじる
		public void CastCircle()
		{
			atEf.CastStart(SManager.instance.useMagic.magicLevel, SManager.instance.useMagic.magicElement);
			float dir = Mathf.Sign(SManager.instance.target.transform.position.x - transform.position.x);
			sb.SisFlip(dir);
			soundStart = true;

		}


		/// <summary>
		/// 外部から魔法中断させる
		/// </summary>
		public void MagicEnd()
		{
			
			_skipCondition = 0;
			//disEnable = false;
			stateJudge = 0;
			waitCast = 0;

			//エフェクト消す
			if (soundStart)
			{
				atEf.CastStop(SManager.instance.useMagic.magicLevel, SManager.instance.useMagic.magicElement);
			}

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
	//		Debug.Log($"なまえ{SManager.instance.useMagic.name}標的{SManager.instance.target}動作{sister.nowMove}");
			if ( delayNow)
			{
				return;
			}


            

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

				//ターゲットが消えてもいいように場所を覚えておく
				if (SManager.instance.target != null) 
				{
					_tgPosition.Set(SManager.instance.target.transform.position.x, SManager.instance.target.transform.position.y);
				}
                else
                {
					//Debug.Log($"ddd{SManager.instance.useMagic.name}標的{SManager.instance.target}動作{sister.nowMove}");

					//disEnable = true;
					bCount = 0;
					_condition.ChangeState(CharacterStates.CharacterConditions.Normal);

					actionNum = 0;
					_movement.ChangeState(CharacterStates.MovementStates.Idle);
					SManager.instance.useMagic = null;

					return;
                }

				//なんかあってもいいように一応ターゲットを保存
				SManager.instance.restoreTarget = SManager.instance.target;

				//   MyInstantiate(SManager.instance.useMagic.fireEffect, goFire, Quaternion.identity).Forget();
				//Addressables.InstantiateAsync(SManager.instance.useMagic.fireEffect, goFire, Quaternion.identity);
				if (SManager.instance.useMagic._moveSt.fireType == SisMagic.FIREBULLET.RAIN)
				{
					//山なりの弾道で打ちたいときとか射出角度決めれたらいいかも
					//位置をランダムにすれば角度はどうでもいい説もある
					SManager.instance.useAngle = GetAim(sb.firePosition.position,_tgPosition);

				}
				sb.mp -= SManager.instance.useMagic.useMP;
			}

			//敵の位置にサーチ攻撃するとき
			if (SManager.instance.useMagic.isChaice)
			{
				goFire.Set(_tgPosition.x, _tgPosition.y, 40);

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
				atEf.BulletCall(SManager.instance.useMagic.effects, goFire, Quaternion.Euler(SManager.instance.useMagic.startRotation), SManager.instance.useMagic.flashEffect);
			}
			//2発目以降の弾で生成中じゃないなら（～秒後に発生とかをやる）
			else if (bCount > 1 && !delayNow)
			{
				DelayInstantiate(SManager.instance.useMagic.effects, goFire, Quaternion.Euler(SManager.instance.useMagic.startRotation), SManager.instance.useMagic.flashEffect).Forget();
			}


			//弾丸を生成し終わったら
			if (bCount >= SManager.instance.useMagic.bulletNumber)
			{
				//Debug.Log($"ddd{SManager.instance.useMagic.name}標的{SManager.instance.target}動作{sister.nowMove}");

				//disEnable = true;
				bCount = 0;
				_condition.ChangeState(CharacterStates.CharacterConditions.Normal);
				//Debug.Log("dddw3d");
				actionNum = 0;
				_movement.ChangeState(CharacterStates.MovementStates.Idle);
				SManager.instance.useMagic = null;


				//弾丸全部出し終わったらロックカーソルを黄色に戻す
				if (SManager.instance.restoreTarget != null && SManager.instance.target != GManager.instance.Player) 
				{
					SManager.instance.restoreTarget.MMGetComponentNoAlloc<EnemyAIBase>().TargetEffectCon(3);
				}

				sb.PositionJudge();
			}
			//	bCount += 1;
		}





		///<summary>
		///判断に利用
		/// </summary>
		#region

		///<summary>
		///標的と行動を再設定する
		///リセット時は攻撃の場合ターゲット消してJudgeSequenceを変える
		///他だとシーケンスだけ入れ替える
		/// </summary>
		void TargetReset(bool _reset)
        {
			//攻撃ステートで
			if (sister.nowMove == SisterParameter.MoveType.攻撃)
			{

			//	Debug.Log($"s1{sister.targetCondition.Length}");

                if (_reset)
                {
					SManager.instance.target = null;
					judgeSequence = 0;
				}

				
				//ターゲットがいないならターゲットを探します。
				if (SManager.instance.target == null)
				{

					//五番目までやる
					//五番目までだから長さから1引いてる
					for (int i = 0; i < sister.targetCondition.Length; i++)
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
						//	Debug.Log($"{SManager.instance.target.name}");
							judgeSequence = i;
							break;
						}


					}
				//	Debug.Log($"s2");

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

				if (SManager.instance.target != null)
				{
					
					//クールタイム消化できてなくてリセットじゃないならダメ
					if(!_reset && disEnable)
                    {
						return;
                    }

					AttackAct(sister.AttackCondition[judgeSequence]);

				}


			}

			//支援の時は対象は決まってるので条件に当てはまる状況か
			//そして当てはまる支援があるかを調べる
			else if (sister.nowMove == SisterParameter.MoveType.支援)
			{
				SManager.instance.target = GManager.instance.Player;

				//リセットなら判断しなおす
				if (_reset)
				{

					for (int i = 0; i < sister.supportPlan.Length; i++)
					{
						if (disEnable && (_skipCondition & (int)Mathf.Pow(2, i)) != (int)Mathf.Pow(2, i))
						{
							continue;
						}

						//	クールタイム中ではなく最後の条件なら無条件でここは通す
						if (i == sister.supportPlan.Length - 1 && !disEnable)
						{
							SupportAct(sister.supportPlan[i]);
							judgeSequence = i;
						}
						else if (SupportJudge(sister.supportPlan[i]))
						{

							SupportAct(sister.supportPlan[i]);
							judgeSequence = i;
							break;
						}
					}
				}
                
				//リセットじゃないなら同じJudgeSequenceで判断
				else
                {
					if (SupportJudge(sister.supportPlan[judgeSequence]))
					{
						SupportAct(sister.supportPlan[judgeSequence]);

					}
				}		

			}
			//支援と同じ
			else if (sister.nowMove == SisterParameter.MoveType.回復)
			{
				SManager.instance.target = GManager.instance.Player;

                if (_reset)
                {
					for (int i = 0; i < sister.recoverCondition.Length; i++)
					{
						if (disEnable && (_skipCondition & (int)Mathf.Pow(2, i)) != (int)Mathf.Pow(2, i))
						{
							continue;
						}

						//	クールタイム中ではなく最後の条件なら無条件でここは通す
						if (i == sister.recoverCondition.Length - 1 && !disEnable)
						{
							RecoverAct(sister.recoverCondition[i]);
							judgeSequence = i;
						}
						else if (HealJudge(sister.recoverCondition[i]))
						{
							RecoverAct(sister.recoverCondition[i]);
							judgeSequence = i;

							break;
						}
					}
				}
				//リセットじゃないなら同じJudgeSequenceで判断
                else
				{
					if (HealJudge(sister.recoverCondition[judgeSequence]))
					{
						RecoverAct(sister.recoverCondition[judgeSequence]);
					}
				}
			}
		}

		/// <summary>
		/// 攻撃ステートでターゲットを決定
		/// </summary>
		/// <param name="condition"></param>
		public GameObject TargetSelect(AttackJudge condition, FireCondition act)
		{

			//最初にリストをクリア
			targetCanList.Clear();

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

					//9999は全ての意
					targetCanList.Add(9999);

					return SecondTargetJudge(condition);

				//-----------------------------------------------------------------------------------------------------
				case AttackJudge.TargetJudge.プレイヤーのHPが規定値に達した際:
					//	ランダムバリュー使ってレコードから指定
					if (condition.highOrLow)
					{
						if (sb.pc.HPRatio() >= condition.percentage / 100f)
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

							//9999は全ての意
							targetCanList.Add(9999);

							return SecondTargetJudge(condition);
						}
					}
					else
					{

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

							//9999は全ての意
							targetCanList.Add(9999);

							return SecondTargetJudge(condition);
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
							//9999は全ての意
							targetCanList.Add(9999);

							return SecondTargetJudge(condition);
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

							//9999は全ての意
							targetCanList.Add(9999);

							return SecondTargetJudge(condition);
						}
					}
					break;
				//-----------------------------------------------------------------------------------------------------
				case AttackJudge.TargetJudge.プレイヤーが状態異常にかかった時://未実装
															  //	ランダムバリュー使ってレコードから指定

					//9999は全ての意
					targetCanList.Add(9999);

					return SecondTargetJudge(condition);
					
				//-----------------------------------------------------------------------------------------------------
				case AttackJudge.TargetJudge.状態異常にかかってる敵://未実装
														 //	ランダムバリュー使ってレコードから指定

					//9999は全ての意
					targetCanList.Add(9999);
					return SecondTargetJudge(condition);
					
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

							//9999は全ての意
							targetCanList.Add(9999);
							return SecondTargetJudge(condition);
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

							//9999は全ての意
							targetCanList.Add(9999);
							return SecondTargetJudge(condition);
						}
					}
					break;
				//-----------------------------------------------------------------------------------------------------
				case AttackJudge.TargetJudge.強敵の存在:
					//強敵を優先
					
					
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
								targetCanList.Add(i);
								//break;
								
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
								targetCanList.Add(i);
								//break;
								
							}
						}
					}
					return SecondTargetJudge(condition);
					//ここに二次処理三次処理をCanListを引数に開始

				
				//-----------------------------------------------------------------------------------------------------
				case AttackJudge.TargetJudge.敵タイプ:
					//   Soldier,//陸の雑兵


					

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


						//9999は全ての意
						targetCanList.Add(9999);

						return SecondTargetJudge(condition);
						//break;
					}
					else
					{
						//int test;
						for (int i = 0; i < SManager.instance.targetList.Count; i++)
						{
							//test = ;


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
									targetCanList.Add(i);
									
									continue;
								}
							}
							//test = 0b01000000;
							if ((condition.percentage & 0b00000010) == 0b00000010)
							{
							//	Debug.Log($"数を教えてね{SManager.instance.targetCondition == null}{SManager.instance.targetCondition.Count}と{i}");
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
									targetCanList.Add(i);
									
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
									targetCanList.Add(i);
									
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
									targetCanList.Add(i);
									
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
									targetCanList.Add(i);
									
									continue;
								}
							}
						}
						return SecondTargetJudge(condition);
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
				magicCanList.Clear();
				switch (condition.condition)
				{
					case FireCondition.ActJudge.斬撃属性:

						
						for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
						{
							if (SManager.instance.attackMagi[i].phyBase > 0 && SManager.instance.attackMagi[i].magicElement == AtEffectCon.Element.slash)
							{
								magicCanList.Add(i);
								break;
							}
						}
						secondATMagicJudge( condition);
						break;
					//-----------------------------------------------------------------------------------------------------
					case FireCondition.ActJudge.刺突属性:
						
						for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
						{
							if (SManager.instance.attackMagi[i].phyBase > 0 && SManager.instance.attackMagi[i].magicElement == AtEffectCon.Element.stab)
							{
								magicCanList.Add(i);
								break;
							}
						}
						secondATMagicJudge( condition);
						break;
					//-----------------------------------------------------------------------------------------------------
					case FireCondition.ActJudge.打撃属性:
						
						for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
						{
							if (SManager.instance.attackMagi[i].phyBase > 0 && SManager.instance.attackMagi[i].magicElement == AtEffectCon.Element.strike)
							{
								magicCanList.Add(i);
								break;
							}
						}
						secondATMagicJudge( condition);
						break;
					//-----------------------------------------------------------------------------------------------------
					case FireCondition.ActJudge.聖属性:
						
						for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
						{
							if (SManager.instance.attackMagi[i].holyBase > 0)
							{
								magicCanList.Add(i);
								break;
							}
						}
						secondATMagicJudge( condition);
						break;
					//-----------------------------------------------------------------------------------------------------
					case FireCondition.ActJudge.闇属性:
						
						for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
						{
							if (SManager.instance.attackMagi[i].darkBase > 0)
							{
								magicCanList.Add(i);
								break;
							}
						}
						secondATMagicJudge( condition);
						break;
					//-----------------------------------------------------------------------------------------------------
					case FireCondition.ActJudge.炎属性:

						
						for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
						{
							if (SManager.instance.attackMagi[i].fireBase > 0)
							{
								magicCanList.Add(i);
								break;
							}
						}
						//Debug.Log($"asgd{magicCanList[0].name}");
						secondATMagicJudge( condition);
						break;
					//-----------------------------------------------------------------------------------------------------
					case FireCondition.ActJudge.雷属性:
						//Debug.Log($"ssssss");
						
						for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
						{
							if (SManager.instance.attackMagi[i].thunderBase > 0)
							{
								//		Debug.Log($"第二段階{SManager.instance.attackMagi[i].name}");
								magicCanList.Add(i);
								break;
							}
						}
						secondATMagicJudge( condition);
						break;
					//-----------------------------------------------------------------------------------------------------
					case FireCondition.ActJudge.毒属性:
						for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
						{
							if (SManager.instance.attackMagi[i].thunderBase >= 0)
							{
								magicCanList.Add(i);
								break;
							}
						}
						secondATMagicJudge( condition);
						break;
					//-----------------------------------------------------------------------------------------------------
					case FireCondition.ActJudge.属性指定なし:

						magicCanList.Add(9999);

						secondATMagicJudge( condition);
						break;
					//-----------------------------------------------------------------------------------------------------
					case FireCondition.ActJudge.移動速度低下攻撃://未実装
						for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
						{
							if (SManager.instance.attackMagi[i].thunderBase >= 0)
							{
								magicCanList.Add(i);
								break;
							}
						}
						secondATMagicJudge( condition);
						break;
					//-----------------------------------------------------------------------------------------------------
					case FireCondition.ActJudge.攻撃力低下攻撃://未実装
						for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
						{
							if (SManager.instance.attackMagi[i].thunderBase >= 0)
							{
								magicCanList.Add(i);
								break;
							}
						}
						secondATMagicJudge( condition);
						break;
					//-----------------------------------------------------------------------------------------------------
					case FireCondition.ActJudge.防御力低下攻撃://未実装
						for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
						{
							if (SManager.instance.attackMagi[i].thunderBase >= 0)
							{
								magicCanList.Add(i);
								break;
							}
						}
						secondATMagicJudge( condition);
						break;
				}
			}
			else
			{

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
							magicCanList.Add(i);
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
			if (sister.nowMove != SisterParameter.MoveType.支援 || !SManager.instance.supportMagi.Any() || condition.ActBase == SupportCondition.MagicJudge.なにもしない)
			{
				return;
			}

			

			if (condition.UseMagic == null)
			{
				magicCanList.Clear();

				if (condition.ActBase == SupportCondition.MagicJudge.回復ステートに)
				{
					SupportStateChange(condition.ActBase);
				}
				else if (condition.ActBase == SupportCondition.MagicJudge.攻撃ステートに)
				{
					SupportStateChange(condition.ActBase);

				}
				else if (condition.ActBase == SupportCondition.MagicJudge.各種支援魔法)
				{

					for (int i = 0; i < SManager.instance.supportMagi.Count; i++)
					{
						//使用したい支援のタイプにそぐうなら
						if (SManager.instance.supportMagi[i].sType == condition.useSupport)
						{
							magicCanList.Add(i);
						}
					}
				}


				if (SManager.instance.supportMagi.Count == 0)
				{
					return;
				}
				else
				{
					if (condition.nextCondition == SupportCondition.AdditionalJudge.指定なし)
					{
						SManager.instance.useMagic = SManager.instance.supportMagi[magicCanList[0]];
						condition.UseMagic = SManager.instance.supportMagi[magicCanList[0]];
						magicCanList.Clear();
					}
					else
					{
						int selectNumber = 150;
						if (condition.nextCondition == SupportCondition.AdditionalJudge.MP使用量)
						{
							if (condition.upDown)
							{
								for (int i = 0; i < SManager.instance.supportMagi.Count; i++)
								{
									if (selectNumber == 150 || SManager.instance.supportMagi[magicCanList[i]].useMP > SManager.instance.supportMagi[selectNumber].useMP)
									{
										selectNumber = magicCanList[i];
									}

								}
							}
							else
							{
								for (int i = 0; i < SManager.instance.supportMagi.Count; i++)
								{
									if (selectNumber == 150 ||  SManager.instance.supportMagi[magicCanList[i]].useMP < SManager.instance.supportMagi[selectNumber].useMP)
									{
										selectNumber = magicCanList[i];
									}
								}
							}
						}
						else if (condition.nextCondition == SupportCondition.AdditionalJudge.詠唱時間)
						{
							if (condition.upDown)
							{
								for (int i = 0; i < SManager.instance.supportMagi.Count; i++)
								{
									if (selectNumber == 150 ||  SManager.instance.supportMagi[magicCanList[i]].castTime > SManager.instance.supportMagi[selectNumber].castTime)
									{
										selectNumber = magicCanList[i];
									}

								}
							}
							else
							{
								for (int i = 0; i < SManager.instance.supportMagi.Count; i++)
								{
									if (selectNumber == 150 ||  SManager.instance.supportMagi[magicCanList[i]].castTime < SManager.instance.supportMagi[selectNumber].castTime)
									{
										selectNumber = magicCanList[i];
									}
								}
							}
						}
						else if (condition.nextCondition == SupportCondition.AdditionalJudge.持続効果時間)
						{
							if (condition.upDown)
							{
								for (int i = 0; i < SManager.instance.supportMagi.Count; i++)
								{
									if (selectNumber == 150 ||  SManager.instance.supportMagi[magicCanList[i]].effectTime > SManager.instance.supportMagi[selectNumber].effectTime)
									{
										selectNumber = magicCanList[i];
									}

								}
							}
							else
							{
								for (int i = 0; i < SManager.instance.supportMagi.Count; i++)
								{
									if (selectNumber == 150 ||  SManager.instance.supportMagi[magicCanList[i]].effectTime < SManager.instance.supportMagi[selectNumber].effectTime)
									{
										selectNumber = magicCanList[i];
									}
								}
							}
						}
						else if (condition.nextCondition == SupportCondition.AdditionalJudge.強化倍率)
						{
							if (condition.upDown)
							{
								for (int i = 0; i < SManager.instance.supportMagi.Count; i++)
								{
									if (selectNumber == 150 ||  SManager.instance.supportMagi[magicCanList[i]].mValue > SManager.instance.supportMagi[selectNumber].mValue)
									{
										selectNumber = magicCanList[i];
									}

								}
							}
							else
							{
								for (int i = 0; i < SManager.instance.supportMagi.Count; i++)
								{
									if (selectNumber == 150 ||  SManager.instance.supportMagi[magicCanList[i]].mValue < SManager.instance.supportMagi[selectNumber].mValue)
									{
										selectNumber = magicCanList[i];
									}
								}
							}
						}
						SManager.instance.useMagic = SManager.instance.supportMagi[selectNumber];

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


			if (sister.nowMove != SisterParameter.MoveType.回復 && sb.nowState == BrainAbility.SisterState.戦い || condition.ActBase == RecoverCondition.MagicJudge.なにもしない)
			{
				//	judgeSequence = 0;
				return;
			}
			//-----------------------------------------------------------------------------------------------------
			else if (condition.ActBase == RecoverCondition.MagicJudge.支援ステートに)
			{
				RecoverStateChange(condition.ActBase);
				return;
			}
			else if (condition.ActBase == RecoverCondition.MagicJudge.攻撃ステートに)
			{
				RecoverStateChange(condition.ActBase);
				return;
			}



			if (condition.UseMagic == null)
			{
				magicCanList.Clear();


				if (SManager.instance.recoverMagi.Count == 0)
				{
					
					SManager.instance.useMagic = null;
					return;
				}

				if (condition.useSupport != SisMagic.SupportType.なし)
				{
					for (int i = 0; i < SManager.instance.recoverMagi.Count; i++)
					{
						//指定する特殊効果があるなら
						if (SManager.instance.recoverMagi[i].sType == condition.useSupport)
						{
							magicCanList.Add(i);
							//Debug.Log($"調べます{SManager.instance.recoverMagi[i].name}");
						}
					}
				}


				if(!magicCanList.Any())
				{
					if (condition.nextCondition == RecoverCondition.AdditionalJudge.指定なし)
					{
						SManager.instance.useMagic = SManager.instance.recoverMagi[magicCanList[0]];

						condition.UseMagic = SManager.instance.recoverMagi[magicCanList[0]];
						magicCanList.Clear();

					}
					else
					{
						int selectNumber = 150;
						if (condition.nextCondition == RecoverCondition.AdditionalJudge.MP使用量)
						{
							if (condition.upDown)
							{
								for (int i = 0; i < SManager.instance.recoverMagi.Count; i++)
								{
									if (selectNumber == 150 || SManager.instance.recoverMagi[magicCanList[magicCanList[i]]].useMP > SManager.instance.recoverMagi[selectNumber].useMP)
									{
										selectNumber = magicCanList[i];
									}

								}
							}
							else
							{
								for (int i = 0; i < SManager.instance.recoverMagi.Count; i++)
								{
									if (selectNumber == 150 || SManager.instance.recoverMagi[magicCanList[i]].useMP < SManager.instance.recoverMagi[selectNumber].useMP)
									{
										selectNumber = magicCanList[i];
									}
								}
							}
						}
						else if (condition.nextCondition == RecoverCondition.AdditionalJudge.詠唱時間)
						{
							if (condition.upDown)
							{
								for (int i = 0; i < SManager.instance.recoverMagi.Count; i++)
								{
									if (selectNumber == 150 || SManager.instance.recoverMagi[magicCanList[magicCanList[i]]].castTime > SManager.instance.recoverMagi[selectNumber].castTime)
									{
										selectNumber = magicCanList[i];
									}

								}
							}
							else
							{
								for (int i = 0; i < SManager.instance.recoverMagi.Count; i++)
								{
									if (selectNumber == 150 || SManager.instance.recoverMagi[magicCanList[i]].castTime < SManager.instance.recoverMagi[selectNumber].castTime)
									{
										selectNumber = magicCanList[i];
									}
								}
							}
						}
						else if (condition.nextCondition == RecoverCondition.AdditionalJudge.リジェネ回復量)
						{
							if (condition.upDown)
							{
								for (int i = 0; i < SManager.instance.recoverMagi.Count; i++)
								{
									if (selectNumber == 150 || SManager.instance.recoverMagi[magicCanList[i]].regeneAmount > SManager.instance.recoverMagi[selectNumber].regeneAmount)
									{
										selectNumber = magicCanList[i];
									}

								}
							}
							else
							{
								for (int i = 0; i < SManager.instance.recoverMagi.Count; i++)
								{
									if (selectNumber == 150 || SManager.instance.recoverMagi[magicCanList[i]].regeneAmount < SManager.instance.recoverMagi[selectNumber].regeneAmount)
									{
										selectNumber = magicCanList[i];
									}
								}
							}
						}
						else if (condition.nextCondition == RecoverCondition.AdditionalJudge.リジェネ総回復量)
						{
							if (condition.upDown)
							{
								for (int i = 0; i < SManager.instance.recoverMagi.Count; i++)
								{
									if (selectNumber == 150 || SManager.instance.recoverMagi[magicCanList[i]].regeneAmount * SManager.instance.recoverMagi[magicCanList[i]].effectTime > SManager.instance.recoverMagi[selectNumber].regeneAmount * SManager.instance.recoverMagi[selectNumber].effectTime)
									{
										selectNumber = magicCanList[i];
									}

								}
							}
							else
							{
								for (int i = 0; i < SManager.instance.recoverMagi.Count; i++)
								{
									if (selectNumber == 150 || SManager.instance.recoverMagi[magicCanList[i]].regeneAmount * SManager.instance.recoverMagi[magicCanList[i]].effectTime < SManager.instance.recoverMagi[selectNumber].regeneAmount * SManager.instance.recoverMagi[selectNumber].effectTime)
									{
										selectNumber = magicCanList[i];
									}
								}
							}
						}
						else if (condition.nextCondition == RecoverCondition.AdditionalJudge.持続効果時間)
						{
							if (condition.upDown)
							{
								for (int i = 0; i < SManager.instance.recoverMagi.Count; i++)
								{
									if (selectNumber == 150 || SManager.instance.recoverMagi[magicCanList[i]].effectTime > SManager.instance.recoverMagi[selectNumber].effectTime)
									{
										selectNumber = magicCanList[i];
									}

								}
							}
							else
							{
								for (int i = 0; i < SManager.instance.recoverMagi.Count; i++)
								{
									if (selectNumber == 150 || SManager.instance.recoverMagi[magicCanList[i]].effectTime < SManager.instance.recoverMagi[selectNumber].effectTime)
									{
										selectNumber = magicCanList[i];
									}
								}
							}
						}
						else if (condition.nextCondition == RecoverCondition.AdditionalJudge.回復量)
						{
							if (condition.upDown)
							{
								for (int i = 0; i < SManager.instance.recoverMagi.Count; i++)
								{
									if (selectNumber == 150 || SManager.instance.recoverMagi[magicCanList[i]].recoverBase > SManager.instance.recoverMagi[selectNumber].recoverBase)
									{
										selectNumber = magicCanList[i];
									}

								}
							}
							else
							{
								for (int i = 0; i < SManager.instance.recoverMagi.Count; i++)
								{
									if (selectNumber == 150 || SManager.instance.recoverMagi[magicCanList[i]].recoverBase < SManager.instance.recoverMagi[selectNumber].recoverBase)
									{
										selectNumber = magicCanList[i];
									}
								}
							}
						}
						else if (condition.nextCondition == RecoverCondition.AdditionalJudge.状態異常回復)
						{
							for (int i = 0; i < SManager.instance.recoverMagi.Count; i++)
							{
								if (selectNumber == 150 || SManager.instance.recoverMagi[magicCanList[i]].cureCondition)
								{
									selectNumber = magicCanList[i];
									break;
								}

							}

						}
						SManager.instance.useMagic = SManager.instance.recoverMagi[selectNumber];
						condition.UseMagic = SManager.instance.recoverMagi[selectNumber];
					}
				}
			}
			else
			{
				SManager.instance.useMagic = condition.UseMagic;
				magicCanList.Clear();
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
		/// <param name="SManager.instance.targetCondition"></param>
		GameObject SecondTargetJudge(AttackJudge condition)
		{

			//	Debug.Log($"sddf{targetCanList[0].name}");
			//	Debug.Log($"sdgs{targetList[0].name}");


			if (targetCanList.Count == 0 || targetCanList == null)
			{
				return null;
			}
			else if (targetCanList.Count >= 1)
			{

				if (condition.wp != AttackJudge.WeakPoint.指定なし)
				{
					///<Summary>
					///属性判断
					/// </Summary>
					#region

					if ((int)condition.wp < 4)
					{
						if (condition.wp != AttackJudge.WeakPoint.斬撃属性)
						{
							for (int i = 0; i < targetCanList.Count; i++)
							{
								if (!SManager.instance.targetCondition[targetCanList[i]].status.wp.Contains(EnemyStatus.WeakPoint.Slash))
								{
									targetCanList.Remove(targetCanList[i]);
								}
							}

						}
						else if (condition.wp != AttackJudge.WeakPoint.刺突属性)
						{
							for (int i = 0; i < targetCanList.Count; i++)
							{
								if (!SManager.instance.targetCondition[targetCanList[i]].status.wp.Contains(EnemyStatus.WeakPoint.Stab))
								{
									targetCanList.Remove(targetCanList[i]);
								}
							}

						}
						else if (condition.wp != AttackJudge.WeakPoint.打撃属性)
						{
							for (int i = 0; i < targetCanList.Count; i++)
							{
								if (!SManager.instance.targetCondition[targetCanList[i]].status.wp.Contains(EnemyStatus.WeakPoint.Strike))
								{
									targetCanList.Remove(targetCanList[i]);
								}
							}
						}
					}

					else
					{
						if (condition.wp != AttackJudge.WeakPoint.炎属性)
						{
							for (int i = 0; i < targetCanList.Count; i++)
							{
								if (!SManager.instance.targetCondition[targetCanList[i]].status.wp.Contains(EnemyStatus.WeakPoint.Fire))
								{
									targetCanList.Remove(targetCanList[i]);
								}
							}
						}
						else if (condition.wp != AttackJudge.WeakPoint.雷属性)
						{
							for (int i = 0; i < targetCanList.Count; i++)
							{
								if (!SManager.instance.targetCondition[targetCanList[i]].status.wp.Contains(EnemyStatus.WeakPoint.Thunder))
								{
									targetCanList.Remove(targetCanList[i]);
								}
							}
						}
						else if (condition.wp != AttackJudge.WeakPoint.聖属性)
						{
							for (int i = 0; i < targetCanList.Count; i++)
							{
								if (!SManager.instance.targetCondition[targetCanList[i]].status.wp.Contains(EnemyStatus.WeakPoint.Holy))
								{
									targetCanList.Remove(targetCanList[i]);
								}
							}
						}
						else if (condition.wp != AttackJudge.WeakPoint.闇属性)
						{
							for (int i = 0; i < targetCanList.Count; i++)
							{
								if (!SManager.instance.targetCondition[targetCanList[i]].status.wp.Contains(EnemyStatus.WeakPoint.Dark))
								{
									targetCanList.Remove(targetCanList[i]);
								}
							}
						}
					}


					#endregion

				}

			}

			if (targetCanList.Count == 0 || targetCanList == null)
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
							for (int i = 0; i < targetCanList.Count; i++)
							{
								if (selectNumber == 150 || SManager.instance.targetCondition[targetCanList[i]]._health.CurrentHealth > SManager.instance.targetCondition[selectNumber]._health.CurrentHealth)
								{
									selectNumber = targetCanList[i];
								}
							}
						}
						else
						{
							for (int i = 0; i < targetCanList.Count; i++)
							{
								if (selectNumber == 150 || SManager.instance.targetCondition[targetCanList[i]]._health.CurrentHealth < SManager.instance.targetCondition[selectNumber]._health.CurrentHealth)
								{
									selectNumber = targetCanList[i];
								}
							}
						}
					}
					else if (condition.nextCondition != AttackJudge.AdditionalJudge.敵の攻撃力)
					{
						if (condition.upDown)
						{
							for (int i = 0; i < targetCanList.Count; i++)
							{
								if (selectNumber == 150 || SManager.instance.targetCondition[targetCanList[i]].status.atkDisplay > SManager.instance.targetCondition[selectNumber].status.atkDisplay)
								{
									selectNumber = targetCanList[i];
								}
							}
						}
						else
						{
							for (int i = 0; i < targetCanList.Count; i++)
							{
								if (selectNumber == 150 || SManager.instance.targetCondition[targetCanList[i]].status.atkDisplay < SManager.instance.targetCondition[selectNumber].status.atkDisplay)
								{
									selectNumber = targetCanList[i];
								}
							}
						}
					}
					else if (condition.nextCondition != AttackJudge.AdditionalJudge.敵の防御力)
					{
						if (condition.upDown)
						{
							for (int i = 0; i < targetCanList.Count; i++)
							{
								if (selectNumber == 150 || SManager.instance.targetCondition[targetCanList[i]].status.defDisplay > SManager.instance.targetCondition[selectNumber].status.defDisplay)
								{
									selectNumber = targetCanList[i];
								}
							}
						}
						else
						{
							for (int i = 0; i < targetCanList.Count; i++)
							{
								if (selectNumber == 150 || SManager.instance.targetCondition[targetCanList[i]].status.defDisplay < SManager.instance.targetCondition[selectNumber].status.defDisplay)
								{
									selectNumber = targetCanList[i];
								}
							}
						}
					}
					else if (condition.nextCondition != AttackJudge.AdditionalJudge.敵の高度)//真なら高い
					{
						if (condition.upDown)
						{
							for (int i = 0; i < targetCanList.Count; i++)
							{
								if (selectNumber == 150 || SManager.instance.targetList[targetCanList[i]].transform.position.y > SManager.instance.targetList[selectNumber].transform.position.y)
								{
									selectNumber = targetCanList[i];
								}
							}
						}
						else
						{
							for (int i = 0; i < targetCanList.Count; i++)
							{
								if (selectNumber == 150 || SManager.instance.targetList[targetCanList[i]].transform.position.y < SManager.instance.targetList[selectNumber].transform.position.y)
								{
									selectNumber = targetCanList[i];
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
							for (int i = 0; i < targetCanList.Count; i++)
							{
								if (selectNumber == 150 || Mathf.Abs( SManager.instance.targetList[targetCanList[i]].transform.position.x - this.gameObject.transform.position.x) < distance)
								{
									selectNumber = targetCanList[i];
									distance = Mathf.Abs(SManager.instance.targetList[targetCanList[i]].transform.position.x - this.gameObject.transform.position.x);
								}
							}
						}
						else
						{
							//遠い
							for (int i = 0; i < targetCanList.Count; i++)
							{
								if (selectNumber == 150 || Mathf.Abs(SManager.instance.targetList[targetCanList[i]].transform.position.x - this.gameObject.transform.position.x) > distance)
								{
									selectNumber = targetCanList[i];
									distance = Mathf.Abs(SManager.instance.targetList[targetCanList[i]].transform.position.x - this.gameObject.transform.position.x);
								}
							}
						}
					}

					if (SManager.instance.target != null)
					{
						SManager.instance.targetCondition[selectNumber].TargetEffectCon(2);
					}
					return SManager.instance.targetList[selectNumber];
				}
				else
				{
					if (SManager.instance.target != null)
					{
						SManager.instance.targetCondition[0].TargetEffectCon(2);
					}
					return SManager.instance.targetList[0];
				}

			}

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="magicList"></param>
		void secondATMagicJudge(FireCondition condition)
		{
			//	Debug.Log($"初期状態{magicList[0].name}");
			if (magicCanList.Count == 0)
			{
				return;
			}
			else
			{
				//	Debug.Log("確認");
				//第一条件
				if (condition.firstCondition != FireCondition.FirstCondition.指定なし)
				{

					if (condition.firstCondition == FireCondition.FirstCondition.敵を吹き飛ばす)
					{
						for (int i = 0; i < magicCanList.Count; i++)
						{
							if (! SManager.instance.attackMagi[magicCanList[i]].isBlow && !SManager.instance.attackMagi[magicCanList[i]].cBlow)
							{
								//Debug.Log("削除");
								magicCanList.Remove(magicCanList[i]);
							}
						}
					}
					//-----------------------------------------------------------------------------------------------------
					else if (condition.firstCondition == FireCondition.FirstCondition.範囲攻撃)
					{
						for (int i = 0; i < magicCanList.Count; i++)
						{
							if (!SManager.instance.attackMagi[magicCanList[i]].isExprode)
							{
								magicCanList.Remove(magicCanList[i]);
							}
						}
						//break;
					}
					else if (condition.firstCondition == FireCondition.FirstCondition.貫通する)
					{
						for (int i = 0; i < magicCanList.Count; i++)
						{
							if (!SManager.instance.attackMagi[magicCanList[i]].penetration)
							{
								magicCanList.Remove(magicCanList[i]);
							}
						}
						//break;
					}
					else if (condition.firstCondition == FireCondition.FirstCondition.追尾する)
					{
						for (int i = 0; i < magicCanList.Count; i++)
						{
							if (SManager.instance.attackMagi[magicCanList[i]]._moveSt.fireType == Magic.FIREBULLET.ANGLE || SManager.instance.attackMagi[magicCanList[i]]._moveSt.fireType == Magic.FIREBULLET.RAIN)
							{
								magicCanList.Remove(magicCanList[i]);
							}
						}
						//break;
					}
					else if (condition.firstCondition == FireCondition.FirstCondition.設置攻撃)
					{
						for (int i = 0; i < magicCanList.Count; i++)
						{
							if (SManager.instance.attackMagi[magicCanList[i]]._moveSt.speedV != 0)
							{
								magicCanList.Remove(magicCanList[i]);
							}
						}
						//break;
					}
					else if (condition.firstCondition == FireCondition.FirstCondition.追尾する)
					{
						for (int i = 0; i < magicCanList.Count; i++)
						{
							if (SManager.instance.attackMagi[magicCanList[i]]._moveSt.fireType == Magic.FIREBULLET.ANGLE || SManager.instance.attackMagi[magicCanList[i]]._moveSt.fireType == Magic.FIREBULLET.RAIN)
							{
								magicCanList.Remove(magicCanList[i]);
							}
						}
						//break;
					}
					else if (condition.firstCondition == FireCondition.FirstCondition.範囲攻撃)
					{
						for (int i = 0; i < magicCanList.Count; i++)
						{
							if (SManager.instance.attackMagi[magicCanList[i]]._moveSt.fireType == Magic.FIREBULLET.RAIN)
							{
								magicCanList.Remove(magicCanList[i]);
							}
						}
						//break;
					}
					else if (condition.firstCondition == FireCondition.FirstCondition.サーチ攻撃)
					{
						for (int i = 0; i < magicCanList.Count; i++)
						{
							if (!SManager.instance.attackMagi[magicCanList[i]].isChaice)
							{
								magicCanList.Remove(magicCanList[i]);
							}
						}
						//break;
					}
				}
			}

			if (magicCanList.Count == 0)
			{
				
				return;
			}
			else
			{
				//	Debug.Log("第三段階");
				if (condition.nextCondition == FireCondition.AdditionalCondition.指定なし)
				{

					SManager.instance.useMagic = SManager.instance.attackMagi[0];
					condition.UseMagic = SManager.instance.attackMagi[0];
					magicCanList.Clear();
				}
				else
				{
					int selectNumber = 150;
					if (condition.nextCondition == FireCondition.AdditionalCondition.MP使用量)
					{
						if (condition.upDown)
						{
							for (int i = 0; i < magicCanList.Count; i++)
							{
								if (selectNumber == 150 || SManager.instance.attackMagi[magicCanList[i]].useMP > SManager.instance.attackMagi[selectNumber].useMP)
								{
									selectNumber = magicCanList[i];
								}

							}
						}
						else
						{
							for (int i = 0; i < magicCanList.Count; i++)
							{
								if (selectNumber == 150 || SManager.instance.attackMagi[magicCanList[i]].useMP < SManager.instance.attackMagi[selectNumber].useMP)
								{
									selectNumber = magicCanList[i];
								}
							}
						}
					}
					else if (condition.nextCondition == FireCondition.AdditionalCondition.攻撃力)
					{
						if (condition.upDown)
						{
							for (int i = 0; i < magicCanList.Count; i++)
							{
								if (selectNumber == 150 || SManager.instance.attackMagi[magicCanList[i]].displayAtk > SManager.instance.attackMagi[selectNumber].displayAtk)
								{
									selectNumber = magicCanList[i];
								}

							}
						}
						else
						{
							for (int i = 0; i < magicCanList.Count; i++)
							{
								if (selectNumber == 150 || SManager.instance.attackMagi[magicCanList[i]].displayAtk < SManager.instance.attackMagi[selectNumber].displayAtk)
								{
									selectNumber = magicCanList[i];
								}
							}
						}
					}
					else if (condition.nextCondition == FireCondition.AdditionalCondition.発射数)
					{
						if (condition.upDown)
						{
							for (int i = 0; i < magicCanList.Count; i++)
							{
								if (selectNumber == 150 || SManager.instance.attackMagi[magicCanList[i]].bulletNumber > SManager.instance.attackMagi[selectNumber].bulletNumber)
								{
									selectNumber = magicCanList[i];
								}

							}

						}
						else
						{
							for (int i = 0; i < magicCanList.Count; i++)
							{
								if (selectNumber == 150 || SManager.instance.attackMagi[magicCanList[i]].bulletNumber < SManager.instance.attackMagi[selectNumber].bulletNumber)
								{
									selectNumber = magicCanList[i];
								}
							}
						}
					}
					else if (condition.nextCondition == FireCondition.AdditionalCondition.削り値)
					{
						if (condition.upDown)
						{
							for (int i = 0; i < magicCanList.Count; i++)
							{
								if (selectNumber == 150 || SManager.instance.attackMagi[magicCanList[i]].shock > SManager.instance.attackMagi[selectNumber].shock)
								{
									selectNumber = magicCanList[i];
								}

							}
						}
						else
						{
							for (int i = 0; i < magicCanList.Count; i++)
							{
								if (selectNumber == 150 || SManager.instance.attackMagi[magicCanList[i]].shock < SManager.instance.attackMagi[selectNumber].shock)
								{
									selectNumber = magicCanList[i];
								}
							}
						}
					}
					else if (condition.nextCondition == FireCondition.AdditionalCondition.詠唱時間)
					{
						if (condition.upDown)
						{
							for (int i = 0; i < magicCanList.Count; i++)
							{
								if (selectNumber == 150 || SManager.instance.attackMagi[magicCanList[i]].castTime > SManager.instance.attackMagi[selectNumber].castTime)
								{
									selectNumber = magicCanList[i];
								}

							}
						}
						else
						{
							for (int i = 0; i < magicCanList.Count; i++)
							{
								if (selectNumber == 150 || SManager.instance.attackMagi[magicCanList[i]].castTime < SManager.instance.attackMagi[selectNumber].castTime)
								{
									selectNumber = magicCanList[i];
								}
							}
						}
					}

					SManager.instance.useMagic = SManager.instance.attackMagi[selectNumber];
					condition.UseMagic = SManager.instance.attackMagi[selectNumber];
					magicCanList.Clear();
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
				sister.nowMove = SisterParameter.MoveType.回復;
			}
			else
			{
				sister.nowMove = SisterParameter.MoveType.支援;
			}
			stateJudge = 0.0f;
			targetJudge = 1000;
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
			targetJudge = 1000;
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
			targetJudge = 1000;
			judgeSequence = 0;
			stateJudge = 0.0f;
		}





		#endregion


		/// <summary>
		/// 敵がいないとき全てリセット
		/// </summary>
		void castCheck()
		{

			if (_movement.CurrentState == CharacterStates.MovementStates.Cast && SManager.instance.target == null)
			{
				_movement.ChangeState(CharacterStates.MovementStates.Idle);
				_condition.ChangeState(CharacterStates.CharacterConditions.Normal);
				waitCast = 0;

				atEf.CastStop(SManager.instance.useMagic.magicLevel, SManager.instance.useMagic.magicElement);

				actionNum = 0;

				waitCast = 0;
			}

		}




		async UniTaskVoid DelayInstantiate(ParticleSystem key, Vector3 position, Quaternion rotation,ParticleSystem flash)
		{
			delayNow = true;
			await UniTask.Delay(TimeSpan.FromSeconds(SManager.instance.useMagic.delayTime));
			atEf.BulletCall(key, position, rotation, flash);
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

		///<summary>
		///  Brainとの連携を行う
		/// </summary>
        #region
		
		///<sumary>
		/// 状態ごとの初期化項目
		/// </sumary>
		public void StateInitialize(bool battle)
        {
            if (battle)
            {
				judgeSequence = 0;
				targetJudge = 1000;
				stateJudge = 1000;

            }
            else
            {
				//これは道中回復開始とそのサウンドを終わらせてる

				if (soundStart)
				{
					atEf.CastStop(SManager.instance.useMagic.magicLevel, SManager.instance.useMagic.magicElement);
					soundStart = false;

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
		}

        #endregion

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