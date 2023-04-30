
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;


public class SisterBrain : MonoBehaviour
{




	//public GameObject player;
	[Header("警戒ステートの時間")]
	public float patrolTime;
	[Header("視覚探知")]
	///<summary>
	///敵検知用
	///</summary>
	public GameObject Serch;

	[Header("全方位の気配探知")]
	///<summary>
	///これで環境物とかに反応
	///</summary>
	public GameObject Serch2;

	[Header("戦闘状態の感知網")]
	///<summary>
	///これで環境物とかに反応
	///</summary>
	public GameObject Serch3;

	[Header("射出地点")]
	///<summary>
	///弾丸出す場所
	///</summary>
	public Transform firePosition;

	[SerializeField]
	 private LayerMask layerMask;

	bool disEnable;
	bool isReset;


	public enum SisterState
    {
		のんびり,
		警戒,
		戦い
    }

	[HideInInspector] public SisterState nowState;


	[Header("シスターさんのステータス")]
	public SisterStatus status;

	public AnimationCurve jMove;
	public AnimationCurve jPower;

	// === 外部パラメータ ======================================
	/*[HideInInspector]*/
	Vector2 distance;//プレイヤーとの距離
	 bool isJump;//ジャンプしてるかどうか
	 float stopTime;//停止時間を数える
	 bool isStop;//停止フラグ
	 float waitTime;//パトロール時の待ち時間数える
//	 bool posiReset;//戦闘の持ち場再設定するフラグ
	 bool nowJump;
	[HideInInspector]
	public bool isGround;

	 Vector2 targetPosition;//敵の場所
	 int initialLayer;
	 bool jumpTrigger;

	 bool isWait;//このフラグが真なら待機モーションする。マントいじったり遊んだり。
				 //モーションは好感度とか進行度でモーション名リストが切り替わって決まる。
	bool isWarp;//ワープ中

	[HideInInspector]public bool isPosition;//持ち場フラグ
	[HideInInspector] public bool nowPosition;//持ち場フラグ
	Vector2 myPosition;


	// === キャッシュ ======================================
	 public Animator sAni;


	public Rigidbody2D rb;//継承先で使うものは

	// === 内部パラメータ ======================================

	 int direction;
	 int directionY;
	 float moveDirectionX;
	 float moveDirectionY;
	 float jumpTime;//ジャンプのクールタイム。処理自体はインパルスで


	 Vector2 basePosition;
	/// <summary>
	/// プレイヤーが向いてる方向。基準
	/// </summary>
	 Vector3 baseDirection;
	 bool isDown;
	 float randomTime = 2.5f;//ランダム移動測定のため
	[HideInInspector] public bool guardHit;

//	public Rigidbody2D GManager.instance.GManager.instance.pm.rb;//プレイヤーのリジッドボディ

	bool isSquat;
	string squatTag = "SquatWall";
	float PositionJudge;
	bool isClose;//一度近くまで行く
	[HideInInspector]public bool isPlay;
	[HideInInspector] public float playPosition;//環境物の場所
	[HideInInspector] public float playDirection;//遊ぶ時の方向
	string jumpTag = "JumpTrigger";
	bool reJump;//もう一回飛ぶ
	bool doJump;//ジャンプ終了
	Vector3 nowPos;//ジャンプ前の地点記録
	float patrolJudge;//警戒時間を数える
	float jumpWait;//ジャンプ可能になるまでの待機時間
	float verticalWait;//垂直ジャンプ可能になるまでの待機時間
	bool isVertical;//垂直飛びするフラグ
	Vector2 waitPosition;
	//持ち場再判断
	[HideInInspector]public float reJudgePositionTime;
	[HideInInspector] public int stateNumber;
	[HideInInspector] public int beforeNumber;
	[HideInInspector] public float reJudgeTime;
	[HideInInspector] public bool changeable;//歩行からダッシュなど変更可能か

	float directionChangeWait;
	float battleEndTime;
	Vector2 move;






	// === コード（Monobehaviour基本機能の実装） ================
	 void Start()
	{
		Serch3.SetActive(false);
		//rb = GetComponent<Rigidbody2D>();
		//GManager.instance.GManager.instance.pm.rb = player.gameObject.GetComponent<Rigidbody2D>();
		
		GravitySet(0.0f);
		/*		if (SManager.instance.sisStatus.equipMagic != null)
				{
					enableFire = true;
				}
		*/
	}



	 void Update()
	{
		// 落下チェック

	}

	void FixedUpdate()
	{
		if (rb.IsTouching(SManager.instance.sisStatus.filter))
		{
			isGround = true;

		}

		else
		{

			RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 2, layerMask);
			if (hit && isGround)
			{


				isGround = true;


			}
			else
			{

				isGround = false;
				//}
			}
		}

		if (isWarp)
        {
			disEnable = true;
        }
        else
        {
			disEnable = false;
        }

		basePosition = GManager.instance.Player.transform.position;
		baseDirection = GManager.instance.Player.transform.localScale;
		//↑プレイヤーが向いてる方向とプレイヤーの位置

		myPosition = this.transform.position;
		distance = basePosition - myPosition;
		direction = distance.x >= 0 ? 1 : -1;//距離が正、または0の時1。そうでないとき-1。方向
		directionY = distance.y >= 0 ? 1 : -1;//ほかのAIで弓構えるときのアニメの判定	にも使えそう

