using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
	///このクラスでプレイヤーは攻撃時に前進する
	///攻撃開始から何秒後に何秒間移動するみたいな感じ
	///レイキャストで障害物を確認したら止まるか敵を押す
	///外部から移動距離を指定したりもできる（ロックオンで敵の位置拾う）
	///接触時は停止か押して前進かの二つを選べる
	///押して前進中は速度が一定になり、予定していた時間で止まる
	///落下攻撃用に距離無制限で進み続けるようなオプションも
	///コライダーのサイズをレイキャスト距離に足せ
	///ガードはいらない
	///「「「「「ルートオブジェクトにつけろ」」」」」
	///簡単に言うと攻撃中前進する機能、衝突する状態で他人や自分を停止させるコードがある、といった感じ
	///
	[AddComponentMenu("Corgi Engine/Character/Abilities/MyAttackMove")]
	public class MyAttackMove : MyAbillityBase
	{
		public override string HelpBoxText() { return "This component allows your character to push blocks. This is not a mandatory component, it will just override CorgiController push settings, and allow you to have a dedicated push animation."; }

		 enum RushState
        {
			停止,
			待機,
			移動
        }

		public enum AttackContactType
        {
		　　通過,//通り抜ける。接触ない
		  　停止,//敵と接触したら止まる
		   押す//敵を押して進んでいく
        }


		/// if this is true, the Character will only be able to push objects while grounded
		[Tooltip("この値が true の場合、キャラクタは接地している間のみオブジェクトを押すことができます。")]
		public bool PushWhenGroundedOnly = false;

		/// the length of the raycast used to detect if we're colliding with a pushable object. Increase this if your animation is flickering.
		[Tooltip("押せるオブジェクトと衝突しているかどうかを検出するために使用するレイキャストの長さ。アニメーションがちらつく場合は、この値を大きくしてください。")]
		public float DetectionRaycastLength = 5f;

		/// <summary>
		/// これが真なら押せない
		/// </summary>
		[SerializeField]
		protected bool _cantPush;

		public bool Pushable
        {
            get { return _cantPush; }
        }

		///外部から設定するやつ
        #region
        /// <summary>
        /// 突進中敵とぶつかった時どんな対応をするか
        /// </summary>
        [HideInInspector]
		public AttackContactType _contactType;

		/// <summary>
		/// 何秒後に移動開始するか
		/// </summary>
		[HideInInspector]
		public float _moveStartTime;
		/// <summary>
		/// 何秒間移動するか
		/// </summary>
		[HideInInspector]
		public float _moveDuration;


		/// <summary>
		/// 落下攻撃で無制限に衝突と前進するフラグ
		/// 距離制限なく進む
		/// </summary>
		bool fallMove;


        #endregion

        protected Vector3 _raycastDirection;
		protected Vector3 _raycastOrigin;



		/// <summary>
		/// 今何か押してるかどうか
		/// </summary>
		bool pushNow;

		/// <summary>
		/// 移動時間計測
		/// この時間が経過するか、
		/// </summary>
		float _moveTime;




		/// <summary>
		/// 何秒かけてどれくらいの距離移動するかで割り出した速度
		/// </summary>
		float _moveSpeed;

		/// <summary>
		/// 現在の突進の状態
		/// </summary>
		RushState nowState;

		[SerializeField]
		LayerMask HitObjectMask;

		/// <summary>
		/// 維持する距離
		/// 衝突するとこの距離の分だけ自分が離れる
		/// </summary>
		float keepDistance;

		/// <summary>
		/// 押す対象のこれ
		/// </summary>
		MyAttackMove pushObject;



		/// <summary>
		/// 今接触してるかどうか
		/// </summary>
		bool contactNow;

		/// <summary>
		/// ガード時とかのここから動かないという地点
		/// </summary>
		float anchorPosition;


		Vector2 posi = new Vector2();

		/// <summary>
		/// アナザーロックでロックされた側が右側にいるかどうか
		/// </summary>
		bool isRightLock;

		/// <summary>
		/// 第三者により行動を制限されている場合
		/// 目の前にガードして停止している敵がいたり
		/// </summary>
		public bool anotherLock;

		/// <summary>
		/// On Start(), we initialize our various flags
		/// </summary>
		protected override void Initialization()
		{
			base.Initialization();
		//	HitObjectMask |= _controller.PlatformMask;
			
		}

		/// <summary>
		/// Every frame we override parameters if needed and cast a ray to see if we're actually pushing anything
		/// </summary>
		public override void ProcessAbility()
		{
			base.ProcessAbility();

			if (!AbilityAuthorized)
			{
				return;
			}


			//衝突行動じゃないときは無効
			//怯んでる時とかも無効に
			if ((_movement.CurrentState != CharacterStates.MovementStates.Attack && _movement.CurrentState != CharacterStates.MovementStates.Guard
				&& _movement.CurrentState != CharacterStates.MovementStates.GuardMove) || _condition.CurrentState == CharacterStates.CharacterConditions.Stunned ||
				_condition.CurrentState == CharacterStates.CharacterConditions.Dead)
			{
				if(pushObject != null)
                {

					//ロック解除して衝突オブジェクトを捨てる
					pushObject.anotherLock = false;
					pushObject = null;
					contactNow = false;
					anchorPosition = transform.position.x;

					//押すのもやめる
					pushNow = false;
				}
                if (nowState != RushState.停止)
                {
					StopRush();
                }
                if (anotherLock)
                {
				//	Debug.Log($"あいいいい{isRightLock}");
					//怯んだりしたら解除
					if (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
                    {
						anotherLock = false;
						return;
					}
					//右にいて後退か、左にいて前進か
					if((isRightLock && _controller.Speed.x > 0) || (!isRightLock && _controller.Speed.x < 0))
                    {
						anotherLock = false;
						return;
					}

					posi.Set(anchorPosition, transform.position.y);
						transform.position = posi;
					//_controller.SetHorizontalForce(0);

				}

				return;
			}


			//突進機能部分
			AttackRush();

			//障害物検知部分
			CollideObjectCheck();





		}

		/// <summary>
		/// RushStateを外部から変えないと動かない
		/// 移動処理をどう書くか
		/// </summary>
		 void AttackRush()
        {
			//Debug.Log($"a{nowState}");
			if (nowState != RushState.停止)
			{
			//	Debug.Log($"b{(_moveTime)}ggg{_moveStartTime}{nowState}結果{_moveTime >= _moveStartTime && nowState == RushState.待機}");
				//b0.0893481　0.73　待機　結果False
				_moveTime += _controller.DeltaTime;

				//移動開始時間が来たら移動開始
				if (_moveTime >= _moveStartTime && nowState == RushState.待機)
				{

					nowState = RushState.移動;
					_moveTime = 0;
				}
				else if (nowState == RushState.移動)
				{
					//落下攻撃中じゃなくて移動する期間超えたら停止
					//落下攻撃中で地面についても終わり
					if ((!fallMove && _moveTime >= _moveDuration) || (fallMove && _controller.State.IsGrounded))
					{
						StopRush();
					}
					//突進中の処理
					else
					{

						//押してる時の処理
						//押してる相手のポジションを操作
						if (pushNow)
						{

							_raycastDirection = _controller.Speed.x > 0 ? transform.right : -transform.right;
							_raycastOrigin = _controller.ColliderCenterPosition + _raycastDirection * (_controller.Width() / 2);
							RaycastHit2D hit = MMDebug.RayCast(_raycastOrigin, _raycastDirection, keepDistance, _controller.PlatformMask, Color.green, _controller.Parameters.DrawRaycastsGizmos);

							//壁に当たるまでは押して進む
							//あと相手が吹き飛ばされていないなら
                            if (hit.collider == null && !BlowNow())
                            {
								
								posi.Set(_raycastOrigin.x + (pushObject.ReturnSizeX() / 2), pushObject.transform.position.y);
									transform.position = posi;
								//_controller.SetHorizontalForce(0);
							}
                            else if(hit.collider != null)
                            {
								
								StopRush();
								return;
                            }
						}

						int direction = _character.IsFacingRight ? 1 : -1;
					//	Debug.Log("ｆｆｆ");
						//ここから移動処理
						//_controller.SetHorizontalForce(1000000000000000000 * direction);
						_controller.SetHorizontalForce(_moveSpeed * direction);
					}
				}
			}
        }





		/// <summary>
		/// Stops the character from pushing or pulling
		/// </summary>
		protected virtual void StopRush()
		{
			_controller.SetHorizontalForce(0);
			nowState = RushState.停止;
			_moveTime = 0;
			anchorPosition = transform.position.x;
		}

		/// <summary>
		/// レイキャストを飛ばして衝突しているかどうか確認する。
		/// 進行方向と現在のスピードで左右のどちらに出すかを確認
		/// 衝突したら停止か押すかを選ぶ
		/// まずはレイキャストで衝突確認する作業から
		/// </summary>
		public void CollideObjectCheck()
		{

			//通過突進の時は処理を行わない
			if (nowState == RushState.移動 && _contactType == AttackContactType.通過)
			{
                if (pushObject != null)
                {
					pushObject.anotherLock = false;
					pushObject = null;
				}
				return;
			}
				// we set our flag to false

				// 速度がある時、プッシュ可能なオブジェクトと衝突しているかどうかを確認するために前方に光線を投射します。

				//止まってる時はむいてる方、動いてる時は動いてる方にレイを
				if (_controller.Speed.x == 0)
			{
				_raycastDirection = _character.IsFacingRight ? transform.right : -transform.right;
			}
			else
			{
				_raycastDirection = _controller.Speed.x > 0 ? transform.right : -transform.right;
			}

			//自分のコライダーの先端から光線を飛ばしてる
			_raycastOrigin = _controller.ColliderCenterPosition + _raycastDirection * (_controller.Width() / 2);


			// we cast our ray to see if we're hitting something
			//何か当たってるか確認するため光線を当てる
			//このHitObjectマスクを地形に当たらないようにしないと地形を検出してロックされ続ける
			RaycastHit2D hit = MMDebug.RayCast(_raycastOrigin, _raycastDirection, DetectionRaycastLength, HitObjectMask, Color.green, _controller.Parameters.DrawRaycastsGizmos);

			//何かしらにヒットしたら
			if (hit)
			{


				//衝突なうじゃない時や衝突オブジェクトが変わった時
				if (!contactNow || hit.collider.gameObject != pushObject.gameObject)
				{

					//初接触時に初期化処理を
					if (hit.collider.gameObject.MMGetComponentNoAlloc<CorgiController>() != null)
					{
						//ロック解除してあげてから入れ替える
						if (pushObject != null)
						{
							pushObject.anotherLock = false;
						}
						pushObject = hit.collider.gameObject.MMGetComponentNoAlloc<MyAttackMove>();
						keepDistance = pushObject.ReturnSizeX() + DetectionRaycastLength;
						contactNow = true;
						anchorPosition = transform.position.x;
					}
					
				}
				//ヒットした物にコントローラーがついていたら
				if (contactNow)
				{
					//攻撃の移動ですり抜けないようにする処理
					//rushNow中限定
					if (nowState == RushState.移動)
					{

						//攻撃の接触タイプが停止ならラッシュやめる
						if (_contactType == AttackContactType.停止)
						{
							StopRush();
						}
						//押すなら押し始める
						else if (_contactType == AttackContactType.押す)
						{
							//取得したオブジェクトが押せるなら
							if (!pushObject.Pushable)
							{
								pushNow = true;
							}
							//押せないなら停止
                            else
                            {
								StopRush();
							}
						}
					}

					//ガードやガード移動、突進せずに攻撃してる時の処理
					else
					{
						//衝突した相手が衝突判定を出すような行動をしていないとき
						//相手を一方向にロックする
						//もし衝突判定が出る行動をしているなら自分は自分で停止する。
                        if (pushObject.ReturnNowCollide())
						{
						//	Debug.Log($"あｄｄｄｄ");
							//相手が自分より右にいるなら
							bool isRight = transform.position.x < pushObject.transform.position.x;

							pushObject.AnotherLock(isRight);
                        }


						if(_controller.Speed.x != 0)
                        {
							//右向いてる時右、左向いてる時に左に進んでたら位置を動かないように
                            if ((_raycastDirection.x > 0 && _controller.Speed.x > 0) ||
								(_raycastDirection.x < 0 && _controller.Speed.x < 0))
                            {
								posi.Set(anchorPosition, transform.position.y);
								transform.position = posi;
								//_controller.SetHorizontalForce(0);
							}
							
                        }
						//自分も止まってるなら相手も止まってるのでelseはいらない


					}
					

				}
			}
			//相手にコントローラーなくて突進中じゃないなら単純に突進とめるだけ
			//Platform相手ならそもそも動かない
			else if(contactNow)
			{
				//衝突オブジェクトがNullじゃないなら

				//ロック解除して衝突オブジェクトを捨てる
				pushObject.anotherLock = false;
				pushObject = null;
				contactNow = false;
				anchorPosition = transform.position.x;

				//押すのもやめる
				pushNow = false;
			}
		}


		/// <summary>
		///  突進開始
		/// 必要な情報を初期化
		/// 外部から呼び出す
		/// 移動する距離は計算に使う
		/// 何秒かけてどれくらいの距離移動するか、でだいたいの速度を出せる
		/// </summary>
		/// <param name="duration">移動する時間の長さ</param>
		/// <param name="distance">移動する距離。ロックオンした敵との距離を入れてもいい</param>
		/// <param name="_type">敵と接触した時の対応</param>
		/// <param name="infinityMove">落下攻撃で無限に横移動し続けるかどうか</param>
		/// <param name="startTime">移動を開始するまでの時間</param>
		public void RushStart(float duration, float distance, AttackContactType _type,bool fallAttack = false, float startTime = 0)
        {

			if (!fallAttack)
			{
				nowState = RushState.待機;
				_moveDuration = duration;
				_moveStartTime = startTime;
				_contactType = _type;
				_moveSpeed = distance / _moveDuration;
				fallMove = false;
			}
            else
            {
				nowState = RushState.待機;
				_moveDuration = duration;
				_moveStartTime = startTime;
				_contactType = AttackContactType.通過;
				_moveSpeed = distance / _moveDuration;
				fallMove = true;
			}
		//	Debug.Log($"きたわね{_moveSpeed}");
		}


        #region//他のオブジェクトに情報を与えるメソッド

        /// <summary>
        /// 突進中と突進中がぶつかって押し合う時の処理は考えない、いまは
        /// </summary>
        /// <returns></returns>
        public int ReturnRushState()
        {
			return (int)nowState;
        }

		/// <summary>
		/// 判断に必要な情報をまとめて吟味して結果だけ教えてくれる
		/// 外部から読み取る
		/// </summary>
		public float ReturnSizeX()
		{
			return _controller.Width();
		}

		/// <summary>
		/// 真なら衝突するような行動はしていない
		/// </summary>
		/// <returns></returns>
		public bool ReturnNowCollide()
        {
			return (_movement.CurrentState != CharacterStates.MovementStates.Attack && _movement.CurrentState != CharacterStates.MovementStates.Guard
				&& _movement.CurrentState != CharacterStates.MovementStates.GuardMove);
        }

		/// <summary>
		/// 外部からのロック
		/// </summary>
		public void AnotherLock(bool isRight)
        {
			anotherLock = true;
			isRightLock = isRight;
			anchorPosition = transform.position.x;
		}
		public CharacterStates.MovementStates Returntest()
		{
			return _movement.CurrentState;
		}


		/// <summary>
		/// 今吹き飛ばされているか
		/// </summary>
		/// <returns></returns>
		public bool BlowNow()
        {
			return _health._blowNow;
        }
        #endregion
    }

}
