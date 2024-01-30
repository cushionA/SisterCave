using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using UnityEngine.AddressableAssets;
using Rewired.Integration.CorgiEngine;
using DarkTonic.MasterAudio;
using System;
using Cysharp.Threading.Tasks;
using static UnityEngine.Rendering.DebugUI;
using Micosmo.SensorToolkit.Example;

namespace MoreMountains.CorgiEngine // you might want to use your own namespace here
{
    /// <summary>
    /// ��Ԉُ�Ƃ��o�t���Ǘ����郁�\�b�h
    /// �񓯊��ł�邩�H
    /// �Ōn�̒~�ς�1�ȏ�~�ς������_�ŏ�Ԉُ�ɒǉ�
    /// ���̌�l�b��10���炢����Ȃ���~�ς���̂�҂�
    /// �~�ς�����Ɣ��ǁB�V�K�~�ς����ƌ����J�n�J�E���g�����Z�b�g�A�~�ϒl���Z�A�����ɓŃ_���[�W�̌��ʂ��㏑���i�_���[�W2�̓ł�4�̓łƂ�����͂��j
    /// ��Ԉُ�~�ς�100�𒴂���Ɣ���
    /// ��Ԉُ�ϐ��͒~�ϒl�J�b�g�B�ł���{�I��p�[�Z���g�͂��Ȃ�Ⴂ�B�U�������x�H�炤�ƓłɂȂ邩�A�Ƃ����񐔂����񌸂邩���ӎ����Đݒ�
    /// �V�X�^�[����̏�Ԉُ�h�ǂ͗�O�I�ɂ��̂������{��������
    /// 
    /// �G�t�F�N�g�R���g���[���[�ƘA�g������
    /// 
    /// 
    /// ��Ԃɂ͎O��ނ���
    /// �E�����Ɍ��ʔ������ď�����i�񕜂Ƃ��j
    /// �E��莞�Ԍp�����Č��ʂ𔭊�����i�łƂ����W�F�l�j
    /// �E�������Ō��ʔ������ď�����i���̍U�������Ƃ��H�j�i���̏ꍇ�ł����Ԑ����͂���H�@�U����60�b�o�߂��c�݂����ȁj
    /// �E�����i�Ȃǂ̉i�������i�ړ����x���������Ƃ��j
    /// 
    /// �K�v�Ȓ�`��
    /// �E���ʃ^�C�v
    /// �E���ʗʂ̐��l
    /// �E���ŏ���
    /// �E���ʔ������̃G�t�F�N�g�ƃT�E���h�̎�ʁi�G�t�F�N�g�ɂ͔����G�t�F�N�g�ƌp���G�t�F�N�g�i�o�t�f�o�t������H�j�ƃG���`���G�t�F�N�g������j
    /// 
    /// �d�l
    /// �E���ʎ���̓w���X�i�̗͕ϓ��A�ϐ��l�ύX�j
    /// �E�_���[�W�I���^�b�`�i�o�t��G���`���̍U���͕ύX�j
    /// �E�ړ����x�ύX�ȂǂŃR���g���[���A�r���e�B���i�A�j�����x�A��������@�\���Ȃ��Ƃȁj
    /// �E�����ň����̂̓A�j���[�V�����𔺂�Ȃ��G�t�F�N�g�Ə�Ԍ��ʂ���
    /// 
    /// 
    /// ���
    /// �E�����̌��ʂ��o�鎞�A�G�t�F�N�g�≹���̏����͂ǂ����邩�i�񕜂Ƌ����Ƃ��j
    /// �E�����������G�t�F�N�g�͑S���o���H�@�ł��p���G�t�F�N�g�Ԃł͗D��֌W������悤�ɂ��邩
    /// �E��x�g������n�͂ǂ����邩�B���̃t���O���g������ŕ񍐂�����H�@�Z��g�p�����Ƃ��ŏ���
    /// 
    /// �@�\
    /// �E�G�t�F�N�g�o��
    /// �E���ʂ��Ǘ��A�I��������
    /// 
    /// 
    /// </summary>
    [AddComponentMenu("Corgi Engine/Character/Abilities/TODO_REPLACE_WITH_ABILITY_NAME")]
    public class ConditionAndEffectControllAbility : MyAbillityBase
    {

        /// 
        /// ��Ԉُ�ƃo�t���܂Ƃ߂���
        /// 
        /// �f�o�t�i�U����A�C�e���ŗ^������̂Ƒ����i�̃f�����b�g�ŗ^������̂�����j
        /// �E��
        /// �E�ғŁi�傫�ȃ_���[�W�ƃX�^�~�i�񕜒ቺ�B���ʎ��ԒZ�߁j
        /// �E�����i��~�B��_���[�W����j
        /// �E�S���i��~�j
        /// �E���فi���@����j
        /// �E����i��_���[�W�㏸�A�X�^�~�i�i�A�[�}�[�j�񕜑��x�ቺ�A��Ԉُ�~�ω������x�����j
        /// �E�߂܂�or�����i�^�_���[�W�����A�K�[�h���\���򉻁B�肪�k����B�����̕������ʏ�j
        /// �E����i�w�C�g�㏸�A�ړ����x�ቺ�j
        /// �E��_���[�W����i����͑����i�Ƃ��ŗ^�������j�i���葮���ɕ�����H�j
        /// �E�^�_���[�W�����i����͑����i�Ƃ��ŗ^�������j
        /// �E�ړ����x�ቺ�i����͑����i�Ƃ��ŗ^�������j
        /// �E�w�C�g�㏸�i����͑����i�Ƃ��ŗ^�������j
        /// �E�A�C�e���̌��ʌ���
        /// 
        /// �o�t
        /// �E���W�F�l
        /// �E�����i��_���[�W�ቺ�A�X�^�~�i�i�A�[�}�[�j�񕜉����A��Ԉُ�~�ό������x�����j
        /// �E�j���i�^�_���[�W����A�K�[�h���\�����j
        /// �E�B���i�w�C�g�����A�ړ����x�����A�������Łj
        /// �E��_���[�W�����i����͑����i�Ƃ��ŗ^�������j�i�ł����@�ŒP�̂ŗ^���Ă����������j
        /// �E�^�_���[�W�����i����͑����i�Ƃ��ŗ^�������j�i�ł����@�ŒP�̂ŗ^���Ă����������j
        /// �E�A�N�V���������i�ړ����x�������i�W�����v�ł���Ƃ��������Ƃ��S������ɓ����B�G�t�F�N�g��A�C�R��������j�i����͑����i�Ƃ��ŗ^�������j�i�ł����@�ŒP�̂ŗ^���Ă����������j
        /// �E����U�������i���@�A�J�E���^�[�A�e�푮���j
        /// �E�A�C�e���̌��ʑ����i�A�C�e�����ʑ����ł܂Ƃ߂Ăǂ̃A�C�e���������Ȃ̂���enum�ŊǗ�����H�j�i��{�I�ɑ����i�Łj
        /// �E�o���A
        /// �E�G���`�����g
        /// �E���U�I�Z���i�����j
        /// 
        /// �P���Ȍ���
        /// �E��
        /// �EMP��
        /// �E��Ԉُ�����i�ǂ̏�Ԉُ��������̂���enum�ŁH�@�����񕜂��邩�ŃG�t�F�N�g���ς��Ƃ��j
        /// �E�o�t�폜
        /// �E�X�^�~�i�ƃA�[�}�[���Z�b�g
        /// 
        /// ///
        /// 

        /// 
        /// ��邱��
        /// 
        /// �E��ԕω��ɍ��킹�ăG�t�F�N�g��A�C�R�����o���@�\
        /// �E��Ԉُ�̒~�ϗʒቺ���ǂ����ʉ����邩
        /// �E�v���C���[�̂����~�ϗʂ�\������
        /// �E����I�ɂ�����ʁi�łƂ��j���悹����@�l����B����̃��j�[�N�C�x���g�Ȃ�Ƃ��H�@����̃��j�[�N�C�x���g�i�Łj��HP����͒���I�ɂ��H
        /// �E�G���`�����g�G�t�F�N�g�ǂ����悤�i�ʃ��[�g�Ɉړ����ĕ���X�v���C�g�ɃA�N�Z�X���邩�j
        /// �E�G�t�F�N�g�o���̂̓��j�[�N�G�t�F�N�g�����ł����H
        /// �E�G�t�F�N�g�̓V�F�[�_�[�ō��������L�����ɂ���enable�؂�ւ��ŕ\�����銴����
        /// 
        /// ///



        #region�@��`





        #region�@�G�t�F�N�g�敪

        /// <summary>
        ///  �C�x���g�̎�ނ��L�q
        ///  �_���[�W�Ȃ�
        ///  �R���f�B�V�����̋�̓I�Ȍ��ʂ͂���ɂȂ�
        ///  
        /// ��̓I�ȃo�t��f�o�t���炱����΂��H
        /// ��ɒ�`���Ƃ���
        /// 
        /// enum�Ŏ��ʕK�v�ȃf�[�^
        /// �E�X�e�[�^�X
        /// �E��������
        /// �E�A�N�V��������
        /// �E�ǂ̃A�C�e�����������邩
        /// �E��Ԉُ�~��
        /// 
        /// �A�N�V���������ɂ���
        /// �E�ǂ̃A�N�V�����ω��Ȃ̂���enum��
        /// �E���ꃂ�[�V�����ɕω��Ƃ��͂Ȃ��B�G�t�F�N�g�����ς��邩�H����Ƃ����ꃂ�[�V������ɗp�ӂ��Ƃ���
        /// �E���ꂼ��Ⴄ���Ǔ�i�W�����v�Ƃ�����������o�����ԁB�p���B���X�^�~�i�񕜂Ƃ����B
        /// �E�ʂɃG�t�F�N�g�͏o���Ȃ��H
        /// �E�ǂ̃A�N�V���������Ă���Enum��int�������[�Ɏ������邩�H
        /// 
        /// 
        /// ��������
        /// �^�[�Q�b�g�Ƒ����g�ݍ��킹��`�ɂ��Ȃ��H
        /// 
        /// ���Ƃ��΂�
        /// enumTarget �ؗ�
        /// enumEffect ���Z
        /// 
        /// �Ƃ�
        /// 
        /// </summary>
        [Flags]
        public enum EventType
        {
            
