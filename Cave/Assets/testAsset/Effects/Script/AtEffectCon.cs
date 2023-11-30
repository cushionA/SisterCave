using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using UnityEngine.AddressableAssets;
using Rewired.Integration.CorgiEngine;
using DarkTonic.MasterAudio;
using Cysharp.Threading.Tasks;
using System.Linq;
using System;
using System.Threading;
using PathologicalGames;


namespace MoreMountains.CorgiEngine // you might want to use your own namespace here
{

    //�����܂��Ȑ݌v
    ///�@<summary>
    //���̋@�\�͉�����G�t�F�N�g���Đ����邽�߂̋@�\�ł�
    ///�@</summary>
    ///�@
    [AddComponentMenu("Corgi Engine/Character/Abilities/AtEffectCon")]
    public class AtEffectCon : MyAbillityBase
    {

        /// ���̃��\�b�h�́A�w���v�{�b�N�X�̃e�L�X�g��\�����邽�߂ɂ̂ݎg�p����܂��B
        public override string HelpBoxText() { return "���̋@�\�͉�����G�t�F�N�g���Đ����邽�߂̋@�\�ł��B"; }


        //�t�B�[���h
        //�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\-�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\
        #region

        //��`
        #region

        /// <summary>
        /// �����̗񋓌^
        /// </summary>
        [Flags]
        public enum Element
        {
            �a������ = 1 << 0,
            �h�ˑ��� = 1 << 1,
            �Ō����� = 1 << 2,
            ������ = 1 << 3,
            �ő��� = 1 << 4,
            ������ = 1 << 5,
            ������ = 1 << 6,
            �� = 1 << 7,
            �Z�H = 1 << 8,
            ���� = 1 << 9,
            �ړ����x�ቺ�U�� = 1 << 10,
            �U���͒ቺ�U�� = 1 << 11,
            �h��͒ቺ�U�� = 1 << 12,
            ��_���� = 1 << 13,//�G�̎�_�������T�[�`���đ���Ɏg��
            �w��Ȃ� = 0
        }

        public enum AdditionalType
        {
            normal,
            dispariable,
            chance
        }

        /// <summary>
        /// �U���̏��
        /// </summary>
        public struct AttackInfo
        {
            public AttackValue.AttackLevel level;
            public int adType;
            public Element element;
            public AttackValue.MotionType type;
        }

        [Serializable]
        public struct EffectAndSound
        {
            [Header("�g�p����G�t�F�N�g")]
            public ParticleSystem particle;

            [Header("�G�t�F�N�g�����Ă��邩�ǂ���")]
            public bool isFollow;

            [SoundGroup]
            [Header("�Ăяo���T�E���h�̖��O")]
            public string sound;

            [Header("�A�j���C�x���g�ŌĂяo���ԍ�")]
            public int callNumber;
        }


        #endregion


        //�C���X�y�N�^�Őݒ�
        #region

        /// <summary>
        /// �G�t�F�N�g�𐶐�����N���X
        /// �I�u�W�F�N�g�v�[���@�\
        /// Spawn�Ő����B�G�t�F�N�g�̍Đ����I������Ǝ����ŏ���
        /// </summary>
        [SerializeField]
        SpawnPool atPool;

        /// <summary>
        /// �G�t�F�N�g�𐶐�����N���X
        /// �I�u�W�F�N�g�v�[���@�\
        /// Spawn�Ő����B�G�t�F�N�g�̍Đ����I������Ǝ����ŏ���
        /// </summary>
        [SerializeField]
        SpawnPool magicPool;


        [Header("�G�t�F�N�g�̃T�C�Y�{��")]
        /// <summary>
        /// �G�t�F�N�g�̃T�C�Y�{��
        /// </summary>
        public float sizeMultipler = 1;


        [SerializeField]
        Transform effectPosi;

        #endregion



        
        #region �����X�e�[�^�X

        AttackInfo _useData;

        /// <summary>
        /// �U���Ɏg���G�t�F�N�g�ƃT�E���h
        /// </summary>
        List<EffectAndSound> useList;

        /// <summary>
        /// �g�p���̉r���G�t�F�N�g��ێ�
        /// </summary>
        Transform castEffect;


        #endregion




        #endregion


        //���\�b�h
        //�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\-�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\
        #region

        /// <summary>
        ///�@�����ŁA�p�����[�^������������K�v������܂��B
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();

        }


        #region �U���֘A�̃��\�b�h

