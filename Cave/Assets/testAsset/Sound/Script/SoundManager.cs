using System.Collections.Generic;
using UnityEngine;
using DarkTonic.MasterAudio;
using System;
using PathologicalGames;
using static MoreMountains.CorgiEngine.EffectControllAbility;
using MoreMountains.CorgiEngine;
using RenownedGames.Apex;
using RenownedGames.Apex.Serialization.Collections.Generic;
using static UnityEditor.PlayerSettings;

/// <summary>
/// �G�t�F�N�g�̃T�C�Y�{���ő̊i�\���\
/// </summary>
namespace MyCode
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager instance = null;

        /// <summary>
        /// ��`
        /// </summary>
        #region

        public struct PreviousEffect
        {
            public string prefab;
            public int state;
            public bool isStrong;
            public bool groundUse;

            // MyCharacter.GroundFeature lastGround;
        }
        public struct PreviousSound
        {
            public string name;
            public int state;
            public bool isStrong;
            public bool groundUse;
            public bool useMultipler;
            // MyCharacter.GroundFeature lastGround;
        }
        [Serializable]
        public class MyParticles : SerializableDictionary<string, ParticleSystem>
        {
            [SerializeField]
            private List<string> keys;

            [SerializeField]
            private List<ParticleSystem> values;

            protected override List<string> GetKeys()
            {
                return keys;
            }

            protected override List<ParticleSystem> GetValues()
            {
                return values;
            }

            protected override void SetKeys(List<string> keys)
            {
                this.keys = keys;
            }

            protected override void SetValues(List<ParticleSystem> values)
            {
                this.values = values;
            }
        }
        public enum SizeTag
        {
            small = 0,
            middle = 1,
            big = 2//����
        }

        #endregion

        #region �t�B�[���h


        /// <summary>
        /// �G�t�F�N�g�𐶐�����N���X
        /// �I�u�W�F�N�g�v�[���@�\
        /// Spawn�Ő����B�G�t�F�N�g�̍Đ����I������Ǝ����ŏ���
        /// </summary>
        [SerializeField]
        SpawnPool _generalPool;



        /// <summary>
        /// ������Ŏw�肷����
        /// �ėp
        /// </summary>
        public MyParticles particles = new MyParticles();


        /// <summary>
        /// �~�ς̌������x���L�^�����f�B�N�V���i��
        /// </summary>
        public Dictionary<ConditionAndEffectControllAbility.RestoreEffect, float> restoreDicreaceDict;




        /// �����ŊǗ�������
        /// ���x���Ƃ��̐����𗘗p������
        /// 
        #region




        // �U���G�t�F�N�g
        #region

        [Foldout("�U���̃G�t�F�N�g")]
        [Header("�a���̃G�t�F�N�g")]
        public ParticleSystem[] slashEf;

        [Foldout("�U���̃G�t�F�N�g")]
        [Header("�Ō��̃G�t�F�N�g")]
        public ParticleSystem[] strikeEf;

        [Foldout("�U���̃G�t�F�N�g")]
        [Header("�h�˂̃G�t�F�N�g")]
        public ParticleSystem[] stabEf;

        [Foldout("�U���̃G�t�F�N�g")]
        [Header("���a���̃G�t�F�N�g")]
        public ParticleSystem[] slashFire;

        [Foldout("�U���̃G�t�F�N�g")]
        [Header("���Ō��̃G�t�F�N�g")]
        public ParticleSystem[] strikeFire;

        [Foldout("�U���̃G�t�F�N�g")]
        [Header("���h�˂̃G�t�F�N�g")]
        public ParticleSystem[] stabFire;

        [Foldout("�U���̃G�t�F�N�g")]
        [Header("���a���̃G�t�F�N�g")]
        public ParticleSystem[] slashThunder;

        [Foldout("�U���̃G�t�F�N�g")]
        [Header("���Ō��̃G�t�F�N�g")]
        public ParticleSystem[] strikeThunder;

        [Foldout("�U���̃G�t�F�N�g")]
        [Header("���h�˂̃G�t�F�N�g")]
        public ParticleSystem[] stabThunder;

        [Foldout("�U���̃G�t�F�N�g")]
        [Header("���a���̃G�t�F�N�g")]
        public ParticleSystem[] slashHoly;

        [Foldout("�U���̃G�t�F�N�g")]
        [Header("���Ō��̃G�t�F�N�g")]
        public ParticleSystem[] strikeHoly;

        [Foldout("�U���̃G�t�F�N�g")]
        [Header("���h�˂̃G�t�F�N�g")]
        public ParticleSystem[] stabHoly;

        [Foldout("�U���̃G�t�F�N�g")]
        [Header("�Ŏa���̃G�t�F�N�g")]
        public ParticleSystem[] slashDark;

        [Foldout("�U���̃G�t�F�N�g")]
        [Header("�őŌ��̃G�t�F�N�g")]
        public ParticleSystem[] strikeDark;

        [Foldout("�U���̃G�t�F�N�g")]
        [Header("�Ŏh�˂̃G�t�F�N�g")]
        public ParticleSystem[] stabDark;

        #endregion

        //���@�G�t�F�N�g
        #region

        [Foldout("�r���̃G�t�F�N�g")]
        [Header("�a���i���j�̉r���G�t�F�N�g")]
        public ParticleSystem[] slashCEffect;

        [Foldout("�r���̃G�t�F�N�g")]
        [Header("�Ō��i��j�̉r���G�t�F�N�g")]
        public ParticleSystem[] strikeCEffect;

        [Foldout("�r���̃G�t�F�N�g")]
        [Header("�h�ˁi�X�j�̉r���G�t�F�N�g")]
        public ParticleSystem[] stabCEffect;

        [Foldout("�r���̃G�t�F�N�g")]
        [Header("���̉r���G�t�F�N�g")]
        public ParticleSystem[] fireCEffect;

        [Foldout("�r���̃G�t�F�N�g")]
        [Header("���̉r���G�t�F�N�g")]
        public ParticleSystem[] thunderCEffect;

        [Foldout("�r���̃G�t�F�N�g")]
        [Header("���̉r���G�t�F�N�g")]
        public ParticleSystem[] holyCEffect;

        [Foldout("�r���̃G�t�F�N�g")]
        [Header("�ł̉r���G�t�F�N�g")]
        public ParticleSystem[] darkCEffect;


        [Foldout("���@�����̃G�t�F�N�g")]
        [Header("�a���i���j�̖��@�����G�t�F�N�g")]
        public ParticleSystem[] slashActiEffect;

        [Foldout("���@�����̃G�t�F�N�g")]
        [Header("�Ō��i��j�̖��@�����G�t�F�N�g")]
        public ParticleSystem[] strikeActiEffect;

        [Foldout("���@�����̃G�t�F�N�g")]
        [Header("�h�ˁi�X�j�̖��@�����G�t�F�N�g")]
        public ParticleSystem[] stabActiEffect;

        [Foldout("���@�����̃G�t�F�N�g")]
        [Header("���̖��@�����G�t�F�N�g")]
        public ParticleSystem[] fireActiEffect;

        [Foldout("���@�����̃G�t�F�N�g")]
        [Header("���̖��@�����G�t�F�N�g")]
        public ParticleSystem[] thunderActiEffect;

        [Foldout("���@�����̃G�t�F�N�g")]
        [Header("���̖��@�����G�t�F�N�g")]
        public ParticleSystem[] holyActiEffect;

        [Foldout("���@�����̃G�t�F�N�g")]
        [Header("�ł̖��@�����G�t�F�N�g")]
        public ParticleSystem[] darkActiEffect;

        #endregion

        #endregion



        #region


        #endregion

        /// <summary>
        /// �T�E���h
        /// </summary>
        #region

        //�����̃T�E���h
        #region

        [SerializeField, Header("����")]
        [Foldout("�����̃T�E���h")]
        [Header("�f���̉�")]//small�͂Ђ��Ђ����Ċ����ɂ���H
        [SoundGroup]
        public String[] bareFootSound;

        [Foldout("�����̃T�E���h")]
        [Header("�f���̕��s")]
        [SoundGroup]
        public String[] bareWalkSound;

        [Foldout("�����̃T�E���h")]
        [Header("�Z���̉�")]
        [SoundGroup]
        public String[] armorFootSound;

        [Foldout("�����̃T�E���h")]
        [Header("�Z���̕�����")]
        [SoundGroup]
        public String[] armorWalkSound;

        [SerializeField, Header("�_�E���̉�")]
        [Foldout("�����̃T�E���h")]
        [Header("�_�E���̉�")]
        [SoundGroup]
        public String[] downSound;

        [Foldout("�����̃T�E���h")]
        [Header("�����̃_�E���̉�")]
        [SoundGroup]
        public String[] armorDownSound;


        [SerializeField, Header("���[�����O�̉�")]
        [Foldout("�����̃T�E���h")]
        [Header("���[�����O�̉�")]
        [SoundGroup]
        public String[] rollSound;

        [Foldout("�����̃T�E���h")]
        [Header("�������[�����O�̉�")]
        [SoundGroup]
        public String[] armorRollSound;


        [SerializeField, Header("�g���났�̉�")]
        [Foldout("�����̃T�E���h")]
        [Header("�g���났���i�U���c�Ċ����j")]
        [SoundGroup]
        public String[] shakeSound;

        [Foldout("�����̃T�E���h")]
        [Header("�����g���났�̉��i��������j")]
        [SoundGroup]
        public String[] armorShakeSound;

        [SerializeField, Header("�W�����v�̉�")]
        [Foldout("�����̃T�E���h")]
        [Header("�W�����v�̉�")]
        [SoundGroup]
        public String[] jumpSound;

        [Foldout("�����̃T�E���h")]
        [Header("�����W�����v")]
        [SoundGroup]
        public String[] armorJumpSound;


        [Foldout("�����̃T�E���h")]
        [Header("�ӂ��̃K�[�h")]
        [SoundGroup]
        public String[] guardSound;

        [Foldout("�����̃T�E���h")]
        [Header("�����K�[�h")]
        [SoundGroup]
        public String[] metalGuardSound;

        //���X�g����Ȃ��z�̓n�[�h�R�[�f�B���O�ŏ����Ă��炨





        #endregion

        //�U���̃T�E���h
        #region

        [Foldout("�U���̃T�E���h")]
        [Header("�a���̉�")]
        [SoundGroup]
        public String[] slashSe;

        [Foldout("�U���̃T�E���h")]
        [Header("�Ō��̉�")]
        [SoundGroup]
        public String[] strikeSe;

        [Foldout("�U���̃T�E���h")]
        [Header("�h�˂̉�")]
        [SoundGroup]
        public String[] stabSe;

        [Foldout("�U���̃T�E���h")]
        [Header("���̉�")]
        [SoundGroup]
        public String[] fireSe;

        [Foldout("�U���̃T�E���h")]
        [Header("���̉�")]
        [SoundGroup]
        public String[] thunderSe;

        [Foldout("�U���̃T�E���h")]
        [Header("���̉�")]
        [SoundGroup]
        public String[] holySe;

        [Foldout("�U���̃T�E���h")]
        [Header("�ł̉�")]
        [SoundGroup]
        public String[] darkSe;
        #endregion

        //���@�r���̉�
        #region

        [Foldout("�r���̃T�E���h")]
        [Header("�a���i���j�̉�")]
        [SoundGroup]
        public String[] slashCast;

        [Foldout("�r���̃T�E���h")]
        [Header("�Ō��i��j�̉�")]
        [SoundGroup]
        public String[] strikeCast;

        [Foldout("�r���̃T�E���h")]
        [Header("�h�ˁi�X�j�̉�")]
        [SoundGroup]
        public String[] stabCast;

        [Foldout("�r���̃T�E���h")]
        [Header("���̉�")]
        [SoundGroup]
        public String[] fireCast;

        [Foldout("�r���̃T�E���h")]
        [Header("���̉�")]
        [SoundGroup]
        public String[] thunderCast;

        [Foldout("�r���̃T�E���h")]
        [Header("���̉�")]
        [SoundGroup]
        public String[] holyCast;

        [Foldout("�r���̃T�E���h")]
        [Header("�ł̉�")]
        [SoundGroup]
        public String[] darkCast;


        [Foldout("���@�����̃T�E���h")]
        [Header("�a���i���j�̉�")]
        [SoundGroup]
        public String[] slashActivate;

        [Foldout("���@�����̃T�E���h")]
        [Header("�Ō��i��j�̉�")]
        [SoundGroup]
        public String[] strikeActivate;

        [Foldout("���@�����̃T�E���h")]
        [Header("�h�ˁi�X�j�̉�")]
        [SoundGroup]
        public String[] stabActivate;

        [Foldout("���@�����̃T�E���h")]
        [Header("���̉�")]
        [SoundGroup]
        public String[] fireActivate;

        [Foldout("���@�����̃T�E���h")]
        [Header("���̉�")]
        [SoundGroup]
        public String[] thunderActivate;

        [Foldout("���@�����̃T�E���h")]
        [Header("���̉�")]
        [SoundGroup]
        public String[] holyActivate;

        [Foldout("���@�����̃T�E���h")]
        [Header("�ł̉�")]
        [SoundGroup]
        public String[] darkActivate;


        #endregion



        #region ���ʃT�E���h

        [SerializeField, Header("������ʂ̉�")]
        [Foldout("���ʃT�E���h")]
        [Header("������ʂ̉�")]//small�͂Ђ��Ђ����Ċ����ɂ���H
        [SoundGroup]
        Dictionary<ConditionAndEffectControllAbility.UniqueEffect,string> conditionSound;

        #endregion


        #endregion

        #endregion



        void Awake()
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




        //����Ȃ����
        #region
        public void StepSound(bool isMetal,SizeTag _size,Transform posi)
        {
            if (isMetal)
            {
                GManager.instance.PlaySound(armorFootSound[(int)_size], posi.position);
            }
            else
            {
                GManager.instance.PlaySound(bareFootSound[(int)_size], posi.position);
            }

        }

        public void JumpSound(bool isMetal, SizeTag _size, Transform posi)
        {
            if (isMetal)
            {
                GManager.instance.PlaySound(armorJumpSound[(int)_size], posi.position);
            }
            else
            {
                GManager.instance.PlaySound(jumpSound[(int)_size], posi.position);
            }

        }
        public void ShakeSound(bool isMetal, SizeTag _size, Transform posi)
        {
            if (isMetal)
            {
                GManager.instance.PlaySound(armorShakeSound[(int)_size], posi.position);
            }
            else
            {
                GManager.instance.PlaySound(shakeSound[(int)_size],posi.position);
            }

        }
        #endregion


        //���ʌn
        #region
        /// <summary>
        /// ���ʃG�t�F�N�g�Đ�
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="state"></param>
        /// <param name="_size"></param>
        /// <param name="ground"></param>
        /// <param name="prevE"></param>
        /// <param name="sizeMultipler"></param>
        /// <returns></returns>
        public PreviousEffect GeneralEffectPlay(Transform pos,SelectState state, SizeTag _size,MyCharacter.GroundFeature ground,PreviousEffect prevE,float sizeMultipler,bool isRight)
        {
            //�O�̏����ƈ�v����Ȃ画�f���s��Ȃ�
            if (prevE.state == (int)state)
            {

                ParticleSystem ef = null;

                //�n�ʎg���X�e�[�g�ł�isStrong���ۂ��ŃG�t�F�N�g�����̑���ɐ����Ԃ��Ƃ��グ�Ă���
                //�n�ʂ̃G�t�F�N�g

                //�n�ʎg�p����A���ʏ�̒n�ʂ���Ȃ�
                if (prevE.groundUse && ground != MyCharacter.GroundFeature.Nomal)
                {
                    if (ground != MyCharacter.GroundFeature.Water)
                    {
                        if (prevE.isStrong)
                        {
                          //  ef = _generalPool.Spawn("WaterDrop", pos.position, pos.rotation);
                        }
                        else
                        {
                        //    ef = _generalPool.Spawn("WaterPiller", pos.position, pos.rotation);
                        }
                    }
                    else if (ground == MyCharacter.GroundFeature.Grass)
                    {
                        if (prevE.isStrong)
                        {
                            //tran = _generalPool.Spawn(particles[prevE.prefab], pos.position, pos.rotation).transform;
                        }
                        else
                        {
                            //tran = _generalPool.Spawn(particles[prevE.prefab], pos.position, pos.rotation).transform;
                        }
                    }
                }

                //����ȊO
                else if(prevE.prefab.Length > 0)
                {
                    ef = _generalPool.Spawn(particles[prevE.prefab], pos.position, pos.rotation);
                }


                //�T�C�Y�ύX
                Vector3 Scale = ef.transform.localScale;

                if (sizeMultipler != 1)
                {

                    Scale *= sizeMultipler;
                }
                //�L�������������Ă�Ȃ甽�΂�
                if (!isRight)
                {
                    Scale.x = Scale.x * -1;
                }
                ef.transform.localScale = Scale;


                return prevE;
            }
            //���Ȃ��Ȃ�Ĕ��f
            else
            {

                //�����Ȃǂ����������̉��𗧂ĂĂ��邩�ǂ���
                bool strong = false;

                int num = (int)state;
                prevE.groundUse = true;

                prevE.state = (int)state;


               string container = null;
                if (num <= 7)
                {
 �@�@�@�@�@�@�@�@�@�@if (state == SelectState.Running)//�n�`�A���A�ς��
                    {
                        container = "Runnning";
                    }
                    else if (state == SelectState.Jumping)//�n�`�A���A�ς��
                    {
                        container = "Jump";
                        strong = true;
                    }
                    else if (state == SelectState.DoubleJumping)//�n�`�����A�ς��
                    {
                        container = "DoubleJumping";
                        ground = MyCharacter.GroundFeature.Nomal;
                        prevE.groundUse = false;
                    }
                }
                else if (num <= 13)
                {

                    if (state == SelectState.FastFlying)//�n�`�����A�ς��
                    {
                        container = "FastFlying";
                        ground = MyCharacter.GroundFeature.Nomal;
                        prevE.groundUse = false;
                    }
                    else if (state == SelectState.Rolling)//�n�`�A���A�ς��
                    {
                        container = "Rolling";
                        strong = true;
                    }

                }
                else
                {

                    if (state == SelectState.Parry)//�n�`�����A�ς��Ȃ�
                    {
                        container = "Parry";
                        ground = MyCharacter.GroundFeature.Nomal;
                        prevE.groundUse = false;
                    }
                    else if (state == SelectState.justGuard)//�n�`�����A�ς��Ȃ�
                    {
                        container = "justGuard";
                        ground = MyCharacter.GroundFeature.Nomal;
                        prevE.groundUse = false;
                    }

                }



                //�G�t�F�N�g���Đ�

                ParticleSystem ef = _generalPool.Spawn(particles[container], pos.position, pos.rotation);
                Vector3 Scale = ef.transform.localScale;


                if (sizeMultipler != 1)
                {
                    
                    
                    Scale *= sizeMultipler;
                  //  tran.localScale = Scale;
                }
                //�L�������������Ă�Ȃ甽�΂�
                if (!isRight)
                {
                    Scale.x = Scale.x * -1;
                }
                ef.transform.localScale = Scale;

                //�O�̏���������
                prevE.prefab = container;
                prevE.isStrong = strong;

                if (ground == MyCharacter.GroundFeature.Water)
                {
                    num = strong ? 1 : 0;
                    //     GManager.instance.PlaySound(Watersound[num], pos.position);
                }
                else if (ground == MyCharacter.GroundFeature.Grass)
                {
                    num = strong ? 1 : 0;
                }
                return prevE;
            }
        }

        public PreviousSound GeneralSoundPlay(Transform pos,SelectState state, float multipler,bool isMetal,SizeTag _size, MyCharacter.GroundFeature ground,PreviousSound prevS)
        {
            //�O�̏����ƈ�v����Ȃ画�f���s��Ȃ�
            if (prevS.state == (int)state)
            {
                //�����Đ�
                if (prevS.useMultipler)
                {
                    GManager.instance.PlaySound(prevS.name, pos.position, pitch: multipler);
                }

                else
                {

                    GManager.instance.PlaySound(prevS.name, pos.position);
                }

                //�n�ʂ̉�

                if (prevS.groundUse && ground != MyCharacter.GroundFeature.Nomal)
                {
                    if (ground != MyCharacter.GroundFeature.Water)
                    {
                        if (prevS.isStrong)
                        {
                            GManager.instance.PlaySound("WaterPiller", pos.position);
                        }
                        else
                        {
                            GManager.instance.PlaySound("WaterDrop", pos.position);
                        }
                    }
                    else if (ground == MyCharacter.GroundFeature.Grass)
                    {
                        if (prevS.isStrong)
                        {
                            GManager.instance.PlaySound("WaterPiller", pos.position);
                        }
                        else
                        {
                            GManager.instance.PlaySound("WaterDrop", pos.position);
                        }
                    }
                }

                return prevS;
            }
            //���Ȃ��Ȃ�Ĕ��f
            else
            {

                //�����Ȃǂ����������̉��𗧂ĂĂ��邩�ǂ���
                bool strong = false;

                int num = (int)state;
                prevS.groundUse = true;
                prevS.useMultipler = true;
                prevS.state = num;

                string container = "";
                if (num <= 5)
                {

                    if (state == SelectState.moving)//�n�`�A���A�ς��
                    {
                        if (isMetal)
                        {
                            container = armorWalkSound[(int)_size];
                        }
                        else
                        {
                            container = bareWalkSound[(int)_size];
                        }
                    }
                    else if (state == SelectState.Running)//�n�`�A���A�ς��
                    {
                        if (isMetal)
                        {
                            container = armorFootSound[(int)_size];
                        }
                        else
                        {
                            container = bareFootSound[(int)_size];
                        }
                    }
                    else if (state == SelectState.Crawling)//�n�`�A���A�ς��
                    {
                        if (isMetal)
                        {
                            container = armorWalkSound[(int)_size];
                        }
                        else
                        {
                           container = bareWalkSound[(int)_size]; 
                        }
                    }
                    else if (state == SelectState.Crouching)//�n�`�A���A�ς��Ȃ�
                    {
                        if (isMetal)
                        {
                            container = armorShakeSound[(int)_size];
                        }
                        else
                        {
                            container = shakeSound[(int)_size];
                        }
                        prevS.useMultipler = false;
                    }
                }
                else if (num <= 9)
                {
                    if (state == SelectState.Jumping)//�n�`�A���A�ς��
                    {
                        if (isMetal)
                        {
                            container = armorJumpSound[0];
                        }
                        else
                        {
                            container = jumpSound[0];
                        }
                        strong = true;
                        prevS.useMultipler = false;
                    }
                    else if (state == SelectState.DoubleJumping)//�n�`�����A�ς��
                    {
                        if (isMetal)
                        {
                            container = armorJumpSound[(int)_size];
                        }
                        else
                        {
                            container = jumpSound[(int)_size];
                        }
                        ground = MyCharacter.GroundFeature.Nomal;
                        prevS.groundUse = false;
                        prevS.useMultipler = false;
                    }
                    else if (state == SelectState.Flying)//�n�`�����A�ς��
                    {

                        ground = MyCharacter.GroundFeature.Nomal;
                        prevS.groundUse = false;
                    }
                    else if (state == SelectState.FastFlying)//�n�`�����A�ς��
                    {
                        ground = MyCharacter.GroundFeature.Nomal;
                        prevS.groundUse = false;
                    }
                }
                else if (num <= 13)
                {
                    if (state == SelectState.Rolling)//�n�`�A���A�ς��
                    {
                        if (isMetal)
                        {
                            container = armorRollSound[(int)_size];
                        }
                        else
                        {
                            container = rollSound[(int)_size];
                        }
                        strong = true;
                    }
                    else if (state == SelectState.Guard)//�n�`�����A�ς��Ȃ�
                    {
                        if (isMetal)
                        {
                            container = armorShakeSound[(int)_size];
                        }
                        else
                        {
                            container = shakeSound[(int)_size];
                        }
                        ground = MyCharacter.GroundFeature.Nomal;
                        prevS.groundUse = false;
                        prevS.useMultipler = false;
                    }
                    else if (state == SelectState.GuardMove)//�n�`�����A�ς��Ȃ�
                    {
                        if (isMetal)
                        {
                            container = armorWalkSound[0];
                        }
                        else
                        {
                            container = bareWalkSound[0];
                        }
                    }
                }
                else
                {

                    if (state == SelectState.Parry)//�n�`�����A�ς��Ȃ�
                    {
                        container = "ParrySuccess";
                        ground = MyCharacter.GroundFeature.Nomal;
                        prevS.groundUse = false;
                        prevS.useMultipler = false;
                    }
                    else if (state == SelectState.justGuard)//�n�`�����A�ς��Ȃ�
                    {
                        container = "Blocking";
                        ground = MyCharacter.GroundFeature.Nomal;
                        prevS.groundUse = false;
                        prevS.useMultipler = false;
                    }
                    else if (state == SelectState.Wakeup)
                    {
                        if (isMetal)
                        {
                            container = armorShakeSound[1];
                        }
                        else
                        {
                            container = shakeSound[1];
                        }
                        ground = MyCharacter.GroundFeature.Nomal;
                        prevS.groundUse = false;
                        prevS.useMultipler = false;
                    }

                }
                

                //�����Đ�
                if (prevS.useMultipler)
                {
                    GManager.instance.PlaySound(container, pos.position,pitch:multipler);
                }
                
                else
                {
                    GManager.instance.PlaySound(container, pos.position);
                }
                //�O�̏���������
                prevS.name = container;
                prevS.isStrong = strong;

                if (!prevS.groundUse && ground != MyCharacter.GroundFeature.Nomal)
                {
                    if (ground == MyCharacter.GroundFeature.Water)
                    {
                        num = strong ? 1 : 0;
                        //     GManager.instance.PlaySound(Watersound[num], pos.position);
                    }
                    else if (ground == MyCharacter.GroundFeature.Grass)
                    {
                        num = strong ? 1 : 0;
                    }
                }

                return prevS;
            }
        }

        /// <summary>
        /// �ڒn���̃G�t�F�N�g�ƃT�E���h��
        /// </summary>
        /// <param name="state"></param>
        /// <param name="isMetal"></param>
        /// <param name="_size"></param>
        /// <param name="ground"></param>
        public void GotGround(Transform pos,SelectState state, bool isMetal, SizeTag _size, MyCharacter.GroundFeature ground)
        {
            
            //��������
            if(state == SelectState.Attack || state == SelectState.Wakeup || _size == SizeTag.big)
            {
                if (isMetal)
                {
                    GManager.instance.PlaySound("NALanding",pos.position);
                }
                //�G�t�F�N�g
                if (ground == MyCharacter.GroundFeature.Nomal)
                {


                }
                //�����ƃG�t�F�N�g�H
                else
                {
                   // GManager.instance.PlaySound("WaterSound", pos.position);
                }
            }
            //�ӂ��̗���
            else
            {

                if (isMetal)
                {
                    GManager.instance.PlaySound("NALanding", pos.position);
                }
                else
                {

                }

                //�G�t�F�N�g
                if (ground == MyCharacter.GroundFeature.Nomal)
                {


                }
                //�����ƃG�t�F�N�g�H
                else
                {

                }

            }

        }



        public void GuardSound(bool isMetal, Equip.GuardType type,in Vector3 pos)
        {
            if (isMetal)
            {
                GManager.instance.PlaySound(metalGuardSound[(int)type], pos);
            }
            else
            {
                GManager.instance.PlaySound(guardSound[(int)type], pos);
            }
        }


        /// <summary>
        /// �X�^�����̋��ʌ���
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="state"></param>
        /// <param name="preStan">�ȑO����X�^�����Ă邩�ǂ���</param>
        public void StanEffect(Transform pos, SelectState state, bool preStan,float sizeMultipler)
        {
            //���߂ẴX�^���Ȃ特���o��
            //�G�t�F�N�g�o�����߃g�����X�t�H�[��
            if (!preStan)
            {

                GManager.instance.PlaySound("stanSound", pos.position);
                /*
                if (sizeMultipler != 1)
                {
                    Transform tran = _generalPool.Spawn(particles["Stan"], pos.position, pos.rotation, pos).transform;
                    Vector3 Scale = tran.localScale;
                    Scale *= sizeMultipler;
                    tran.localScale = Scale;
                }*/
            }

            if (state == SelectState.GBreake)
            {
                GManager.instance.PlaySound("GuardBreake", pos.position);
                //gb�G�t�F�N�g��
                /*
                if (sizeMultipler != 1)
                {
                    Transform tran = _generalPool.Spawn(particles["GuardBreake"], pos.position, pos.rotation, pos).transform;
                    Vector3 Scale = tran.localScale;
                    Scale *= sizeMultipler;
                    tran.localScale = Scale;
                }*/
            }

        }


        /// <summary>
        /// ���̉��ƃG�t�F�N�g
        /// </summary>
        /// <param name="pos"></param>
        public void DeathEffect(in Vector3 pos)
        {
            _generalPool.Spawn(particles["Death"],pos, particles["Death"].transform.rotation);
            
            GManager.instance.PlaySound("FadeSound", pos);
        }




        #endregion

        ///�U���n
        #region

        /// <summary>
        /// ����̉��ƃG�t�F�N�g
        /// ��{�̍U�����[�V�����ɂ����
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="level">����A�^�b�N�^�C�v�H</param>
        /// <param name="element"></param>
        /// <param name="subElement"></param>
        public void GeneralAttack(Transform pos,int level ,int element,int motion, float sizeMultipler)
        {
            ParticleSystem ef = null;

            //�T�u�������Ȃ��Ȃ�
            if (element < 3) 
            {
                //�a��
                if (motion == 0)
                {
                    Debug.Log($"��{level}");
                    //���ʃG�t�F�N�g�͒ǐՂ�����Ĕ��f��

                    GManager.instance.PlaySound(slashSe[level], pos.position);
                    ef = _generalPool.ControlSpawn(slashEf[level], pos.position, pos.rotation,pos);
                }
                //�h��
                else if (motion == 1)
                {
                    GManager.instance.PlaySound(stabSe[level], pos.position);
                    ef = _generalPool.ControlSpawn(stabEf[level], pos.position, pos.rotation, pos);
                }
                //�Ō�
                else if(motion == 3)
                {
                  //  Debug.Log($"��{level}");
                    GManager.instance.PlaySound(strikeSe[level], pos.position);
                    ef = _generalPool.ControlSpawn(strikeEf[level], pos.position, pos.rotation, pos);
                }
            }
            else
            {
                //�a��
                if (motion == 0)
                {
                    //��
                    if (element == 3)
                    {
                        GManager.instance.PlaySound(slashSe[level], pos.position);
                        GManager.instance.PlaySound(holySe[level], pos.position);
                        ef = _generalPool.ControlSpawn(slashHoly[level], pos.position, pos.rotation, pos);
                    }
                    //��
                    else if (element == 4)
                    {
                        GManager.instance.PlaySound(slashSe[level], pos.position);
                        GManager.instance.PlaySound(darkSe[level], pos.position);
                        ef = _generalPool.ControlSpawn(slashDark[level], pos.position, pos.rotation, pos);
                    }
                    //��
                    else if (element == 5)
                    {
                        GManager.instance.PlaySound(slashSe[level], pos.position);
                        GManager.instance.PlaySound(fireSe[level], pos.position);
                        ef = _generalPool.ControlSpawn(slashFire[level], pos.position, pos.rotation, pos);
                    }
                    //��
                    else
                    {
                        GManager.instance.PlaySound(slashSe[level], pos.position);
                        GManager.instance.PlaySound(thunderSe[level], pos.position);
                        ef = _generalPool.ControlSpawn(slashThunder[level], pos.position, pos.rotation, pos);
                    }
                }
                //�h��
                else if (motion == 1)
                {
                    //��
                    if (element == 3)
                    {
                        GManager.instance.PlaySound(stabSe[level], pos.position);
                        GManager.instance.PlaySound(holySe[level], pos.position);
                        ef = _generalPool.ControlSpawn(stabHoly[level], pos.position, pos.rotation, pos);
                    }
                    //��
                    else if (element == 4)
                    {
                        GManager.instance.PlaySound(stabSe[level], pos.position);
                        GManager.instance.PlaySound(darkSe[level], pos.position);
                        ef = _generalPool.ControlSpawn(stabDark[level], pos.position, pos.rotation, pos);
                    }
                    //��
                    else if (element == 5)
                    {
                        GManager.instance.PlaySound(stabSe[level], pos.position);
                        GManager.instance.PlaySound(fireSe[level], pos.position);
                        ef = _generalPool.ControlSpawn(stabFire[level], pos.position, pos.rotation, pos);
                    }
                    //��
                    else
                    {
                        GManager.instance.PlaySound(stabSe[level], pos.position);
                        GManager.instance.PlaySound(thunderSe[level], pos.position);
                        ef = _generalPool.ControlSpawn(stabThunder[level], pos.position, pos.rotation, pos);
                    }
                }
                //�Ō�
                else if(motion == 2)
                {
                    //��
                    if (element == 3)
                    {
                        GManager.instance.PlaySound(strikeSe[level], pos.position);
                        GManager.instance.PlaySound(holySe[level], pos.position);
                        ef = _generalPool.ControlSpawn(strikeHoly[level], pos.position, pos.rotation, pos);
                    }
                    //��
                    else if (element == 4)
                    {
                        GManager.instance.PlaySound(strikeSe[level], pos.position);
                        GManager.instance.PlaySound(darkSe[level], pos.position);
                        ef = _generalPool.ControlSpawn(strikeDark[level], pos.position, pos.rotation, pos);
                    }
                    //��
                    else if (element == 5)
                    {
                        GManager.instance.PlaySound(strikeSe[level], pos.position);
                        GManager.instance.PlaySound(fireSe[level], pos.position);
                        ef = _generalPool.ControlSpawn(strikeFire[level], pos.position, pos.rotation, pos);
                    }
                    //��
                    else
                    {
                        GManager.instance.PlaySound(strikeSe[level], pos.position);
                        GManager.instance.PlaySound(thunderSe[level], pos.position);
                        ef = _generalPool.ControlSpawn(strikeThunder[level], pos.position, pos.rotation, pos);
                    }
                }
            }


            if (ef != null)
            {
                Vector3 ls = ef.transform.localScale;

                //�L�������������Ă�Ȃ甽�΂�
                if (pos.root.localScale.x < 0)
                {
                    ls.x = ls.x * -1;
                }

                //�T�C�Y�ύX
                if (sizeMultipler != 1)
                {
                    ls *= sizeMultipler;
                }
                ef.transform.localScale = ls;
                Debug.Log($"���񂿂񂿂񂤂񂿂񂿂�{ls.x}");
            }
        }


        /// <summary>
        /// ����̉��ƃG�t�F�N�g
        /// ��{�̍U�����[�V�����ɂ����
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="level">����A�^�b�N�^�C�v�H</param>
        /// <param name="element"></param>
        /// <param name="subElement"></param>
        public void GAttackS(Vector3 pos, int level, int element, int motion, float sizeMultipler)
        {


            //�T�u�������Ȃ��Ȃ�
            if (element < 3)
            {
                //�a��
                if (motion == 0)
                {
                    Debug.Log($"��{level}");
                    //���ʃG�t�F�N�g�͒ǐՂ�����Ĕ��f��

                    GManager.instance.PlaySound(slashSe[level], pos);
                    //ef = _generalPool.Spawn(slashEf[level], pos, pos.rotation, pos);
                }
                //�h��
                else if (motion == 1)
                {
                    GManager.instance.PlaySound(stabSe[level], pos);
                    //ef = _generalPool.Spawn(stabEf[level], pos, pos.rotation, pos);
                }
                //�Ō�
                else if (motion == 3)
                {
                    Debug.Log($"��{level}");
                    GManager.instance.PlaySound(strikeSe[level], pos);
                    //ef = _generalPool.Spawn(strikeEf[level], pos, pos.rotation, pos);
                }
            }
            else
            {
                //�a��
                if (motion == 0)
                {
                    //��
                    if (element == 3)
                    {
                        GManager.instance.PlaySound(slashSe[level], pos);
                        GManager.instance.PlaySound(holySe[level], pos);
                        //ef = _generalPool.Spawn(slashHoly[level], pos, pos.rotation, pos);
                    }
                    //��
                    else if (element == 4)
                    {
                        GManager.instance.PlaySound(slashSe[level], pos);
                        GManager.instance.PlaySound(darkSe[level], pos);
                        //ef = _generalPool.Spawn(slashDark[level], pos, pos.rotation, pos);
                    }
                    //��
                    else if (element == 5)
                    {
                        GManager.instance.PlaySound(slashSe[level], pos);
                        GManager.instance.PlaySound(fireSe[level], pos);
                        //ef = _generalPool.Spawn(slashFire[level], pos, pos.rotation, pos);
                    }
                    //��
                    else
                    {
                        GManager.instance.PlaySound(slashSe[level], pos);
                        GManager.instance.PlaySound(thunderSe[level], pos);
                        //ef = _generalPool.Spawn(slashThunder[level], pos, pos.rotation, pos);
                    }
                }
                //�h��
                else if (motion == 1)
                {
                    //��
                    if (element == 3)
                    {
                        GManager.instance.PlaySound(stabSe[level], pos);
                        GManager.instance.PlaySound(holySe[level], pos);
                        //ef = _generalPool.Spawn(stabHoly[level], pos, pos.rotation, pos);
                    }
                    //��
                    else if (element == 4)
                    {
                        GManager.instance.PlaySound(stabSe[level], pos);
                        GManager.instance.PlaySound(darkSe[level], pos);
                        //ef = _generalPool.Spawn(stabDark[level], pos, pos.rotation, pos);
                    }
                    //��
                    else if (element == 5)
                    {
                        GManager.instance.PlaySound(stabSe[level], pos);
                        GManager.instance.PlaySound(fireSe[level], pos);
                        //ef = _generalPool.Spawn(stabFire[level], pos, pos.rotation, pos);
                    }
                    //��
                    else
                    {
                        GManager.instance.PlaySound(stabSe[level], pos);
                        GManager.instance.PlaySound(thunderSe[level], pos);
                        //ef = _generalPool.Spawn(stabThunder[level], pos, pos.rotation, pos);
                    }
                }
                //�Ō�
                else if (motion == 2)
                {
                    //��
                    if (element == 3)
                    {
                        GManager.instance.PlaySound(strikeSe[level], pos);
                        GManager.instance.PlaySound(holySe[level], pos);
                        //ef = _generalPool.Spawn(strikeHoly[level], pos, pos.rotation, pos);
                    }
                    //��
                    else if (element == 4)
                    {
                        GManager.instance.PlaySound(strikeSe[level], pos);
                        GManager.instance.PlaySound(darkSe[level], pos);
                        //ef = _generalPool.Spawn(strikeDark[level], pos, pos.rotation, pos);
                    }
                    //��
                    else if (element == 5)
                    {
                        GManager.instance.PlaySound(strikeSe[level], pos);
                        GManager.instance.PlaySound(fireSe[level], pos);
                        //ef = _generalPool.Spawn(strikeFire[level], pos, pos.rotation, pos);
                    }
                    //��
                    else
                    {
                        GManager.instance.PlaySound(strikeSe[level], pos);
                        GManager.instance.PlaySound(thunderSe[level], pos);
                        //ef = _generalPool.Spawn(strikeThunder[level], pos, pos.rotation, pos);
                    }
                }
            }

        }




        /// <summary>
        /// �U�����̃p���B�s�G�t�F�N�g�Ȃǂ��o��
        /// </summary>
        /// <param name="type"></param>
        /// <param name="pos"></param>
        public void AdditionalEffect(int type,Transform pos,float sizeMultipler)
        {
            if (type == 2)
            {
                //�p���B�s�̉��ƃG�t�F�N�g
                GManager.instance.PlaySound("DisenableParry", pos.position);

                if (sizeMultipler != 1)
                {
                    Transform tran = _generalPool.ControlSpawn(particles["DisParry"], pos.position, pos.rotation, pos).transform;
                    Vector3 Scale = tran.localScale;
                    Scale *= sizeMultipler;
                    tran.localScale = Scale;
                }
                else
                {
                    _generalPool.ControlSpawn(particles["DisParry"], pos.position, pos.rotation, pos);
                }
            }

        }


        #endregion


        //���@�n
        #region


        /// <summary>
        /// ���ƃG�t�F�N�g�o����I�I
        /// </summary>
        /// <param name="magicLevel"></param>
        /// <param name="element"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
         public Transform CastEfCall(int magicLevel, AtEffectCon.Element element,Transform pos)
        {

            //����T�|�[�g�ƃf�o�t�A�񕜂��K�v����Ȃ��H
            if ((int)element < 4)
            {
                if (element == AtEffectCon.Element.�a������)
                {
                    GManager.instance.PlaySound(slashCast[magicLevel], pos.position);
                     return _generalPool.ControlSpawn(slashCEffect[magicLevel], pos.position, pos.rotation,pos).transform;
                }
                else if (element == AtEffectCon.Element.�h�ˑ���)
                {
                    GManager.instance.PlaySound(stabCast[magicLevel], pos.position);
                    return _generalPool.ControlSpawn(stabCEffect[magicLevel], pos.position, pos.rotation,pos).transform;
                }
                else if (element == AtEffectCon.Element.�Ō�����)
                {
                    GManager.instance.PlaySound(strikeCast[magicLevel], pos.position);
                    return _generalPool.ControlSpawn(strikeCEffect[magicLevel], pos.position, pos.rotation,pos).transform;
                }
                else if (element == AtEffectCon.Element.������)
                {
                    GManager.instance.PlaySound(holyCast[magicLevel], pos.position);
                    return _generalPool.ControlSpawn(holyCEffect[magicLevel], pos.position, pos.rotation,pos).transform;
                }
            }
            else
            {
                if (element == AtEffectCon.Element.�ő���)
                {
                    GManager.instance.PlaySound(darkCast[magicLevel], pos.position);
                    return _generalPool.ControlSpawn(darkCEffect[magicLevel], pos.position, pos.rotation,pos).transform;
                }
                else if (element == AtEffectCon.Element.������)
                {
                    
                    GManager.instance.PlaySound(fireCast[magicLevel], pos.position);
                    return _generalPool.ControlSpawn(fireCEffect[magicLevel], pos.position, pos.rotation,pos).transform;
                }
                else if (element == AtEffectCon.Element.������)
                {
                    GManager.instance.PlaySound(thunderCast[magicLevel], pos.position);
                    return _generalPool.ControlSpawn(thunderCEffect[magicLevel], pos.position, pos.rotation,pos).transform;
                }
               
            }
           return null;
        }


        /// <summary>
        /// �r���G�t�F�N�g�����ĉ����������㔭���G�t�F�N�g
        /// ����
        /// </summary>
        /// <param name="inst"></param>
        public void CastEfClear(Transform inst, int magicLevel, AtEffectCon.Element element)
        {

            _generalPool.Despawn(inst);



            if ((int)element < 4)
            {
                if (element == AtEffectCon.Element.�a������)
                {
                    _generalPool.Spawn(slashActiEffect[magicLevel], inst.position, inst.rotation);
                    GManager.instance.StopSound(slashCast[magicLevel], isStop: true);
                    GManager.instance.PlaySound(slashActivate[magicLevel], inst.position);
                }
                else if (element == AtEffectCon.Element.�h�ˑ���)
                {
                    _generalPool.Spawn(stabActiEffect[magicLevel], inst.position, inst.rotation);
                    GManager.instance.StopSound(stabCast[magicLevel], isStop: true);
                    GManager.instance.PlaySound(stabActivate[magicLevel], inst.position);
                }
                else if (element == AtEffectCon.Element.�Ō�����)
                {
                    _generalPool.Spawn(strikeActiEffect[magicLevel], inst.position, inst.rotation);
                    GManager.instance.StopSound(strikeCast[magicLevel], isStop: true);
                    GManager.instance.PlaySound(strikeActivate[magicLevel], inst.position);
                }
                else if (element == AtEffectCon.Element.������)
                {
                    _generalPool.Spawn(holyActiEffect[magicLevel], inst.position, inst.rotation);
                    GManager.instance.StopSound(holyCast[magicLevel],isStop: true);
                    GManager.instance.PlaySound(holyActivate[magicLevel], inst.position);
                }
            }
            else
            {
                if (element == AtEffectCon.Element.�ő���)
                {
                    _generalPool.Spawn(darkActiEffect[magicLevel], inst.position, inst.rotation);
                    GManager.instance.StopSound(darkCast[magicLevel], isStop: true);
                    GManager.instance.PlaySound(darkActivate[magicLevel], inst.position);
                }
                else if (element == AtEffectCon.Element.������)
                {
                    _generalPool.Spawn(fireActiEffect[magicLevel], inst.position, inst.rotation);
                    GManager.instance.StopSound(fireCast[magicLevel], isStop: true);
                    GManager.instance.PlaySound(fireActivate[magicLevel], inst.position);
                }
                else if (element == AtEffectCon.Element.������)
                {
                    _generalPool.Spawn(thunderActiEffect[magicLevel], inst.position, inst.rotation);
                    GManager.instance.StopSound(thunderCast[magicLevel], isStop: true);
                    GManager.instance.PlaySound(thunderActivate[magicLevel], inst.position);
                }
                else if (element == AtEffectCon.Element.�w��Ȃ�)
                {

                }
            }

        }

        /// <summary>
        /// �r�����f
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="magicLevel"></param>
        /// <param name="element"></param>
        public void CastStop(Transform inst, int magicLevel, AtEffectCon.Element element)
        {
            _generalPool.Despawn(inst);
            if ((int)element < 4)
            {
                if (element == AtEffectCon.Element.�a������)
                {
 
                    GManager.instance.StopSound(slashCast[magicLevel], isStop: true);

                }
                else if (element == AtEffectCon.Element.�h�ˑ���)
                {
                    GManager.instance.StopSound(stabCast[magicLevel], isStop: true);
                }
                else if (element == AtEffectCon.Element.�Ō�����)
                {
                    GManager.instance.StopSound(strikeCast[magicLevel], isStop: true);
                }
                else if (element == AtEffectCon.Element.������)
                {
                    GManager.instance.StopSound(holyCast[magicLevel], isStop: true);
                }
            }
            else
            {
                if (element == AtEffectCon.Element.�ő���)
                {
                    GManager.instance.StopSound(darkCast[magicLevel], isStop: true);
                }
                else if (element == AtEffectCon.Element.������)
                {
                    GManager.instance.StopSound(fireCast[magicLevel], isStop: true);
                }
                else if (element == AtEffectCon.Element.������)
                {
                    GManager.instance.StopSound(thunderCast[magicLevel], isStop: true);
                }
                else if (element == AtEffectCon.Element.none)
                {

                }
            }
        }






        #endregion


        #region ���ʃT�E���h

        /// <summary>
        /// ��ԕω��̉���炷
        /// 
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="pos"></param>
        public void ConditionEffectSound(ConditionAndEffectControllAbility.UniqueEffect effect,in Vector3 pos)
        {

            if (conditionSound.ContainsKey(effect))
            {
                GManager.instance.PlaySound(conditionSound[effect], pos);
            }

        }


        #endregion
    }
}