            HP�ϓ�,//�I�������ς�����łɂȂ��
            MP�ϓ�,
            �X�e�[�^�X��Z,
            �X�e�[�^�X���Z,
            �̗͍ő�l�ϓ�,
            �X�^�~�i�񕜑��x�ϓ�,
            �A�[�}�[�񕜑��x�ϓ�,
            �^�_���[�W��Z,//�������Ƃɕ����邩
            ��_���[�W��Z,//���G�o���A�͔�_���[�W100�J�b�g�ɂ��邩
            �K�[�h�_���[�W�ϓ�,
            �K�[�h���ϓ�,
            ��~,//�S���Ⓚ��
            ������,
            ���n�_���[�W����,
            ���@�֎~,
            �ړ����x�ϓ�,
            �w�C�g�ϓ�,
            ����,//���S�����̃C�x���g������Αh��
            ����U������,//����̍U������������B�J�E���^�[�Ƃ� ���ߍU���Ƃ�
            �A�C�e���̌��ʕϓ�,
            �I�u�W�F�N�g����,//�o���A�Ƃ��T�e���C�g�H
            ���ʎ��ԕω�,//�f�o�t�t���O���^�Ȃ�f�o�t�B�ϓ����_�Ŏ��Ԃ��ω�����悤�ɁB�������Ŕ{��������


            /// 
            /// �����ŃX�e�[�^�X�Ƃ������Ă����H
            /// �{���Ɖ��Z�������������Ă�����
            /// ����ŁA������X�e�[�^�X�ɔ��f����`
            /// �X�e�[�^�X�����̂͒P��̐��l�ŁA������enum�ŋ敪�����������l�������Ă���
            /// �A������
            /// �ǂ��Ŏg�p���邩�ŋ敪�����悤��
            /// �w���X�Ŏg����Ƃ��͂�����x�܂Ƃ߂ăr�b�g�������悤�ɂ��悤
            /// 

            ��b�X�e�[�^�X����,//�̗́A�ő�̗́A�X�^�~�i�A�A�[�}�[
            �v���C���[�X�e�[�^�X����,
            �V�X�^�[�X�e�[�^�X����,
            �U���͕ϓ�����,//�Ƃ͂�����A�U���͂����Z���ď�Z���ď�Z���؂ꂽ�琔�l�ς���ˁH�@���Z�Ə�Z�͐��l�̏ꍇ������ׂ��ł́H
            �h��͕ϓ�����,//�����h��Ƃ����
            �K�[�h���\�ϓ�����,
            �^�_���[�W�{���ϓ�,//�����ŕ�������
            ��_���[�W�{���ϓ�,
            �A�N�V�����ω�,
            �g���K�[�C�x���g,//�U�����A�K�[�h���A�p���B���A�񕜎��A������Ȃǂ̓���g���K�[�̃C�x���g
            �A�C�e�����ʕϓ�,
            ����o�t,//�����ŁA�o���A�����A�o�t���ʎ��ԉ����A�����ȂǁB�J�E���^�[�U���{���Ƃ�������ɂ��邩
            ����f�o�t,//��~�Ƃ��B��~�̓X�^���̏I�����A�j���̏I���ł͂Ȃ���Ԉُ�̏I���܂łƂ���B���@�֎~���H
            ��Ԉُ�~��,//�łƂ����������̂����܂��Ă����ߒ�
            ��Ԉُ����

        }


        #region �C�x���g�^�C�v���Ƃ̌��ʂ̑ΏۑI���i�ؗ͂Ƃ��B�W�F�l���b�N����j


        #region �L�����N�^�[���ʂ̗v�f


        /// <summary>
        /// ��b�X�e�[�^�X�ɕϓ����ʂ�^���鎞
        /// ���̑Ώۂ�I��
        /// </summary>
        public enum BaseStatusChangeSelect
        {
            HP�ϓ�,//��Z���Z�܂�
            MP�ϓ�,
            MP�񕜑��x�ϓ�,//����̓V�X�^�[���񂩂�
            �ő�HP�ϓ�,
            �ő�MP�ϓ�,
            �ő�X�^�~�i�ϓ�,//�����܂ŏ�Z���Z�܂�
            �X�^�~�i�񕜑��x�ϓ�,
            �X�^�~�i����{��,//�K�[�h�X�^�~�i�����͊܂܂Ȃ�
            �ő�A�[�}�[�ϓ�,
            �A�[�}�[�񕜑��x�ϓ�,
            �w�C�g�{���ϓ�,
            �o�t���ʌ��ʎ��ԕω�,
            �f�o�t���ʌ��ʎ��ԕω�
        }


        /// <summary>
        /// �U���͂Ɩh��́A���Ƃ̓V�[���h�̔{���ϓ�
        /// ��_���[�W�A�^�_���[�W�̑������Ƃ̔{������̔{���ɂ��g��
        /// ����Ƃ��֌W�Ȃ����������_���[�W�v�Z��U���͌v�Z��
        /// ��Z�{������Z���Ƃ��ĉ�������悤�ɐ��l������
        /// 
        /// �U�����h�䂩�̓C�x���g�^�C�v�ł킩���
        /// </summary>
        public enum DamageStatusSelect
        {
            �S��,
            ����,
            �a��,
            �Ō�,
            �h��,
            ��,
            ��,
            ��,
            ��,
            �A�[�}�[���{��//����̓V�[���h���ƃV�[���h�̍���R�ɂȂ�B�܂��S�̂Ɋ܂܂Ȃ�
                �@�@�@�@�@�@//��_���[�W��^�_���[�W�{���ł͐G��Ȃ�

        }



        /// <summary>
        /// �A�N�V�����ɕω����N���鎞�̑Ώۂ̑I�ѕ�
        /// </summary>
        public enum ActionChangeSelect
        {
            �ړ����x�{���ϓ�,
            ��������Q���ԕϓ�,
            �W�����v�񐔕ϓ�,

        }


        /// <summary>
        /// ����o�t�̌��ʑI��
        /// </summary>
        public enum SpecialBuffSelect
        {
            ��������,
            ����,//���S�����̃C�x���g������Αh��
            �A�C�e���̌��ʕϓ�,
            �I�u�W�F�N�g����,//�o���A�Ƃ��T�e���C�g�H�@�o���A����̂ɃT�e���C�g�͖����B�d�������B�������Ă���o�^�H

        }

        public enum SpecialDebuff
        {
            ��~,
            ���@�֎~,

        }


        /// <summary>
        /// ���l���~�ς��Ĕ��ǂ���n��
        /// </summary>
        public enum RestoreEffect
        {
            �Œ~��,
        }


        /// <summary>
        /// ���炩�̓���Ȃ��������Ŕ�������C�x���g
        /// </summary>
        public enum TriggerEvent
        {
            �U����,
            ���j��,
        }



        #endregion

        #region �v���C���[�ŗL�̗v�f



        /// <summary>
        /// �v���C���[�X�e�[�^�X���ϓ������ꍇ��
        /// �Ώۂ̎��ʂɗ��p����
        /// </summary>
        public enum PlayerStatusSelect
        {
            �S�\�͒l,
            ������,
            ���v��,
            ����,
            �ؗ�,
            �Z��,
            ����

        }



        /// <summary>
        /// �A�C�e���g�p���Ɍ��͂��ϓ�����
        /// ���̑Ώۂ����߂�
        /// </summary>
        public enum ItemEffectSelect
        {
            �񕜃A�C�e��,
            �U���A�C�e��
        }



        #endregion




        #endregion





        /// <summary>
        /// �d�ˊ|���s�\�̌ŗL�G�t�F�N�g
        /// ���ʂ����������ŁA�Ȃ����d�����ł��Ȃ����́i�łƂ��͒P��ł������H�j
        /// ��������ƁA��̓I�ɂ͂��ꂪ�Ȃ��ȊO���Ɠo�^�����
        /// �Ȃ��ȊO�̎��������ʂ���������ĂȂ����𑖍�����
        /// �����ăL�����Z������
        /// ����ŃG�t�F�N�g�Ƃ����Ǘ�����H
        /// 
        /// ���ꂪ���鎞�͂���A���ꂪ�Ȃ��Ȃ�C�x���g�^�C�v�ŃG�t�F�N�g��A�C�R�����o��
        /// 
        /// �񋓎q�ɓ��ꂽ���l�̓G�t�F�N�g�\���Ȃǂ̗D��x
        /// ���ɓ����Ă鐔�l�����ł������Ǝ��ʂł��邩����S����
        /// 
        /// �\�[�g���X�g��UI�}�l�[�W���[�̕��ɒu����
        /// ����ł�����ւ������邩��ȁc
        /// </summary>
        public enum UniqueEffect
        {
            �� = 1 << 25,
            �ғ� = 1 << 26,
            ���� = 1 << 28,
            �S�� = 1 << 27,
            ���� = 1 << 24,
            ���� = 1 << 23,
            �߂܂� = 1 << 21,
            ���� = 1 << 22,
            ���� = 1 << 12,
            �^�_���[�W�f�o�t = 1 << 17,//����͂������d�v������G�t�F�N�g�K�v
            ��_���[�W�f�o�t = 1 << 18,//����͂������d�v������G�t�F�N�g�K�v
            �ő�HP�ቺ = 1 << 16,
            �X�e�[�^�X�ቺ = 1 << 7,
            �U���͒ቺ = 1 << 10,
            �h��͒ቺ = 1 << 11,
            ������ = 1 << 4,//�A�N�V������ړ����x�n
      �@�@�@�A�C�e�����ʒቺ_�֎~ = 1 << 2,

            ���W�F�l = 1 << 18,
            MP���W�F�l = 1 << 17,
            ���� = 1 << 20,
            �j�� = 1 << 19,
            �B�� = 1 << 5,
            �^�_���[�W�o�t = 1 << 15,//����͂������d�v������G�t�F�N�g�K�v
            ��_���[�W�o�t = 1 << 14,//����͂������d�v������G�t�F�N�g�K�v
            �ő�HP�㏸ = 1 << 13,
            �X�e�[�^�X�㏸ = 1 << 6,
            �U���͏㏸ = 1 << 9,
            �h��͏㏸ = 1 << 8,
            ���ꋭ�� = 1 << 3,//�A�N�V������ړ����x�n
            �A�C�e�����ʌ��� = 1 << 1,


            �Ȃ� = 0
        }




        #endregion



        #region �I������


        /// <summary>
        /// ���ʂ��I���������
        /// 
        /// ����R���e�i�Ɛ��������ʂɎ������N���X�ɂ��ĕ��������g�ݍ��킹����悤�ɂ���H
        /// ���Ƃ���Ƃ͕ʂɔ����������厖����Ȃ��H
        /// ���Ԃ��ƂƂ������Ƃ��ϔC�i�U���������Ƃ��j�F�X���邶����
        /// 
        /// </summary>
        public enum EventEndCondition
        {
            ����,
            ����,
            ����_�ĕt�^,
            ����_���C�x���g�d����,//�Ⴆ�Ώj�����ʒ��Ɏw��񐔍ēx�t�^����Ə����ď㏑�������B1��Ȃ�d�������Ȃ�
            �g�p��,
            �g�p��_�C�x���g�d����,//�g�p�񐔂Ō��Ȃ�����w��񐔍ĕt�^���ꂽ��㏑��

            ����_��x�g�p_�d���֎~,//���Ԃ��o����x�g�p���邩�d�����邩

            �t�^��,//���񓯂��n���̌��ʒǉ��ł��邩�̐���
            ���l����,//�ł̒~�ςƂ��͎��Ԍo�߂Ō��葱����B���Z�l���[���ɂȂ�Ώ���
            �����ύX,
            �i��//���ʂ��x�e�܂ŉi��
        }


        /// <summary>
        /// �C�x���g�̏I������
        /// �ǂꂩ��ł���������ƏI���
        /// 
        /// ���ԂƐ��l�~�ς͕K���z���0�ɔz�u
        /// </summary>
        public class EventEndJudge
        {
            /// <summary>
            /// �I������
            /// </summary>
            [Header("�I������")]
            public EventEndCondition endCondition;


            /// <summary>
            /// �g�p�񐔁A���邢�͌p�����ԂȂ�
            /// �I�������ɂ���Ă͎g��Ȃ�
            /// 
            /// �~�όn�̏ꍇ�A�����ɒ~�ϒl������
            /// </summary>
            [Header("���ʏI�����鐧���B���邢�͐��l�~�ϗ�")]
            public float effectLimit;

            /// <summary>
            /// �����𐔂���
            /// �Œ~�ςƂ��~�ϐ��l�n�͒~�ς�����ɐ�����
            /// 0��100�Ō��ʏI���B100�Ȃ�łɕω�
            /// </summary>
            [HideInInspector]
            public float limitCounter;


            /// <summary>
            /// ���l���Z�n��
            /// </summary>
            /// <returns></returns>
            public bool ConditionCheck()
            {
                if(endCondition == EventEndCondition.�g�p�� || endCondition == EventEndCondition.�t�^��)
                {
                limitCounter++;

                //�I���񐔂ɒB���Ă邩���m�F
                //�����Ă���I������
                return limitCounter >= effectLimit;
                }
                

                return true;
            }


            /// <summary>
            /// ���ʎ��Ԓ����Ă邩��Ԃ����
            /// </summary>
            /// <param name="nowTime"></param>
            /// <returns></returns>
            public bool TimeCheck(float nowTime)
            {
                //0�Ȃ珉��������
                if(limitCounter == 0)
                {
                    limitCounter = nowTime;
                }

                return (nowTime - limitCounter) >= effectLimit;
            }


            /// <summary>
            /// ���l�~�όn�̏����Ǘ��Ɏg��
            /// ���l�ω��ɉ����ďI�����ǂ���������
            /// </summary>
            /// <param name="isAdd"></param>
            /// <param name="changeValue"></param>
            public bool RestoreConditionController(bool isAdd,float changeValue)
            {
                
                //���Ԍ���
                if (!isAdd)
                {

                    limitCounter -= changeValue;

                    return limitCounter <= 0;
                }
                else
                {
                    limitCounter += changeValue;

                    return limitCounter >= 100;
                }
            }


        }


        #endregion




        #region�@���̃N���X�ł̃G�t�F�N�g�L�^�A�Ǘ��p


        /// 
        /// �����Ƃ��Ă̓x�[�X�N���X�Ƃ��ďI�����������ʊǗ�
        /// ���s���ɋ��������Ԑ��ŕω�����悤��
        /// �l�̎擾�Ȃǂ�����
        /// �C�x���g�^�C�v���ƂɈ�ϐ�������
        /// �ł̒~�ςƂ��͂����Ɖ��Z���Ă���
        /// �����ĉ��Z����������łɕω�
        /// ���Z�l���~�ϗʂŏ�Z�l�����ǎ��̃_���[�W��
        /// 
        /// 
        /// �j���Ƃ��̕������ʂ����d�ˊ|���֎~�̂�͂ǂ����悤
        /// 
        /// 
        /// ���傢�҂Ă�
        /// �^�C�v�Ə�����I�����������ۑ�������
        /// �C�x���g�^�C�v�Ɛ��l��r�b�g�����������
        /// ��ۑ����K�v�Ȃ̂ł�
        /// 
        /// ���Ƀt���O�n�Ȃ�ăC�x���g�^�C�v�ƑΏۑI�𐔒l���������Ă�΂�����
        /// ����ł����l�͂��ꂼ��̏I�������Ɣ{���Ƃ����Z�������ĂȂ��ƃ_����
        /// 
        /// ���������邩
        /// �t���O�n�̓C�x���g�^�C�v�ɂ��ϐ����
        /// ����̓r�b�g���Z�Ńt���O�Ǘ�
        /// ���₻��Ȃ��ƂȂ�
        /// �C�x���g�^�C�v���ƂɑS�Ẵt���O�ɂ��ꂼ�ꂢ���ʂ��؂�邩���L�^���Ȃ���
        /// ���Ⴀ�t���O����ƂɏI���������L�^�����
        /// 
        /// 
        /// ���l�n�̓C�x���g�ƑΏۈ���Ƃɐ��l�̕ϓ��܂ŋL�^����
        /// 
        /// 
        /// �t���O�n
        /// �C�x���g�^�C�v���ƂɃ^�C�v�̃t���O�������I���������L�^����
        /// �Ȃ��t���O�Ĕ��s���͑O�̏����Ɠ���ւ��Ō��ʉ���
        /// 
        /// ���l�n
        /// �C�x���g�^�C�v���ƂɑΏۂ̐��������X�g�����\��������H
        /// ����Ƃ����ׂĈ�̃��X�g�ɂ܂Ƃ߂邩�A�������悤
        /// ���̏�őΏۂ��Ƃ̔{���������݂ǂ��Ȃ��Ă邩���L�^����
        /// 
        /// �C�x���g�^�C�v���ƂɏI�������Ɛ��l�ƑΏۂ��߂����X�g���
        /// ���ꂼ��̑Ώۂ̐��l�L�^���Ώۂ̐�
        /// 
        /// ///


        #region�@�ۑ�����G�t�F�N�g�f�[�^

        /// <summary>
        /// ����͂��̃A�r���e�B���Ǘ�����
        /// �X�e�[�^�X�Ȃǂ̕ϓ����L�^���āA��������ؗ͂Ƃ��ʂɎQ�Ƃ���
        /// �v���C���[�L�����N�^�[�Ȃ�X�e�[�^�X�Ƃ��A�C�e���t���O�Ƃ��̕ω����󂯓������悤�Ɏ��O�ɗp�ӂ��Ă����A�����
        /// ConditionDataBase<CharaStatus>�I�Ȃ̍���Ă� 
        /// 
        /// �K�v�Ȃ̂͌��{�̐��l�ɃA�N�Z�X����@�\
        /// ������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class ConditionDataBase
        {

            /// <summary>
            /// �R���X�g���N�^����
            /// </summary>
            /// <param name="uniqueType"></param>
            /// <param name="eventType"></param>
            /// <param name="selectCondition"></param>
            /// <param name="endCondition"></param>
            /// <param name="effectLimit"></param>
            public ConditionDataBase(UniqueEffect uniqueType, EventType eventType, int selectCondition, EventEndJudge[] endCondition)
            {
                this.uniqueType = uniqueType;

                this.eventType = eventType;

                this.selectCondition = selectCondition;

                this.endCondition = endCondition;

                endConditionNum = endCondition.Length;
            }

            /// <summary>
            /// �������ʂ������j�[�N�G�t�F�N�g�̈ꕔ�ł��邩
            /// ���������Ȃ�Ǘ��������ς��
            /// �I�������ł͏d�˂������������ʂɊĎ�����
            /// 
            /// ���j�[�N�C�x���g������ꍇfor���̒��ŃC�x���g�^�C�v�ŃG�t�F�N�g���΂��Ȃ�
            /// �Ō�Ƀ��j�[�N�C�x���g�ŃG�t�F�N�g��A�C�R�����o��
            /// </summary>
            public UniqueEffect uniqueType;


            /// <summary>
            /// �C�x���g�̃^�C�v
            /// ���j�[�N�C�x���g����`����Ă��Ȃ���΂���ŃA�C�R���Ƃ��o��
            /// </summary>
            public EventType eventType;


            /// <summary>
            /// ����̃^�C�v�̃C�x���g�̒���
            /// �I������������enum����int�ɕϊ����Ă���
            /// </summary>
            public int selectCondition;


            /// <summary>
            /// �I������
            /// </summary>
            public EventEndJudge[] endCondition;

            /// <summary>
            /// �I�������̐�
            /// </summary>
            int endConditionNum;


            /// <summary>
            /// ���̏������폜����Ă邩
            /// �폜����Ă�Ȃ玞���҂��⎞�����ʂȂǑS���ł��؂�
            /// </summary>
            [HideInInspector]
            public bool isDelete;

            /// <summary>
            /// ����̃C�x���g������̏I�������𖞂��������ǂ���
            /// �m�F����
            /// �^�U�l�͐^�̏ꍇ
            /// 
            /// 
            /// </summary>
            public bool ConditionCheck(int selectType, EventEndCondition checkCondition)
            {
                //�r�b�g���Z�ɂ���
                //�����w��ł���悤��
                if ((selectType & selectCondition) > 0)
                {
                    return false;
                }


                
                for(int i = 0; i < endConditionNum; i++)
                {
                    //�g�p�񐔂��g�p�����Ɋ܂܂�Ă�Ȃ�Ԃ�
                    if (endCondition[i].endCondition == checkCondition)
                    {
                        //�g�p�𔽉f
                        //�����ă��~�b�g�����Ă��炱�̃C�x���g������
                        return endCondition[i].ConditionCheck();

                    }
                }

                return false;
            }



            /// <summary>
            /// ���̃C�x���g�̎��Ԃ��o�߂��Ă邩�ǂ���
            /// �A�r���e�B�{�̂������I�ɌĂяo��
            /// </summary>
            public bool TimeCheck(int nowTime)
            {


                for (int i = 0; i < endConditionNum; i++)
                {
                    //�g�p�񐔂��g�p�����Ɋ܂܂�Ă�Ȃ�Ԃ�
                    if (endCondition[i].endCondition == EventEndCondition.����)
                    {
                        //�g�p�𔽉f
                        //�����ă��~�b�g�����Ă��炱�̃C�x���g������
                        return endCondition[i].TimeCheck(nowTime);

                    }
                }

                return false;
            }



        }

        /// <summary>
        /// ���l����n�̏�ԕω����Ǘ����邽�߂̃N���X
        /// ����ɐ��l�����i�[���Ă���
        /// </summary>
        public class ConditionDataValue�@: ConditionDataBase
        {

            /// <summary>
            /// �R���X�g���N�^
            /// </summary>
            /// <param name="uniqueType"></param>
            /// <param name="eventType"></param>
            /// <param name="selectCondition"></param>
            /// <param name="endCondition"></param>
            /// <param name="efectLimit"></param>
            /// <param name="value"></param>
            /// <param name="isAdd"></param>
            public ConditionDataValue(UniqueEffect uniqueType, EventType eventType, int selectCondition, EventEndJudge[] endCondition,float value,bool isAdd) : base(uniqueType, eventType, selectCondition, endCondition)
            {
                changeValue = value;

                this.isAdd = isAdd;
            }

            /// <summary>
            /// ���̃C�x���g�łǂ̐��l���ǂ��ς���Ă邩�̋L�^
            /// 
            /// 
            /// y�͉��Z���ꂽ���l
            /// ���ꂪ�ύX���ꂽ��w���X�Ƃ��̐��l�̖{�̂�����
            /// �@�\�ɃA�N�Z�X
            /// 
            /// x�͏�Z���ꂽ���l
            /// ���ړI�̐��l�����{�ł��邩�A�Ƃ������Ɏg��
            /// ���ꂪ�ω����邽�тɃw���X�Ƃ��ɃA�N�Z�X����
            /// addValue�̐��l�����������l��ς���
            /// 0�{�ɂȂ��ɂ͂����Ȃ��̂Ŋ�{1��
            /// </summary>
            public float changeValue;

            /// <summary>
            /// ���ꂪ�^�Ȃ���Z�l
            /// </summary>
            public bool isAdd;


        }


        #endregion


        #region�@�f�[�^��ێ�������ꕨ






        /// <summary>
        /// �C�x���g�^�C�v����ƂɎ���
        /// 
        /// �C�x���g�^�C�v���ƂɏI�������Ɛ��l�ƑΏۂ��߂����X�g���
        /// ���ꂼ��̑Ώۂ̐��l�L�^���Ώۂ̐�����
        /// 
        /// </summary>
        public class ValueEventHolder
        {
            /// <summary>
            /// �C�x���g�^�C�v���ƂɏI�������Ɛ��l�ƑΏۂ��߂����X�g
            /// </summary>
            public List<ConditionDataValue> events;


            /// <summary>
            /// ����͑Ώۂ̊e���ڂ̐��l���ǂ��Ȃ��Ă邩�̋L�^�p
            /// 
            /// 
            /// y�͉��Z���ꂽ���l
            /// ���ꂪ�ύX���ꂽ��w���X�Ƃ��̐��l�̖{�̂�����
            /// �@�\�ɃA�N�Z�X
            /// 
            /// x�͏�Z���ꂽ���l
            /// ���ړI�̐��l�����{�ł��邩�A�Ƃ������Ɏg��
            /// ���ꂪ�ω����邽�тɃw���X�Ƃ��ɃA�N�Z�X����
            /// addValue�̐��l�����������l��ς���
            /// 0�{�ɂȂ��ɂ͂����Ȃ��̂Ŋ�{1��
            /// </summary>
            public Vector2[] valueArray;


            /// <summary>
            /// �ێ����Ă���C�x���g��
            /// </summary>
            int eventCount;

            /// <summary>
            /// �G�t�F�N�g�Ǘ��A�r���e�B
            /// </summary>
            public ConditionAndEffectControllAbility myConditionAbility;


            /// <summary>
            /// �ŏ�����enum��int�ɕϊ����Ĉ����ɗ^���悤
            /// �����Œǉ��񍐂����Ēǉ��������̃C�x���g������
            /// </summary>

            public void SetValue(ConditionDataValue data)
            {
                //���Z����łȂ��Ȃ�
                //��Z
                if (!data.isAdd)
                {
                    //�{���Ɋ|���Z������
                    valueArray[data.selectCondition].x *= data.changeValue;
                }
                else
                {
                    //�����Z�ŉ��Z���𑫂�
                    valueArray[data.selectCondition].y += data.changeValue;
                }

                //���X�g�ɒǉ��������ɂ��
                //�O�����C�x���g�������ԂŎO�܂ł����t�^�ł��Ȃ��̂��ǉ����ꂽ��ǂ��Ȃ�񂾂낤�H
                //����͎l�ڂŒǉ����Ă���
                //����������ɒǉ����ꂽ�������
                EndConditionCheck(data.selectCondition, EventEndCondition.�t�^��);


                if (data.endCondition[0].endCondition == EventEndCondition.����)
                {
                    
                }

                //���X�g�ɒǉ�
                events.Add(data);

                eventCount++;

                //���j�[�N�C�x���g������Ȃ�ǉ�����
                //���j�[�N�C�x���g�͊e���@�═�킪�܂ތ��ʂ̒��ŏے��I�Ȉ�ɂ�������悤��
                //�U���ƃK�[�h���グ��悤�Ȍ��ʂł��A�U���A�b�v�����ɂ������j�[�N�C�x���g����������
                if (data.uniqueType != UniqueEffect.�Ȃ�)
                {
                    myConditionAbility.EffectAndIconAdd(data.uniqueType);
                }


            }


            /// <summary>
            /// �ŏ�����enum��int�ɕϊ����Ĉ����ɗ^���悤
            /// 
            /// ���݂̔{���𔽉f�����I��Ώۂ��Ƃ̐��l��Ԃ�
            /// ���ʐ؂�̎��w���X����Ăяo��
            /// </summary>
            /// <param name="addSet"></param>
            /// <param name="selectCondition"></param>
            /// <param name="value"></param>
            public float GetValue(int selectCondition, float originalValue,bool isUse = false)
            {
                if (isUse)
                {

                }

                //�{����������
                originalValue *= valueArray[selectCondition].x;

                //���Z����
                return originalValue + valueArray[selectCondition].y;


            }



            #region �I���Ǘ��n

            /// <summary>
            /// �g�p�C�x���g�����ŏ����`�F�b�N
            /// �g�p�����Ώۂ̓Z���N�g�R���f�B�V�����Ō���
            /// �C�x���g�R���f�B�V�����͏�ԊǗ��@�\���̂��󂯎���Ă����ɃC�x���g���΂�
            /// 
            /// �U�����̎g�p�ł͕��������ł̎g�p�Ȃǂ��l������
            /// �̂�selectCondition�͕����r�b�g���Z�\
            /// 
            /// ���؂Ƃ��Ă�
            /// �R���g���[������ԊǗ��X�N���v�g���C�x���g�^�C�v����z���_�[������o�����z���_�[����g�p�C�x���g��΂�
            /// 
            /// �Ή����Ă�̂�
            /// �����A�g�p�񐔁A�t�^��
            /// </summary>
            /// <param name="useCondition">�g�p�����C�x���g�B�����r�b�g����ă��V</param>
            public void EndConditionCheck(int useCondition,EventEndCondition endCondition)
            {

                for (int i = 0; i < eventCount; i++)
                {
                    //�C�x���g�Ɏg�p�C�x���g��`���Ă���
                    if (events[i].ConditionCheck(useCondition,endCondition))
                    {
                        //�I�������𒴂��Ă������
                        EventEnd(events[i]);
                    }
                }
            }


            ///�@�~�ό����Ƃ��̃`�F�b�N�͕�����
            ///�@�~�σC�x���g�͌ŗL�łȂ���΂Ȃ�Ȃ��̂ŁA�����̎��_�ŕ�����



            /// <summary>
            /// ��b�Ɉ����x���ԏ�������������Ă��邩���m�F����
            /// </summary>
            /// <param name="nowTime"></param>
            public void TimeCheck(int nowTime)
            {
                for (int i = 0; i < eventCount; i++)
                {
                    //�C�x���g�ɒǉ��C�x���g��`���Ă���
                    if (events[i].TimeCheck(nowTime))
                    {
                        //�I�������̒ǉ��񐔂𒴂��Ă������
                        EventEnd(events[i]);
                    }
                }
            }



            #endregion


            /// <summary>
            /// �C�x���g�̏I�����\�b�h
            /// 
            /// �~�σC�x���g�̔��ǂȂǂ͂����ł͂��Ȃ����ƂɂȂ���
            /// </summary>
            /// <param name="data"></param>
              public void EventEnd(ConditionDataValue data)
            {
                //�f�[�^���폜���ꂽ��Ԃɂ���
                data.isDelete = true;

                //�C�x���g�폜
                events.Remove(data);

                eventCount--;


                //���j�[�N�C�x���g������Ȃ�
                if(data.uniqueType != UniqueEffect.�Ȃ�)
                {
                    bool isUniqueEnd = true;

                    for (int i = 0; i < eventCount; i++)
                    {
                        //�܂��������j�[�N�C�x���g�c���Ă邩
                        if (events[i].uniqueType == data.uniqueType)
                        {
                            isUniqueEnd = false;
                            break;
                        }
                    }

                    //�������̃��j�[�N�C�x���g���c���ĂȂ��Ȃ�
                    if (isUniqueEnd)
                    {
                        myConditionAbility.ConditionDelete(data.uniqueType);
                    }
                }


            }



            #region ���l�~�όn�Ǘ�


            /// <summary>
            /// ���l��~�ς��Ă���
            /// �����Ĕ�������n�̏�Ԉُ���̃`�F�b�N
            /// 
            /// ��Ԉُ�񕜂͂��̌��ʂ��󂯎������
            /// �I�������Ƃ͓Ɨ������`�ŏ���
            /// </summary>
            public bool SetRestoreCondition(ConditionDataValue data)
            {
                //���łɂ��邩
                //���Ȃ炷�łɂ���
                int isContain = -1;

                if (eventCount > 0)
                {

                    for (int i = 0; i < eventCount; i++)
                    {
                        //�C�x���g�ɒǉ��C�x���g��`���Ă���
                        if (events[i].selectCondition == data.selectCondition)
                        {
                            isContain = i;
                            break;
                        }
                    }
                }

                //���Ă͂܂��Ԉُ�~�ς��܂��Ȃ��Ȃ�
                //�V���ɓo�^
                if(isContain == -1)
                {
                    //�~�ϒl���Z

                    //�����~�ϒl�𖞂������Ȃ甭��
                    bool isOver = data.endCondition[0].RestoreConditionController(true, data.endCondition[0].effectLimit);

                    if (isOver)
                    {
                        //����
                        RestoreEffectStart(data.selectCondition);
                        
                        //�ꔭ���ǂȂ�~�ϒǉ��͂����ɖ߂�
                        return false;
                    }

                    //����ɃC�x���g��������
                    events.Add(data);
                    eventCount++;

                    return true;
                    
                }
                //���Ă͂܂��Ԉُ�~�ς����ɂ���Ȃ�
                //�����ɐ��l��ǉ�
                //����Ɍ��ʂ��㏑������
                else
                {
                    //���ʂ��㏑��
                    events[isContain].changeValue = data.changeValue;

                    //����ɐ��l��������
                    //�����~�ϒl�𖞂������Ȃ甭��
                    bool isOver = events[isContain].endCondition[0].RestoreConditionController(true,data.endCondition[0].effectLimit);

                    if (isOver)
                    {
                        
                        RestoreEffectStart(events[isContain].selectCondition);

                        //�~�σC�x���g�I��
                        EventEnd(events[isContain]);
                    }
                    return false;
                }



            }


            ///�@���Ԍo�߂Œ~�ό���������邼
            ///�@
            /// �K�v�Ȃ���
            /// 
            /// ���ԊԊu���ƂɌ��ʂ𔭊�������
            /// ���ԊԊu���Ƃɐ��l���������Ă������
            /// 
            /// �ĂP�A���ԊǗ��n�̃C�x���g���ꊇ�œ���郊�X�g�Ɋi�[���Ĉ�b���Ƃ�for������
            /// �ĂQ�A�ċA�֐��̔񓯊����\�b�h���΂��Ď��Ԍv���A���łɏ����Ă���I��
            /// 
            /// �Q���ȁB���ꂼ��^�C�~���O���Ⴄ��
            /// 
            /// 
            /// ///
            




            /// <summary>
            /// ���l�~�σC�x���g�̔���
            /// </summary>
            void RestoreEffectStart(int selectCondition)
            {

                //�Ŕ���
                if(selectCondition == (int)RestoreEffect.�Œ~��)
                {
                    //�ŃC�x���g����

                }

            }




            #endregion

        }



        /// <summary>
        /// �t���O�Ǘ��p�̃f�[�^�i�[�N���X
        /// ����̃C�x���g�^�C�v�̃t���O�����ׂăr�b�g�ŊǗ�
        /// </summary>
        public class BooleanEventHolder
        {


            /// <summary>
            /// �C�x���g�^�C�v���ƂɏI�������ƑΏۂ��߂����X�g
            /// </summary>
            public List<ConditionDataBase> events;


            /// <summary>
            /// �C�x���g�^�C�v���܂ރt���O�̓r�b�g�ŊǗ����悤��
            /// </summary>
            public int valueBit;


            /// <summary>
            /// �ێ����Ă���C�x���g��
            /// </summary>
            int eventCount;


            /// <summary>
            /// �G�t�F�N�g�Ǘ��A�r���e�B
            /// </summary>
            public ConditionAndEffectControllAbility myConditionAbility;

            /// <summary>
            /// �ŏ�����enum��int�ɕϊ����Ĉ����ɗ^���悤
            /// </summary>
            /// <param name="selectCondition"></param>
            /// <param name="isStart">���ʊJ�n��</param>
            public void SetValue(ConditionDataBase data)
            {

                /*
                //�J�n�Ȃ�t���O��^�ɂ���
                if (isStart)
                {
                    valueBit |= 1 << selectCondition;
                }
                //�I���Ȃ甽�]�r�b�g�ł����邱�Ƃŏ��������
                else
                {
                    valueBit &= ~(1 << selectCondition);
                }
                */

                //�t���O��^�ɂ���
                valueBit |= 1 << data.selectCondition;


                //���X�g�ɒǉ��������ɂ��
                //�O�����C�x���g�������ԂŎO�܂ł����t�^�ł��Ȃ��̂��ǉ����ꂽ��ǂ��Ȃ�񂾂낤�H
                //����͎l�ڂŒǉ����Ă���
                //����������ɒǉ����ꂽ�������
                EndConditionCheck(data.selectCondition, EventEndCondition.�t�^��);

                //���X�g�ɒǉ�
                events.Add(data);

                //���j�[�N�C�x���g������Ȃ�ǉ�����
                //���j�[�N�C�x���g�͊e���@�═�킪�܂ތ��ʂ̒��ŏے��I�Ȉ�ɂ�������悤��
                //�U���ƃK�[�h���グ��悤�Ȍ��ʂł��A�U���A�b�v�����ɂ������j�[�N�C�x���g����������
                if(data.uniqueType != UniqueEffect.�Ȃ�)
                {
                    myConditionAbility.EffectAndIconAdd(data.uniqueType);
                }



                eventCount++;
            }


            /// <summary>
            /// �ŏ�����enum��int�ɕϊ����Ĉ����ɗ^���悤
            /// 
            /// ���݂̔{���𔽉f�������l��Ԃ�
            /// ���ʐ؂�̎��w���X����Ăяo��
            /// </summary>
            /// <param name="addSet"></param>
            /// <param name="selectCondition"></param>
            /// <param name="value"></param>
            public bool GetValue(int selectCondition)
            {
                //���̃r�b�g��������܂�ł��邩
                return (valueBit & 1 << selectCondition) > 0;
            }



            #region �I���Ǘ��n

            /// <summary>
            /// �g�p�C�x���g�����ŏ����`�F�b�N
            /// �g�p�����Ώۂ̓Z���N�g�R���f�B�V�����Ō���
            /// �C�x���g�R���f�B�V�����͏�ԊǗ��@�\���̂��󂯎���Ă����ɃC�x���g���΂�
            /// 
            /// �U�����̎g�p�ł͕��������ł̎g�p�Ȃǂ��l������
            /// �̂�selectCondition�͕����r�b�g���Z�\
            /// 
            /// ���؂Ƃ��Ă�
            /// �R���g���[������ԊǗ��X�N���v�g���C�x���g�^�C�v����z���_�[������o�����z���_�[����g�p�C�x���g��΂�
            /// 
            /// �Ή����Ă�̂�
            /// �����A�g�p�񐔁A�t�^��
            /// </summary>
            /// <param name="useCondition">�g�p�����C�x���g�B�����r�b�g����ă��V</param>
            public void EndConditionCheck(int useCondition, EventEndCondition endCondition)
            {

                for (int i = 0; i < eventCount; i++)
                {
                    //�C�x���g�Ɏg�p�C�x���g��`���Ă���
                    if (events[i].ConditionCheck(useCondition, endCondition))
                    {
                        //�I�������𒴂��Ă������
                        EventEnd(events[i]);
                    }
                }
            }


            ///�@�~�ό����Ƃ��̃`�F�b�N�͕�����
            ///�@�~�σC�x���g�͌ŗL�łȂ���΂Ȃ�Ȃ��̂ŁA�����̎��_�ŕ�����

            /// <summary>
            /// ��b�Ɉ����x���ԏ�������������Ă��邩���m�F����
            /// </summary>
            /// <param name="nowTime"></param>
            public void TimeCheck(int nowTime)
            {
                for (int i = 0; i < eventCount; i++)
                {
                    //�C�x���g�ɒǉ��C�x���g��`���Ă���
                    if (events[i].TimeCheck(nowTime))
                    {
                        //�I�������̒ǉ��񐔂𒴂��Ă������
                        EventEnd(events[i]);
                    }
                }
            }


            ///�@��邱��
            ///�@�~�ό����n�̏����쐬�i��b�Ɉ��B�܂��C�x���g�ǉ����ɏ���������B�~�ς��A�N�e�B�u�Ȃ�����̃C�x���g�ɐ��l�𑫂��悤�Ȍ`�B�ϐ��̓w���X�Œ~�ϒl�����炵�Ă������ɔ�΂��j
            ///�@�i���������Γőϐ������C�x���g�Ƃ������������H�j
            ///�@���Ƃ͑S�̊Ǘ��V�X�e�������
            ///�@�U�����Ƃ��Ɏg�p���Ƃ��̃C�x���g��΂��̂̓C�x���g�^�C�v���Ƃɂ���
            ///�@�g�p�A�t�^�A�Ȃǂ͌ʂŎg�p�悩��ʒm
            ///�@���Ԃƒ~�ςɊւ��Ă͊撣��


            #endregion


            /// <summary>
            /// �C�x���g�̏I�����\�b�h
            /// �����œŒ~�ςƂ��͓łɕς��
            /// </summary>
            /// <param name="data"></param>
            public void EventEnd(ConditionDataBase data)
            {
                //�f�[�^���폜���ꂽ��Ԃɂ���
                data.isDelete = true;

                //�C�x���g�폜
                events.Remove(data);
                eventCount--;


                //���j�[�N�C�x���g������Ȃ�
                if (data.uniqueType != UniqueEffect.�Ȃ�)
                {
                    bool isUniqueEnd = true;

                    for (int i = 0; i < eventCount; i++)
                    {
                        //�܂��������j�[�N�C�x���g�c���Ă邩
                        if (events[i].uniqueType == data.uniqueType)
                        {
                            isUniqueEnd = false;
                            break;
                        }
                    }

                    //�������̃��j�[�N�C�x���g���c���ĂȂ��Ȃ�
                    if (isUniqueEnd)
                    {
                        myConditionAbility.ConditionDelete(data.uniqueType);
                    }
                }
            }


            #region ���l�~�όn�Ǘ�


            /// <summary>
            /// ���l��~�ς��Ă���
            /// �����Ĕ�������n�̏�Ԉُ���̃`�F�b�N
            /// 
            /// ��Ԉُ�񕜂͂��̌��ʂ��󂯎������
            /// �I�������Ƃ͓Ɨ������`�ŏ���
            /// </summary>
            public bool SetRestoreCondition(ConditionDataBase data)
            {
                //���łɂ��邩
                //���Ȃ炷�łɂ���
                int isContain = -1;

                if (eventCount > 0)
                {

                    for (int i = 0; i < eventCount; i++)
                    {
                        //�C�x���g�ɒǉ��C�x���g��`���Ă���
                        if (events[i].selectCondition == data.selectCondition)
                        {
                            isContain = i;
                            break;
                        }
                    }
                }

                //���Ă͂܂��Ԉُ�~�ς��܂��Ȃ��Ȃ�
                //�V���ɓo�^
                if (isContain == -1)
                {
                    //�~�ϒl���Z

                    //�����~�ϒl�𖞂������Ȃ甭��
                    bool isOver = data.endCondition[0].RestoreConditionController(true, data.endCondition[0].effectLimit);

                    if (isOver)
                    {
                        //����
                        RestoreEffectStart(data.selectCondition);

                        //�ꔭ���ǂȂ�~�ϒǉ��͂����ɖ߂�
                        return false;
                    }

                    //����ɃC�x���g��������
                    events.Add(data);
                    eventCount++;

                    return true;

                }
                //���Ă͂܂��Ԉُ�~�ς����ɂ���Ȃ�
                //�����ɐ��l��ǉ�
                //����Ɍ��ʂ��㏑������
                else
                {

                    //����ɐ��l��������
                    //�����~�ϒl�𖞂������Ȃ甭��
                    bool isOver = events[isContain].endCondition[0].RestoreConditionController(true, data.endCondition[0].effectLimit);

                    if (isOver)
                    {

                        RestoreEffectStart(events[isContain].selectCondition);

                        //�~�σC�x���g�I��
                        EventEnd(events[isContain]);
                    }
                    return false;
                }



            }


            ///�@���Ԍo�߂Œ~�ό���������邼
            ///�@
            /// �K�v�Ȃ���
            /// 
            /// ���ԊԊu���ƂɌ��ʂ𔭊�������
            /// ���ԊԊu���Ƃɐ��l���������Ă������
            /// 
            /// �ĂP�A���ԊǗ��n�̃C�x���g���ꊇ�œ���郊�X�g�Ɋi�[���Ĉ�b���Ƃ�for������
            /// �ĂQ�A�ċA�֐��̔񓯊����\�b�h���΂��Ď��Ԍv���A���łɏ����Ă���I��
            /// 
            /// �Q���ȁB���ꂼ��^�C�~���O���Ⴄ��
            /// 
            /// 
            /// ///





            /// <summary>
            /// ���l�~�σC�x���g�̔���
            /// </summary>
            void RestoreEffectStart(int selectCondition)
            {

                //�Ŕ���
                if (selectCondition == (int)RestoreEffect.�Œ~��)
                {
                    //�ŃC�x���g����

                }

            }




            #endregion



        }


        #endregion



        #endregion

    #region ����▂�@�A�A�C�e���Ȃǂ̊O���N���X�Ō��ʂ��Ǘ����邽�߂̒�`



        /// <summary>
        /// �N�������C�x���g���Ǘ����邽�߂̕�
        /// �L�^���ĉ������邽�߂�
        /// 
        /// �񐔏��������ꍇ�͎g�p�񍐂��󂯂�
        /// �����O�ꂽ���Ƀo�t�������󂯂�
        /// ���Ԑ؂�f�o�t���؂��
        /// 
        /// �K�v�Ȃ���
        /// �E��������
        /// �E�L�^�p�ϐ�
        /// �E�C�x���g�^�C�v eventType
        /// �E��p����Ώ� selectCondition
        /// �E��p���e�i���l�̏ꍇ�͉��Z���{������������t���O�����j 
        /// 
        /// �ύX�_�͂��̃A�r���e�B�Ńf�[�^��������
        /// ����͐ݒ�Ŏg�����߂ɂ���
        /// ����̏�Ƃ��Ō��͂Ƃ�������Ō��߂�
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class ConditionDataContainerBoolean<T> where T : struct, IComparable, IConvertible, IFormattable
        {


            /// <summary>
            /// �������ʂ������j�[�N�G�t�F�N�g�̈ꕔ�ł��邩
            /// ���������Ȃ�Ǘ��������ς��
            /// �I�������ł͏d�˂������������ʂɊĎ�����
            /// 
            /// ���j�[�N�C�x���g������ꍇfor���̒��ŃC�x���g�^�C�v�ŃG�t�F�N�g���΂��Ȃ�
            /// �Ō�Ƀ��j�[�N�C�x���g�ŃG�t�F�N�g��A�C�R�����o��
            /// </summary>
            [Header("����Ȍ��ʂ̏����ݒ�")]
            public UniqueEffect uniqueType;


            /// <summary>
            /// �C�x���g�̃^�C�v
            /// ���j�[�N�C�x���g����`����Ă��Ȃ���΂���ŃA�C�R���Ƃ��o��
            /// </summary>
            [Header("���ʂ̕���")]
            public EventType _type;


            /// <summary>
            /// ����̃^�C�v�̃C�x���g�̒���
            /// �I������������enum����int�ɕϊ����Ă���
            /// </summary>
            [Header("�I���������")]
            public T selectCondition;

            /// <summary>
            /// �I������
            /// </summary>
            [Header("�I�������B���Ԃƒ~�ς͔z���0��")]
            public EventEndJudge[] endCondition;



        }

        /// <summary>
        /// �N�������C�x���g���Ǘ����邽�߂̕�
        /// �L�^���ĉ������邽�߂�
        /// 
        /// �񐔏��������ꍇ�͎g�p�񍐂��󂯂�
        /// �����O�ꂽ���Ƀo�t�������󂯂�
        /// ���Ԑ؂�f�o�t���؂��
        /// 
        /// �K�v�Ȃ���
        /// �E��������
        /// �E�L�^�p�ϐ�
        /// �E�C�x���g�^�C�v eventType
        /// �E��p����Ώ� selectCondition
        /// �E��p���e�i���l�̏ꍇ�͉��Z���{������������t���O�����j 
        /// 
        /// �ύX�_�͂��̃A�r���e�B�Ńf�[�^��������
        /// ����͐ݒ�Ŏg�����߂ɂ���
        /// ����̏�Ƃ��Ō��͂Ƃ�������Ō��߂�
        /// 
        /// 
        /// ���l�ύX���i�邱����ł͌��ʗʂȂǂ��ǉ������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class ConditionDataContainerFloat<T> : ConditionDataContainerBoolean<T> where T : struct, IComparable, IConvertible, IFormattable
        {
            /// <summary>
            /// ��Z���ʂł��邩�ǂ���
            /// </summary>
            public bool multipler;

            /// <summary>
            /// ��̓I�Ȍ��ʗ�
            /// </summary>
            public float effectAmount;

        }

        #endregion



        #endregion
        /// ���̃��\�b�h�́A�w���v�{�b�N�X�̃e�L�X�g��\�����邽�߂ɂ̂ݎg�p����܂��B
        /// �\�͂̃C���X�y�N�^�̖`���ɂ���
        public override string HelpBoxText() { return "��Ԉُ�܂ރf�o�t��o�t���Ǘ�����"; }




        /// <summary>
        /// �C�x���g�z���_�[�̃f�B�N�V���i���[
        /// �K�v�ɉ����ėv�f���J�X�^��
        /// �I�[�f�B���ŃV���A���C�Y���悤��
        /// </summary>
        [SerializeField]
        Dictionary<EventType, ValueEventHolder> valueEvents;

        /// <summary>
        /// �C�x���g�z���_�[�̃f�B�N�V���i���[
        /// �K�v�ɉ����ėv�f���J�X�^��
        /// �I�[�f�B���ŃV���A���C�Y���悤��
        /// </summary>
        [SerializeField]
        Dictionary<EventType, BooleanEventHolder> booleanEvents;




        /// <summary>
        /// �X�v���C�g�V�F�[�_�[�̃G�t�F�N�g
        /// �K�v�ɉ�����SetActive
        /// �I�[�f�B���ŃV���A���C�Y���悤��
        /// </summary>
        [SerializeField]
        Dictionary<UniqueEffect, GameObject> effectDictionary;

        /// <summary>
        /// �v�f���[���ȏ�̐��l�n�̃C�x���g�̐�
        /// </summary>
        int valueEventsCount;

        /// <summary>
        /// �v�f���[���ȏ�̃t���O�n�̃C�x���g�̐�
        /// </summary>
        int booleanEventsCount;


        #region ���j�[�N�G�t�F�N�g�֘A�̕ϐ�

        /// <summary>
        /// ����Ńr�b�g���Z����
        /// </summary>
        int uniqueSetChecker;

        /// <summary>
        /// ���j�[�N�G�t�F�N�g�̐�
        /// </summary>
        readonly int uniqueEffectCount = 28;

        UniqueEffect nowMainEffect;



        #endregion

        /// <summary>
        ///�@�����ŁA�p�����[�^������������K�v������܂��B
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();


            //�z���_�[�ɃA�r���e�B�ւ̎Q�Ƃ�^����
            foreach(ValueEventHolder holder in valueEvents.Values)
            {
                holder.myConditionAbility = this;
            }

            foreach (BooleanEventHolder holder in booleanEvents.Values)
            {
                holder.myConditionAbility = this;
            }

        }

            //���ʏ����n�C�x���g�͌ʂɃC�x���g�󂯎���邩
            //�����������̃z���_�[�����邩�ǂ����̃`�F�b�N�Ƃ���
            //���ƃf�[�^�폜�����̃f�[�^�����Ƃɖ߂���Ƃ������Ȃ���

        #region �R���f�B�V�����ǉ�

        /// <summary>
        /// �C�x���g��ݒ肷�邽�߂̃��\�b�h
        /// �t���O�Ǘ��̂��߂̃I�[�o�[���[�h
        /// ���ʂ̃G�t�F�N�g��v������͕̂ʂ̏���
        /// </summary>
        /// <param name="uniqueType"></param>
        /// <param name="eventType"></param>
        /// <param name="selectCondition"></param>
        /// <param name="endCondition"></param>
        /// <param name="efectLimit"></param>
        /// <param name="value"></param>
        /// <param name="isAdd"></param>
        public void ConditionSetting(UniqueEffect uniqueType,EventType eventType,int selectCondition, EventEndJudge[] endCondition,float effectLimit)//,float value,bool isAdd)
        {



            ConditionDataBase data = new ConditionDataBase(uniqueType,eventType,selectCondition,endCondition);


            //��������I�������ɉ����Ď��ԃJ�E���g�Ƃ����n�߂�

            //�I�������ɏ��X�ɐ��l���������܂܂�Ă�Ȃ�
            if (endCondition[0].endCondition == EventEndCondition.���l����)
            {
                if (valueEvents[eventType].SetRestoreCondition(data))
                {
                    EffectValueDecrease(data);
                }
                return;
            }
            //�I�����ԂɎ��ԃJ�E���g����Ȃ琧�����ԊJ�n
            else if (data.endCondition[0].endCondition == EventEndCondition.����)
            {
                LimitWait(data, true).Forget();
            }

            //��������C�x���g�^�C�v���Ƃɕ�����BooleanEventHolder�������Ă���
            //�l�����X�g�i�t���O�̏ꍇ�z��ł������j�Ɋi�[���āAsetValue�܂�
            booleanEvents[eventType].SetValue(data);
        }

        /// <summary>
        /// �C�x���g��ݒ肷�邽�߂̃��\�b�h
        /// ���l�Ǘ��̂��߂̃I�[�o�[���[�h
        /// </summary>
        /// <param name="uniqueType"></param>
        /// <param name="eventType"></param>
        /// <param name="selectCondition"></param>
        /// <param name="endCondition"></param>
        /// <param name="efectLimit"></param>
        /// <param name="value"></param>
        /// <param name="isAdd"></param>
        public void ConditionSetting(UniqueEffect uniqueType, EventType eventType, int selectCondition, EventEndJudge[] endCondition,float value,bool isAdd)
        {

            ConditionDataValue data = new ConditionDataValue(uniqueType, eventType, selectCondition, endCondition,value,isAdd);

            if (endCondition[0].endCondition == EventEndCondition.���l����)
            {
                if (valueEvents[eventType].SetRestoreCondition(data))
                {
                    EffectValueDecrease(data);
                }
                return;
            }
            //�������ԊJ�n
            else if (data.endCondition[0].endCondition == EventEndCondition.����)
            {
                LimitWait(data,true).Forget();
            }

            //��������C�x���g�^�C�v���Ƃɕ�����Holder�������Ă���
            //�l�����X�g�Ɋi�[���āAsetValue�܂�
            valueEvents[eventType].SetValue(data);
        }

        #endregion


        #region �A�C�R���A�G�t�F�N�g�Ǘ�



        /// <summary>
        /// �G�t�F�N�g�ƃA�C�R���ǉ�
        /// �G�t�F�N�g�̓f�B�N�V���i���[�ŊǗ�
        /// �A�C�R����isPlayer�Ȃ�N��
        /// UI�X�N���v�g��UniqueEvent��n���Č���
        /// </summary>
        void EffectAndIconAdd(UniqueEffect addEffect)
        {
            //�G�t�F�N�g���Ȃ����A���Ɋ܂܂�Ă���Ȃ�
            if(((int)addEffect & uniqueSetChecker) > 0)
            {
                return;
            }

            //�G�t�F�N�g��ǉ�
            uniqueSetChecker |= (int)addEffect;

            //�����ăG�t�F�N�g���Z�b�g
            //���ǃA�C�R���X�V�őS�����邩�珈���͋��ʂł���
            //���̎��̈�ԗD��x��������Ƃ����Ȃ��Ă���
            EffectAndIconSet();

        }


        /// <summary>
        /// ���j�[�N�G�t�F�N�g�̏��Ŏ��ɂ����Ȃ�����
        /// �܂��A�C�R��������
        /// �����ăA�C�R���̒��ŐV�����L���ɂȂ���̂�T��
        /// 
        /// �G�t�F�N�g������
        /// �����Ĉ�ԗD��x�������G�t�F�N�g��T��
        /// �D��x�͊�{�f�o�t�����ɂȂ�悤�ɂ���H
        /// 
        /// �A�C�R��������Ĉ�����
        /// �Ȃ̂ŃA�C�R�����G�t�F�N�g���������݈�ԗD��x�����G�t�F�N�g�����߂Ă�
        /// ���ƍ����̂���ꂽ������ւ���@�\�����Ȃ��Ƃ�
        /// 
        /// �C�׃��g����������A�������j�[�N�C�x���g���Ȃ���΂���Ă�ō폜����
        /// </summary>
        void ConditionDelete(UniqueEffect deleteEffect)
        {


            //���]�r�b�g�ŃG�t�F�N�g���폜
            uniqueSetChecker &= ~(int)deleteEffect;

            EffectAndIconSet();
        }




        /// <summary>
        /// �G�t�F�N�g�ƃA�C�R���Ǘ�
        /// �G�t�F�N�g�̓f�B�N�V���i���[�ŊǗ�
        /// �A�C�R����isPlayer�Ȃ�N��
        /// UI�X�N���v�g��UniqueEvent��n���Č���
        /// </summary>
        void EffectAndIconSet()
        {
            int maxBitPoint;

            //���j�[�N�G�t�F�N�g���Ȃ�����Ȃ��Ȃ�
            if (uniqueSetChecker > 0)
            {
            //�D��x�ő�̃r�b�g�͉����ڂ�
            maxBitPoint = MaxBitCheck(0);

            //�r�b�g�ŃG�t�F�N�g���Z�b�g����
            EffectSetExe((UniqueEffect)(1 << maxBitPoint));

            }
            else
            {
                maxBitPoint = -1;
            }


            //���������̓v���C���[����
            if (!isPlayer)
            {
                return;
            }


            //�A�C�R���͔��܂łƂ���
            for (int i = 0; i < 8; i++)
            {
                //�Ԃ�l���^�ŁA�܂����̃A�C�R�����Z�b�g����]�n������Ȃ�
                if (MainUICon.instance.ConditionIconSet(maxBitPoint,i))
                {
                    //1�������Ƃň���炷
                    maxBitPoint = MaxBitCheck(uniqueEffectCount - maxBitPoint + 1);
                }
                else
                {
                    break;
                }
            }
        }


        /// <summary>
        /// �܂Ƃ����ʃG�t�F�N�g�����ۂɂ���
        /// 
        /// </summary>
        void EffectSetExe(UniqueEffect effect)
        {
            if(effect == nowMainEffect)
            {
                return;
            }

            //�O�̂�������
            effectDictionary[nowMainEffect].SetActive(false);

            //���̂�����
            effectDictionary[effect].SetActive(true);

            nowMainEffect = effect;

        }




        /// <summary>
        /// �X�^�[�g�|�W�V��������T���āA�������ڂɍő�̃r�b�g������̂���m�郁�\�b�h
        /// </summary>
        /// <param name="startPoint"></param>
        /// <returns></returns>
        int MaxBitCheck(int startPoint)
        {
            //���[�v���鐔���`�F�b�N
            int length = uniqueEffectCount - startPoint;

            if(length < 0)
            {
                return -1;
            }

            //�ŏ��Ɏg���r�b�g
            int useBit = (1 << startPoint);

            for (int i = 0; i < length; i--)
            {
                //����r�b�g�����炵�Ă���
                if(((useBit << i) & uniqueSetChecker) > 0)
                {
                    //���Ă͂܂�r�b�g�������ڂɂ��邩��Ԃ�
                    return (uniqueEffectCount - (i + startPoint));
                }

            }

            return -1;
        }





        #endregion



        #region ���ԊǗ�


        /// <summary>
        /// �������ԑ҂�
        /// ���ʏI�����Ă����߂�
        /// ����͑S������
        /// </summary>
        async UniTaskVoid LimitWait(ConditionDataBase data,bool isValue)
        {

            //���ԑ҂�
            //���Ԍn�̂�͕K���ŏ��̏����ɂ���
            await UniTask.Delay(TimeSpan.FromSeconds(data.endCondition[0].effectLimit), cancellationToken:destroyCancellationToken);

            //�폜����Ă�Ȃ�
            if (data.isDelete)
            {
                //�v�f��j�����Ė߂�
                data = null;

                return;

            }

            //�q�N���X����e�N���X�ւƃL���X�g�����Ƃ��Ă�
            //���̎q�N���X�̗v�f�ւ̎Q�Ƃ͕ϐ���������ĂȂ�
            //�Q�ƌ^������B�Ȃ�Ń��X�g����̍폜�Ɏg����

            //���l�f�[�^�Ȃ�
            if (isValue)
            {
                valueEvents[data.eventType].EventEnd((ConditionDataValue)data);
            }
            //�t���O�f�[�^�Ȃ�
            else
            {
                booleanEvents[data.eventType].EventEnd(data);
            }

        }

        /// <summary>
        /// �����Ő��l�~�ϗʂ����炷
        /// �łƂ��̒~�ϗʂ�
        /// 
        /// ������͒~�ϗʂȂ̂ŁA���l�f�[�^�ȊO�̑��݂��z�肵�Ă���
        /// �ǂ�����Ē~�ς̏I��������z�񂩂�ǂݎ�邩
        /// 0�ɌŒ肷��H
        /// </summary>
        public async UniTaskVoid EffectValueDecrease(ConditionDataBase data,float waitTime, bool isValue)
        {

            //���ԑ҂�
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: destroyCancellationToken);

            //�폜����Ă�Ȃ�
            if (data.isDelete)
            {
                //�v�f��j�����Ė߂�
                data = null;

                return;

            }


            //�~�ς����x�����Ă���
            //���������߂�
            if (data.endCondition[0].limitCounter >= 100)
            {
                return;
            }


            //�l�����炷�B
            //���炷�ʂ��l�����邩�A���邢�͌��炷���Ԃ̕ω������őΉ����邩
            data.endCondition[0].limitCounter -= 10;



            //���l���[���ȉ��ɂȂ�����p��
            if (data.endCondition[0].limitCounter <= 0)
            {
                //���l�f�[�^�Ȃ�
                if (isValue)
                {
                    valueEvents[data.eventType].EventEnd((ConditionDataValue)data);
                }
                //�t���O�f�[�^�Ȃ�
                else
                {
                    booleanEvents[data.eventType].EventEnd(data);
                }

                return;
            }

            //�܂������Z�ł���Ȃ�ċA�Ăяo��
            EffectValueDecrease(data,waitTime,isValue).Forget();

        }



        /// <summary>
        /// �Ȃ�Ő��l�f�[�^�����Ȃ̂�
        /// ����́A�������ʂŉ��x���g���ĈӖ�������̂͐��l�f�[�^�����Ȃ�����
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        async UniTaskVoid EffectTimeWait(ConditionDataValue data)
        {

            //���ԑ҂�
            await UniTask.Delay(TimeSpan.FromSeconds(data.endCondition[0].effectLimit), cancellationToken: destroyCancellationToken);

            //�폜���ꂽ�Ȃ�
            if (data.isDelete)
            {
                //�v�f��j�����Ė߂�
                data = null;

                return;

            }

            //�ċA�Ăяo��
            EffectTimeWait(data).Forget();
        }


        #endregion





















        #region �M�~�b�N

        /// <summary>
        /// �A�C�e���̌��ʂ��L��
        /// </summary>
        /// <param name="data"></param>
        void GimickAct(EventData data, Collider2D collision)
        {
            Debug.Log($"tttttt{data.type}");
            //��������
            if (data.effectTime == 0 || data.type == EventType.scoreGet)
            {
                if (data.type == EventType.damage)
                {
                    //���G�Ȃ�
                    if (_myStatus.invincible)
                    {
                        //���G�̉��炷�H

                        return;
                    }

                    //�X�R�A��10�ւ�
                    ScoreManager.instance.ScoreChange(-100);
                    bool isDie = ScoreManager.instance.LifeChange(true);

                    StopAction();

                    gameObject.layer = 8;

                    if (isDie)
                    {
                        ScoreManager.instance.isDie = true;
                        _myStatus.Die = true;
                        efCon.ActionChange(CharacterController.CharacterState.none);
                        efCon.ConditionChange(CharacterCondition.die);
                    }
                    else
                    {
                        efCon.ConditionChange(CharacterCondition.damage);
                    }
                }
                else if (data.type == EventType.recover)
                {
                    efCon.PlaySound("Recover", ScoreManager.instance.PlayerPosi);
                    ScoreManager.instance.LifeChange();
                    //�񕜃G�t�F�N�g
                    efCon.ConditionChange(CharacterCondition.heal);
                }

                else if (data.type == EventType.scoreGet)
                {
                    efCon.PlaySound("ScoreUp", ScoreManager.instance.PlayerPosi);
                    ScoreManager.instance.ScoreChange((int)data.effectTime);
                }
                else if (data.type == EventType.Random)
                {

                }
            }

            //���Ԏ�������
            else
            {
                //���G�Ȃ�o�b�h�X�e�[�^�X�͎󂯕t���Ȃ�
                if (_myStatus.invincible && data.bad)
                {
                    //���G�̉��炷�H
                    return;
                }

                ConditionContraller(data);
            }


        }

        void ConditionContraller(EventData data)
        {



            if (_effectData.Any())
            {
                for (int i = 0; i < _effectData.Count; i++)
                {
                    if (data.type == _effectData[i].type)
                    {
                        //   Debug.Log($"��dsasdasdwewer{data.type}{_effectData[i].type}{_effectData.Count}");
                        return;
                    }
                }
            }

            if (!data.bad)
            {
                //  Debug.Log("wsdaewer");
                if (data.type == EventType.invincible)
                {
                    _myStatus.invincible = true;
                    SpriteEffect.SetActive(true);
                    //  Debug.Log($"��er{_myStatus.invincible}");
                    //���G�̉��炷?
                    MasterAudio.PlaySound3DAtVector3AndForget("Incivle", transform.position);
                }
                else if (data.type == EventType.boostJump)
                {
                    _myStatus.boostJump = true;
                    efCon.BuffChange(ExtraCondition.boostJump);
                }


                /*
                             else if (data.type == EventType)
                {

                }
                 */
            }
            else
            {
                if (data.type == EventType.badSight)
                {
                    MasterAudio.PlaySound3DAtVector3AndForget("BadSight", transform.position);
                    BeautifySettings.settings.blurIntensity.value = 1.2f;
                }
            }

            data.timer = Time.time;
            //  Debug.Log($"������{data.type}{data.timer}");
            _effectData.Add(data);

        }


        /// <summary>
        /// �N�����ɏ�Ԃ����ɖ߂�
        /// </summary>
        void ConditionRecovery(EventData[] dataArray)//, GimickCondition visualData)
        {


            for (int i = 0; i < dataArray.Length; i++)
            {
                if (!dataArray[i].bad)
                {
                    //  Debug.Log("wsdaewer");
                    if (dataArray[i].type == EventType.invincible)
                    {
                        _myStatus.invincible = true;
                        SpriteEffect.SetActive(true);
                        //  Debug.Log($"��er{_myStatus.invincible}");
                        //���G�̉��炷?
                        MasterAudio.PlaySound3DAtVector3AndForget("Incivle", transform.position);
                    }
                    else if (dataArray[i].type == EventType.boostJump)
                    {
                        _myStatus.boostJump = true;
                        efCon.BuffChange(ExtraCondition.boostJump);
                    }

                }
                else
                {
                    if (dataArray[i].type == EventType.badSight)
                    {
                        MasterAudio.PlaySound3DAtVector3AndForget("BadSight", transform.position);
                        BeautifySettings.settings.blurIntensity.value = 1.2f;
                    }
                }
                dataArray[i].timer = Time.time;
                //  Debug.Log($"������{dataArray[i].type}{dataArray[i].timer}");
                _effectData.Add(dataArray[i]);
            }


        }




        /// <summary>
        /// ��Ԃ̎��Ԃ��͂���
        /// </summary>
        void ConditionTimer()
        {
            if (_effectData.Any())
            {
                for (int i = 0; i < _effectData.Count; i++)
                {

                    //���Ԓ����������
                    //�܂��͍����G�ŏ�Ԃ�邢��Ȃ�
                    if (Time.time - _effectData[i].timer > _effectData[i].effectTime || _myStatus.invincible && _effectData[i].bad)
                    {
                        //   Debug.Log($"wwwdwd{_effectData[i].type}{Time.time - _effectData[i].timer > _effectData[i].effectTime}{_effectData[i].timer}");
                        ConditionEnd(_effectData[i]);
                        _effectData.Remove(_effectData[i]);
                    }

                }
            }

        }

        /// <summary>
        /// �����Ԃ��I��点��
        /// </summary>
        void ConditionEnd(EventData data)
        {

            if (data.bad)
            {
                if (data.type == EventType.badSight)
                {
                    BeautifySettings.settings.blurIntensity.value = 0f;
                }
            }
            else
            {
                if (data.type == EventType.invincible)
                {
                    _myStatus.invincible = false;
                    SpriteEffect.SetActive(false);
                    gameObject.layer = 0;

                }
                else if (data.type == EventType.boostJump)
                {
                    _myStatus.boostJump = false;

                }
            }


        }

        public async UniTask DieRecover()
        {

            if (_effectData.Any())
            {
                int count = _effectData.Count;
                for (int i = 0; i < count; i++)
                {
                    ConditionEnd(_effectData[i]);
                }
            }
            efCon.ConditionChange(CharacterCondition.none);
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            this.gameObject.layer = 0;
            _myStatus.Die = false;
        }


        #endregion

    }
}