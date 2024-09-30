using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dice : MonoBehaviour
{
    public int diceNum;
    public DiceFace[] faces;
    public Rigidbody rb;
    public float timer = 1;
    public bool timerRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        faces = GetComponentsInChildren<DiceFace>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.velocity.magnitude < 1 && !timerRunning)
        {
            GetDiceFace();
            timerRunning = true;
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
    public void ThrowDice(Vector3 forceVector, float force)
    {
        float rand1 = Random.Range(0.0f, 1.0f);
        float rand2 = Random.Range(0.0f, 1.0f);
        float rand3 = Random.Range(0.0f, 1.0f);
        rb.AddForce(forceVector * force, ForceMode.Impulse);
        rb.AddTorque(force * rand1, force * rand2, force * rand3, ForceMode.Impulse);
    }
}
