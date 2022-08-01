using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine.UI;
using Rewired.Integration.CorgiEngine;
using Cysharp.Threading.Tasks;

namespace MoreMountains.InventoryEngine
{
    /// <summary>
    /// ゲームからインベントリを呼び出す方法の例です。
    /// 入力クラスとGUIマネージャクラスで処理することをお勧めします。
    /// こいつの処理パクって自分のを作る
    /// インベントリごとにスクリプトを分ける
    /// これはメインインベントリをボタンで開くためのコード
    /// これもしかするとインベントリを操作するためのコードなのかも
    /// どのインベントリとか関係なく全てのインベントリの操作や表示をつかさどる
    /// インベントリパネル関連だねこれ
    /// </summary>
    public class MainInventoryCon : MonoBehaviour, MMEventListener<MMInventoryEvent>
    {
        [Header("Targets")]
        [MMInformation("インベントリ コンテナ（インベントリを開いたり閉じたりするときにオン/オフする CanvasGroup）、メインの InventoryDisplay、および InventoryDisplay を開いたときにその下に表示されるオーバーレイをここにバインドします。", MMInformationAttribute.InformationType.Info, false)]
        /// インベントリ開閉ボタンを押したときに表示/非表示にしたい要素をすべて含むCanvasGroup
        /// あるキャンバスグループつきオブジェクトの下に要素をまとめる
        public CanvasGroup TargetInventoryContainer;
        /// インベントリの表示せってい
        public MyInventoryDisplay TargetInventoryDisplay;
        /// インベントリを開く/閉じるときにその下で使用されるフェーダー（？）
        public CanvasGroup Overlay;

        /// <summary>
        /// 閲覧対象を切り替えるためのインベントリの名称
        /// InventoryDisplayのTargetInventoryNameに代入して使う
        /// この名前変更した後はInventoryDisplayに再描画までがセット
        ///   0 メイン1装備2コア3魔法4キーアイテム5敵図鑑　の順
        /// </summary>
        [SerializeField]
        string[] InventoryNames;

        [Header("開始時の挙動")]
        [MMInformation("HideContainerOnStart を true に設定すると、このフィールドのすぐ上に定義されている TargetInventoryContainer は、Scene ビューで表示したままでも、開始時に自動的に非表示にされます。セットアップに便利です。", MMInformationAttribute.InformationType.Info, false)]
        /// この値が true の場合、インベントリ・コンテナは起動時に自動的に非表示になります。
        public bool HideContainerOnStart = true;

        [Header("操作の許可")]
        [MMInformation("ここで、インベントリキャッチを開くときだけ入力させるか、させないかを決めることができます。", MMInformationAttribute.InformationType.Info, false)]
        /// つまりtrueだとインベントリを開くボタンを開くときの一方通行にするということ。同じボタンでとじられない
        public bool InputOnlyWhenOpen = true;

        [Header("キー設定")]

        //この辺あんま重要じゃないどうせこれで制御しない
        //input.で制御してるもんじゃ
        //UseとかEquipとかはUI上にボタンで持たせる

        [MMInformation("ここでは、お好みの様々なキーバインドを設定する必要があります。デフォルトでいくつか設定されていますが、自由に変更してください。", MMInformationAttribute.InformationType.Info, false)]


        protected RewiredCorgiEngineInputManager ReIManager;

        //   /*
        /// the key used to open/close the inventory
        public KeyCode ToggleInventoryKey = KeyCode.I;
        /// the alt key used to open/close the inventory
        public KeyCode ToggleInventoryAltKey = KeyCode.Joystick1Button6;
        /// the alt key used to open/close the inventory
        public KeyCode CancelKey = KeyCode.Escape;
        /// the alt key used to open/close the inventory
        public KeyCode CancelKeyAlt = KeyCode.Joystick1Button7;

        /// the key used to go to the next inventory
        public string NextInvKey = "page down";
        /// the alt key used to go to the next inventory
        public string NextInvAltKey = "joystick button 4";
        /// the key used to go to the previous inventory
        public string PrevInvKey = "page up";
        /// the alt key used to go to the previous inventory
        public string PrevInvAltKey = "joystick button 5";
    //  */
        [Header("Close Bindings")]
        /// このインベントリーがオープンしたら強制的に閉鎖されるはずの他のインベントリーのリスト
        public List<string> CloseList;

        public enum ManageButtonsModes { Interactable, SetActive }

