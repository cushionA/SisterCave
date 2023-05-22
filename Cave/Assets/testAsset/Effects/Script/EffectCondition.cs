using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using UnityEditor;
using DarkTonic.MasterAudio;

[System.Serializable]
public class EffectCondition
{



    [Header("�Đ�����X�e�[�g")]
    [MMInspectorGroup("�Đ�����", true)]

    ///<summary>
    /// �ǂ̃X�e�[�g�ŏo�Ă��邩
    /// �ŏ��Ƀ��X�g��鎞�Ɋ���U����
    /// �������̑I�񂾃X�e�[�g�̐�����ォ�珇�Ɋ���U��
    /// </summary>
    public MoreMountains.CorgiEngine.EffectControllAbility.SelectState _useState;




    public enum EmitType
    {
        Soon,//�ŏ��Ɉ�x����o��
        Repeat,//���̃X�e�[�g�̊ԌJ��Ԃ�
        Wait,// �����҂��čĐ�
        WaitRepeat,//�����҂��ČJ��Ԃ�
        Loop,//�����Ɩ炵������
        WaitLoop,
        End//��ԏI���������
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
    }

    [Header("�G�t�F�N�g�̐ݒ�")]
    [MMInspectorGroup("�G�t�F�N�g�̐ݒ�", true)]
    [MMCondition("_effectOn", true)]
    [Tooltip("�G�t�F�N�g�̐ݒ胊�X�g")]
    public List<StateEffect> _stateEffects;


    [Header("�T�E���h�̐ݒ�")]
    [MMInspectorGroup("�T�E���h�̐ݒ�", true)]


    [MMCondition("_soundOn",false)]
    [Tooltip("���̐ݒ胊�X�g")]
    public List<StateSound> _stateSounds;






}
