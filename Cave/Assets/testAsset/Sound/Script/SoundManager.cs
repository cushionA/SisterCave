using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkTonic.MasterAudio;
using System;

namespace MyCode
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager instance = null;



        public enum SizeTag
        {
            small = 0,
            middle = 1,
            big = 2,
            huge = 3//����
        }

        public enum ElementalSoundTag
        {
            normal = 0,
            fire = 1,
            thunder = 2,
            dark = 3,
            holy = 4

        }

        [SerializeField,Header("�U���̉�")]

        [Header("�a���ƑŌ��̉�")]
        [SoundGroup]
        public List<String> swingSound;

        [Header("�h�˂̉�")]
        [SoundGroup]
        public List<String> stabSound;

        [Header("�i���̉�")]
        [SoundGroup]
        public List<String> fistSound;

        [SerializeField, Header("����")]

        [Header("�f���̉�")]//small�͂Ђ��Ђ����Ċ����ɂ���H
        [SoundGroup]
        public List<String> bareFootSound;


        [Header("�f���̕��s")]
        [SoundGroup]
        public List<String> bareWalkSound;

        [Header("�Z���̉�")]
        [SoundGroup]
        public List<String> armorFootSound;

        [Header("�Z���̕�����")]
        [SoundGroup]
        public List<String> armorWalkSound;

        [SerializeField, Header("�_�E���̉�")]

        [Header("�_�E���̉�")]
        [SoundGroup]
        public List<String> downSound;

        [Header("�����̃_�E���̉�")]
        [SoundGroup]
        public List<String> armorDownSound;

        [SerializeField, Header("���[�����O�̉�")]

        [Header("���[�����O�̉�")]
        [SoundGroup]
        public List<String> rollSound;

        [Header("�������[�����O�̉�")]
        [SoundGroup]
        public List<String> armorRollSound;

        [SerializeField, Header("�g���났�̉�")]

        [Header("�g���났���i�U���c�Ċ����j")]
        [SoundGroup]
        public List<String> shakeSound;

        [Header("�����g���났�̉��i��������j")]
        [SoundGroup]
        public List<String> armorShakeSound;

        [SerializeField, Header("�W�����v�̉�")]

        [Header("�W�����v�̉�")]
        [SoundGroup]
        public List<String> jumpSound;

        [Header("�����W�����v")]
        [SoundGroup]
        public List<String> armorJumpSound;

        [SerializeField, Header("�u���b�L���O�A�p���B�̉�")]

        [Header("�u���b�L���O�̉�")]
        [SoundGroup]
        public string blockingSound;

        [Header("�p���B�̉�")]
        [SoundGroup]
        public string parrySound;


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

    }
}