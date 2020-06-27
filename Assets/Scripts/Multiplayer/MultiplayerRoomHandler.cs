using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RoomHandler : MonoBehaviour
{
    [SerializeField]
    Button btn_room1;

    [SerializeField]
    Button btn_room2;

    [SerializeField]
    Button btn_room3;
    RoomController roomController;

    private RoomOptions roomOpsOpen = new RoomOptions(){IsVisible = true, IsOpen = true, MaxPlayers = (byte)25};

    void Start()
    {
        roomController = GameObject.Find("RoomController").GetComponent<RoomController>();
        btn_room1.onClick.AddListener(()=> StartCoroutine(SwitchRoom(1,"Room1",roomOpsOpen,TypedLobby.Default)));
        btn_room2.onClick.AddListener(()=> StartCoroutine(SwitchRoom(2,"Room2",roomOpsOpen,TypedLobby.Default)));
        btn_room3.onClick.AddListener(()=> StartCoroutine(SwitchRoom(3,"Room3",roomOpsOpen,TypedLobby.Default)));   
    }

    IEnumerator SwitchRoom(int index, string roomname, RoomOptions roomOptions, TypedLobby typedLobby){
        PhotonNetwork.LeaveRoom();
        yield return new WaitForSeconds(1f);
        roomController.mpSceneIndex = index;
        PhotonNetwork.JoinOrCreateRoom(roomname,roomOptions,typedLobby);
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
