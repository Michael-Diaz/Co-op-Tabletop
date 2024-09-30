using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

public class DiceFace : MonoBehaviour
{
    public bool isFaceUp = false;
    public int faceNumber;
    public float threshold = 1;
    public TextMeshProUGUI faceNumText;

    // Start is called before the first frame update
    void Start()
    {
        faceNumText.text = faceNumber.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.up.y >= threshold)
        {
            isFaceUp = true;
        }
        else
        {
            isFaceUp = false;
        }
    }
}
