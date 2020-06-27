using System;
using Photon.Pun;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class sets up the photon session for every unity scene.
/// Does not handle the network stuff.
/// </summary>
public class MultiplayerSceneSetupController : MonoBehaviour
{
    /// <summary>
    /// Static accessible instance of the SessionSetupController (Singleton pattern)
    /// </summary>
    public static MultiplayerSceneSetupController Instance { get; private set; }
    
    private GameObject _playerCharacterObject;

    public bool IsReady { get; private set; }
    private static bool _setupAfterSceneLoad;

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
    }
    
    private void OnSceneLoad(Scene scene, LoadSceneMode loadSceneMode)
    {
        // When a scene is loaded this object is not ready
        IsReady = false;
        
        // Is true, is the player is the master client, and the current scene was just reloaded
        // See in RoomController.cs
        if (_setupAfterSceneLoad)
        {
            Setup();
            _setupAfterSceneLoad = false;
        }
    }

    public void Setup()
    {
        Debug.Log("Creating Player model for " + PhotonNetwork.LocalPlayer.NickName);

        //Ist noch ein Cube, aber hier kann später der Avatar stehen
        _playerCharacterObject = PhotonNetwork.Instantiate(Path.Combine("MultiplayerPrefabs", "PhotonPlayer"), Vector3.zero, Quaternion.identity);
        _playerCharacterObject.transform.parent = SceneReferences.PlayerObject;
        _playerCharacterObject.transform.localPosition = Vector3.zero;

        // Fast fix for player seeing himself
        // Feel free to change if this is for all other players too
        foreach (Renderer r in _playerCharacterObject.GetComponentsInChildren<Renderer>())
        {
            r.enabled = false;
        }
        
        IsReady = true;
    }

    public void SetupAfterSceneLoad()
    {
        _setupAfterSceneLoad = true;
    }
}
