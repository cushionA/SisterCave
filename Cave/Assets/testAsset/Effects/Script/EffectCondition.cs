using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using UnityEditor;
using DarkTonic.MasterAudio;

[System.Serializable]
public class EffectCondition
{



    [Header("再生するステート")]
    [MMInspectorGroup("再生条件", true)]

    ///<summary>
    /// どのステートで出てくるか
    /// 最初にリスト作る時に割り振られる
    /// あっちの選んだステートの数から上から順に割り振る
    /// </summary>
    public MoreMountains.CorgiEngine.EffectControllAbility.SelectState _useState;




    public enum EmitType
    {
        Soon,//最初に一度きり出す
        Repeat,//そのステートの間繰り返す
        Wait,// 少し待って再生
        WaitRepeat,//少し待って繰り返し
        Loop,//ずっと鳴らし続ける
        WaitLoop,
        End//状態終わった時に
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
    }

    [Header("エフェクトの設定")]
    [MMInspectorGroup("エフェクトの設定", true)]
    [MMCondition("_effectOn", true)]
    [Tooltip("エフェクトの設定リスト")]
    public List<StateEffect> _stateEffects;


    [Header("サウンドの設定")]
    [MMInspectorGroup("サウンドの設定", true)]


    [MMCondition("_soundOn",false)]
    [Tooltip("音の設定リスト")]
    public List<StateSound> _stateSounds;






}
