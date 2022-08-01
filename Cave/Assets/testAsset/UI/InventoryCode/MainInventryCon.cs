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
    /// �Q�[������C���x���g�����Ăяo�����@�̗�ł��B
    /// ���̓N���X��GUI�}�l�[�W���N���X�ŏ������邱�Ƃ������߂��܂��B
    /// �����̏����p�N���Ď����̂����
    /// �C���x���g�����ƂɃX�N���v�g�𕪂���
    /// ����̓��C���C���x���g�����{�^���ŊJ�����߂̃R�[�h
    /// �������������ƃC���x���g���𑀍삷�邽�߂̃R�[�h�Ȃ̂���
    /// �ǂ̃C���x���g���Ƃ��֌W�Ȃ��S�ẴC���x���g���̑����\���������ǂ�
    /// �C���x���g���p�l���֘A���˂���
    /// </summary>
    public class MainInventoryCon : MonoBehaviour, MMEventListener<MMInventoryEvent>
    {
        [Header("Targets")]
        [MMInformation("�C���x���g�� �R���e�i�i�C���x���g�����J����������肷��Ƃ��ɃI��/�I�t���� CanvasGroup�j�A���C���� InventoryDisplay�A����� InventoryDisplay ���J�����Ƃ��ɂ��̉��ɕ\�������I�[�o�[���C�������Ƀo�C���h���܂��B", MMInformationAttribute.InformationType.Info, false)]
        /// �C���x���g���J�{�^�����������Ƃ��ɕ\��/��\���ɂ������v�f�����ׂĊ܂�CanvasGroup
        /// ����L�����o�X�O���[�v���I�u�W�F�N�g�̉��ɗv�f���܂Ƃ߂�
        public CanvasGroup TargetInventoryContainer;
        /// �C���x���g���̕\�������Ă�
        public MyInventoryDisplay TargetInventoryDisplay;
        /// �C���x���g�����J��/����Ƃ��ɂ��̉��Ŏg�p�����t�F�[�_�[�i�H�j
        public CanvasGroup Overlay;

        /// <summary>
        /// �{���Ώۂ�؂�ւ��邽�߂̃C���x���g���̖���
        /// InventoryDisplay��TargetInventoryName�ɑ�����Ďg��
        /// ���̖��O�ύX�������InventoryDisplay�ɍĕ`��܂ł��Z�b�g
        ///   0 ���C��1����2�R�A3���@4�L�[�A�C�e��5�G�}�Ӂ@�̏�
        /// </summary>
        [SerializeField]
        string[] InventoryNames;

        [Header("�J�n���̋���")]
        [MMInformation("HideContainerOnStart �� true �ɐݒ肷��ƁA���̃t�B�[���h�̂�����ɒ�`����Ă��� TargetInventoryContainer �́AScene �r���[�ŕ\�������܂܂ł��A�J�n���Ɏ����I�ɔ�\���ɂ���܂��B�Z�b�g�A�b�v�ɕ֗��ł��B", MMInformationAttribute.InformationType.Info, false)]
        /// ���̒l�� true �̏ꍇ�A�C���x���g���E�R���e�i�͋N�����Ɏ����I�ɔ�\���ɂȂ�܂��B
        public bool HideContainerOnStart = true;

        [Header("����̋���")]
        [MMInformation("�����ŁA�C���x���g���L���b�`���J���Ƃ��������͂����邩�A�����Ȃ��������߂邱�Ƃ��ł��܂��B", MMInformationAttribute.InformationType.Info, false)]
        /// �܂�true���ƃC���x���g�����J���{�^�����J���Ƃ��̈���ʍs�ɂ���Ƃ������ƁB�����{�^���łƂ����Ȃ�
        public bool InputOnlyWhenOpen = true;

        [Header("�L�[�ݒ�")]

        //���̕ӂ���܏d�v����Ȃ��ǂ�������Ő��䂵�Ȃ�
        //input.�Ő��䂵�Ă���񂶂�
        //Use�Ƃ�Equip�Ƃ���UI��Ƀ{�^���Ŏ�������

        [MMInformation("�����ł́A���D�݂̗l�X�ȃL�[�o�C���h��ݒ肷��K�v������܂��B�f�t�H���g�ł������ݒ肳��Ă��܂����A���R�ɕύX���Ă��������B", MMInformationAttribute.InformationType.Info, false)]


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
        /// ���̃C���x���g���[���I�[�v�������狭���I�ɕ������͂��̑��̃C���x���g���[�̃��X�g
        public List<string> CloseList;

        public enum ManageButtonsModes { Interactable, SetActive }

        [Header("Buttons")]
        /// ���ꂪ true �̏ꍇ�AInputManager �͌��ݑI������Ă���X���b�g�Ɋ�Â��āA�C���x���g���R���g���[���{�^���̃C���^���N�V�����̏�Ԃ�ύX���܂��B
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

        /// �L���ȃX���b�g��Ԃ�
        public InventorySlot CurrentlySelectedInventorySlot { get; set; }

        [Header("State")]
        /// ���ꂪ�^�̏ꍇ�A�֘A����C���x���g���͊J���Ă���A�����łȂ��ꍇ�͕��Ă��܂��B
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
        /// �N�����ɎQ�Ƃ��擾���A�z�b�g�o�[���X�g���������܂��B
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
        /// ���t���[���A�C���x���g���A�z�b�g�o�[�̓��͂��`�F�b�N���A���݂̑I�����`�F�b�N���܂��B
        /// </summary>
        protected virtual void Update()
        {
            HandleInventoryInput();
            HandleHotbarsInput();
            CheckCurrentlySelectedSlot();
            HandleButtons();
        }

        /// <summary>
        /// �t���[�����ƂɁA���݂ǂ̃X���b�g���I������Ă��邩���`�F�b�N���A�ۑ�����
        /// �I������Ă�X���b�g��use��Equip�Ȃǂ̃��\�b�h���Ăяo���̂Ɏg��
        /// ���I��ł�X���b�g�̃��\�b�h���Ăяo��
        /// </summary>
        protected virtual void CheckCurrentlySelectedSlot()
        {
            //�܂����ݑI�𒆂̃I�u�W�F�N�g���i�[
            _currentSelection = EventSystem.current.currentSelectedGameObject;
            if (_currentSelection == null)
            {
                return;
            }
            //�i�[�����I�u�W�F�N�g�ɃX���b�g�R���|�[�l���g������Ȃ猻�ݑI�𒆂̃X���b�g��
            //�Ȃ��Ȃ�X�V���Ȃ�
            _currentInventorySlot = _currentSelection.gameObject.MMGetComponentNoAlloc<InventorySlot>();
            if (_currentInventorySlot != null)
            {
                CurrentlySelectedInventorySlot = _currentInventorySlot;
            }
        }

        /// <summary>
        /// ManageButtons��true�ɐݒ肳��Ă���ꍇ�A���ݑI������Ă���X���b�g�Ɋ�Â��A�C���x���g���[�R���g���[����Θb���ɂ��邩���Ȃ��������肵�܂��B
        /// ����̓C���x���g���X���b�g�Ƃ������A�C�e�����I������Ă鎞�����u�g�p�v�Ƃ��̃{�^�����o��������
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
        /// �{�^���̃I���E�I�t�Ɏg�p�����������\�b�h
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
        /// ���݂̏�Ԃɉ����ăC���x���g���[�p�l�����J���܂��B
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
        /// �C���x���g���[�p�l�����J��
        /// </summary>
        public virtual void OpenInventory()
        {
            //�ŏ��ɌĂт܂��傤
            //�V�X�e�����Ƃ�����C���x���g���ɐ؂�ւ��������

            if (CloseList.Count > 0)
            {
                //���̃C���x���g�����J���Ƃ��A����ɕ���C���x���g�������
                foreach (string playerID in CloseList)
                {
                    MMInventoryEvent.Trigger(MMInventoryEventType.InventoryCloseRequest, null, "", null, 0, 0, playerID);
                }
            }

            // �|�[�Y�ɂ���

            if (_canvasGroup != null)
            {
                _canvasGroup.blocksRaycasts = true;
            }

            // �C���x���g�����J��
            //�C���x���g���J�����Ƃ����C�x���g���g���K�[
            MMInventoryEvent.Trigger(MMInventoryEventType.InventoryOpens, null, TargetInventoryDisplay.TargetInventoryName, TargetInventoryDisplay.TargetInventory.Content[0], 0, 0, TargetInventoryDisplay.PlayerID);
            MMGameEvent.Trigger("inventoryOpens");
            InventoryIsOpen = true;

            StartCoroutine(MMFade.FadeCanvasGroup(TargetInventoryContainer, 0.2f, 1f));
            StartCoroutine(MMFade.FadeCanvasGroup(Overlay, 0.2f, 0.85f));
        }

        /// <summary>
        /// �C���x���g���p�l�������
        /// </summary>
        public virtual void CloseInventory()
        {
            // �|�[�Y����

            if (_canvasGroup != null)
            {
                _canvasGroup.blocksRaycasts = false;
            }
            // �C���x���g�������
            MMInventoryEvent.Trigger(MMInventoryEventType.InventoryCloses, null, TargetInventoryDisplay.TargetInventoryName, null, 0, 0, TargetInventoryDisplay.PlayerID);
            MMGameEvent.Trigger("inventoryCloses");
            InventoryIsOpen = false;

            StartCoroutine(MMFade.FadeCanvasGroup(TargetInventoryContainer, 0.2f, 0f));
            StartCoroutine(MMFade.FadeCanvasGroup(Overlay, 0.2f, 0f));
        }

        /// <summary>
        /// �݌Ɋ֘A�̓��͂��������A����Ɋ�Â��čs������B
        /// HandleInput����
        /// �K�v�ȓ��͂ƑΉ����鏈�����ȉ���
        /// �����ȊO��UI��̃{�^���ŏ�������
        /// 
        /// ���C���C���x���g���̊J�A�Ƃ������|�[�Y�{�^���H
        /// �q�ɂƃC���x���g���̍s����
        /// �A�C�e���̏ڍׂ̐؂�ւ��i����̍U���͂ƃe�L�X�g��؂�ւ���悤�ȁj
        /// �X���b�g�ɑ΂��Č���{�^���Ŏg�p�Ȃǂ̃{�^����\���A�����ăL�����Z���ŃX���b�g�ɖ߂�i�������p�ł���j
        /// �L�����Z����UI������
        /// </summary>
        protected virtual void HandleInventoryInput()
        {
            // ���݂̍݌ɂ��\������Ă��Ȃ��ꍇ�́A���������ɏI�����܂��B
            if (_currentInventoryDisplay == null)
            {
                return;
            }
/*
            // �g�O���C���x���g���[�L�[����������
            if (Input.GetKeyDown(ToggleInventoryKey) || Input.GetKeyDown(ToggleInventoryAltKey))
            {
                // �C���x���g�����J���Ă��Ȃ��Ȃ�J��
                if (!InventoryIsOpen)
                {
                    OpenInventory();
                }
                // �J���Ă�Ȃ����
                else
                {
                    CloseInventory();
                }
            }
            //�N���[�Y�{�^���ŃC���x���g�������
            if ((Input.GetKeyDown(CancelKey)) || (Input.GetKeyDown(CancelKeyAlt)))
            {
                if (InventoryIsOpen)
                {
                    CloseInventory();
                }
            }
            */
            // �����A�I�[�v�����̓��͂��������Ă��炸�A�C���x���g�������ݕ��Ă���ꍇ�́A���������ɏI�����܂��B
            if (InputOnlyWhenOpen && !InventoryIsOpen)
            {
                return;
            }
/*
            // �O�̃C���x���g���ցB�q�ɂƃC���x���g�����s��������悤�ȏ�ʂŎg������
            //�g��Ȃ��ˁH
            if (Input.GetKeyDown(PrevInvKey) || Input.GetKeyDown(PrevInvAltKey))
            {
                if (_currentInventoryDisplay.GoToInventory(-1) != null)
                {
                    _currentInventoryDisplay = _currentInventoryDisplay.GoToInventory(-1);
                }
            }

            // ���̃C���x���g���̃p�l����
            if (Input.GetKeyDown(NextInvKey) || Input.GetKeyDown(NextInvAltKey))
            {
                if (_currentInventoryDisplay.GoToInventory(1) != null)
                {
                    _currentInventoryDisplay = _currentInventoryDisplay.GoToInventory(1);
                }
            }
            
            // �A�C�e���̈ړ�
            if (Input.GetKeyDown(MoveKey) || Input.GetKeyDown(MoveAltKey))
            {
                if (CurrentlySelectedInventorySlot != null)
                {
                    CurrentlySelectedInventorySlot.Move();
                }
            }

            // �������g�p
            if (Input.GetKeyDown(EquipOrUseKey) || Input.GetKeyDown(EquipOrUseAltKey))
            {
                EquipOrUse();
            }

            // ����
            if (Input.GetKeyDown(EquipKey) || Input.GetKeyDown(EquipAltKey))
            {
                if (CurrentlySelectedInventorySlot != null)
                {
                    CurrentlySelectedInventorySlot.Equip();
                }
            }

            // �g�p
            if (Input.GetKeyDown(UseKey) || Input.GetKeyDown(UseAltKey))
            {
                if (CurrentlySelectedInventorySlot != null)
                {
                    CurrentlySelectedInventorySlot.Use();
                }
            }

            // �̂Ă�
            if (Input.GetKeyDown(DropKey) || Input.GetKeyDown(DropAltKey))
            {
                if (CurrentlySelectedInventorySlot != null)
                {
                    CurrentlySelectedInventorySlot.Drop();
                }
            }*/
        }

        /// <summary>
        /// �A�C�e���▂�@�̃z�b�g�o�[�̓��͂��`�F�b�N���A����ɉ�������������܂��B
        /// �z�b�g�o�[�ɕ\������A�C�e��������L�[�ŕς�����A�C�e���▂�@���g������
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
        /// �C���x���g���̃��j���[�\�������ݕ\�����ׂ����ɕς���
        /// �����ăi�r�Q�[�V�����Ɏg���{�^��������ւ���
        /// �����C���x���g���Ȃ̂����X�C���x���g���Ȃ̂��q�ɃC���x���g���Ȃ̂�
        /// �C���x���g���f�B�X�v���C��؂�ւ���
        /// �񋓌^�̓[������
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
            //�ĕ`��
            �@TargetInventoryDisplay.SetupInventoryDisplay();

        }





        //----------------------------------------------------------------------�������牺�̃��\�b�h�Q��UI�̃{�^������Ăяo��




        /// <summary>
        /// ����/�g�p�{�^�����������Ƃ��ɁA2�̃��\�b�h�̂����ǂ�����Ăяo���������肷��
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

        //-------------------------------------------�����̓z��UI����Ă�

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
        /// �I�������X���b�g�̈ړ����@���g���K�[����
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

        //�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[


        /// <summary>
        /// MMInventoryEvent��ߑ����ď�������B
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
        /// Enable�ɂ���ƁAMMInventoryEvent�̃��X�j���O���J�n���܂��B
        /// </summary>
        protected virtual void OnEnable()
        {
            this.MMEventStartListening<MMInventoryEvent>();
        }

        /// <summary>
        /// Disable�̏ꍇ�AMMInventoryEvent�̃��b�X�����~���܂��B
        /// </summary>
        protected virtual void OnDisable()
        {
            this.MMEventStopListening<MMInventoryEvent>();
        }




    }
}
