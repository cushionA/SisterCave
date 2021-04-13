using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SisterFire : MonoBehaviour
{
	public SisterParameter sister;
	public SisterStatus status;
	public Transform firePosition;
	SisterFireBullet sisF;
	SisterBrain sb;
	bool enableFire;
	List<GameObject> targetPlan;
	GameObject target;
	List<SisMagic> useSupport;//未使用の支援
	List<float> effectiveTime;//支援魔法、リジェネ、攻撃の時間をはかる

	float waitCast;//詠唱時間待ち

	float stateJudge = 30;
	float targetJudge = 30;

	int judgeSequence;

	// Start is called before the first frame update
	void Start()
	{
		sb = GetComponent<SisterBrain>();
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (sb.nowState == SisterBrain.SisterState.戦い && sb.nowPegion)
		{
			stateJudge += Time.fixedDeltaTime;
			if (status.equipMagic == null)
			{
				return;
			}
			if (SManager.instance.targetList == null)
			{
				//ステート解除
			}
			if (stateJudge >= sister.stateResetRes)
			{
				//ステートリセット
				sister.nowMove = sister.priority;
				stateJudge = 0.0f;
			}
			if (sister.nowMove == SisterParameter.MoveType.攻撃)
			{
				targetJudge += Time.fixedDeltaTime;
				if (targetJudge >= sister.targetResetRes || SManager.instance.targetRecord.Count != SManager.instance.targetCondition.Count)
				{

					SManager.instance.GetEnemyCondition();
					target = null;
					judgeSequence = 1;
					TargetSelect(sister.firstTarget);
					if(target == null)
                    {
						TargetSelect(sister.secondTarget);
						judgeSequence = 2;
					}
					if (target == null)
					{
						TargetSelect(sister.thirdTarget);
						judgeSequence = 3;
					}
					if (target == null)
					{
						TargetSelect(sister.forthTarget);
						judgeSequence = 4;
					}
					if(target == null)
                    {
						target = SManager.instance.targetRecord[RandomValue(0, SManager.instance.targetRecord.Count - 1)];
						judgeSequence = 5;
                    }
					targetJudge = 0.0f;
					//EnemyRecordとtargetConditionは一致してる。
					//敵情報更新
				}
				if(target != null)
                {
					if(judgeSequence == 1)
                    {
						AttackAct(sister.firstAttack);
                    }
					else if (judgeSequence == 2)
                    {
						AttackAct(sister.secondAttack);
                    }
					else if (judgeSequence == 3)
					{
						AttackAct(sister.thirdAttack);
					}
					else if (judgeSequence == 4)
					{
						AttackAct(sister.forthAttack);
					}
					else if (judgeSequence == 5)
					{
						AttackAct(sister.nonAttack);
					}

				}
				//クールタイム実装頼んだ
				ActionFire();

			}

			else if (sister.nowMove == SisterParameter.MoveType.支援)
			{
                //かかってない支援があるかどうかとかを調べなきゃダメ。ifなのかな。
                //もし一個目の条件が真なら発動、みたいな形式にすべき
                //じゃあスイッチ文のそれぞれのcaseのとこではHPの残量とかを調べるのか
                if (SupportJudge(sister.firstPlan))
                {
					SupportAct(sister.firstSupport);
                }
				else if (SupportJudge(sister.secondPlan))
				{
					SupportAct(sister.secondSupport);
				}
				else if (SupportJudge(sister.thirdPlan))
				{
					SupportAct(sister.thirdSupport);
				}
				else if (SupportJudge(sister.forthPlan))
				{
					SupportAct(sister.forthSupport);
				}
                else
                {
					SupportAct(sister.nonSupport);
                }
			}
			else if (sister.nowMove == SisterParameter.MoveType.回復)
			{
				if (HealJudge(sister.firstCondition))
				{
					RecoverAct(sister.firstRecover);
				}
				else if (HealJudge(sister.secondCondition))
				{
					RecoverAct(sister.secondRecover);
				}
				else if (HealJudge(sister.thirdCondition))
				{
					RecoverAct(sister.thirdRecover);
				}
				else if (HealJudge(sister.forthCondition))
				{
					RecoverAct(sister.forthRecover);
				}
				else
				{
					RecoverAct(sister.nonRecover);
				}

			}

		}
	}


	public void ActionFire(float random = 0.0f)
	{//ランダムに入れてもいいけど普通に入れてもいい

		if (status.mp >= status.useMagic.useMP && SManager.instance.targetObj != null)
		{
			waitCast += Time.fixedDeltaTime;
			if (waitCast >= SManager.instance.sisStatus.useMagic.castTime)
				if (random != 0)
				{
					firePosition.position = new Vector3
						(firePosition.position.x + random, firePosition.position.y + random, firePosition.position.z);//銃口から
				}
			Transform goFire = firePosition;

				//sAni.Play(SManager.instance.sisStatus.attackName[attackNumber]);

				for (int x = 0; x >= SManager.instance.sisStatus.useMagic.bulletNumber; x++)
				{
					Addressables.InstantiateAsync(SManager.instance.sisStatus.useMagic.effects, goFire.position, Quaternion.identity);//.Result;//発生位置をPlayer
				}

		}
		else
		{
			return;
		}
	}

	/// <summary>
	/// XとYの間で乱数を出す
	/// </summary>
	/// <param name="X"></param>
	/// <param name="Y"></param>
	/// <returns></returns>
	public int RandomValue(int X, int Y)
	{
		return Random.Range(X, Y + 1);

	}


	/// <summary>
	/// 攻撃ステートでターゲットを決定
	/// </summary>
	/// <param name="condition"></param>
	public void TargetSelect(SisterParameter.TargetJudge condition)
	{
		switch (condition)
		{
			case SisterParameter.TargetJudge.強敵:
				//強敵を優先
				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.TargetJudge.非強敵:

				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.TargetJudge.斬撃属性が弱点:

				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.TargetJudge.刺突属性が弱点:

				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.TargetJudge.打撃属性が弱点:

				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.TargetJudge.聖属性が弱点:

				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.TargetJudge.闇属性が弱点:

				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.TargetJudge.炎属性が弱点:

				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.TargetJudge.雷属性が弱点:

				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.TargetJudge.敵タイプ:

				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.TargetJudge.距離近いのから:

				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.TargetJudge.距離遠いのから:

				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.TargetJudge.敵のHP多いのから:

				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.TargetJudge.敵のHP少ないのから:

				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.TargetJudge.プレイヤーの体力がマックス:
				//以下の場合攻撃すると標的は敵リストの先頭に。大技を撃ったり回復等の他のモードに移行したり
				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.TargetJudge.プレイヤーの体力が半分以下:

				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.TargetJudge.プレイヤーの体力が二割以下:

				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.TargetJudge.状態異常にかかった時:

				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.TargetJudge.なし:
				//	ランダムバリュー使ってレコードから指定
				break;
			//-----------------------------------------------------------------------------------------------------
		}

	}

	/// <summary>
	/// 攻撃ステートで使用魔法を決定
	/// </summary>
	/// <param name="condition"></param>
	public void AttackAct(SisterParameter.AttackConditional condition)
	{
		switch (condition)
		{
			case SisterParameter.AttackConditional.MP消費が多い:
				SManager.instance.attackMagi.Sort((a, b) => (int)(b.useMP - a.useMP));
				SManager.instance.sisStatus.useMagic = SManager.instance.attackMagi[0];
				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.AttackConditional.MP消費が少ない:
				SManager.instance.attackMagi.Sort((a, b) => (int)(a.useMP - b.useMP));
				SManager.instance.sisStatus.useMagic = SManager.instance.attackMagi[0];
				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.AttackConditional.詠唱時間が短い:
				SManager.instance.attackMagi.Sort((a, b) => (int)(a.castTime - b.castTime));
				SManager.instance.sisStatus.useMagic = SManager.instance.attackMagi[0];
				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.AttackConditional.斬撃属性:
				foreach(SisMagic s in SManager.instance.attackMagi)
                {
					if(s.phyBase > 0 && s.atType == Magic.AttackType.Slash)
                    {
						SManager.instance.sisStatus.useMagic = s;
						break;
                    }
                }
				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.AttackConditional.刺突属性:
				foreach (SisMagic s in SManager.instance.attackMagi)
				{
					if (s.phyBase > 0 && s.atType == Magic.AttackType.Stab)
					{
						SManager.instance.sisStatus.useMagic = s;
						break;
					}
				}
				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.AttackConditional.打撃属性:
				foreach (SisMagic s in SManager.instance.attackMagi)
				{
					if (s.phyBase > 0 && s.atType == Magic.AttackType.Strike)
					{
						SManager.instance.sisStatus.useMagic = s;
						break;
					}
				}
				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.AttackConditional.聖属性:
				foreach (SisMagic s in SManager.instance.attackMagi)
				{
					if (s.holyBase >= 0)
					{
						SManager.instance.sisStatus.useMagic = s;
						break;
					}
				}
				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.AttackConditional.闇属性:
				foreach (SisMagic s in SManager.instance.attackMagi)
				{
					if (s.darkBase >= 0)
					{
						SManager.instance.sisStatus.useMagic = s;
						break;
					}
				}
				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.AttackConditional.炎属性:
				foreach (SisMagic s in SManager.instance.attackMagi)
				{
					if (s.fireBase >= 0)
					{
						SManager.instance.sisStatus.useMagic = s;
						break;
					}
				}
				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.AttackConditional.雷属性:
				foreach (SisMagic s in SManager.instance.attackMagi)
				{
					if (s.thunderBase >= 0)
					{
						SManager.instance.sisStatus.useMagic = s;
						break;
					}
				}
				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.AttackConditional.弾速が早い:
				SManager.instance.attackMagi.Sort((a, b) => (int)(b.speedV - a.speedV));
				SManager.instance.sisStatus.useMagic = SManager.instance.attackMagi[0];
				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.AttackConditional.敵を吹き飛ばせる:
				foreach (SisMagic s in SManager.instance.attackMagi)
				{
					if (s.isBlow)
					{
						SManager.instance.sisStatus.useMagic = s;
						break;
					}
				}
				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.AttackConditional.爆発する:
				foreach (SisMagic s in SManager.instance.attackMagi)
				{
					if (s.isExprode)
					{
						SManager.instance.sisStatus.useMagic = s;
						break;
					}
				}
				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.AttackConditional.発射数が多い:
				SManager.instance.attackMagi.Sort((a, b) => (int)(b.bulletNumber - a.bulletNumber));
				SManager.instance.sisStatus.useMagic = SManager.instance.attackMagi[0];
				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.AttackConditional.範囲攻撃:
				foreach (SisMagic s in SManager.instance.attackMagi)
				{
					if (s.isWide)
					{
						SManager.instance.sisStatus.useMagic = s;
						break;
					}
				}
				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.AttackConditional.貫通する:
				foreach (SisMagic s in SManager.instance.attackMagi)
				{
					if (s.penetration)
					{
						SManager.instance.sisStatus.useMagic = s;
						break;
					}
				}
				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.AttackConditional.状態異常つき:
					//異常攻撃力実装後
				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.AttackConditional.支援魔法:
				sister.nowMove = SisterParameter.MoveType.支援;
				SManager.instance.sisStatus.useMagic = null;
				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.AttackConditional.回復魔法:
				sister.nowMove = SisterParameter.MoveType.回復;
				SManager.instance.sisStatus.useMagic = null;
				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.AttackConditional.何もしない:
				SManager.instance.sisStatus.useMagic = null;
				break;
			//-----------------------------------------------------------------------------------------------------
		}
	}

	public bool SupportJudge(SisterParameter.SupportJudge condition)
	{
		switch (condition)
		{
			case SisterParameter.SupportJudge.かかっていない支援がある:
				useSupport.Clear();
				foreach(SisMagic s in SManager.instance.supportMagi)
                {
                    if (!s.effectNow)
                    {
						useSupport.Add(s);
                    }
                }

				return useSupport != null ? true : false; 
				//break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.SupportJudge.アクション強化がない:

				useSupport.Clear();
				foreach (SisMagic s in SManager.instance.supportMagi)
				{
					if (!s.effectNow && s.sType == SisMagic.SupportType.アクション強化)
					{
						useSupport.Add(s);
					}
				}

				return useSupport != null ? true : false;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.SupportJudge.エンチャントがない:

				useSupport.Clear();
				foreach (SisMagic s in SManager.instance.supportMagi)
				{
					if (!s.effectNow && s.sType == SisMagic.SupportType.エンチャント)
					{
						useSupport.Add(s);
					}
				}

				return useSupport != null ? true : false;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.SupportJudge.バリアがない:

				useSupport.Clear();
				foreach (SisMagic s in SManager.instance.supportMagi)
				{
					if (!s.effectNow && s.sType == SisMagic.SupportType.バリア)
					{
						useSupport.Add(s);
					}
				}

				return useSupport != null ? true : false;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.SupportJudge.防御強化がない:

				useSupport.Clear();
				foreach (SisMagic s in SManager.instance.supportMagi)
				{
					if (!s.effectNow && s.sType == SisMagic.SupportType.防御強化)
					{
						useSupport.Add(s);
					}
				}

				return useSupport != null ? true : false;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.SupportJudge.攻撃強化がない:

				useSupport.Clear();
				foreach (SisMagic s in SManager.instance.supportMagi)
				{
					if (!s.effectNow && s.sType == SisMagic.SupportType.攻撃強化)
					{
						useSupport.Add(s);
					}
				}

				return useSupport != null ? true : false;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.SupportJudge.プレイヤーの体力がマックス:

				return GManager.instance.pStatus.hp == GManager.instance.pStatus.maxHp ? true : false;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.SupportJudge.プレイヤーの体力が半分以下:

				return GManager.instance.pStatus.hp <= GManager.instance.pStatus.maxHp / 2 ? true : false;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.SupportJudge.プレイヤーの体力が二割以下:

				return GManager.instance.pStatus.hp <= GManager.instance.pStatus.maxHp / 5 ? true : false;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.SupportJudge.状態異常にかかった時:

				return GManager.instance.badCondition ? true : false;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.SupportJudge.なし:
				//	ランダムバリュー使ってレコードから指定
				return true;
			//-----------------------------------------------------------------------------------------------------
		}
		return false;
	}

	public void SupportAct(SisterParameter.SupportConditional condition)
	{
		switch (condition)
		{
			case SisterParameter.SupportConditional.エンチャント:
                foreach (SisMagic s in useSupport)
                {
                    if (!s.effectNow)
                    {
						SManager.instance.sisStatus.useMagic = s;
					}
                }
				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.SupportConditional.バリア:

              foreach (SisMagic s in useSupport)
				{
					if (!s.effectNow)
					{
						SManager.instance.sisStatus.useMagic = s;
					}
				}
				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.SupportConditional.アクション強化:
				foreach (SisMagic s in useSupport)
				{
					if (!s.effectNow)
					{
						SManager.instance.sisStatus.useMagic = s;
					}
				}
				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.SupportConditional.攻撃強化:
				foreach (SisMagic s in useSupport)
				{
					if (!s.effectNow)
					{
						SManager.instance.sisStatus.useMagic = s;
					}
				}
				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.SupportConditional.防御強化:
				foreach (SisMagic s in useSupport)
				{
					if (!s.effectNow)
					{
						SManager.instance.sisStatus.useMagic = s;
					}
				}
				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.SupportConditional.攻撃魔法:
				SManager.instance.sisStatus.useMagic = null;
				sister.nowMove = SisterParameter.MoveType.攻撃;
				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.SupportConditional.回復魔法:
				SManager.instance.sisStatus.useMagic = null;
				sister.nowMove = SisterParameter.MoveType.回復;
				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.SupportConditional.何もしない:
				SManager.instance.sisStatus.useMagic = null;
				break;
		}
	}

	public bool HealJudge(SisterParameter.RecoverJudge condition)
	{
		switch (condition)
		{
			case SisterParameter.RecoverJudge.プレイヤーの体力がマックス:

				return GManager.instance.pStatus.hp == GManager.instance.pStatus.maxHp ? true : false;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.RecoverJudge.プレイヤーの体力が半分以下:

				return GManager.instance.pStatus.hp <= GManager.instance.pStatus.maxHp / 2 ? true : false;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.RecoverJudge.プレイヤーの体力が二割以下:

				return GManager.instance.pStatus.hp <= GManager.instance.pStatus.maxHp / 5 ? true : false;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.RecoverJudge.状態異常にかかった時:

				return GManager.instance.badCondition ? true : false;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.RecoverJudge.リジェネが切れたとき:
				useSupport.Clear();
				foreach (SisMagic s in SManager.instance.recoverMagi)
				{
					if (!s.effectNow && s.sType == SisMagic.SupportType.リジェネ)
					{
						useSupport.Add(s);
					}
				}
				return useSupport != null ? true : false;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.RecoverJudge.なし:
				//	ランダムバリュー使ってレコードから指定
				return true;
				//-----------------------------------------------------------------------------------------------------
		}
		return false;
	}

	public void RecoverAct(SisterParameter.RecoverConditional condition)
	{
		switch (condition)
		{
			case SisterParameter.RecoverConditional.状態異常を解除する回復:
				foreach (SisMagic s in SManager.instance.recoverMagi)
				{
					if (s.cureCondition)
					{
						SManager.instance.sisStatus.useMagic = s;
						break;
					}
				}
				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.RecoverConditional.リジェネ時間が長い回復:
				SManager.instance.recoverMagi.Sort((a, b) => (int)(b.effectTime - a.effectTime));
				SManager.instance.sisStatus.useMagic = SManager.instance.recoverMagi[0];
				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.RecoverConditional.回復量が大きい回復:
				SManager.instance.recoverMagi.Sort((a, b) => (int)(SManager.instance.SetRecoverAmount(b) - SManager.instance.SetRecoverAmount(a)));
				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.RecoverConditional.MP消費が少ない回復:
				SManager.instance.recoverMagi.Sort((a, b) => (int)(a.useMP - b.useMP));
				SManager.instance.sisStatus.useMagic = SManager.instance.recoverMagi[0];
				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.RecoverConditional.詠唱時間が短い回復:
				SManager.instance.recoverMagi.Sort((a, b) => (int)(a.castTime - b.castTime));
				SManager.instance.sisStatus.useMagic = SManager.instance.recoverMagi[0];
				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.RecoverConditional.攻撃魔法:
				sister.nowMove = SisterParameter.MoveType.攻撃;
				SManager.instance.sisStatus.useMagic = null;
				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.RecoverConditional.支援魔法:
				sister.nowMove = SisterParameter.MoveType.支援;
				SManager.instance.sisStatus.useMagic = null;
				break;
			//-----------------------------------------------------------------------------------------------------
			case SisterParameter.RecoverConditional.何もしない:
				SManager.instance.sisStatus.useMagic = null;
				break;
		}
	}


}
