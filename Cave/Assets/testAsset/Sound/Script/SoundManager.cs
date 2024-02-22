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
/// エフェクトのサイズ倍率で体格表現可能
/// </summary>
namespace MyCode
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager instance = null;

        /// <summary>
        /// 定義
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
            big = 2//特大
        }

        #endregion

        #region フィールド


        /// <summary>
        /// エフェクトを生成するクラス
        /// オブジェクトプール機能
        /// Spawnで生成。エフェクトの再生が終了すると自動で消滅
        /// </summary>
        [SerializeField]
        SpawnPool _generalPool;



        /// <summary>
        /// 文字列で指定するやつ
        /// 汎用
        /// </summary>
        public MyParticles particles = new MyParticles();


        /// <summary>
        /// 蓄積の減少速度を記録したディクショナリ
        /// </summary>
        public Dictionary<ConditionAndEffectControllAbility.RestoreEffect, float> restoreDicreaceDict;




        /// 数字で管理するやつ
        /// レベルとかの数字を利用するやつ
        /// 
        #region




        // 攻撃エフェクト
        #region

        [Foldout("攻撃のエフェクト")]
        [Header("斬撃のエフェクト")]
        public ParticleSystem[] slashEf;

        [Foldout("攻撃のエフェクト")]
        [Header("打撃のエフェクト")]
        public ParticleSystem[] strikeEf;

        [Foldout("攻撃のエフェクト")]
        [Header("刺突のエフェクト")]
        public ParticleSystem[] stabEf;

        [Foldout("攻撃のエフェクト")]
        [Header("炎斬撃のエフェクト")]
        public ParticleSystem[] slashFire;

        [Foldout("攻撃のエフェクト")]
        [Header("炎打撃のエフェクト")]
        public ParticleSystem[] strikeFire;

        [Foldout("攻撃のエフェクト")]
        [Header("炎刺突のエフェクト")]
        public ParticleSystem[] stabFire;

        [Foldout("攻撃のエフェクト")]
        [Header("雷斬撃のエフェクト")]
        public ParticleSystem[] slashThunder;

        [Foldout("攻撃のエフェクト")]
        [Header("雷打撃のエフェクト")]
        public ParticleSystem[] strikeThunder;

        [Foldout("攻撃のエフェクト")]
        [Header("雷刺突のエフェクト")]
        public ParticleSystem[] stabThunder;

        [Foldout("攻撃のエフェクト")]
        [Header("光斬撃のエフェクト")]
        public ParticleSystem[] slashHoly;

        [Foldout("攻撃のエフェクト")]
        [Header("光打撃のエフェクト")]
        public ParticleSystem[] strikeHoly;

        [Foldout("攻撃のエフェクト")]
        [Header("光刺突のエフェクト")]
        public ParticleSystem[] stabHoly;

        [Foldout("攻撃のエフェクト")]
        [Header("闇斬撃のエフェクト")]
        public ParticleSystem[] slashDark;

        [Foldout("攻撃のエフェクト")]
        [Header("闇打撃のエフェクト")]
        public ParticleSystem[] strikeDark;

        [Foldout("攻撃のエフェクト")]
        [Header("闇刺突のエフェクト")]
        public ParticleSystem[] stabDark;

        #endregion

        //魔法エフェクト
        #region

        [Foldout("詠唱のエフェクト")]
        [Header("斬撃（風）の詠唱エフェクト")]
        public ParticleSystem[] slashCEffect;

        [Foldout("詠唱のエフェクト")]
        [Header("打撃（岩）の詠唱エフェクト")]
        public ParticleSystem[] strikeCEffect;

        [Foldout("詠唱のエフェクト")]
        [Header("刺突（氷）の詠唱エフェクト")]
        public ParticleSystem[] stabCEffect;

        [Foldout("詠唱のエフェクト")]
        [Header("炎の詠唱エフェクト")]
        public ParticleSystem[] fireCEffect;

        [Foldout("詠唱のエフェクト")]
        [Header("雷の詠唱エフェクト")]
        public ParticleSystem[] thunderCEffect;

        [Foldout("詠唱のエフェクト")]
        [Header("光の詠唱エフェクト")]
        public ParticleSystem[] holyCEffect;

        [Foldout("詠唱のエフェクト")]
        [Header("闇の詠唱エフェクト")]
        public ParticleSystem[] darkCEffect;


        [Foldout("魔法発動のエフェクト")]
        [Header("斬撃（風）の魔法発動エフェクト")]
        public ParticleSystem[] slashActiEffect;

        [Foldout("魔法発動のエフェクト")]
        [Header("打撃（岩）の魔法発動エフェクト")]
        public ParticleSystem[] strikeActiEffect;

        [Foldout("魔法発動のエフェクト")]
        [Header("刺突（氷）の魔法発動エフェクト")]
        public ParticleSystem[] stabActiEffect;

        [Foldout("魔法発動のエフェクト")]
        [Header("炎の魔法発動エフェクト")]
        public ParticleSystem[] fireActiEffect;

        [Foldout("魔法発動のエフェクト")]
        [Header("雷の魔法発動エフェクト")]
        public ParticleSystem[] thunderActiEffect;

        [Foldout("魔法発動のエフェクト")]
        [Header("光の魔法発動エフェクト")]
        public ParticleSystem[] holyActiEffect;

        [Foldout("魔法発動のエフェクト")]
        [Header("闇の魔法発動エフェクト")]
        public ParticleSystem[] darkActiEffect;

        #endregion

        #endregion



        #region


        #endregion

        /// <summary>
        /// サウンド
        /// </summary>
        #region

        //動きのサウンド
        #region

        [SerializeField, Header("足音")]
        [Foldout("動きのサウンド")]
        [Header("素足の音")]//smallはひたひたって感じにする？
        [SoundGroup]
        public String[] bareFootSound;

        [Foldout("動きのサウンド")]
        [Header("素足の歩行")]
        [SoundGroup]
        public String[] bareWalkSound;

        [Foldout("動きのサウンド")]
        [Header("鎧足の音")]
        [SoundGroup]
        public String[] armorFootSound;

        [Foldout("動きのサウンド")]
        [Header("鎧足の歩く音")]
        [SoundGroup]
        public String[] armorWalkSound;

        [SerializeField, Header("ダウンの音")]
        [Foldout("動きのサウンド")]
        [Header("ダウンの音")]
        [SoundGroup]
        public String[] downSound;

        [Foldout("動きのサウンド")]
        [Header("金属のダウンの音")]
        [SoundGroup]
        public String[] armorDownSound;


        [SerializeField, Header("ローリングの音")]
        [Foldout("動きのサウンド")]
        [Header("ローリングの音")]
        [SoundGroup]
        public String[] rollSound;

        [Foldout("動きのサウンド")]
        [Header("金属ローリングの音")]
        [SoundGroup]
        public String[] armorRollSound;


        [SerializeField, Header("身じろぎの音")]
        [Foldout("動きのサウンド")]
        [Header("身じろぎ音（ザっ…て感じ）")]
        [SoundGroup]
        public String[] shakeSound;

        [Foldout("動きのサウンド")]
        [Header("金属身じろぎの音（がしゃっ）")]
        [SoundGroup]
        public String[] armorShakeSound;

        [SerializeField, Header("ジャンプの音")]
        [Foldout("動きのサウンド")]
        [Header("ジャンプの音")]
        [SoundGroup]
        public String[] jumpSound;

        [Foldout("動きのサウンド")]
        [Header("金属ジャンプ")]
        [SoundGroup]
        public String[] armorJumpSound;


        [Foldout("動きのサウンド")]
        [Header("ふつうのガード")]
        [SoundGroup]
        public String[] guardSound;

        [Foldout("動きのサウンド")]
        [Header("金属ガード")]
        [SoundGroup]
        public String[] metalGuardSound;

        //リストじゃない奴はハードコーディングで書いてもらお





        #endregion

        //攻撃のサウンド
        #region

        [Foldout("攻撃のサウンド")]
        [Header("斬撃の音")]
        [SoundGroup]
        public String[] slashSe;

        [Foldout("攻撃のサウンド")]
        [Header("打撃の音")]
        [SoundGroup]
        public String[] strikeSe;

        [Foldout("攻撃のサウンド")]
        [Header("刺突の音")]
        [SoundGroup]
        public String[] stabSe;

        [Foldout("攻撃のサウンド")]
        [Header("炎の音")]
        [SoundGroup]
        public String[] fireSe;

        [Foldout("攻撃のサウンド")]
        [Header("雷の音")]
        [SoundGroup]
        public String[] thunderSe;

        [Foldout("攻撃のサウンド")]
        [Header("光の音")]
        [SoundGroup]
        public String[] holySe;

        [Foldout("攻撃のサウンド")]
        [Header("闇の音")]
        [SoundGroup]
        public String[] darkSe;
        #endregion

        //魔法詠唱の音
        #region

        [Foldout("詠唱のサウンド")]
        [Header("斬撃（風）の音")]
        [SoundGroup]
        public String[] slashCast;

        [Foldout("詠唱のサウンド")]
        [Header("打撃（岩）の音")]
        [SoundGroup]
        public String[] strikeCast;

        [Foldout("詠唱のサウンド")]
        [Header("刺突（氷）の音")]
        [SoundGroup]
        public String[] stabCast;

        [Foldout("詠唱のサウンド")]
        [Header("炎の音")]
        [SoundGroup]
        public String[] fireCast;

        [Foldout("詠唱のサウンド")]
        [Header("雷の音")]
        [SoundGroup]
        public String[] thunderCast;

        [Foldout("詠唱のサウンド")]
        [Header("光の音")]
        [SoundGroup]
        public String[] holyCast;

        [Foldout("詠唱のサウンド")]
        [Header("闇の音")]
        [SoundGroup]
        public String[] darkCast;


        [Foldout("魔法発動のサウンド")]
        [Header("斬撃（風）の音")]
        [SoundGroup]
        public String[] slashActivate;

        [Foldout("魔法発動のサウンド")]
        [Header("打撃（岩）の音")]
        [SoundGroup]
        public String[] strikeActivate;

        [Foldout("魔法発動のサウンド")]
        [Header("刺突（氷）の音")]
        [SoundGroup]
        public String[] stabActivate;

        [Foldout("魔法発動のサウンド")]
        [Header("炎の音")]
        [SoundGroup]
        public String[] fireActivate;

        [Foldout("魔法発動のサウンド")]
        [Header("雷の音")]
        [SoundGroup]
        public String[] thunderActivate;

        [Foldout("魔法発動のサウンド")]
        [Header("光の音")]
        [SoundGroup]
        public String[] holyActivate;

        [Foldout("魔法発動のサウンド")]
        [Header("闇の音")]
        [SoundGroup]
        public String[] darkActivate;


        #endregion



        #region 効果サウンド

        [SerializeField, Header("特殊効果の音")]
        [Foldout("効果サウンド")]
        [Header("特殊効果の音")]//smallはひたひたって感じにする？
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




        //いらないやつ
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


        //共通系
        #region
        /// <summary>
        /// 共通エフェクト再生
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
            //前の条件と一致するなら判断を行わない
            if (prevE.state == (int)state)
            {

                ParticleSystem ef = null;

                //地面使うステートではisStrongか否かでエフェクト生成の代わりに水しぶきとか上げていく
                //地面のエフェクト

                //地面使用する、かつ通常の地面じゃない
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

                //それ以外
                else if(prevE.prefab.Length > 0)
                {
                    ef = _generalPool.Spawn(particles[prevE.prefab], pos.position, pos.rotation);
                }


                //サイズ変更
                Vector3 Scale = ef.transform.localScale;

                if (sizeMultipler != 1)
                {

                    Scale *= sizeMultipler;
                }
                //キャラが左向いてるなら反対に
                if (!isRight)
                {
                    Scale.x = Scale.x * -1;
                }
                ef.transform.localScale = Scale;


                return prevE;
            }
            //しないなら再判断
            else
            {

                //水音などが強い勢いの音を立てているかどうか
                bool strong = false;

                int num = (int)state;
                prevE.groundUse = true;

                prevE.state = (int)state;


               string container = null;
                if (num <= 7)
                {
 　　　　　　　　　　if (state == SelectState.Running)//地形アリ、変わる
                    {
                        container = "Runnning";
                    }
                    else if (state == SelectState.Jumping)//地形アリ、変わる
                    {
                        container = "Jump";
                        strong = true;
                    }
                    else if (state == SelectState.DoubleJumping)//地形無し、変わる
                    {
                        container = "DoubleJumping";
                        ground = MyCharacter.GroundFeature.Nomal;
                        prevE.groundUse = false;
                    }
                }
                else if (num <= 13)
                {

                    if (state == SelectState.FastFlying)//地形無し、変わる
                    {
                        container = "FastFlying";
                        ground = MyCharacter.GroundFeature.Nomal;
                        prevE.groundUse = false;
                    }
                    else if (state == SelectState.Rolling)//地形アリ、変わる
                    {
                        container = "Rolling";
                        strong = true;
                    }

                }
                else
                {

                    if (state == SelectState.Parry)//地形無し、変わらない
                    {
                        container = "Parry";
                        ground = MyCharacter.GroundFeature.Nomal;
                        prevE.groundUse = false;
                    }
                    else if (state == SelectState.justGuard)//地形無し、変わらない
                    {
                        container = "justGuard";
                        ground = MyCharacter.GroundFeature.Nomal;
                        prevE.groundUse = false;
                    }

                }



                //エフェクトを再生

                ParticleSystem ef = _generalPool.Spawn(particles[container], pos.position, pos.rotation);
                Vector3 Scale = ef.transform.localScale;


                if (sizeMultipler != 1)
                {
                    
                    
                    Scale *= sizeMultipler;
                  //  tran.localScale = Scale;
                }
                //キャラが左向いてるなら反対に
                if (!isRight)
                {
                    Scale.x = Scale.x * -1;
                }
                ef.transform.localScale = Scale;

                //前の情報を初期化
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
            //前の条件と一致するなら判断を行わない
            if (prevS.state == (int)state)
            {
                //音を再生
                if (prevS.useMultipler)
                {
                    GManager.instance.PlaySound(prevS.name, pos.position, pitch: multipler);
                }

                else
                {

                    GManager.instance.PlaySound(prevS.name, pos.position);
                }

                //地面の音

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
            //しないなら再判断
            else
            {

                //水音などが強い勢いの音を立てているかどうか
                bool strong = false;

                int num = (int)state;
                prevS.groundUse = true;
                prevS.useMultipler = true;
                prevS.state = num;

                string container = "";
                if (num <= 5)
                {

                    if (state == SelectState.moving)//地形アリ、変わる
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
                    else if (state == SelectState.Running)//地形アリ、変わる
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
                    else if (state == SelectState.Crawling)//地形アリ、変わる
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
                    else if (state == SelectState.Crouching)//地形アリ、変わらない
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
                    if (state == SelectState.Jumping)//地形アリ、変わる
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
                    else if (state == SelectState.DoubleJumping)//地形無し、変わる
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
                    else if (state == SelectState.Flying)//地形無し、変わる
                    {

                        ground = MyCharacter.GroundFeature.Nomal;
                        prevS.groundUse = false;
                    }
                    else if (state == SelectState.FastFlying)//地形無し、変わる
                    {
                        ground = MyCharacter.GroundFeature.Nomal;
                        prevS.groundUse = false;
                    }
                }
                else if (num <= 13)
                {
                    if (state == SelectState.Rolling)//地形アリ、変わる
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
                    else if (state == SelectState.Guard)//地形無し、変わらない
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
                    else if (state == SelectState.GuardMove)//地形無し、変わらない
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

                    if (state == SelectState.Parry)//地形無し、変わらない
                    {
                        container = "ParrySuccess";
                        ground = MyCharacter.GroundFeature.Nomal;
                        prevS.groundUse = false;
                        prevS.useMultipler = false;
                    }
                    else if (state == SelectState.justGuard)//地形無し、変わらない
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
                

                //音を再生
                if (prevS.useMultipler)
                {
                    GManager.instance.PlaySound(container, pos.position,pitch:multipler);
                }
                
                else
                {
                    GManager.instance.PlaySound(container, pos.position);
                }
                //前の情報を初期化
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
        /// 接地時のエフェクトとサウンドを
        /// </summary>
        /// <param name="state"></param>
        /// <param name="isMetal"></param>
        /// <param name="_size"></param>
        /// <param name="ground"></param>
        public void GotGround(Transform pos,SelectState state, bool isMetal, SizeTag _size, MyCharacter.GroundFeature ground)
        {
            
            //強い落下
            if(state == SelectState.Attack || state == SelectState.Wakeup || _size == SizeTag.big)
            {
                if (isMetal)
                {
                    GManager.instance.PlaySound("NALanding",pos.position);
                }
                //エフェクト
                if (ground == MyCharacter.GroundFeature.Nomal)
                {


                }
                //水音とエフェクト？
                else
                {
                   // GManager.instance.PlaySound("WaterSound", pos.position);
                }
            }
            //ふつうの落下
            else
            {

                if (isMetal)
                {
                    GManager.instance.PlaySound("NALanding", pos.position);
                }
                else
                {

                }

                //エフェクト
                if (ground == MyCharacter.GroundFeature.Nomal)
                {


                }
                //水音とエフェクト？
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
        /// スタン時の共通効果
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="state"></param>
        /// <param name="preStan">以前からスタンしてるかどうか</param>
        public void StanEffect(Transform pos, SelectState state, bool preStan,float sizeMultipler)
        {
            //初めてのスタンなら音を出す
            //エフェクト出すためトランスフォーム
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
                //gbエフェクトも
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
        /// 死の音とエフェクト
        /// </summary>
        /// <param name="pos"></param>
        public void DeathEffect(in Vector3 pos)
        {
            _generalPool.Spawn(particles["Death"],pos, particles["Death"].transform.rotation);
            
            GManager.instance.PlaySound("FadeSound", pos);
        }




        #endregion

        ///攻撃系
        #region

        /// <summary>
        /// 武器の音とエフェクト
        /// 基本の攻撃モーションにつくやつ
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="level">これアタックタイプ？</param>
        /// <param name="element"></param>
        /// <param name="subElement"></param>
        public void GeneralAttack(Transform pos,int level ,int element,int motion, float sizeMultipler)
        {
            ParticleSystem ef = null;

            //サブ属性がないなら
            if (element < 3) 
            {
                //斬撃
                if (motion == 0)
                {
                    Debug.Log($"ｓ{level}");
                    //共通エフェクトは追跡するって判断で

                    GManager.instance.PlaySound(slashSe[level], pos.position);
                    ef = _generalPool.ControlSpawn(slashEf[level], pos.position, pos.rotation,pos);
                }
                //刺突
                else if (motion == 1)
                {
                    GManager.instance.PlaySound(stabSe[level], pos.position);
                    ef = _generalPool.ControlSpawn(stabEf[level], pos.position, pos.rotation, pos);
                }
                //打撃
                else if(motion == 3)
                {
                  //  Debug.Log($"あ{level}");
                    GManager.instance.PlaySound(strikeSe[level], pos.position);
                    ef = _generalPool.ControlSpawn(strikeEf[level], pos.position, pos.rotation, pos);
                }
            }
            else
            {
                //斬撃
                if (motion == 0)
                {
                    //聖
                    if (element == 3)
                    {
                        GManager.instance.PlaySound(slashSe[level], pos.position);
                        GManager.instance.PlaySound(holySe[level], pos.position);
                        ef = _generalPool.ControlSpawn(slashHoly[level], pos.position, pos.rotation, pos);
                    }
                    //闇
                    else if (element == 4)
                    {
                        GManager.instance.PlaySound(slashSe[level], pos.position);
                        GManager.instance.PlaySound(darkSe[level], pos.position);
                        ef = _generalPool.ControlSpawn(slashDark[level], pos.position, pos.rotation, pos);
                    }
                    //炎
                    else if (element == 5)
                    {
                        GManager.instance.PlaySound(slashSe[level], pos.position);
                        GManager.instance.PlaySound(fireSe[level], pos.position);
                        ef = _generalPool.ControlSpawn(slashFire[level], pos.position, pos.rotation, pos);
                    }
                    //雷
                    else
                    {
                        GManager.instance.PlaySound(slashSe[level], pos.position);
                        GManager.instance.PlaySound(thunderSe[level], pos.position);
                        ef = _generalPool.ControlSpawn(slashThunder[level], pos.position, pos.rotation, pos);
                    }
                }
                //刺突
                else if (motion == 1)
                {
                    //聖
                    if (element == 3)
                    {
                        GManager.instance.PlaySound(stabSe[level], pos.position);
                        GManager.instance.PlaySound(holySe[level], pos.position);
                        ef = _generalPool.ControlSpawn(stabHoly[level], pos.position, pos.rotation, pos);
                    }
                    //闇
                    else if (element == 4)
                    {
                        GManager.instance.PlaySound(stabSe[level], pos.position);
                        GManager.instance.PlaySound(darkSe[level], pos.position);
                        ef = _generalPool.ControlSpawn(stabDark[level], pos.position, pos.rotation, pos);
                    }
                    //炎
                    else if (element == 5)
                    {
                        GManager.instance.PlaySound(stabSe[level], pos.position);
                        GManager.instance.PlaySound(fireSe[level], pos.position);
                        ef = _generalPool.ControlSpawn(stabFire[level], pos.position, pos.rotation, pos);
                    }
                    //雷
                    else
                    {
                        GManager.instance.PlaySound(stabSe[level], pos.position);
                        GManager.instance.PlaySound(thunderSe[level], pos.position);
                        ef = _generalPool.ControlSpawn(stabThunder[level], pos.position, pos.rotation, pos);
                    }
                }
                //打撃
                else if(motion == 2)
                {
                    //聖
                    if (element == 3)
                    {
                        GManager.instance.PlaySound(strikeSe[level], pos.position);
                        GManager.instance.PlaySound(holySe[level], pos.position);
                        ef = _generalPool.ControlSpawn(strikeHoly[level], pos.position, pos.rotation, pos);
                    }
                    //闇
                    else if (element == 4)
                    {
                        GManager.instance.PlaySound(strikeSe[level], pos.position);
                        GManager.instance.PlaySound(darkSe[level], pos.position);
                        ef = _generalPool.ControlSpawn(strikeDark[level], pos.position, pos.rotation, pos);
                    }
                    //炎
                    else if (element == 5)
                    {
                        GManager.instance.PlaySound(strikeSe[level], pos.position);
                        GManager.instance.PlaySound(fireSe[level], pos.position);
                        ef = _generalPool.ControlSpawn(strikeFire[level], pos.position, pos.rotation, pos);
                    }
                    //雷
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

                //キャラが左向いてるなら反対に
                if (pos.root.localScale.x < 0)
                {
                    ls.x = ls.x * -1;
                }

                //サイズ変更
                if (sizeMultipler != 1)
                {
                    ls *= sizeMultipler;
                }
                ef.transform.localScale = ls;
                Debug.Log($"うんちんちんうんちんちん{ls.x}");
            }
        }


        /// <summary>
        /// 武器の音とエフェクト
        /// 基本の攻撃モーションにつくやつ
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="level">これアタックタイプ？</param>
        /// <param name="element"></param>
        /// <param name="subElement"></param>
        public void GAttackS(Vector3 pos, int level, int element, int motion, float sizeMultipler)
        {


            //サブ属性がないなら
            if (element < 3)
            {
                //斬撃
                if (motion == 0)
                {
                    Debug.Log($"ｓ{level}");
                    //共通エフェクトは追跡するって判断で

                    GManager.instance.PlaySound(slashSe[level], pos);
                    //ef = _generalPool.Spawn(slashEf[level], pos, pos.rotation, pos);
                }
                //刺突
                else if (motion == 1)
                {
                    GManager.instance.PlaySound(stabSe[level], pos);
                    //ef = _generalPool.Spawn(stabEf[level], pos, pos.rotation, pos);
                }
                //打撃
                else if (motion == 3)
                {
                    Debug.Log($"あ{level}");
                    GManager.instance.PlaySound(strikeSe[level], pos);
                    //ef = _generalPool.Spawn(strikeEf[level], pos, pos.rotation, pos);
                }
            }
            else
            {
                //斬撃
                if (motion == 0)
                {
                    //聖
                    if (element == 3)
                    {
                        GManager.instance.PlaySound(slashSe[level], pos);
                        GManager.instance.PlaySound(holySe[level], pos);
                        //ef = _generalPool.Spawn(slashHoly[level], pos, pos.rotation, pos);
                    }
                    //闇
                    else if (element == 4)
                    {
                        GManager.instance.PlaySound(slashSe[level], pos);
                        GManager.instance.PlaySound(darkSe[level], pos);
                        //ef = _generalPool.Spawn(slashDark[level], pos, pos.rotation, pos);
                    }
                    //炎
                    else if (element == 5)
                    {
                        GManager.instance.PlaySound(slashSe[level], pos);
                        GManager.instance.PlaySound(fireSe[level], pos);
                        //ef = _generalPool.Spawn(slashFire[level], pos, pos.rotation, pos);
                    }
                    //雷
                    else
                    {
                        GManager.instance.PlaySound(slashSe[level], pos);
                        GManager.instance.PlaySound(thunderSe[level], pos);
                        //ef = _generalPool.Spawn(slashThunder[level], pos, pos.rotation, pos);
                    }
                }
                //刺突
                else if (motion == 1)
                {
                    //聖
                    if (element == 3)
                    {
                        GManager.instance.PlaySound(stabSe[level], pos);
                        GManager.instance.PlaySound(holySe[level], pos);
                        //ef = _generalPool.Spawn(stabHoly[level], pos, pos.rotation, pos);
                    }
                    //闇
                    else if (element == 4)
                    {
                        GManager.instance.PlaySound(stabSe[level], pos);
                        GManager.instance.PlaySound(darkSe[level], pos);
                        //ef = _generalPool.Spawn(stabDark[level], pos, pos.rotation, pos);
                    }
                    //炎
                    else if (element == 5)
                    {
                        GManager.instance.PlaySound(stabSe[level], pos);
                        GManager.instance.PlaySound(fireSe[level], pos);
                        //ef = _generalPool.Spawn(stabFire[level], pos, pos.rotation, pos);
                    }
                    //雷
                    else
                    {
                        GManager.instance.PlaySound(stabSe[level], pos);
                        GManager.instance.PlaySound(thunderSe[level], pos);
                        //ef = _generalPool.Spawn(stabThunder[level], pos, pos.rotation, pos);
                    }
                }
                //打撃
                else if (motion == 2)
                {
                    //聖
                    if (element == 3)
                    {
                        GManager.instance.PlaySound(strikeSe[level], pos);
                        GManager.instance.PlaySound(holySe[level], pos);
                        //ef = _generalPool.Spawn(strikeHoly[level], pos, pos.rotation, pos);
                    }
                    //闇
                    else if (element == 4)
                    {
                        GManager.instance.PlaySound(strikeSe[level], pos);
                        GManager.instance.PlaySound(darkSe[level], pos);
                        //ef = _generalPool.Spawn(strikeDark[level], pos, pos.rotation, pos);
                    }
                    //炎
                    else if (element == 5)
                    {
                        GManager.instance.PlaySound(strikeSe[level], pos);
                        GManager.instance.PlaySound(fireSe[level], pos);
                        //ef = _generalPool.Spawn(strikeFire[level], pos, pos.rotation, pos);
                    }
                    //雷
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
        /// 攻撃時のパリィ不可エフェクトなどを出す
        /// </summary>
        /// <param name="type"></param>
        /// <param name="pos"></param>
        public void AdditionalEffect(int type,Transform pos,float sizeMultipler)
        {
            if (type == 2)
            {
                //パリィ不可の音とエフェクト
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


        //魔法系
        #region


        /// <summary>
        /// 音とエフェクト出すよ！！
        /// </summary>
        /// <param name="magicLevel"></param>
        /// <param name="element"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
         public Transform CastEfCall(int magicLevel, AtEffectCon.Element element,Transform pos)
        {

            //これサポートとデバフ、回復も必要じゃない？
            if ((int)element < 4)
            {
                if (element == AtEffectCon.Element.斬撃属性)
                {
                    GManager.instance.PlaySound(slashCast[magicLevel], pos.position);
                     return _generalPool.ControlSpawn(slashCEffect[magicLevel], pos.position, pos.rotation,pos).transform;
                }
                else if (element == AtEffectCon.Element.刺突属性)
                {
                    GManager.instance.PlaySound(stabCast[magicLevel], pos.position);
                    return _generalPool.ControlSpawn(stabCEffect[magicLevel], pos.position, pos.rotation,pos).transform;
                }
                else if (element == AtEffectCon.Element.打撃属性)
                {
                    GManager.instance.PlaySound(strikeCast[magicLevel], pos.position);
                    return _generalPool.ControlSpawn(strikeCEffect[magicLevel], pos.position, pos.rotation,pos).transform;
                }
                else if (element == AtEffectCon.Element.聖属性)
                {
                    GManager.instance.PlaySound(holyCast[magicLevel], pos.position);
                    return _generalPool.ControlSpawn(holyCEffect[magicLevel], pos.position, pos.rotation,pos).transform;
                }
            }
            else
            {
                if (element == AtEffectCon.Element.闇属性)
                {
                    GManager.instance.PlaySound(darkCast[magicLevel], pos.position);
                    return _generalPool.ControlSpawn(darkCEffect[magicLevel], pos.position, pos.rotation,pos).transform;
                }
                else if (element == AtEffectCon.Element.炎属性)
                {
                    
                    GManager.instance.PlaySound(fireCast[magicLevel], pos.position);
                    return _generalPool.ControlSpawn(fireCEffect[magicLevel], pos.position, pos.rotation,pos).transform;
                }
                else if (element == AtEffectCon.Element.雷属性)
                {
                    GManager.instance.PlaySound(thunderCast[magicLevel], pos.position);
                    return _generalPool.ControlSpawn(thunderCEffect[magicLevel], pos.position, pos.rotation,pos).transform;
                }
               
            }
           return null;
        }


        /// <summary>
        /// 詠唱エフェクト消して音も消した後発動エフェクト
        /// 完成
        /// </summary>
        /// <param name="inst"></param>
        public void CastEfClear(Transform inst, int magicLevel, AtEffectCon.Element element)
        {

            _generalPool.Despawn(inst);



            if ((int)element < 4)
            {
                if (element == AtEffectCon.Element.斬撃属性)
                {
                    _generalPool.Spawn(slashActiEffect[magicLevel], inst.position, inst.rotation);
                    GManager.instance.StopSound(slashCast[magicLevel], isStop: true);
                    GManager.instance.PlaySound(slashActivate[magicLevel], inst.position);
                }
                else if (element == AtEffectCon.Element.刺突属性)
                {
                    _generalPool.Spawn(stabActiEffect[magicLevel], inst.position, inst.rotation);
                    GManager.instance.StopSound(stabCast[magicLevel], isStop: true);
                    GManager.instance.PlaySound(stabActivate[magicLevel], inst.position);
                }
                else if (element == AtEffectCon.Element.打撃属性)
                {
                    _generalPool.Spawn(strikeActiEffect[magicLevel], inst.position, inst.rotation);
                    GManager.instance.StopSound(strikeCast[magicLevel], isStop: true);
                    GManager.instance.PlaySound(strikeActivate[magicLevel], inst.position);
                }
                else if (element == AtEffectCon.Element.聖属性)
                {
                    _generalPool.Spawn(holyActiEffect[magicLevel], inst.position, inst.rotation);
                    GManager.instance.StopSound(holyCast[magicLevel],isStop: true);
                    GManager.instance.PlaySound(holyActivate[magicLevel], inst.position);
                }
            }
            else
            {
                if (element == AtEffectCon.Element.闇属性)
                {
                    _generalPool.Spawn(darkActiEffect[magicLevel], inst.position, inst.rotation);
                    GManager.instance.StopSound(darkCast[magicLevel], isStop: true);
                    GManager.instance.PlaySound(darkActivate[magicLevel], inst.position);
                }
                else if (element == AtEffectCon.Element.炎属性)
                {
                    _generalPool.Spawn(fireActiEffect[magicLevel], inst.position, inst.rotation);
                    GManager.instance.StopSound(fireCast[magicLevel], isStop: true);
                    GManager.instance.PlaySound(fireActivate[magicLevel], inst.position);
                }
                else if (element == AtEffectCon.Element.雷属性)
                {
                    _generalPool.Spawn(thunderActiEffect[magicLevel], inst.position, inst.rotation);
                    GManager.instance.StopSound(thunderCast[magicLevel], isStop: true);
                    GManager.instance.PlaySound(thunderActivate[magicLevel], inst.position);
                }
                else if (element == AtEffectCon.Element.指定なし)
                {

                }
            }

        }

        /// <summary>
        /// 詠唱中断
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="magicLevel"></param>
        /// <param name="element"></param>
        public void CastStop(Transform inst, int magicLevel, AtEffectCon.Element element)
        {
            _generalPool.Despawn(inst);
            if ((int)element < 4)
            {
                if (element == AtEffectCon.Element.斬撃属性)
                {
 
                    GManager.instance.StopSound(slashCast[magicLevel], isStop: true);

                }
                else if (element == AtEffectCon.Element.刺突属性)
                {
                    GManager.instance.StopSound(stabCast[magicLevel], isStop: true);
                }
                else if (element == AtEffectCon.Element.打撃属性)
                {
                    GManager.instance.StopSound(strikeCast[magicLevel], isStop: true);
                }
                else if (element == AtEffectCon.Element.聖属性)
                {
                    GManager.instance.StopSound(holyCast[magicLevel], isStop: true);
                }
            }
            else
            {
                if (element == AtEffectCon.Element.闇属性)
                {
                    GManager.instance.StopSound(darkCast[magicLevel], isStop: true);
                }
                else if (element == AtEffectCon.Element.炎属性)
                {
                    GManager.instance.StopSound(fireCast[magicLevel], isStop: true);
                }
                else if (element == AtEffectCon.Element.雷属性)
                {
                    GManager.instance.StopSound(thunderCast[magicLevel], isStop: true);
                }
                else if (element == AtEffectCon.Element.none)
                {

                }
            }
        }






        #endregion


        #region 効果サウンド

        /// <summary>
        /// 状態変化の音を鳴らす
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