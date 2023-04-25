using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using MoreMountains.InventoryEngine;
using UnityEngine.UI;
using Rewired.Integration.CorgiEngine;
using Rewired;
using Rewired.Integration.UnityUI;
using TMPro;
using MoreMountains.Feedbacks;

public class MainUICon : MonoBehaviour
{
    public static MainUICon instance = null;

    [HideInInspector] public bool selectWindow;

    [HideInInspector] public bool menuButtonOff;

    [HideInInspector]
    public bool UIOn;

    [HideInInspector]
    public bool isMenu;
    public GameObject masterUI;

    [HideInInspector] public GameObject selectButton;//�eUI�̑I��p

    /// <summary>
    ///     �\������E�B���h�E��؂�ւ���
    /// </summary>
    public TMP_Dropdown _windowSelecter;

    //������ʁA�C���x���g���A�V�X�e���ݒ�

    public GameObject equipWindow; 
    public GameObject inventoryWindow;
    public GameObject systemWindow;


    [HideInInspector]
    public bool isTips;

    [HideInInspector]
    public bool tipNeed;



    public SaveWinCon saveWin;

    public RewiredEventSystem eventSystem;
    public RewiredStandaloneInputModule stIn;


    public RewiredCorgiEngineInputManager _reInput;

    //  Button eq;
    SisUI sis;


    //�ŏ��̏������s�������̃t���O
    bool isInitial;

    /// <summary>
    /// ��������
    /// </summary>
    [HideInInspector]
    public bool isConversation;



    ///<summary>
    ///  �C���x���g���̃X�N���[���Ɏg��
    /// </summary>
    #region







    //UI�ł̓��͂�����������ɂȂ�܂ł̎���




    //�������v���p
    float pressTime;

    //���������s�[�g���Ɏ��ɓ�������
    float nextTime;


    //���݃X�N���[�����邩
    bool _scrollEnable;

    #endregion


    /// <summary>
    /// ���UI�Ŏg���p�����[�^
    /// </summary>
    #region

    ///<summary>
    /// �ݒ蒆�̃E�B���h�E�͂Ȃɂ�
    /// ��{�Ƃ��U���Ƃ�
    /// 1�A�^�[�Q�b�g�ݒ� 2�A�U���ݒ�
    /// 3�A�x������ 4�A�x������
    /// 5�A�񕜏��� 6�A�񕜌���
    ///</summary>
    [HideInInspector]
    public int settingNumber;

    ///<summary>
    /// ���Ԗڂ̏�����ҏW�����B�Q�Ɛ�l��
    /// �������ҏW���Ƃ�
    /// �Z�b�e�B���O�i���o�[�ƕ��p�œ���ł���
    ///</summary>
    [HideInInspector]
    public int editNumber;

    ///<summary>
    /// �ҏW�p�̃p�����[�^�B���낢��Q�Ƃ���B
    ///</summary>

    public SisterParameter editParameter;

    public GameObject secondDrop;
    //1�ڂ̑�
    public GameObject firstDrop;
    //�l�ݒ�
    [HideInInspector]
    public GameObject valueWindow;

    /// <summary>
    /// �h���b�v�_�E���ύX�ɂ�����ւ��Ώ�
    /// </summary>
    [HideInInspector]
    public Selectable changeTarget;

    /// <summary>
    /// ��قǃZ�b�g�����Q�[���I�u�W�F�N�g�B�Q�Ɨp
    /// </summary>
    [HideInInspector]
    public GameObject beforeSet;

    /// <summary>
    /// �ڑ��ύX����Ƃ�
    /// </summary>
    public int isChange;

    /// <summary>
    /// �I�[�g�q�[���ݒ蒆
    /// </summary>
    [HideInInspector]
    public bool isAH;

    /// <summary>
    /// �Z�[�u�������ǂ����A�F�X�g����
    /// �Q�[���J�n�����炷�U��
    /// </summary>
    [HideInInspector]
    public bool isSave;
    [HideInInspector]
    public bool editNow;

    [SerializeField]
    Transform TipsWindow;

    [SerializeField]
    MainInventryCon _inventoryCon;

