using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowDice : MonoBehaviour
{
    public GameObject dice;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            if (dice != null)
            {
                GameObject temp = (GameObject)Instantiate(dice, new Vector3(0,3,0), Quaternion.identity);

                Dice temp2 = temp.GetComponent<Dice>();

                temp2.ThrowDice(new Vector3(0, .5f, 1), 5);
            }
        }
    }
}
