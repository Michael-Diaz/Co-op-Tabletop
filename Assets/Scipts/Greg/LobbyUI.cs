using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button CreateLobbyButton;
    [SerializeField] private Button JoinLobbyButton;
    [SerializeField] private GameObject StartMenu;
    [SerializeField] private GameObject InputUsernameMenu;
    [SerializeField] private GameObject ListPlayerMenu;
    [SerializeField] private GameObject LobbyMenu;

    [SerializeField] private TMP_InputField UserNameInputField;
    [SerializeField] private LobbyManager lobby;
    [SerializeField] private Button LeaveLobbyButton;
    private List<GameObject> ListPrefabs;
    [SerializeField] private GameObject listPrefab;
    [SerializeField] private Transform listContainer;

    // Function that listens for lobby update (player leaves or joins)
    
    async public void CreateLobby()
    {
        Debug.Log("Create Lobby Button pressed");
        await lobby.CreateLobby();
        StartMenu.gameObject.SetActive(false);
        InputUsernameMenu.gameObject.SetActive(true);
        //ListPlayerMenu.gameObject.SetActive(true);
    }

    async public void JoinLobby()
    {
        await lobby.JoinLobby();
        StartMenu.gameObject.SetActive(false);
    }

    async public void LeaveLobby()
    {
        await lobby.JoinLobby();
        StartMenu.gameObject.SetActive(false);
    }

    async public void UpdatePlayerName()
    {
        await lobby.UpdatePlayerName("Billy");
        ListPlayerMenu.gameObject.SetActive(true);
        InputUsernameMenu.gameObject.SetActive(false);
    }

    async public void StartSession()
    {
        LobbyMenu.gameObject.SetActive(false);
    }



    private void OnEnable()
    {
        LobbyManager.OnLobbyUpdated += UpdateLobbyUI;
    }

    private void OnDisable()
    {
        LobbyManager.OnLobbyUpdated -= UpdateLobbyUI;
    }

    private void UpdateLobbyUI(List<Player> players)
    {
        ClearLobbyUI();
        int i = 0; 
        foreach(Player p in players)
        {
            
            GameObject temp =  Instantiate(listPrefab, listContainer);
            temp.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,40*i);
            ListPrefabs.Add(temp);

            temp.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = p.Data["PlayerName"].Value;
            Debug.Log($"{ temp.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text }");
            i++; 
        }
    }

    private void ClearLobbyUI()
    {
        foreach(GameObject g in ListPrefabs)
        {
            Destroy(g);
            ListPrefabs.Remove(g);
        }
    }
}
