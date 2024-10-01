using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dice : MonoBehaviour
{
    public int diceNum;
    public DiceFace[] faces;
    public Rigidbody rb;
    public float timer = 3;
    public bool timerRunning = false;
    public bool diceMoving = false;

    void Awake()
    {
        faces = GetComponentsInChildren<DiceFace>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /*if(Input.GetKeyDown(KeyCode.E))
        {
            GetDiceFace();
        }*/

        if (diceMoving)
        {
            // Used to see if the die is not moving and the timer is not already running
            if (rb.IsSleeping() && !timerRunning)
            {
                GetDiceFace();
                timerRunning = true;
                diceMoving = false;
            }
        }

        if (timerRunning)
        {
            timer -= Time.deltaTime;
        }

        if (timer <= 0)
        {
            timerRunning = false;

            Destroy(this.gameObject);
        }
    }

    /* This function gets the array of faces that is made in awake and then searches through the array to see which face has the isFaceUp
     * variable set to true. It then assigns the number on the die face to the diceNum variable*/
    public void GetDiceFace()
    {
        foreach (DiceFace face in faces)
        {
            if (face.isFaceUp == true)
            {
                diceNum = face.faceNumber;
            }
        }
    }

    /* This function takes in a vector named forceVector and a float named force. The function adds the force provided with the force provided
     * to the objects rigidbody along the vector that is provided. It then adds torgue to the object on the vector (1, 1, 1) multiplied by the
     * force provided multiplied random floats between 0 and 1. The function then sets the diceMoving variable to true. */
    public void ThrowDice(Vector3 forceVector, float force)
    {
        float rand1 = Random.Range(0.0f, 1.0f);
        float rand2 = Random.Range(0.0f, 1.0f);
        float rand3 = Random.Range(0.0f, 1.0f);
        rb.AddForce(forceVector * force, ForceMode.Impulse);
        rb.AddTorque(force * rand1, force * rand2, force * rand3, ForceMode.Impulse);

        diceMoving = true;
    }
}
