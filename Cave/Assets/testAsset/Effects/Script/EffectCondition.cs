using DarkTonic.MasterAudio;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EffectCondition
{
    //定義
    #region


    public enum EmitType
    {
        Soon,//最初に一度きり出す
        Repeat,//そのステートの間繰り返す
        Wait,// 少し待って再生
        WaitRepeat,//少し待って繰り返し
        Loop,//ずっと鳴らし続ける
        WaitLoop,
        End,//状態終わった時に
        None//再生しない
    }


    [System.Serializable]
    /// <summary>
    /// エフェクトの詳細データ。
    /// 何番目に再生されるかはリストの設定でよろしく
    /// </summary>
    public class StateEffect
    {
        [Tooltip("出すエフェクト")]
        public ParticleSystem _useEffect;

        [Tooltip("Soonはすぐに再生、Repeatはステートの間ループ、Waitは待ってから出現")]
        public EmitType _emitType = EmitType.Soon;

        [Tooltip("エフェクトが出てくる場所")]
        public Transform _emitPosition;

        [Tooltip("エフェクトが出たあとついてくるか")]
        public bool _isFollow;

        [Tooltip(" 再生スピードがアニメの再生速度の影響を受けるかどうか")]
        public bool _matchAnime;


        [Tooltip("親の方向を無視するかどうか")]
        /// <summary>
        /// 親の方向無視するなら
        /// </summary>
        public bool ignoreDirection;

        /// <summary>
        /// このエフェクトが既に使われたかどうか
        /// ループやリピートで使う
        /// </summary>
        [HideInInspector]
        public bool isUsed;
    }


    [System.Serializable]
    /// <summary>
    /// サウンドの詳細データ。
    /// 何番目に再生されるかはリストの設定でよろしく
    /// </summary>
    public class StateSound
    {
        [Tooltip("出すエフェクト")]
        [SoundGroup]
        public string _useSound;

        [Tooltip("Soonはすぐに再生、Repeatはステートの間ループ、Waitは待ってから出現")]
        public EmitType _playType = EmitType.Soon;

        [Tooltip("エフェクトが出たあとついてくるか")]
        public bool _isFollow;

        [Tooltip(" 再生スピードがアニメの再生速度の影響を受けるかどうか")]
        public bool _matchAnime;

        /// <summary>
        /// この音声が既に使われたかどうか
        /// ループやリピートで使う
        /// </summary>
        [HideInInspector]
        public bool isUsed;
    }

    #endregion



    [Header("再生するステート")]
    ///<summary>
    /// どのステートで出てくるか
    /// 最初にリスト作る時に割り振られる
    /// あっちの選んだステートの数から上から順に割り振る
    /// </summary>
    public MoreMountains.CorgiEngine.EffectControllAbility.SelectState _useState;

    /// <summary>
    /// 歩行とかで共通の足音やらを使うならそっちで
    /// </summary>
    [Header("共通エフェクトを使用するか")]
    [Tooltip("共通の設定を使用するか")]
    public bool generalEffect;

    [Header("エフェクトの設定")]
    [Tooltip("エフェクトの設定リスト")]
    public List<StateEffect> _stateEffects;



    /// <summary>
    /// 歩行とかで共通の足音やらを使うならそっちで
    /// </summary>
    [Header("共通サウンドを使用するか")]
    [Tooltip("共通の設定を使用するか")]
    public bool generalSound;

    [Header("サウンドの設定")]
    [Tooltip("音の設定リスト")]
    public List<StateSound> _stateSounds;






}
