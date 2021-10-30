using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTest : MonoBehaviour
{
    [SerializeField] GameObject Enemy;
    [SerializeField] bool isBattle;

    // Start is called before the first frame update
    void Start()
    {
        if (isBattle && Enemy != null)
        {
            Enemy.transform.position = this.gameObject.transform.position;
            GManager.instance.Player.transform.position = this.gameObject.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