    //���߂���@�\
    //
    //�h���b�v�_�E���̂����E�B���h�E����p�ӂ���
    //���i�K�̃{�^�����������Ƃ��̑����o��B�h���b�v�_�E���̗v�f�̓{�^�����Ƃɂ����ԍ��Ɛݒ�ԍ����犄��o���B�Q�Ɛ��
    //�Q�Ɛ�͕ϐ��ɂ��Ď��B�܂��A���R�������Ȃ��ł͉����o�Ă��Ȃ�
    //�h���b�v�_�E���őI�����ꂽ�^�C�v�ɉ����ē����悤�ȃh���b�v�_�E�������Ă���`�F�b�N�{�b�N�X�����Ă��肷��E�B���h�E���o��B
    //�h���b�v�_�E���Ɗ܂߂čő�O�̃E�B���h�E���o��
    //�s���̓N�[���^�C������������H�܂�l�ڂ�

    //�x���Ɖ񕜏����̏ꍇ
    //�E�B���h�E���
    //��Ԉُ�̎��A�`�F�b�N�{�b�N�X���o��i���͖���j
    //�؂�Ă�x���A������`�F�b�N�{�b�N�X�i���͖���j
    //�V�X�^�[����ƃv���C���[��MP��HP�A���̓t�H�[���ƈȏ�ȉ��w��
    //�G�^�C�v�̓`�F�b�N�{�b�N�X�B
    //�`�F�b�N�{�b�N�X���Ƃɔ�����ꂽ���l�Ř_���ς����錅��I�񂾂肷��Η��p�ł�����

    //�E�B���h�E�[���͋��G���o�邩�ǂ����Ǝw��Ȃ�
    //�h���b�v�_�E���Ƃ��ŉ���I��ł邩�ŕ\�������������ς��Ă���������

    //�x���񕜍s�̍s���͌����O��
    //��Ԗڂŉ������Ȃ��⑼�ֈڍs�A��Ԗڂŉ������Ȃ���I�ԂƃE�B���h�E�̐����ς��
    //�O�Ԗڂł͏����ɉ����Ĉȏ�ȉ���I��
    //�x�����ʂ͉񕜂����W�F�l�ƃo���A�A�x��������ȊO�Ƃ��ݕ����������悳����

    //�U������
    //�܂��[���͂Ȃ�
    //��{�I�ɑS�Ă̏����œG�̎�_�ƓG�p�����[�^�[�ƈʒu�ł̎w�肪�ł���̂Ŋ�{�O�ȏ�
    //�������Ȃ������O��
    //���͎l�B�V�X�^�[����ƃv���C���[��HP,MP�͐��l�w��Ə㉺�ɂ��āB���G�̑��݂͂��Ȃ��Ƃ����~�����̂ō��
    //�G�^�C�v��G��v���C���[�̏�Ԉُ�A�؂�Ă�x���̓`�F�b�N�{�b�N�X�B�����͈������Ɛ��l�̉��߂��Ⴄ�����ŏ����͓����B���ʂ̃X�N���v�g�ň�����ł��傤�B
    //��Ԗڂ̔��ł͂����I�Ԃ����B�O�Ԗڂł͑I��ŁA����ɗႦ��MP�g�p�ʂȂ炻�ꂪ���������Ȃ������I�ׂ�B
    //�ŏ�����h���b�v�_�E����MP����ʑ����Ƃ����Ȃ��Ƃ������̂��A�ʂɐݒ肷��̂��͔C����B

    //�������͂��̏����ōU���ɗނ���s�����Ƃ�ꍇ�̐ݒ�A�݂����Ȃ̂���������

    //�x���񕜏����͌����ړI�Ƀh���b�v�_�E���A�ݒ�
    //�x���񕜑I���̓h���b�v�_�E���A�h���b�v�_�E���i�ݒ�j�A�h���b�v�_�E���A�ݒ���

    //�U�������͌����ړI�ɂ͏������h���b�v�_�E���A�O���Ɋ�Â��ݒ�A��_�ݒ�A��O�����h���b�v�_�E���A�`�F�b�N�{�b�N�X���
    //���@�I�����h���b�v�_�E���A��������h���b�v�_�E���A��O�i�K�h���b�v�_�E���ƃ`�F�b�N�{�b�N�X��B

