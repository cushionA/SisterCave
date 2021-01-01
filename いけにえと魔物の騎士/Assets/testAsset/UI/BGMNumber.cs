using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGMNumber : MonoBehaviour
{
    // Start is called before the first frame update
    public Text number;
    public Slider indicator;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        number.text = $"BGM:{indicator.value}";
    }
}
