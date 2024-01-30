using MoreMountains.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;
using Rewired.Integration.CorgiEngine;
namespace MoreMountains.CorgiEngine
{
	[SelectionBase]
	/// <summary>
	/// このクラスは、キャラクターのCorgiControllerコンポーネントを操縦します。
	/// 入力を改良するためのモノですが、Flipとかも変えてもいいかも
	/// ジャンプ、ダッシュ、シュートなど、キャラクターのすべてのゲームルールを実装する場所です。
	/// アニメーターのパラメーターは Grounded (bool), xSpeed (float), ySpeed (float), 
	/// CollidingLeft (bool), CollidingRight (bool), CollidingBelow (bool), CollidingAbove (bool), Idle (bool).
	/// 例えば、状態遷移に分散を追加するのに便利です。
	/// RandomConstant : 0から1000の間のランダムなint型。スタート時に生成され、アニメーターの寿命が尽きるまで一定に保たれます。
	/// 同じタイプのキャラクターを異なる挙動にするのに便利です。
	/// </summary>
	[AddComponentMenu("Corgi Engine/Character/Core/MyCharacter")]
	public class MyCharacter : Character
	{

		/// associated input manager
		public new RewiredCorgiEngineInputManager  LinkedInputManager { get; protected set; }


		protected  MyAbillityBase[] _myAbilities;

		[HideInInspector]
		//Idle状態になるのを禁止する
		//Idleだけじゃなく放置したらなるやつ全般にいいかも
		public bool banIdle = false;


		int abillityCount;

		
		public enum GroundFeature
        {
			Nomal,//普通
			Water,//水
			Grass,//草

        }

		/// <summary>
		/// 現在立っている地形の特徴
		/// 地面側から送るか？
		/// </summary>
		public GroundFeature nowGround = GroundFeature.Nomal;

		/// <summary>
		/// アビリティを取得し、さらに使用するためにキャッシュする。
		/// 実行時に能力を追加する場合は、必ずこれを呼び出します。
		/// 理想的には、実行時にコンポーネントを追加するのは避けたいところです。
		/// その代わりにコンポーネントを有効化/無効化するのが最善である。
		/// しかし、もし必要であれば、このメソッドを呼び出す。
		/// </summary>
		public override void CacheAbilities()
        {


			//①
			// 私たちは、私たちのレベルですべての能力をつかむ
			//CharacterAbilityを継承するものすべてをGet
		//	_characterAbilities = this.gameObject.GetComponents<CharacterAbility>();
			_myAbilities = this.gameObject.GetComponents<MyAbillityBase>();


			// ユーザがさらにノードを指定した場合
			if ((AdditionalAbilityNodes != null) && (AdditionalAbilityNodes.Count > 0))
            {
				// リストを作成します。
				//List<CharacterAbility> tempAbilityList = new List<CharacterAbility>();
				List<MyAbillityBase> abillityList = new List<MyAbillityBase>();

				abillityCount = _myAbilities.Length;

				// ①ですでに発見している能力をすべてリストにしました。
				for (int i = 0; i < abillityCount; i++)
                {
					//tempAbilityList.Add(_characterAbilities[i]);
				//	tempAbilityList.Add((CharacterAbility)_myAbilities[i]);
					abillityList.Add(_myAbilities[i]);
					//_myAbilities[i] = (MyAbillityBase)_characterAbilities[i];
				}



				int nodeCount = AdditionalAbilityNodes.Count;

				//ノードがちゃんとあるならノードアビリティ追加
				if (nodeCount > 0) 
				{

					// ノードのアビリティを追加する。
					for (int j = 0; j < nodeCount; j++)
					{
						
						MyAbillityBase[] tempArray = AdditionalAbilityNodes[j].GetComponentsInChildren<MyAbillityBase>();


						//全部加え切ってからでよくね
						//_myAbilities = abillityList.ToArray();

						abillityCount = tempArray.Length;
						for(int i = 0;i<abillityCount;i++)
						{
							//ノードアビリティ追加
							abillityList.Add(tempArray[i]);
						}
					}
				}

               _myAbilities = abillityList.ToArray();
				abillityCount = _myAbilities.Length;

				
				//これ一旦消してみよう
				//コーギーの純正アビリティとは決別する形で
				//_characterAbilities = Array.ConvertAll<MyAbillityBase, CharacterAbility>(_myAbilities, ReturnMyAbility);
				
				
			}

            _abilitiesCachedOnce = true;
        }

		private CharacterAbility ReturnMyAbility(MyAbillityBase ability)
        {
			return (CharacterAbility) ability;
        }

