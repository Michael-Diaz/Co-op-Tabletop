using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SphereCircle : NetworkBehaviour
{
    [SerializeField] public GameObject player;
    public float radius = 5f;
    public float speed = 5f;
    private float angle;

    // Update is called once per frame
    void Update()
    {
        angle += speed * Time.deltaTime;
        float x = player.transform.position.x + Mathf.Cos(angle) * radius;
        float z = player.transform.position.z + Mathf.Sin(angle) * radius;
        transform.position = new Vector3(x, player.transform.position.y, z);
    }

    public void SetTransform(GameObject test)
    {
        player = test;
    }
}
