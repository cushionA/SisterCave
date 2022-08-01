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

public class MainUICon : MonoBehaviour
{
    public static MainUICon instance = null;

    [HideInInspector] public bool selectWindow;
    [HideInInspector] public bool openWindow;
    [HideInInspector] public bool menuButtonOff;

    [HideInInspector]
    public bool isMenu;
    public GameObject masterUI;

    [HideInInspector] public GameObject selectButton;//�eUI�̑I��p


    public Button eqB;
    public Button useB;
    public Button weaponB;
    public Button magicB;
    public Button coreB;
    public Button keyB;
    public Button materialB;
    public Button libraryB;
    public Button systemB;

    //������ʁA�C���x���g���A�V�X�e���ݒ�

    public GameObject equipWindow; 
    public GameObject inventoryWindow;
    public GameObject systemWindow;


    [HideInInspector]
    public bool isTips;

    [HideInInspector]
    public bool tipNeed;

    public bool isReBuild;
    public GameObject Scon;
    public SaveWinCon saveWin;

    public RewiredEventSystem eventSystem;
    public RewiredStandaloneInputModule stIn;

    /// <summary>
    /// ���͈ꗗ
    /// </summary>
    #region
    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction0;

    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction1;

    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction2;

    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction3;

    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction4;

    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction5;

    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction6;

    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction7;

    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction8;

    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction9;

    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction10;

    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction11;

    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction12;

    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction13;

    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction14;

    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction15;

    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction16;

    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction17;

    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction18;

    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction19;

    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction20;

    #endregion
    //  Button eq;
    SisUI sis;



    bool isFirst;
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

    [SerializeField] Scrollbar useBar;




    //���̃X�N���[���œ���������
    float pos;


    //UI�ł̓��͂�����������ɂȂ�܂ł̎���
    float repeatDelay;

    //���������s�[�g���ɉ��b���Ƃ�UI�̓��͂��s���邩
    float _actCool;

    //�������v���p
    float pressTime;

    //���������s�[�g���Ɏ��ɓ�������
    float nextTime;

    //���݃C���x���g�����J���Ă��邩
    bool _inventoryNow;

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
    /// ����ɍU��0~2�A��3~5�Ƃ��ɂ��ĂP�Ȃ����2�Ȃ�s���Ƃ������ɕ�����
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
    MainInventoryCon _inventoryCon;

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

