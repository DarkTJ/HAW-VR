using Photon.Pun;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionSetupController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CreatePlayer();
    }

    private void CreatePlayer()
    {
        Debug.Log("Creating Player model for " + PhotonNetwork.LocalPlayer.NickName);
        //Ist noch ein Cube, aber hier kann später das gesammte Player prefab stehen
        PhotonNetwork.Instantiate(Path.Combine("MultiplayerPrefabs", "PhotonPlayer"), new Vector3(Random.Range(0,3),Random.Range(0,3),0), Quaternion.identity);
    }
    
}
