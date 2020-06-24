using Photon.Pun;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionSetupController : MonoBehaviour
{

    [SerializeField]
    GameObject OVRHeadset;
    // Start is called before the first frame update
    void Start()
    {   
        CreatePlayer();
    }

    private GameObject CreatePlayer()
    {
        Debug.Log("Creating Player model for " + PhotonNetwork.LocalPlayer.NickName);
        //Ist noch ein Cube, aber hier kann später der Avatar stehen
       GameObject player = PhotonNetwork.Instantiate(Path.Combine("MultiplayerPrefabs", "PhotonPlayer"), Vector3.zero, Quaternion.identity);
       //player.transform.parent = OVRHeadset.transform;
       return player;
    }
}
