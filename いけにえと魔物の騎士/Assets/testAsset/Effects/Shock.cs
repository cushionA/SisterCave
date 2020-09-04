using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Shock : MonoBehaviour
{
    ParticleSystem particle;
    void Start()
    {
        particle = this.GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (particle.isStopped) //パーティクルが終了したか判別
        {
            Addressables.ReleaseInstance(this.gameObject);
        }
    }
}
