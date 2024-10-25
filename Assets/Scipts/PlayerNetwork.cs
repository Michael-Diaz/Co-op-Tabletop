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

        //if (spawnedObject != null)
        //{
        //    OrbitAroundPlayer();
        //}
    }

    private void FixedUpdate()
    {
        if (spawnedObject != null)
        {
            OrbitAroundPlayer();
        }
    }

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

    private void OrbitAroundPlayer()
    {
        orbitAngle += orbitSpeed * Time.deltaTime;
        float radians = orbitAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(radians) * orbitRadius, 0, Mathf.Sin(radians) * orbitRadius);
        spawnedObject.transform.position = transform.position + offset;
        Debug.Log($"Spawned Object Position (Server): {spawnedObject.transform.position}");
    }
    public override void OnNetworkSpawn()
    {
        Debug.Log($"Network SPawn called for clientId: {OwnerClientId}, IsOWner {IsOwner}");
        if (IsOwner)
        {
            userName.Value = LoadUsername();
            Debug.Log($"Network Spawn clientId: {OwnerClientId}, IsOWner {IsOwner}");

        }
    }
    public string LoadUsername()
    {
        return PlayerPrefs.GetString("Username", "");
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

        spawnedObject.GetComponent<NetworkObject>().Despawn(true);
        Destroy(spawnedObject);
        spawnedObject = null;
    }
}
