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
			if (status.equipCombination != null && GManager.instance.InputR.GetButtonDown(MainUICon.instance.rewiredAction19))
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
				else if (!combiEnable  && combinationCool >= 1f && status.equipCombination.useMP <= status.maxMp)
				{
			//	Debug.Log($"お肉売り場{conboChain}");
					combinationCool = 0;
					conboChain = 0;
					//combiEnable = true;
					SManager.instance.actNow = true;
					SManager.instance.castNow = false;
			//		SManager.instance.sisStatus.mp -= status.equipCombination.useMP;
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

    }


	public void ActionFire(float random = 0.0f) { }

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
					//targetCanStatus.Remove(statusList[removeNumber[i] - i]);
				}
				removeNumber = null;
				#endregion

			}

		}

		if (targetList.Count == 0)
		{
			targetCanList = null;
			//targetCanStatus = null;
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
		　　//targetCanStatus = null;
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
					////targetCanStatus.Remove(statusList[removeNumber[i] - i]);
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



		//	TargetSelect(status.equipCombination.mainTarget[conboChain], dammy);

			if (SManager.instance.target == null)
			{
				//TargetSelect(status.equipCombination.subTarget[conboChain], dammy);
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







