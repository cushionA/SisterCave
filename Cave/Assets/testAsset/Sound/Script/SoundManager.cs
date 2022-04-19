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



        [Header("aŒ‚‚Æ‘ÅŒ‚‚Ì‰¹")]
        [SoundGroup]
        public List<String> swingSound;

        [Header("h“Ë‚Ì‰¹")]
        [SoundGroup]
        public List<String> stabSound;

        [Header("Ši“¬‚Ì‰¹")]
        [SoundGroup]
        public List<String> fistSound;

        [Header("‘f‘«‚Ì‰¹")]
        [SoundGroup]
        public List<String> bareFootSound;


        [Header("‘f‘«‚Ì•às")]
        [SoundGroup]
        public List<String> bareWalkSound;

        [Header("ŠZ‘«‚Ì‰¹")]
        [SoundGroup]
        public List<String> armorFootSound;

        [Header("ŠZ‘«‚Ì•à‚­‰¹")]
        [SoundGroup]
        public List<String> armorWalkSound;

        [Header("ƒ_ƒEƒ“‚Ì‰¹")]
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