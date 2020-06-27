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
       if (PhotonNetwork.IsMasterClient)
       {
           Debug.Log("Loading Room " + PhotonNetwork.CurrentRoom.Name);
           PhotonNetwork.LoadLevel(mpSceneIndex);
           
           // When the player is the master client the scene will be reloaded
           // That means that the session should be setup after this scene is reloaded
           MultiplayerSceneSetupController.Instance.SetupAfterSceneLoad();
       }
       else
       {
           // When the player is not the master client the session can be set up right away
           MultiplayerSceneSetupController.Instance.Setup();
       }
   }
}
