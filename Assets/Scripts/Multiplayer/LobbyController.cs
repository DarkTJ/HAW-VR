﻿using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyController : MonoBehaviourPunCallbacks
{

    [SerializeField]
    Button btn_joinLobby;

    

    // Start is called before the first frame update
    void Start()
    {
        btn_joinLobby.onClick.AddListener(()=> PhotonNetwork.JoinRoom("Lobby"));
    }

    public override void OnConnectedToMaster()
    {
       Debug.Log(PhotonNetwork.LocalPlayer.NickName + " is connected to master");
    }

    public override void OnJoinRoomFailed(short returnCode, string message){
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " failed to join Lobby: " + message );
        CreateRoom("Lobby");
    }

    void CreateRoom(string roomname){
       Debug.Log("Creating room " + roomname);
       RoomOptions roomOpsLobby = new RoomOptions(){IsVisible = true, IsOpen = true, MaxPlayers = (byte)25};
       PhotonNetwork.CreateRoom("Lobby",roomOpsLobby);
       Debug.Log("Room " + roomname + " was created!");
    }

}
