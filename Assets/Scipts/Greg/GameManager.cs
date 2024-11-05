using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Use this if you're using TextMeshPro
using System.Collections.Generic;
using Unity.Collections;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private GameObject playerListEntryPrefab; // Prefab for each player name in the list
    [SerializeField] private Transform playerListContainer; // Container for player name entries
    [SerializeField] private Button startGameButton; // Button to start the game

    private void Awake()
    {
        startGameButton.onClick.AddListener(StartGame);
        //startGameButton.gameObject.SetActive(false); // Hide the button initially
    }

    private void OnEnable()
    {
        Debug.Log("OnEnabled Called");
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

        }
        Debug.Log("OnEnabled Finished");
    }

    private void OnDisable()
    {
        Debug.Log("OnDisabled Called");
        if (NetworkManager.Singleton != null)
        { 
           NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
           NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
        Debug.Log("OnDisabled Finished");
    }

    private void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log($"Connected Clients: {NetworkManager.Singleton.ConnectedClients.Count}");
            foreach (var client in NetworkManager.Singleton.ConnectedClients)
            {
                Debug.Log($"Client ID: {client.Key}");
            }

            Invoke(nameof(UpdatePlayerListForClientsServerRpc),0.5f);
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            UpdatePlayerListForClientsServerRpc(); // Refresh player list UI on disconnection
        }
    }

    [ClientRpc]
    private void UpdatePlayerListClientRpc(FixedString32Bytes[] playerNames)
    {
        ClearPlayerList();
        Debug.Log("UpdatePlayerListClientRpc Called");
        foreach (var playername in playerNames)
        {
            // Instantiate a player name entry for each connected client
            GameObject entry = Instantiate(playerListEntryPrefab, playerListContainer);
            entry.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = playername.ToString();
        }
        Debug.Log("UpdatePlayerListClientRpc Finished");
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdatePlayerListForClientsServerRpc()
    {
        Debug.Log("UpdatePlayerListForClientsServerRpc Called");
        if (!IsServer)
        {
            return;
        }

        List<FixedString32Bytes> playerNames = new List<FixedString32Bytes>();
        
        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            Debug.Log("Client Found");
            var playerNetwork = client.Value.PlayerObject.GetComponent<PlayerNetwork>();
            if (playerNetwork != null)
            {
                // Instantiate a player name entry for each connected client
                //GameObject entry = Instantiate(playerListEntryPrefab, playerListContainer);
                //entry.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = playerNetwork.userName.Value.ToString();
                playerNames.Add(playerNetwork.userName.Value);
            }
        }
        UpdatePlayerListClientRpc(playerNames.ToArray());
        Debug.Log("UpdatePlayerListForClientsServerRpc Finished");
        // Show the start button if at least 2 players are connected
        //startGameButton.gameObject.SetActive(NetworkManager.Singleton.ConnectedClients.Count > 1);
    }

    private void ClearPlayerList()
    {
        // Destroy all previous player entries to refresh the list
        foreach (Transform child in playerListContainer)
        {
            Destroy(child.gameObject);
        }
    }

    public void StartGame()
    {
        // Load the gameplay scene or trigger gameplay setup logic here
        // Example: SceneManager.LoadScene("GameScene");
        Debug.Log("Starting the game!");
    }
}