		//	Debug.Log($"現在のステート{nowState}");
		SisterFall();
		EscapeWarp();
		if (nowState == SisterState.戦い)
		{
			//Debug.Log($"位置についてますかー{nowPosition}");
			SManager.instance.BattleEndCheck();
			BattleEnd();

			if (!isPosition)
			{
				isPosition = true;
			}
			PositionSetting();
			if (!nowPosition)
			{
				TriggerJump();
				Verticaljump();
			}
		}

		else if (nowState == SisterState.警戒)
		{
			patrolJudge += Time.fixedDeltaTime;


			//Serch.SetActive(true);ほかのステートに移動するときに
			//Serch2.SetActive(true);

			if (!SManager.instance.actNow)
			{
				PatrolMove();
		

			//	PositionSetting();
				TriggerJump();
				Verticaljump();
	        }

			//	PositionChange();
				if (patrolJudge >= patrolTime)
				{
					beforeNumber = 0;
					reJudgeTime = 0;
					stateNumber = 2;
					changeable = true;
					nowState = SisterState.のんびり;
					//Serch3.SetActive(false);
					//Serch2.SetActive(true);
					//Serch.SetActive(true);
				}


		}
		else if (nowState == SisterState.のんびり)
		{
			if (!SManager.instance.actNow)
			{
				PlayMove();

				TriggerJump();
				Verticaljump();
			}
			WaitJudge();
			//待機状態には色々する

			//PositionChange();

			//if()

		//	SisterFall();
			//SetVelocity();
			if (isVertical)
			{
				GroundJump(0, status.jumpPower * 1.4f); //status.addSpeed * transform.localScale.x/3);
			}
			else
			{
				GroundJump(status.jumpSpeed * transform.localScale.x, status.jumpPower * 1.2f); //status.addSpeed * transform.localScale.x/3);
			}
		}
	}

	/// <summary>
	/// 支援位置につかせるメソッド
	/// </summary>
	public void PositionSetting()
    {
		if (Serch3.activeSelf == false)
		{
			Serch3.SetActive(true);
		}
        #region
        //  if (!posiReset)
        //   {
        /*	for(int i =0; i >= SManager.instance.targetList.Count; i++)
            {
                //敵が単体なら
                if (i == 0)
                {
                    SManager.instance.closestEnemy = SManager.instance.targetList[0].transform.position.x;
                }
                //敵が複数なら
                else
                {
                    if(Mathf.Abs(transform.position.x - SManager.instance.targetList[i].transform.position.x) < Mathf.Abs(transform.position.x - SManager.instance.closestEnemy))
                    {
                        SManager.instance.closestEnemy = SManager.instance.targetList[i].transform.position.x;
                    }
                }
                //調べ終わったら
                if(i >= SManager.instance.targetList.Count - 1)
                {
                    posiReset = true;
                }
            }*/
        //	SManager.instance.GetClosestEnemyX();
        //	posiReset = true;

        //}
        //	else if (posiReset)
        //	{
        #endregion
        if (!SManager.instance.actNow && isPosition && isGround && !disEnable) { 
			//ポジションにつきに行く。かつついてなくて地面にいる
			if (!nowPosition)
			{
				SManager.instance.GetClosestEnemyX();
				float mDirection = Mathf.Sign(SManager.instance.closestEnemy - myPosition.x);
				//一番近い敵が右にいるとき
				reJudgeTime += Time.fixedDeltaTime;
				if (mDirection >= 0)
				{
					//没
					#region
					/*			//プレイヤーの左に行く
								if (myPosition.x <= basePosition.x - status.battleDis && myPosition.x >= basePosition.x - status.battleDis * 2)
								{
									rb.velocity = Vector2.zero;
									Flip(-1);
									//おんぶする
									//ポジションに到着
									nowPosition = true;
									//("Stand");
								}
								else if (myPosition.x > basePosition.x - status.battleDis)
								{
									Flip(-1);
									//move.Set(status.addSpeed * (-status.dashSpeed - rb.velocity.x), 0);
									rb.AddForce(move);
									//("Dash");
								}
								else if (myPosition.x < basePosition.x - status.battleDis * 2)
								{
									Flip(1);
									//move.Set(status.addSpeed * (status.dashSpeed - rb.velocity.x), 0);
									rb.AddForce(move);
									//("Dash");
								}*/
					#endregion

					
					if (reJudgeTime >= 1.5)
					{
						if (SManager.instance.closestEnemy - myPosition.x >= status.battleDis)
						{
							stateNumber = 0;
							directionChangeWait = 99;
						}
						else// if (SManager.instance.closestEnemy - myPosition.x < status.battleDis)
						{
							stateNumber = 1;
							directionChangeWait = 99;

						}
						reJudgeTime = 0;
					}
					if (stateNumber == 0)
					{
					//	Debug.Log("停止");
						rb.velocity = Vector2.zero;
						Flip(mDirection,true);
						//おんぶする
						//ポジションに到着
						nowPosition = true;
						//("Stand");
						reJudgeTime = 100;
					}
					else if (stateNumber == 1)
					{
					//	Debug.Log($"逃げる{mDirection}");
						Flip(-mDirection);
						//move.Set(status.addSpeed * (-status.dashSpeed - rb.velocity.x), 0);
						rb.AddForce(move);
						//("Dash");
					}

					else
					{
						reJudgeTime = 100;
					}
				}
				else// if (SManager.instance.closestEnemy - myPosition.x < 0)
				{
					#region
					/*
					if (myPosition.x >= basePosition.x + status.battleDis && myPosition.x <= basePosition.x + status.battleDis * 2)
					{
						rb.velocity = Vector2.zero;
						Flip(1);
						nowPosition = true;
						//ポジションに到着
						//("Stand");
					}
					else if (myPosition.x < basePosition.x + status.battleDis)
					{
						Flip(1);
						//move.Set(status.addSpeed * (status.dashSpeed + rb.velocity.x), 0);
						rb.AddForce(move);
						//("Dash");
					}
					else if (myPosition.x > basePosition.x + status.battleDis * 2)
					{
						Flip(-1);
						//move.Set(status.addSpeed * (-status.dashSpeed + rb.velocity.x), 0);
						rb.AddForce(move);
						//("Dash");
					}

					*/
					#endregion
					//reJudgeTime += Time.fixedDeltaTime;
					if (reJudgeTime >= 1.5)
					{
						if (Mathf.Abs(SManager.instance.closestEnemy - myPosition.x) >= status.battleDis)
						{
							stateNumber = 2;

						}
						else// if (Mathf.Abs(SManager.instance.closestEnemy - myPosition.x) < status.battleDis)
						{
							stateNumber = 3;

						}
						reJudgeTime = 0;
					}
					if (stateNumber == 2)
					{
						rb.velocity = Vector2.zero;
						Flip(mDirection,true);
						//おんぶする
						//ポジションに到着
						nowPosition = true;
						//("Stand");
						reJudgeTime = 100;
					}
					else if(stateNumber == 3) 
					{
						Flip(-mDirection);

						//move.Set(status.addSpeed * (status.dashSpeed - rb.velocity.x), 0);
						rb.AddForce(move);
						//("Dash");
					}
                    else
                    {
						reJudgeTime = 100;
                    }
				}
			}
			else if( nowPosition)
            {
				//再判定するかどうか
				//範囲内にて聞いたら再判定
				if (!SManager.instance.castNow)
				{
					float atDirection;
					reJudgePositionTime += Time.fixedDeltaTime;
					if (SManager.instance.target != null)
					{
						 atDirection = SManager.instance.target.transform.position.x - myPosition.x;
						rb.velocity = Vector2.zero;
						atDirection = Mathf.Sign(atDirection);
						Flip(atDirection);
					}
               /*     else
                    {
						atDirection = direction;
                    }*/
					
					//おんぶする
				}
			if (reJudgePositionTime >= SManager.instance.sisStatus.escapeTime)
                {
					reJudgePositionTime = 0.0f;
					reJudgeTime = 150;
					SManager.instance.GetClosestEnemyX();
					//プレイヤーがワープ上限より離れていたら

					//敵が逃げるゾーンより近かったから

						nowPosition = false;
					directionChangeWait = 99;
				}

            }
	//	}
		}
	}


	/// <summary>
	/// 戦闘中の位置取り
	/// </summary>
	public void PositionChange()
    {
        if (GManager.instance.pm.isSquat && !isSquat && GManager.instance.pm.rb.velocity == Vector2.zero)
        {
			PositionJudge += Time.fixedDeltaTime;
            if (nowPosition)
            {
				if(PositionJudge >= 2.0f)
                {
					isPosition = false;
					PositionJudge = 0.0f;
				}
            }
			else if (!nowPosition)
            {
				if (PositionJudge >= 5.0f)
				{
					isPosition = true; ;
					PositionJudge = 0.0f;
				}
			}
        }
        else
        {
			PositionJudge = 0.0f;
        }

    }

