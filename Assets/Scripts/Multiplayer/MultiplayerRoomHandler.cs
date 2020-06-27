using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles the joining and leaving of photon rooms.
/// </summary>
public class MultiplayerRoomHandler : MonoBehaviour
{
    /// <summary>
    /// Static accessible instance of the MultiplayerRoomHandler (Singleton pattern)
    /// </summary>
    public static MultiplayerRoomHandler Instance { get; private set; }
    
    private readonly RoomOptions _roomOpsOpen = new RoomOptions()
    {
        IsVisible = true, IsOpen = true, MaxPlayers = 25
    };

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
        
        // Always keep this object alive
        DontDestroyOnLoad(gameObject);
    }

    public void SwitchRoom(int index)
    {
        string roomName = "Lobby";
        switch (index)
        {
            case 1: roomName = "Mapping";
                break;
        }

        StartCoroutine(C_SwitchRoom(index, roomName, _roomOpsOpen, TypedLobby.Default));
    }

    private IEnumerator C_SwitchRoom(int index, string roomName, RoomOptions roomOptions, TypedLobby typedLobby){
        PhotonNetwork.LeaveRoom();
        yield return new WaitForSeconds(1f);
        SceneReferences.RoomController.mpSceneIndex = index;
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, typedLobby);
    }
}
