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



        [Header("�a���ƑŌ��̉�")]
        [SoundGroup]
        public List<String> swingSound;

        [Header("�h�˂̉�")]
        [SoundGroup]
        public List<String> stabSound;

        [Header("�i���̉�")]
        [SoundGroup]
        public List<String> fistSound;

        [Header("�f���̉�")]
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

        [Header("�_�E���̉�")]
        [SoundGroup]
        public List<String> downSound;

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

    }
}