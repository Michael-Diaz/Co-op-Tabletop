using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    private Rigidbody rb;
    private NetworkRigidbody networkRb;
    private Vector3 movementInput;
    public float moveSpeed = 5f;
    //[SerializeField] private float interpolationSpeed = 10f;
    [SerializeField] private GameObject spawnObjectPrefab;
    private GameObject spawnedObject;
    [SerializeField] public NetworkVariable<FixedString32Bytes> userName { get; private set; } = new NetworkVariable<FixedString32Bytes>("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    //Test Spawn Movement Sync
    private float orbitRadius = 2.0f;
    private float orbitSpeed = 50f;
    private float orbitAngle = 0f;
   

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        networkRb = GetComponent<NetworkRigidbody>();
        rb.isKinematic = false;
    }

    void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.T)) {
            SpawnObjectServerRpc();
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            DespawnObjectServerRpc();
        }

        movementInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if(movementInput.magnitude > 0.1f)
        {
            MovePlayerServerRpc(movementInput);
        }

        if (spawnedObject != null)
        {
            UpdateOrbitPositionServerRpc();
            //OrbitAroundPlayer();
        }
    }

    //private void FixedUpdate()
    //{
    //    if (spawnedObject != null)
    //    {
    //        OrbitAroundPlayer();
    //    }
    //}

    [ServerRpc]
    private void MovePlayerServerRpc(Vector3 movementInput, ServerRpcParams rpcParams = default)
    {
        Debug.Log($"MovePlayerServerRpc called with input: {movementInput} by {OwnerClientId}");

        Vector3 moveWithVelocity = movementInput * moveSpeed;
        if (rb.isKinematic)
            Debug.LogError("NOOOOO ITS Kinematic");
        else
            rb.velocity = new Vector3(moveWithVelocity.x, rb.velocity.y, moveWithVelocity.z);

        Debug.Log($"Rigidbody Velocity: {rb.velocity}");
        UpdateClientPositionClientRpc(rb.velocity);
    }
    [ClientRpc]
    private void UpdateClientPositionClientRpc(Vector3 position)
    {
            rb.velocity = position;
            Debug.Log($"Updated Velocity: {transform.position} by client {OwnerClientId}");
    }

    [ServerRpc]
    private void UpdateOrbitPositionServerRpc()
    {
        orbitAngle += orbitSpeed * Time.deltaTime;
        float radians = orbitAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(radians) * orbitRadius, 0, Mathf.Sin(radians) * orbitRadius);
        Vector3 newPosition =  transform.position + offset;
        UpdateOrbitPositionClientRpc(newPosition);
        //Debug.Log($"Spawned Object Position (Server): {spawnedObject.transform.position}");
    }

    [ClientRpc]
    private void UpdateOrbitPositionClientRpc(Vector3 newPosition)
    {
        if(spawnedObject != null)
        {
            spawnedObject.transform.position = newPosition;//Vector3.Lerp(spawnedObject.transform.position, newPosition, Time.deltaTime * interpolationSpeed);
            Debug.Log($"Spawned Object Position (Server): {spawnedObject.transform.position}");
        }
        //orbitAngle += orbitSpeed * Time.deltaTime;
        //float radians = orbitAngle * Mathf.Deg2Rad;
        //Vector3 offset = new Vector3(Mathf.Cos(radians) * orbitRadius, 0, Mathf.Sin(radians) * orbitRadius);
        //spawnedObject.transform.position = transform.position + offset;
    }

    public override void OnNetworkSpawn()
    {
        //Debug.Log($"Network SPawn called for clientId: {OwnerClientId}, IsOWner {IsOwner}");
        if (IsOwner)
        {
            string username = PlayerPrefs.GetString("Username", "Guest");
            userName.Value = username;
            Debug.Log($"On Network Spawn in Player Network clientId: {OwnerClientId}, IsOWner {IsOwner} Username: {userName.Value}");

        }
    }
    public string LoadUsername()
    {
        return PlayerPrefs.GetString("Username");
    }
    [ServerRpc]
    private void SpawnObjectServerRpc(ServerRpcParams rpcParams = default)
    {
        if (spawnedObject != null)
        {
            Debug.Log("Object exists");
            return;
        }

        spawnedObject = Instantiate(spawnObjectPrefab, transform.position + Vector3.right * 2, Quaternion.identity);
        NetworkObject networkObject = spawnedObject.GetComponent<NetworkObject>();

        if (networkObject != null)
        {
            networkObject.Spawn(true);
            NotifyClientsOfSpawnedObjectClientRpc(spawnedObject.GetComponent<NetworkObject>().NetworkObjectId);
        }
        else
        {
            Debug.Log("Object does not have a network object enabled");
        }
    }
    [ServerRpc]
    private void DespawnObjectServerRpc(ServerRpcParams rpcParams = default)
    {
        if (spawnedObject == null)
        {
            Debug.Log("No Object to despawn");
            return;
        }
        NetworkObject networkObject = spawnedObject.GetComponent<NetworkObject>();

        if(networkObject != null)
        {
            NotifyClientsOfDespawnedObjectClientRpc(networkObject.NetworkObjectId);
            networkObject.Despawn(true);
            Destroy(spawnedObject);
            spawnedObject = null;
        }
    }

    [ClientRpc]
    private void NotifyClientsOfSpawnedObjectClientRpc(ulong networkObjectId, ClientRpcParams rpcParams = default)
    {
        NetworkObject networkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
        if (networkObject != null)
        {
            Debug.Log($"SpawnedObject for Clients {networkObjectId}");
        }
    }
    [ClientRpc]
    private void NotifyClientsOfDespawnedObjectClientRpc(ulong networkObjectId, ClientRpcParams rpcParams = default)
    {
        if(NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out var networkObject))
        {
            networkObject.Despawn();
            Destroy(networkObject.gameObject);
            Debug.Log($"DespawnedObject for Clients {networkObjectId}");
        }
    }

}