        [Header("Buttons")]
        /// これが true の場合、InputManager は現在選択されているスロットに基づいて、インベントリコントロールボタンのインタラクションの状態を変更します。
        public bool ManageButtons = false;
        /// the selected mode to enable buttons with (interactable will change the button's interactable state, SetActive will enable/disable the button's game object
        [MMCondition("ManageButtons", true)]
        public ManageButtonsModes ManageButtonsMode = ManageButtonsModes.SetActive;
        /// the button used to equip or use an item
        [MMCondition("ManageButtons", true)]
        public Button EquipUseButton;
        /// the button used to move an item
        [MMCondition("ManageButtons", true)]
        public Button MoveButton;
        /// the button used to drop an item
        [MMCondition("ManageButtons", true)]
        public Button DropButton;
        /// the button used to equip an item
        [MMCondition("ManageButtons", true)]
        public Button EquipButton;
        /// the button used to use an item
        [MMCondition("ManageButtons", true)]
        public Button UseButton;
        /// the button used to unequip an item
        [MMCondition("ManageButtons", true)]
        public Button UnEquipButton;

        /// 有効なスロットを返す
        public InventorySlot CurrentlySelectedInventorySlot { get; set; }

        [Header("State")]
        /// これが真の場合、関連するインベントリは開いており、そうでない場合は閉じています。
        [MMReadOnly]
        public bool InventoryIsOpen;

        protected CanvasGroup _canvasGroup;
        protected GameObject _currentSelection;
        protected InventorySlot _currentInventorySlot;
        protected List<InventoryHotbar> _targetInventoryHotbars;
        protected InventoryDisplay _currentInventoryDisplay;
        private bool _isEquipUseButtonNotNull;
        private bool _isEquipButtonNotNull;
        private bool _isUseButtonNotNull;
        private bool _isUnEquipButtonNotNull;
        private bool _isMoveButtonNotNull;
        private bool _isDropButtonNotNull;

        /// <summary>
        /// 起動時に参照を取得し、ホットバーリストを準備します。
        /// </summary>
        protected virtual void Start()
        {
            _isDropButtonNotNull = DropButton != null;
            _isMoveButtonNotNull = MoveButton != null;
            _isUnEquipButtonNotNull = UnEquipButton != null;
            _isUseButtonNotNull = UseButton != null;
            _isEquipButtonNotNull = EquipButton != null;
            _isEquipUseButtonNotNull = EquipUseButton != null;
            _currentInventoryDisplay = TargetInventoryDisplay;
            InventoryIsOpen = false;
            _targetInventoryHotbars = new List<InventoryHotbar>();
            _canvasGroup = GetComponent<CanvasGroup>();
            foreach (InventoryHotbar go in FindObjectsOfType(typeof(InventoryHotbar)) as InventoryHotbar[])
            {
                _targetInventoryHotbars.Add(go);
            }
            if (HideContainerOnStart)
            {
                if (TargetInventoryContainer != null) { TargetInventoryContainer.alpha = 0; }
                if (Overlay != null) { Overlay.alpha = 0; }
                EventSystem.current.sendNavigationEvents = false;
                if (_canvasGroup != null)
                {
                    _canvasGroup.blocksRaycasts = false;
                }
            }
        }

        /// <summary>
        /// 毎フレーム、インベントリ、ホットバーの入力をチェックし、現在の選択をチェックします。
        /// </summary>
        protected virtual void Update()
        {
            HandleInventoryInput();
            HandleHotbarsInput();
            CheckCurrentlySelectedSlot();
            HandleButtons();
        }

        /// <summary>
        /// フレームごとに、現在どのスロットが選択されているかをチェックし、保存する
        /// 選択されてるスロットはuseやEquipなどのメソッドを呼び出すのに使う
        /// 今選んでるスロットのメソッドを呼び出す
        /// </summary>
        protected virtual void CheckCurrentlySelectedSlot()
        {
            //まず現在選択中のオブジェクトを格納
            _currentSelection = EventSystem.current.currentSelectedGameObject;
            if (_currentSelection == null)
            {
                return;
            }
            //格納したオブジェクトにスロットコンポーネントがあるなら現在選択中のスロットに
            //ないなら更新しない
            _currentInventorySlot = _currentSelection.gameObject.MMGetComponentNoAlloc<InventorySlot>();
            if (_currentInventorySlot != null)
            {
                CurrentlySelectedInventorySlot = _currentInventorySlot;
            }
        }

