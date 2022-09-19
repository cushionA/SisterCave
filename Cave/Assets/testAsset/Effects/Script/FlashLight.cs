using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FunkyCode;

/// <summary>
/// ��u��������X�N���v�g
/// </summary>
public class FlashLight : MonoBehaviour
{
    [SerializeField]LightSprite2D sprite;
    [Header("���݂��鎞��")]
    [SerializeField] float exTime = 0.1f;
    float lightTime;

    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        lightTime += Time.deltaTime;

        if(lightTime >= exTime)
        {
            sprite.enabled = false;
            lightTime = 0;
        }
    }
}