		/// <summary>
		/// キャラクターのプレイヤーIDに対応するInputManagerを取得します（存在する場合）。
		/// </summary>
		public override void SetInputManager()
		{
			if (CharacterType == CharacterTypes.AI)
			{
				LinkedInputManager = null;
				UpdateInputManagersInAbilities();
				return;
			}

			// we get the corresponding input manager
			if (!string.IsNullOrEmpty(PlayerID))
			{
				LinkedInputManager = null;
				InputManager[] foundInputManagers = FindObjectsOfType(typeof(RewiredCorgiEngineInputManager)) as RewiredCorgiEngineInputManager[];
				
				
				foreach (RewiredCorgiEngineInputManager foundInputManager in foundInputManagers)
				{
					if (foundInputManager.PlayerID == PlayerID)
					{
						LinkedInputManager = foundInputManager;
					}
				}
			}
			UpdateInputManagersInAbilities();
		}

		/// <summary>
		/// Sets a new input manager for this Character and all its abilities
		/// </summary>
		/// <param name="inputManager"></param>
		public override void SetInputManager(InputManager inputManager)
		{
			LinkedInputManager = (RewiredCorgiEngineInputManager)inputManager;
			UpdateInputManagersInAbilities();
		}

		/// <summary>
		/// Updates the linked input manager for all abilities
		/// </summary>
		protected override void UpdateInputManagersInAbilities()
		{
			if (_characterAbilities == null)
			{
				return;
			}
			for (int i = 0; i < _myAbilities.Length; i++)
			{
				_myAbilities[i].SetInputManager(LinkedInputManager);
			}
		}

		/// <summary>
		/// Resets the input for all abilities
		/// </summary>
		public override void ResetInput()
		{
			if (_characterAbilities == null)
			{
				return;
			}

			for(int i = 0; i < abillityCount; i++)
			{
				_myAbilities[i].ResetInput();
			}
		}

		/// <summary>
		/// Sets the player ID
		/// </summary>
		/// <param name="newPlayerID">New player ID.</param>
		public override void SetPlayerID(string newPlayerID)
		{
			PlayerID = newPlayerID;
			SetInputManager();
		}


		/// <summary>
		/// Calls all registered abilities' Early Process methods
		/// インプットもここ
		/// </summary>
		protected override void EarlyProcessAbilities()
		{
			//Debug.Log($"dddアビリティのながさ{_myAbilities.Length}コーギーアビリティの長さ{_characterAbilities.Length}");
			//UI出てる時は操作不可
			if (MainUICon.instance.UIOn && CharacterType == CharacterTypes.Player)
			{
				return;
			}

            for (int i = 0; i < abillityCount; i++)
            {


		//		Debug.Log("asdf");
				if (_myAbilities[i].enabled && _myAbilities[i].AbilityInitialized)
				{
				//	Debug.Log("af");
					_myAbilities[i].EarlyProcessAbility();
				}
			}
		}

		/// <summary>
		/// Calls all registered abilities' Process methods
		/// </summary>
		protected override void ProcessAbilities()
		{
			//	Debug.Log($"sssss{_myAbilities[i].GetType()}");

			//UI出てる時は操作不可
			if (MainUICon.instance.UIOn && CharacterType == CharacterTypes.Player)
			{
				return;
			}

            for (int i = 0; i < abillityCount; i++)
            {
				if (_myAbilities[i].enabled && _myAbilities[i].AbilityInitialized)
				{
					_myAbilities[i].ProcessAbility();
				}
			}

		}

		/// <summary>
		/// Calls all registered abilities' Late Process methods
		/// </summary>
		protected override void LateProcessAbilities()
		{
			//UI出てる時は操作不可
			if (MainUICon.instance.UIOn && CharacterType == CharacterTypes.Player)
			{
				return;
			}


            for (int i = 0; i < abillityCount; i++)
            {
				if (_myAbilities[i].enabled && _myAbilities[i].AbilityInitialized)
				{
					_myAbilities[i].LateProcessAbility();
				}
			}
		}

		/// <summary>
		/// We do this every frame. This is separate from Update for more flexibility.
		/// </summary>
		protected override void EveryFrame()
		{
			HandleCharacterStatus();

			// we process our abilities
			EarlyProcessAbilities();

			if (Time.timeScale != 0f)
			{
				ProcessAbilities();
				LateProcessAbilities();
				HandleCameraTarget();
			}

			// we send our various states to the animator.		 
			UpdateAnimators();
			RotateModel();
		}