        /// <summary>
        /// ManageButtonsがtrueに設定されている場合、現在選択されているスロットに基づき、インベントリーコントロールを対話式にするかしないかを決定します。
        /// これはインベントリスロットというかアイテムが選択されてる時だけ「使用」とかのボタンを出現させる
        /// </summary>
        protected virtual void HandleButtons()
        {
            if (!ManageButtons)
            {
                return;
            }

            if (CurrentlySelectedInventorySlot != null)
            {
                if (_isUseButtonNotNull)
                {
                    SetButtonState(UseButton, CurrentlySelectedInventorySlot.Usable());
                }

                if (_isEquipButtonNotNull)
                {
                    SetButtonState(EquipButton, CurrentlySelectedInventorySlot.Equippable());
                }

                if (_isEquipUseButtonNotNull)
                {
                    SetButtonState(EquipUseButton, CurrentlySelectedInventorySlot.Usable() ||
                                                   CurrentlySelectedInventorySlot.Equippable());
                }

                if (_isUnEquipButtonNotNull)
                {
                    SetButtonState(UnEquipButton, CurrentlySelectedInventorySlot.Unequippable());
                }

                if (_isMoveButtonNotNull)
                {
                    SetButtonState(MoveButton, CurrentlySelectedInventorySlot.Movable());
                }

                if (_isDropButtonNotNull)
                {
                    SetButtonState(DropButton, CurrentlySelectedInventorySlot.Droppable());
                }
            }
            else
            {
                SetButtonState(UseButton, false);
                SetButtonState(EquipButton, false);
                SetButtonState(EquipUseButton, false);
                SetButtonState(DropButton, false);
                SetButtonState(MoveButton, false);
                SetButtonState(UnEquipButton, false);
            }
        }

        /// <summary>
        /// ボタンのオン・オフに使用される内部メソッド
        /// </summary>
        /// <param name="targetButton"></param>
        /// <param name="state"></param>
        protected virtual void SetButtonState(Button targetButton, bool state)
        {
            if (ManageButtonsMode == ManageButtonsModes.Interactable)
            {
                targetButton.interactable = state;
            }
            else
            {
                targetButton.gameObject.SetActive(state);
            }
        }

        /// <summary>
        /// 現在の状態に応じてインベントリーパネルを開閉します。
        /// </summary>
        public virtual void ToggleInventory()
        {
            if (InventoryIsOpen)
            {
                CloseInventory();
            }
            else
            {
                OpenInventory();
            }
        }

        /// <summary>
        /// インベントリーパネルを開く
        /// </summary>
        public virtual void OpenInventory()
        {
            //最初に呼びましょう
            //システム窓とかからインベントリに切り替わった時に

            if (CloseList.Count > 0)
            {
                //このインベントリを開くとき、代わりに閉じるインベントリを閉じる
                foreach (string playerID in CloseList)
                {
                    MMInventoryEvent.Trigger(MMInventoryEventType.InventoryCloseRequest, null, "", null, 0, 0, playerID);
                }
            }

            // ポーズにする

            if (_canvasGroup != null)
            {
                _canvasGroup.blocksRaycasts = true;
            }

            // インベントリを開く
            //インベントリ開いたというイベントをトリガー
            MMInventoryEvent.Trigger(MMInventoryEventType.InventoryOpens, null, TargetInventoryDisplay.TargetInventoryName, TargetInventoryDisplay.TargetInventory.Content[0], 0, 0, TargetInventoryDisplay.PlayerID);
            MMGameEvent.Trigger("inventoryOpens");
            InventoryIsOpen = true;

            StartCoroutine(MMFade.FadeCanvasGroup(TargetInventoryContainer, 0.2f, 1f));
            StartCoroutine(MMFade.FadeCanvasGroup(Overlay, 0.2f, 0.85f));
        }

        /// <summary>
        /// インベントリパネルを閉じる
        /// </summary>
        public virtual void CloseInventory()
        {
            // ポーズ解除

            if (_canvasGroup != null)
            {
                _canvasGroup.blocksRaycasts = false;
            }
            // インベントリを閉じる
            MMInventoryEvent.Trigger(MMInventoryEventType.InventoryCloses, null, TargetInventoryDisplay.TargetInventoryName, null, 0, 0, TargetInventoryDisplay.PlayerID);
            MMGameEvent.Trigger("inventoryCloses");
            InventoryIsOpen = false;

            StartCoroutine(MMFade.FadeCanvasGroup(TargetInventoryContainer, 0.2f, 0f));
            StartCoroutine(MMFade.FadeCanvasGroup(Overlay, 0.2f, 0f));
        }

