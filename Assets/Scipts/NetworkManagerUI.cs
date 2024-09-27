using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TestManager : MonoBehaviour
{
    [SerializeField] private Button serverButton;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;

    void Awake()
    {
        serverButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartServer();
        });

        hostButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
        });

        clientButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
        });
    }


    
    //void OnGUI()
    //{
    //    GUILayout.BeginArea(new Rect(10, 10, 300, 300));
    //    if (!m_NetworkManager.IsClient && !m_NetworkManager.IsServer)
    //    {
    //        StartButtons();
    //    }
    //    else
    //    {
    //        StatusLabels();

    //        SubmitNewPosition();
    //    }

    //    GUILayout.EndArea();
    //}

    //static void StartButtons()
    //{
    //    if (GUILayout.Button("Host")) m_NetworkManager.StartHost();
    //    if (GUILayout.Button("Client")) m_NetworkManager.StartClient();
    //    if (GUILayout.Button("Server")) m_NetworkManager.StartServer();
    //}

    //static void StatusLabels()
    //{
    //    var mode = m_NetworkManager.IsHost ?
    //        "Host" : m_NetworkManager.IsServer ? "Server" : "Client";

    //    GUILayout.Label("Transport: " +
    //        m_NetworkManager.NetworkConfig.NetworkTransport.GetType().Name);
    //    GUILayout.Label("Mode: " + mode);
    //}

    //static void SubmitNewPosition()
    //{
    //    if (GUILayout.Button(m_NetworkManager.IsServer ? "Move" : "Request Position Change"))
    //    {
    //        if (m_NetworkManager.IsServer && !m_NetworkManager.IsClient)
    //        {
    //            foreach (ulong uid in m_NetworkManager.ConnectedClientsIds)
    //                m_NetworkManager.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<HelloWorldPlayer>().Move();
    //        }
    //        else
    //        {
    //            //var playerObject = m_NetworkManager.SpawnManager.GetLocalPlayerObject();
    //            //var player = playerObject.GetComponent<HelloWorldPlayer>();
    //           // player.Move();
    //        }
    //    }
    //}
}
