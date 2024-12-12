using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowDice : MonoBehaviour
{
    public GameObject d4;
    public GameObject d6;
    public GameObject d8;
    public GameObject d12;
    public GameObject d20;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Dice die;
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (d4 != null)
            {
                GameObject temp = (GameObject)Instantiate(d4, new Vector3(0,3,0), Quaternion.identity);

                die = temp.GetComponentInChildren<Dice>();

                die.ThrowDice(new Vector3(0, .5f, 1), 5);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (d6 != null)
            {
                GameObject temp = (GameObject)Instantiate(d6, new Vector3(0, 3, 0), Quaternion.identity);

                die = temp.GetComponentInChildren<Dice>();

                die.ThrowDice(new Vector3(0, .5f, 1), 5);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (d8 != null)
            {
                GameObject temp = (GameObject)Instantiate(d8, new Vector3(0, 3, 0), Quaternion.identity);

                die = temp.GetComponentInChildren<Dice>();

                die.ThrowDice(new Vector3(0, .5f, 1), 5);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (d12 != null)
            {
                GameObject temp = (GameObject)Instantiate(d12, new Vector3(0, 3, 0), Quaternion.identity);

                die = temp.GetComponentInChildren<Dice>();

                die.ThrowDice(new Vector3(0, .5f, 1), 5);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (d20 != null)
            {
                GameObject temp = (GameObject)Instantiate(d20, new Vector3(0, 3, 0), Quaternion.identity);

                die = temp.GetComponentInChildren<Dice>();

                die.ThrowDice(new Vector3(0, .5f, 1), 5);
            }
        }
    }
}