		/// <summary>
		/// This is called at Update() and sets each of the animators parameters to their corresponding State values
		/// </summary>
		protected override void UpdateAnimators()
		{
			if ((UseDefaultMecanim) && (_animator != null))
			{
				MMAnimatorExtensions.UpdateAnimatorBool(_animator, _groundedAnimationParameter, _controller.State.IsGrounded, _animatorParameters, PerformAnimatorSanityChecks);
				MMAnimatorExtensions.UpdateAnimatorBool(_animator, _airborneSpeedAnimationParameter, Airborne, _animatorParameters, PerformAnimatorSanityChecks);
				MMAnimatorExtensions.UpdateAnimatorBool(_animator, _aliveAnimationParameter, (ConditionState.CurrentState != CharacterStates.CharacterConditions.Dead), _animatorParameters, PerformAnimatorSanityChecks);
				MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _xSpeedSpeedAnimationParameter, _controller.Speed.x, _animatorParameters, PerformAnimatorSanityChecks);
				MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _ySpeedSpeedAnimationParameter, _controller.Speed.y, _animatorParameters, PerformAnimatorSanityChecks);
				MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _worldXSpeedSpeedAnimationParameter, _controller.WorldSpeed.x, _animatorParameters, PerformAnimatorSanityChecks);
				MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _worldYSpeedSpeedAnimationParameter, _controller.WorldSpeed.y, _animatorParameters, PerformAnimatorSanityChecks);
				MMAnimatorExtensions.UpdateAnimatorBool(_animator, _collidingLeftAnimationParameter, _controller.State.IsCollidingLeft, _animatorParameters, PerformAnimatorSanityChecks);
				MMAnimatorExtensions.UpdateAnimatorBool(_animator, _collidingRightAnimationParameter, _controller.State.IsCollidingRight, _animatorParameters, PerformAnimatorSanityChecks);
				MMAnimatorExtensions.UpdateAnimatorBool(_animator, _collidingBelowAnimationParameter, _controller.State.IsCollidingBelow, _animatorParameters, PerformAnimatorSanityChecks);
				MMAnimatorExtensions.UpdateAnimatorBool(_animator, _collidingAboveAnimationParameter, _controller.State.IsCollidingAbove, _animatorParameters, PerformAnimatorSanityChecks);
				MMAnimatorExtensions.UpdateAnimatorBool(_animator, _idleSpeedAnimationParameter, (MovementState.CurrentState == CharacterStates.MovementStates.Idle && !banIdle), _animatorParameters, PerformAnimatorSanityChecks);
				MMAnimatorExtensions.UpdateAnimatorBool(_animator, _facingRightAnimationParameter, IsFacingRight, _animatorParameters);

				UpdateAnimationRandomNumber();
				MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _randomAnimationParameter, _animatorRandomNumber, _animatorParameters, PerformAnimatorSanityChecks);

                for (int i = 0; i < abillityCount; i++)
                {
					if (_myAbilities[i].enabled && _myAbilities[i].AbilityInitialized)
					{
						_myAbilities[i].UpdateAnimator();
					}
				}
			}
		}

		/// <summary>
		/// Flips the character and its dependencies (jetpack for example) horizontally
		/// </summary>
		public override void Flip(bool IgnoreFlipOnDirectionChange = false)
		{
		//	Debug.Log($"すし{GManager.instance.shit}");
			// if we don't want the character to flip, we do nothing and exit
			if (!FlipModelOnDirectionChange && !RotateModelOnDirectionChange && !IgnoreFlipOnDirectionChange)
			{
				return;
			}
	//		Debug.Log($"し");
			if (!CanFlip)
			{
				return;
			}
		//	Debug.Log($"お");
			if (!FlipModelOnDirectionChange && !RotateModelOnDirectionChange && IgnoreFlipOnDirectionChange)
			{
				if (CharacterModel != null)
				{
					//
					CharacterModel.transform.localScale = Vector3.Scale(CharacterModel.transform.localScale, ModelFlipValue);
				}
				else
				{
					// if we're sprite renderer based, we revert the flipX attribute
					if (_spriteRenderer != null)
					{
						_spriteRenderer.flipX = !_spriteRenderer.flipX;
					}
				}
			}
			//Debug.Log($"おすし");
			// Flips the character horizontally
			FlipModel();

			if (_animator != null)
			{
				MMAnimatorExtensions.SetAnimatorTrigger(_animator, _flipAnimationParameter, _animatorParameters, PerformAnimatorSanityChecks);
			}
			//右向いてるフラグを反転。
			//右向いてるフラグは最初に右向いてるかどうかを検査して値を決めて以降は反転で処理する。
			IsFacingRight = !IsFacingRight;

			// we tell all our abilities we should flip
			for(int i = 0; i < abillityCount; i++)
			{
				if (_myAbilities[i].enabled)
				{
					_myAbilities[i].Flip();
				}
			}
		}



		/// <summary>
		/// スタンした時に呼ばれる
		/// アビリティを停止させる
		/// 
		/// 必要そうなのは
		/// ・ジャンプ
		/// ・しゃがみ
		/// ・走り
		/// ・攻撃
		/// ・シスターさんの攻撃
		/// ・敵AI
		/// ・敵攻撃
		/// ・ガード
		/// </summary>
		public void StunStopAbillity()
		{
            for (int i = 0; i < abillityCount; i++)
            {
                if (_myAbilities[i].enabled && _myAbilities[i].AbilityInitialized)
                {
                    _myAbilities[i].StopAbillity();
                }
            }
        }

	}
}