/*	public void SetVelocity()
	{

		if (rb.velocity.x > SManager.instance.sisStatus.velocityMax.x || rb.velocity.x < SManager.instance.sisStatus.velocityMin.x)
		{
			Mathf.Clamp(rb.velocity.x, SManager.instance.sisStatus.velocityMin.x, SManager.instance.sisStatus.velocityMax.x);
		}
		if (rb.velocity.y > SManager.instance.sisStatus.velocityMax.y || rb.velocity.y < SManager.instance.sisStatus.velocityMin.y)
		{
			Mathf.Clamp(rb.velocity.y, SManager.instance.sisStatus.velocityMin.y, SManager.instance.sisStatus.velocityMax.y);
		}

	}*/

	public void Flip(float direction,bool isForce = false)
	{
		if (!isStop && (directionChangeWait >= 0.3 || isForce) )
		{
			// Switch the way the player is labelled as facing.

			// Multiply the player's x local scale by -1.
			Vector3 theScale = transform.localScale;
			theScale.x = direction;
			transform.localScale = theScale;
			directionChangeWait = 0;
		}
        else
        {
			directionChangeWait += Time.fixedDeltaTime;

        }
	}
	public void ATFlip()
	{

			Vector3 theScale = transform.localScale;
			theScale.x = SManager.instance.target.transform.position.x - myPosition.x >= 0 ? 1 : -1;
			transform.localScale = theScale;
	}
	/// <summary>
	/// 待機中の哨戒行動
	/// </summary>
	public void PatrolMove()
	{	
		if (isGround && !disEnable)
		{
			if (reJudgeTime >= 1 && changeable)
			{
				if (Mathf.Abs(distance.x) > status.walkDistance)
				{
					stateNumber = 1;
				}

				else if ((Mathf.Abs(distance.x) <= status.walkDistance && Mathf.Abs(distance.x) > status.patrolDistance) || (Mathf.Sign(GManager.instance.pm.rb.velocity.x) == Mathf.Sign(transform.localScale.x) && GManager.instance.pm.rb.velocity.x != 0))
				{
					stateNumber = 2;
				}
				else if ((Mathf.Sign(GManager.instance.pm.rb.velocity.x) != Mathf.Sign(transform.localScale.x) || GManager.instance.pm.rb.velocity.x == 0) && Mathf.Abs(distance.x) <= status.patrolDistance)
				{
				
					stateNumber = 3;
				}
				if (beforeNumber == 0)
				{
					beforeNumber = stateNumber;
					changeable = true;
				}
				else if (beforeNumber == stateNumber)
				{
					changeable = true;
				}
				else if (beforeNumber != stateNumber)
				{
					changeable = false;
					beforeNumber = 0;
					directionChangeWait = 99;
				}

			}
			else if (reJudgeTime >= 1 && !changeable)
			{
				reJudgeTime = 0;
			}
			else if (reJudgeTime < 1)
			{
				reJudgeTime += Time.fixedDeltaTime;
				changeable = true;
			}
			if (stateNumber == 1)
			{
				
				Flip(direction);
				//move.Set(status.addSpeed * (status.patrolSpeed * direction - rb.velocity.x), 0);
				rb.AddForce(move);
				isWait = false;
				//("Move");
			}
			else if (stateNumber == 2)
			{
			//Debug.Log("でそ");
				Flip(direction);
				//move.Set(status.addSpeed * (status.walkSpeed * direction - rb.velocity.x), 0);
				rb.AddForce(move);
				isWait = false;
				//("Walk");
			}
			else if (stateNumber == 3)
			{
			//	Debug.Log("です");
				rb.velocity.Set(0,0);
				isWait = true;
				//("Stand");
			}


		}
	}
	public void PlayMove()
	{//のんびり
		if (isGround && !disEnable)
		{
			if (reJudgeTime >= 1.5 && changeable)
			{

				if (Mathf.Abs(distance.x) > status.playDistance || isClose)
				{
					stateNumber = 1;
					isClose = true;//接近しようっていうフラグ
					if (Mathf.Abs(distance.x) <= status.patrolDistance)
					{
						//くっついたら接近指示フラグ解除
						isClose = false;
					}
				}
				else if (Mathf.Abs(distance.x) <= status.playDistance && Mathf.Abs(distance.x) > status.patrolDistance)
				{
					if (isPlay)
					{
						//環境物に接触してる時

						if (myPosition.x >= playPosition - 2 && myPosition.x <= playPosition + 2)
						{
							stateNumber = 2;
						}
						else if (myPosition.x < playPosition)
						{
							stateNumber = 3;
						}
						else if (myPosition.x > playPosition)
						{
							stateNumber = 4;
						}
					}
					else if (Mathf.Abs(distance.x) > status.patrolDistance + status.walkDistance)
					{
						stateNumber = 5;
					}
					else if (Mathf.Abs(distance.x) <= status.patrolDistance + status.walkDistance && Mathf.Abs(distance.x) > status.patrolDistance)
					{
						stateNumber = 6;
					}

					//Flip(direction);
					
				}
				else if (Mathf.Abs(distance.x) <= status.patrolDistance)
				{
					//くっついてるとき
					//近くにいる
					if (isPlay)
					{
						//環境物に接触
						waitTime = 0.0f;
						if (myPosition.x >= playPosition - 2 && myPosition.x <= playPosition + 2)
						{
							stateNumber = 7;
						}
						else if (myPosition.x < playPosition)
						{
							stateNumber = 8;
						}
						else if (myPosition.x > playPosition)
						{
							stateNumber = 9;
						}
					}
					else if (GManager.instance.pm.rb.velocity.x == 0 && !isPlay)
					{
						stateNumber = 10;
					}
					else
					{
						stateNumber = 11;
					}

					if(beforeNumber == 0)
                    {
						beforeNumber = stateNumber;
						changeable = true;
					}
					else if (beforeNumber == stateNumber)
                    {
						changeable = true;
                    }
					else if(beforeNumber != stateNumber)
                    {
						changeable = false;
						beforeNumber = 0;
						directionChangeWait = 99;
					}

				}
			}
			else if (reJudgeTime >= 1.5 && !changeable)
            {
				reJudgeTime = 0;
            }
			else if (reJudgeTime < 1.5f)
            {
				reJudgeTime += Time.fixedDeltaTime;
				changeable = true;
            }

			if (stateNumber == 1)
			{
				//遊びでうろついていい範囲から離れてるならくっつくまで走る。
				//isCloseは接近しなければならないというフラグ
				//いったんすぐ近くまで来よう
				Flip(direction);
				//move.Set(status.addSpeed * (status.dashSpeed * direction - rb.velocity.x), 0);
				rb.AddForce(move);
				//	isWait = false;//立ち止まってる。

			}

					//環境物に接触してる時

					 else if (stateNumber == 2)
					{

						//環境物のすぐそばにいるとき
						move.Set(0, rb.velocity.y);rb.velocity = move;
						//("Stand");
					}
					else if (stateNumber == 3)
					{
						Flip(1);
				//環境物より後ろにいるとき
				        //move.Set(status.addSpeed * (status.dashSpeed - rb.velocity.x), 0);
						rb.AddForce(move);
						//("Move");
					}
					else if (stateNumber == 4)
					{
						Flip(-1);
				//環境物より前にいるとき
				        //move.Set(status.addSpeed * (-status.dashSpeed - rb.velocity.x), 0);
						rb.AddForce(move);
						//("Move");
					}
				
				else if (stateNumber == 5)
				{
					//遊んでいい範囲にいて、くっついてなくて環境物がないとき。小走りで行く
					//警戒距離まで来る
					//isWait = true;
					Flip(direction);
				    //move.Set(status.addSpeed * (status.dashSpeed * direction - rb.velocity.x), 0);
					rb.AddForce(move);
					//isWait = false;//立ち止まってる。
					//("Move");
				}
				else if(stateNumber == 6)
				{

					//("Walk");
					//waitTime = 0.0f;
					Flip(direction);
				//move.Set(status.addSpeed * (direction * status.walkSpeed - rb.velocity.x), 0);
					rb.AddForce(move);
				}

				//Flip(direction);
				//rb.AddForce(move(status.addSpeed * (status.dashSpeed * direction - rb.velocity.x), 0));

			

					//環境物に接触
				//	waitTime = 0.0f;
					else if (stateNumber == 7)
					{
						//環境物のすぐそばにいるとき
						move.Set(0, rb.velocity.y);
				        rb.velocity = move;
						//("Stand");
					}
					else if (stateNumber == 8)
					{
						Flip(1);
				//環境物より後ろにいるとき
				//move.Set(status.addSpeed * (status.dashSpeed - rb.velocity.x), 0);
						rb.AddForce(move);
						//("Move");
					}
					else if (stateNumber == 9)
					{
						Flip(-1);
				//環境物より前にいるとき
				        //move.Set(status.addSpeed * (-status.dashSpeed - rb.velocity.x), 0);
						rb.AddForce(move);
						//("Move");
					}
				
				else if (stateNumber == 10)
				{
					waitTime += Time.fixedDeltaTime;
					Flip(direction);
					//プレイヤーが立ち止まってて環境物がないとき
					move.Set(0, rb.velocity.y);rb.velocity = move;
					//("Stand");
					if (waitTime >= SManager.instance.sisStatus.waitRes && !isWait)
					{
						waitPosition = basePosition;
						isWait = true;
						waitTime = 0.0f;
					}
				}
				else
				{
					//("Walk");
					waitTime = 0.0f;
					Flip(direction);
     				//move.Set(status.addSpeed * (direction * status.walkSpeed - rb.velocity.x), 0);
					rb.AddForce(move);
				}

			

            #region
            /*保存			if (Mathf.Abs(distance.x) > status.playDistance || isClose)
                        {
                            //遊びでうろついていい範囲から離れてるならくっつくまで走る。
                            //isCloseは接近しなければならないというフラグ
                            //いったんすぐ近くまで来よう
                            Flip(direction);
                            rb.AddForce(move(status.addSpeed * (status.dashSpeed * direction - rb.velocity.x), 0));
                        //	isWait = false;//立ち止まってる。
                            isClose = true;//接近しようっていうフラグ
                            if (Mathf.Abs(distance.x) <= status.patrolDistance)
                            {
                                //くっついたら接近指示フラグ解除
                                isClose = false;
                            }
                        }
                        else if (Mathf.Abs(distance.x) <= status.playDistance && Mathf.Abs(distance.x) > status.patrolDistance)
                        {
                            //遊んでいい範囲にいて、くっつかなくていい
                            //遊んでいい距離だが、いるべき距離にはいない

                            if (isPlay)
                            {
                                //環境物に接触してる時

                                if (myPosition.x >= playPosition - 2 && myPosition.x <= playPosition + 2)
                                {

                                    //環境物のすぐそばにいるとき
                                    rb.velocity = move(0, rb.velocity.y);
                                    //("Stand");
                                }
                                else if (myPosition.x < playPosition)
                                {
                                    Flip(1);
                                    //環境物より後ろにいるとき
                                    rb.AddForce(move(status.addSpeed * (status.dashSpeed - rb.velocity.x), 0));
                                    //("Move");
                                }
                                else if (myPosition.x > playPosition)
                                {
                                    Flip(-1);
                                    //環境物より前にいるとき
                                    rb.AddForce(move(status.addSpeed * (-status.dashSpeed - rb.velocity.x), 0));
                                    //("Move");
                                }
                            }
                            else if(Mathf.Abs(distance.x) > status.patrolDistance + status.walkDistance)
                            {
                                //遊んでいい範囲にいて、くっついてなくて環境物がないとき。小走りで行く
                                //警戒距離まで来る
                                //isWait = true;
                                Flip(direction);
                                rb.AddForce(move(status.addSpeed * (status.dashSpeed * direction - rb.velocity.x), 0));
                                //isWait = false;//立ち止まってる。
                                //("Move");
                            }
                            else if(Mathf.Abs(distance.x) <= status.patrolDistance + status.walkDistance && Mathf.Abs(distance.x) > status.patrolDistance)
                            {

                                //("Walk");
                                //waitTime = 0.0f;
                                Flip(direction);
                                rb.AddForce(move(status.addSpeed * (direction * status.walkSpeed - rb.velocity.x), 0));
                            }

                            //Flip(direction);
                            //rb.AddForce(move(status.addSpeed * (status.dashSpeed * direction - rb.velocity.x), 0));

                        }
                        else if (Mathf.Abs(distance.x) <= status.patrolDistance)
                        {
                            //くっついてるとき
                            //近くにいる
                            if (isPlay)
                            {
                                //環境物に接触
                                waitTime = 0.0f;
                                if (myPosition.x >= playPosition - 2 && myPosition.x <= playPosition + 2)
                                {
                                    //環境物のすぐそばにいるとき
                                    rb.velocity = move(0, rb.velocity.y);
                                    //("Stand");
                                }
                                else if (myPosition.x < playPosition)
                                {
                                    Flip(1);
                                    //環境物より後ろにいるとき
                                    rb.AddForce(move(status.addSpeed * (status.dashSpeed - rb.velocity.x), 0));
                                    //("Move");
                                }
                                else if (myPosition.x > playPosition)
                                {
                                    Flip(-1);
                                    //環境物より前にいるとき
                                    rb.AddForce(move(status.addSpeed * (-status.dashSpeed - rb.velocity.x), 0));
                                    //("Move");
                                }
                            }
                            else if (GManager.instance.GManager.instance.pm.rb.velocity.x == 0 && !isPlay)
                            {
                                waitTime += Time.fixedDeltaTime;
                                Flip(direction);
                                //プレイヤーが立ち止まってて環境物がないとき
                                rb.velocity = rb.velocity = move(0, rb.velocity.y);
                                //("Stand");
                                if (waitTime >= SManager.instance.sisStatus.waitRes && !isWait)
                                {
                                    waitPosition = basePosition;
                                    isWait = true;
                                    waitTime = 0.0f;
                                }
                            }
                            else
                            {
                                //("Walk");
                                waitTime = 0.0f;
                                Flip(direction);
                                rb.AddForce(move(status.addSpeed * (direction * status.walkSpeed - rb.velocity.x), 0));
                            }

                        }*/
            #endregion
        }
    }


	/// <summary>
	/// 待機中停止してきょろきょろ振り向きだけ。こいつはパトロールしないからwaitTimeは使いまわしでいい
	/// </summary>
	public void Wait()
	{
				//("Stand");
				waitTime += Time.fixedDeltaTime;
				move.Set(0, rb.velocity.y);rb.velocity = move;
				if (waitTime >= SManager.instance.sisStatus.waitRes)
				{
					Flip(-transform.localScale.x);//反転させます
					waitTime = 0.0f;
				}
	}


    public void TriggerJump()
    {

		if (jumpWait<= 3.0f)
		{
			jumpWait += Time.fixedDeltaTime;
		}
		//////Debug.log($"ジャンプトリガー{jumpTrigger}");
		if (jumpTrigger && jumpWait >= 2.0f && isGround)
		{
				    isJump = true;
				    jumpWait = 0;
				jumpTrigger = false;

			
			
		/*if (doJump)
			{//一度ジャンプしてるなら
				if(nowPos.y != transform.position.y)
                {
					reJump = false;
					doJump = false;
					jumpTrigger = false;
                }
				else if(nowPos.y　== transform.position.y)
                {
					reJump = true;
					doJump = false;

                }
			}*/
		}
    }

	public void TriggerCancel()
    {

    }

	/// <summary>
	///	地上キャラ用のジャンプ。ジャンプ状態は自動解除。ジャンプキャンセルとセット
	/// </summary>	
	public void GroundJump(float jumpMove, float jumpPower)
	{
		if (!isStop&& !isDown && !disEnable)
		{
			/*	if (isJump)
				{
					//("Jump");
					jumpTime += Time.fixedDeltaTime;
					rb.AddForce(move(jumpMove,jumpPower));
					//GravitySet(0);
					//nowJump = true;
				}
				if (jumpTime >= status.jumpRes)
				{
					////(status.motionIndex["落下"]);
					isJump = false;
					jumpTime = 0.0f;
					//jumpTrigger = false;
				}*/
			////(SManager.instance.sisStatus.motionIndex["ジャンプ"]);
			//jumpTime += Time.fixedDeltaTime;
			//rb.AddForce(move(jumpMove, jumpPower), ForceMode2D.Impulse);
			//GravitySet(0);
			////(SManager.instance.sisStatus.motionIndex["落下"]);
			//	jumpTime = 0.0f;
			//GravitySet(SManager.instance.sisStatus.firstGravity);

			if (isJump)
            {
				jumpTime += Time.fixedDeltaTime;
				move.Set(jumpMove * jMove.Evaluate(jumpTime), jumpPower * jPower.Evaluate(jumpTime));
				rb.AddForce(move);
				//("Jump");
				if (isGround && jumpTime > 0.1)
                {//
					rb.velocity = Vector2.zero;
					isJump = false;
					jumpTime = 0.0f;
					jumpTrigger = false;
					isVertical = false;
                }

			}


			}
	}

	/// <summary>
	/// ジャンプ終了着地判定。地上キャラのみ
	/// </summary>
	public void Verticaljump()
	{
		if (isGround && (Mathf.Abs(distance.x) <= status.patrolDistance) && distance.y > 5 && isGround)
        {
			if (verticalWait <= 3.0f)
			{
				verticalWait += Time.fixedDeltaTime;
			}
			if (verticalWait >= 2.0f)
			{
				isVertical = true;
				isJump = true;
				verticalWait = 0.0f;

			}
		}
	}


	/// <summary>
	/// レイヤー変更。Avoidなんかと使ってもいいが単体でも使える
	/// </summary>
	/// <param name="layerNumber"></param>
	public void SetLayer(int layerNumber)
	{

		this.gameObject.layer = layerNumber;

	}

	/// <summary>
	/// 重力設定
	/// </summary>
	/// <param name="gravity"></param>
	public void GravitySet(float gravity)
	{
		rb.gravityScale = gravity;
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
	/// stopResを数えた後にisStop解除のメソッドを呼ぶ。isStop中はすべて止まる。硬直やダウンにどうぞ
	/// </summary>
	/// <param name="stopRes"></param>
	public void AllStop(float stopRes)
	{
		//stopTime += Time.fixedDeltaTime;
		SetLayer(initialLayer);
		isStop = true;
		nowJump = false;
		rb.velocity = Vector2.zero;
		Invoke("StopBreak", stopRes);
	}

	/// <summary>
	/// 停止解除。単体では使わない
	/// </summary>
	public void StopBreak()
	{
		SetLayer(17);
		isStop = false;
		rb.velocity = Vector2.zero;
	}



	/// <summary>
	/// ダウン状態のメソッド
	/// </summary>
	public void Down()
	{
		////Debug.log("吹き飛ぶ");

		if (isDown && rb.IsTouching(SManager.instance.sisStatus.filter))
		{
			////(SManager.instance.sisStatus.motionIndex["ダウン"]);
			GravitySet(SManager.instance.sisStatus.firstGravity);
			AllStop(1f);

			isDown = false;
			//ダウンアニメ
		}

	}


	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (isGround)
		{

			//トンネルの中をしゃがみトリガーで満たす
			if (collision.tag == squatTag && GManager.instance.pm.isSquat)
			{
				isSquat = true;
			}
			if (collision.tag == jumpTag && isGround)
			{
				//GetComponentはなるべくせぬように
				if (collision.gameObject.GetComponent<JumpTrigger>().jumpDirection == transform.localScale.x)
				{
					jumpTrigger = true;

				}
			}

		}
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (isGround)
		{
			//トンネルの中をしゃがみトリガーで満たす
			if (collision.tag == squatTag && GManager.instance.pm.isSquat)
			{
				isSquat = true;
			}
			if (collision.tag == jumpTag && isGround)
			{
				if (collision.gameObject.GetComponent<JumpTrigger>().jumpDirection == transform.localScale.x) 
				{
					jumpTrigger = true;

				}
			}
			/*else if (collision.tag == jumpTag && reJump)
			{
				jumpTrigger = true;
				jumpTime = 0.0f;
			}*/
		}
	}
	private void OnTriggerExit2D(Collider2D collision)
	{
		if (isGround)
		{
			if (collision.tag == squatTag && !GManager.instance.pm.isSquat)
			{
				isSquat = false;
			}
			if (collision.tag == jumpTag)
			{
				jumpTrigger = false;

			}
		}
	}

	/// <summary>
	/// 待機状態解除判断のためのメソッド
	/// </summary>
	public void WaitJudge()
    {
        if (isWait)
        {
			if(Mathf.Abs(basePosition.x) - Mathf.Abs(waitPosition.x) > Mathf.Abs(status.patrolDistance)/2 || basePosition.y != waitPosition.y)
            {
				isWait = false;
            }
        }
    }

	public void SisterFall()
    {
		/*if (!isGround && !isPosition && !isJump)
        {
			GravitySet(SManager.instance.sisStatus.firstGravity);
		}
        else
        {
			GravitySet(0);
		}*/
		if (!isGround)
		{
			GravitySet(SManager.instance.sisStatus.firstGravity);
			if (!isWarp) 
			{
				//("Fall"); 
			}
		}
		else
		{
			//GravitySet(0);
		}
	}



	/// <summary>
	/// ランダムに移動するメソッド
	/// </summary>
	public void Feint()
	{
		randomTime += Time.fixedDeltaTime;

		if (randomTime >= 3)
		{
			moveDirectionX = RandomValue(0, 100) >= 50 ? 1 : -1;
			moveDirectionY = RandomValue(0, 100) >= 50 ? -1 : 1;
			randomTime = 0.0f;
		}
		//move.Set(SManager.instance.sisStatus.addSpeed * (-SManager.instance.sisStatus.walkSpeed - rb.velocity.x) * moveDirectionX,0);
		rb.AddForce(move);

	}




	/// <summary>
	/// 戦闘時に逃げるワープ
	/// </summary>
	private void EscapeWarp()
    {
		if (!isJump)
		{
			if (!isWarp)
			{
				//転送後
				if ((Mathf.Abs(distance.x) >= SManager.instance.sisStatus.warpDistance.x || Mathf.Abs(distance.y) >= SManager.instance.sisStatus.warpDistance.y))// && GManager.instance.pm.isGround)
				{

					//プレイヤーがどっちにいるかで座標が分かれる
					if (baseDirection.x >= 0)
					{
						move.Set(basePosition.x - 6, basePosition.y);
						move.y = RayGroundCheck(move) + 10;
						transform.position = move;
						Flip(1);
					}
					else
					{
						move.Set(basePosition.x + 6, basePosition.y);
						move.y = RayGroundCheck(move) + 10;
						
						Flip(-1);
					}


					nowPosition = false;
					isPosition = false;
				//	SManager.instance.isEscape = true;
					//escapeがtrueの時警戒から戦闘にならない
					stateNumber = 0;
					beforeNumber = 0;
					reJudgeTime = 0;
					changeable = true;

					//("WarpPray");

					
					if (nowState == SisterState.戦い)
					{
						reJudgeTime = 0;
						nowState = SisterState.警戒;
						
						reJudgePositionTime = 0;


						SManager.instance.targetCondition.Clear();
						SManager.instance.target = null;
					}
					//EscapeWarp();
					//逃走ワープメソッドで元に戻す
				}
			}
            else
            {
				battleEndTime += Time.fixedDeltaTime;
				//("WarpPray");
				if (!CheckEnd("WarpPray") || battleEndTime >= 6)
                {

					battleEndTime = 0;
					isWarp = false;
                }

			}
		}
	}

	/// <summary>
	/// 戦闘終了
	/// </summary>
	private void BattleEnd()
    {

		//敵がいなかったら
		if (SManager.instance.isBattleEnd && !(SManager.instance.targetList.Count == 0))
		{
				battleEndTime += Time.fixedDeltaTime;

			//時間計測して警戒に移行
			 if (battleEndTime >= 2 && !isReset)
			     {

				isReset = true;
			//六秒以上敵検知せんずれば警戒フェイズへ

				}

		}
		else
		{
			isReset = false;
			SManager.instance.isBattleEnd = false;
			battleEndTime = 0.0f;
		}
	}
	public void WarpEffect()
    {
		Transform gofire = firePosition;

		//Transform rotate = SManager.instance.useMagic.castEffect.LoadAssetAsync<Transform>().Result as Transform;

		Addressables.InstantiateAsync("WarpCircle", gofire.position, gofire.rotation);//.Result;//発生位置をPlayer
		GManager.instance.PlaySound("Warp", transform.position);
	}

    public float RayGroundCheck(Vector2 position)
    {

		//真下にレイを飛ばしましょう
			RaycastHit2D onHitRay = Physics2D.Raycast(position, Vector2.down, Mathf.Infinity,layerMask.value);

		//  ////Debug.log($"{onHitRay.transform.gameObject}");
		////Debug.DrawRay(i_fromPosition,i_toTargetDir * SerchRadius);

		//Debug.Log($"当たったもの{onHitRay.transform.gameObject.name}");
		return onHitRay.point.y;
	}
	bool CheckEnd(string Name)
	{

		if (!sAni.GetCurrentAnimatorStateInfo(0).IsName(Name))// || sAni.GetCurrentAnimatorStateInfo(0).IsName("OStand"))
		{   // ここに到達直後はnormalizedTimeが"Default"の経過時間を拾ってしまうので、Resultに遷移完了するまではreturnする。
			return true;
		}
		if (sAni.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
		{   // 待機時間を作りたいならば、ここの値を大きくする。
			return true;
		}
		//AnimatorClipInfo[] clipInfo = sAni.GetCurrentAnimatorClipInfo(0);

		////Debug.Log($"アニメ終了");

		return false;

		// return !(sAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
		//  (_currentStateName);
	}

	public void StepSound(int type)
	{
		if (type == 0)
		{
			GManager.instance.PlaySound("LightFootStep", transform.position);
		}
		else
		{
			GManager.instance.PlaySound("LightWalkStep", transform.position);
		}
		if (GManager.instance.isWater)
		{
			GManager.instance.PlaySound("WaterStep", transform.position);
		}
	}
	public void AnimeSound(string useSoundName)
	{

		GManager.instance.PlaySound(useSoundName, transform.position);


	}
	public void AnimeChaise(string useSoundName)
	{

		GManager.instance.FollowSound(useSoundName, transform);

	}

/*	/// <summary>
	/// アニメイベント
	/// 詠唱完了時に一度呼ぶ
	/// 魔法レベルと属性で
	/// </summary>
	public void CallCastSound()
    {
		//待てこれキャストサウンド介する必要ある？
		//普通に判断して普通に入れよ
		GManager.instance.PlaySound("NormalCastEnd", transform.position);
	}*/




}