        /// <summary>
        /// �U���̃f�[�^���擾����
        /// </summary>
        /// <param name="level"></param>
        /// <param name="adType"></param>
        /// <param name="element"></param>
        /// <param name="type"></param>
        public void EffectPrepare(AttackValue.AttackLevel level, int adType, Element element, AttackValue.MotionType type)
        {

            if (adType != 0)
            {
                MyCode.SoundManager.instance.AdditionalEffect(adType,effectPosi,sizeMultipler);
            }
            //�ʏ�U��
            _useData.level = level;
            if (level != AttackValue.AttackLevel.Special)
            {
                
                _useData.adType = adType;
                _useData.element = element;
                _useData.type = type;
            }
        }


        /// <summary>
        /// �U���G�t�F�N�g���ĂԃA�j���C�x���g
        /// </summary>
        /// <param name="num"></param>
        public void AttackEfEvent(int num = 0)
        {
            //�ʏ�U��
            if (_useData.level != AttackValue.AttackLevel.Special)
            {
                //������
                if (_useData.level == AttackValue.AttackLevel.SEOnly)
                {
                    MyCode.SoundManager.instance.GAttackS(effectPosi.position, (int)_useData.level, (int)_useData.element, (int)_useData.type, sizeMultipler);
                }
                //�G�t�F�N�g�t��
                else
                {
                    MyCode.SoundManager.instance.GeneralAttack(effectPosi, (int)_useData.level, (int)_useData.element, (int)_useData.type, sizeMultipler);
                }
            }
            else
            {
                if (useList != null && useList.Any())
                {
                    for (int i = 0; i < useList.Count; i++)
                    {
                        if (useList[i].callNumber == num)
                        {
                            if (useList[i].sound != null)
                            {
                                GManager.instance.PlaySound(useList[i].sound, effectPosi.position);
                            }
                            if (useList[i].particle != null)
                            {
                                if (useList[i].isFollow)
                                {
                                    atPool.ControlSpawn(useList[i].particle, effectPosi.position, effectPosi.rotation, effectPosi);
                                }
                                else
                                {
                                    atPool.Spawn(useList[i].particle, effectPosi.position, effectPosi.rotation);
                                }
                            }

                        }

                    }
                }


            }
        }





        #endregion





        #region ���@�G�t�F�N�g�̍Đ�


        public void CastStart(AttackValue.AttackLevel level, Element element)
        {
            castEffect = MyCode.SoundManager.instance.CastEfCall((int)level, element, effectPosi);
        }

        public void CastEnd(AttackValue.AttackLevel level, Element element)
        {
            MyCode.SoundManager.instance.CastEfClear(castEffect,(int)level,element);
            castEffect = null;
        }

        public void CastStop(AttackValue.AttackLevel level, Element element)
        {
            MyCode.SoundManager.instance.CastStop(castEffect, (int)level, element);
            castEffect = null;
        }

        public Transform BulletCall(ParticleSystem bullet, Vector3 pos, Quaternion rotation, ParticleSystem flashEf = null)
        {
            
            if (flashEf != null)
            {
                magicPool.Spawn(flashEf,pos,rotation);
            }

            return magicPool.Spawn(bullet,pos,rotation).transform;
        }

        public void BulletClear(Transform inst)
        {
            magicPool.Despawn(inst);
        }



        #endregion


        #region�@��b�@�\

        /// <summary>
        /// �U���Ɏg���ŗL�G�t�F�N�g�̐ݒ�
        /// </summary>
        /// <param name="_newList"></param>
        /// <param name="_newPrefab"></param>
        public void ATResorceReset(List<EffectAndSound> _newList,List<PrefabPool> _newPrefab)
        {
            if (useList != null)
            {
                useList.Clear();
            }
            
            if (_newList.Any())
            {
               
                useList = _newList;

            }

            //�G�t�F�N�g�����Z�b�g
            atPool.CleanUp();

            if (!_newPrefab.Any())
            {
                return;
            }
            for (int i = 0; i < _newPrefab.Count; i++)
            {
                atPool.CreatePrefabPool(atPool.ObjectSetting(_newPrefab[i]));
            }
        }

        /// <summary>
        /// ���@�Ɏg���ŗL�G�t�F�N�g�̐ݒ�
        /// </summary>
        /// <param name="_newPrefab"></param>
        public void MagicResorceReset(List<PrefabPool> _newPrefab)
        {
            //�G�t�F�N�g�����Z�b�g
            magicPool.CleanUp();

            if (!_newPrefab.Any())
            {
                return;
            }
            for (int i = 0; i < _newPrefab.Count; i++)
            {
                magicPool.CreatePrefabPool(magicPool.ObjectSetting(_newPrefab[i]));
            }
        }



        #endregion




        #endregion


    }
}
