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
            huge = 3//特大
        }

        public enum ElementalSoundTag
        {
            normal = 0,
            fire = 1,
            thunder = 2,
            dark = 3,
            holy = 4

        }

        [SerializeField,Header("攻撃の音")]

        [Header("斬撃と打撃の音")]
        [SoundGroup]
        public List<String> swingSound;

        [Header("刺突の音")]
        [SoundGroup]
        public List<String> stabSound;

        [Header("格闘の音")]
        [SoundGroup]
        public List<String> fistSound;

        [SerializeField, Header("足音")]

        [Header("素足の音")]//smallはひたひたって感じにする？
        [SoundGroup]
        public List<String> bareFootSound;


        [Header("素足の歩行")]
        [SoundGroup]
        public List<String> bareWalkSound;

        [Header("鎧足の音")]
        [SoundGroup]
        public List<String> armorFootSound;

        [Header("鎧足の歩く音")]
        [SoundGroup]
        public List<String> armorWalkSound;

        [SerializeField, Header("ダウンの音")]

        [Header("ダウンの音")]
        [SoundGroup]
        public List<String> downSound;

        [Header("金属のダウンの音")]
        [SoundGroup]
        public List<String> armorDownSound;

        [SerializeField, Header("ローリングの音")]

        [Header("ローリングの音")]
        [SoundGroup]
        public List<String> rollSound;

        [Header("金属ローリングの音")]
        [SoundGroup]
        public List<String> armorRollSound;

        [SerializeField, Header("身じろぎの音")]

        [Header("身じろぎ音（ザっ…て感じ）")]
        [SoundGroup]
        public List<String> shakeSound;

        [Header("金属身じろぎの音（がしゃっ）")]
        [SoundGroup]
        public List<String> armorShakeSound;

        [SerializeField, Header("ジャンプの音")]

        [Header("ジャンプの音")]
        [SoundGroup]
        public List<String> jumpSound;

        [Header("金属ジャンプ")]
        [SoundGroup]
        public List<String> armorJumpSound;

        [SerializeField, Header("ブロッキング、パリィの音")]

        [Header("ブロッキングの音")]
        [SoundGroup]
        public string blockingSound;

        [Header("パリィの音")]
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