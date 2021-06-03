using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetFPS : MonoBehaviour
{
     [SerializeField] int FPS;
    /// <summary>
    /// Reflect measurement results every 'EveryCalcurationTime' seconds.
    /// EveryCalcurationTime 秒ごとに計測結果を反映する
    /// </summary>
    [SerializeField, Range(0.1f, 1.0f)]
    float EveryCalcurationTime = 0.5f;

    [SerializeField] bool isEnable;

    // Start is called before the first frame update
    void Awake()
    {
        Application.targetFrameRate = FPS;
    }

    /// <summary>
    /// FPS value
    /// </summary>
    public float Rate
    {
        get; private set;
    }

    int frameCount;
    float prevTime;

    void Start()
    {
        Application.targetFrameRate = FPS;
        frameCount = 0;
        prevTime = 0.0f;
        Rate = 0.0f;
    }
    void Update()
    {
        if (isEnable)
        {
            Debug.Log($"FPSは{Rate}");
        }
        frameCount++;
        float time = Time.realtimeSinceStartup - prevTime;

        // n秒ごとに計測
        if (time >= EveryCalcurationTime)
        {
            Rate = frameCount / time;

            frameCount = 0;
            prevTime = Time.realtimeSinceStartup;
        }
    }
}
