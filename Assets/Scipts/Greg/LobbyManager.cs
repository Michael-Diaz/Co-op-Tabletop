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

        private async Task Start()
        {
            await UnityServices.InitializeAsync();

         
            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
            };

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            playerName = "TestName" + UnityEngine.Random.Range(1, 100);
        }

        public Player GetPlayer()
        {
            return new Player
            {
                //Id = AuthenticationService.Instance.PlayerId,
                Data = new Dictionary<string, PlayerDataObject>
                    {
                        { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
                    }
            };
        }
        public async Task<bool> CreateLobby()
        {
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                Debug.Log("Player is not authenticated. Cannot create lobby.");
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            
            }

            try
            {
                string lobbyName = "Lobby";
                int maxPlayers = 4;
           
                CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
                {
                    IsPrivate = false,
                    Player = GetPlayer(),
                    Data = new Dictionary<string, DataObject>
                {
                    {"GameMode", new DataObject(DataObject.VisibilityOptions.Public, "DemoGame")},
                    {"Map", new DataObject(DataObject.VisibilityOptions.Public, "DemoMap")}
                }
                };
                Unity.Services.Lobbies.Models.Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);
                //Debug.Log("Created Lobby " + lobby.Name + " " + lobby.MaxPlayers + " " + lobby.Id + " " + lobby.LobbyCode);
                hostLobby = lobby;
                joinedLobby = hostLobby;
                PrintPlayers(hostLobby);
                Debug.Log("Created Lobby " + lobby.Name + " " + lobby.MaxPlayers + " " + lobby.Id + " " + lobby.LobbyCode);
                return true;
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError($"LobbyServiceException: {e.Message} (Status Code: {e.ErrorCode})");

                // Handle specific error codes if needed
                if (e.ErrorCode == (int)LobbyExceptionReason.Unauthorized)
                {
                    Debug.LogError("Unauthorized: Ensure the player is signed in.");
                }
                else if (e.ErrorCode == (int)LobbyExceptionReason.ValidationError)
                {
                    Debug.LogError("Invalid request: Check lobby creation parameters.");
                }
                // Add additional specific error handling as necessary

                return false; // Indicate failed lobby creation
            }
        }
        private async Task ListLobby()
        {
            try
            {
                QueryLobbiesOptions queryOptions = new QueryLobbiesOptions
                {
                    Count = 25,
                    Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                },
                    Order = new List<QueryOrder>
                {
                    new QueryOrder(false,QueryOrder.FieldOptions.Created)
                }
                };

                QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
                Debug.Log("Lobbies Found: " + queryResponse.Results.Count);
                foreach (Unity.Services.Lobbies.Models.Lobby l in queryResponse.Results)
                {
                    Debug.Log(l.Name + " " + l.MaxPlayers + " " + l.Data["GameMode"].Value);
                }
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
        public async Task JoinLobby()
        {
            try
            {
                QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
                await Lobbies.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id);
                OnLobbyUpdated?.Invoke(joinedLobby.Players);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        private async Task JoinLobbyWithCode(string lobbyCode)
        {
            try
            {
                JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
                {
                    Player = GetPlayer()
                };
                Unity.Services.Lobbies.Models.Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
                joinedLobby = lobby;
                Debug.Log("Join Lobby with " + lobbyCode);
                PrintPlayers(joinedLobby);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        private async Task QuickJoinLobby()
        {
            try
            {
                await Lobbies.Instance.QuickJoinLobbyAsync();
                // Debug.Log("Join Lobby with " + lobbyCode);

            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }



        // Update is called once per frame
        private async Task Update()
        {
            await HandleLobbyHeartbeat();
            await HandleLobbyPollForUpdates();
        }

        public async Task UpdatePlayerName(string newPlayerName)
        {
            try
            {
                playerName = newPlayerName;
                await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
                {
                    Data = new Dictionary<string, PlayerDataObject> {
                { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
            }
                });
                PrintPlayers(joinedLobby);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        private async Task LeaveLobby()
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                joinedLobby = null;
                hostLobby = null;
                OnLobbyUpdated?.Invoke(joinedLobby.Players);
                Debug.Log("Left the Lobby Successfully");
            }
            catch (LobbyServiceException e)
            {
                Debug.Log("Error LEaving Lobby");
                Debug.Log(e);
        }
        }

        private async Task KickPlayer()
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, joinedLobby.Players[1].Id);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        private async Task MigrateLobbyHost()
        {
            try
            {
                hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
                {
                    HostId = joinedLobby.Players[1].Id
                });
                joinedLobby = hostLobby;

                PrintPlayers(hostLobby);
                // Debug.Log("Join Lobby with " + lobbyCode);

            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        private async Task DeleteLobby()
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
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
            Debug.Log("Players in Lobby " + lobby.Name);
            foreach (Player p in lobby.Players)
            {
                Debug.Log(p.Id + " " + p.Data["PlayerName"].Value + lobby.Data["GameMode"].Value + " " + lobby.Data["Map"].Value);
                Debug.Log($"{p.Id} {p.Data["PlayerName"]} {lobby.Data["GameMode"].Value} {lobby.Data["Map"].Value}");
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

        private async Task UpdateLobbyGameMode(string gameMode)
        {
            try
            {
                hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                {
                    {"GameMode", new DataObject(DataObject.VisibilityOptions.Public,gameMode) }
                }
                });
                joinedLobby = hostLobby;

                PrintPlayers(hostLobby);
                // Debug.Log("Join Lobby with " + lobbyCode);

            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }
