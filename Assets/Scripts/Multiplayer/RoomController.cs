using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviourPunCallbacks
{

    [SerializeField]    
   public int mpSceneIndex;

   public override void OnEnable()
   {
       PhotonNetwork.AddCallbackTarget(this);
   }

   public override void OnDisable()
   {
       PhotonNetwork.RemoveCallbackTarget(this);
   }

   public override void OnJoinedRoom(){
       Debug.Log(PhotonNetwork.LocalPlayer.NickName + " joined room " + PhotonNetwork.CurrentRoom.Name);
       StartGame();
   }

   private void StartGame(){
       if(PhotonNetwork.IsMasterClient){
           Debug.Log("Loading Room " + PhotonNetwork.CurrentRoom.Name);
            PhotonNetwork.LoadLevel(mpSceneIndex);
           
       }
   }
}
