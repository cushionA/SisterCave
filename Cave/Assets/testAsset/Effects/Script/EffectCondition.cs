using DarkTonic.MasterAudio;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EffectCondition
{
    //��`
    #region


    public enum EmitType
    {
        Soon,//�ŏ��Ɉ�x����o��
        Repeat,//���̃X�e�[�g�̊ԌJ��Ԃ�
        Wait,// �����҂��čĐ�
        WaitRepeat,//�����҂��ČJ��Ԃ�
        Loop,//�����Ɩ炵������
        WaitLoop,
        End,//��ԏI���������
        None//�Đ����Ȃ�
    }


    [System.Serializable]
    /// <summary>
    /// �G�t�F�N�g�̏ڍ׃f�[�^�B
    /// ���ԖڂɍĐ�����邩�̓��X�g�̐ݒ�ł�낵��
    /// </summary>
    public class StateEffect
    {
        [Tooltip("�o���G�t�F�N�g")]
        public ParticleSystem _useEffect;

        [Tooltip("Soon�͂����ɍĐ��ARepeat�̓X�e�[�g�̊ԃ��[�v�AWait�͑҂��Ă���o��")]
        public EmitType _emitType = EmitType.Soon;

        [Tooltip("�G�t�F�N�g���o�Ă���ꏊ")]
        public Transform _emitPosition;

        [Tooltip("�G�t�F�N�g���o�����Ƃ��Ă��邩")]
        public bool _isFollow;

        [Tooltip(" �Đ��X�s�[�h���A�j���̍Đ����x�̉e�����󂯂邩�ǂ���")]
        public bool _matchAnime;


        [Tooltip("�e�̕����𖳎����邩�ǂ���")]
        /// <summary>
        /// �e�̕�����������Ȃ�
        /// </summary>
        public bool ignoreDirection;

        /// <summary>
        /// ���̃G�t�F�N�g�����Ɏg��ꂽ���ǂ���
        /// ���[�v�⃊�s�[�g�Ŏg��
        /// </summary>
        [HideInInspector]
        public bool isUsed;
    }


    [System.Serializable]
    /// <summary>
    /// �T�E���h�̏ڍ׃f�[�^�B
    /// ���ԖڂɍĐ�����邩�̓��X�g�̐ݒ�ł�낵��
    /// </summary>
    public class StateSound
    {
        [Tooltip("�o���G�t�F�N�g")]
        [SoundGroup]
        public string _useSound;

        [Tooltip("Soon�͂����ɍĐ��ARepeat�̓X�e�[�g�̊ԃ��[�v�AWait�͑҂��Ă���o��")]
        public EmitType _playType = EmitType.Soon;

        [Tooltip("�G�t�F�N�g���o�����Ƃ��Ă��邩")]
        public bool _isFollow;

        [Tooltip(" �Đ��X�s�[�h���A�j���̍Đ����x�̉e�����󂯂邩�ǂ���")]
        public bool _matchAnime;

        /// <summary>
        /// ���̉��������Ɏg��ꂽ���ǂ���
        /// ���[�v�⃊�s�[�g�Ŏg��
        /// </summary>
        [HideInInspector]
        public bool isUsed;
    }

    #endregion



    [Header("�Đ�����X�e�[�g")]
    ///<summary>
    /// �ǂ̃X�e�[�g�ŏo�Ă��邩
    /// �ŏ��Ƀ��X�g��鎞�Ɋ���U����
    /// �������̑I�񂾃X�e�[�g�̐�����ォ�珇�Ɋ���U��
    /// </summary>
    public MoreMountains.CorgiEngine.EffectControllAbility.SelectState _useState;

    /// <summary>
    /// ���s�Ƃ��ŋ��ʂ̑��������g���Ȃ炻������
    /// </summary>
    [Header("���ʃG�t�F�N�g���g�p���邩")]
    [Tooltip("���ʂ̐ݒ���g�p���邩")]
    public bool generalEffect;

    [Header("�G�t�F�N�g�̐ݒ�")]
    [Tooltip("�G�t�F�N�g�̐ݒ胊�X�g")]
    public List<StateEffect> _stateEffects;



    /// <summary>
    /// ���s�Ƃ��ŋ��ʂ̑��������g���Ȃ炻������
    /// </summary>
    [Header("���ʃT�E���h���g�p���邩")]
    [Tooltip("���ʂ̐ݒ���g�p���邩")]
    public bool generalSound;

    [Header("�T�E���h�̐ݒ�")]
    [Tooltip("���̐ݒ胊�X�g")]
    public List<StateSound> _stateSounds;






}
