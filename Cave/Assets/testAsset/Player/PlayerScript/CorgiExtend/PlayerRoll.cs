using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using Rewired.Integration.CorgiEngine;
using DarkTonic.MasterAudio;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// このクラスをキャラクターに追加すると、表面に沿って「転がる」ことができるようになり、オプションで敵を通り抜け、方向を制御し続けることができるようになります。
    /// アニメーターパラメーター : ローリング、スタートローリング
    /// dashも混ぜるか？
    /// まぜずにやりとりで
    /// </summary>
 //   [AddComponentMenu("Corgi Engine/Character/Abilities/Character Roll")]
    public class PlayerRoll : CharacterAbility
    {
        /// This method is only used to display a helpbox text at the beginning of the ability's inspector
        public override string HelpBoxText() { return "Add this class to a character and it'll be able to 'roll' along surfaces, with options to go through enemies, and keep controlling direction."; }

        [Header("Roll")]

        /// 何秒転がる
		[Tooltip("ローリング時間")]
        public float RollDuration = 0.5f;
        /// the speed of the roll (a multiplier of the regular walk speed)
        [Tooltip("転がる速さが通常の歩く速さの何倍か")]
        public float RollSpeed = 3f;
        /// if this is true, horizontal input won't be read, and the character won't be able to change direction during a roll
        [Tooltip("trueの場合、水平方向の入力は読み込まれず、ロール中に方向を変えることはできません。")]
        public bool BlockHorizontalInput = false;
        /// if this is true, no damage will be applied during the roll, and the character will be able to go through enemies
        /// //このパラメーターがかかわる処理を見れば無敵処理がわかる
        [Tooltip("trueの場合、ロール中にダメージが与えられず、敵をスルーできるようになります。")]
        public bool PreventDamageCollisionsDuringRoll = false;

        //方向
        [Header("Direction")]

        /// the roll's aim properties
        [Tooltip("the roll's aim properties")]
        public MMAim Aim;
        /// the minimum amount of input required to apply a direction to the roll
        [Tooltip(" ロールに方向を与えるために必要な最小限の入力量")]
        public float MinimumInputThreshold = 0.1f;
        /// if this is true, the character will flip when rolling and facing the roll's opposite direction
        [Tooltip("これが真なら、キャラクターはロール時に反転し、ロールの反対側を向きます。")]
        public bool FlipCharacterIfNeeded = true;

        //これコルーチンなんだ
        public enum SuccessiveRollsResetMethods { Grounded, Time }

        [Header("Cooldown")]
        /// the duration of the cooldown between 2 rolls (in seconds)
        [Tooltip("次のローリングまでに必要な時間")]
        public float RollCooldown = 1f;

        [Header("Uses")]
        /// whether or not rolls can be performed infinitely
        [Tooltip("無限にローリングできるか")]
        public bool LimitedRolls = false;
        /// the amount of successive rolls a character can perform, only if rolls are not infinite
        [Tooltip("the amount of successive rolls a character can perform, only if rolls are not infinite")]
        [MMCondition("LimitedRolls", true)]
        public int SuccessiveRollsAmount = 1;
        /// the amount of rollss left (runtime value only), only if rolls are not infinite
        [Tooltip("ローリングの残り回数")]
        [MMCondition("LimitedRolls", true)]
        [MMReadOnly]
        public int SuccessiveRollsLeft = 1;
        /// when in time reset mode, the duration, in seconds, after which the amount of rolls left gets reset, only if rolls are not infinite
        [Tooltip("when in time reset mode, the duration, in seconds, after which the amount of rolls left gets reset, only if rolls are not infinite")]
        [MMCondition("LimitedRolls", true)]
        public float SuccessiveRollsResetDuration = 2f;

        protected float _cooldownTimeStamp = 0;
        protected Vector2 _rollDirection;
        protected bool _shouldKeepRolling = true;
        protected IEnumerator _rollCoroutine;
        protected float _lastRollAt = 0f;
        protected float _currentDirection;
        protected float _drivenInput;
        protected float _originalMultiplier = 1f;
        protected bool _originalInvulnerability = true;

        // animation parameters
        protected const string _rollingAnimationParameterName = "Rolling";
        protected int _rollingAnimationParameter;
        protected const string _startedRollingAnimationParameterName = "StartedRolling";
        protected int _startedRollingAnimationParameter;
        protected RewiredCorgiEngineInputManager ReInput;

        /// <summary>
        /// アクトロールフラグは走るアクションとかからオンにしてもらうやつで、オンにしてもらえないとローリング不可能
        /// </summary>
        [HideInInspector]
        public bool actRoll;

        int initialLayer;


        /// <summary>
        /// Initializes our aim instance
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            Aim.Initialization();
            SuccessiveRollsLeft = SuccessiveRollsAmount;
            ReInput = (RewiredCorgiEngineInputManager)_inputManager;
        }

        /// <summary>
        /// At the start of each cycle, we check if we're pressing the roll button. If we
        /// </summary>
        protected override void HandleInput()
        {
            //ボタンのさまざまな可能な状態：オフ（デフォルトのアイドル状態）、
            //ButtonDown（ボタンが初めて押された）、ButtonPressed（ボタンが押された）、ButtonUp（ボタンが離された）
            if (actRoll)
            {
                //こいつがカギ
                StartRoll();
                
            }
        }

        /// <summary>
        /// The second of the 3 passes you can have in your ability. Think of it as Update()
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();

            HandleAmountOfRollsLeft();
        }

        /// <summary>
        /// Causes the character to roll or dive (depending on the vertical movement at the start of the roll)
        /// </summary>
        public virtual void StartRoll()
        {
            if (!RollAuthorized())
            {
                return;
            }

            if (!RollConditions())
            {
                return;
            }
            actRoll = false;
            InitiateRoll();
        }

        /// <summary>
        /// このメソッドは、ロールの内部条件（ロール間のクールダウン、ロールの残り量）を評価し、ロールが実行できる場合はtrueを、そうでない場合はfalseを返す
        /// </summary>
        /// <returns></returns>
        public virtual bool RollConditions()
        {
            // if we're in cooldown between two rolls, we prevent roll
            if (_cooldownTimeStamp > Time.time)
            {
                return false;
            }

            // if we don't have rolls left, we prevent roll
            if (SuccessiveRollsLeft <= 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// ロール残量をリセットする条件を満たしているかどうかを確認する
        /// </summary>
        protected virtual void HandleAmountOfRollsLeft()
        {
            if ((SuccessiveRollsLeft >= SuccessiveRollsAmount) || (Time.time - _lastRollAt < RollCooldown))
            {
                return;
            }

            if (Time.time - _lastRollAt > SuccessiveRollsResetDuration)
            {
                SetSuccessiveRollsLeft(SuccessiveRollsAmount);
            }
        }

        /// <summary>
        /// 連続ローリングの残量をリセットする方法
        /// </summary>
        /// <param name="newAmount"></param>
        public virtual void SetSuccessiveRollsLeft(int newAmount)
        {
            SuccessiveRollsLeft = newAmount;
        }

        /// <summary>
        /// このメソッドは、ロールの外部条件（状態、他の能力）を評価し、ロールが実行できる場合はtrueを、そうでない場合はfalseを返す
        /// </summary>
        /// <returns></returns>
        public virtual bool RollAuthorized()
        {
            // if the roll action is enabled in the permissions, we continue, if not we do nothing
            if (!AbilityAuthorized
                || (!_controller.State.IsGrounded)
                || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
                || (_movement.CurrentState == CharacterStates.MovementStates.LedgeHanging)
                || (_movement.CurrentState == CharacterStates.MovementStates.Gripping))
                return false;

            return true;
        }

        /// <summary>
        /// ロール前の全パラメータを初期化し、ロール前のフィードバックをトリガーする。
        /// ここでスタミナ使おうか
        /// </summary>
		public virtual void InitiateRoll()
        {
            // we set its rolling state to true
            _movement.ChangeState(CharacterStates.MovementStates.Rolling);

            // we start our sounds
            PlayAbilityStartFeedbacks();

            // we initialize our various counters and checks
            _shouldKeepRolling = true;
            _cooldownTimeStamp = Time.time + RollCooldown;
            _lastRollAt = Time.time;
            _originalMultiplier = _characterHorizontalMovement.AbilityMovementSpeedMultiplier;
            if (PreventDamageCollisionsDuringRoll)
            {
                if (_health != null)
                {
                    _originalInvulnerability = _health.Invulnerable;
                    _health.Invulnerable = true;

                }
                _character.gameObject.layer = GManager.instance.avoidLayer;
//レイヤー変えんぞこれ
//                _controller.gameObject.layer = 
            }

            if (LimitedRolls)
            {
                SuccessiveRollsLeft -= 1;
            }

            ComputeRollDirection();
            CheckFlipCharacter();

            // we launch the boost corountine with the right parameters
            _rollCoroutine = RollCoroutine();
            StartCoroutine(_rollCoroutine);
        }

        /// <summary>
        /// 選択されたオプションに基づき、ロール方向を計算する
        /// </summary>
        protected virtual void ComputeRollDirection()
        {
            // we compute our direction
            if (_character.LinkedInputManager != null)
            {
                Aim.PrimaryMovement = _character.LinkedInputManager.PrimaryMovement;
                Aim.SecondaryMovement = _character.LinkedInputManager.SecondaryMovement;
            }

            Aim.CurrentPosition = this.transform.position;
            _rollDirection = Aim.GetCurrentAim();

            if (_rollDirection.magnitude < MinimumInputThreshold)
            {
                _rollDirection = _character.IsFacingRight ? Vector2.right : Vector2.left;
            }
            else
            {
                _rollDirection = _rollDirection.normalized;
            }

            _currentDirection = _rollDirection.x > 0f ? 1f : -1f;
        }

        /// <summary>
        /// Checks whether or not a character flip is required, and flips the character if needed
        /// </summary>
        protected virtual void CheckFlipCharacter()
        {
            // we flip the character if needed
            if (FlipCharacterIfNeeded && (Mathf.Abs(_rollDirection.x) > 0.05f))
            {
                if (_character.IsFacingRight != (_rollDirection.x > 0f))
                {
                    _character.Flip();
                }
            }
        }


        /// <summary>
        /// 時間経過とともにプレイヤーを方向転換させるコルーチン
        /// </summary>
        protected virtual IEnumerator RollCoroutine()
        {
            // if the character is not in a position where it can move freely, we do nothing.
            if (!AbilityAuthorized
                || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal))
            {
                yield break;
            }

            _characterHorizontalMovement.ReadInput = false;
            _characterHorizontalMovement.AbilityMovementSpeedMultiplier = RollSpeed;

            if (_animator != null)
            {
                //アニメーションのトリガーここで引いてる
                MMAnimatorExtensions.SetAnimatorTrigger(_animator, _startedRollingAnimationParameter,
                    _character._animatorParameters, _character.PerformAnimatorSanityChecks);
            }

            float rollStartedAt = Time.time;

            // we keep rolling until we've reached our target distance or until we get interrupted
            while ((Time.time - rollStartedAt < RollDuration)
                && _shouldKeepRolling
                && !_controller.State.TouchingLevelBounds
                && _movement.CurrentState == CharacterStates.MovementStates.Rolling)
            {
                if (!BlockHorizontalInput)
                {
                    _drivenInput = _horizontalInput;
                }

                bool gravityShouldReverseInput = false;
                if (_characterGravity != null)
                {
                    gravityShouldReverseInput = _characterGravity.ShouldReverseInput();
                }

                if (_drivenInput != 0f)
                {
                    _drivenInput = gravityShouldReverseInput ? -_drivenInput : _drivenInput;
                    _currentDirection = (_drivenInput < 0f) ? -1f : 1f;
                }

                //ここで移動させてる
                //他の物理機能も気になるね
                _characterHorizontalMovement.SetHorizontalMove(gravityShouldReverseInput ? -_currentDirection : _currentDirection);

                yield return null;
            }
            StopRoll();
        }

        /// <summary>
        /// Stops the roll coroutine and resets all necessary parts of the character
        /// </summary>
        public virtual void StopRoll()
        {
            _characterHorizontalMovement.ReadInput = true;
            _characterHorizontalMovement.AbilityMovementSpeedMultiplier = _originalMultiplier;

            if (PreventDamageCollisionsDuringRoll)
            {
                if (_health != null)
                {
                    //これでダメージ受けないようにしてる
                    //レイヤー変える処理入れよう
                    _health.Invulnerable = _originalInvulnerability;
                    _character.gameObject.layer = initialLayer;
                }
            }

            if (_rollCoroutine != null)
            {
                StopCoroutine(_rollCoroutine);
            }

            // we play our exit sound
            StopStartFeedbacks();
            PlayAbilityStopFeedbacks();

            // once the boost is complete, if we were rolling, we make it stop and start the roll cooldown
            if (_movement.CurrentState == CharacterStates.MovementStates.Rolling)
            {
                if (_controller.State.IsGrounded)
                {
                    _movement.ChangeState(CharacterStates.MovementStates.Idle);
                }
                else
                {
                    _movement.RestorePreviousState();
                }
            }
        }

        /// <summary>
        /// Adds required animator parameters to the animator parameters list if they exist
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {
            RegisterAnimatorParameter(_rollingAnimationParameterName, AnimatorControllerParameterType.Bool, out _rollingAnimationParameter);
            RegisterAnimatorParameter(_startedRollingAnimationParameterName, AnimatorControllerParameterType.Trigger, out _startedRollingAnimationParameter);
        }

        /// <summary>
        /// サイクルの最後に、アニメーターのローリング状態を更新します。
        /// </summary>
        public override void UpdateAnimator()
        {
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _rollingAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Rolling), _character._animatorParameters, _character.PerformAnimatorSanityChecks);
        }

        /// <summary>
        /// リセット能力では、行われたすべての変更をキャンセルします
        /// </summary>
        public override void ResetAbility()
        {
            base.ResetAbility();
            if (_condition.CurrentState == CharacterStates.CharacterConditions.Normal)
            {
                StopRoll();
            }
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _rollingAnimationParameter, false, _character._animatorParameters, _character.PerformAnimatorSanityChecks);
        }
    }
}
