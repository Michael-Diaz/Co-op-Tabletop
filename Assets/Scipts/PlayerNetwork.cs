using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{

    [SerializeField] private GameObject spawnObjectPrefab;
    void Update()
    {
        if (Input.GetKey(KeyCode.T)) {
            //TestServerRpc();
            GameObject spawnTransform =  Instantiate(spawnObjectPrefab);
            spawnTransform.GetComponent<NetworkObject>().Spawn(true);
        }


        if (!IsOwner) return;



        Vector3 moveDirection = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.W)) moveDirection.z = +1f;
        if (Input.GetKey(KeyCode.S)) moveDirection.z = -1f;
        if (Input.GetKey(KeyCode.A)) moveDirection.x = -1f;
        if (Input.GetKey(KeyCode.D)) moveDirection.x = +1f;

        float moveSpeed = 2f;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    [ServerRpc]
    private void TestServerRpc()
    {
        Debug.Log("TestServerRpc" + OwnerClientId);
    }

    [ClientRpc]
    private void TestClientRpc()
    {
        Debug.Log("TestClientRpc" + OwnerClientId);
    }
}