    //�ݒ�E�B���h�E�̈ʒu�͑O�̃h���b�v�_�E�����瑋���̂������l�ŏ㉺�ɓ���
    //�K���h���b�v�_�E�����邩��
    //�h���b�v�_�E���̓E�B���h�E�^�C�v�ɉ����ē����������s���B

    //�K�v�ȃ`�F�b�N�{�b�N�X
    //�����W�A�G�^�C�v�S�Cbool�n�A���������Ȃ����̈�����Bbool�����̓h���b�v�_�E���ɂ���B
    //�K�v�ȃT�|�[�g�̓h���b�v�_�E���ɂ���

    //�ƂȂ�Ɛݒ�E�B���h�E�̃^�C�v�̓h���b�v�_�E����ƁA���l��bool�A�����`�F�b�N�{�b�N�X�ƒZ���`�F�b�N�{�b�N�X�̎l��ނɂȂ�B
    //����������ʉ�����̂ɕK�v�Ȃ͎̂Q�Ɛ�Ɖ���ݒ肷�邩�̃^�C�g���ƁA

    //�����ɔԍ������Ĕԍ��ɍ������X�g�Ɏ���������BSelectable�ł�����Q�Ƃ���B
    //��Ɖ��A�ǂ�����ݒ肷�邩���Ȃ�����I�ׂ�B


    /// <summary>
    /// ��قǂ܂ł̓���
    /// </summary>
    int lastDirection;

    /// <summary>
    /// ��قǂ܂őI�����Ă�����
    /// </summary>
    int lastNumber = 100;

    #endregion
    [SerializeField]
    MyScroll _scroll;
    [SerializeField]
    MyInventoryDisplay[] _useInventory;
    bool _slotNow = true;

    [SerializeField]
    Selectable[] startButton;

    [SerializeField]
    GameObject[] OptionWindow;

    /// <summary>
    /// �݂�Ȃ��g����t�B�[�h�o�b�N
    /// </summary>
    #region

    public MMFeedbacks _TimeStop;

    public MMFeedbacks _TimeStart;

    #endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        sis = GetComponent<SisUI>();

        // ������I����Ԃɂ���
        // eq = equip.GetComponent<Button>();



