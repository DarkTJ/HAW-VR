using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class joinLobby : MonoBehaviour
{   

    [SerializeField]
    Button btn_backToLobby;
     RoomOptions roomOpsLobby = new RoomOptions(){IsVisible = true, IsOpen = true, MaxPlayers = (byte)25};

     RoomController roomController;

    // Start is called before the first frame update
    void Start()
    {

         roomController = GameObject.Find("RoomController").GetComponent<RoomController>();
        btn_backToLobby.onClick.AddListener(()=>StartCoroutine(SwitchRoom(4,"Lobby",roomOpsLobby,TypedLobby.Default)));
    }

     IEnumerator SwitchRoom(int index, string roomname, RoomOptions roomOptions, TypedLobby typedLobby){
        PhotonNetwork.LeaveRoom();
        yield return new WaitForSeconds(0.5f);
        roomController.mpSceneIndex = 4;
        PhotonNetwork.JoinOrCreateRoom(roomname,roomOptions,typedLobby);

    }

}