        /// <summary>
        /// 在庫関連の入力を処理し、それに基づいて行動する。
        /// HandleInputだね
        /// 必要な入力と対応する処理を以下に
        /// それら以外はUI上のボタンで処理する
        /// 
        /// メインインベントリの開閉、というかポーズボタン？
        /// 倉庫とインベントリの行き来
        /// アイテムの詳細の切り替え（武器の攻撃力とテキストを切り替えるような）
        /// スロットに対して決定ボタンで使用などのボタンを表示、そしてキャンセルでスロットに戻る（処理流用できる）
        /// キャンセルでUIを消す
        /// </summary>
        protected virtual void HandleInventoryInput()
        {
            // 現在の在庫が表示されていない場合は、何もせずに終了します。
            if (_currentInventoryDisplay == null)
            {
                return;
            }
/*
            // トグルインベントリーキーが押されると
            if (Input.GetKeyDown(ToggleInventoryKey) || Input.GetKeyDown(ToggleInventoryAltKey))
            {
                // インベントリが開いていないなら開く
                if (!InventoryIsOpen)
                {
                    OpenInventory();
                }
                // 開いてるなら閉じる
                else
                {
                    CloseInventory();
                }
            }
            //クローズボタンでインベントリを閉じる
            if ((Input.GetKeyDown(CancelKey)) || (Input.GetKeyDown(CancelKeyAlt)))
            {
                if (InventoryIsOpen)
                {
                    CloseInventory();
                }
            }
            */
            // もし、オープン時の入力しか許可しておらず、インベントリが現在閉じている場合は、何もせずに終了します。
            if (InputOnlyWhenOpen && !InventoryIsOpen)
            {
                return;
            }
/*
            // 前のインベントリへ。倉庫とインベントリを行き来するような場面で使うかも
            //使わなくね？
            if (Input.GetKeyDown(PrevInvKey) || Input.GetKeyDown(PrevInvAltKey))
            {
                if (_currentInventoryDisplay.GoToInventory(-1) != null)
                {
                    _currentInventoryDisplay = _currentInventoryDisplay.GoToInventory(-1);
                }
            }

            // 次のインベントリのパネルへ
            if (Input.GetKeyDown(NextInvKey) || Input.GetKeyDown(NextInvAltKey))
            {
                if (_currentInventoryDisplay.GoToInventory(1) != null)
                {
                    _currentInventoryDisplay = _currentInventoryDisplay.GoToInventory(1);
                }
            }
            
            // アイテムの移動
            if (Input.GetKeyDown(MoveKey) || Input.GetKeyDown(MoveAltKey))
            {
                if (CurrentlySelectedInventorySlot != null)
                {
                    CurrentlySelectedInventorySlot.Move();
                }
            }

            // 装備か使用
            if (Input.GetKeyDown(EquipOrUseKey) || Input.GetKeyDown(EquipOrUseAltKey))
            {
                EquipOrUse();
            }

            // 装備
            if (Input.GetKeyDown(EquipKey) || Input.GetKeyDown(EquipAltKey))
            {
                if (CurrentlySelectedInventorySlot != null)
                {
                    CurrentlySelectedInventorySlot.Equip();
                }
            }

            // 使用
            if (Input.GetKeyDown(UseKey) || Input.GetKeyDown(UseAltKey))
            {
                if (CurrentlySelectedInventorySlot != null)
                {
                    CurrentlySelectedInventorySlot.Use();
                }
            }

            // 捨てる
            if (Input.GetKeyDown(DropKey) || Input.GetKeyDown(DropAltKey))
            {
                if (CurrentlySelectedInventorySlot != null)
                {
                    CurrentlySelectedInventorySlot.Drop();
                }
            }*/
        }

        /// <summary>
        /// アイテムや魔法のホットバーの入力をチェックし、それに応じた動作をします。
        /// ホットバーに表示するアイテムを方向キーで変えたりアイテムや魔法を使ったり
        /// </summary>
        protected virtual void HandleHotbarsInput()
        {
            if (!InventoryIsOpen)
            {
                foreach (InventoryHotbar hotbar in _targetInventoryHotbars)
                {
                    if (hotbar != null)
                    {
                        if (Input.GetKeyDown(hotbar.HotbarKey) || Input.GetKeyDown(hotbar.HotbarAltKey))
                        {
                            hotbar.Action();
                        }
                    }
                }
            }
        }