        nextTime = stIn.repeatDelay;
    }

    // Update is called once per frame
    void Update()
    {
        //   if (GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction5))
        //    {
        //      ////
        //    }
        if (eventSystem.currentSelectedGameObject != null)
        {
            //  Debug.Log($"��{eventSystem.currentSelectedGameObject}{selectButton}");
        }
        else
        {
            //Debug.Log($"unnchi");
        }
   //     Debug.Log($"��{stIn.RewiredInputManager.get(stIn.verticalAxis)}");




        if (UIOn)
        {

        if (tipNeed && isTips)
        {

            TipsWindow.gameObject.SetActive(true);
        }
        else
        {
            
            TipsWindow.gameObject.SetActive(false);
            isTips = false;

        }
        if (_reInput.TipsButton.State.CurrentState == MMInput.ButtonStates.ButtonDown && eventSystem.currentSelectedGameObject != null)
        {
          //  Debug.Log($"s");
            tipNeed = !tipNeed;
        }
            if (selectButton != eventSystem.currentSelectedGameObject)
            {
                selectButton = eventSystem.currentSelectedGameObject;
                if (selectButton == _windowSelecter.gameObject)
                {

                    _scroll.ResetPoss();
                }
            }
        }

        //UI���Ă�̂�isMenu����Ȃ��Ƃ�
        if (!(UIOn && !isMenu))
        {

            if (_reInput.MenuCallButton.State.CurrentState == MMInput.ButtonStates.ButtonDown && !isConversation)
            {
                //���j���[�W�J�{�^���������ƃ��j���[�̕\����\����؂�ւ�
                if (isMenu && !selectWindow)
                {

                    isMenu = false;
                    masterUI.SetActive(false);
                    UIOn = false;
                    _TimeStart?.PlayFeedbacks();

                    for (int i = 0;i <= OptionWindow.Length - 1;i++)
                    {
                        OptionWindow[i].SetActive(false);
                    }
                    isInitial = false;
                    ButtonOn();

                    _scrollEnable = false;
                }

                else if (!isMenu)
                {
                    

                    _TimeStop?.PlayFeedbacks();

                    isMenu = true;
                    UIOn = true;
                    masterUI.SetActive(true);

                }
            }

           if (isMenu)
            {



 

                if (!isInitial)
                {
                    

                    _windowSelecter.Select();
                    _windowSelecter.value = 0;

                }
                //�h���b�v�_�E���̐��l����͂ŕς�����
                else if(selectButton == _windowSelecter.gameObject)
                {
                   
                    //�E����
                    if (_reInput.UIMovement.x > 0)
                    {

                        //�O�̃t���[����Raw���́i0��1���]1�j���獡�̓��͂�������0�ȉ��Ȃ�
                        //��������ƒ��������̍����J�[�\���ړ��ɏォ�牺�ɂ����Ȃ�ς�����̎��ォ�牺�ɋ}���]���Ȃ�
                        //����͑O�̓��͂�-1��0�̎��ɏ���������Ƃ��ƂȂ�B�j���[�g�����������͂����ɐ؂�ւ�����
                        if (lastDirection <= 0)
                        {
                            pressTime = 0;
                            nextTime = stIn.repeatDelay;
                        }

                        if (pressTime == 0 || pressTime > nextTime)
                        {
                            if (pressTime > stIn.repeatDelay)
                            {
                                //���ɓ����̂�actcool�b��
                                nextTime += stIn.repeatDelay;
                            }
                            if (_windowSelecter.value != _windowSelecter.options.Count - 1)
                            {
                                _windowSelecter.value++;
                            }
                            else
                            {
                                _windowSelecter.value = 0;
                            }

                        }
                        pressTime += Time.unscaledDeltaTime;

                    }
                    //������
                    else if (_reInput.UIMovement.x < 0)
                    {
                        //�O�̃t���[����Raw���́i0��1���]1�j���獡�̓��͂�������0�ȉ��Ȃ�
                        //��������ƒ��������̍����J�[�\���ړ��ɏォ�牺�ɂ����Ȃ�ς�����̎��ォ�牺�ɋ}���]���Ȃ�
                        //����͑O�̓��͂�1��0�̎��ɉ����������Ƃ��ƂȂ�B�j���[�g����������͂��牺�ɐ؂�ւ�����
                        if (lastDirection >= 0)
                        {
                            pressTime = 0;
                            nextTime = stIn.repeatDelay;
                        }

                        if (pressTime == 0 || pressTime > nextTime)
                        {
                            if (pressTime > stIn.repeatDelay)
                            {
                                //���ɓ����̂�actcool�b��
                                nextTime += stIn.repeatDelay;
                            }
                            if (_windowSelecter.value != 0)
                            {
                                _windowSelecter.value--;
                            }
                            else
                            {
                                _windowSelecter.value = _windowSelecter.options.Count - 1;
                            }
                        }
                        pressTime += Time.unscaledDeltaTime;
                    }
                    else
                    {
                        pressTime = 0;
                    }
                    lastDirection = (int)_reInput.UIMovement.x;
                }
                // Debug.Log($"value{_windowSelecter.value}to{lastNumber}");
                if (_slotNow && selectButton != _windowSelecter.gameObject)
                {
                    if (_scrollEnable == false)
                    {
                        pressTime = 0;
                    }
                    _scrollEnable = true;
                }
                else
                {
                    //�J�[�\������̃{�^���ɖ߂������ԏ�ɃX�N���[������
                    //           useBar.value = 1;
                    _scrollEnable = false;
                }

                //�؂�ւ���Ă�Ȃ�
                if (_windowSelecter.value != lastNumber || !isInitial)
                {

                    //window��SetActive�̂����ɃC���x���g���̕`��Ώۂ�ς���
                    //�V�X�e���ݒ肾���͑��̑�

                    //�X���b�g�ɃJ�[�\�����邩
                    

                    //�C�x���g�g���K�[��
                    if (_windowSelecter.value == 0 || !isInitial)
                    {//�Z���N�g���Ă���{�^�������[�̃{�^���̎�

                        equipWindow.SetActive(true);
                        
                        
                        if (lastNumber != 0 && !(lastNumber >= _windowSelecter.options.Count - 1))
                        {
                            _useInventory[lastNumber - 1].gameObject.SetActive(false);
                            inventoryWindow.SetActive(false);
                        }
                        else
                        {
                            systemWindow.SetActive(false);
                        }
                        lastNumber = 0;
                        //�\������UI�̑I��
                        isInitial = true;
                        _slotNow = false;
                        NavigationSetting(0);

                    }
                    //�ŏ��ƍŌ�ȊO�Ȃ�
                    else if (_windowSelecter.value != 0 && !(_windowSelecter.value >= _windowSelecter.options.Count - 1))
                    {
 �@�@�@�@�@�@�@�@�@�@�@Debug.Log("�ӂ���");

                        if (lastNumber != 0 && !(lastNumber >= _windowSelecter.options.Count - 1))
                        {
                         //  
                            _useInventory[lastNumber - 1].gameObject.SetActive(false);
                        }
                        else
                        {
                            inventoryWindow.SetActive(true);
                            systemWindow.SetActive(false);
                            equipWindow.SetActive(false);
                        }
                        _useInventory[_windowSelecter.value - 1].gameObject.SetActive(true);

                        //�C���x���g������Ȃ���
                        if (_useInventory[_windowSelecter.value - 1] != null) 
                        {
                            _inventoryCon.MenuReset(_useInventory[_windowSelecter.value - 1]);
                        }
                      //  Debug.Log($"ddddddd{lastNumber -1}{_windowSelecter.value - 1}de{_useInventory[lastNumber - 1].gameObject.name}");
                        lastNumber = _windowSelecter.value;
                       
                        _slotNow = true;
                    }
                    else
                    {

                        if (lastNumber != 0 && !(lastNumber >= _windowSelecter.options.Count - 1))
                        {
                            _useInventory[lastNumber - 1].gameObject.SetActive(false);
                            equipWindow.SetActive(false);
                        }
                        else
                        {
                             inventoryWindow.SetActive(false);
                        }
                        systemWindow.SetActive(true);
                        
                        NavigationSetting(1);
                        lastNumber = _windowSelecter.value;
                        _slotNow = false;
                    }

                   

                }

                JButton();
            } 
        }
    }


    public void MenuCancel()
    {
        GManager.instance.InputR.controllers.maps.SetMapsEnabled(true, "Default");
        GManager.instance.InputR.controllers.maps.SetMapsEnabled(false, "UI");
        selectWindow = false;

        isMenu = false;


        selectButton = null;
        ButtonOn();
        // MainUICon.instance.selectButton = null;
        // ToolManager.instance.selectItem = null;
        isInitial = false;
    }



    public void ButtonOff()
    {

    }

    public void ButtonOn()
    {
    }

    public void ConversationStart()
    {

        isConversation = true;
        ////Debug.Log("���H�ׂ���");
    }

    public void ConversationEnd()
    {

        isConversation = false;
    }

    public void JButton()
    {
        //�C���x���g�����̃A�C�e���̐���data.count

        //����d�v�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[
        //�@ pos = 1f / ((float)data.Count - 4.0f);
        //MyItem.rowLength��20�ɂ�����܂��Aint�Ő錾���Ă���̂ŁAfloat�ɒu�����Ă��܂��B
        //�����́@ScrollBar��Value�̏���A�Z���̐��A�\���\�ȃZ���̐��B
        if (!_scrollEnable)
        {
            _scroll._inputRead = false;
            return;
        }
        //   Debug.Log($"ad{_reInput.UIMovement.y}");
        //�Ȃ�Ȃ�l�i�ڂ܂ł͓����Ȃ��悤�ɂ��Ă�������
        _scroll._inputRead = true;

    }

    /// <summary>
    /// ���؂�ւ��ƍŏ��̃{�^���̊ԂŃi�r�Q�[�V������ݒ肷��
    /// </summary>
    /// <param name="i"></param>
    public void NavigationSetting(int i)
    {
        Navigation startPoint = _windowSelecter.navigation;
        startPoint.selectOnDown = startButton[i];
        _windowSelecter.navigation = startPoint;

        startPoint = startButton[i].navigation;
        startPoint.selectOnUp = _windowSelecter;
        startButton[i].navigation = startPoint;
    }

}