        _actCool = (1 / stIn.inputActionsPerSecond);
        repeatDelay = stIn.repeatDelay;
        nextTime = repeatDelay;
    }

    // Update is called once per frame
    void Update()
    {
        //   if (GManager.instance.InputR.GetButtonDown(MainUICon.instance.rewiredAction5))
        //    {
        //      ////Debug.log("unnchi");
        //    }


        if (tipNeed && isTips)
        {
            TipsWindow.gameObject.SetActive(true);
        }
        else
        {
            TipsWindow.gameObject.SetActive(false);
            isTips = false;

        }
        if (GManager.instance.InputR.GetButtonDown(MainUICon.instance.rewiredAction20) && eventSystem.currentSelectedGameObject != null)
        {
            tipNeed = !tipNeed;
        }

        /*    if (isConversation)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        */

        //  //Debug.Log($"����{Time.timeScale}");
  /*      if (MaterialManager.instance.isUseMenu || ToolManager.instance.isUseMenu || EquipManager.instance.isUseMenu
            || EnemyDataManager.instance.isUseMenu || CoreManager.instance.isUseMenu || ToolManager.instance.isUseMenu || KeyManager.instance.isUseMenu)
        {
            openWindow = true;

        }
        else
        {
            openWindow = false;
        }
        // ////Debug.log($"�����Đ^�ɂȂ�{openWindow}");
  */


        // ////Debug.log($"�������m�Ftrue�ɂȂ�{isInitial}");

        if (!sis.sisMenu)
        {
            if (GManager.instance.InputR.GetButtonDown(MainUICon.instance.rewiredAction14) && !isConversation)
            {
                //���j���[�W�J�{�^���������ƃ��j���[�̕\����\����؂�ւ�
                if (isMenu && !selectWindow && !openWindow)
                {
                    isFirst = false;
                    isMenu = false;
                    isReBuild = false;
                    Scon.SetActive(false);


                    //////Debug.log($"qawsedfrgjui,lo{usec.isEver}");
                    isInitial = false;
                    ButtonOn();
                    GManager.instance.InputR.controllers.maps.SetMapsEnabled(true, "Default");
                    GManager.instance.InputR.controllers.maps.SetMapsEnabled(false, "UI");
                    _inventoryNow = false;
                    _scrollEnable = false;
                }

                else if (!isMenu)
                {
                    isFirst = false;
                    GManager.instance.InputR.controllers.maps.SetMapsEnabled(false, "Default");
                    GManager.instance.InputR.controllers.maps.SetMapsEnabled(true, "UI");
                    // isFirst = true;
                    isMenu = true;
                    isReBuild = true;
                    Scon.SetActive(true);
                }
            }

            //���j���[��W�J
            if (!isMenu && !isConversation)
            {
                // //Debug.Log("��؂���");
                Time.timeScale = 1.0f;
                if (!isFirst)
                {
                    masterUI.SetActive(false);
                    isFirst = true;
                }
            }
            //���j���[�W�J��
            else if (isMenu)
            {
                bool change = false;

                if (selectButton != eventSystem.currentSelectedGameObject)
                {
                    selectButton = eventSystem.currentSelectedGameObject;
                    change = true;
                }

                Time.timeScale = 0;
                
                if (!isFirst)
                {
                    eqB.Select();
                    eqB.onClick.Invoke();
                    isFirst = true;
                    masterUI.SetActive(true);
                }

                //�؂�ւ���Ă�Ȃ�
                if (change)
                {

                    //window��SetActive�̂����ɃC���x���g���̕`��Ώۂ�ς���
                    //�V�X�e���ݒ肾���͑��̑�

                    //�X���b�g�ɃJ�[�\�����邩
                    bool _slotNow = true;

                    //�C�x���g�g���K�[��
                    if (selectButton == eqB.gameObject && !isInitial)
                    {//�Z���N�g���Ă���{�^�������[�̃{�^���̎�




                        equipWindow.SetActive(true);
                        systemWindow.SetActive(false);
                        inventoryWindow.SetActive(false);


                        _inventoryNow = false;

                        //�\������UI�̑I��
                        isInitial = true;
                        _slotNow = false;
                    }
                    else if (selectButton == useB.gameObject)
                    {
                        inventoryWindow.SetActive(true);
                        systemWindow.SetActive(false);
                        equipWindow.SetActive(false);
                        _inventoryCon.MenuReset(0);
                        _inventoryNow = true;
                        isInitial = false;
                        _slotNow = false;
                    }
                    else if (selectButton == weaponB.gameObject)
                    {

                        inventoryWindow.SetActive(true);
                        systemWindow.SetActive(false);
                        equipWindow.SetActive(false);
                        isInitial = false;
                        _inventoryNow = true;
                        _inventoryCon.MenuReset(1);
                        _slotNow = false;
                    }
                    else if (selectButton == coreB.gameObject)
                    {

                        inventoryWindow.SetActive(true);
                        systemWindow.SetActive(false);
                        isInitial = false;
                        equipWindow.SetActive(false);
                        _inventoryCon.MenuReset(2);
                        _inventoryNow = true;
                        _slotNow = false;
                    }
                    else if (selectButton == keyB.gameObject)
                    {

                        inventoryWindow.SetActive(true);
                        systemWindow.SetActive(false);
                        isInitial = false;
                        equipWindow.SetActive(false);
                        _inventoryCon.MenuReset(3);
                        _inventoryNow = true;
                        _slotNow = false;
                    }
                    else if (selectButton == magicB.gameObject)
                    {

                        inventoryWindow.SetActive(true);
                        systemWindow.SetActive(false);
                        equipWindow.SetActive(false);
                        isInitial = false;
                        _inventoryNow = true;
                        _inventoryCon.MenuReset(4);
                        _slotNow = false;
                    }
                    else if (selectButton == materialB.gameObject)
                    {

                        inventoryWindow.SetActive(true);
                        systemWindow.SetActive(false);
                        equipWindow.SetActive(false);
                        isInitial = false;
                        _inventoryNow = true;
                        _inventoryCon.MenuReset(5);
                        _slotNow = false;
                    }
                    else if (selectButton == libraryB.gameObject)
                    {

                        inventoryWindow.SetActive(true);
                        systemWindow.SetActive(false);
                        equipWindow.SetActive(false);
                        isInitial = false;
                        _inventoryNow = true;
                        _inventoryCon.MenuReset(6);
                        _slotNow = false;
                    }
                    else if (selectButton == systemB.gameObject)
                    {

                        equipWindow.SetActive(false);
                        systemWindow.SetActive(true);
                        inventoryWindow.SetActive(false);
                        isInitial = false;
                        _inventoryNow = false;
                        _slotNow = false;
                    }
                    if(_slotNow && _inventoryNow)
                    {
                        _scrollEnable = true;
                    }
                    else if (!_slotNow && _inventoryNow)
                    {
                        //�J�[�\������̃{�^���ɖ߂������ԏ�ɃX�N���[������
                        useBar.value = 0;
                        _scrollEnable = false;
                    }


                }
            }
        }
    }


    public void MenuCancel()
    {
        GManager.instance.InputR.controllers.maps.SetMapsEnabled(true, "Default");
        GManager.instance.InputR.controllers.maps.SetMapsEnabled(false, "UI");
        selectWindow = false;
        isFirst = false;
        isMenu = false;
        isReBuild = false;
        Scon.SetActive(false);

        selectButton = null;
        ButtonOn();
        // MainUICon.instance.selectButton = null;
        // ToolManager.instance.selectItem = null;
        isInitial = false;
    }

    void EquipMenuReset()
    {

       // ToolManager.instance.isEquipMenu = false;
    }

    //   public void UseReBuild()
    //    {
    //      isReBuild = true;
    //      usec.isEver = false;
    //   }

    public void ButtonOff()
    {
        weaponB.enabled = false;
        useB.enabled = false;
        coreB.enabled = false;
        magicB.enabled = false;
        keyB.enabled = false;
        useB.enabled = false;
        systemB.enabled = false;
        libraryB.enabled = false;
        menuButtonOff = true;
        selectButton = null;
    }
    public void ButtonOn()
    {
        weaponB.enabled = true;
        useB.enabled = true;
        coreB.enabled = true;
        magicB.enabled = true;
        keyB.enabled = true;
        useB.enabled = true;
        systemB.enabled = true;
        libraryB.enabled = true;
        menuButtonOff = false;
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



        if (GManager.instance.InputR.GetAxisRaw(MainUICon.instance.rewiredAction15) > 0)
        {

            //�O�̃t���[����Raw���́i0��1���]1�j���獡�̓��͂�������0�ȉ��Ȃ�
            //��������ƒ��������̍����J�[�\���ړ��ɏォ�牺�ɂ����Ȃ�ς�����̎��ォ�牺�ɋ}���]���Ȃ�
            //����͑O�̓��͂�-1��0�̎��ɏ���������Ƃ��ƂȂ�B�j���[�g�����������͂����ɐ؂�ւ�����
            if (GManager.instance.InputR.GetAxisPrev(MainUICon.instance.rewiredAction15) - GManager.instance.InputR.GetAxisRaw(MainUICon.instance.rewiredAction15) < 0)
            {
                pressTime = 0;
                nextTime = repeatDelay;
            }
            
            if(pressTime == 0 ||  pressTime > nextTime)
            {
                if (pressTime > repeatDelay)
                {
                    //���ɓ����̂�actcool�b��
                    nextTime += _actCool;
                }
                useBar.value += pos;
            }
            pressTime += Time.deltaTime;

        }
        else if (GManager.instance.InputR.GetAxisRaw(MainUICon.instance.rewiredAction15) < 0)
        {
            //�O�̃t���[����Raw���́i0��1���]1�j���獡�̓��͂�������0�ȉ��Ȃ�
            //��������ƒ��������̍����J�[�\���ړ��ɏォ�牺�ɂ����Ȃ�ς�����̎��ォ�牺�ɋ}���]���Ȃ�
            //����͑O�̓��͂�1��0�̎��ɉ����������Ƃ��ƂȂ�B�j���[�g����������͂��牺�ɐ؂�ւ�����
            if (GManager.instance.InputR.GetAxisPrev(MainUICon.instance.rewiredAction15) - GManager.instance.InputR.GetAxisRaw(MainUICon.instance.rewiredAction15) > 0)
            {
                pressTime = 0;
                nextTime = repeatDelay;
            }

            if (pressTime == 0 || pressTime > nextTime)
            {
                if (pressTime > repeatDelay)
                {
                    //���ɓ����̂�actcool�b��
                    nextTime += _actCool;
                }
                useBar.value -= pos;
            }
            pressTime += Time.deltaTime;
        }

        //�}�C�i�X�Ƃ��ɂȂ�Ȃ��悤�ɒ���
        if (useBar.value > 1)
        {
            useBar.value = 1;

        }
        else if (useBar.value < 0)
        {
            useBar.value = 0.01f;
        }



    }


}

