﻿using System;
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
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
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

        _playerCharacterObject = PhotonNetwork.Instantiate(Path.Combine("MultiplayerPrefabs", "Avatar"), Vector3.zero, Quaternion.identity);
        SceneReferences.AvatarTransformController.SetAvatarTransform(_playerCharacterObject.transform);
        
        // Locally disable renderers
        _playerCharacterObject.GetComponent<AvatarController>().SetRenderers(false);
        _playerCharacterObject.GetComponent<AvatarController>().SetName();
        
        IsReady = true;
    }

    public void SetupAfterSceneLoad()
    {
        _setupAfterSceneLoad = true;
    }
}
