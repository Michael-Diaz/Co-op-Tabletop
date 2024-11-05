using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button serverButton;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private TMP_InputField UserNameInputField;
    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
        SetPlayerUsername();
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        SetPlayerUsername();
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        SetPlayerUsername();
    }

    public void SetPlayerUsername()
    {
        string username = UserNameInputField.text;
        PlayerPrefs.SetString("Username", username);
        Debug.Log($"Player: {PlayerPrefs.GetString("Username")}");
    }
}
