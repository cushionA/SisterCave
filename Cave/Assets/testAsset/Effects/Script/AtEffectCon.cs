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

    //おおまかな設計
    ///　<summary>
    //この機能は音声やエフェクトを再生するための機能です
    ///　</summary>
    ///　
    [AddComponentMenu("Corgi Engine/Character/Abilities/AtEffectCon")]
    public class AtEffectCon : MyAbillityBase
    {

        /// このメソッドは、ヘルプボックスのテキストを表示するためにのみ使用されます。
        public override string HelpBoxText() { return "この機能は音声やエフェクトを再生するための機能です。"; }


        //フィールド
        //――――――――――――――――――――――――――――――――――――――――-―――――――――――――――――――――――
        #region

        //定義
        #region

        /// <summary>
        /// 属性の列挙型
        /// </summary>
        [Flags]
        public enum Element
        {
            斬撃属性 = 1 << 0,
            刺突属性 = 1 << 1,
            打撃属性 = 1 << 2,
            聖属性 = 1 << 3,
            闇属性 = 1 << 4,
            炎属性 = 1 << 5,
            雷属性 = 1 << 6,
            毒 = 1 << 7,
            浸食 = 1 << 8,
            凍結 = 1 << 9,
            移動速度低下攻撃 = 1 << 10,
            攻撃力低下攻撃 = 1 << 11,
            防御力低下攻撃 = 1 << 12,
            弱点属性 = 1 << 13,//敵の弱点属性をサーチして代わりに使う
            指定なし = 0
        }

        public enum AdditionalType
        {
            normal,
            dispariable,
            chance
        }

        /// <summary>
        /// 攻撃の情報
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
            [Header("使用するエフェクト")]
            public ParticleSystem particle;

            [Header("エフェクトがついてくるかどうか")]
            public bool isFollow;

            [SoundGroup]
            [Header("呼び出すサウンドの名前")]
            public string sound;

            [Header("アニメイベントで呼び出す番号")]
            public int callNumber;
        }


        #endregion


        //インスペクタで設定
        #region

        /// <summary>
        /// エフェクトを生成するクラス
        /// オブジェクトプール機能
        /// Spawnで生成。エフェクトの再生が終了すると自動で消滅
        /// </summary>
        [SerializeField]
        SpawnPool atPool;

        /// <summary>
        /// エフェクトを生成するクラス
        /// オブジェクトプール機能
        /// Spawnで生成。エフェクトの再生が終了すると自動で消滅
        /// </summary>
        [SerializeField]
        SpawnPool magicPool;


        [Header("エフェクトのサイズ倍率")]
        /// <summary>
        /// エフェクトのサイズ倍率
        /// </summary>
        public float sizeMultipler = 1;


        [SerializeField]
        Transform effectPosi;

        #endregion



        
        #region 内部ステータス

        AttackInfo _useData;

        /// <summary>
        /// 攻撃に使うエフェクトとサウンド
        /// </summary>
        List<EffectAndSound> useList;

        /// <summary>
        /// 使用中の詠唱エフェクトを保持
        /// </summary>
        Transform castEffect;


        #endregion




        #endregion


        //メソッド
        //――――――――――――――――――――――――――――――――――――――――-―――――――――――――――――――――――
        #region

        /// <summary>
        ///　ここで、パラメータを初期化する必要があります。
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();

        }


        #region 攻撃関連のメソッド

        /// <summary>
        /// 攻撃のデータを取得する
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
            //通常攻撃
            _useData.level = level;
            if (level != AttackValue.AttackLevel.Special)
            {
                
                _useData.adType = adType;
                _useData.element = element;
                _useData.type = type;
            }
        }


        /// <summary>
        /// 攻撃エフェクトを呼ぶアニメイベント
        /// </summary>
        /// <param name="num"></param>
        public void AttackEfEvent(int num = 0)
        {
            //通常攻撃
            if (_useData.level != AttackValue.AttackLevel.Special)
            {
                //音だけ
                if (_useData.level == AttackValue.AttackLevel.SEOnly)
                {
                    MyCode.SoundManager.instance.GAttackS(effectPosi.position, (int)_useData.level, (int)_useData.element, (int)_useData.type, sizeMultipler);
                }
                //エフェクト付き
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





        #region 魔法エフェクトの再生


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


        #region　基礎機能

        /// <summary>
        /// 攻撃に使う固有エフェクトの設定
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

            //エフェクトをリセット
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
        /// 魔法に使う固有エフェクトの設定
        /// </summary>
        /// <param name="_newPrefab"></param>
        public void MagicResorceReset(List<PrefabPool> _newPrefab)
        {
            //エフェクトをリセット
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
