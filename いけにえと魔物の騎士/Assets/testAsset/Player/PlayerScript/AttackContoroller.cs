using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackContoroller : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] BoxCollider2D col1;
    [SerializeField]CircleCollider2D col2;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        col1.enabled = false;
        col2.enabled = false;
    }



}
