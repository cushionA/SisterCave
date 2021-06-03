using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMove : MonoBehaviour
{
    public Vector2 jumpForce;
    public bool Test;

    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Test)
        {
            rb.AddForce(jumpForce, ForceMode2D.Impulse);
            //Debug.log("危険");
        }

    }
}
