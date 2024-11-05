using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
//using LobbyManager; // Ensure this matches the namespace in your LobbyManager script

public class PlayModeTestScript //: MonoBehaviour
{
    //private LobbyManager lobbyScript;

    //[UnitySetUp]
    //public IEnumerator SetUp()
    //{
    //    GameObject lobbyGameObject = new GameObject("LobbyManagerGameObject");
    //    lobbyScript = lobbyGameObject.AddComponent<LobbyManager>(); // Correctly attach LobbyManager
    //    yield return null; // Wait for setup to complete
    //}

    //[UnityTest]
    //public IEnumerator TestCreateLobby()
    //{
    //    //yield return lobbyScript.CreateLobby(); // Wait for the lobby creation to finish

    //    Assert.IsNotNull(lobbyScript.hostLobby, "Lobby was not created.");
    //    Assert.IsNotNull(lobbyScript.joinedLobby, "Joined lobby should be the same as host lobby.");
    //}

    //[UnityTest]
    //public IEnumerator TestJoinLobby()
    //{
    //    // Create a lobby first
    //   // yield return lobbyScript.CreateLobby();

    //    // Join the created lobby
    //    yield return lobbyScript.JoinLobby();

    //    Assert.IsNotNull(lobbyScript.joinedLobby, "Lobby was not joined.");
    //}

    //[UnityTest]
    //public IEnumerator TestUpdatePlayerName()
    //{
    //    string newPlayerName = "UpdatedName";
    //    //yield return lobbyScript.CreateLobby(); // Ensure a lobby exists before updating

    //    yield return lobbyScript.UpdatePlayerName(newPlayerName); // Update player name

    //    // Optionally, you can add a wait if necessary to allow the update to process
    //    yield return new WaitForSeconds(0.5f); // Adjust timing as necessary for your async operations

    //    Assert.AreEqual(newPlayerName, lobbyScript.playerName, "Player name was not updated.");
    //}

    //[UnityTearDown]
    //public IEnumerator TearDown()
    //{
    //    if (lobbyScript != null)
    //    {
    //        Object.Destroy(lobbyScript.gameObject);
    //    }
    //    yield return null;
    //}
}
