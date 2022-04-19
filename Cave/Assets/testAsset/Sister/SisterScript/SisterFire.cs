using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.UI;

/// <summary>
/// 設計思想
/// 条件とその条件の時どんな魔法を使うかで決まる
/// 攻撃のターゲットは条件１での審査から条件５の審査まで行う
/// しかし自由度低いな。ほぼ思い通りにできないしさすがにストレス

/// 考えてる仕様としてはさらに詠唱中エフェクトをつける
/// それだけか
/// 位置はシスターさんのルート下のEffectの位置で決める
/// 判断の時決定した魔法のMPが足りない選択肢はスルー
/// 魔法装備変えた時点で登録しといてよくね、条件ごとに使う魔法は
/// 毎回決め直さずもう決定しておく
/// 使う魔法決まったら共通の発動モジュールで実行
/// あしたエフェクトとアニメイベント試す
/// 魔法に発射エフェクトなるものをつけてもいいかも
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
/// </summary>

public class SisterFire : MonoBehaviour
{

	public SisterParameter sister;
	public SisterStatus status;
	//public GameObject firePosition;
	//	SisterFireBullet sisF;
	SisterBrain sb;
	bool disEnable;
	//List<GameObject> targetPlan;
	//GameObject target;
	List<SisMagic> useSupport;//未使用の支援
	List<float> effectiveTime;//支援魔法、リジェネ、攻撃の時間をはかる

	float waitCast;//詠唱時間待ち
	float coolTime;
	float stateJudge = 30;
	//float targetJudge = 30;

	int judgeSequence;
	bool useMagic;
	
	/// <summary>
	/// 詠唱中つかう音
	/// </summary>
	string castSound;

	/// <summary>
	/// 詠唱の音鳴らすサイン
	/// </summary>
	byte soundStart = 0;

	int bCount;
	float recoverTime;

	List<GameObject> targetCanList;
	List<EnemyBase> targetCanStatus;
	List<SisMagic> magicCanList;

	byte targetType = 0;
	/// <summary>
	/// 連携が今何段目か
	/// </summary>
	int conboChain = 0;

	[SerializeField] CombinationAct ca;
	/// <summary>
	/// 連携対象設定で使うためのダミー
	/// </summary>
	[SerializeField] FireCondition dammy;

	bool combiEnable = true;
	
	float combinationCool;

	//攻撃ステートから初期化終わったか
	bool isReset;
	//回復時間
	float healJudge;

	//弾丸生成を待っているか
	bool delayNow;

	[SerializeField]
	Text cCounter;

	[SerializeField]
	Transform cIcon;

	[SerializeField]
	Slider cSlider;

	//最初完了音鳴らさない
	bool first;

	// Start is called before the first frame update
	void Start()
	{
		sister.nowMove = sister.priority;
		sb = GetComponent<SisterBrain>();
	}

    private void Update()
    {
		if (!combiEnable && ((!SManager.instance.actNow) || (SManager.instance.actNow && SManager.instance.castNow)))
		{
			//	Debug.Log($"野菜売り場");
			//連携のクールタイム計測
			combinationCool += Time.deltaTime;

		 	cSlider.value = combinationCool / status.equipCombination.coolTime;

			if (combinationCool >= status.equipCombination.coolTime)
			{
				combinationCool = 0;
				conboChain = 0;
				combiEnable = true;
				cIcon.gameObject.SetActive(true);
				if (!first)
				{
					first = true;
				}
				else
				{
					GManager.instance.PlaySound("CombiCharge", GManager.instance.Player.transform.position);
				}
			}
		}
		else if (combiEnable && conboChain > 0)
		{
			//
			//チェイン中でかつ行動してないとき

			combinationCool += Time.deltaTime;
			if (combinationCool >= 3.5f)
			{
				//Debug.Log($"お菓子売り場{conboChain}");
				combinationCool = 0;
				conboChain = 0;
				combiEnable = false;
				
			}
		}

        if (!combiEnable && conboChain == 0)
        {
			cCounter.text = (Mathf.Round(status.equipCombination.coolTime - combinationCool)).ToString();
		}



		if (status.equipCombination != null && !GManager.instance.isDown && !GManager.instance.onGimmick)
		{
			if (status.equipCombination != null && GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction19))
			{
				//条件の意図としてはコンビネーションが設定されてて地面にいてクールタイム中じゃなくて魔法発動中じゃない
				//コンボ可能フラグはコンボ中は不可になってる。チェイン数見て最後に改めて戻される。
				if (sb.isGround && combiEnable && !useMagic) //&& !(SManager.instance.actNow && !SManager.instance.castNow))
				{
//Debug.Log($"お肉売り場{conboChain}");
					//コンボ中にコンボ受付時間切れを待つ時間計測のタイマーを再入力するたびにリセット
					combinationCool = 0;
					//SManager.instance.castNow = false;
					combiEnable = false;
					SManager.instance.actNow = true;
					SManager.instance.castNow = false;
					Combination();
					cCounter.text = ((int)status.equipCombination.coolTime).ToString();
					if (cIcon.gameObject.activeSelf)
					{
						cIcon.gameObject.SetActive(false);
					}
					cSlider.value = 0;
				}
				else if (!combiEnable  && combinationCool >= 1f && status.equipCombination.useMP <= status.mp)
				{
			//	Debug.Log($"お肉売り場{conboChain}");
					combinationCool = 0;
					conboChain = 0;
					//combiEnable = true;
					SManager.instance.actNow = true;
					SManager.instance.castNow = false;
					SManager.instance.sisStatus.mp -= status.equipCombination.useMP;
					Combination();
					cCounter.text = ((int)status.equipCombination.coolTime).ToString();
					if (cIcon.gameObject.activeSelf)
					{
						cIcon.gameObject.SetActive(false);
					}
					cSlider.value = 0;
				}
			}
		}

    }

    // Update is called once per frame
    void FixedUpdate()
	{
        //	Debug.Log($"ギン{targetJudge}");
        //	Debug.Log($"状態開示{sister.nowMove}");
        //	Debug.Log($"判断基準開示{targetType}");

        //Debug.lo
		/*
        if (!combiEnable && ((!SManager.instance.actNow) || (SManager.instance.actNow && SManager.instance.castNow)))
        {
		//	Debug.Log($"野菜売り場");
			//連携のクールタイム計測
			combinationCool += Time.fixedDeltaTime;

			if(combinationCool >= status.equipCombination.coolTime)
            {
				combinationCool = 0;
				conboChain = 0;
				combiEnable = true;
            }
        }
        else if(combiEnable && conboChain > 0)
        {
			//
			//チェイン中でかつ行動してないとき

				combinationCool += Time.fixedDeltaTime;
				if (combinationCool >= 3.5f)
				{
				//Debug.Log($"お菓子売り場{conboChain}");
					combinationCool = 0;
					conboChain = 0;
					combiEnable = false;
				}
        }*/
		if (useMagic)
		{

			MagicUse(SManager.instance.useMagic.HRandom, SManager.instance.useMagic.VRandom);

		}
		//
		//	Debug.Log($"確認{RandomValue(0,100)}");
		if (sb.nowState == SisterBrain.SisterState.戦い && sb.nowPosition)
		{
			if (isReset)
			{
				if (soundStart == 2)
				{
					soundStart = 0;
					GManager.instance.StopSound(castSound, 0.5f);
					FireSoundJudge();
				}
			//	targetJudge = sister.targetResetRes;
				//Debug.Log("終戦");
				disEnable = false;
				SManager.instance.actNow = false;
				useMagic = false;
				stateJudge = 0;
				waitCast = 0;
				coolTime = 0;
			//	SManager.instance.target.GetComponent<EnemyBase>().TargetEffectCon(3);
				SManager.instance.target = null;
				SManager.instance.castNow = false;
				isReset = false;
			}
			//	 ここまで確認
			//魔法使用中

			else
			{
				if (disEnable && !SManager.instance.actNow)
				{

					CoolTimeWait();
				}
				CastStop().Forget();

				if (status.equipMagic == null)
				{
					return;
				}

				if (SManager.instance.targetList == null)
				{

				}

					//	targetJudge += Time.fixedDeltaTime;
					stateJudge += Time.fixedDeltaTime;
				if (!SManager.instance.actNow && !disEnable && !SManager.instance.castNow)
				{
					

					if (stateJudge >= sister.stateResetRes)
					{
						//ステートリセット
						sister.nowMove = sister.priority;
						stateJudge = 0.0f;
					}



					if (sister.nowMove == SisterParameter.MoveType.攻撃)
					{
						//SManager.instance.target.GetComponent<EnemyBase>().TargetEffectCon(3);
						//SManager.instance.target = null;


						if (/*targetJudge >= sister.targetResetRes ||*/ SManager.instance.target == null)
						{
							
							judgeSequence = 0;
							//SManager.instance.GetEnemyCondition();
							//SManager.instance.target = null;
					//		if (SManager.instance.target == null)
					//		{
								judgeSequence = 1;
							//	if (sister.firstAttack.condition != FireCondition.ActJudge.なにもしない)
							//	{
                     //               Debug.Log("ペンギン");
									TargetSelect(sister.firstTarget, sister.firstAttack);

								//}
						//	}
							if (SManager.instance.target == null)
							{
								//if (sister.secondAttack.condition != FireCondition.ActJudge.なにもしない)
							//	{
									//Debug.Log("ダメなこと");
									TargetSelect(sister.secondTarget, sister.secondAttack);
									judgeSequence = 2;
								//}
							}
							if (SManager.instance.target == null)
							{
						//		if (sister.thirdAttack.condition != FireCondition.ActJudge.なにもしない)
						//		{
									TargetSelect(sister.thirdTarget,sister.thirdAttack);
									judgeSequence = 3;
							//	}
							}
							if (SManager.instance.target == null)
							{
						//		if (sister.fourthAttack.condition != FireCondition.ActJudge.なにもしない)
							//	{
									TargetSelect(sister.forthTarget,sister.fourthAttack);
									judgeSequence = 4;
							//	}
							}
							if (SManager.instance.target == null)
							{
						//		if (sister.fiveAttack.condition != FireCondition.ActJudge.なにもしない)
							//	{
									TargetSelect(sister.fiveTarget,sister.fiveAttack);
									judgeSequence = 5;
							//	}
							}
							if (SManager.instance.target == null)
							{
								if (sister.nonAttack.condition == FireCondition.ActJudge.回復行動に移行 || sister.nonAttack.condition == FireCondition.ActJudge.支援行動に移行)
								{
									AttackStateChange(sister.nonAttack);
									return;
								}
								else if (sister.nonAttack.condition != FireCondition.ActJudge.なにもしない)
								{
									SManager.instance.target = SManager.instance.targetList[RandomValue(0, SManager.instance.targetList.Count - 1)];
									judgeSequence = 6;
								}
							}
							
							//EnemyRecordとtargetConditionは一致してる。
							//敵情報更新
						}
						if (SManager.instance.target != null)
						{
							//Debug.Log("使用魔法設定ができてないのが問題");

							if(sister.nowMove != SisterParameter.MoveType.攻撃)
                            {
								judgeSequence = 0;
								return;
								

							}
							if (judgeSequence == 1)
							{

								AttackAct(sister.firstAttack);
								//Debug.Log("第一段階");
							}
							else if (judgeSequence == 2)
							{
								AttackAct(sister.secondAttack);
							//	Debug.Log($"あじゃばー");
							}
							else if (judgeSequence == 3)
							{
								AttackAct(sister.thirdAttack);
								
							}
							else if (judgeSequence == 4)
							{
								AttackAct(sister.fourthAttack);
							}
							else if (judgeSequence == 5)
							{
								AttackAct(sister.nonAttack);
							}
							else if (judgeSequence == 6)
							{
								AttackAct(sister.nonAttack);
							}

						}

						//クールタイム実装頼んだ
						//ActionFire();

					}

					else if (sister.nowMove == SisterParameter.MoveType.支援)
					{
						SManager.instance.target = GManager.instance.Player;
						//じょうけんみたしてる
						if (SupportJudge(sister.firstPlan))
						{
							SupportAct(sister.firstPlan);
						}
						else if (SupportJudge(sister.secondPlan))
						{
							SupportAct(sister.secondPlan);
						}
						else if (SupportJudge(sister.thirdPlan))
						{
							SupportAct(sister.thirdPlan);
						}
						else if (SupportJudge(sister.forthPlan))
						{
							SupportAct(sister.forthPlan);
						}
						else if (SupportJudge(sister.fivePlan))
						{
							SupportAct(sister.fivePlan);
						}
						else
						{
							SupportAct(sister.sixPlan);
						}
					}
					else if (sister.nowMove == SisterParameter.MoveType.回復)
					{
						//SManager.instance.target = GManager.instance.Player;
						if (HealJudge(sister.firstRecover))
						{
							
							RecoverAct(sister.firstRecover);
						}
						else if (HealJudge(sister.secondRecover))
						{
							RecoverAct(sister.secondRecover);
						}
						else if (HealJudge(sister.thirdRecover))
						{
							RecoverAct(sister.thirdRecover);
						}
						else if (HealJudge(sister.forthRecover))
						{
							RecoverAct(sister.forthRecover);
						}
						else if (HealJudge(sister.fiveRecover))
						{
							RecoverAct(sister.fiveRecover);
						}
						else
						{
							RecoverAct(sister.nonRecover);
						}
					}

				}

				//ちゃんとターゲットと使用魔法が設定されていれば
				if (SManager.instance.target != null && SManager.instance.useMagic != null)
				{
				//	bool isWrong = false;
					if (SManager.instance.useMagic.mType == SisMagic.MagicType.Attack)
					{
						if (sister.nowMove != SisterParameter.MoveType.攻撃 && targetType != 1)
						{
							Debug.Log("チンクル");
							//	isWrong = true;
							return;
						}

					}
					else if (SManager.instance.useMagic.mType == SisMagic.MagicType.Recover)
					{
						if (sister.nowMove != SisterParameter.MoveType.回復 && targetType != 2)
						{
							return;
							//isWrong = true;
						}

					}
					else
					{
						if (sister.nowMove != SisterParameter.MoveType.支援)
						{
							return;
						//	isWrong = true;
						}
					}
				//	if (!isWrong) 
			//		{
					ActionFire();
					//SManager.instance.target = null;
				//    }
                  //  else
                 //   {
					//	SManager.instance.target = null;
					//	SManager.instance.useMagic = null;
				//		SManager.instance.actNow = false;
				//	}
				}

			}
		}

		else if (sb.nowState != SisterBrain.SisterState.戦い)
		{
			if (!isReset)
			{
				if (soundStart == 2)
				{
					soundStart = 0;
					GManager.instance.StopSound(castSound, 0.5f);
					FireSoundJudge();
				}
		//		targetJudge = sister.targetResetRes;
				//Debug.Log("終戦");
				disEnable = false;
				SManager.instance.actNow = false;
				useMagic = false;
				stateJudge = 0;
				waitCast = 0;
				coolTime = 0;
				SManager.instance.target = null;
				SManager.instance.castNow = false;
				isReset = true;
				//healJudge = 5f;
			}
            else
            {
                if (sister.autoHeal)
                {
					//bool healAct = false;
					

					if (!SManager.instance.actNow && !SManager.instance.castNow)
					{
                       healJudge += Time.fixedDeltaTime;
						if (healJudge >= 3f)
						{
							//	healAct = true;
							//SManager.instance.target = GManager.instance.Player;
							//SManager.instance.useMagic = null;
							//SManager.instance.target = GManager.instance.Player;
							if (HealJudge(sister.nFirstRecover))
							{

								RecoverAct(sister.nFirstRecover);
								//Debug.Log($"1{SManager.instance.useMagic}");
							}
							else if (HealJudge(sister.nSecondRecover))
							{
								RecoverAct(sister.nSecondRecover);
								//Debug.Log("2");
							}
							else if (HealJudge(sister.nThirdRecover))
							{
								RecoverAct(sister.nThirdRecover);
								//Debug.Log("3");
							}
							else
							{
								SManager.instance.useMagic = null;
								//Debug.Log("4");
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

						//Debug.Log("げせぬ");
						ActionFire();
					}
					}
            }
		}
		//行動してないとき魔力回復
		if (!SManager.instance.actNow)
		{
			recoverTime += Time.fixedDeltaTime;
			if (recoverTime >= 2.5 && SManager.instance.sisStatus.mp < SManager.instance.sisStatus.maxMp)
			{
				float recoverAmo = disEnable ? SManager.instance.sisStatus.mpRecover * 1.2f : SManager.instance.sisStatus.mpRecover;
				
				SManager.instance.sisStatus.mp += (recoverAmo + SManager.instance.sisStatus.additionalRecover);
				recoverTime = 0;
			}
		}

		if (SManager.instance.sisStatus.mp >= SManager.instance.sisStatus.maxMp)
		{
			SManager.instance.sisStatus.mp = SManager.instance.sisStatus.maxMp;

		}

	}


	public void ActionFire(float random = 0.0f)
	{//ランダムに入れてもいいけど普通に入れてもいい

		if (status.mp >= SManager.instance.useMagic.useMP && SManager.instance.target != null)
		{

		//	Debug.Log($"つかう魔法{SManager.instance.useMagic.name}");
		//	Debug.Log($"判断番号{judgeSequence}");
			SManager.instance.actNow = true;
			waitCast += Time.fixedDeltaTime;
			if (waitCast >= SManager.instance.useMagic.castTime && SManager.instance.target != null)
			{

				//sb.//(SManager.instance.useMagic.FireAnime);
				//useMagic = true;
				disEnable = true;
				SManager.instance.castNow = false;
			//	soundStart = false;
				GManager.instance.StopSound(castSound, 0.5f);
			}
			else
			{
			//	Debug.Log("詠唱中");
				SManager.instance.castNow = true;
				//sb.//(SManager.instance.useMagic.castAnime);
                if (soundStart == 1)
                {
					GManager.instance.PlaySound(castSound,transform.position);
					soundStart = 2;
                }
			}
		}
		else
		{
		//	Debug.Log("詠唱中止");
			GManager.instance.StopSound(castSound, 0.5f);
			SManager.instance.castNow = false;
			SManager.instance.actNow = false;
			waitCast = 0;
			//disEnable = true;
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
	/// 攻撃ステートでターゲットを決定
	/// </summary>
	/// <param name="condition"></param>
	public void TargetSelect(AttackJudge condition,FireCondition act)
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
					return;
				}
                else if(act.condition == FireCondition.ActJudge.なにもしない)
                {
					return;
                }
				targetCanList = new List<GameObject>(SManager.instance.targetList);
				targetCanStatus = new List<EnemyBase>(SManager.instance.targetCondition);
				SecondTargetJudge(targetCanList, condition, targetCanStatus);

				break;
			//-----------------------------------------------------------------------------------------------------
			case AttackJudge.TargetJudge.プレイヤーのHPが規定値に達した際:
				//	ランダムバリュー使ってレコードから指定
				if (condition.highOrLow)
				{
					if (GManager.instance.hp / GManager.instance.maxHp >= condition.percentage / 100f)
					{
						if (act.condition == FireCondition.ActJudge.回復行動に移行 || act.condition == FireCondition.ActJudge.支援行動に移行)
						{

							AttackStateChange(act);
							return;
						}
						else if (act.condition == FireCondition.ActJudge.なにもしない)
						{
							return;
						}
						targetCanList = new List<GameObject>(SManager.instance.targetList);
						targetCanStatus = new List<EnemyBase>(SManager.instance.targetCondition);
						SecondTargetJudge(targetCanList, condition, targetCanStatus);
					}
				}
				else
				{
			//		Debug.Log($"あああ{GManager.instance.hp / GManager.instance.maxHp}と{condition.percentage / 100}");
					//Debug.Log($"あああ{condition.percentage / 100f}");
					if (GManager.instance.hp / GManager.instance.maxHp <= condition.percentage / 100f)
					{

						if (act.condition == FireCondition.ActJudge.回復行動に移行 || act.condition == FireCondition.ActJudge.支援行動に移行)
						{
							//kuroko++;
							//Debug.Log($"朝顔{sister.nowMove}");

							AttackStateChange(act);
						return;
					//		testes = true;
						}
						else if (act.condition == FireCondition.ActJudge.なにもしない)
						{
							return;
						}

						targetCanList = new List<GameObject>(SManager.instance.targetList);
						targetCanStatus = new List<EnemyBase>(SManager.instance.targetCondition);
						SecondTargetJudge(targetCanList, condition, targetCanStatus);
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
							return;
						}
						else if (act.condition == FireCondition.ActJudge.なにもしない)
						{
							return;
						}
						targetCanList = new List<GameObject>(SManager.instance.targetList);
						targetCanStatus = new List<EnemyBase>(SManager.instance.targetCondition);
						SecondTargetJudge(targetCanList, condition, targetCanStatus);
					}
				}
				else
				{
					if (GManager.instance.mp / GManager.instance.maxMp <= condition.percentage / 100f)
					{

						if (act.condition == FireCondition.ActJudge.回復行動に移行 || act.condition == FireCondition.ActJudge.支援行動に移行)
						{

							AttackStateChange(act);
							return;
						}
						else if (act.condition == FireCondition.ActJudge.なにもしない)
						{
							return;
						}

						targetCanList = new List<GameObject>(SManager.instance.targetList);
						targetCanStatus = new List<EnemyBase>(SManager.instance.targetCondition);
						SecondTargetJudge(targetCanList, condition, targetCanStatus);
					}
				}
				break;
			//-----------------------------------------------------------------------------------------------------
			case AttackJudge.TargetJudge.プレイヤーが状態異常にかかった時://未実装
														  //	ランダムバリュー使ってレコードから指定
				targetCanList = new List<GameObject>(SManager.instance.targetList);
				targetCanStatus = new List<EnemyBase>(SManager.instance.targetCondition);
				SecondTargetJudge(targetCanList, condition, targetCanStatus);
				break;
			//-----------------------------------------------------------------------------------------------------
			case AttackJudge.TargetJudge.状態異常にかかってる敵://未実装
													 //	ランダムバリュー使ってレコードから指定
				targetCanList = new List<GameObject>(SManager.instance.targetList);
				targetCanStatus = new List<EnemyBase>(SManager.instance.targetCondition);
				SecondTargetJudge(targetCanList, condition, targetCanStatus);
				break;
			//-----------------------------------------------------------------------------------------------------
			case AttackJudge.TargetJudge.自分のMPが規定値に達した際:
				if (condition.highOrLow)
				{
					if (SManager.instance.sisStatus.mp / SManager.instance.sisStatus.maxMp >= condition.percentage / 100f)
					{
						if (act.condition == FireCondition.ActJudge.回復行動に移行 || act.condition == FireCondition.ActJudge.支援行動に移行)
						{

							AttackStateChange(act);
							return;
						}
						else if (act.condition == FireCondition.ActJudge.なにもしない)
						{
							return;
						}
						targetCanList = new List<GameObject>(SManager.instance.targetList);
						targetCanStatus = new List<EnemyBase>(SManager.instance.targetCondition);
						SecondTargetJudge(targetCanList, condition, targetCanStatus);
					}
				}
				else
				{

					if (SManager.instance.sisStatus.mp / SManager.instance.sisStatus.maxMp <= condition.percentage / 100f)
					{
						if (act.condition == FireCondition.ActJudge.回復行動に移行 || act.condition == FireCondition.ActJudge.支援行動に移行)
						{

							AttackStateChange(act);
							return;
						}
						else if (act.condition == FireCondition.ActJudge.なにもしない)
						{
							return;
						}
						targetCanList = new List<GameObject>(SManager.instance.targetList);
						targetCanStatus = new List<EnemyBase>(SManager.instance.targetCondition);
						SecondTargetJudge(targetCanList, condition, targetCanStatus);
					}
				}
				break;
			//-----------------------------------------------------------------------------------------------------
			case AttackJudge.TargetJudge.強敵の存在:
				//強敵を優先
				targetCanList = new List<GameObject>();
				targetCanStatus = new List<EnemyBase>();
				if (condition.highOrLow)
				{
					for (int i = 0; i < SManager.instance.targetList.Count; i++)
					{
						if (SManager.instance.targetCondition[i].status.strong)
						{
							if (act.condition == FireCondition.ActJudge.回復行動に移行 || act.condition == FireCondition.ActJudge.支援行動に移行)
							{

								AttackStateChange(act);
								return;
							}
							else if (act.condition == FireCondition.ActJudge.なにもしない)
							{
								return;
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
								return;
							}
							else if (act.condition == FireCondition.ActJudge.なにもしない)
							{
								return;
							}
							targetCanList.Add(SManager.instance.targetList[i]);
							//break;
							targetCanStatus.Add(SManager.instance.targetCondition[i]);
						}
					}
				}
				SecondTargetJudge(targetCanList, condition, targetCanStatus);
				//ここに二次処理三次処理をCanListを引数に開始

				break;
			//-----------------------------------------------------------------------------------------------------
			case AttackJudge.TargetJudge.敵タイプ:
				//   Soldier,//陸の雑兵

				targetCanList = new List<GameObject>();
				targetCanStatus = new List<EnemyBase>();

				//選ぶ敵タイプがすべてと選択されてるなら
				if (condition.percentage == 0b00011111)
				{
					
					if (act.condition == FireCondition.ActJudge.回復行動に移行 || act.condition == FireCondition.ActJudge.支援行動に移行)
					{

						AttackStateChange(act);
						return;
					}
					else if (act.condition == FireCondition.ActJudge.なにもしない)
					{
						return;
					}
					targetCanList = new List<GameObject>(SManager.instance.targetList);
					targetCanStatus = new List<EnemyBase>(SManager.instance.targetCondition);
					
					
					SecondTargetJudge(targetCanList, condition, targetCanStatus);
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
									return;
								}
								else if (act.condition == FireCondition.ActJudge.なにもしない)
								{
									return;
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
									return;
								}
								else if (act.condition == FireCondition.ActJudge.なにもしない)
								{
									return;
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
									return;
								}
								else if (act.condition == FireCondition.ActJudge.なにもしない)
								{
									return;
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
									return;
								}
								else if (act.condition == FireCondition.ActJudge.なにもしない)
								{
									return;
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
									return;
								}
								else if (act.condition == FireCondition.ActJudge.なにもしない)
								{
									return;
								}
								targetCanList.Add(SManager.instance.targetList[i]);
								targetCanStatus.Add(SManager.instance.targetCondition[i]);
								continue;
							}
						}
					}
					SecondTargetJudge(targetCanList, condition, targetCanStatus);
				}
				
				break;
		　　//-----------------------------------------------------------------------------------------------------
		}

	}

	/// <summary>
	/// 攻撃ステートで使用魔法を決定
	/// </summary>
	/// <param name="condition"></param>
	public void AttackAct(FireCondition condition)
	{
		//	Debug.Log($"重要状態開示{sister.nowMove }");
		//Debug.Log($"asgd{condition.condition}");
		if (condition.UseMagic == null) {
			//Debug.Log("ghdks");
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
		//	Debug.Log($"おかしなとき{condition.UseMagic.name}");
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
					return GManager.instance.hp / GManager.instance.maxHp >= condition.percentage / 100f ? true : false;
				}
                else
                {
					return GManager.instance.hp / GManager.instance.maxHp <= condition.percentage / 100f ? true : false;
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
					return SManager.instance.sisStatus.mp / SManager.instance.sisStatus.maxMp >= condition.percentage / 100f ? true : false;
				}
				else
				{
					return SManager.instance.sisStatus.mp / SManager.instance.sisStatus.maxMp <= condition.percentage / 100f ? true : false;
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
				return GManager.instance.hp / GManager.instance.maxHp >= condition.percentage / 100f ? true : false;
			}
			else
			{
				return GManager.instance.hp / GManager.instance.maxHp <= condition.percentage / 100f ? true : false;
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
				return SManager.instance.sisStatus.mp / SManager.instance.sisStatus.maxMp >= condition.percentage / 100f ? true : false;
			}
			else
			{
				return SManager.instance.sisStatus.mp / SManager.instance.sisStatus.maxMp <= condition.percentage / 100f ? true : false;
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


		if (sister.nowMove != SisterParameter.MoveType.回復 && sb.nowState == SisterBrain.SisterState.戦い)
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
				//Debug.Log($"焼肉{SManager.instance.useMagic.name}");
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
					Debug.Log($"君の名は{SManager.instance.useMagic.name}");
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
		targetType = 2;
	}

	/// <summary>
	///魔力回復などのために行動ごとにクールタイム設定可能 
	/// </summary>
	void CoolTimeWait()
	{

		waitCast += Time.fixedDeltaTime;
		//sb.//("Stand");
		if (waitCast >= coolTime + 0.5f)
		{
			disEnable = false;
			waitCast = 0;
			//SManager.instance.target.GetComponent<EnemyBase>().TargetEffectCon(3);
			SManager.instance.target = null;
		}

	}

	/// <summary>
	/// 攻撃時アニメイベント
	/// 詠唱完了時に一度呼ぶ
	/// 魔法レベルと属性で音判断
	/// </summary>
	public void EventFire()
	{
		useMagic = true;
		//待てこれキャストサウンド介する必要ある？
		//普通に判断して普通に入れよ
		FireSoundJudge();
	}

	/// <summary>
	/// for文ではないが	bcountを超えるまでuseMagicが真なので発動し続ける
	/// </summary>
	/// <param name="hRandom"></param>
	/// <param name="vRandom"></param>
	void MagicUse(int hRandom, int vRandom)
	{
		//	Debug.Log($"ハザード{SManager.instance.useMagic.name}標的{SManager.instance.target}動作{sister.nowMove}");
		if (!delayNow)
		{
			bCount += 1;
		}
		Vector3 goFire = sb.firePosition.position;
		if (bCount == 1)
		{
          //   MyInstantiate(SManager.instance.useMagic.fireEffect, goFire, Quaternion.identity).Forget();
			//Addressables.InstantiateAsync(SManager.instance.useMagic.fireEffect, goFire, Quaternion.identity);
			if (SManager.instance.useMagic.fireType == SisMagic.FIREBULLET.RAIN)
			{
				//山なりの弾道で打ちたいときとか射出角度決めれたらいいかも
				//位置をランダムにすれば角度はどうでもいい説もある
				SManager.instance.useMagic.angle = GetAim(sb.firePosition.position, SManager.instance.target.transform.position);

			}
			SManager.instance.sisStatus.mp -= SManager.instance.useMagic.useMP;
		}


		if (SManager.instance.useMagic.isChaice)
		{
			goFire.Set(SManager.instance.target.transform.position.x, SManager.instance.target.transform.position.y, SManager.instance.target.transform.position.y);

		}
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
		if (SManager.instance.useMagic.delayTime == 0 || bCount == 1)
		{
			Addressables.InstantiateAsync(SManager.instance.useMagic.effects, goFire, Quaternion.Euler(SManager.instance.useMagic.startRotation));
		}
        else if(bCount > 1 && !delayNow)
        {
			DelayInstantiate(SManager.instance.useMagic.effects, goFire, Quaternion.Euler(SManager.instance.useMagic.startRotation)).Forget();
        }
		if (bCount >= SManager.instance.useMagic.bulletNumber)
		{
			//Debug.Log($"テンペスト{SManager.instance.useMagic.name}標的{SManager.instance.target}動作{sister.nowMove}");
			useMagic = false;
			//disEnable = true;
			waitCast = 0;
			coolTime = SManager.instance.useMagic.coolTime;
			bCount = 0;
			SManager.instance.actNow = false;
			//SManager.instance.target = null;
			SManager.instance.useMagic = null;
			SManager.instance.target.GetComponent<EnemyBase>().TargetEffectCon(3);
		}
		//	bCount += 1;
	}


	/// <summary>
	/// アニメイベント
	/// 詠唱中のために音とエフェクトをセット
	/// 使用する魔法を引数にして魔法レベルと属性で見る
	public void CastEffect()
	{
		//	Transform goFire = firePosition.transform;

		//Transform goFire = firePosition;

		////(SManager.instance.sisStatus.attackName[attackNumber]);

		//	for (int x = 0; x >= SManager.instance.useMagic.bulletNumber; x++)
		//	{

		Transform gofire = sb.firePosition;

        //Transform rotate = SManager.instance.useMagic.castEffect.LoadAssetAsync<Transform>().Result as Transform;

      //  MyInstantiate(SManager.instance.useMagic.castEffect, gofire.position, gofire.rotation).Forget();//.Result;//発生位置をPlayer
		Addressables.InstantiateAsync(SManager.instance.useMagic.castEffect, gofire.position, gofire.rotation);
		//	}

		castSound = "normalCast";
		soundStart = 1;
		sb.ATFlip();
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
	void SecondTargetJudge(List<GameObject> targetList, AttackJudge condition, List<EnemyBase> statusList)
	{

	//	Debug.Log($"sddf{targetCanList[0].name}");
	//	Debug.Log($"sdgs{targetList[0].name}");


		if (targetList.Count == 0 || targetList == null)
		{
			return;
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
					targetCanList.Remove(targetList[removeNumber[i] - i]);
					targetCanStatus.Remove(statusList[removeNumber[i] - i]);
				}
				removeNumber = null;
				#endregion

			}

		}

		if (targetList.Count == 0)
		{
			targetCanList = null;
			targetCanStatus = null;
			return;

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
							if (selectNumber == 150 || statusList[i].hp > statusList[selectNumber].hp)
							{
								selectNumber = i;
							}
						}
					}
					else
					{
						for (int i = 0; i < targetList.Count; i++)
						{
							if (selectNumber == 150 || statusList[i].hp < statusList[selectNumber].hp)
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
				
				SManager.instance.target = targetList[selectNumber];
                SManager.instance.target.GetComponent<EnemyBase>().TargetEffectCon(2);
			}
            else
            {
				
				SManager.instance.target = targetList[0];
				SManager.instance.target.GetComponent<EnemyBase>().TargetEffectCon(2);
			}
			targetCanList = null;
		　　targetCanStatus = null;
		}
		targetType = 1;
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
		if(condition.condition == FireCondition.ActJudge.回復行動に移行)
        {
			//Debug.Log("万死");
			sister.nowMove = SisterParameter.MoveType.回復;
		}
        else
        {
			sister.nowMove = SisterParameter.MoveType.支援;
		}
		stateJudge = 0.0f;
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

			SManager.instance.target = null;
			sister.nowMove = SisterParameter.MoveType.攻撃;
		}
		else
		{
			sister.nowMove = SisterParameter.MoveType.回復;
		}
		stateJudge = 0.0f;
		//	return;
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
			SManager.instance.target.GetComponent<EnemyBase>().TargetEffectCon(3);
			SManager.instance.target = null;
			sister.nowMove = SisterParameter.MoveType.攻撃;
		}
		else
		{
			sister.nowMove = SisterParameter.MoveType.支援;
		}
		SManager.instance.useMagic = null;
		stateJudge = 0.0f;
	}

