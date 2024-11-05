using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject[] Menus;
    [SerializeField] private GameObject ClientMenu;
    [SerializeField] private GameObject UsernameMenu;
    [SerializeField] private GameObject ListPlayersMenu;
    [SerializeField] private GameObject EntireMenu;
    [SerializeField] private GameObject backGround;
    [SerializeField] private Button LeaveLobbyButton;
    private List<GameObject> ListPrefabs = new List<GameObject>();
    [SerializeField] private GameObject listPrefab;
    [SerializeField] private Transform listContainer;
    //private string playerName;

    public void SwitchMenu(GameObject menuToActivate)
    {
        foreach(GameObject g in Menus)
        {
            g.SetActive(false);
        }
        menuToActivate.SetActive(true);
    }
    
    public void DisableUI()
    {
        //Debug.Log("Create Lobby Button pressed");
        SwitchMenu(EntireMenu);
        backGround.SetActive(false);
    }
    public void JoinLobby()
    {
        SwitchMenu(ListPlayersMenu);
    }
    public void LeaveLobby()
    {
        SwitchMenu(ListPlayersMenu);
    }

    //private void OnEnable()
    //{
    //    //LobbyManager.OnLobbyUpdated += UpdateLobbyUI;
    //}

    //private void OnDisable()
    //{
    //    //LobbyManager.OnLobbyUpdated -= UpdateLobbyUI;
    //}

    private void ClearListUI()
    {
        foreach(GameObject g in ListPrefabs)
        {
            Destroy(g);
        }
        ListPrefabs.Clear();
    }
}
