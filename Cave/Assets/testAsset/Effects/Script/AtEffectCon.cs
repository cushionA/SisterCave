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
        public enum Element
        {
            slash,
            stab,
            strike,
            holy,
            dark,
            fire,
            thunder,
            none
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
                MyCode.SoundManager.instance.AdditionalEffect(adType,transform,sizeMultipler);
            }
            //�ʏ�U��
            if (level != AttackValue.AttackLevel.Special)
            {
                _useData.level = level;
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
                MyCode.SoundManager.instance.GeneralAttack(transform,(int)_useData.level,(int)_useData.element,(int)_useData.type,sizeMultipler);
            }
            else
            {
                for (int i = 0;i < useList.Count;i++)
                {
                    if(useList[i].callNumber == num)
                    {
                        if(useList[i].sound != null)
                        {
                            GManager.instance.PlaySound(useList[i].sound, transform.position);
                        }
                        if(useList[i].particle != null)
                        {
                            if (useList[i].isFollow)
                            {
                                atPool.Spawn(useList[i].particle, transform.position, transform.rotation, transform);
                            }
                            else
                            {
                                atPool.Spawn(useList[i].particle, transform.position, transform.rotation);
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
            castEffect = MyCode.SoundManager.instance.CastEfCall((int)level, element, transform);
        }

        public void CastEnd(AttackValue.AttackLevel level, Element element)
        {
            MyCode.SoundManager.instance.CastEfClear(castEffect,(int)level,element,transform);
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
            useList.Clear(); 
            
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
                atPool.CreatePrefabPool(_newPrefab[i]);
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
                magicPool.CreatePrefabPool(_newPrefab[i]);
            }
        }



        #endregion




        #endregion


    }
}