/*	async  UniTaskVoid MyInstantiate(object key,Vector3 firePosi,Quaternion rotate)
    {
		await Addressables.InstantiateAsync(key, firePosi, rotate);
		try 
		{
			var result = await Addressables.InstantiateAsync(key,firePosi,rotate);

		}
        catch
        {

        }
	}*/

	void FireSoundJudge()
    {
		GManager.instance.PlaySound("NormalCastEnd", transform.position);
	}
	async UniTaskVoid CastStop()
    {
		await UniTask.RunOnThreadPool(() => castCheck());
    }
	void castCheck()
    {
		if(SManager.instance.castNow && SManager.instance.target == null)
        {
			SManager.instance.castNow = false;
			SManager.instance.actNow = false;

		}

    }

	 void Combination()
    {

        if (status.equipCombination.isTargeting)
        {
			if (SManager.instance.target != null)
			{
				SManager.instance.target.GetComponent<EnemyBase>().TargetEffectCon(3);
				SManager.instance.target = null;
			}
		/*	for (int i = 0; i >= status.equipCombination.chainNumber || SManager.instance.target != null; i++)
			{

            }*/



			TargetSelect(status.equipCombination.mainTarget[conboChain], dammy);

			if (SManager.instance.target == null)
			{
				TargetSelect(status.equipCombination.subTarget[conboChain], dammy);
			}
			if(SManager.instance.target == null)
            {
				SManager.instance.target = this.gameObject;
			}
		}
	   ca.CombinationDo(status.equipCombination,conboChain);
		if (SManager.instance.target != null || !status.equipCombination.isTargeting)
		{
			conboChain++;
			//Debug.Log($"お肉売り場{conboChain}");
			if (conboChain >= status.equipCombination.chainNumber)
			{
				conboChain = 0;
				//combiEnable = false;
			}
			else
			{
				combiEnable = true;
			}

			SManager.instance.target = null;
			SManager.instance.actNow = false;
			SManager.instance.target = null;
		}
	}


	 async UniTaskVoid DelayInstantiate(object key, Vector3 position, Quaternion rotation, Transform parent = null, bool trackHandle = true)
    {
		delayNow = true;
		await UniTask.Delay(TimeSpan.FromSeconds(SManager.instance.useMagic.delayTime));
		await Addressables.InstantiateAsync(key,position,rotation,parent);
		delayNow = false;
	}


	public void AttackEnd()
    {
		useMagic = false;
		//disEnable = true;
		waitCast = 0;
	//	coolTime = SManager.instance.useMagic.coolTime;
		bCount = 0;
		SManager.instance.actNow = false;
		SManager.instance.castNow = false;
		SManager.instance.target = null;
		SManager.instance.useMagic = null;
		//SManager.instance.target.GetComponent<EnemyBase>().TargetEffectCon(3);
	}


}







