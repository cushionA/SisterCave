namespace Rewired.Integration.CorgiEngine {
    using UnityEngine;
    using MoreMountains.CorgiEngine;
    using MoreMountains.Tools;

    /// <summary>
	/// この永続的なシングルトンは、入力を処理し、プレーヤーにコマンドを送信します。
	/// 重要：このスクリプトの実行順序は-100でなければならない。
	/// スクリプトの実行順序は、スクリプトのファイルをクリックし、スクリプトのインスペクタの右下にある実行順序ボタンをクリックすることで定義できます。
	/// See https://docs.unity3d.com/Manual/class-ScriptExecution.html for more details
	/// </summary>
    [AddComponentMenu("Corgi Engine/Managers/Rewired Input Manager")]
    public class RewiredCorgiEngineInputManager : AlterInputManager {

        private const string rewiredSystemPauseActionName = "SystemPause";
        private bool _initialized;
        private Rewired.Player _rewiredPlayer;
        private int _rewiredActionId_horizontal;
        private int _rewiredActionId_vertical;

        private int _rewiredActionId_siteHorizontal;
        private int _rewiredActionId_siteVertical;
        private int _rewiredActionId_UIHorizontal;
        private int _rewiredActionId_UIVertical;
        private int[] _rewiredButtonIds;
        private int _rewiredSystemPauseButtonId;

        public Vector2 UIMovement { get { return _UIMovement; } }

        public Vector2 SiteMovement { get { return _siteMovement; } }

        /// <summary>
        /// これが真なら入力されてる。
        /// </summary>
        bool _inputCheck;


        /// <summary>
        /// スタート時に使用するモードを探し、軸とボタンを初期化します。
        /// </summary>
        protected override void Start() {
            base.Start();

            if(!ReInput.isReady) {
                Debug.LogError("Rewired: Rewired was not initialized. Setup could not be performed. A Rewired Input Manager must be in the scene and enabled. Falling back to default input handler.");
                return;
            }

            // Get the Rewired Id based on the PlayerID string
            _rewiredPlayer = ReInput.players.GetPlayer(PlayerID);
            if(_rewiredPlayer == null) {
                Debug.LogError("Rewired: No Rewired Player was found for the PlayerID string \"" + PlayerID + "\". Falling back to default input handler.");
                return;
            }

            _initialized = true;
        }

        /// <summary>
        /// ボタンを初期化します。ボタンを増やしたい場合は、必ずベースとなるInputManagerクラスのInintializeButtonsメソッドに登録してください。
        /// </summary>
        protected override void InitializeButtons() {
            base.InitializeButtons();
            if(!ReInput.isReady) return;

            // Rewired Action Id を文字列ではなく整数値でキャッシュすることで高速化を図りました。
            _rewiredButtonIds = new int[ButtonList.Count];
            for(int i = 0; i < _rewiredButtonIds.Length; i++) _rewiredButtonIds[i] = -1; // init to invalid
            for(int i = 0; i < _rewiredButtonIds.Length; i++) {
                string actionName = StripPlayerIdFromActionName(ButtonList[i].ButtonID); 
                if (string.IsNullOrEmpty(actionName)) continue;
                _rewiredButtonIds[i] = ReInput.mapping.GetActionId(actionName);
          //      Debug.Log($"test{actionName}{_rewiredButtonIds[i]}");
                // Find the Shoot action so we can reuse it instead of ShootAxis
                //ShootAxisの代わりにShootアクションを再利用できるように検索します。

            }
            _rewiredSystemPauseButtonId = ReInput.mapping.GetActionId(rewiredSystemPauseActionName);
        }

        /// <summary>
		/// 軸のIDを初期化します。
		/// </summary>
		protected override void InitializeAxis() {


            _axisHorizontal = "MoveHorizontal";
             _axisVertical = "MoveVertical";
         //   _axisSecondaryHorizontal = "_SecondaryHorizontal";
        //    _axisSecondaryVertical = "_SecondaryVertical";
           // _axisShoot = "_ShootAxis";
         //   _axisShootSecondary = "_SecondaryShootAxis";
            SiteVertical = "SiteVertical";
            SiteHorizontal = "SiteHorizontal";
            UIVertical = "UIVertical";
            UIHorizontal = "UIHorizontal";

            if (!ReInput.isReady) return;
           
            // Cache the Rewired Action Id integers instead of using strings for speed
            _rewiredActionId_horizontal = ReInput.mapping.GetActionId(StripPlayerIdFromActionName(_axisHorizontal));
            _rewiredActionId_vertical = ReInput.mapping.GetActionId(StripPlayerIdFromActionName(_axisVertical));
            //_rewiredActionId_secondaryHorizontal = ReInput.mapping.GetActionId(StripPlayerIdFromActionName(_axisSecondaryHorizontal));
            //_rewiredActionId_secondaryVertical = ReInput.mapping.GetActionId(StripPlayerIdFromActionName(_axisSecondaryVertical));
            _rewiredActionId_siteHorizontal = ReInput.mapping.GetActionId(StripPlayerIdFromActionName(SiteHorizontal));
            _rewiredActionId_siteVertical = ReInput.mapping.GetActionId(StripPlayerIdFromActionName(SiteVertical));
            _rewiredActionId_UIVertical = ReInput.mapping.GetActionId(StripPlayerIdFromActionName(UIVertical));
            _rewiredActionId_UIHorizontal = ReInput.mapping.GetActionId(StripPlayerIdFromActionName(UIHorizontal));
        }

        /// <summary>
	    /// アップデート時には、各種コマンドを確認し、それに応じて値や状態を更新します。
	    /// </summary>
	    protected override void Update() {
            if(!_initialized) {
                base.Update();
                return;
            }
            SetMovement();
            //  SetSecondaryMovement();
            //  SetShootAxis();
            if (MainUICon.instance.UIOn) 
            {
                SetUIMovement();
            }
            GetInputButtons();
          
        }

        /// <summary>
        /// _inputCheckを初期化する
        /// </summary>
        protected override void LateUpdate()
        {
            base.LateUpdate();
            _inputCheck = false;
        }

        /// <summary>
        /// 入力の変化を監視し、それに応じてボタンの状態を更新する。
        /// </summary>
        protected override void GetInputButtons() 
        {
            if(!_initialized) {
                base.GetInputButtons();
                return;
            }
            //ボタンが一個ずつ押されてるか確認中
            for(int i = 0; i < _rewiredButtonIds.Length; i++) 
            {
     //           Debug.Log($"でえええ{_rewiredButtonIds[i]}");
                if (_rewiredPlayer.GetButton(_rewiredButtonIds[i])) {

                    //Debug.Log("push");
                    ButtonList[i].TriggerButtonPressed();
                    _inputCheck = true;
                }
                if(_rewiredPlayer.GetButtonDown(_rewiredButtonIds[i])) {
                    ButtonList[i].TriggerButtonDown();
                    _inputCheck = true;
                }
                if(_rewiredPlayer.GetButtonUp(_rewiredButtonIds[i])) {
                    ButtonList[i].TriggerButtonUp();
                }
            }


            // Special handling for System Pause
            // Allow the System Player to trigger Pause on all players so the key
            // only has to be mapped to one fixed key and the assignment can be protected.
            Rewired.Player systemPlayer = ReInput.players.GetSystemPlayer();
            if(systemPlayer.GetButtonDown(_rewiredSystemPauseButtonId)) 
            {
                PauseButton.TriggerButtonDown();
            }
            if(systemPlayer.GetButtonUp(_rewiredSystemPauseButtonId)) {
                PauseButton.TriggerButtonUp();
            }
           // Debug.Log($"ccccccc{AvoidButton.State.CurrentState}");
            //   Debug.Log($"ddddddd{AvoidButton.State.CurrentState}");
        }

        /// <summary>
		/// LateUpdate()で呼び出され、登録された全てのボタンの状態を処理する。
		/// </summary>
		public override void ProcessButtonStates() {
            base.ProcessButtonStates();
            if(!_initialized) return;

            // Update the ShootAxis state which is separate from other buttons
            if(ShootAxis == MMInput.ButtonStates.ButtonDown) {
                ShootAxis = MMInput.ButtonStates.ButtonPressed;
            }
            if(ShootAxis == MMInput.ButtonStates.ButtonUp) {
                ShootAxis = MMInput.ButtonStates.Off;
            }
        }

        /// <summary>
        /// Called every frame, gets primary movement values from Rewired Player
        /// </summary>
        public override void SetMovement() {
            if(!_initialized) {
                base.SetMovement();
                return;
            }
            if(SmoothMovement) {

                _primaryMovement.x = _rewiredPlayer.GetAxis(_rewiredActionId_horizontal);
                _primaryMovement.y = _rewiredPlayer.GetAxis(_rewiredActionId_vertical);
              //  Debug.Log($"あああ{_primaryMovement.x}");
            } else {
               
                _primaryMovement.x = _rewiredPlayer.GetAxisRaw(_rewiredActionId_horizontal);
                _primaryMovement.y = _rewiredPlayer.GetAxisRaw(_rewiredActionId_vertical); 
                
            }
            //if(_primaryMovement)
        }



        /// <summary>
        /// Called every frame, gets secondary movement values from Rewired player
        /// </summary>
        public override void SetSiteMovement()
        {
            if (!_initialized)
            {
                base.SetSiteMovement();
                return;
            }
            if (SmoothMovement)
            {
                _siteMovement.x = _rewiredPlayer.GetAxis(_rewiredActionId_siteHorizontal);
                _siteMovement.y = _rewiredPlayer.GetAxis(_rewiredActionId_siteVertical);
            }
            else
            {
                _siteMovement.x = _rewiredPlayer.GetAxisRaw(_rewiredActionId_siteHorizontal);
                _siteMovement.y = _rewiredPlayer.GetAxisRaw(_rewiredActionId_siteVertical);
            }
        }


        /// <summary>
        /// Called every frame, gets secondary movement values from Rewired player
        /// </summary>
        public override void SetUIMovement()
        {
            if (!_initialized)
            {
                base.SetUIMovement();
                return;
            }
            if (SmoothMovement)
            {
                _UIMovement.x = _rewiredPlayer.GetAxis(_rewiredActionId_UIHorizontal);
                _UIMovement.y = _rewiredPlayer.GetAxis(_rewiredActionId_UIVertical);
            }
            else
            {
                _UIMovement.x = _rewiredPlayer.GetAxisRaw(_rewiredActionId_UIHorizontal);
                _UIMovement.y = _rewiredPlayer.GetAxisRaw(_rewiredActionId_UIVertical);
            }
        }



        /// <summary>
        /// This is not used.
        /// </summary>
        /// <param name="movement">Movement.</param>
        public override void SetMovement(Vector2 movement) {
            if(!_initialized) base.SetMovement(movement);
        }



        /// <summary>
        /// This is not used.
        /// </summary>
        /// <param name="">.</param>
        public override void SetHorizontalMovement(float horizontalInput) {
            if(!_initialized) base.SetHorizontalMovement(horizontalInput);
        }

        /// <summary>
        /// This is not used.
        /// </summary>
        /// <param name="">.</param>
        public override void SetVerticalMovement(float verticalInput) {
            if(!_initialized) base.SetVerticalMovement(verticalInput);
        }




        /// <summary>
        /// PlayerIDとアクション名を組み合わせた文字列から、アクション名を取得する。
        /// </summary>
        /// <param name="action">PlayerIDを前置詞としたアクション文字列</param>。
        /// <returns>PlayerIDを除いたアクションの文字列。
        private string StripPlayerIdFromActionName(string action) {
            if(string.IsNullOrEmpty(action)) return string.Empty;
            if(!action.StartsWith(PlayerID)) return action;
            return action.Substring(PlayerID.Length + 1); // strip PlayerID and underscore
        }

        /// <summary>
        /// Converts button input into MMInput.ButtonStates.
        /// </summary>
        /// <param name="player">The Rewired Player.</param>
        /// <param name="actionId">The Action Id.</param>
        /// <returns>Button state</returns>
        private static MMInput.ButtonStates GetButtonState(Rewired.Player player, int actionId) {
            MMInput.ButtonStates state = MMInput.ButtonStates.Off;
            if(player.GetButton(actionId)) state = MMInput.ButtonStates.ButtonPressed;
            if(player.GetButtonDown(actionId)) state = MMInput.ButtonStates.ButtonDown;
            if(player.GetButtonUp(actionId)) state = MMInput.ButtonStates.ButtonUp;
            return state;
        }

        /// <summary>
        /// Gets the Rewired Action Id for an Action name string.
        /// </summary>
        /// <param name="actionName">The Action name.</param>
        /// <param name="warn">Log a warning if the Action does not exist?</param>
        /// <returns>Returns the Action id or -1 if the Action does not exist.</returns>
        public static int GetRewiredActionId(string actionName, bool warn) {
            if(string.IsNullOrEmpty(actionName)) return -1;
            int id = ReInput.mapping.GetActionId(actionName);
            if(id < 0 && warn) Debug.LogWarning("No Rewired Action found for Action name \"" + actionName + "\"");
            return id;
        }

            /// <summary>
    /// 何かボタンがいじられてるかどうか
    /// </summary>
    /// <returns></returns>
    public bool CheckButtonUsing()
    {
        return (_inputCheck);
    }


    }
}