        /// <summary>
        /// インベントリのメニュー表示を現在表示すべき物に変える
        /// そしてナビゲーションに使うボタンも入れ替える
        /// 装備インベントリなのかお店インベントリなのか倉庫インベントリなのか
        /// インベントリディスプレイを切り替える
        /// 列挙型はゼロから
        /// </summary>
        public void MenuReset(int request)
        {
            //    useB,weaponB,magicB,coreB,keyB,materialB,libraryB;

            if (request == (int)Inventory.InventoryTypes.Main)
            {
                TargetInventoryDisplay.TargetInventoryName = InventoryNames[0];
                TargetInventoryDisplay._outerButton = MainUICon.instance.useB;
            }
            else if (request == (int)Inventory.InventoryTypes.Equipment)
            {
                TargetInventoryDisplay.TargetInventoryName = InventoryNames[1];
                TargetInventoryDisplay._outerButton = MainUICon.instance.weaponB;
            }
            else if (request == (int)Inventory.InventoryTypes.Core)
            {
                TargetInventoryDisplay.TargetInventoryName = InventoryNames[2];
                TargetInventoryDisplay._outerButton = MainUICon.instance.coreB;
            }
            else if (request == (int)Inventory.InventoryTypes.Magic)
            {
                TargetInventoryDisplay.TargetInventoryName = InventoryNames[3];
                TargetInventoryDisplay._outerButton = MainUICon.instance.magicB;
            }
            else if (request == (int)Inventory.InventoryTypes.KeyItem)
            {
                TargetInventoryDisplay.TargetInventoryName = InventoryNames[4];
                TargetInventoryDisplay._outerButton = MainUICon.instance.keyB;
            }
            else if (request == (int)Inventory.InventoryTypes.Library)
            {
                TargetInventoryDisplay.TargetInventoryName = InventoryNames[5];
                TargetInventoryDisplay._outerButton = MainUICon.instance.libraryB;
            }
            //再描画
            　TargetInventoryDisplay.SetupInventoryDisplay();

        }





        //----------------------------------------------------------------------ここから下のメソッド群はUIのボタンから呼び出す




        /// <summary>
        /// 装備/使用ボタンを押したときに、2つのメソッドのうちどちらを呼び出すかを決定する
        /// </summary>
        public virtual void EquipOrUse()
        {
            if (CurrentlySelectedInventorySlot.Equippable())
            {
                CurrentlySelectedInventorySlot.Equip();
            }
            if (CurrentlySelectedInventorySlot.Usable())
            {
                CurrentlySelectedInventorySlot.Use();
            }
        }

        //-------------------------------------------ここの奴をUIから呼ぶ

        public virtual void Equip()
        {
            CurrentlySelectedInventorySlot.Equip();
        }

        public virtual void Use()
        {
            CurrentlySelectedInventorySlot.Use();
        }

        public virtual void UnEquip()
        {
            CurrentlySelectedInventorySlot.UnEquip();
        }

        /// <summary>
        /// 選択したスロットの移動方法をトリガーする
        /// </summary>
        public virtual void Move()
        {
            CurrentlySelectedInventorySlot.Move();
        }

        /// <summary>
        /// Triggers the selected slot's drop method
        /// </summary>
        public virtual void Drop()
        {
            CurrentlySelectedInventorySlot.Drop();
        }

        //ーーーーーーーーーーーーーーーーーーーーーーーーー


        /// <summary>
        /// MMInventoryEventを捕捉して処理する。
        /// </summary>
        /// <param name="inventoryEvent">Inventory event.</param>
        public virtual void OnMMEvent(MMInventoryEvent inventoryEvent)
        {
            if (inventoryEvent.PlayerID != TargetInventoryDisplay.PlayerID)
            {
                return;
            }

            if (inventoryEvent.InventoryEventType == MMInventoryEventType.InventoryCloseRequest)
            {
                CloseInventory();
            }
        }

        /// <summary>
        /// Enableにすると、MMInventoryEventのリスニングを開始します。
        /// </summary>
        protected virtual void OnEnable()
        {
            this.MMEventStartListening<MMInventoryEvent>();
        }

        /// <summary>
        /// Disableの場合、MMInventoryEventのリッスンを停止します。
        /// </summary>
        protected virtual void OnDisable()
        {
            this.MMEventStopListening<MMInventoryEvent>();
        }




    }
}
