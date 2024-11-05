using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Threading.Tasks;
using System;


    public class LobbyManager : MonoBehaviour
    {
        public static event Action<List<Player>> OnLobbyUpdated;
        public Unity.Services.Lobbies.Models.Lobby hostLobby;
        public Unity.Services.Lobbies.Models.Lobby joinedLobby;
        private float heartbeatTimer;
        private float lobbyUpdateTimer;
        public string playerName;
        private Player playerInst;

        private void Start()
        {
            //await UnityServices.InitializeAsync();

            playerName = playerName = "PlayerName" + UnityEngine.Random.Range(1, 100);
        //AuthenticationService.Instance.SignedIn += () =>
        //{
        //    Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        //};

        //await AuthenticationService.Instance.SignInAnonymouslyAsync();
        //playerName = "TestName" + UnityEngine.Random.Range(1, 100);
    }
        public Player MakePlayer()
        {
            return new Player
            {
                //id = AuthenticationService.Instance.PlayerId,
                Data = new Dictionary<string, PlayerDataObject>
                    {
                        { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
                    }
            };
        }
        public void CreateLobby()
        {
            string lobbyName = "MyLobby";
            int maxPlayer = 4;
            playerInst = MakePlayer();
            hostLobby = new Unity.Services.Lobbies.Models.Lobby(name: lobbyName, maxPlayers: maxPlayer, isPrivate: false);
            joinedLobby = hostLobby;
            Debug.Log("Created Lobby " + hostLobby.Name + " " + hostLobby.MaxPlayers + " " + hostLobby.Id + " " + hostLobby.LobbyCode +"  " + hostLobby.Players);
            OnLobbyUpdated?.Invoke(hostLobby.Players);
        }

        public void JoinLobby()
        {
            Debug.LogWarning("CALLING JOINLOBBY");
            joinedLobby = hostLobby;
            OnLobbyUpdated?.Invoke(joinedLobby.Players);
        //try
        //    {
        //        QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
        //        await Lobbies.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id);
        //        OnLobbyUpdated?.Invoke(joinedLobby.Players);
        //    }
        //    catch (LobbyServiceException e)
        //    {
        //        Debug.Log(e);
        //    }
        }

    public string GetPlayerUsername()
    {
        return playerName;
    }
   
    // Update is called once per frame
    private async Task Update()
        {
            await HandleLobbyHeartbeat();
            await HandleLobbyPollForUpdates();
        }

        public void UpdatePlayerName(string newPlayerName)
        {
        
            
                Debug.LogWarning($"Original Player: {playerName}");
                playerName = newPlayerName;
                playerInst = MakePlayer();
          OnLobbyUpdated?.Invoke(joinedLobby.Players);
           
    }

        public void LeaveLobby()
        {
                joinedLobby = null;
                hostLobby = null;
                OnLobbyUpdated?.Invoke(new List<Player>());
                Debug.Log("Left the Lobby Successfully");
      
        }


        private async Task HandleLobbyHeartbeat()
        {
            if (hostLobby != null)
            {
                heartbeatTimer -= Time.deltaTime;
                if (heartbeatTimer < 0f)
                {
                    float heartbeatTimerMax = 15;
                    heartbeatTimer = heartbeatTimerMax;
                    await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
                }
            }
        }

        private void PrintPlayers(Unity.Services.Lobbies.Models.Lobby lobby)
        {
            //Debug.Log("Players in Lobby " + lobby.Name);
            foreach (Player p in lobby.Players)
            {
                //Debug.Log(p.Id + " " + p.Data["PlayerName"].Value + lobby.Data["GameMode"].Value + " " + lobby.Data["Map"].Value);
                Debug.Log($"{p.Id} {p.Data["PlayerName"].Value} {lobby.Data["GameMode"].Value} {lobby.Data["Map"].Value}");
            }
        }

        private async Task HandleLobbyPollForUpdates()
        {
            if (joinedLobby != null)
            {
                lobbyUpdateTimer -= Time.deltaTime;
                if (lobbyUpdateTimer < 0f)
                {
                    float lobbyTimerMax = 1.1f;
                    lobbyUpdateTimer = lobbyTimerMax;
                    Unity.Services.Lobbies.Models.Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                    //if (lobby.Players.Count != joinedLobby.Players.Count)
                    //{
                       // OnLobbyUpdated?.Invoke();
                    //}
                    joinedLobby = lobby;
                }
            }
        }

    //private void UpdateLobbyUI(List<Player> players)
    //{
    //    // Notifies UI about lobby updates
    //    OnLobbyUpdated?.Invoke(players);
    //}
    //NOTINUSE
    //private async Task UpdateLobbyGameMode(string gameMode)
    //{
    //    try
    //    {
    //        hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
    //        {
    //            Data = new Dictionary<string, DataObject>
    //        {
    //            {"GameMode", new DataObject(DataObject.VisibilityOptions.Public,gameMode) }
    //        }
    //        });
    //        joinedLobby = hostLobby;

    //        PrintPlayers(hostLobby);
    //        // Debug.Log("Join Lobby with " + lobbyCode);

    //    }
    //    catch (LobbyServiceException e)
    //    {
    //        Debug.Log(e);
    //    }
    //}
}
